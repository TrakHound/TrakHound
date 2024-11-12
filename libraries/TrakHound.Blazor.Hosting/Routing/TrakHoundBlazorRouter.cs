// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;
using System.Reflection.Metadata;

namespace TrakHound.Blazor.Routing
{
    public class TrakHoundBlazorRouter : TrakHoundBlazorRouter<TrakHoundRouteData> { }

    public abstract class TrakHoundBlazorRouter<TRouteData> : IComponent, IHandleAfterRender, IDisposable where TRouteData : TrakHoundRouteData
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

        [Inject]
        TrakHoundAuthenticationService AuthenticationService { get; set; }

        [Parameter]
        public RenderFragment NotFound { get; set; }

        [Parameter]
        public RenderFragment<TRouteData> Found { get; set; }

        [Parameter]
        public RenderFragment Navigating { get; set; }

        [Parameter]
        public EventCallback<NavigationContext> OnNavigateAsync { get; set; }


        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
            _location = NavigationManager.Uri;
            NavigationManager.LocationChanged += HandleLocationChanged;
        }

        protected virtual void OnRefresh(TrakHoundRouteData routeData)
        {
            var outputRouteData = new TrakHoundRouteData();
            outputRouteData.RouteManager = RouteManager;
            outputRouteData.AppId = routeData.AppId;
            outputRouteData.AppName = routeData.AppName;
            outputRouteData.Package = routeData.Package;
            outputRouteData.Client = routeData.Client;
            outputRouteData.Volume = routeData.Volume;
            outputRouteData.Logger = routeData.Logger;
            outputRouteData.Session = routeData.Session;
            outputRouteData.BaseUrl = routeData.BaseUrl;
            outputRouteData.BasePath = routeData.BasePath;
            outputRouteData.BaseLocation = routeData.BaseLocation;
            outputRouteData.Data = routeData.Data;

            // Filter out Parameters that aren't implmented in the Handler Type
            if (routeData != null && routeData.Data != null && routeData.Data.PageType != null && !routeData.Data.RouteValues.IsNullOrEmpty())
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

            if (Found != null) _renderHandle.Render(Found(outputRouteData as TRouteData));
        }

        public async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (Found == null)
            {
                //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(Found)}.");
            }

            if (NotFound == null)
            {
                //throw new InvalidOperationException($"The {nameof(TrakHoundBlazorRouter)} component requires a value for the parameter {nameof(NotFound)}.");
            }

            RouteManager.Initialise();
            await Refresh(false);
        }

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

        private async void HandleLocationChanged(object sender, LocationChangedEventArgs args)
        {
            _location = args.Location;
            await Refresh(args.IsNavigationIntercepted);
            //if (_renderHandle.IsInitialized)
            //{
            //    _ = RunOnNavigateAsync(NavigationManager.ToBaseRelativePath(_location), args.IsNavigationIntercepted).Preserve();
            //}
        }


        internal virtual async Task Refresh(bool isNavigationIntercepted)
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



            var relativeUri = NavigationManager.ToBaseRelativePath(_location);
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

                // Set Route Parameters
                var routeParameters = GetRouteParameters(matchResult.MatchedRoute.Handler, matchResult.MatchedRoute.Template, relativeUri);
                if (!routeParameters.IsNullOrEmpty())
                {
                    foreach (var routeParameter in routeParameters)
                    {
                        parameters.Remove(routeParameter.Key);
                        parameters.Add(routeParameter.Key, routeParameter.Value);
                    }
                }

                // Set Query Parameters
                var queryParameters = ParseQueryString(matchResult.MatchedRoute.Handler, NavigationManager.ToBaseRelativePath(_location));
                if (!queryParameters.IsNullOrEmpty())
                {
                    foreach (var queryParameter in queryParameters)
                    {
                        parameters.Remove(queryParameter.Key);
                        parameters.Add(queryParameter.Key, queryParameter.Value);
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
                routeData.RouteManager = RouteManager;
                routeData.AppId = matchResult.MatchedRoute.AppId;
                routeData.AppName = matchResult.MatchedRoute.AppName;
                routeData.Package = matchResult.MatchedRoute.Package;
                routeData.Client = matchResult.MatchedRoute.Client;
                routeData.Volume = matchResult.MatchedRoute.Volume;
                routeData.Logger = matchResult.MatchedRoute.Logger;
                routeData.Session = await AuthenticationService.GetSession();
                routeData.BaseUrl = matchResult.MatchedRoute.BaseUrl;
                routeData.BasePath = matchResult.MatchedRoute.BasePath;
                routeData.BaseLocation = matchResult.MatchedRoute.BaseLocation;
                routeData.Data = new RouteData(matchResult.MatchedRoute.Handler, parameters);

                OnRefresh(routeData);
            }
            else
            {
                if (NotFound != null) _renderHandle.Render(NotFound);
            }
        }

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
