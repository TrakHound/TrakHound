// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Instances;
using TrakHound.Logging;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Requests;
using TrakHound.Services;
using TrakHound.Volumes;

namespace TrakHound.Api
{
    public class TrakHoundApiProvider : ITrakHoundApiProvider
    {
        private const string _packagesDirectory = "_packages";
        private const string _packageCategory = "api";

        private const string _routeParameterRegexPattern = @"(\{(.*?)\})";
        private static readonly Regex _routeParameterRegex = new Regex(_routeParameterRegexPattern, RegexOptions.Compiled);
        private static readonly ApiRouteComparer _routeComparer = new ApiRouteComparer();

        private readonly ITrakHoundInstance _instance;
        private readonly ITrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundClientProvider _clientProvider;
        private readonly ITrakHoundVolumeProvider _volumeProvider;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly TrakHoundPackageManager _packageManager;
        private readonly string _baseUrl;

        private ApiRouteConfiguration[] _routes;
        private readonly CacheDictionary<string, bool?> _routeCache = new CacheDictionary<string, bool?>();
        private readonly Dictionary<string, IOrderedEnumerable<MethodRoute>> _methodRoutes = new Dictionary<string, IOrderedEnumerable<MethodRoute>>();

        private readonly Dictionary<string, ApiPackageConfiguration> _packages = new Dictionary<string, ApiPackageConfiguration>(); // PackageHash + RouterId => ApiPackageConfiguration
        private readonly Dictionary<string, string> _installedConfigurationHashes = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private readonly Dictionary<string, string> _installedModuleHashes = new Dictionary<string, string>(); // Configuration.Id => Module.Hash
        private readonly ListDictionary<string, ITrakHoundConsumer<TrakHoundApiResponse>> _consumers = new ListDictionary<string, ITrakHoundConsumer<TrakHoundApiResponse>>(); // ApiId => Consumers
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(2000);
        private readonly object _lock = new object();


        public event EventHandler<TrakHoundApiInformation> ApiAdded;

        public event EventHandler<string> ApiRemoved;

        public event EventHandler<Exception> ApiLoadError;

        public event TrakHoundApiLogHandler ApiLogUpdated;


        public TrakHoundApiProvider(
            ITrakHoundInstance instance,
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundModuleProvider moduleProvider,
            ITrakHoundClientProvider clientProvider,
            ITrakHoundVolumeProvider volumeProvider,
            TrakHoundPackageManager packageManager
            )
        {
            _instance = instance;
            _clientProvider = clientProvider;
            _volumeProvider = volumeProvider;

            // Set Base URL (HTTP Address)
            if (_instance != null)
            {
                _baseUrl = _instance.Information.GetInterface("HTTP")?.GetBaseUrl();

                _instance.SecurityManager.AddResource(Security.TrakHoundIdentityResourceType.System, "System.Api.Information");
            }

            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ConfigurationUpdated;
            _configurationProfile.ConfigurationRemoved += ConfigurationUpdated;

            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ITrakHoundApiController>(_packageCategory);

            _delayLoadEvent.Elapsed += LoadDelayElapsed;
        }

        public void Dispose()
        {
            _delayLoadEvent.Elapsed -= LoadDelayElapsed;
            _delayLoadEvent.Dispose();

            lock (_lock)
            {
                var consumers = _consumers.Values;
                if (!consumers.IsNullOrEmpty())
                {
                    foreach (var consumer in consumers) consumer.Dispose();
                }

                _routes = null;
                _routeCache.Dispose();
                _methodRoutes.Clear();
                _packages.Clear();
                _installedConfigurationHashes.Clear();
                _installedModuleHashes.Clear();
            }
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


        private void ConfigurationUpdated(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration != null && configuration.Category == TrakHoundApiConfiguration.ConfigurationCategory)
            {
                _delayLoadEvent.Set();
            }
        }


        public bool IsRouteValid(string route)
        {
            if (route != null)
            {
                var fRoute = route?.TrimStart('/');

                var controllers = GetControllersByRoute(fRoute);
                return controllers != null;
            }

            return false;
        }

