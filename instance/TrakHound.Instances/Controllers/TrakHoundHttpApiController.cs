// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Instances;

namespace TrakHound.Http
{
    public class TrakHoundHttpApiController : ControllerBase
    {
        public const string RoutePrefix = "api-endpoint";

        public const string SubscribeSuffix = "subscribe";
        public const string PublishSuffix = "publish";
        public const string DeleteSuffix = "delete";

        private readonly ITrakHoundInstance _server;
        private readonly TrakHoundHttpWebSocketManager _webSocketManager;


        public TrakHoundHttpApiController(ITrakHoundInstance server, TrakHoundHttpWebSocketManager webSocketManager)
        {
            _server = server;
            _webSocketManager = webSocketManager;
        }

        public async Task<IActionResult> GetByRoute()
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                var queryParameters = new Dictionary<string, string>();
                if (!Request.Query.IsNullOrEmpty())
                {
                    foreach (var queryParameter in Request.Query)
                    {
                        queryParameters.Add(queryParameter.Key, queryParameter.Value);
                    }
                }

                var requestContentType = Request.Headers.ContentType;

                Stream requestBody = null;
                if (Request.Body != null)
                {
                    requestBody = new MemoryStream();
                    await Request.Body.CopyToAsync(requestBody);
                    requestBody.Seek(0, SeekOrigin.Begin);
                }

                var httpMethod = Request.Method;
                var requestRoute = Request.Path.Value.TrimStart('/');
                requestRoute = TrakHound.Url.RemoveFirstFragment(requestRoute); // Remove "api-endpoint" prefix

                var response = new TrakHoundApiResponse();
                if (IsQueryRequest(httpMethod, requestRoute))
                {
                    response = await client.Api.Query(requestRoute, requestBody, requestContentType, queryParameters);
                }
                else if (IsSubscribeRequest(httpMethod, requestRoute))
                {
                    var subsribeRequest = TrakHound.Url.RemoveLastFragment(requestRoute); // remove 'subscribe' suffix
                    await Subscribe(subsribeRequest, requestBody, requestContentType, queryParameters);
                }
                else if (IsPublishRequest(httpMethod, requestRoute))
                {
                    var publishRequest = TrakHound.Url.RemoveLastFragment(requestRoute); // remove 'publish' suffix
                    response = await client.Api.Publish(publishRequest, requestBody, requestContentType, queryParameters);
                }
                else if (IsDeleteRequest(httpMethod, requestRoute))
                {
                    var deleteRequest = requestRoute.EndsWith(DeleteSuffix) ? TrakHound.Url.RemoveLastFragment(requestRoute) : requestRoute; // remove 'delete' suffix
                    response = await client.Api.Delete(deleteRequest, requestBody, requestContentType, queryParameters);
                }

                // Dispose of Request Body Stream
                if (requestBody != null) requestBody.Dispose();

