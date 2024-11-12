// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public class TrakHoundDefinitionQueryResult : ITrakHoundDefinitionQueryResult
    {
        public string Query { get; set; }

        public string Uuid { get; set; }


        public TrakHoundDefinitionQueryResult() { }

        public TrakHoundDefinitionQueryResult(string query, string uuid)
        {
            Query = query;
            Uuid = uuid;
        }


        public override string ToString()
        {
            return $"{Query}::{Uuid}";
        }
    }
}
