// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrakHound.Functions;
using TrakHound.Http;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemFunctionsClient : ITrakHoundSystemFunctionsClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;


        public TrakHoundHttpSystemFunctionsClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundFunctionInformation>> GetInformation()
        {
            var url = Url.Combine(BaseUrl, "_functions/information");
            return await RestRequest.Get<IEnumerable<TrakHoundFunctionInformation>>(url);
        }

        public async Task<TrakHoundFunctionInformation> GetInformation(string apiId)
        {
            var url = Url.Combine(BaseUrl, "_functions/information");
            url = Url.Combine(url, apiId);

            return await RestRequest.Get<TrakHoundFunctionInformation>(url);
        }

        public async Task<IEnumerable<TrakHoundFunctionInformation>> GetInformation(IEnumerable<string> functionIds)
        {
            var url = Url.Combine(BaseUrl, "_functions/information");
            return await RestRequest.Post<IEnumerable<TrakHoundFunctionInformation>>(url, functionIds);
        }


        public async Task<TrakHoundFunctionResponse> Run(string functionId, IReadOnlyDictionary<string, string> inputParameters = null, string runId = null, long timestamp = 0)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.FunctionsPrefix);
            url = Url.Combine(url, "run");
            url = Url.Combine(url, functionId);
            url = Url.AddQueryParameter(url, "runId", runId);
            if (timestamp > 0) url = Url.AddQueryParameter(url, "timestamp", timestamp);

            var httpResponse = await RestRequest.PostResponse(url, inputParameters);

            var runResponse = new TrakHoundFunctionResponse();
            runResponse.StatusCode = httpResponse.StatusCode;

            if (httpResponse.Content != null)
            {
                var json = Encoding.UTF8.GetString(httpResponse.Content);
                runResponse.AddJsonParameter("object", json);
            }
            
            return runResponse;
        }
    }
}
