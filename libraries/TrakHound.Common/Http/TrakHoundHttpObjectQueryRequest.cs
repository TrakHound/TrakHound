// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpObjectQueryRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("queries")]
        public IEnumerable<string> Queries { get; set; }

        [JsonPropertyName("parentLevel")]
        public int ParentLevel { get; set; }

        [JsonPropertyName("parentUuids")]
        public IEnumerable<string> ParentUuids { get; set; }


        public TrakHoundHttpObjectQueryRequest() { }

        public TrakHoundHttpObjectQueryRequest(TrakHoundObjectQueryRequest request)
        {
            Type = request.Type.ToString();
            Namespace = request.Namespace;
            Queries = request.Queries;
            ParentLevel = request.ParentLevel;
            ParentUuids = request.ParentUuids;
        }


        public TrakHoundObjectQueryRequest ToRequest()
        {
            var request = new TrakHoundObjectQueryRequest();
            request.Type = Type.ConvertEnum<TrakHoundObjectQueryRequestType>();
            request.Namespace = Namespace;
            request.Queries = Queries;
            request.ParentLevel = ParentLevel;
            request.ParentUuids = ParentUuids;
            return request;
        }
    }
}
