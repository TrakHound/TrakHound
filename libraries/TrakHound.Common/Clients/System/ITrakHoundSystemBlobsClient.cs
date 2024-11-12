// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemBlobsClient
    {
        Task<Stream> Read(string blobId, string routerId = null);

        Task<bool> Publish(string blobId, Stream content, string routerId = null);

        Task<bool> Delete(string blobId, string routerId = null);
    }
}
