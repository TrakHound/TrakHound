// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;

namespace TrakHound.Drivers
{
    public interface IBlobPublishDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<bool>> Publish(string blobId, Stream content);
    }
}
