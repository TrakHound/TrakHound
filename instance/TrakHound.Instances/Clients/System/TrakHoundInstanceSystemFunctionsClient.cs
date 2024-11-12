// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Functions;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemFunctionsClient : ITrakHoundSystemFunctionsClient
    {
        private readonly TrakHoundFunctionManager _functionManager;


        public TrakHoundInstanceSystemFunctionsClient(TrakHoundFunctionManager functionManager)
        {
            _functionManager = functionManager;
        }


        public async Task<IEnumerable<TrakHoundFunctionInformation>> GetInformation()
        {
            return _functionManager.GetInformation();
        }

        public async Task<TrakHoundFunctionInformation> GetInformation(string functionId)
        {
            return _functionManager.GetInformation(functionId);
        }

        public async Task<IEnumerable<TrakHoundFunctionInformation>> GetInformation(IEnumerable<string> functionIds)
        {
            return _functionManager.GetInformation(functionIds);
        }


        public async Task<TrakHoundFunctionResponse> Run(string functionId, IReadOnlyDictionary<string, string> inputParameters = null, string runId = null, long timestamp = 0)
        {
            if (!string.IsNullOrEmpty(functionId))
            {
                return await _functionManager.Run(functionId, inputParameters, runId, timestamp);
            }

            return default;
        }
    }
}
