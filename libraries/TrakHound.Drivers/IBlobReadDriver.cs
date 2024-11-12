// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;

namespace TrakHound.Drivers
{
    public interface IBlobReadDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<Stream>> Read(string blobId);
    }
}
