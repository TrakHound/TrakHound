// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public class TrakHoundSourceQueryResult : ITrakHoundSourceQueryResult
    {
        public string Uuid { get; set; }

        public TrakHoundSourceQueryRequestType QueryType { get; set; }

        public string Query { get; set; }

        public int ParentLevel { get; set; }

        public string ParentUuid { get; set; }

        public string RequestedParentUuid { get; set; }

        public bool IsRoot { get; set; }


        public TrakHoundSourceQueryResult() { }

        public TrakHoundSourceQueryResult(
            TrakHoundSourceQueryRequestType queryType,
            string query,
            string uuid
            )
        {
            QueryType = queryType;
            Query = query;
            Uuid = uuid;
        }

        public TrakHoundSourceQueryResult(
            TrakHoundSourceQueryRequestType queryType,
            string query,
            string uuid,
            int parentLevel,
            string parentUuid,
            string requestedParentUuid,
            bool isRoot
            )
        {
            QueryType = queryType;
            Query = query;
            Uuid = uuid;
            ParentLevel = parentLevel;
            ParentUuid = parentUuid;
            RequestedParentUuid = requestedParentUuid;
            IsRoot = isRoot;
        }


        public override string ToString()
        {
            if (!string.IsNullOrEmpty(ParentUuid)) return $"{QueryType}::{Query}::{Uuid}::{ParentLevel}::{ParentUuid}::{RequestedParentUuid}::{IsRoot}";
            else return $"{QueryType}::{Query}::{Uuid}";
        }
    }
}
