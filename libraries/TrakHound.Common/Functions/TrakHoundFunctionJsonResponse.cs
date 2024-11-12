// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Functions
{
    public class TrakHoundFunctionJsonResponse
    {
        public string Id { get; set; }

        public string PackageId { get; set; }

        public string PackageVersion { get; set; }

        public string EngineId { get; set; }

        public int StatusCode { get; set; }

        public long Started { get; set; }

        public long Completed { get; set; }

        public Dictionary<string, string> Parameters { get; set; }


        public TrakHoundFunctionJsonResponse() { }

        public TrakHoundFunctionJsonResponse(TrakHoundFunctionResponse response)
        {
            Id = response.Id;
            PackageId = response.PackageId;
            PackageVersion = response.PackageVersion;
            EngineId = response.EngineId;
            StatusCode = response.StatusCode;
            Started = response.Started;
            Completed = response.Completed;
            Parameters = response.Parameters?.ToDictionary(o => o.Key, o=> o.Value);
        }


        public TrakHoundFunctionResponse ToResponse()
        {
            var response = new TrakHoundFunctionResponse();
            response.Id = Id;
            response.PackageId = PackageId;
            response.PackageVersion = PackageVersion;
            response.EngineId = EngineId;
            response.StatusCode = StatusCode;
            response.Started = Started;
            response.Completed = Completed;

            if (!Parameters.IsNullOrEmpty())
            {
                foreach (var parameter in Parameters)
                {
                    response.AddParameter(parameter.Key, parameter.Value);
                }
            }

            return response;
        }
    }
}
