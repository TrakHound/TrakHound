// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Requests
{
    public interface ITrakHoundEntityOperation
    {
        byte[] EntryId { get; }

        byte Category { get; }

        byte Class { get; }

        TrakHoundEntityOperationType OperationType { get; }
    }
}
