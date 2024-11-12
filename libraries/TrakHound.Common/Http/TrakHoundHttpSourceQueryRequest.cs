// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpSourceQueryRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("queries")]
        public IEnumerable<string> Queries { get; set; }

        [JsonPropertyName("parentLevel")]
        public int ParentLevel { get; set; }

        [JsonPropertyName("parentUuids")]
        public IEnumerable<string> ParentUuids { get; set; }


        public TrakHoundHttpSourceQueryRequest() { }

        public TrakHoundHttpSourceQueryRequest(TrakHoundSourceQueryRequest request)
        {
            Type = request.Type.ToString();
            Queries = request.Queries;
            ParentLevel = request.ParentLevel;
            ParentUuids = request.ParentUuids;
        }


        public TrakHoundSourceQueryRequest ToRequest()
        {
            var request = new TrakHoundSourceQueryRequest();
            request.Type = Type.ConvertEnum<TrakHoundSourceQueryRequestType>();
            request.Queries = Queries;
            request.ParentLevel = ParentLevel;
            request.ParentUuids = ParentUuids;
            return request;
        }
    }
}
