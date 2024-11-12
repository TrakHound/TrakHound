// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundSourceQueryResult
    {
        string Uuid { get; }

        TrakHoundSourceQueryRequestType QueryType { get; }

        string Query { get; }

        int ParentLevel { get; }

        string ParentUuid { get; }

        string RequestedParentUuid { get; }

        bool IsRoot { get; }
    }
}
