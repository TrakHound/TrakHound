// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class SourceDriver : HttpEntityDriver<ITrakHoundSourceEntity>, ISourceQueryDriver
    {
        public SourceDriver() { }

        public SourceDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundSourceQueryResult>>();

            var queryResults = await Client.System.Entities.Sources.QueryUuidChain(uuids);
            var dQueryResults = queryResults?.ToListDictionary(o => o.Query);

            if (!queryResults.IsNullOrEmpty())
            {
                foreach (var uuid in uuids)
                {
                    var targetQueryResults = dQueryResults?.Get(uuid);
                    if (!targetQueryResults.IsNullOrEmpty())
                    {
                        foreach (var targetQueryResult in targetQueryResults)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundSourceQueryResult>(Id, uuid, TrakHoundResultType.Ok, targetQueryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundSourceQueryResult>(Id, uuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                foreach (var uuid in uuids)
                {
                    results.Add(new TrakHoundResult<ITrakHoundSourceQueryResult>(Id, uuid, TrakHoundResultType.NotFound));
                }
            }

            return new TrakHoundResponse<ITrakHoundSourceQueryResult>(results);
        }
    }
}
