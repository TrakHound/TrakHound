// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundObjectQueryResult
    {
        string Uuid { get; }

        TrakHoundObjectQueryRequestType QueryType { get; }

        string Namespace { get; }

        string Query { get; }

        int ParentLevel { get; }

        string ParentUuid { get; }

        string RequestedParentUuid { get; }

        bool IsRoot { get; }

        long Skip { get; }

        long Take { get; }

        SortOrder SortOrder { get; }
    }
}