                if (response.IsValid())
                {
                    //// Set Parameters as Headers
                    //if (!response.Parameters.IsNullOrEmpty())
                    //{
                    //    foreach (var parameter in response.Parameters)
                    //    {
                    //        Response.Headers.Add(parameter.Key, parameter.Value);
                    //    }
                    //}

                    if (response.ContentType == null || MimeTypes.IsText(response.ContentType))
                    {
                        return new TrakHoundApiHttpResult(response);
                    }
                    else
                    {
                        if (!MimeTypes.IsAttachment(response.ContentType))
                        {
                            return File(response.Content, response.ContentType);
                        }
                        else
                        {
                            return File(response.Content, response.ContentType, response.GetParameter("Filename"));
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        //public async Task<IActionResult> GetByPackage(string packageId)
        //{
        //    var client = _server.ClientProvider.GetClient();
        //    if (client != null && !string.IsNullOrEmpty(packageId))
        //    {
        //        string packageVersion = null;

        //        var queryParameters = new Dictionary<string, string>();
        //        if (!Request.Query.IsNullOrEmpty())
        //        {
        //            foreach (var queryParameter in Request.Query)
        //            {
        //                if (queryParameter.Key == "packageVersion") packageVersion = queryParameter.Value;
        //                else queryParameters.Add(queryParameter.Key, queryParameter.Value);
        //            }
        //        }

        //        var requestContentType = Request.Headers.ContentType;

        //        byte[] body = null;
        //        if (Request.Body != null)
        //        {
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                await Request.Body.CopyToAsync(memoryStream);
        //                body = memoryStream.ToArray();
        //            }
        //        }


        //        var httpMethod = Request.Method;
        //        var requestPath = TrakHound.Url.RemoveFirstFragment(Request.Path); // Remove "/api" prefix
        //        requestPath = TrakHound.Url.RemoveFirstFragment(requestPath); // Remove "/{packageId}" prefix

        //        var response = new TrakHoundApiResponse();
        //        if (IsQueryRequest(httpMethod, requestPath))
        //        {
        //            response = await client.System.Api.Query(packageId, packageVersion, requestPath, body, requestContentType, queryParameters);
        //        }
        //        else if (IsSubscribeRequest(httpMethod, requestPath))
        //        {
        //            var subsribeRequest = TrakHound.Url.RemoveLastFragment(requestPath); // remove 'subscribe' suffix



        //            //return await Subscribe(subsribeRequest, body, requestContentType, queryParameters); // NEED TO UPDATE FOR PACKAGE ID



        //        }
        //        else if (IsPublishRequest(httpMethod, requestPath))
        //        {
        //            var publishRequest = TrakHound.Url.RemoveLastFragment(requestPath); // remove 'publish' suffix
        //            response = await client.System.Api.Publish(packageId, packageVersion, publishRequest, body, requestContentType, queryParameters);
        //        }
        //        //else if (IsDeleteRequest(httpMethod, requestPath))
        //        //{
        //        //    var deleteRequest = TrakHound.Url.RemoveLastFragment(requestPath); // remove 'delete' suffix
        //        //    response = await client.Api.Delete(packageId, packageVersion, deleteRequest, body, requestContentType, queryParameters);
        //        //}

        //        if (response.IsValid())
        //        {
        //            if (response.ContentType == null || MimeTypes.IsText(response.ContentType))
        //            {
        //                if (response.Content != null)
        //                {
        //                    // Add Response Parameters as Headers
        //                    if (!response.Parameters.IsNullOrEmpty())
        //                    {
        //                        foreach (var parameter in response.Parameters)
        //                        {
        //                            if (!string.IsNullOrEmpty(parameter.Key) && !string.IsNullOrEmpty(parameter.Value))
        //                            {
        //                                Response.Headers.Add($"{HttpConstants.ApiParameterHeaderPrefix}-{parameter.Key}", parameter.Value);
        //                            }
        //                        }
        //                    }

        //                    byte[] requestBody = null;
        //                    if (response.Content != null)
        //                    {
        //                        using (var readStream = new MemoryStream())
        //                        {
        //                            await response.Content.CopyToAsync(readStream);
        //                            requestBody = readStream.ToArray();
        //                        }
        //                    }
        //                    var contentBytes = System.Text.Encoding.ASCII.GetString(requestBody);
        //                    var content = Content(contentBytes, response.ContentType);
        //                    content.StatusCode = response.StatusCode;
        //                    return content;
        //                }
        //                else if (response.Success)
        //                {
        //                    return Ok();
        //                }
        //                else
        //                {
        //                    return StatusCode(response.StatusCode);
        //                }
        //            }
        //            else
        //            {
        //                return File(response.Content, response.ContentType, response.GetParameter("filename"));
        //            }
        //        }
        //        else
        //        {
        //            return StatusCode(500, "No Response Returned from API");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        private async Task Subscribe(string route, Stream requestBody, string contentType, Dictionary<string, string> queryParameters)
        {
            var getConsumer = async (Stream requestBody) =>
            {
                var client = _server.ClientProvider.GetClient();
                if (client != null)
                {
                    return await client.Api.Subscribe(route, requestBody, contentType, queryParameters);
                }
                return null;
            };

            var formatResponse = (TrakHoundApiResponse response) =>
            {
                var jsonResponse = new TrakHoundApiJsonResponse(response);
                var json = Json.Convert(jsonResponse);
                return json.ToUtf8Bytes();
            };

            await _webSocketManager.Create<TrakHoundApiResponse>(HttpContext, getConsumer, formatResponse);
        }

        public IActionResult GetRouteInformations()
        {
            var information = _server.ApiProvider.GetRouteInformation();
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }

        public IActionResult GetPackageInformations()
        {
            var information = _server.ApiProvider.GetPackageInformation();
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }

        public IActionResult GetInformation(string apiId)
        {
            var information = _server.ApiProvider.GetInformation(apiId);
            if (information != null)
            {
                return Ok(information);
            }

            return NotFound();
        }



        public static bool IsQueryRequest(string httpMethod, string uri)
        {
            if (!string.IsNullOrEmpty(httpMethod) && !string.IsNullOrEmpty(uri))
            {
                var method = httpMethod.ToUpper();
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                if (method == "GET" || method == "POST")
                {
                    return lastFragment != SubscribeSuffix && lastFragment != PublishSuffix && lastFragment != DeleteSuffix;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public static bool IsSubscribeRequest(string httpMethod, string uri)
        {
            if (!string.IsNullOrEmpty(httpMethod) && !string.IsNullOrEmpty(uri))
            {
                var method = httpMethod.ToUpper();
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                if (method == "GET" || method == "POST")
                {
                    return lastFragment == SubscribeSuffix;
                }
            }

            return false;
        }

        public static bool IsPublishRequest(string httpMethod, string uri)
        {
            if (!string.IsNullOrEmpty(httpMethod) && !string.IsNullOrEmpty(uri))
            {
                var method = httpMethod.ToUpper();
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                if (method == "PUT" || method == "POST")
                {
                    return lastFragment == PublishSuffix;
                }
            }

            return false;
        }

        public static bool IsDeleteRequest(string httpMethod, string uri)
        {
            if (!string.IsNullOrEmpty(httpMethod) && !string.IsNullOrEmpty(uri))
            {
                var method = httpMethod.ToUpper();
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                if (method == "DELETE")
                {
                    return true;
                }
                else if (method == "POST")
                {
                    return lastFragment == DeleteSuffix;
                }
            }

            return false;
        }
    }
}
