// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class SourceMetadataDriver : HttpEntityDriver<ITrakHoundSourceMetadataEntity>, ISourceMetadataQueryDriver
    {
        public SourceMetadataDriver() { }

        public SourceMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>> Query(IEnumerable<string> sourceUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundSourceMetadataEntity>>();

            if (!sourceUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Sources.Metadata.QueryBySourceUuid(sourceUuids);
                var dEntities = entities?.ToListDictionary(o => o.SourceUuid);

                foreach (var sourceUuid in sourceUuids)
                {
                    var targetEntities = dEntities?.Get(sourceUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, sourceUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, sourceUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundSourceMetadataEntity>(results);
        }
    }
}
