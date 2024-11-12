// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Functions;

namespace TrakHound.Clients
{
    public class TrakHoundFunctionsClient : ITrakHoundFunctionsClient
    {
		private const string _apiRoute = "functions";

		private readonly ITrakHoundClient _client;


		public TrakHoundFunctionsClient(ITrakHoundClient client)
		{
			_client = client;
		}


        public async Task<IEnumerable<TrakHoundFunctionInformation>> List()
        {
			var route = Url.Combine(_apiRoute, "list");
            return await _client.Api.QueryJson<IEnumerable<TrakHoundFunctionInformation>>(route);
        }

        public async Task<TrakHoundFunctionInformation> GetByFunctionId(string functionId)
		{
			var route = TrakHoundPath.Combine(_apiRoute, functionId);
			return await _client.Api.QueryJson<TrakHoundFunctionInformation>(route);
		}

		public async Task<IEnumerable<TrakHoundFunctionInformation>> GetByFunctionId(IEnumerable<string> functionIds)
		{
			return null;
		}


		public async Task<TrakHoundFunctionResponse> Run(string functionId, IReadOnlyDictionary<string, string> inputParameters = null, string runId = null, DateTime? timestamp = null)
		{
            var route = TrakHoundPath.Combine(_apiRoute, functionId, "run");

            var queryParameters = new Dictionary<string, string>();
            queryParameters.Add("runId", runId);
            if (timestamp != null) queryParameters.Add("timestamp", timestamp.Value.ToUnixTime().ToString());

            var jsonResponse = await _client.Api.QueryJson<TrakHoundFunctionJsonResponse>(route, inputParameters, queryParameters: queryParameters);
			return jsonResponse != null ? jsonResponse.ToResponse() : new TrakHoundFunctionResponse();
        }
    }
}
