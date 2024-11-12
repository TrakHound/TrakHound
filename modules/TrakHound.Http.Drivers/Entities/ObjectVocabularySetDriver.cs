// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectVocabularySetDriver : 
        HttpEntityDriver<ITrakHoundObjectVocabularySetEntity>, 
        IObjectVocabularySetQueryDriver
    {
        public ObjectVocabularySetDriver() { }

        public ObjectVocabularySetDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>> QueryByObjectUuid(IEnumerable<string> objectUuids, long skip, long take, SortOrder sortOrder)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectVocabularySetEntity>>();

            if (!objectUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.VocabularySet.QueryByObjectUuid(objectUuids, skip, take, sortOrder);
                var dEntities = entities?.ToListDictionary(o => o.ObjectUuid);

                foreach (var objectUuid in objectUuids)
                {
                    var targetEntities = dEntities?.Get(objectUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectVocabularySetEntity>(Id, objectUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectVocabularySetEntity>(Id, objectUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectVocabularySetEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>(results);
        }
    }
}
