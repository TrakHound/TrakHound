// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using TrakHound.Apps;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Modules;
using TrakHound.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TrakHound.Volumes;

namespace TrakHound.Apps
{
    public class TrakHoundAppInjectionServiceManagerDebug : ITrakHoundAppInjectionServiceManager, IDisposable
    {
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly ITrakHoundClientProvider _clientProvider;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly Dictionary<string, TrakHoundAppInjectionServiceConfiguration> _serviceConfigurations = new Dictionary<string, TrakHoundAppInjectionServiceConfiguration>(); // TypeId => InjectionServiceInfo
        private readonly Dictionary<string, object> _services = new Dictionary<string, object>(); // ServiceId => Service (object)
        private readonly ListDictionary<string, string> _scopeTransientServices = new ListDictionary<string, string>(); // ScopeId => ServiceId
        private readonly ListDictionary<string, string> _scopeScopedServices = new ListDictionary<string, string>(); // ScopeId => ServiceId
        private readonly ListDictionary<string, string> _scopeSingletonServices = new ListDictionary<string, string>(); // ScopeId => ServiceId
        private readonly object _lock = new object();


        public IServiceProvider ServiceProvider { get; set; }


        public TrakHoundAppInjectionServiceManagerDebug(ITrakHoundClientProvider clientProvider, ITrakHoundVolumeProvider volumeProvider)
        {
            _moduleManager = new TrakHoundModuleManagerDebug<ITrakHoundAppInjectionService>();
            _clientProvider = clientProvider;
            _volumeProvider = volumeProvider;

            Load();
        }

        public void Dispose() { }

        public void DisposeScope(string scopeId)
        {
            if (!string.IsNullOrEmpty(scopeId))
            {
                IEnumerable<string> serviceKeys;
                lock (_lock)
                {
                    serviceKeys = _scopeTransientServices.Get(scopeId);
                    if (serviceKeys == null) serviceKeys = _scopeScopedServices.Get(scopeId);
                    //if (serviceKeys == null) serviceKeys = _scopeSingletonServices.Get(scopeId);
                }

                if (!serviceKeys.IsNullOrEmpty())
                {
                    foreach (var serviceKey in serviceKeys)
                    {
                        DisposeService(serviceKey);
                    }
                }
            }
        }

        private void DisposeService(string serviceKey)
        {
            if (!string.IsNullOrEmpty(serviceKey))
            {
                try
                {
                    object service;
                    lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                    if (service != null)
                    {
                        // Dispose of the service
                        if (typeof(IDisposable).IsAssignableFrom(service.GetType()))
                        {
                            ((IDisposable)service).Dispose();
                        }

                        // Remove from services list
                        lock (_lock) _services.Remove(serviceKey);
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }


        private void Load()
        {
            var domainAssemblies = Assemblies.Load();
            //var domainAssemblies = Assemblies.Get();
            //var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (domainAssemblies != null)
            {
                foreach (var domainAssembly in domainAssemblies)
                {
                    var types = domainAssembly.GetTypes().Where(o => typeof(ITrakHoundAppInjectionService).IsAssignableFrom(o) && !o.IsAbstract && !o.IsInterface);
                    if (!types.IsNullOrEmpty())
                    {
                        foreach (var type in types)
                        {
                            var key = CreateTypeKey(type);
                            var configuration = new TrakHoundAppInjectionServiceConfiguration(null, _clientProvider.GetClient(), _volumeProvider.GetVolume("volume"), type);

                            lock (_lock)
                            {
                                if (!_serviceConfigurations.ContainsKey(key))
                                {
                                    _serviceConfigurations.Add(key, configuration);
                                }
                            }
                        }
                    }
                }
            }
        }

        //private void Load()
        //{
        //    var types = Assemblies.GetTypes<ITrakHoundAppInjectionService>();
        //    if (!types.IsNullOrEmpty())
        //    {
        //        foreach (var type in types)
        //        {
        //            var key = CreateTypeKey(type);
        //            var configuration = new TrakHoundAppInjectionServiceConfiguration(null, null, type);

        //            lock (_lock)
        //            {
        //                if (!_serviceConfigurations.ContainsKey(key))
        //                {
        //                    _serviceConfigurations.Add(key, configuration);
        //                }
        //            }
        //        }
        //    }
        //}


        public object GetService(string scopeId, Type serviceType)
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceType != null)
            {
                var serviceKey = CreateServiceKey(scopeId, serviceType);

                object service;
                lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                if (service == null)
                {
                    var typeKey = CreateTypeKey(serviceType);

                    TrakHoundAppInjectionServiceConfiguration serviceConfiguration;
                    lock (_lock) serviceConfiguration = _serviceConfigurations.GetValueOrDefault(typeKey);

                    if (serviceConfiguration != null)
                    {
                        service = CreateService(scopeId, serviceConfiguration);

                        if (service != null)
                        {
                            Console.WriteLine($"GetService() : scopeId = {scopeId} : type = {serviceType.Name}");

                            RegisterService(scopeId, service);
                        }
                    }
                }

                return service;
            }

            return null;
        }

        public object GetService<TServiceType>(string scopeId, Type serviceType) where TServiceType : ITrakHoundAppInjectionService
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceType != null)
            {
                var serviceKey = CreateServiceKey(scopeId, serviceType);

                object service;
                lock (_lock) service = _services.GetValueOrDefault(serviceKey);

                if (service == null)
                {
                    var typeKey = CreateTypeKey(serviceType);

                    TrakHoundAppInjectionServiceConfiguration serviceConfiguration;
                    lock (_lock) serviceConfiguration = _serviceConfigurations.GetValueOrDefault(typeKey);

                    if (serviceConfiguration != null)
                    {
                        service = CreateService(scopeId, serviceConfiguration);

                        if (service != null)
                        {
                            Console.WriteLine($"GetService() : scopeId = {scopeId} : type = {serviceType.Name}");

                            RegisterService<TServiceType>(scopeId, service);
                        }
                    }
                }

                return service;
            }

