// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Entities
{
    public class TrakHoundHttpObjectQueryResult
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }


        public TrakHoundHttpObjectQueryResult() { }

        public TrakHoundHttpObjectQueryResult(string query, string uuid)
        {
            Query = query;
            Uuid = uuid;
        }


        public ITrakHoundObjectQueryResult ToQueryResult()
        {
            var result = new TrakHoundObjectQueryResult();
            result.Query = Query;
            result.Uuid = Uuid;
            return result;
        }
    }
}
