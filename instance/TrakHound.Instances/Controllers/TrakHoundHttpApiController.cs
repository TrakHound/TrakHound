// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
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

                var requestRoute = Request.Path.Value.TrimStart('/');
                requestRoute = TrakHound.Url.RemoveFirstFragment(requestRoute); // Remove "api-endpoint" prefix

                var response = new TrakHoundApiResponse();
                if (IsQueryRequest(Request, requestRoute))
                {
                    response = await client.Api.Query(requestRoute, requestBody, requestContentType, queryParameters);
                }
                else if (IsSubscribeRequest(Request, requestRoute))
                {
                    var subsribeRequest = TrakHound.Url.RemoveLastFragment(requestRoute); // remove 'subscribe' suffix
                    await Subscribe(subsribeRequest, requestBody, requestContentType, queryParameters);
                }
                else if (IsPublishRequest(Request, requestRoute))
                {
                    var publishRequest = TrakHound.Url.RemoveLastFragment(requestRoute); // remove 'publish' suffix
                    response = await client.Api.Publish(publishRequest, requestBody, requestContentType, queryParameters);
                }
                else if (IsPublishStreamRequest(Request, requestRoute))
                {
                    var publishRequest = TrakHound.Url.RemoveLastFragment(requestRoute); // remove 'publish' suffix
                    await CreatePublishStream(publishRequest, requestContentType, queryParameters);
                }
                else if (IsDeleteRequest(Request, requestRoute))
                {
                    var deleteRequest = requestRoute.EndsWith(DeleteSuffix) ? TrakHound.Url.RemoveLastFragment(requestRoute) : requestRoute; // remove 'delete' suffix
                    response = await client.Api.Delete(deleteRequest, requestBody, requestContentType, queryParameters);
                }

                // Dispose of Request Body Stream
                if (requestBody != null) requestBody.Dispose();

                if (response.IsValid())
                {
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

        private async Task CreatePublishStream(string route, string contentType, Dictionary<string, string> queryParameters)
        {
            var client = _server.ClientProvider.GetClient();
            if (client != null)
            {
                var consumer = new TrakHoundConsumer<byte[]>();
                consumer.Received += async (s, responseBody) =>
                {
                    await client.Api.Publish(route, responseBody, contentType, queryParameters);
                };

                var formatResponse = (byte[] response) =>
                {
                    return response;
                };

                await _webSocketManager.CreateClient<byte[]>(HttpContext, consumer, formatResponse);
            }
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



        public static bool IsQueryRequest(HttpRequest request, string uri)
        {
            if (request != null && !string.IsNullOrEmpty(uri))
            {
                var method = request.Method?.ToUpper();
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

        public static bool IsSubscribeRequest(HttpRequest request, string uri)
        {
            if (request != null && !string.IsNullOrEmpty(uri))
            {
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                return request.HttpContext.WebSockets.IsWebSocketRequest && lastFragment == SubscribeSuffix;
            }

            return false;
        }

        public static bool IsPublishRequest(HttpRequest request, string uri)
        {
            if (request != null && !string.IsNullOrEmpty(uri))
            {
                var method = request.Method?.ToUpper();
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                if (method == "PUT" || method == "POST")
                {
                    return !request.HttpContext.WebSockets.IsWebSocketRequest && lastFragment == PublishSuffix;
                }
            }

            return false;
        }

        public static bool IsPublishStreamRequest(HttpRequest request, string uri)
        {
            if (request != null && !string.IsNullOrEmpty(uri))
            {
                var lastFragment = TrakHound.Url.GetLastFragment(uri);

                return request.HttpContext.WebSockets.IsWebSocketRequest && lastFragment == PublishSuffix;
            }

            return false;
        }

        public static bool IsDeleteRequest(HttpRequest request, string uri)
        {
            if (request != null && !string.IsNullOrEmpty(uri))
            {
                var method = request.Method?.ToUpper();
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
