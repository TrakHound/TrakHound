// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectDriver : MemoryEntityDriver<ITrakHoundObjectEntity>
        //IObjectQueryDriver,
        //IObjectQueryStoreDriver
    {
        private readonly ListDictionary<string, string> _parentChildren = new ListDictionary<string, string>(); // ParentUuid => ChildUuids
        private readonly ListDictionary<string, string> _rootChildren = new ListDictionary<string, string>(); // RootUuid => ChildUuids
        private readonly ListDictionary<string, string> _childRoots = new ListDictionary<string, string>(); // ChildUuid => RootUuids
        private readonly HashSet<string> _emptyParent = new HashSet<string>(); // ParentUuid
        private readonly HashSet<string> _emptyRoot = new HashSet<string>(); // RootUuid
        private readonly HashSet<string> _emptyChild = new HashSet<string>(); // ChildUuid

        private readonly ListDictionary<string, string> _queryReverseResults = new ListDictionary<string, string>(); // Uuid => QueryKeys
        private readonly ListDictionary<string, string> _queryParentReverseResults = new ListDictionary<string, string>(); // ParentKey => QueryKeys
        private readonly ListDictionary<string, string> _queryDefinitionReverseResults = new ListDictionary<string, string>(); // DefinitionUuid => QueryKeys
        private readonly ListDictionary<string, QueryResult> _queryResults = new ListDictionary<string, QueryResult>(); // QueryKey => QueryResult
        private readonly ListDictionary<string, string> _emptyQueryResults = new ListDictionary<string, string>(); // ParentKey => QueryKey

        private readonly ListDictionary<string, string> _pathResults = new ListDictionary<string, string>(); // Path => Uuids
        private readonly Dictionary<string, string> _pathReverseResults = new Dictionary<string, string>(); // Uuid => Path


        struct QueryResult
        {
            public string Uuid { get; set; }
            public string ParentUuid { get; set; }

            public QueryResult(string uuid, string parentUuid)
            {
                Uuid = uuid;
                ParentUuid = parentUuid;
            }
        }


        public ObjectDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            if (!queryRequest.Queries.IsNullOrEmpty())
            {
                foreach (var query in queryRequest.Queries)
                {
                    if (!queryRequest.ParentUuids.IsNullOrEmpty())
                    {
                        foreach (var requestedParentUuid in queryRequest.ParentUuids)
                        {
                            var isRoot = requestedParentUuid == null;
                            var parentKey = requestedParentUuid != null ? requestedParentUuid : "$ROOT$";

                            var queryKey = $"{queryRequest.Namespace}::{queryRequest.Type}::{query}::{queryRequest.ParentLevel}::{parentKey}::{skip}::{take}::{(int)sortOrder}";

                            var queryResults = _queryResults.Get(queryKey);
                            if (!queryResults.IsNullOrEmpty())
                            {
                                foreach (var queryResult in queryResults)
                                {
                                    var result = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, queryResult.Uuid, queryRequest.ParentLevel, queryResult.ParentUuid, requestedParentUuid, isRoot);
                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Ok, result, queryResult.Uuid));
                                }
                            }
                            else
                            {
                                var emptyResults = _emptyQueryResults.Get(parentKey);
                                if (emptyResults != null && emptyResults.Contains(queryKey))
                                {
                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Empty));
                                }
                                else
                                {
                                    results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.NotFound));
                                }
                            }
                        }
                    }
                    else
                    {
                        var parentKey = "$ROOT$";
                        var queryKey = $"{queryRequest.Namespace}::{queryRequest.Type}::{query}::{skip}::{take}::{(int)sortOrder}";

                        var queryResults = _queryResults.Get(queryKey);
                        if (!queryResults.IsNullOrEmpty())
                        {
                            foreach (var queryResult in queryResults)
                            {
                                var result = new TrakHoundObjectQueryResult(queryRequest.Type, queryRequest.Namespace, query, queryResult.Uuid);
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Ok, result, queryResult.Uuid));
                            }
                        }
                        else
                        {
                            var emptyResults = _emptyQueryResults.Get(parentKey);
                            if (emptyResults != null && emptyResults.Contains(queryKey))
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, query, TrakHoundResultType.NotFound));
                            }
                        }
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
            return TrakHoundResponse<ITrakHoundObjectQueryResult>.RouteNotConfigured(Id, "QueryRoot");
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<ITrakHoundObjectQueryResult>.RouteNotConfigured(Id, "QueryRoot");
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            lock (_lock)
            {
                if (!parentUuids.IsNullOrEmpty())
                {
                    foreach (var parentUuid in parentUuids)
                    {
                        var key = parentUuid != null ? parentUuid : "$ROOT$";

                        var childUuids = _parentChildren.Get(key);
                        if (!childUuids.IsNullOrEmpty())
                        {
                            foreach (var childUuid in childUuids)
                            {
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, parentUuid, childUuid);
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Ok, queryResult, childUuid));
                            }
                        }
                        else
                        {
                            if (_emptyParent.Contains(key))
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, parentUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                }              
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            lock (_lock)
            {
                if (!childUuids.IsNullOrEmpty())
                {
                    foreach (var childUuid in childUuids)
                    {
                        var childObject = _entities.GetValueOrDefault(childUuid);
                        if (childObject != null)
                        {
                            var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, childUuid, childObject.ParentUuid);
                            results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult, childObject.ParentUuid));
                        }
                        else
                        {
                            if (_emptyChild.Contains(childUuid))
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                }
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            lock (_lock)
            {
                if (!rootUuids.IsNullOrEmpty())
                {
                    foreach (var rootUuid in rootUuids)
                    {
                        var key = rootUuid != null ? rootUuid : "$ROOT$";

                        var childUuids = _rootChildren.Get(key);
                        if (!childUuids.IsNullOrEmpty())
                        {
                            foreach (var childUuid in childUuids)
                            {
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, rootUuid, childUuid);
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Ok, queryResult, childUuid));
                            }
                        }
                        else
                        {
                            if (_emptyRoot.Contains(key))
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, rootUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                }
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

            lock (_lock)
            {
                if (!childUuids.IsNullOrEmpty())
                {
                    foreach (var childUuid in childUuids)
                    {
                        var rootUuids = _childRoots.Get(childUuid);
                        if (!rootUuids.IsNullOrEmpty())
                        {
                            foreach (var rootUuid in rootUuids)
                            {
                                var queryResult = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, null, childUuid, rootUuid);
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult, rootUuid));
                            }
                        }
                        else
                        {
                            if (_emptyChild.Contains(childUuid))
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                }
            }

            return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        }


        public async Task<TrakHoundResponse<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, "QueryChildCount");
        }

        public async Task<TrakHoundResponse<TrakHoundCount>> QueryDescendantCount(IEnumerable<string> uuids)
        {
            return TrakHoundResponse<TrakHoundCount>.RouteNotConfigured(Id, "QueryDescendantCount");
        }


        public async Task<TrakHoundResponse<string>> ListNamespaces(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<string>.RouteNotConfigured(Id, "ListNamespaces");
        }


        //      public async Task<TrakHoundResponse<ITrakHoundObjectQueryResult>> QueryByPath(IEnumerable<string> paths)
        //      {
        //          var results = new List<TrakHoundResult<ITrakHoundObjectQueryResult>>();

        //          if (!paths.IsNullOrEmpty())
        //          {
        //              foreach (var path in paths)
        //              {
        //                  var uuids = _pathResults.Get(path);
        //                  if (!uuids.IsNullOrEmpty())
        //                  {
        //                      foreach (var uuid in uuids)
        //                      {
        //                          var result = new TrakHoundObjectQueryResult(TrakHoundObjectQueryRequestType.Uuid, path, uuid);
        //                          results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, path, TrakHoundResultType.Ok, result, uuid));
        //                      }
        //                  }
        //                  else
        //                  {
        //                      results.Add(new TrakHoundResult<ITrakHoundObjectQueryResult>(Id, path, TrakHoundResultType.NotFound));
        //                  }
        //              }
        //          }

        //          return new TrakHoundResponse<ITrakHoundObjectQueryResult>(results);
        //      }

        //      public async Task<TrakHoundResponse<bool>> PathExists(IEnumerable<string> paths)
        //{
        //	var results = new List<TrakHoundResult<bool>>();

        //          if (!paths.IsNullOrEmpty())
        //          {
        //              foreach (var path in paths)
        //              {
        //                  results.Add(new TrakHoundResult<bool>(Id, path, TrakHoundResultType.RouteNotConfigured));
        //              }
        //          }

        //	return new TrakHoundResponse<bool>(results);
        //}


        public async Task<TrakHoundResponse<bool>> StoreQuery(IEnumerable<ITrakHoundQueryRequestResult> queryResults)
        {
            return TrakHoundResponse<bool>.RouteNotConfigured(Id, "Query");
        }

        public async Task<TrakHoundResponse<bool>> StorePath(IEnumerable<ITrakHoundQueryRequestResult> queryResults)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (!queryResults.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var queryResult in queryResults)
                    {
                        if (!queryResult.TargetIds.IsNullOrEmpty())
                        {
                            foreach (var targetId in queryResult.TargetIds)
                            {
                                _pathResults.Remove(queryResult.Query);
                                _pathResults.Add(queryResult.Query, targetId);

                                _pathReverseResults.Remove(targetId);
                                _pathReverseResults.Add(targetId, queryResult.Query);
                            }
                        }
                    }
                }
            }

            return new TrakHoundResponse<bool>(results);
        }

        public async Task<TrakHoundResponse<bool>> StoreParents(IEnumerable<ITrakHoundObjectQueryResult> queryResults)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (!queryResults.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    var parentUuids = queryResults.Select(o => o.Query).Distinct();
                    foreach (var parentUuid in parentUuids)
                    {
                        var key = parentUuid != null ? parentUuid : "$ROOT$";

                        var parentResults = queryResults.Where(o => o.Query == parentUuid);
                        foreach (var parentResult in parentResults)
                        {
                            if (parentResult.Uuid != null)
                            {
                                _parentChildren.Add(key, parentResult.Uuid);
                            }
                            else
                            {
                                _emptyParent.Add(parentResult.Query);
                            }

                            results.Add(new TrakHoundResult<bool>(Id, parentUuid, TrakHoundResultType.Ok, true));
                        }
                    }
                }
            }

            return new TrakHoundResponse<bool>(results);
        }

        public async Task<TrakHoundResponse<bool>> StoreChildren(IEnumerable<ITrakHoundObjectQueryResult> queryResults)
        {
            return TrakHoundResponse<bool>.RouteNotConfigured(Id, "NONE");
        }

        public async Task<TrakHoundResponse<bool>> StoreChildrenByRoots(IEnumerable<ITrakHoundObjectQueryResult> queryResults)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (!queryResults.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    var rootUuids = queryResults.Select(o => o.Query).Distinct();
                    foreach (var rootUuid in rootUuids)
                    {
                        var key = rootUuid != null ? rootUuid : "$ROOT$";

                        var rootResults = queryResults.Where(o => o.Query == rootUuid);
                        foreach (var rootResult in rootResults)
                        {
                            if (rootResult.Uuid != null)
                            {
                                _rootChildren.Add(key, rootResult.Uuid);
                            }
                            else
                            {
                                _emptyRoot.Add(rootResult.Query);
                            }

                            results.Add(new TrakHoundResult<bool>(Id, rootUuid, TrakHoundResultType.Ok, true));
                        }
                    }
                }
            }

            return new TrakHoundResponse<bool>(results);
        }

        public async Task<TrakHoundResponse<bool>> StoreRootsByChildren(IEnumerable<ITrakHoundObjectQueryResult> queryResults)
        {
            var results = new List<TrakHoundResult<bool>>();

            if (!queryResults.IsNullOrEmpty())
            {
                var childUuids = queryResults.Select(o => o.Query).Distinct();
                var dQueryResults = queryResults.ToListDictionary(o => o.Query);

                lock (_lock)
                {
                    foreach (var childUuid in childUuids)
                    {
                        //var childResults = queryResults.Where(o => o.Query == childUuid);
                        var childResults = dQueryResults.Get(childUuid);
                        foreach (var childResult in childResults)
                        {
                            if (childResult.Uuid != null)
                            {
                                _childRoots.Add(childUuid, childResult.Uuid);
                            }
                            else
                            {
                                _emptyChild.Add(childResult.Query);
                            }

                            results.Add(new TrakHoundResult<bool>(Id, childUuid, TrakHoundResultType.Ok, true));
                        }
                    }
                }
            }

            return new TrakHoundResponse<bool>(results);
        }

        public async Task<TrakHoundResponse<bool>> StoreResults(IEnumerable<ITrakHoundObjectQueryResult> queryResults)
        {
            if (!queryResults.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var queryResult in queryResults)
                    {
                        var requestedParentKey = queryResult.IsRoot ? "$ROOT$" : queryResult.RequestedParentUuid;

                        string queryKey;
                        if (!string.IsNullOrEmpty(queryResult.RequestedParentUuid) || queryResult.IsRoot) queryKey = $"{queryResult.Namespace}::{queryResult.QueryType}::{queryResult.Query}::{queryResult.ParentLevel}::{requestedParentKey}::{queryResult.Skip}::{queryResult.Take}::{(int)queryResult.SortOrder}";
                        else queryKey = $"{queryResult.Namespace}::{queryResult.QueryType}::{queryResult.Query}::{queryResult.Skip}::{queryResult.Take}::{(int)queryResult.SortOrder}";

                        var parentKey = queryResult.ParentUuid != null ? queryResult.ParentUuid : "$ROOT$";

                        if (queryResult.QueryType == TrakHoundObjectQueryRequestType.DefinitionUuid)
                        {
                            _queryDefinitionReverseResults.Add(queryResult.Query, queryKey);
                        }

                        if (queryResult.Uuid != null)
                        {
                            _queryResults.Add(queryKey, new QueryResult(queryResult.Uuid, parentKey));
                            _queryReverseResults.Add(queryResult.Uuid, queryKey);
                            _queryParentReverseResults.Add(parentKey, queryKey);
                        }
                        else
                        {
                            _emptyQueryResults.Add(requestedParentKey, queryKey);
                        }
                    }
                }
            }

            return TrakHoundResponse<bool>.RouteNotConfigured(Id, "StoreResults");
        }

        public async Task<TrakHoundResponse<bool>> Invalidate(IEnumerable<string> objectUuids)
        {
            return TrakHoundResponse<bool>.RouteNotConfigured(Id, null);
        }


        protected override bool PublishCompare(ITrakHoundObjectEntity newEntity, ITrakHoundObjectEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Priority >= existingEntity.Priority && newEntity.Created > existingEntity.Created : true;
		}

        protected override void OnPublishBefore(IEnumerable<ITrakHoundObjectEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (!string.IsNullOrEmpty(entity.Uuid))
                {
                    lock (_lock)
                    {
                        ClearEntity(entity);
                    }
                }
            }
        }

        protected override void OnDeleteBefore(IEnumerable<EntityDeleteRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    lock (_lock)
                    {
                        var entity = _entities.GetValueOrDefault(request.Target);
                        if (entity != null)
                        {
                            ClearEntity(entity);
                        }
                    }
                }
            }
        }

        private void ClearEntity(ITrakHoundObjectEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.Uuid))
            {
                var parentKey = entity.ParentUuid != null ? entity.ParentUuid : "$ROOT$";

                _parentChildren.Remove(entity.Uuid);
                _parentChildren.Remove(parentKey);
                _rootChildren.Remove(entity.Uuid);
                _rootChildren.Remove(parentKey);

                if (!string.IsNullOrEmpty(entity.DefinitionUuid))
                {
                    var definitionQueryKeys = _queryDefinitionReverseResults.Get(entity.DefinitionUuid);
                    if (!definitionQueryKeys.IsNullOrEmpty())
                    {
                        foreach (var queryKey in definitionQueryKeys) _queryResults.Remove(queryKey);
                        _queryDefinitionReverseResults.Remove(entity.DefinitionUuid);
                    }
                }

                // Remove Query Results
                var queryKeys = _queryReverseResults.Get(entity.Uuid);
                if (!queryKeys.IsNullOrEmpty())
                {
                    foreach (var querykey in queryKeys) _queryResults.Remove(querykey);
                }

                // Remove Parent Query Results
                queryKeys = _queryParentReverseResults.Get(entity.ParentUuid);
                if (!queryKeys.IsNullOrEmpty())
                {
                    foreach (var queryKey in queryKeys) _queryResults.Remove(queryKey);
                }

                // Remove Parent Query Results
                queryKeys = _queryParentReverseResults.Get("$ROOT$");
                if (!queryKeys.IsNullOrEmpty())
                {
                    foreach (var queryKey in queryKeys) _queryResults.Remove(queryKey);
                }

                // Remove Empty Results;
                _emptyParent.Remove(entity.Uuid);
                _emptyParent.Remove(parentKey);
                _emptyRoot.Remove(entity.Uuid);
                _emptyRoot.Remove(parentKey);
                _emptyQueryResults.Remove(entity.Uuid);
                _emptyQueryResults.Remove(parentKey);

                // Remove from Path Results
                var path = _pathReverseResults.GetValueOrDefault(entity.Uuid);
                if (path != null)
                {
                    _pathResults.Remove(path);
                    _pathReverseResults.Remove(entity.Uuid);
                }
            }
        }
    }
}
