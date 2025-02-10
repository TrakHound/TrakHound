// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers
{
    public interface IKeyValuePublishDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<bool>> Publish(string collection, IReadOnlyDictionary<string, string> entries, TimeSpan? ttl = null);
    }
}
