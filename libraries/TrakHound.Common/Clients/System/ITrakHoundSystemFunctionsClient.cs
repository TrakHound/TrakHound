// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Functions;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemFunctionsClient
    {
        Task<IEnumerable<TrakHoundFunctionInformation>> GetInformation();

        Task<TrakHoundFunctionInformation> GetInformation(string functionId);

        Task<IEnumerable<TrakHoundFunctionInformation>> GetInformation(IEnumerable<string> functionIds);


        Task<TrakHoundFunctionResponse> Run(string functionId, IReadOnlyDictionary<string, string> inputParameters = null, string runId = null, long timestamp = 0);
    }
}
