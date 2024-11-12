// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;

namespace TrakHound.Http.Drivers
{
    public class BlobDriver :
        HttpDriver,
        IBlobReadDriver,
        IBlobPublishDriver
    {
        public BlobDriver() { }

        public BlobDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<Stream>> Read(string blobId)
        {
            var results = new List<TrakHoundResult<Stream>>();

            if (!string.IsNullOrEmpty(blobId))
            {
                var stream = await Client.System.Blobs.Read(blobId);
                if (stream != null)
                {
                    results.Add(new TrakHoundResult<Stream>(Id, blobId, TrakHoundResultType.Ok, stream));
                }
                else
                {
                    results.Add(new TrakHoundResult<Stream>(Id, blobId, TrakHoundResultType.NotFound));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<Stream>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<Stream>(results);
        }

        public async Task<TrakHoundResponse<bool>> Publish(string blobId, Stream stream)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (!string.IsNullOrEmpty(blobId))
            {
                if (await Client.System.Blobs.Publish(blobId, stream))
                {
                    results.Add(new TrakHoundResult<bool>(Id, blobId, TrakHoundResultType.Ok, true));
                }
                else
                {
                    results.Add(new TrakHoundResult<bool>(Id, blobId, TrakHoundResultType.InternalError));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<bool>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<bool>(results);
        }
    }
}
