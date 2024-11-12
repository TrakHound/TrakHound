// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundBlob> GetBlob(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundBlob>> GetBlobs(string objectPath, string routerId = null);

        Task<string> GetBlobString(string objectPath, string routerId = null);

        Task<byte[]> GetBlobBytes(string objectPath, string routerId = null);

        Task<Stream> GetBlobStream(string objectPath, string routerId = null);


        Task<bool> PublishBlobString(string objectPath, string content, string filename = null, bool async = false, string routerId = null);

        Task<bool> PublishBlobJson(string objectPath, string content, string filename = null, bool async = false, string routerId = null);

        Task<bool> PublishBlob(string objectPath, string content, string contentType = "text/plain", string filename = null, bool async = false, string routerId = null);
   
        Task<bool> PublishBlob(string objectPath, byte[] content, string contentType = "application/octet-stream", string filename = null, bool async = false, string routerId = null);

        Task<bool> PublishBlob(string objectPath, Stream stream, string contentType = "application/octet-stream", string filename = null, bool async = false, string routerId = null);


        Task<bool> DeleteBlob(string objectPath, string routerId = null);
    }
}
