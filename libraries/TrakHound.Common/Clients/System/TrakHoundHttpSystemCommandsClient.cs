// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Commands;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemCommandsClient : ITrakHoundSystemCommandsClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemCommandsClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundCommandResponse>> Run(string commandId, IReadOnlyDictionary<string, string> parameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.CommandsPrefix);
            url = Url.Combine(url, "run");
            url = Url.AddQueryParameter(url, "commandId", commandId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponses = await RestRequest.Post<IEnumerable<TrakHoundCommandJsonResponse>>(url, parameters);
            if (httpResponses.IsNullOrEmpty())
            {
                var responses = new List<TrakHoundCommandResponse>();
                foreach (var httpResponse in httpResponses)
                {
                    var runResponse = new TrakHoundCommandResponse();
                    runResponse.StatusCode = httpResponse.StatusCode;
                    runResponse.ContentType = httpResponse.ContentType;
                    runResponse.Content = TrakHoundCommandResponse.GetContentStreamFromBase64String(httpResponse.Content);
                    responses.Add(runResponse);
                }
                return responses;
            }

            return null;
        }
    }
}
