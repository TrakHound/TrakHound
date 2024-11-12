// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;
using System.Reflection.Metadata;
using TrakHound.Blazor.Routing;

namespace TrakHound.Blazor.Components
{
    public partial class TrakHoundApp : ComponentBase, IDisposable
    {
        RenderHandle _renderHandle;
        bool _navigationInterceptionEnabled;
        string _location;

        private CancellationTokenSource _onNavigateCts;
        private Task _previousOnNavigateTask = Task.CompletedTask;


        protected RenderHandle RenderHandle => _renderHandle;


        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private INavigationInterception NavigationInterception { get; set; }

        [Inject]
        ITrakHoundPageRouteManager RouteManager { get; set; }


        // Add Cascading Parameters (not needed. Cascades from root Router?)
        // - AppId
        // - Client
        // - BaseUrl
        // - BasePath


        [Parameter]
        public string BasePath { get; set; }

        [Parameter]
        public string Route { get; set; }

        [Parameter]
        public TrakHoundRouteData RouteData { get; set; }

        [Parameter]
        public RenderFragment NotFound { get; set; }

        [Parameter]
        public RenderFragment<RouteData> Found { get; set; }

        [Parameter]
        public RenderFragment Navigating { get; set; }

        [Parameter]
        public EventCallback<NavigationContext> OnNavigateAsync { get; set; }

        [Parameter]
        public Dictionary<string, object> Parameters { get; set; }


        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
            _location = NavigationManager.Uri;
            NavigationManager.LocationChanged += HandleLocationChanged;
        }

        protected virtual void OnRefresh(TrakHoundRouteData routeData)
        {
            var outputRouteData = new TrakHoundRouteData();
            outputRouteData.AppId = routeData.AppId;
            outputRouteData.Client = routeData.Client;
            outputRouteData.BaseUrl = routeData.BaseUrl;
            outputRouteData.BasePath = Url.Combine(BasePath, routeData.BasePath);
            outputRouteData.BaseLocation = routeData.BaseLocation;
            outputRouteData.Data = routeData.Data;

            // Filter out Parameters that aren't implmented in the Handler Type
            if (routeData != null && routeData.Data.PageType != null && !routeData.Data.RouteValues.IsNullOrEmpty())
            {
                var usedParameters = new Dictionary<string, object>();

                var properties = routeData.Data.PageType.GetProperties();
                if (!properties.IsNullOrEmpty())
                {
                    foreach (var parameter in routeData.Data.RouteValues)
                    {
                        if (!string.IsNullOrEmpty(parameter.Key))
                        {
                            var property = properties.FirstOrDefault(o => o.Name.ToLower() == parameter.Key.ToLower());
                            if (property != null)
                            {
                                usedParameters.Add(parameter.Key, parameter.Value);
                            }
                        }
                    }
                }

                outputRouteData.Data = new RouteData(routeData.Data.PageType, usedParameters);
            }

            RouteData = outputRouteData;

            //_renderHandle.Render(Found(routeData));
        }

        protected override Task OnParametersSetAsync()
        {
            //parameters.SetParameterProperties(this);

            if (Found == null)
            {
                //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(Found)}.");
            }

            if (NotFound == null)
            {
                //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(NotFound)}.");
            }

            RouteManager.Initialise();
            Refresh(false);

            return Task.CompletedTask;
        }

        //public Task SetParametersAsync(ParameterView parameters)
        //{
        //    parameters.SetParameterProperties(this);

        //    if (Found == null)
        //    {
        //        //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(Found)}.");
        //    }

        //    if (NotFound == null)
        //    {
        //        //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(NotFound)}.");
        //    }

        //    RouteManager.Initialise();
        //    Refresh(false);

        //    return Task.CompletedTask;
        //}

        //public Task SetParametersAsync(ParameterView parameters)
        //{
        //    parameters.SetParameterProperties(this);

        //    if (Found == null)
        //    {
        //        //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(Found)}.");
        //    }

        //    if (NotFound == null)
        //    {
        //        //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(NotFound)}.");
        //    }

