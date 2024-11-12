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


        public async Task<TrakHoundCommandResponse> Run(string commandId, IReadOnlyDictionary<string, string> parameters = null, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.CommandsPrefix);
            url = Url.Combine(url, "run");
            url = Url.AddQueryParameter(url, "commandId", commandId);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PostResponse(url, parameters);

            var runResponse = new TrakHoundCommandResponse();
            runResponse.StatusCode = httpResponse.StatusCode;
            runResponse.ContentType = httpResponse.ContentType;
            runResponse.Content = httpResponse.Content;

            return runResponse;
        }
    }
}
