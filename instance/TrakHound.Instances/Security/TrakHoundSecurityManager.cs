// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Configurations;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Volumes;

namespace TrakHound.Security
{
    public sealed class TrakHoundSecurityManager : ITrakHoundSecurityManager
    {
        private const string _packageCategory = "identity";

        private readonly TrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly TrakHoundPackageManager _packageManager;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly Dictionary<string, ITrakHoundIdentityProvider> _identityProviders = new Dictionary<string, ITrakHoundIdentityProvider>(); // ProviderId => Provider
        private readonly Dictionary<string, TrakHoundSecurityProfile> _profiles = new Dictionary<string, TrakHoundSecurityProfile>(); // ProfileId => Profile
        private readonly ListDictionary<string, string> _profileAssignments = new ListDictionary<string, string>(); // ProfileId => AssignmentId[]
        private readonly Dictionary<string, TrakHoundIdentityAssignment> _assignments = new Dictionary<string, TrakHoundIdentityAssignment>(); // AssignmentId => Assignment
        private readonly Dictionary<string, string> _assignmentProfiles = new Dictionary<string, string>(); // AssignmentId => ProfileId
        private readonly Dictionary<string, TrakHoundIdentityResource> _resources = new Dictionary<string, TrakHoundIdentityResource>(); // ResourceId => Resource
        private readonly ListDictionary<string, string> _resourceProfiles = new ListDictionary<string, string>(); // ResourceId => ProfileId[]
        private readonly ListDictionary<string, string> _resourcePermissions = new ListDictionary<string, string>();
        private readonly Dictionary<string, TrakHoundAuthenticationRequest> _requests = new Dictionary<string, TrakHoundAuthenticationRequest>();
        private readonly Dictionary<string, string> _sessionProviders = new Dictionary<string, string>(); // SessionId => ProviderId
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(2000);
        private readonly DelayEvent _delaySaveEvent = new DelayEvent(1000);
        private readonly object _lock = new object();


        private readonly Dictionary<string, string> _installedConfigurations = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private readonly Dictionary<string, string> _installedModuleHashes = new Dictionary<string, string>(); // Configuration.Id => Module.Hash


        public IEnumerable<ITrakHoundIdentityProvider> IdentityProviders => _identityProviders.Values;

        public event EventHandler<ITrakHoundIdentityProvider> IdentityProviderAdded;

        public event EventHandler<string> IdentityProviderRemoved;


        public TrakHoundSecurityManager(
            TrakHoundConfigurationProfile configurationProfile,
            ITrakHoundModuleProvider moduleProvider,
            ITrakHoundVolumeProvider volumeProvider,
            TrakHoundPackageManager packageManager
            )
        {
            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ProviderConfigurationAdded;
            _configurationProfile.ConfigurationRemoved += ProviderConfigurationRemoved;

            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ITrakHoundIdentityProvider>(_packageCategory);

            _volumeProvider = volumeProvider;

            _delayLoadEvent.Elapsed += LoadDelayElapsed;
            _delaySaveEvent.Elapsed += SaveDelayElapsed;
        }


        private void PackageAdded(object sender, TrakHoundPackage package)
        {
            if (package != null && package.Category == _packageCategory)
            {
                _delayLoadEvent.Set();
            }
        }

        private void PackageRemoved(object sender, TrakHoundPackageChangedArgs args)
        {
            if (args.PackageCategory == _packageCategory)
            {
                _delayLoadEvent.Set();
            }
        }


        private void ProviderConfigurationAdded(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration.Category == TrakHoundIdentityProviderConfiguration.ConfigurationCategory)
            {
                AddProviderConfiguration((ITrakHoundIdentityProviderConfiguration)configuration);
            }
        }