        //    RouteManager.Initialise();
        //    Refresh(false);

        //    return Task.CompletedTask;
        //}

        public Task OnAfterRenderAsync()
        {
            if (!_navigationInterceptionEnabled)
            {
                _navigationInterceptionEnabled = true;
                return NavigationInterception.EnableNavigationInterceptionAsync();
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= HandleLocationChanged;
        }

        private void HandleLocationChanged(object sender, LocationChangedEventArgs args)
        {
            //_location = args.Location;
            //Refresh(args.IsNavigationIntercepted);



            //if (_renderHandle.IsInitialized)
            //{
            //    _ = RunOnNavigateAsync(NavigationManager.ToBaseRelativePath(_location), args.IsNavigationIntercepted).Preserve();
            //}
        }


        internal virtual void Refresh(bool isNavigationIntercepted)
        {
            // If an `OnNavigateAsync` task is currently in progress, then wait
            // for it to complete before rendering. Note: because _previousOnNavigateTask
            // is initialized to a CompletedTask on initialization, this will still
            // allow first-render to complete successfully.
            if (_previousOnNavigateTask.Status != TaskStatus.RanToCompletion)
            {
                if (Navigating != null)
                {
                    _renderHandle.Render(Navigating);
                }
                return;
            }


            var relativeUri = Route;
            if (!relativeUri.StartsWith('/')) relativeUri = "/" + relativeUri;

            var matchUri = relativeUri;
            if (matchUri.IndexOf('?') > -1)
            {
                matchUri = matchUri.Substring(0, matchUri.IndexOf('?'));
            }

            var matchResult = RouteManager.Match(matchUri);

            if (matchResult.IsMatch)
            {
                var parameters = new Dictionary<string, object>();

                // Set Property Parameters (passed as the 'Parameters' property to the component)
                var propertyParameters = GetPropertyParameters(matchResult.MatchedRoute.Handler, Parameters);
                if (!propertyParameters.IsNullOrEmpty())
                {
                    foreach (var propertyParameter in propertyParameters)
                    {
                        if (!parameters.ContainsKey(propertyParameter.Key) || propertyParameter.Value != null)
                        {
                            parameters.Remove(propertyParameter.Key);
                            parameters.Add(propertyParameter.Key, propertyParameter.Value);
                        }
                    }
                }

                // Set Route Parameters
                var routeParameters = GetRouteParameters(matchResult.MatchedRoute.Handler, matchResult.MatchedRoute.Template, relativeUri);
                if (!routeParameters.IsNullOrEmpty())
                {
                    foreach (var routeParameter in routeParameters)
                    {
                        if (!parameters.ContainsKey(routeParameter.Key) || routeParameter.Value != null)
                        {
                            parameters.Remove(routeParameter.Key);
                            parameters.Add(routeParameter.Key, routeParameter.Value);
                        }
                    }
                }

                // Set Query Parameters
                var queryParameters = ParseQueryString(matchResult.MatchedRoute.Handler, relativeUri);
                //var queryParameters = ParseQueryString(matchResult.MatchedRoute.Handler, NavigationManager.ToBaseRelativePath(_location));
                if (!queryParameters.IsNullOrEmpty())
                {
                    foreach (var queryParameter in queryParameters)
                    {
                        if (!parameters.ContainsKey(queryParameter.Key) || queryParameter.Value != null)
                        {
                            parameters.Remove(queryParameter.Key);
                            parameters.Add(queryParameter.Key, queryParameter.Value);
                        }
                    }
                }

                //// Filter out Parameters that aren't implmented in the Handler Type
                //if (!parameters.IsNullOrEmpty())
                //{
                //    var usedParameters = new Dictionary<string, object>();

                //    var properties = matchResult.MatchedRoute.Handler.GetProperties();
                //    if (!properties.IsNullOrEmpty())
                //    {
                //        foreach (var parameter in parameters)
                //        {
                //            if (!string.IsNullOrEmpty(parameter.Key))
                //            {
                //                var property = properties.FirstOrDefault(o => o.Name.ToLower() == parameter.Key.ToLower());
                //                if (property != null)
                //                {
                //                    usedParameters.Add(parameter.Key, parameter.Value);
                //                }
                //            }
                //        }
                //    }

                //    parameters = usedParameters;
                //}

                var routeData = new TrakHoundRouteData();
                routeData.AppId = matchResult.MatchedRoute.AppId;
                routeData.Client = matchResult.MatchedRoute.Client;
                routeData.BaseUrl = matchResult.MatchedRoute.BaseUrl;
                routeData.BasePath = matchResult.MatchedRoute.BasePath;
                routeData.BaseLocation = matchResult.MatchedRoute.BaseLocation;
                routeData.Data = new RouteData(matchResult.MatchedRoute.Handler, parameters);

                OnRefresh(routeData);
            }
            else
            {
                //_renderHandle.Render(NotFound);
            }


            //var relativePath = NavigationManager.ToBaseRelativePath(_locationAbsolute.AsSpan());
            //var locationPathSpan = TrimQueryOrHash(relativePath);
            //var locationPath = $"/{locationPathSpan}";

            //// In order to avoid routing twice we check for RouteData
            //if (RoutingStateProvider?.RouteData is { } endpointRouteData)
            //{
            //    // Other routers shouldn't provide RouteData, this is specific to our router component
            //    // and must abide by our syntax and behaviors.
            //    // Other routers must create their own abstractions to flow data from their SSR routing
            //    // scheme to their interactive router.
            //    Log.NavigatingToComponent(_logger, endpointRouteData.PageType, locationPath, _baseUri);
            //    // Post process the entry to add Blazor specific behaviors:
            //    // - Add 'null' for unused route parameters.
            //    // - Convert constrained parameters with (int, double, etc) to the target type.
            //    endpointRouteData = RouteTable.ProcessParameters(endpointRouteData);
            //    _renderHandle.Render(Found(endpointRouteData));
            //    return;
            //}

            //RefreshRouteTable();

            //var context = new RouteContext(locationPath);
            //Routes.Route(context);

            //if (context.Handler != null)
            //{
            //    if (!typeof(IComponent).IsAssignableFrom(context.Handler))
            //    {
            //        throw new InvalidOperationException($"The type {context.Handler.FullName} " +
            //            $"does not implement {typeof(IComponent).FullName}.");
            //    }

            //    Log.NavigatingToComponent(_logger, context.Handler, locationPath, _baseUri);

            //    var routeData = new RouteData(
            //        context.Handler,
            //        context.Parameters ?? _emptyParametersDictionary);

            //    _renderHandle.Render(Found(routeData));

            //    // If you navigate to a different path, then after the next render we'll update the scroll position
            //    if (relativePath != _updateScrollPositionForHashLastLocation)
            //    {
            //        _updateScrollPositionForHashLastLocation = relativePath.ToString();
            //        _updateScrollPositionForHash = true;
            //    }
            //}
            //else
            //{
            //    if (!isNavigationIntercepted)
            //    {
            //        Log.DisplayingNotFound(_logger, locationPath, _baseUri);

            //        // We did not find a Component that matches the route.
            //        // Only show the NotFound content if the application developer programatically got us here i.e we did not
            //        // intercept the navigation. In all other cases, force a browser navigation since this could be non-Blazor content.
            //        _renderHandle.Render(NotFound ?? DefaultNotFoundContent);
            //    }
            //    else
            //    {
            //        Log.NavigatingToExternalUri(_logger, _locationAbsolute, locationPath, _baseUri);
            //        NavigationManager.NavigateTo(_locationAbsolute, forceLoad: true);
            //    }
            //}
        }

        //private void Refresh()
        //{
        //    var relativeUri = NavigationManager.ToBaseRelativePath(_location);
        //    //var parameters = ParseQueryString(relativeUri);

        //    if (relativeUri.IndexOf('?') > -1)
        //    {
        //        relativeUri = relativeUri.Substring(0, relativeUri.IndexOf('?'));
        //    }

        //    if (!relativeUri.StartsWith('/')) relativeUri = "/" + relativeUri;
        //    var matchResult = RouteManager.Match(relativeUri);

        //    if (matchResult.IsMatch)
        //    {
        //        var parameters = new Dictionary<string, object>();

        //        // Set Route Parameters
        //        var routeParameters = GetRouteParameters(matchResult.MatchedRoute.Handler, matchResult.MatchedRoute.Template, relativeUri);
        //        if (!routeParameters.IsNullOrEmpty())
        //        {
        //            foreach (var routeParameter in routeParameters)
        //            {
        //                parameters.Remove(routeParameter.Key);
        //                parameters.Add(routeParameter.Key, routeParameter.Value);
        //            }
        //        }

        //        // Set Query Parameters
        //        var queryParameters = ParseQueryString(matchResult.MatchedRoute.Handler, NavigationManager.ToBaseRelativePath(_location));
        //        if (!queryParameters.IsNullOrEmpty())
        //        {
        //            foreach (var queryParameter in queryParameters)
        //            {
        //                parameters.Remove(queryParameter.Key);
        //                parameters.Add(queryParameter.Key, queryParameter.Value);
        //            }
        //        }

        //        //// Filter out Parameters that aren't implmented in the Handler Type
        //        //if (!parameters.IsNullOrEmpty())
        //        //{
        //        //    var usedParameters = new Dictionary<string, object>();

        //        //    var properties = matchResult.MatchedRoute.Handler.GetProperties();
        //        //    if (!properties.IsNullOrEmpty())
        //        //    {
        //        //        foreach (var parameter in parameters)
        //        //        {
        //        //            if (!string.IsNullOrEmpty(parameter.Key))
        //        //            {
        //        //                var property = properties.FirstOrDefault(o => o.Name.ToLower() == parameter.Key.ToLower());
        //        //                if (property != null)
        //        //                {
        //        //                    usedParameters.Add(parameter.Key, parameter.Value);
        //        //                }
        //        //            }
        //        //        }
        //        //    }

        //        //    parameters = usedParameters;
        //        //}

        //        var routeData = new TrakHoundRouteData();
        //        routeData.AppId = matchResult.MatchedRoute.AppId;
        //        routeData.Client = matchResult.MatchedRoute.Client;
        //        routeData.BaseUrl = matchResult.MatchedRoute.BaseUrl;
        //        routeData.BasePath = matchResult.MatchedRoute.BasePath;
        //        routeData.Data = new RouteData(matchResult.MatchedRoute.Handler, parameters);

        //        OnRefresh(routeData);
        //    }
        //    else
        //    {
        //        _renderHandle.Render(NotFound);
        //    }
        //}

        internal async ValueTask RunOnNavigateAsync(string path, bool isNavigationIntercepted)
        {
            // Cancel the CTS instead of disposing it, since disposing does not
            // actually cancel and can cause unintended Object Disposed Exceptions.
            // This effectively cancels the previously running task and completes it.
            _onNavigateCts?.Cancel();
            // Then make sure that the task has been completely cancelled or completed
            // before starting the next one. This avoid race conditions where the cancellation
            // for the previous task was set but not fully completed by the time we get to this
            // invocation.
            await _previousOnNavigateTask;

            //var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            //_previousOnNavigateTask = tcs.Task;

            //if (!OnNavigateAsync.HasDelegate)
            //{
            //    Refresh(isNavigationIntercepted);
            //}

            //_onNavigateCts = new CancellationTokenSource();
            //var navigateContext = new NavigationContext(path, );
            ////var navigateContext = new NavigationContext(path, _onNavigateCts.Token);

            //var cancellationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            //navigateContext.CancellationToken.Register(state =>
            //    ((TaskCompletionSource)state).SetResult(), cancellationTcs);

            //try
            //{
            //    // Task.WhenAny returns a Task<Task> so we need to await twice to unwrap the exception
            //    var task = await Task.WhenAny(OnNavigateAsync.InvokeAsync(navigateContext), cancellationTcs.Task);
            //    await task;
            //    tcs.SetResult();
            //    Refresh(isNavigationIntercepted);
            //}
            //catch (Exception e)
            //{
            //    //_renderHandle.Render(builder => ExceptionDispatchInfo.Throw(e));
            //}
        }


        private Dictionary<string, object> GetPropertyParameters(Type handlerType, Dictionary<string, object> parameters)
        {
            var parameterValues = new Dictionary<string, object>();

            if (handlerType != null && !parameters.IsNullOrEmpty())
            {
                var properties = handlerType.GetProperties();
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        var parameterAttribute = property.GetCustomAttribute<ParameterAttribute>();
                        if (parameterAttribute != null)
                        {
                            var propertyParameterValue = parameters.GetValueOrDefault(property.Name);
                            object parameterValue = null;

                            try
                            {
                                if (property.PropertyType == typeof(DateTime))
                                {
                                    parameterValue = propertyParameterValue?.ToString().ToDateTime();
                                }
                                else
                                {
                                    try
                                    {
                                        parameterValue = Convert.ChangeType(propertyParameterValue, property.PropertyType);
                                    }
                                    catch { }
                                }
                            }
                            catch { }

                            parameterValues.Add(property.Name, parameterValue);
                        }
                    }
                }
            }

