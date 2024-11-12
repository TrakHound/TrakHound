// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundSourcedEntity
    {
        void SetSource(string sourceUuid, bool overwrite = false);
    }
}
