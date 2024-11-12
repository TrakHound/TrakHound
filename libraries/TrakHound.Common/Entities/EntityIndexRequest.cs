// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public struct EntityIndexRequest
    {
        public string Target { get; set; }

        public TrakHoundIndexQueryType QueryType { get; set; }

        public string Value { get; set; }


        public EntityIndexRequest() { }

        public EntityIndexRequest(string target, TrakHoundIndexQueryType queryType, string value)
        {
            Target = target;
            QueryType = queryType;
            Value = value;
        }
    }
}