            return parameterValues;
        }

        private Dictionary<string, object> GetRouteParameters(Type handlerType, string template, string uri)
        {
            var parameterValues = new Dictionary<string, object>();

            if (handlerType != null && !string.IsNullOrEmpty(uri))
            {
                var properties = handlerType.GetProperties();
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        var parameterAttribute = property.GetCustomAttribute<ParameterAttribute>();
                        if (parameterAttribute != null)
                        {
                            var routeParameterValue = Url.GetRouteParameter(uri, template, property.Name.ToCamelCase());
                            object parameterValue = null;

                            try
                            {
                                if (property.PropertyType == typeof(DateTime))
                                {
                                    parameterValue = routeParameterValue.ToDateTime();
                                }
                                else
                                {
                                    try
                                    {
                                        parameterValue = Convert.ChangeType(routeParameterValue, property.PropertyType);
                                    }
                                    catch { }
                                }
                            }
                            catch { }

                            parameterValues.Add(property.Name, parameterValue);
                        }
                    }
                }
            }

            return parameterValues;
        }

        private Dictionary<string, object> ParseQueryString(Type handlerType, string uri)
        {
            var queryParameters = new Dictionary<string, object>();

            var querystring = new Dictionary<string, string>();
            foreach (string kvp in uri.Substring(uri.IndexOf("?") + 1).Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (kvp != "" && kvp.Contains("="))
                {
                    var pair = kvp.Split('=');
                    var key = pair[0]?.ToLower();
                    if (key != null)
                    {
                        querystring.Remove(key);
                        querystring.Add(key, pair[1]);
                    }
                }
            }

            if (handlerType != null && !string.IsNullOrEmpty(uri))
            {
                var properties = handlerType.GetProperties();
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        var parameterAttribute = property.GetCustomAttribute<SupplyParameterFromQueryAttribute>();
                        if (parameterAttribute != null)
                        {
                            var propertyName = !string.IsNullOrEmpty(parameterAttribute.Name) ? parameterAttribute.Name : property.Name;

                            var queryParameterValue = querystring.GetValueOrDefault(propertyName.ToLower());
                            if (queryParameterValue != null)
                            {
                                object parameterValue = null;

                                try
                                {
                                    if (property.PropertyType == typeof(DateTime))
                                    {
                                        parameterValue = queryParameterValue.ToDateTime();
                                    }
                                    else
                                    {
                                        try
                                        {
                                            parameterValue = Convert.ChangeType(queryParameterValue, property.PropertyType);
                                        }
                                        catch { }
                                    }
                                }
                                catch { }

                                queryParameters.Add(property.Name, parameterValue);
                            }
                        }
                    }
                }
            }

            return queryParameters;
        }
    }
}
