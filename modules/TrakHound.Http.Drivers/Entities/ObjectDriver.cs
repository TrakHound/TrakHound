// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectDriver :
        HttpEntityDriver<ITrakHoundObjectEntity>,
        IObjectQueryDriver,
        IObjectDeleteDriver
    {
        public ObjectDriver() { }

        public ObjectDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!queryRequest.Queries.IsNullOrEmpty())
            {
                foreach (var query in queryRequest.Queries)
                {
                    var entities = await Client.System.Entities.Objects.Query(queryRequest, skip, take, sortOrder);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, entity.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            var entities = await Client.System.Entities.Objects.QueryRoot(skip, take, sortOrder);
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, "All", entity.Uuid);
                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, "All", TrakHoundResultType.Ok, queryResult));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!namespaces.IsNullOrEmpty())
            {
                foreach (var ns in namespaces)
                {
                    var entities = await Client.System.Entities.Objects.QueryRoot(ns, skip, take, sortOrder);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, ns, ns, entity.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, ns, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, ns, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!parentUuids.IsNullOrEmpty())
            {
                foreach (var parentUuid in parentUuids)
                {
                    var entities = await Client.System.Entities.Objects.QueryByParentUuid(parentUuid, skip, take, sortOrder);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, entity.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!childUuids.IsNullOrEmpty())
            {
                foreach (var childUuid in childUuids)
                {
                    var entities = await Client.System.Entities.Objects.QueryByChildUuid(childUuid, skip, take, sortOrder);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, childUuid, entity.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!rootUuids.IsNullOrEmpty())
            {
                foreach (var rootUuid in rootUuids)
                {
                    var entities = await Client.System.Entities.Objects.QueryChildrenByRootUuid(rootUuid, skip, take, sortOrder);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, rootUuid, entity.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!childUuids.IsNullOrEmpty())
            {
                foreach (var childUuid in childUuids)
                {
                    var entities = await Client.System.Entities.Objects.QueryRootByChildUuid(childUuid, skip, take, sortOrder);
                    if (!entities.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, childUuid, entity.Uuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<string>> ListNamespaces(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<string>>();

            var namespaces = await Client.System.Entities.Objects.ListNamespaces(skip, take, sortOrder);
            if (!namespaces.IsNullOrEmpty())
            {
                foreach (var ns in namespaces)
                {
                    results.Add(new TrakHoundResult<string>(Id, ns, TrakHoundResultType.Ok, ns));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<string>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<string>(results);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids)
        {
            var results = new List<TrakHoundResult<TrakHoundCount>>();

            var counts = await Client.System.Entities.Objects.QueryChildCount(uuids);
            if (!counts.IsNullOrEmpty())
            {
                foreach (var count in counts)
                {
                    results.Add(new TrakHoundResult<TrakHoundCount>(Id, count.Target, TrakHoundResultType.Ok, count));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<TrakHoundCount>(Id, "All", TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<TrakHoundCount>(results);
        }


        public async Task<TrakHoundResponse<bool>> DeleteByRootUuid(IEnumerable<string> uuids)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (await Client.System.Entities.Objects.DeleteByRootUuid(uuids))
            {
                foreach (var uuid in uuids)
                {
                    results.Add(new TrakHoundResult<bool>(Id, uuid, TrakHoundResultType.Ok, true));
                }
            }
            else
            {
                foreach (var uuid in uuids)
                {
                    results.Add(new TrakHoundResult<bool>(Id, uuid, TrakHoundResultType.NotFound));
                }
            }

            return new TrakHoundResponse<bool>(results);
        }
    }
}
