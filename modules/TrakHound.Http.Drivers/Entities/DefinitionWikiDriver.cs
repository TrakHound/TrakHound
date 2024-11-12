// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class DefinitionWikiDriver : HttpEntityDriver<ITrakHoundDefinitionWikiEntity>, IDefinitionWikiQueryDriver
    {
        public DefinitionWikiDriver() { }

        public DefinitionWikiDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundDefinitionWikiEntity>> Query(IEnumerable<string> definitionUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundDefinitionWikiEntity>>();

            if (!definitionUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Definitions.Wiki.QueryByDefinitionUuid(definitionUuids);
                var dEntities = entities?.ToListDictionary(o => o.DefinitionUuid);

                foreach (var definitionUuid in definitionUuids)
                {
                    var targetEntities = dEntities?.Get(definitionUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionWikiEntity>(Id, definitionUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionWikiEntity>(Id, definitionUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundDefinitionWikiEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundDefinitionWikiEntity>(results);
        }
    }
}
