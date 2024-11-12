// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Entities
{
    public class TrakHoundHttpDefinitionQueryResult
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }


        public TrakHoundHttpDefinitionQueryResult() { }

        public TrakHoundHttpDefinitionQueryResult(string query, string uuid)
        {
            Query = query;
            Uuid = uuid;
        }


        public ITrakHoundDefinitionQueryResult ToQueryResult()
        {
            var result = new TrakHoundDefinitionQueryResult();
            result.Query = Query;
            result.Uuid = Uuid;
            return result;
        }
    }
}
