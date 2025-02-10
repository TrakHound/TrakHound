// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers
{
    public interface IBlobDeleteDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<bool>> Delete(string blobId);
    }
}
