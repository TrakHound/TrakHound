// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Functions;

namespace TrakHound.Clients
{
    public interface ITrakHoundFunctionsClient
    {
        Task<IEnumerable<TrakHoundFunctionInformation>> List();

        Task<TrakHoundFunctionInformation> GetByFunctionId(string functionId);

        Task<IEnumerable<TrakHoundFunctionInformation>> GetByFunctionId(IEnumerable<string> functionIds);


        Task<TrakHoundFunctionResponse> Run(string functionId, IReadOnlyDictionary<string, string> inputParameters = null, string runId = null, DateTime? timestamp = null);
    }
}