        public TrakHoundApiEndpointInformation GetEndpointInformation(string endpointType, string route)
        {
            var apiInformations = GetRouteInformation();
            if (!apiInformations.IsNullOrEmpty())
            {
                var fMatchRoute = route?.TrimStart('/');

                foreach (var apiInformation in apiInformations)
                {
                    if (!apiInformation.Controllers.IsNullOrEmpty())
                    {
                        foreach (var controllerInformation in apiInformation.Controllers)
                        {
                            if (!controllerInformation.EndPoints.IsNullOrEmpty())
                            {
                                foreach (var endpointInformation in controllerInformation.EndPoints)
                                {
                                    if (endpointInformation.Type == endpointType)
                                    {
                                        var endpointRoute = Url.Combine(apiInformation.Route, controllerInformation.Route, endpointInformation.Route);
                                        var fEndpointRoute = endpointRoute?.TrimStart('/');

                                        if (IsRouteMatch(fMatchRoute, fEndpointRoute))
                                        {
                                            endpointInformation.Api = apiInformation;
                                            endpointInformation.Controller = controllerInformation;
                                            return endpointInformation;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }


        #region "Information"

        public IEnumerable<TrakHoundApiInformation> GetRouteInformation()
        {
            var informations = new List<TrakHoundApiInformation>();

            lock (_lock)
            {
                if (!_routes.IsNullOrEmpty())
                {
                    foreach (var route in _routes)
                    {
                        var information = TrakHoundApiInformation.Create(route.Controllers, route.Configuration, route.Package, route.PackageReadMe);
                        if (information != null)
                        {
                            informations.Add(information);
                        }
                    }
                }
            }

            return informations;
        }

        public IEnumerable<TrakHoundApiInformation> GetPackageInformation()
        {
            var informations = new List<TrakHoundApiInformation>();

            //IEnumerable<ITrakHoundModule> matchedModules;
            //lock (_lock) matchedModules = _moduleManager.Modules?.ToList();
            //if (!matchedModules.IsNullOrEmpty())
            //{
            //    foreach (var module in matchedModules)
            //    {
            //        var apiConfiguration = CreateDefaultConfiguration(module);

            //        var information = TrakHoundApiInformation.Create(apiConfiguration, module.ModuleType, module.Package, module.PackageReadMe);
            //        if (information != null)
            //        {
            //            informations.Add(information);
            //        }
            //    }
            //}

            return informations;
        }

        //public IEnumerable<TrakHoundApiInformation> GetPackageInformation()
        //{
        //    var informations = new List<TrakHoundApiInformation>();

        //    IEnumerable<ITrakHoundModule> matchedModules;
        //    lock (_lock) matchedModules = _moduleManager.Modules?.ToList();
        //    if (!matchedModules.IsNullOrEmpty())
        //    {
        //        foreach (var module in matchedModules)
        //        {
        //            var apiConfiguration = CreateDefaultConfiguration(module);

        //            var information = TrakHoundApiInformation.Create(apiConfiguration, module.ModuleType, module.Package, module.PackageReadMe);
        //            if (information != null)
        //            {
        //                informations.Add(information);
        //            }
        //        }
        //    }

        //    return informations;
        //}

        public TrakHoundApiInformation GetInformation(string apiId)
        {
            TrakHoundApiInformation information = null;

            if (!_routes.IsNullOrEmpty())
            {
                ApiRouteConfiguration route;
                lock (_lock) route = _routes.FirstOrDefault(o => o.Configuration.Id == apiId);
                if (!route.Controllers.IsNullOrEmpty())
                {
                    information = TrakHoundApiInformation.Create(route.Controllers, route.Configuration, route.Package, route.PackageReadMe);
                }
            }

            //if (info == null)
            //{
            //    IEnumerable<ITrakHoundModule> matchedModules;
            //    lock (_lock) matchedModules = _moduleManager.Modules?.ToList();
            //    if (!matchedModules.IsNullOrEmpty())
            //    {
            //        foreach (var module in matchedModules)
            //        {
            //            var apiConfiguration = CreateDefaultConfiguration(module);
            //            if (apiConfiguration.Id == apiId)
            //            {
            //                info = TrakHoundApiInformation.Create(apiConfiguration, module.ModuleType, module.Package, module.PackageReadMe);
            //            }
            //        }
            //    }
            //}

            return information;
        }

        #endregion

        #region "Endpoints"

        public async Task<TrakHoundApiResponse> QueryByRoute(
            string route,
            Stream requestBody,
            string requestContentType = "application/octet-stream",
            Dictionary<string, string> queryParameters = null
            )
        {
            if (!string.IsNullOrEmpty(route))
            {
                var controllers = GetControllersByRoute(route);
                if (!controllers.IsNullOrEmpty())
                {
                    var configuration = controllers.FirstOrDefault()?.Configuration;
                    var requestedRoute = route.ToLower();
                    var apiRoute = configuration?.Route?.ToLower();

                    var path = Url.RemoveBase(requestedRoute, apiRoute); // Remove Api Configuration Route prefix

                    return await Process<TrakHoundApiQueryAttribute>(controllers, path, queryParameters, requestBody, requestContentType);
                }
            }

            return new TrakHoundApiResponse();
        }

        public async Task<TrakHoundApiResponse> QueryByPackage(
            string packageId,
            string packageVersion,
            string path,
            Stream requestBody,
            string requestContentType = "application/octet-stream",
            Dictionary<string, string> queryParameters = null,
            string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                var controllers = GetControllersByPackage(packageId, packageVersion);
                if (!controllers.IsNullOrEmpty())
                {
                    return await Process<TrakHoundApiQueryAttribute>(controllers, path, queryParameters, requestBody, requestContentType);
                }
            }

            return new TrakHoundApiResponse();
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeByRoute(
            string route,
            Stream requestBody,
            string requestContentType = "application/octet-stream",
            Dictionary<string, string> queryParameters = null
            )
        {
            if (!string.IsNullOrEmpty(route))
            {
                var controllers = GetControllersByRoute(route);
                if (!controllers.IsNullOrEmpty())
                {
                    var configuration = controllers.FirstOrDefault()?.Configuration;
                    var requestedRoute = route.ToLower();
                    var apiRoute = configuration?.Route?.ToLower();

                    var path = Url.RemoveBase(requestedRoute, apiRoute); // Remove Api Configuration Route prefix

                    var consumer = await ProcessSubscribe<TrakHoundApiSubscribeAttribute>(controllers, path, queryParameters, requestBody);
                    if (consumer != null)
                    {
                        _consumers.Add(configuration.Id, consumer);

                        return consumer;
                    }
                }
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeByPackage(
            string packageId,
            string packageVersion,
            string path,
            Stream requestBody,
            string requestContentType = "application/octet-stream",
            Dictionary<string, string> queryParameters = null,
            string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                var controllers = GetControllersByPackage(packageId, packageVersion);
                if (!controllers.IsNullOrEmpty())
                {
                    var consumer = await ProcessSubscribe<TrakHoundApiSubscribeAttribute>(controllers, path, queryParameters, requestBody);
                    if (consumer != null)
                    {
                        var key = $"{packageId}:{packageVersion}";
                        _consumers.Add(key, consumer);

                        return consumer;
                    }
                }
            }

            return null;
        }

        public async Task<TrakHoundApiResponse> PublishByRoute(string route, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            if (!string.IsNullOrEmpty(route))
            {
                var controllers = GetControllersByRoute(route);
                if (!controllers.IsNullOrEmpty())
                {
                    var configuration = controllers.FirstOrDefault()?.Configuration;
                    var requestedRoute = route.ToLower();
                    var apiRoute = configuration?.Route?.ToLower();

                    var path = Url.RemoveBase(requestedRoute, apiRoute); // Remove Api Configuration Route prefix

                    return await Process<TrakHoundApiPublishAttribute>(controllers, path, queryParameters, requestBody, requestContentType);
                }
            }

            return new TrakHoundApiResponse();
        }

        public async Task<TrakHoundApiResponse> PublishByPackage(string packageId, string packageVersion, string path, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null, string routerId = null)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                var controllers = GetControllersByPackage(packageId, packageVersion);
                if (!controllers.IsNullOrEmpty())
                {
                    return await Process<TrakHoundApiPublishAttribute>(controllers, path, queryParameters, requestBody, requestContentType);
                }
            }

            return new TrakHoundApiResponse();
        }

        public async Task<TrakHoundApiResponse> Delete(string route, Stream requestBody, string requestContentType = "application/octet-stream", Dictionary<string, string> queryParameters = null)
        {
            if (!string.IsNullOrEmpty(route))
            {
                var controllers = GetControllersByRoute(route);
                if (!controllers.IsNullOrEmpty())
                {
                    var configuration = controllers.FirstOrDefault()?.Configuration;
                    var requestedRoute = route.ToLower();
                    var apiRoute = configuration?.Route?.ToLower();

                    var path = Url.RemoveBase(requestedRoute, apiRoute); // Remove Api Configuration Route prefix

                    return await Process<TrakHoundApiDeleteAttribute>(controllers, path, queryParameters, requestBody, requestContentType);
                }
            }

            return new TrakHoundApiResponse();
        }

        #endregion

        #region "Process"

        class MethodRoute
        {
            public string Route { get; set; }

            public ITrakHoundApiController Controller { get; set; }

            public MethodInfo Method { get; set; }

            public override string ToString()
            {
                return $"{Route} => {Method?.Name}";
            }
        }

        class MethodParameters
        {
            public Dictionary<string, MethodRouteParameter> RouteParameters { get; set; } = new();
            public Dictionary<string, MethodQueryParameter> QueryParameters { get; set; } = new();
            public MethodBodyParameter BodyParameter { get; set; }
            public int Count { get; set; }
        }

        class MethodRouteParameter
        {
            public string Name { get; set; }
            public int Index { get; set; }
            public ParameterInfo Parameter { get; set; }
            public FromRouteAttribute Attribute { get; set; }
        }

        class MethodQueryParameter
        {
            public string Name { get; set; }
            public int Index { get; set; }
            public ParameterInfo Parameter { get; set; }
            public FromQueryAttribute Attribute { get; set; }
        }

        class MethodBodyParameter
        {
            public string Name { get; set; }
            public int Index { get; set; }
            public ParameterInfo Parameter { get; set; }
            public FromBodyAttribute Attribute { get; set; }
        }

        private readonly Dictionary<string, MethodParameters> _cachedParameters = new Dictionary<string, MethodParameters>();

        private MethodParameters GetMethodParameters(MethodInfo methodInfo)
        {
            MethodParameters methodParameters;

            var cacheKey = GetMethodId(methodInfo);

            lock (_lock) methodParameters = _cachedParameters.GetValueOrDefault(cacheKey);
            if (methodParameters == null)
            {
                methodParameters = new MethodParameters();

                var parameters = methodInfo.GetParameters();
                if (!parameters.IsNullOrEmpty())
                {
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var methodParameter = parameters[i];
                        string methodParameterName = methodParameter.Name;

                        // Route Parameters
                        var routeAttribute = methodParameter.GetCustomAttribute<FromRouteAttribute>();
                        if (routeAttribute != null)
                        {
                            var routeParameter = new MethodRouteParameter();
                            routeParameter.Name = methodParameterName;
                            routeParameter.Index = i;
                            routeParameter.Parameter = methodParameter;
                            routeParameter.Attribute = routeAttribute;
                            methodParameters.RouteParameters.Add(routeParameter.Name, routeParameter);
                            methodParameters.Count++;
                        }

                        // Query Parameters
                        var queryAttribute = methodParameter.GetCustomAttribute<FromQueryAttribute>();
                        if (queryAttribute != null)
                        {
                            var queryParameter = new MethodQueryParameter();
                            queryParameter.Name = methodParameterName;
                            queryParameter.Index = i;
                            queryParameter.Parameter = methodParameter;
                            queryParameter.Attribute = queryAttribute;
                            methodParameters.QueryParameters.Add(queryParameter.Name, queryParameter);
                            methodParameters.Count++;
                        }

                        // Body Parameters
                        var bodyAttribute = methodParameter.GetCustomAttribute<FromBodyAttribute>();
                        if (bodyAttribute != null)
                        {
                            var bodyParameter = new MethodBodyParameter();
                            bodyParameter.Name = methodParameterName;
                            bodyParameter.Index = i;
                            bodyParameter.Parameter = methodParameter;
                            bodyParameter.Attribute = bodyAttribute;
                            methodParameters.BodyParameter = bodyParameter;
                            methodParameters.Count++;
                        }
                    }
                }

                lock (_lock)
                {
                    _cachedParameters.Remove(cacheKey);
                    _cachedParameters.Add(cacheKey, methodParameters);
                }
            }

            return methodParameters;
        }



        private async Task<TrakHoundApiResponse> Process<TAttribute>(
            IEnumerable<ITrakHoundApiController> controllers,
            string url,
            Dictionary<string, string> queryParameters,
            Stream requestBody,
            string requestContentType = "application/octet-stream"
            )
            where TAttribute : TrakHoundApiEndpointRouteAttribute
        {
            if (!controllers.IsNullOrEmpty())
            {
                var routes = new List<MethodRoute>();

                foreach (var controller in controllers)
                {
                    var controllerType = controller.GetType();
                    var controllerRouteKey = $"{controllerType.AssemblyQualifiedName}:{typeof(TAttribute).AssemblyQualifiedName}";

                    IOrderedEnumerable<MethodRoute> cachedRoutes;
                    lock (_lock) cachedRoutes = _methodRoutes.GetValueOrDefault(controllerRouteKey);
                    if (cachedRoutes.IsNullOrEmpty())
                    {
                        var controllerRoute = Url.PathSeparator.ToString();
                        var controllerRoutes = new List<MethodRoute>();

                        var controllerAttribute = controllerType.GetCustomAttribute<TrakHoundApiControllerAttribute>();
                        if (controllerAttribute != null)
                        {
                            controllerRoute = controllerAttribute.Route;
                        }

                        var methods = controllerType.GetMethods();
                        if (!methods.IsNullOrEmpty())
                        {
                            foreach (var method in methods)
                            {
                                if (method.ReturnType != null)
                                {
                                    if (!method.ReturnType.GenericTypeArguments.IsNullOrEmpty())
                                    {
                                        var responseType = method.ReturnType.GenericTypeArguments[0];
                                        if (typeof(TrakHoundApiResponse).IsAssignableFrom(responseType))
                                        {
                                            var attribute = method.GetCustomAttribute<TAttribute>();
                                            if (attribute != null)
                                            {
                                                var route = new MethodRoute();
                                                route.Controller = controller;
                                                route.Route = Url.Combine(controllerRoute, attribute.Route);
                                                route.Method = method;
                                                controllerRoutes.Add(route);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        var oRoutes = controllerRoutes.OrderByDescending(o => o.Route);
                        routes.AddRange(oRoutes);
                        lock (_lock)
                        {
                            _methodRoutes.Remove(controllerRouteKey);
                            _methodRoutes.Add(controllerRouteKey, oRoutes);
                        }
                    }
                    else
                    {
                        routes.AddRange(cachedRoutes);
                    }
                }

                if (!routes.IsNullOrEmpty())
                {
                    var oRoutes = routes.OrderBy(o => o.Route, _routeComparer);
                    foreach (var route in oRoutes)
                    {
                        var fUrl = url?.TrimStart('/');

                        if (IsRouteMatch(fUrl, route.Route))
                        {
                            var requestId = Guid.NewGuid().ToString();
                            var requestedRoute = !string.IsNullOrEmpty(route.Route) ? route.Route : "/";

                            var controller = (TrakHoundApiController)route.Controller;
                            controller.Logger.LogDebug($"Request Received : {requestId} : Route = '{requestedRoute}'");

                            if (!queryParameters.IsNullOrEmpty())
                            {
                                foreach (var queryParameter in queryParameters)
                                {
                                    controller.Logger.LogDebug($"Request Received : {requestId} : Query Parameter : Name = '{queryParameter.Key}' : Value = {queryParameter.Value}");
                                }
                            }

                            var stpw = System.Diagnostics.Stopwatch.StartNew();
                            var response = await ProcessRoute(route.Controller, route.Method, route.Route, url, queryParameters, requestBody, requestContentType);

                            stpw.Stop();
                            var responseMilliseconds = ((double)stpw.ElapsedTicks) / 10000;
                            controller.Logger.LogDebug($"Request Response : {requestId} : Status Code = {response.StatusCode} : Duration = {Math.Round(responseMilliseconds, 4)}ms");

                            return response;
                        }
                    }
                }
            }

            return new TrakHoundApiResponse();
        }

        private async Task<ITrakHoundConsumer<TrakHoundApiResponse>> ProcessSubscribe<TAttribute>(
            IEnumerable<ITrakHoundApiController> controllers,
            string url,
            Dictionary<string, string> queryParameters,
            Stream requestBody
            )
            where TAttribute : TrakHoundApiEndpointRouteAttribute
        {
            if (!controllers.IsNullOrEmpty())
            {
                foreach (var controller in controllers)
                {
                    var controllerType = controller.GetType();

                    var controllerRoute = Url.PathSeparator.ToString();

                    var controllerAttribute = controllerType.GetCustomAttribute<TrakHoundApiControllerAttribute>();
                    if (controllerAttribute != null)
                    {
                        controllerRoute = controllerAttribute.Route;
                    }

                    var methods = controllerType.GetMethods();
                    if (!methods.IsNullOrEmpty())
                    {
                        foreach (var method in methods)
                        {
                            if (method.ReturnType != null)
                            {
                                if (!method.ReturnType.GenericTypeArguments.IsNullOrEmpty())
                                {
                                    var responseType = method.ReturnType.GenericTypeArguments[0];
                                    if (typeof(ITrakHoundConsumer<TrakHoundApiResponse>).IsAssignableFrom(responseType))
                                    {
                                        var attribute = method.GetCustomAttribute<TAttribute>();
                                        if (attribute != null)
                                        {
                                            var route = Url.Combine(controllerRoute, attribute.Route);
                                            var task = ProcessSubscribeRoute(controller, method, route, url, queryParameters, requestBody);
                                            if (task != null)
                                            {
                                                try
                                                {
                                                    return await task;
                                                }
                                                catch (Exception ex)
                                                {
                                                    return null;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private async Task<TrakHoundApiResponse> ProcessRoute(ITrakHoundApiController controller, MethodInfo method, string route, string url, Dictionary<string, string> queryParameters, Stream requestBody, string requestContentType = "application/octet-stream")
        {
            // Extract the Route Parameters from the URL
            var routeParameters = GetRouteParameters(url, route);

            object[] inputParameters = null;

            var methodParameters = GetMethodParameters(method);
            if (methodParameters != null && methodParameters.Count > 0)
            {
                inputParameters = new object[methodParameters.Count];

                // Route Parameters
                foreach (var parameterEntry in methodParameters.RouteParameters)
                {
                    var parameterName = parameterEntry.Key;
                    var parameterIndex = parameterEntry.Value.Index;
                    var parameter = parameterEntry.Value.Parameter;

                    object inputParameterValue = null;
                    bool set = false;

                    var routeParameter = routeParameters?.GetValueOrDefault(parameterEntry.Key);
                    if (routeParameter != null)
                    {
                        inputParameterValue = ChangeType(routeParameter.Value, parameterEntry.Value.Parameter.ParameterType);
                        set = true;
                    }

                    if (set)
                    {
                        inputParameters[parameterIndex] = inputParameterValue;
                    }
                    else
                    {
                        if (parameter.HasDefaultValue)
                        {
                            inputParameters[parameterIndex] = parameter.DefaultValue;
                        }
                    }
                }

                // Query Parameters
                foreach (var parameterEntry in methodParameters.QueryParameters)
                {
                    var parameterName = parameterEntry.Key;
                    var parameterIndex = parameterEntry.Value.Index;
                    var parameter = parameterEntry.Value.Parameter;

                    object inputParameterValue = null;
                    bool set = false;

                    var queryParameter = queryParameters?.GetValueOrDefault(parameterEntry.Key);
                    if (queryParameter != null)
                    {
                        inputParameterValue = ChangeType(queryParameter, parameterEntry.Value.Parameter.ParameterType);
                        set = true;
                    }

                    if (set)
                    {
                        inputParameters[parameterIndex] = inputParameterValue;
                    }
                    else
                    {
                        if (parameter.HasDefaultValue)
                        {
                            inputParameters[parameterIndex] = parameter.DefaultValue;
                        }
                    }
                }

                // Body Parameter
                if (methodParameters.BodyParameter != null)
                {
                    var parameterIndex = methodParameters.BodyParameter.Index;
                    var parameter = methodParameters.BodyParameter.Parameter;
                    var parameterAttribute = methodParameters.BodyParameter.Attribute;

                    object inputParameterValue = null;
                    bool set = false;

                    var contentType = parameterAttribute.ContentType;
                    if (string.IsNullOrEmpty(contentType)) contentType = FromBodyAttribute.GetDefaultContentType(parameter.ParameterType);

                    if (requestBody != null)
                    {
                        switch (contentType)
                        {
                            case "application/json":

                                try
                                {
                                    inputParameterValue = JsonSerializer.Deserialize(requestBody, parameter.ParameterType);
                                    set = true;
                                }
                                catch { }
                                break;

                            case "text/plain":

                                try
                                {
                                    byte[] requestBodyBytes = null;
                                    using (var readStream = new MemoryStream())
                                    {
                                        requestBody.CopyTo(readStream);
                                        readStream.Seek(0, SeekOrigin.Begin);
                                        requestBodyBytes = readStream.ToArray();
                                    }
                                    inputParameterValue = Convert.ChangeType(System.Text.Encoding.UTF8.GetString(requestBodyBytes), parameter.ParameterType);
                                    set = true;
                                }
                                catch { }
                                break;

                            case "application/octet-stream":

                                // Set Parameter as byte[]
                                if (parameter.ParameterType == typeof(byte[]))
                                {
                                    byte[] requestBodyBytes = null;
                                    using (var readStream = new MemoryStream())
                                    {
                                        requestBody.CopyTo(readStream);
                                        if (readStream.CanSeek) readStream.Seek(0, SeekOrigin.Begin);
                                        requestBodyBytes = readStream.ToArray();
                                    }
                                    inputParameterValue = requestBodyBytes;
                                    set = true;
                                }

                                // Set Parameter as Stream
                                if (parameter.ParameterType == typeof(Stream))
                                {
                                    if (requestBody.CanSeek) requestBody.Seek(0, SeekOrigin.Begin);

                                    inputParameterValue = requestBody;
                                    set = true;
                                }

                                break;
                        }
                    }

                    if (set)
                    {
                        inputParameters[parameterIndex] = inputParameterValue;
                    }
                    else
                    {
                        if (parameter.HasDefaultValue)
                        {
                            inputParameters[parameterIndex] = parameter.DefaultValue;
                        }
                    }
                }
            }

            try
            {
                return await (Task<TrakHoundApiResponse>)method.Invoke(controller, inputParameters);
            }
            catch (Exception ex)
            {
                var errorResponse = new TrakHoundApiResponse();
                errorResponse.StatusCode = 500;
                errorResponse.ContentType = "text/plain";
                errorResponse.Content = new MemoryStream(StringFunctions.ToUtf8Bytes(ex.Message));
                return errorResponse;
            }
        }


        //private Task<TrakHoundApiResponse> ProcessRoute(ITrakHoundApiController controller, MethodInfo method, string route, string url, Dictionary<string, string> queryParameters, Stream requestBody, string requestContentType = "application/octet-stream")
        //{
        //    var fUrl = url?.TrimStart('/');

        //    if (IsRouteMatch(fUrl, route))
        //    {
        //        // Extract the Route Parameters from the URL
        //        var routeParameters = GetRouteParameters(fUrl, route);

        //        object[] inputParameters = null;

        //        var methodParameters = GetMethodParameters(method);
        //        if (methodParameters != null && methodParameters.Count > 0)
        //        {
        //            inputParameters = new object[methodParameters.Count];

        //            // Route Parameters
        //            foreach (var parameterEntry in methodParameters.RouteParameters)
        //            {
        //                var parameterName = parameterEntry.Key;
        //                var parameterIndex = parameterEntry.Value.Index;
        //                var parameter = parameterEntry.Value.Parameter;

        //                object inputParameterValue = null;
        //                bool set = false;

        //                var routeParameter = routeParameters?.GetValueOrDefault(parameterEntry.Key);
        //                if (routeParameter != null)
        //                {
        //                    inputParameterValue = ChangeType(routeParameter.Value, parameterEntry.Value.Parameter.ParameterType);
        //                    set = true;
        //                }

        //                if (set)
        //                {
        //                    inputParameters[parameterIndex] = inputParameterValue;
        //                }
        //                else
        //                {
        //                    if (parameter.HasDefaultValue)
        //                    {
        //                        inputParameters[parameterIndex] = parameter.DefaultValue;
        //                    }
        //                }
        //            }

        //            // Query Parameters
        //            foreach (var parameterEntry in methodParameters.QueryParameters)
        //            {
        //                var parameterName = parameterEntry.Key;
        //                var parameterIndex = parameterEntry.Value.Index;
        //                var parameter = parameterEntry.Value.Parameter;

        //                object inputParameterValue = null;
        //                bool set = false;

        //                var queryParameter = queryParameters?.GetValueOrDefault(parameterEntry.Key);
        //                if (queryParameter != null)
        //                {
        //                    inputParameterValue = ChangeType(queryParameter, parameterEntry.Value.Parameter.ParameterType);
        //                    set = true;
        //                }

        //                if (set)
        //                {
        //                    inputParameters[parameterIndex] = inputParameterValue;
        //                }
        //                else
        //                {
        //                    if (parameter.HasDefaultValue)
        //                    {
        //                        inputParameters[parameterIndex] = parameter.DefaultValue;
        //                    }
        //                }
        //            }

        //            // Body Parameter
        //            if (methodParameters.BodyParameter != null)
        //            {
        //                var parameterIndex = methodParameters.BodyParameter.Index;
        //                var parameter = methodParameters.BodyParameter.Parameter;
        //                var parameterAttribute = methodParameters.BodyParameter.Attribute;

        //                object inputParameterValue = null;
        //                bool set = false;

        //                var contentType = parameterAttribute.ContentType;
        //                if (string.IsNullOrEmpty(contentType)) contentType = FromBodyAttribute.GetDefaultContentType(parameter.ParameterType);

        //                if (requestBody != null)
        //                {
        //                    switch (contentType)
        //                    {
        //                        case "application/json":

        //                            try
        //                            {
        //                                inputParameterValue = JsonSerializer.Deserialize(requestBody, parameter.ParameterType);
        //                                set = true;
        //                            }
        //                            catch { }
        //                            break;

        //                        case "text/plain":

        //                            try
        //                            {
        //                                byte[] requestBodyBytes = null;
        //                                using (var readStream = new MemoryStream())
        //                                {
        //                                    requestBody.CopyTo(readStream);
        //                                    readStream.Seek(0, SeekOrigin.Begin);
        //                                    requestBodyBytes = readStream.ToArray();
        //                                }
        //                                inputParameterValue = Convert.ChangeType(System.Text.Encoding.UTF8.GetString(requestBodyBytes), parameter.ParameterType);
        //                                set = true;
        //                            }
        //                            catch { }
        //                            break;

        //                        case "application/octet-stream":

        //                            // Set Parameter as byte[]
        //                            if (parameter.ParameterType == typeof(byte[]))
        //                            {
        //                                byte[] requestBodyBytes = null;
        //                                using (var readStream = new MemoryStream())
        //                                {
        //                                    requestBody.CopyTo(readStream);
        //                                    if (readStream.CanSeek) readStream.Seek(0, SeekOrigin.Begin);
        //                                    requestBodyBytes = readStream.ToArray();
        //                                }
        //                                inputParameterValue = requestBodyBytes;
        //                                set = true;
        //                            }

        //                            // Set Parameter as Stream
        //                            if (parameter.ParameterType == typeof(Stream))
        //                            {
        //                                if (requestBody.CanSeek) requestBody.Seek(0, SeekOrigin.Begin);

        //                                inputParameterValue = requestBody;
        //                                set = true;
        //                            }

        //                            break;
        //                    }
        //                }

        //                if (set)
        //                {
        //                    inputParameters[parameterIndex] = inputParameterValue;
        //                }
        //                else
        //                {
        //                    if (parameter.HasDefaultValue)
        //                    {
        //                        inputParameters[parameterIndex] = parameter.DefaultValue;
        //                    }
        //                }
        //            }
        //        }

        //        try
        //        {
        //            return (Task<TrakHoundApiResponse>)method.Invoke(controller, inputParameters);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }

        //    return null;
        //}

        private Task<ITrakHoundConsumer<TrakHoundApiResponse>> ProcessSubscribeRoute(ITrakHoundApiController controller, MethodInfo method, string route, string url, Dictionary<string, string> queryParameters, Stream requestBody, string requestContentType = "application/octet-stream")
        {
            var fUrl = url?.TrimStart('/');

            if (IsRouteMatch(fUrl, route))
            {
                // Extract the Route Parameters from the URL
                var routeParameters = GetRouteParameters(fUrl, route);

                object[] inputParameters = null;

                var methodParameters = GetMethodParameters(method);
                if (methodParameters != null && methodParameters.Count > 0)
                {
                    inputParameters = new object[methodParameters.Count];

                    // Route Parameters
                    foreach (var parameterEntry in methodParameters.RouteParameters)
                    {
                        var parameterName = parameterEntry.Key;
                        var parameterIndex = parameterEntry.Value.Index;
                        var parameter = parameterEntry.Value.Parameter;

                        object inputParameterValue = null;
                        bool set = false;

                        var routeParameter = routeParameters?.GetValueOrDefault(parameterEntry.Key);
                        if (routeParameter != null)
                        {
                            inputParameterValue = ChangeType(routeParameter.Value, parameterEntry.Value.Parameter.ParameterType);
                            set = true;
                        }

                        if (set)
                        {
                            inputParameters[parameterIndex] = inputParameterValue;
                        }
                        else
                        {
                            if (parameter.HasDefaultValue)
                            {
                                inputParameters[parameterIndex] = parameter.DefaultValue;
                            }
                        }
                    }

                    // Query Parameters
                    foreach (var parameterEntry in methodParameters.QueryParameters)
                    {
                        var parameterName = parameterEntry.Key;
                        var parameterIndex = parameterEntry.Value.Index;
                        var parameter = parameterEntry.Value.Parameter;

                        object inputParameterValue = null;
                        bool set = false;

                        var queryParameter = queryParameters?.GetValueOrDefault(parameterEntry.Key);
                        if (queryParameter != null)
                        {
                            inputParameterValue = ChangeType(queryParameter, parameterEntry.Value.Parameter.ParameterType);
                            set = true;
                        }

                        if (set)
                        {
                            inputParameters[parameterIndex] = inputParameterValue;
                        }
                        else
                        {
                            if (parameter.HasDefaultValue)
                            {
                                inputParameters[parameterIndex] = parameter.DefaultValue;
                            }
                        }
                    }

                    // Body Parameter
                    if (methodParameters.BodyParameter != null)
                    {
                        var parameterIndex = methodParameters.BodyParameter.Index;
                        var parameter = methodParameters.BodyParameter.Parameter;
                        var parameterAttribute = methodParameters.BodyParameter.Attribute;

                        object inputParameterValue = null;
                        bool set = false;

                        var contentType = parameterAttribute.ContentType;
                        if (string.IsNullOrEmpty(contentType)) contentType = FromBodyAttribute.GetDefaultContentType(parameter.ParameterType);

                        if (requestBody != null)
                        {
                            switch (contentType)
                            {
                                case "application/json":

                                    try
                                    {
                                        inputParameterValue = JsonSerializer.Deserialize(requestBody, parameter.ParameterType);
                                        set = true;
                                    }
                                    catch { }
                                    break;

                                case "text/plain":

                                    try
                                    {
                                        byte[] requestBodyBytes = null;
                                        using (var readStream = new MemoryStream())
                                        {
                                            requestBody.CopyTo(readStream);
                                            readStream.Seek(0, SeekOrigin.Begin);
                                            requestBodyBytes = readStream.ToArray();
                                        }
                                        inputParameterValue = Convert.ChangeType(System.Text.Encoding.UTF8.GetString(requestBodyBytes), parameter.ParameterType);
                                        set = true;
                                    }
                                    catch { }
                                    break;

                                case "application/octet-stream":

                                    // Set Parameter as byte[]
                                    if (parameter.ParameterType == typeof(byte[]))
                                    {
                                        byte[] requestBodyBytes = null;
                                        using (var readStream = new MemoryStream())
                                        {
                                            requestBody.CopyTo(readStream);
                                            readStream.Seek(0, SeekOrigin.Begin);
                                            requestBodyBytes = readStream.ToArray();
                                        }
                                        inputParameterValue = requestBodyBytes;
                                        set = true;
                                    }

                                    // Set Parameter as Stream
                                    if (parameter.ParameterType == typeof(Stream))
                                    {
                                        inputParameterValue = requestBody;
                                        set = true;
                                    }

                                    break;
                            }
                        }

                        if (set)
                        {
                            inputParameters[parameterIndex] = inputParameterValue;
                        }
                        else
                        {
                            if (parameter.HasDefaultValue)
                            {
                                inputParameters[parameterIndex] = parameter.DefaultValue;
                            }
                        }
                    }
                }

                try
                {
                    return (Task<ITrakHoundConsumer<TrakHoundApiResponse>>)method.Invoke(controller, inputParameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }


        private IEnumerable<ITrakHoundApiController> GetControllersByRoute(string route)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(route) && !_routes.IsNullOrEmpty())
                {
                    var compareRoute = route.ToLower();

                    foreach (var routeConfig in _routes)
                    {
                        if (!string.IsNullOrEmpty(routeConfig.Route))
                        {
                            if (compareRoute.StartsWith(routeConfig.Route.ToLower()))
                            {
                                return routeConfig.Controllers;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerable<ITrakHoundApiController> GetControllersByPackage(string packageId, string packageVersion = null, string routerId = null)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(packageId) && !_routes.IsNullOrEmpty())
                {
                    var module = _moduleManager.Get(packageId, packageVersion);
                    if (module != null)
                    {
                        var client = _clientProvider.GetClient();
                        if (client != null)
                        {
                            client.AddMiddleware(new TrakHoundSourceMiddleware(module));

                            var configuration = new TrakHoundApiConfiguration();

                            var controllers = new List<ITrakHoundApiController>();
                            foreach (var moduleType in module.ModuleTypes)
                            {
                                var controller = CreateInstance(module, moduleType);
                                if (controller != null)
                                {
                                    controllers.Add(controller);
                                }
                            }

                            return controllers;
                        }
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodRoute> GetControllerMethodsByRoute<TAttribute>(string route) where TAttribute : TrakHoundApiEndpointRouteAttribute
        {
            var controllers = GetControllersByRoute(route);
            if (!controllers.IsNullOrEmpty())
            {
                var methodRoutes = new List<MethodRoute>();

                foreach (var controller in controllers)
                {
                    var controllerType = controller.GetType();
                    var controllerRouteKey = $"{controllerType.AssemblyQualifiedName}:{typeof(TAttribute).AssemblyQualifiedName}";

                    IOrderedEnumerable<MethodRoute> cachedRoutes;
                    lock (_lock) cachedRoutes = _methodRoutes.GetValueOrDefault(controllerRouteKey);
                    if (cachedRoutes.IsNullOrEmpty())
                    {
                        var controllerRoute = Url.PathSeparator.ToString();
                        var controllerRoutes = new List<MethodRoute>();

                        var controllerAttribute = controllerType.GetCustomAttribute<TrakHoundApiControllerAttribute>();
                        if (controllerAttribute != null)
                        {
                            controllerRoute = controllerAttribute.Route;
                        }

                        var methods = controllerType.GetMethods();
                        if (!methods.IsNullOrEmpty())
                        {
                            foreach (var method in methods)
                            {
                                if (method.ReturnType != null)
                                {
                                    if (!method.ReturnType.GenericTypeArguments.IsNullOrEmpty())
                                    {
                                        var responseType = method.ReturnType.GenericTypeArguments[0];
                                        if (typeof(TrakHoundApiResponse).IsAssignableFrom(responseType))
                                        {
                                            var attribute = method.GetCustomAttribute<TAttribute>();
                                            if (attribute != null)
                                            {
                                                var methodRoute = new MethodRoute();
                                                methodRoute.Controller = controller;
                                                methodRoute.Route = Url.Combine(controllerRoute, attribute.Route);
                                                methodRoute.Method = method;
                                                controllerRoutes.Add(methodRoute);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        var oRoutes = controllerRoutes.OrderByDescending(o => o.Route);
                        methodRoutes.AddRange(oRoutes);
                        lock (_lock)
                        {
                            _methodRoutes.Remove(controllerRouteKey);
                            _methodRoutes.Add(controllerRouteKey, oRoutes);
                        }
                    }
                    else
                    {
                        methodRoutes.AddRange(cachedRoutes);
                    }
                }

                if (!methodRoutes.IsNullOrEmpty())
                {
                    return methodRoutes;
                }
            }

            return null;
        }

        //private IEnumerable<TrakHoundApiControllerInformation> GetControllerInformationByRoute(string route)
        //{
        //    lock (_lock)
        //    {
        //        if (!string.IsNullOrEmpty(route) && !_routes.IsNullOrEmpty())
        //        {
        //            var compareRoute = route.ToLower();

        //            foreach (var routeConfig in _routes)
        //            {
        //                if (!string.IsNullOrEmpty(routeConfig.Route))
        //                {
        //                    if (compareRoute.StartsWith(routeConfig.Route.ToLower()))
        //                    {
        //                        return routeConfig.Controllers;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}


        private bool IsRouteMatch(string url, string route)
        {
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(route)) return true;

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(route))
            {
                var match = false;

                var cacheKey = $"{url}:{route}";
                var cachedMatch = _routeCache.Get(cacheKey);
                if (cachedMatch != null) match = cachedMatch.Value;
                if (cachedMatch == null)
                {
                    var routeRegex = CreateRouteRegexPattern(route);
                    var fUrl = Url.RemoveQueryParameters(url);

                    var urlRegex = new Regex(routeRegex);
                    if (urlRegex.IsMatch(fUrl))
                    {
                        var urlParts = fUrl.Split('/');
                        var routeParts = route.Split('/');

                        match = route.Contains('*') || (urlParts != null && routeParts != null && urlParts.Length == routeParts.Length);
                        //match = urlParts != null && routeParts != null && urlParts.Length == routeParts.Length;
                    }

                    _routeCache.Add(cacheKey, match);
                }

                return match;
            }

            return false;
        }

        private static string CreateRouteRegexPattern(string route)
        {
            var pattern = route;

            if (!string.IsNullOrEmpty(route))
            {
                if (_routeParameterRegex.IsMatch(route))
                {
                    var matches = _routeParameterRegex.Matches(route);
                    if (!matches.IsNullOrEmpty())
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Groups?.Count > 1)
                            {
                                var matchText = match.Groups[1].ToString();

                                pattern = pattern.Replace(matchText, ".*");
                            }
                        }
                    }
                }
            }

            return $"^{pattern}$";
        }

        private static Dictionary<string, RouteParameter> GetRouteParameters(string url, string route)
        {
            var outputParameters = new Dictionary<string, RouteParameter>();

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(route))
            {
                var urlParts = url.Split('/');
                var routeParts = route.Split('/');

                var parameterNames = new List<string>();

                //if (urlParts != null && routeParts != null && urlParts.Length == routeParts.Length)
                if (urlParts != null && routeParts != null)
                {
                    for (var i = 0; i < urlParts.Length && i < routeParts.Length; i++)
                    {
                        var urlPart = urlParts[i];
                        var routePart = routeParts[i];

                        //if (_routeParameterRegex.IsMatch(routePart))
                        //{
                        //    var match = _routeParameterRegex.Match(routePart);
                        //    if (match != null)
                        //    {
                        //        var matchText = match.Groups[1].ToString();
                        //        var routeParameterName = match.Groups[2].ToString();

                        //        //Url.GetRouteParameter(url, route, routeParameterName);

                        //        parameterNames.Add(routeParameterName);
                        //    }
                        //}

                        //// Check for Slug Match
                        //var slugRegex = new Regex(@"(\{\*(.*?)(?:\:.+)?\})");
                        //if (slugRegex.IsMatch(routePart))
                        //{
                        //    var match = slugRegex.Match(routePart);
                        //    if (match != null)
                        //    {
                        //        var matchText = match.Groups[1].ToString();
                        //        var routeParameterName = match.Groups[2].ToString();

                        //        if (routeParameterName == parameterName)
                        //        {
                        //            var slugParts = new List<string>();
                        //            for (var j = i; j < urlParts.Length; j++) slugParts.Add(urlParts[j]);

                        //            return string.Join('/', slugParts);
                        //        }
                        //    }
                        //}

                        //// Check for Exact Match
                        //var exactRegex = new Regex(@"(\{(.*?)(?:\:.+)?\})");
                        //if (exactRegex.IsMatch(routePart))
                        //{
                        //    var match = exactRegex.Match(routePart);
                        //    if (match != null)
                        //    {
                        //        var matchText = match.Groups[1].ToString();
                        //        var routeParameterName = match.Groups[2].ToString();

                        //        if (routeParameterName == parameterName) return urlPart;
                        //    }
                        //}

                        if (_routeParameterRegex.IsMatch(routePart))
                        {
                            var match = _routeParameterRegex.Match(routePart);
                            if (match != null)
                            {
                                var matchText = match.Groups[1].ToString();
                                var routeParameterName = match.Groups[2].ToString().TrimStart('*');

                                parameterNames.Add(routeParameterName);
                            }
                        }
                    }
                }

                foreach (var parameterName in parameterNames)
                {
                    var parameterValue = Url.GetRouteParameter(url, route, parameterName);
                    outputParameters.Add(parameterName, new RouteParameter(parameterName, parameterValue));
                }
            }

            return outputParameters;
        }

        private static object ChangeType(object obj, Type type)
        {
            if (obj != null)
            {
                var convertType = type;

                if (type.IsValueType && Nullable.GetUnderlyingType(type) != null)
                {
                    convertType = Nullable.GetUnderlyingType(type);
                }

                try
                {
                    if (convertType == typeof(DateTime))
                    {
                        return obj.ToString().ToDateTime();
                    }
                    else if (typeof(Enum).IsAssignableFrom(convertType) && obj.GetType() == typeof(string))
                    {
                        if (Enum.TryParse(convertType, (string)obj, true, out var result))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return Convert.ChangeType(obj, convertType);
                    }
                }
                catch { }
            }

            return default;
        }

        #endregion


        private void LoadDelayElapsed(object sender, EventArgs args)
        {
            Load();
        }

        public void Load()
        {
            var foundRoutes = new List<string>();

            var configurations = _configurationProfile.Get<TrakHoundApiConfiguration>(TrakHoundApiConfiguration.ConfigurationCategory);
            if (!configurations.IsNullOrEmpty())
            {
                foreach (var configuration in configurations.ToList())
                {
                    if (!string.IsNullOrEmpty(configuration.Route))
                    {
                        var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                        if (module != null)
                        {
                            // Get the Installed Hash (to check if the configuration has changed)
                            var installedConfigurationHash = _installedConfigurationHashes.GetValueOrDefault(configuration.Id);
                            var installedModuleHash = _installedModuleHashes.GetValueOrDefault(configuration.Id);

                            if (configuration.Hash != installedConfigurationHash || module.Package.Hash != installedModuleHash)
                            {
                                _installedConfigurationHashes.Remove(configuration.Id);
                                _installedModuleHashes.Remove(configuration.Id);

                                ConfigureRoute(configuration, module);

                                // Add to installed List
                                _installedConfigurationHashes.Add(configuration.Id, configuration.Hash);
                                _installedModuleHashes.Add(configuration.Id, module.Package.Hash);
                            }

                            foundRoutes.Add(configuration.Route);
                        }
                    }
                }
            }

            // Remove unused Routes
            lock (_lock)
            {
                if (!_routes.IsNullOrEmpty())
                {
                    var routeConfigurations = _routes.ToList();
                    var routes = routeConfigurations.Select(o => o.Route).ToList();
                    foreach (var route in routes)
                    {
                        if (!foundRoutes.Contains(route))
                        {
                            routeConfigurations.RemoveAll(o => o.Route == route);
                        }
                    }

                    routeConfigurations = routeConfigurations.OrderByDescending(o => o.Route).ToList();
                    _routes = routeConfigurations.ToArray();
                }
            }
        }

        private void ConfigureRoute(TrakHoundApiConfiguration configuration, ITrakHoundModule module)
        {
            if (configuration != null && module != null && !module.ModuleTypes.IsNullOrEmpty())
            {
                var controllers = new List<ITrakHoundApiController>();
                foreach (var moduleType in module.ModuleTypes)
                {
                    var client = _clientProvider.GetClient();

                    var volumeId = configuration.VolumeId ?? configuration.Id;
                    var volume = _volumeProvider.GetVolume(volumeId);

                    var logger = new TrakHoundLogger(configuration.Id);
                    logger.LogEntryReceived += LogEntryReceived;

                    if (client != null && volume != null)
                    {
                        var source = TrakHoundSourceEntry.CreateModuleSource(module);

                        var sourceChain = new TrakHoundSourceChain();
                        sourceChain.Add(source);
                        sourceChain.Add(TrakHoundSourceEntry.CreateApplicationSource());
                        sourceChain.Add(TrakHoundSourceEntry.CreateUserSource());
                        sourceChain.Add(TrakHoundSourceEntry.CreateDeviceSource());
                        sourceChain.Add(TrakHoundSourceEntry.CreateNetworkSource());
                        sourceChain.Add(TrakHoundSourceEntry.CreateInstanceSource(_instance.Id));

                        client.RouterId = configuration.RouterId;
                        client.AddMiddleware(new TrakHoundSourceMiddleware(sourceChain));

                        var controller = CreateInstance(module, moduleType);
                        if (controller != null)
                        {
                            ((TrakHoundApiController)controller).Id = configuration.Id;
                            ((TrakHoundApiController)controller).InstanceId = _instance.Id;
                            ((TrakHoundApiController)controller).BaseUrl = _baseUrl;
                            ((TrakHoundApiController)controller).BasePath = configuration.Route;
                            ((TrakHoundApiController)controller).BaseLocation = $"{_packagesDirectory}/{module.PackageId}/{module.PackageVersion}";
                            ((TrakHoundApiController)controller).SourceChain = sourceChain;
                            ((TrakHoundApiController)controller).Source = source;
                            ((TrakHoundApiController)controller).SetConfiguration(configuration);
                            ((TrakHoundApiController)controller).SetPackage(module.Package);
                            ((TrakHoundApiController)controller).SetClient(client);
                            ((TrakHoundApiController)controller).SetVolume(volume);
                            ((TrakHoundApiController)controller).SetLogger(logger);

                            controllers.Add(controller);
                        }
                    }
                }

                List<ApiRouteConfiguration> routes;
                lock (_lock) routes = _routes?.ToList();
                if (routes == null) routes = new List<ApiRouteConfiguration>();

                routes.RemoveAll(o => o.Route == configuration.Route);
                routes.Add(new ApiRouteConfiguration(configuration.Route, module.Package, configuration, controllers, module.PackageReadMe));
                routes = routes.OrderByDescending(o => o.Route).ToList();

                lock (_lock) _routes = routes.ToArray();

                var apiInformation = TrakHoundApiInformation.Create(controllers, configuration, module.Package, module.PackageReadMe);
                if (apiInformation != null)
                {
                    // Add Security Resources
                    foreach (var controllerInformation in apiInformation.Controllers)
                    {
                        foreach (var endpointInformation in controllerInformation.EndPoints)
                        {
                            var resourceId = $"api:{apiInformation.Id}:{controllerInformation.Id}:{endpointInformation.Id}";

                            if (!endpointInformation.Permissions.IsNullOrEmpty())
                            {
                                _instance.SecurityManager.AddResource(Security.TrakHoundIdentityResourceType.Api, resourceId, endpointInformation.Permissions);
                            }
                            else
                            {
                                _instance.SecurityManager.AddResource(Security.TrakHoundIdentityResourceType.Api, resourceId);
                            }
                        }
                    }

                    if (ApiAdded != null) ApiAdded.Invoke(this, apiInformation);
                }
            }
        }

        private void LogEntryReceived(object sender, TrakHoundLogItem item)
        {
            if (sender != null)
            {
                var logger = sender as ITrakHoundLogger;
                if (logger != null)
                {
                    if (ApiLogUpdated != null) ApiLogUpdated.Invoke(logger.Name, item);
                }
            }
        }

        private ITrakHoundApiConfiguration CreateDefaultConfiguration(ITrakHoundModule module)
        {
            var configuration = new TrakHoundApiConfiguration();
            configuration.Id = module.Package.Hash;
            configuration.PackageId = module.PackageId;
            configuration.PackageVersion = module.PackageVersion;
            return configuration;
        }

        private ITrakHoundApiController CreateInstance(ITrakHoundModule module, Type type)
        {
            if (type != null)
            {
                try
                {
                    var constructor = type.GetConstructor(new Type[] { });
                    if (constructor != null)
                    {
                        return (ITrakHoundApiController)constructor.Invoke(new object[] { });
                    }
                }
                catch (Exception ex)
                {
                    if (ApiLoadError != null) ApiLoadError.Invoke(this, ex);
                }
            }

            return null;
        }


        private static string GetMethodId(MethodInfo methodInfo)
        {
            var signatureString = string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.Name).ToArray());
            var returnTypeName = methodInfo.ReturnType.Name;

            if (methodInfo.IsGenericMethod)
            {
                var typeParamsString = string.Join(",", methodInfo.GetGenericArguments().Select(g => g.AssemblyQualifiedName).ToArray());

                // returns a string like this: "Assembly.YourSolution.YourProject.YourClass:YourMethod(Param1TypeName,...,ParamNTypeName):ReturnTypeName
                return string.Format("{0}:{1}<{2}>({3}):{4}", methodInfo.DeclaringType.AssemblyQualifiedName, methodInfo.Name, typeParamsString, signatureString, returnTypeName);
            }

            return string.Format("{0}:{1}({2}):{3}", methodInfo.DeclaringType.AssemblyQualifiedName, methodInfo.Name, signatureString, returnTypeName);
        }
    }
}
