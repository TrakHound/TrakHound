// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class DefinitionDriver : HttpEntityDriver<ITrakHoundDefinitionEntity>, IDefinitionQueryDriver
    {
        public DefinitionDriver() { }

        public DefinitionDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<string>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<string>>();

            var entities = await Client.System.Entities.Definitions.Query(pattern, skip, take, sortOrder);
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    results.Add(new TrakHoundResult<string>(Id, pattern, TrakHoundResultType.Ok, entity.Uuid));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<string>(Id, pattern, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<string>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            var queryResults = await Client.System.Entities.Definitions.QueryIdsByType(types, skip, take, sortOrder);
            var dQueryResults = queryResults?.ToListDictionary(o => o.Query);

            if (!queryResults.IsNullOrEmpty())
            {
                foreach (var type in types)
                {
                    var targetQueryResults = dQueryResults?.Get(type);
                    if (!targetQueryResults.IsNullOrEmpty())
                    {
                        foreach (var targetQueryResult in targetQueryResults)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.Ok, targetQueryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                foreach (var type in types)
                {
                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.NotFound));
                }
            }

            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            var queryResults = await Client.System.Entities.Definitions.QueryByParentUuid(parentUuids, skip, take, sortOrder);
            var dQueryResults = queryResults?.ToListDictionary(o => o.ParentUuid);

            if (!queryResults.IsNullOrEmpty())
            {
                foreach (var parentUuid in parentUuids)
                {
                    var targetQueryResults = dQueryResults?.Get(parentUuid);
                    if (!targetQueryResults.IsNullOrEmpty())
                    {
                        foreach (var targetQueryResult in targetQueryResults)
                        {
                            var queryResult = new TrakHoundDefinitionQueryResult(parentUuid, targetQueryResult.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                foreach (var parentUuid in parentUuids)
                {
                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.NotFound));
                }
            }

            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            var queryResults = await Client.System.Entities.Definitions.QueryByParentUuid(childUuids, skip, take, sortOrder);
            var dQueryResults = queryResults?.ToListDictionary(o => o.ParentUuid);

            if (!queryResults.IsNullOrEmpty())
            {
                foreach (var childUuid in childUuids)
                {
                    var targetQueryResults = dQueryResults?.Get(childUuid);
                    if (!targetQueryResults.IsNullOrEmpty())
                    {
                        foreach (var targetQueryResult in targetQueryResults)
                        {
                            var queryResult = new TrakHoundDefinitionQueryResult(childUuid, targetQueryResult.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                foreach (var childUuid in childUuids)
                {
                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
                }
            }

            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results);
        }
    }
}
