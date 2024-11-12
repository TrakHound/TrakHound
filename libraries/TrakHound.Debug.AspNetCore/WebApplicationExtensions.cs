// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Volumes;

namespace TrakHound.Debug.AspNetCore
{
    public static class WebApplicationExtensions
    {
        public static void MapTrakHoundApiEndpoints(this WebApplication app, ITrakHoundClient client, ITrakHoundVolume volume, string instanceId = null)
        {
            var assembly = Assembly.GetCallingAssembly();
            var controllerTypes = assembly.GetTypes().Where(o => typeof(ITrakHoundApiController).IsAssignableFrom(o));
            if (!controllerTypes.IsNullOrEmpty())
            {
                foreach (var controllerType in controllerTypes)
                {
                    var controllerConstructor = controllerType.GetConstructor(new Type[] { });
                    if (controllerConstructor != null)
                    {
                        var moduleConfiguration = new TrakHoundApiConfiguration();

                        // Create Instance of the Controller
                        var controller = controllerConstructor.Invoke(new object[] { });
                        ((TrakHoundApiController)controller).Id = $"{controllerType.Assembly.Location}:{controllerType.AssemblyQualifiedName}";
                        ((TrakHoundApiController)controller).InstanceId = instanceId;
                        ((TrakHoundApiController)controller).BaseUrl = null;
                        ((TrakHoundApiController)controller).BasePath = null;
                        ((TrakHoundApiController)controller).BaseLocation = null;
                        ((TrakHoundApiController)controller).SourceChain = null; // Probably need to set this
                        ((TrakHoundApiController)controller).Source = null; // Probably need to set this
                        ((TrakHoundApiController)controller).SetConfiguration(moduleConfiguration);
                        ((TrakHoundApiController)controller).SetClient(client);
                        ((TrakHoundApiController)controller).SetVolume(volume);

                        var controllerRoute = Url.PathSeparator.ToString();
                        var controllerName = controllerType.Name;

                        var controllerAttribute = controllerType.GetCustomAttribute<TrakHoundApiControllerAttribute>();
                        if (controllerAttribute != null)
                        {
                            if (!string.IsNullOrEmpty(controllerAttribute.Route)) controllerRoute = controllerAttribute.Route;
                            if (!string.IsNullOrEmpty(controllerAttribute.Name)) controllerName = controllerAttribute.Name;
                        }


                        // Get List of Endpoint Methods
                        var endpointMethods = controllerType.GetMethods().Where(o => o.GetCustomAttribute<TrakHoundApiEndpointRouteAttribute>() != null);
                        if (!endpointMethods.IsNullOrEmpty())
                        {
                            foreach (var endpointMethod in endpointMethods)
                            {
                                var routeAttribute = endpointMethod.GetCustomAttribute<TrakHoundApiEndpointRouteAttribute>();
                                if (routeAttribute != null)
                                {
                                    var groupAttribute = endpointMethod.GetCustomAttribute<TrakHoundApiGroupAttribute>();

                                    // Get Endpoint Route
                                    var route = Url.Combine(controllerRoute, routeAttribute.Route, GetEndpointRouteSuffix(routeAttribute));
                                    route = $"/{route}";

                                    // Get HTTP Methods based on Route Type
                                    var httpMethods = GetEndpointHttpMethods(routeAttribute);

                                    // Get OpenApi Parameters to use for Swagger
                                    var openApiParameters = GetOpenApiParameters(endpointMethod);

                                    // Get OpenApi Request Body to use for Swagger
                                    var openApiRequestBody = GetOpenApiRequestBody(endpointMethod);

                                    // Map Endpoint for each Http Method
                                    foreach (var httpMethod in httpMethods)
                                    {
                                        //var name = $"({httpMethod}) {controllerName} {endpointMethod.Name}";

                                        //if (!openApiParameters.IsNullOrEmpty())
                                        //{
                                        //    var nameParameters = new List<string>();
                                        //    foreach (var openApiParameter in openApiParameters) nameParameters.Add(openApiParameter.Name);
                                        //    name += $"({string.Join(", ", nameParameters)})";
                                        //}

                                        // Map Endpoint
                                        var routeBuilder = app.MapMethods(route, new string[] { httpMethod }, async (HttpContext httpContext) =>
                                        {
                                            // Get Parameters based on Http Request
                                            var parameters = await GetEndpointParameters(controllerRoute, endpointMethod, routeAttribute, httpContext);

                                            // Invoke Method and Get Response
                                            TrakHoundApiResponse response = await (Task<TrakHoundApiResponse>)endpointMethod.Invoke(controller, parameters);

                                            // Process Response to IResult
                                            return ProcessResult(response);
                                        });

                                        // Set Controller Name
                                        routeBuilder.WithTags(controllerName);

                                        // Set Endpoint Name
                                        routeBuilder.WithSummary(endpointMethod.Name);

                                        // Set Group
                                        if (groupAttribute != null && !string.IsNullOrEmpty(groupAttribute.Name))
                                        {
                                            
                                            routeBuilder.WithTags(groupAttribute.Name);
                                            //routeBuilder.WithGroupName(groupAttribute.Name);
                                        }

                                        // Set OpenApi Parameters
                                        routeBuilder.WithOpenApi(options =>
                                        {
                                            // Add Open API Parameters
                                            foreach (var openApiParameter in openApiParameters) options.Parameters.Add(openApiParameter);

                                            // Add Open API Request Body Definition
                                            options.RequestBody = openApiRequestBody;

                                            return options;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string[] GetEndpointHttpMethods(TrakHoundApiEndpointRouteAttribute routeAttribute)
        {
            var methods = new List<string>();

            if (routeAttribute != null)
            {
                switch (routeAttribute.Type)
                {
                    case TrakHoundApiRouteType.Query:
                        methods.Add("Get");
                        methods.Add("Post");
                        break;

                    case TrakHoundApiRouteType.Subscribe:
                        methods.Add("Get");
                        methods.Add("Post");
                        break;

                    case TrakHoundApiRouteType.Publish:
                        methods.Add("Put");
                        methods.Add("Post");
                        break;

                    case TrakHoundApiRouteType.Delete:
                        methods.Add("Delete");
                        methods.Add("Post");
                        break;
                }
            }

            return methods.ToArray();
        }

        private static string GetEndpointRouteSuffix(TrakHoundApiEndpointRouteAttribute routeAttribute)
        {
            if (routeAttribute != null)
            {
                switch (routeAttribute.Type)
                {
                    case TrakHoundApiRouteType.Query: return "";

                    case TrakHoundApiRouteType.Subscribe: return "subscribe";

                    case TrakHoundApiRouteType.Publish: return "publish";

                    case TrakHoundApiRouteType.Delete: return "delete";
                }
            }

            return "";
        }

        private static async Task<object[]> GetEndpointParameters(string controllerRoute, MethodInfo endpointMethod, TrakHoundApiEndpointRouteAttribute routeAttribute, HttpContext httpContext)
        {
            var parameters = new List<object>();

            var endpointParameters = endpointMethod.GetParameters();
            if (!endpointParameters.IsNullOrEmpty())
            {
                var route = Url.Combine(controllerRoute, routeAttribute.Route);

                foreach (var endpointParameter in endpointParameters)
                {
                    object inputParameterValue = null;
                    bool set = false;

                    if (endpointParameter.GetCustomAttribute<FromRouteAttribute>() != null)
                    {
                        if (endpointParameter.ParameterType == typeof(DateTime))
                        {
                            var queryDateTime = Url.GetRouteParameter(httpContext.Request.Path, route, endpointParameter.Name);
                            inputParameterValue = queryDateTime.ToDateTime();
                            set = true;
                        }
                        else
                        {
                            try
                            {
                                var queryParameterValue = Url.GetRouteParameter(httpContext.Request.Path, route, endpointParameter.Name);
                                inputParameterValue = Convert.ChangeType(queryParameterValue, endpointParameter.ParameterType);
                                set = true;
                            }
                            catch { }
                        }
                    }
                    else if (endpointParameter.GetCustomAttribute<FromBodyAttribute>() != null)
                    {
                        var fromBodyAttribute = endpointParameter.GetCustomAttribute<FromBodyAttribute>();

                        byte[] requestBody = null;
                        if (httpContext.Request.Body != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await httpContext.Request.Body.CopyToAsync(memoryStream);
                                requestBody = memoryStream.ToArray();
                            }
                        }

                        switch (fromBodyAttribute.ContentType)
                        {
                            case "application/json":

                                try
                                {
                                    inputParameterValue = JsonSerializer.Deserialize(requestBody, endpointParameter.ParameterType);
                                    set = true;
                                }
                                catch { }
                                break;

                            case "text/plain":

                                try
                                {
                                    inputParameterValue = Convert.ChangeType(System.Text.Encoding.UTF8.GetString(requestBody), endpointParameter.ParameterType);
                                    set = true;
                                }
                                catch { }
                                break;

                            default:
                                inputParameterValue = requestBody;
                                set = true;
                                break;
                        }
                    }
                    else
                    {
                        if (endpointParameter.ParameterType == typeof(DateTime))
                        {
                            var queryDateTime = httpContext.Request.Query[endpointParameter.Name].FirstOrDefault();
                            inputParameterValue = queryDateTime.ToDateTime();
                            set = true;
                        }
                        else
                        {
                            try
                            {
                                var queryParameterValue = httpContext.Request.Query[endpointParameter.Name].FirstOrDefault();
                                inputParameterValue = Convert.ChangeType(queryParameterValue, endpointParameter.ParameterType);
                                set = true;
                            }
                            catch { }
                        }
                    }

                    if (set)
                    {
                        parameters.Add(inputParameterValue);
                    }
                    else
                    {
                        if (endpointParameter.HasDefaultValue)
                        {
                            parameters.Add(endpointParameter.DefaultValue);
                        }
                    }
                }
            }

            return parameters.ToArray();
        }

        private static IEnumerable<OpenApiParameter> GetOpenApiParameters(MethodInfo endpointMethod)
        {
            var apiParameters = new List<OpenApiParameter>();

            var endpointParameters = endpointMethod.GetParameters();
            var routeParameters = endpointParameters?.Where(o => o.GetCustomAttribute<FromRouteAttribute>() != null);
            var queryParameters = endpointParameters?.Where(o => o.GetCustomAttribute<FromQueryAttribute>() != null);

            if (!routeParameters.IsNullOrEmpty())
            {
                foreach (var parameter in routeParameters)
                {
                    var apiParameter = new OpenApiParameter();
                    apiParameter.Name = parameter.Name;
                    apiParameter.In = ParameterLocation.Path;
                    apiParameters.Add(apiParameter);
                }
            }

            if (!queryParameters.IsNullOrEmpty())
            {
                foreach (var parameter in queryParameters)
                {
                    var apiParameter = new OpenApiParameter();
                    apiParameter.Name = parameter.Name;
                    apiParameter.In = ParameterLocation.Query;
                    apiParameters.Add(apiParameter);
                }
            }

            return apiParameters;
        }

        private static OpenApiRequestBody GetOpenApiRequestBody(MethodInfo endpointMethod)
        {
            var endpointParameters = endpointMethod.GetParameters();
            var bodyParameters = endpointParameters?.Where(o => o.GetCustomAttribute<FromBodyAttribute>() != null);
            if (!bodyParameters.IsNullOrEmpty())
            {
                var requestBody = new OpenApiRequestBody();

                var content = new Dictionary<string, OpenApiMediaType>();
                foreach (var parameter in bodyParameters)
                {
                    var fromBodyAttribute = parameter.GetCustomAttribute<FromBodyAttribute>();
                    if (fromBodyAttribute != null)
                    {
                        if (!string.IsNullOrEmpty(fromBodyAttribute.ContentType))
                        {
                            var mediaType = new OpenApiMediaType();

                            var schema = new OpenApiSchema();
                            schema.Type = "string";
                            mediaType.Schema = schema;

                            content.Add(fromBodyAttribute.ContentType, mediaType);
                        }

                        requestBody.Required = !parameter.HasDefaultValue;
                    }
                }
                requestBody.Content = content;

                return requestBody;
            }

            return null;
        }

        private static IResult ProcessResult(TrakHoundApiResponse response)
        {
            if (response.Content != null)
            {
                var contentString = response.GetContentUtf8String();

                return Results.Content(contentString, response.ContentType, System.Text.Encoding.UTF8, response.StatusCode);
            }

            return Results.StatusCode(response.StatusCode);
        }
    }
}
