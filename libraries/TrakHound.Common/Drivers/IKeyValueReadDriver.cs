// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers
{
    public interface IKeyValueReadDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<TrakHoundEntityKeyValueResponse>> Read(string collection, IEnumerable<string> keys);
    }
}