            return null;
        }

        private object CreateService(string scopeId, TrakHoundAppInjectionServiceConfiguration serviceConfiguration)
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceConfiguration != null)
            {
                try
                {
                    var serviceProvider = new TrakHoundServiceProviderDebug(scopeId, this);

                    // Add App Configuration
                    serviceProvider.AddService<ITrakHoundAppConfiguration>(serviceConfiguration.Configuration);

                    // Add client to allow injection of ITrakHoundClient for the specified RouterId set in the AppConfiguration
                    serviceProvider.AddService<ITrakHoundClient>(serviceConfiguration.Client);

                    var service = ActivatorUtilities.CreateInstance(serviceProvider, serviceConfiguration.ServiceType);
                    serviceProvider.AddService(service);
                    return service;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }


        public IEnumerable<TrakHoundAppInjectionServiceConfiguration> GetServiceConfigurations<TServiceType>() where TServiceType : ITrakHoundAppInjectionService
        {
            var result = new List<TrakHoundAppInjectionServiceConfiguration>();

            IEnumerable<TrakHoundAppInjectionServiceConfiguration> allServices;
            lock (_lock) allServices = _serviceConfigurations.Values;

            if (!allServices.IsNullOrEmpty())
            {
                foreach (var service in allServices)
                {
                    if (typeof(TServiceType).IsAssignableFrom(service.ServiceType))
                    {
                        result.Add(service);
                    }
                }
            }

            return result;
        }

        public IEnumerable<TrakHoundAppInjectionServiceConfiguration> GetRequiredServiceConfigurations<TServiceType>() where TServiceType : ITrakHoundAppInjectionService
        {
            var result = new List<TrakHoundAppInjectionServiceConfiguration>();

            if (typeof(ITrakHoundTransientAppInjectionService).IsAssignableFrom(typeof(TServiceType)))
            {
                result.AddRange(GetServiceConfigurations<ITrakHoundTransientAppInjectionService>());
                result.AddRange(GetServiceConfigurations<ITrakHoundScopedAppInjectionService>());
                result.AddRange(GetServiceConfigurations<ITrakHoundSingletonAppInjectionService>());
            }
            else if (typeof(ITrakHoundScopedAppInjectionService).IsAssignableFrom(typeof(TServiceType)))
            {
                result.AddRange(GetServiceConfigurations<ITrakHoundScopedAppInjectionService>());
                result.AddRange(GetServiceConfigurations<ITrakHoundSingletonAppInjectionService>());
            }
            else if (typeof(ITrakHoundSingletonAppInjectionService).IsAssignableFrom(typeof(TServiceType)))
            {
                result.AddRange(GetServiceConfigurations<ITrakHoundSingletonAppInjectionService>());
            }

            return result;
        }


        public void RegisterService(string scopeId, object service)
        {
            if (!string.IsNullOrEmpty(scopeId) && service != null)
            {
                var serviceKey = CreateServiceKey(scopeId, service.GetType());

                lock (_lock)
                {
                    if (!_services.ContainsKey(serviceKey)) _services.Add(serviceKey, service);
                }

                // Add to list of scope services

                if (typeof(ITrakHoundTransientAppInjectionService).IsAssignableFrom(service.GetType())) _scopeTransientServices.Add(scopeId, serviceKey);
                if (typeof(ITrakHoundScopedAppInjectionService).IsAssignableFrom(service.GetType())) _scopeScopedServices.Add(scopeId, serviceKey);
                if (typeof(ITrakHoundSingletonAppInjectionService).IsAssignableFrom(service.GetType())) _scopeSingletonServices.Add(scopeId, serviceKey);
            }
        }

        public void RegisterService<TServiceType>(string scopeId, object service) where TServiceType : ITrakHoundAppInjectionService
        {
            if (!string.IsNullOrEmpty(scopeId) && service != null)
            {
                var serviceKey = CreateServiceKey(scopeId, service.GetType());

                lock (_lock)
                {
                    if (!_services.ContainsKey(serviceKey)) _services.Add(serviceKey, service);
                }

                // Add to list of scope services

                if (typeof(ITrakHoundTransientAppInjectionService).IsAssignableFrom(service.GetType())) _scopeTransientServices.Add(scopeId, serviceKey);
                if (typeof(ITrakHoundScopedAppInjectionService).IsAssignableFrom(service.GetType())) _scopeScopedServices.Add(scopeId, serviceKey);
                if (typeof(ITrakHoundSingletonAppInjectionService).IsAssignableFrom(service.GetType())) _scopeSingletonServices.Add(scopeId, serviceKey);
            }
        }


        private static string CreateServiceKey(string scopeId, Type serviceType)
        {
            if (!string.IsNullOrEmpty(scopeId) && serviceType != null)
            {
                return $"{scopeId}:{CreateTypeKey(serviceType)}".ToMD5Hash();
            }

            return null;
        }

        private static string CreateTypeKey(Type serviceType)
        {
            if (serviceType != null && serviceType.Assembly != null)
            {
                return $"{serviceType.Assembly.Location}:{serviceType.FullName}".ToMD5Hash();
            }

            return null;
        }
    }
}
