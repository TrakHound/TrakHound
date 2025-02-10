// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers
{
    public interface IBlobPublishDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<bool>> Publish(string blobId, Stream content, IReadOnlyDictionary<string, string> metadata = null);
    }
}
