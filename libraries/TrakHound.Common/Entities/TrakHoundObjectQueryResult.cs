// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public class TrakHoundObjectQueryResult : ITrakHoundObjectQueryResult
    {
        public string Uuid { get; set; }

        public TrakHoundObjectQueryRequestType QueryType { get; set; }

        public string Namespace { get; set; }

        public string Query { get; set; }

        public int ParentLevel { get; set; }

        public string ParentUuid { get; set; }

        public string RequestedParentUuid { get; set; }

        public bool IsRoot { get; set; }

        public long Skip { get; set; }

        public long Take { get; set; }

        public SortOrder SortOrder { get; set; }


        public TrakHoundObjectQueryResult() { }

        public TrakHoundObjectQueryResult(
            TrakHoundObjectQueryRequestType queryType,
            string ns,
            string query,
            string uuid
            )
        {
            QueryType = queryType;
            Namespace = ns;
            Query = query;
            Uuid = uuid;
        }

        public TrakHoundObjectQueryResult(
            TrakHoundObjectQueryRequestType queryType,
            string ns,
            string query,
            string uuid,
            int parentLevel,
            string parentUuid,
            string requestedParentUuid,
            bool isRoot
            )
        {
            QueryType = queryType;
            Namespace = ns;
            Query = query;
            Uuid = uuid;
            ParentLevel = parentLevel;
            ParentUuid = parentUuid;
            RequestedParentUuid = requestedParentUuid;
            IsRoot = isRoot;
        }


        public override string ToString()
        {
            if (!string.IsNullOrEmpty(ParentUuid)) return $"{Namespace}::{QueryType}::{Query}::{Uuid}::{ParentLevel}::{ParentUuid}::{RequestedParentUuid}::{IsRoot}::{Skip}::{Take}::{(int)SortOrder}";
            else return $"{Namespace}::{QueryType}::{Query}::{Uuid}::{Skip}::{Take}::{(int)SortOrder}";
        }
    }
}