        private void ProviderConfigurationRemoved(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration.Category == TrakHoundIdentityProviderConfiguration.ConfigurationCategory)
            {
                RemoveProviderConfiguration(configuration.Id);
            }
        }


        public async Task<TrakHoundAuthenticationResponse> Authenticate(TrakHoundAuthenticationRequest authenticationRequest)
        {
            string message = null;

            if (!string.IsNullOrEmpty(authenticationRequest.ResourceId))
            {
                // Add Request to Cache (used for Callback)
                lock (_lock) _requests.Add(authenticationRequest.Id, authenticationRequest);

                // Get Profile
                var profiles = GetProfilesByResourceId(authenticationRequest.ResourceId);
                if (!profiles.IsNullOrEmpty())
                {
                    TrakHoundAuthenticationResponse response;

                    foreach (var profile in profiles.OrderByDescending(o => o.Priority))
                    {
                        if (!string.IsNullOrEmpty(profile.ProviderId))
                        {
                            // Get Profile Assignment IDs
                            var assignmentIds = _profileAssignments.Get(profile.Id);
                            if (!assignmentIds.IsNullOrEmpty())
                            {
                                // Get Assignments
                                var assignments = new List<TrakHoundIdentityAssignment>();
                                foreach (var assignmentId in assignmentIds)
                                {
                                    var assignment = _assignments.GetValueOrDefault(assignmentId);
                                    if (assignment != null && IsAssignmentMatch(assignment.ResourceId, authenticationRequest.ResourceId))
                                    {
                                        assignments.Add(assignment);
                                    }
                                }

                                // Get Identity Provider
                                var provider = GetProvider(profile.ProviderId);
                                if (provider != null)
                                {
                                    // Authenticate with the Identity Provider and get a Authentication Response
                                    response = await provider.Authenticate(this, authenticationRequest);
                                    if (response.Session != null && response.Session.SessionId != null)
                                    {
                                        lock (_lock)
                                        {
                                            _sessionProviders.Remove(response.Session.SessionId);
                                            _sessionProviders.Add(response.Session.SessionId, provider.Id);
                                        }

                                        // Handle an Authenticated Session
                                        var authenticatedSession = response.Session as ITrakHoundAuthenticatedSession;
                                        if (authenticatedSession != null)
                                        {
                                            var mappedRoles = new List<string>();

                                            // Match Roles to Assignment Scopes
                                            if (!authenticatedSession.Roles.IsNullOrEmpty())
                                            {
                                                foreach (var role in authenticatedSession.Roles)
                                                {
                                                    foreach (var assignment in assignments)
                                                    {
                                                        if (!assignment.Roles.IsNullOrEmpty() && !assignment.Permissions.IsNullOrEmpty())
                                                        {
                                                            foreach (var assignmentRole in assignment.Roles)
                                                            {
                                                                // Probably need to use a better "Match" pattern. Regex?

                                                                if (assignmentRole == "*" || role.StartsWith(assignmentRole.TrimEnd('*')))
                                                                {
                                                                    mappedRoles.AddRange(assignment.Permissions);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                if (!mappedRoles.IsNullOrEmpty())
                                                {
                                                    var mappedSession = new TrakHoundAuthenticatedSession();
                                                    mappedSession.SessionId = authenticatedSession.SessionId;
                                                    mappedSession.ProviderId = provider.Id;
                                                    mappedSession.User = authenticatedSession.User;
                                                    mappedSession.Roles = mappedRoles;
                                                    mappedSession.ValidFrom = authenticatedSession.ValidFrom;
                                                    mappedSession.ValidTo = authenticatedSession.ValidTo;
                                                    mappedSession.Parameters = authenticatedSession.Parameters;

                                                    response = new TrakHoundAuthenticationResponse(
                                                        response.RequestId,
                                                        response.ProviderId,
                                                        response.ResourceId,
                                                        response.Action,
                                                        mappedSession
                                                        );
                                                }
                                                else
                                                {
                                                    response = TrakHoundAuthenticationResponse.Fail(response.RequestId, response.ProviderId, response.ResourceId);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("NO MAPPED ROLES ??");
                                            }
                                        }

                                        // Handle an Annonymous Session
                                        var annonymousSession = response.Session as ITrakHoundAnnonymousSession;
                                        if (annonymousSession != null)
                                        {
                                            var mappedRoles = new List<string>();

                                            // Match Roles to Assignment Scopes
                                            if (!annonymousSession.Roles.IsNullOrEmpty())
                                            {
                                                foreach (var role in annonymousSession.Roles)
                                                {
                                                    foreach (var assignment in assignments)
                                                    {
                                                        if (!assignment.Roles.IsNullOrEmpty() && !assignment.Permissions.IsNullOrEmpty())
                                                        {
                                                            foreach (var assignmentRole in assignment.Roles)
                                                            {
                                                                if (role.StartsWith(assignmentRole.TrimEnd('*')))
                                                                {
                                                                    mappedRoles.AddRange(assignment.Permissions);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            var mappedSession = new TrakHoundAnnonymousSession();
                                            mappedSession.SessionId = annonymousSession.SessionId;
                                            mappedSession.ProviderId = provider.Id;
                                            mappedSession.Roles = mappedRoles;
                                            mappedSession.Parameters = annonymousSession.Parameters;

                                            response = new TrakHoundAuthenticationResponse(
                                                response.RequestId,
                                                response.ProviderId,
                                                response.ResourceId,
                                                response.Action,
                                                mappedSession
                                                );
                                        }
                                    }


                                    // Should be a "Success" or "Fail" ??
                                    if (response.Session != null ||
                                        (response.Action != null && (response.Action.Type == TrakHoundIdentityActionType.Redirect || response.Action.Type == TrakHoundIdentityActionType.Fail)))
                                    {
                                        return response;
                                    }
                                }
                                else
                                {
                                    message = $"Identity Provider Not Found : {profile.ProviderId}";
                                }
                            }
                            else
                            {
                                message = $"Identity Profile Not Found : {profile.Id}";
                            }
                        }
                        else
                        {
                            var successAction = new TrakHoundIdentitySuccessAction();
                            successAction.Message = $"No Identity Provider specified for the Resource : {authenticationRequest.ResourceId}";

                            return new TrakHoundAuthenticationResponse(authenticationRequest.Id, null, authenticationRequest.ResourceId, successAction, null);
                        }
                    }
                }
                else
                {
                    var session = new TrakHoundAnnonymousSession();
                    session.SessionId = Guid.NewGuid().ToString();
                    session.Roles = new string[] { "*" }; // Matches ANY role
                    session.Parameters = authenticationRequest.Parameters;
                    message = $"No Identity Profile Found for the Resource : {authenticationRequest.ResourceId}";
                    return TrakHoundAuthenticationResponse.Success(authenticationRequest.Id, null, authenticationRequest.ResourceId, session, message);
                }
            }
            else
            {
                message = $"No Resource Specified : {authenticationRequest.ResourceId}";
            }

            var errorAction = new TrakHoundIdentityErrorAction();
            errorAction.Message = message;

            return new TrakHoundAuthenticationResponse(authenticationRequest.Id, null, authenticationRequest.ResourceId, errorAction, null);
        }

        public async Task<TrakHoundAuthenticationSessionCloseResponse> Revoke(TrakHoundAuthenticationSessionCloseRequest request)
        {
            if (!string.IsNullOrEmpty(request.SessionId))
            {
                string providerId;
                lock (_lock) providerId = _sessionProviders.GetValueOrDefault(request.SessionId);
                if (providerId != null)
                {
                    var provider = GetProvider(providerId);
                    if (provider != null)
                    {
                        return await provider.Revoke(this, request);
                    }
                    else
                    {

                    }
                }
            }

            return new TrakHoundAuthenticationSessionCloseResponse();
        }

        public async Task<TrakHoundAuthenticationCallbackResponse> Callback(string providerId, TrakHoundAuthenticationCallbackRequest request)
        {
            var provider = GetProvider(providerId);
            if (provider != null)
            {
                return await provider.HandleCallback(this, request);
            }
            else
            {

            }

            return new TrakHoundAuthenticationCallbackResponse();
        }


        public TrakHoundIdentityResource GetResource(string resourceId)
        {
            if (!string.IsNullOrEmpty(resourceId))
            {
                lock (_lock)
                {
                    return _resources.GetValueOrDefault(resourceId);
                }
            }

            return null;
        }

        public void AddResource(TrakHoundIdentityResourceType resourceType, string resourceId)
        {
            var resource = new TrakHoundIdentityResource(resourceType, resourceId);

            AddResourceInternal(resource);
        }

        public void AddResource(TrakHoundIdentityResourceType resourceType, string resourceId, string permission)
        {
            IEnumerable<string> permissions = null;
            if (!string.IsNullOrEmpty(permission)) permissions = new string[] { permission };

            var resource = new TrakHoundIdentityResource(resourceType, resourceId, permissions);

            AddResourceInternal(resource);
        }

        public void AddResource(TrakHoundIdentityResourceType resourceType, string resourceId, IEnumerable<string> permissions)
        {
            var resource = new TrakHoundIdentityResource(resourceType, resourceId, permissions);

            AddResourceInternal(resource);
        }

        public void AddResourceInternal(TrakHoundIdentityResource resource)
        {
            if (resource != null && !string.IsNullOrEmpty(resource.Id))
            {
                lock (_lock)
                {
                    _resources.Remove(resource.Id);
                    _resources.Add(resource.Id, resource);
                }

                _resourcePermissions.Remove(resource.Id);
                if (!resource.Permissions.IsNullOrEmpty())
                {
                    foreach (var permission in resource.Permissions)
                    {
                        _resourcePermissions.Add(resource.Id, permission);
                    }
                }
            }
        }

        public void RemoveResource(string resourceId)
        {
            if (!string.IsNullOrEmpty(resourceId))
            {
                lock (_lock)
                {
                    _resources.Remove(resourceId);
                    _resourcePermissions.Remove(resourceId);
                }
            }
        }


        public IEnumerable<TrakHoundSecurityProfile> GetProfiles()
        {
            lock (_lock)
            {
                return _profiles.Values;
            }
        }

        public TrakHoundSecurityProfile GetProfile(string profileId)
        {
            if (!string.IsNullOrEmpty(profileId))
            {
                lock (_lock)
                {
                    return _profiles.GetValueOrDefault(profileId);
                }
            }

            return null;
        }

        public IEnumerable<TrakHoundSecurityProfile> GetProfilesByResourceId(string resourceId)
        {
            if (!string.IsNullOrEmpty(resourceId))
            {
                lock (_lock)
                {
                    var matchedProfileIds = new HashSet<string>();
                    foreach (var resourceKey in _resourceProfiles.Keys)
                    {
                        if (resourceKey.EndsWith('*'))
                        {
                            var resourcePattern = resourceKey.TrimEnd('*');
                            if (resourceId.StartsWith(resourcePattern))
                            {
                                var profileIds = _resourceProfiles.Get(resourceKey);
                                if (!profileIds.IsNullOrEmpty())
                                {
                                    foreach (var profileId in profileIds) matchedProfileIds.Add(profileId);
                                }
                            }
                        }
                        else
                        {
                            var profileIds = _resourceProfiles.Get(resourceId);
                            if (!profileIds.IsNullOrEmpty())
                            {
                                foreach (var profileId in profileIds) matchedProfileIds.Add(profileId);
                            }
                        }
                    }

                    var profiles = new List<TrakHoundSecurityProfile>();
                    foreach (var profileId in matchedProfileIds)
                    {
                        var profile = _profiles.GetValueOrDefault(profileId);
                        if (profile != null) profiles.Add(profile);
                    }
                    return profiles;
                }
            }

            return null;
        }

        public void AddProfile(string profileId, string providerId, string description = null)
        {
            var profile = new TrakHoundSecurityProfile();
            profile.Id = profileId;
            profile.Description = description;
            profile.ProviderId = providerId;

            AddProfileInternal(profile);
            _delaySaveEvent.Set();
        }

        public void AddProfile(TrakHoundSecurityProfile profile)
        {
            AddProfileInternal(profile);
            _delaySaveEvent.Set();
        }

        private void AddProfileInternal(TrakHoundSecurityProfile profile)
        {
            if (profile != null && !string.IsNullOrEmpty(profile.Id))
            {
                lock (_lock)
                {
                    _profiles.Remove(profile.Id);
                    _profiles.Add(profile.Id, profile);
                }

                if (!profile.Assignments.IsNullOrEmpty())
                {
                    foreach (var assignment in profile.Assignments)
                    {
                        AddAssignmentInternal(profile.Id, assignment);
                    }
                }
            }
        }

        public void RemoveProfile(string profileId)
        {
            if (!string.IsNullOrEmpty(profileId))
            {
                try
                {
                    var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "identity");
                    path = System.IO.Path.Combine(path, $"{profileId}.config.yaml");

                    if (File.Exists(path))
                    {
                        File.Delete(path);

                        lock (_lock) _profiles.Remove(profileId);
                    }
                    else
                    {
                        lock (_lock) _profiles.Remove(profileId);
                    }
                }
                catch { }
            }
        }


        private static bool IsAssignmentMatch(string pattern, string resourceId)
        {
            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(resourceId))
            {
                if (pattern == "*")
                {
                    return true;
                }
                else if (pattern.EndsWith('*'))
                {
                    var matchPattern = pattern.TrimEnd('*');
                    return resourceId.StartsWith(matchPattern);
                }
                else
                {
                    return resourceId == pattern;
                }
            }

            return false;
        }

        public void AddAssignment(string profileId, TrakHoundIdentityAssignment assignment)
        {
            AddAssignmentInternal(profileId, assignment);
            _delaySaveEvent.Set();
        }

        public void AddAssignmentInternal(string profileId, TrakHoundIdentityAssignment assignment)
        {
            if (!string.IsNullOrEmpty(profileId) && assignment != null && !string.IsNullOrEmpty(assignment.Id) && !string.IsNullOrEmpty(assignment.ResourceId))
            {
                lock (_lock)
                {
                    var profile = _profiles.GetValueOrDefault(profileId);
                    if (profile != null)
                    {
                        var profileAssignments = profile.Assignments?.ToList();
                        if (profileAssignments == null) profileAssignments = new List<TrakHoundIdentityAssignment>();
                        profileAssignments.RemoveAll(o => o.Id == assignment.Id);
                        profileAssignments.Add(assignment);
                        profile.Assignments = profileAssignments;

                        _assignments.Remove(assignment.Id);
                        _assignments.Add(assignment.Id, assignment);

                        _profileAssignments.Add(profile.Id, assignment.Id);

                        _assignmentProfiles.Remove(assignment.Id);
                        _assignmentProfiles.Add(assignment.Id, profile.Id);

                        _resourceProfiles.Add(assignment.ResourceId, profile.Id);
                    }
                }
            }
        }

        public void RemoveAssignment(string assignmentId)
        {
            if (!string.IsNullOrEmpty(assignmentId))
            {
                lock (_lock)
                {
                    var profileId = _assignmentProfiles.GetValueOrDefault(assignmentId);
                    if (profileId != null)
                    {
                        _profileAssignments.Remove(profileId, assignmentId);

                        var profile = _profiles.GetValueOrDefault(profileId);
                        if (profile != null)
                        {
                            var profileAssignments = profile.Assignments?.ToList();
                            if (profileAssignments == null) profileAssignments = new List<TrakHoundIdentityAssignment>();
                            profileAssignments.RemoveAll(o => o.Id == assignmentId);
                            profile.Assignments = profileAssignments;
                        }
                    }

                    var assignment = _assignments.GetValueOrDefault(assignmentId);
                    if (assignment != null)
                    {
                        _resourceProfiles.Remove(assignment.ResourceId, profileId);
                    }

                    _assignmentProfiles.Remove(assignmentId);
                    _assignments.Remove(assignmentId);
                }

                _delaySaveEvent.Set();
            }
        }


        public IEnumerable<ITrakHoundIdentityProvider> GetProviders()
        {
            lock (_lock)
            {
                return _identityProviders.Values;
            }
        }

        public ITrakHoundIdentityProvider GetProvider(string providerId)
        {
            if (!string.IsNullOrEmpty(providerId))
            {
                lock (_lock)
                {
                    return _identityProviders.GetValueOrDefault(providerId);
                }
            }

            return null;
        }

        public void AddProviderConfiguration(ITrakHoundIdentityProviderConfiguration configuration)
        {
            if (configuration != null)
            {
                var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                if (module != null)
                {
                    LoadProviderConfiguration(module, configuration);
                }
            }
        }

        public void RemoveProviderConfiguration(string configurationId)
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                lock (_lock) _identityProviders.Remove(configurationId.ToLower());

                if (IdentityProviderRemoved != null) IdentityProviderRemoved.Invoke(this, configurationId);
            }
        }


        private void LoadDelayElapsed(object sender, EventArgs args)
        {
            Load();
        }

        public void Load()
        {
            LoadProviders();
            LoadProfiles();
        }

        private void LoadProviders()
        {
            var configurations = _configurationProfile.Get<TrakHoundIdentityProviderConfiguration>(TrakHoundIdentityProviderConfiguration.ConfigurationCategory);
            if (!configurations.IsNullOrEmpty())
            {
                foreach (var configuration in configurations.ToList())
                {
                    if (!string.IsNullOrEmpty(configuration.Id))
                    {
                        var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                        if (module != null)
                        {
                            // Get the Installed Hash (to check if the configuration has changed)
                            var installedHash = _installedConfigurations.GetValueOrDefault(configuration.Id);
                            var installedModuleHash = _installedModuleHashes.GetValueOrDefault(configuration.Id);

                            if (configuration.Hash != installedHash || module.Package.Hash != installedModuleHash)
                            {
                                _installedConfigurations.Remove(configuration.Id);
                                _installedModuleHashes.Remove(configuration.Id);

                                LoadProviderConfiguration(module, configuration);

                                // Add to installed List
                                _installedConfigurations.Add(configuration.Id, configuration.Hash);
                                _installedModuleHashes.Add(configuration.Id, module.Package.Hash);
                            }
                        }
                    }
                }
            }
        }

        private void LoadProfiles()
        {
            var profilesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "identity");
            if (Directory.Exists(profilesDir))
            {
                var profilePaths = Directory.GetFiles(profilesDir);
                if (!profilePaths.IsNullOrEmpty())
                {
                    foreach (var profilePath in profilePaths)
                    {
                        var profile = TrakHoundSecurityProfile.Read(profilePath);
                        if (profile != null)
                        {
                            AddProfileInternal(profile);
                        }
                    }
                }
            }
        }

        public void LoadProvider(ITrakHoundIdentityProvider identityProvider)
        {
            if (identityProvider != null && !string.IsNullOrEmpty(identityProvider.Id))
            {
                lock (_lock)
                {
                    _identityProviders.Remove(identityProvider.Id);
                    _identityProviders.Add(identityProvider.Id, identityProvider);
                }
            }
        }

        private void LoadProviderConfiguration(ITrakHoundModule module, ITrakHoundIdentityProviderConfiguration configuration)
        {
            var volumeId = configuration.VolumeId ?? configuration.Id;
            var volume = _volumeProvider.GetVolume(volumeId);

            var provider = CreateProviderInstance(module, configuration, volume);
            if (provider != null && !string.IsNullOrEmpty(provider.Id))
            {
                lock (_lock)
                {
                    _identityProviders.Remove(provider.Id);
                    _identityProviders.Add(provider.Id, provider);
                }

                if (IdentityProviderAdded != null) IdentityProviderAdded.Invoke(this, provider);
            }
        }

        private ITrakHoundIdentityProvider CreateProviderInstance(ITrakHoundModule module, ITrakHoundIdentityProviderConfiguration configuration, ITrakHoundVolume volume)
        {
            if (module != null && module.ModuleTypes != null && configuration != null)
            {
                try
                {
                    var constructor = module.ModuleTypes?.FirstOrDefault().GetConstructor(new Type[] { typeof(ITrakHoundIdentityProviderConfiguration), typeof(ITrakHoundVolume) });
                    if (constructor != null)
                    {
                        return (ITrakHoundIdentityProvider)constructor.Invoke(new object[] { configuration, volume });
                    }
                }
                catch { }
            }

            return null;
        }


        private async void SaveDelayElapsed(object sender, EventArgs args)
        {
            await Save();
        }

        public async Task Save()
        {
            IEnumerable<TrakHoundSecurityProfile> profiles;
            lock (_lock) profiles = _profiles.Values.ToList();
            if (!profiles.IsNullOrEmpty())
            {
                foreach (var profile in profiles)
                {
                    await profile.Save();
                }
            }
        }


        public TrakHoundAuthenticationRequest GetAuthenticationRequest(string requestId)
        {
            if (!string.IsNullOrEmpty(requestId))
            {
                lock (_lock)
                {
                    return _requests.GetValueOrDefault(requestId);
                }
            }

            return null;
        }
    }
}
