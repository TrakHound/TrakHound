// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TrakHound.Packages;
using TrakHound.Security;

namespace TrakHound.Api
{
    public class TrakHoundApiInformation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; }

        [JsonPropertyName("packageVersion")]
        public string PackageId { get; set; }

        [JsonPropertyName("packageVersion")]
        public string PackageVersion { get; set; }

        [JsonPropertyName("packageBuildDate")]
        public DateTime PackageBuildDate { get; set; }

        [JsonPropertyName("packageHash")]
        public string PackageHash { get; set; }

        [JsonPropertyName("packageIcon")]
        public string PackageIcon { get; set; }

        [JsonPropertyName("packageReadMe")]
        public string PackageReadMe { get; set; }

        [JsonPropertyName("repository")]
        public string Repository { get; set; }

        [JsonPropertyName("repositoryBranch")]
        public string RepositoryBranch { get; set; }

        [JsonPropertyName("repositoryDirectory")]
        public string RepositoryDirectory { get; set; }

        [JsonPropertyName("repositoryCommit")]
        public string RepositoryCommit { get; set; }

        [JsonPropertyName("trakhoundVersion")]
        public string TrakHoundVersion { get; set; }

        [JsonPropertyName("controllers")]
        public IEnumerable<TrakHoundApiControllerInformation> Controllers { get; set; }



        public static TrakHoundApiInformation Create(IEnumerable<ITrakHoundApiController> controllers, ITrakHoundApiConfiguration configuration, TrakHoundPackage package, string packageReadme = null)
        {
            if (!controllers.IsNullOrEmpty() && configuration != null && package != null)
            {
                var information = new TrakHoundApiInformation();
                information.Id = configuration.Id;
                information.Route = configuration.Route;
                information.PackageId = configuration.PackageId;

                if (package != null)
                {
                    information.PackageVersion = package.Version;
                    information.PackageBuildDate = package.BuildDate;
                    information.PackageHash = package.Hash;
                    information.PackageIcon = package.GetMetadata(TrakHoundPackage.ImageName);

                    information.TrakHoundVersion = package.GetMetadata(TrakHoundPackage.TrakHoundVersionName);

                    information.Repository = package.GetMetadata(TrakHoundPackage.RepositoryName);
                    information.RepositoryBranch = package.GetMetadata(TrakHoundPackage.RepositoryBranchName);
                    information.RepositoryDirectory = package.GetMetadata(TrakHoundPackage.RepositoryDirectoryName);
                    information.RepositoryCommit = package.GetMetadata(TrakHoundPackage.RepositoryCommitName);
                }

                information.PackageReadMe = packageReadme;

                var controllerInformations = new List<TrakHoundApiControllerInformation>();
                foreach (var controller in controllers)
                {
                    controllerInformations.Add(CreateControllerInformation(configuration, controller.GetType()));
                }
                information.Controllers = controllerInformations;

                return information;
            }

            return null;
        }

        public static TrakHoundApiControllerInformation CreateControllerInformation(ITrakHoundApiConfiguration configuration, Type controllerType)
        {
            if (configuration != null && controllerType != null)
            {
                string route = Url.PathSeparator.ToString();
                string name = controllerType.Name;
                string description = null;

                var controllerAttribute = controllerType.GetCustomAttribute<TrakHoundApiControllerAttribute>();
                if (controllerAttribute != null)
                {
                    if (!string.IsNullOrEmpty(controllerAttribute.Route)) route = controllerAttribute.Route;
                    if (!string.IsNullOrEmpty(controllerAttribute.Name)) name = controllerAttribute.Name;
                    if (!string.IsNullOrEmpty(controllerAttribute.Description)) description = controllerAttribute.Description;
                }

                var information = new TrakHoundApiControllerInformation();
                information.Id = $"{configuration.Id}:{controllerType.Name}".ToMD5Hash();
                information.Route = route;
                information.Name = name;
                information.Description = description;
                information.EndPoints = GetEndpointInformation(configuration, controllerType, configuration.Route);

                return information;
            }

            return null;
        }

        public static IEnumerable<TrakHoundApiEndpointInformation> GetEndpointInformation(ITrakHoundApiConfiguration configuration, Type controllerType, string route = null)
        {
            var endpoints = new List<TrakHoundApiEndpointInformation>();

            try
            {
                var methods = controllerType.GetMethods();
                if (!methods.IsNullOrEmpty())
                {
                    foreach (var method in methods)
                    {
                        if (!method.IsConstructor)
                        {
                            // Process Route Attribute
                            var routeAttribute = GetRouteAttribute(method);
                            if (routeAttribute != null)
                            {
                                var endpoint = new TrakHoundApiEndpointInformation();
                                endpoint.Id = $"{routeAttribute.Type.ToString()}:{routeAttribute.Route}".ToMD5Hash();
                                endpoint.Type = routeAttribute.Type.ToString();
                                endpoint.Route = !string.IsNullOrEmpty(routeAttribute.Route) ? routeAttribute.Route : Url.PathSeparator.ToString();
                                endpoint.Description = routeAttribute.Description;

                                // Set Group
                                var groupAttribute = method.GetCustomAttribute<TrakHoundApiGroupAttribute>();
                                if (groupAttribute != null && !string.IsNullOrEmpty(groupAttribute.Name))
                                {
                                    endpoint.Group = groupAttribute.Name;
                                }

                                var parameterAttributes = method.GetCustomAttributes<TrakHoundApiParameterAttribute>();

                                // Set Parameters
                                var methodParameters = method.GetParameters();
                                if (!methodParameters.IsNullOrEmpty())
                                {
                                    var endpointParameters = new List<TrakHoundApiParameterInformation>();

                                    foreach (var methodParameter in methodParameters)
                                    {
                                        var endpointParameterType = GetParameterType(methodParameter);
                                        if (endpointParameterType >= 0)
                                        {
                                            var endpointParameter = new TrakHoundApiParameterInformation();
                                            endpointParameter.Name = methodParameter.Name;
                                            endpointParameter.Type = ((TrakHoundApiParameterType)endpointParameterType).ToString();
                                            endpointParameter.DataType = GetParameterDataType(methodParameter);
                                            endpointParameter.Required = methodParameter.IsOptional;
                                            endpointParameter.DefaultValue = methodParameter.DefaultValue?.ToString();

                                            // Add Parameter Description from TrakHoundApiParameterAttribute
                                            var parameterAttribute = parameterAttributes?.FirstOrDefault(o => o.Name == methodParameter.Name);
                                            if (parameterAttribute != null)
                                            {
                                                endpointParameter.Description = parameterAttribute.Description;
                                            }

                                            endpointParameters.Add(endpointParameter);
                                        }
                                    }

                                    endpoint.Parameters = endpointParameters;
                                }


                                // Add Responses
                                var responseAttributes = method.GetCustomAttributes<TrakHoundApiResponseAttribute>();
                                if (!responseAttributes.IsNullOrEmpty())
                                {
                                    var responses = new List<TrakHoundApiResponseInformation>();

                                    foreach (var responseAttribute in responseAttributes)
                                    {
                                        var response = TrakHoundApiResponseInformation.Create(responseAttribute.StatusCode, responseAttribute.ReturnType);
                                        if (response != null) responses.Add(response);
                                    }

                                    endpoint.Responses = responses;
                                }

                                // Add Permissions
                                var permissionAttributes = method.GetCustomAttributes<TrakHoundPermissionAttribute>();
                                if (!permissionAttributes.IsNullOrEmpty())
                                {
                                    var endpointPermissions = new List<string>();
                                    foreach (var permissionAttribute in permissionAttributes)
                                    {
                                        if (!string.IsNullOrEmpty(permissionAttribute.Permission))
                                        {
                                            endpointPermissions.Add(permissionAttribute.Permission);
                                        }
                                    }
                                    endpoint.Permissions = endpointPermissions;
                                }

                                endpoints.Add(endpoint);
                            }
                        }
                    }
                }
            }
            catch { }

            return endpoints;
        }


        private static TrakHoundApiEndpointRouteAttribute GetRouteAttribute(MethodInfo method)
        {
            if (method != null)
            {
                var queryAttribute = method.GetCustomAttribute<TrakHoundApiQueryAttribute>();
                if (queryAttribute != null) return queryAttribute;

                var subscribeAttribute = method.GetCustomAttribute<TrakHoundApiSubscribeAttribute>();
                if (subscribeAttribute != null) return subscribeAttribute;

                var publishAttribute = method.GetCustomAttribute<TrakHoundApiPublishAttribute>();
                if (publishAttribute != null) return publishAttribute;

                var deleteAttribute = method.GetCustomAttribute<TrakHoundApiDeleteAttribute>();
                if (deleteAttribute != null) return deleteAttribute;
            }

            return null;
        }

        private static int GetParameterType(ParameterInfo methodParameter)
        {
            if (methodParameter.GetCustomAttribute<FromRouteAttribute>() != null)
            {
                return (int)TrakHoundApiParameterType.Route;
            }
            else if (methodParameter.GetCustomAttribute<FromQueryAttribute>() != null)
            {
                return (int)TrakHoundApiParameterType.Query;
            }
            else if (methodParameter.GetCustomAttribute<FromBodyAttribute>() != null)
            {
                return (int)TrakHoundApiParameterType.Body;
            }

            return -1;
        }

        private static string GetParameterDataType(ParameterInfo methodParameter)
        {
            var type = methodParameter.ParameterType;

            // Handle Nullable<T> Type
            if (type.IsValueType && Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            // Handle Array Type
            if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                type = type.GetGenericArguments()?.FirstOrDefault();

                return type != null ? $"{type?.Name.ToTitleCase()}[]" : null;
            }

            return type?.Name.ToTitleCase();
        }
    }
}
