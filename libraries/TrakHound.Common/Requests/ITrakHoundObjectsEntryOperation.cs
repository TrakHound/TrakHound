// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Requests
{
    public interface ITrakHoundObjectsEntryOperation : ITrakHoundEntityEntryOperation
    {

        string AssemblyDefinitionId { get; set; }
    }
}
