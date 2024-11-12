// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class DefinitionDriver : MemoryEntityDriver<ITrakHoundDefinitionEntity>,
        IDefinitionQueryDriver,
        IDefinitionQueryStoreDriver
    {
        private readonly ListDictionary<string, string> _parentChildren = new ListDictionary<string, string>(); // ParentUuid => ChildUuids
        private readonly ListDictionary<string, string> _rootChildren = new ListDictionary<string, string>(); // RootUuid => ChildUuids
        private readonly ListDictionary<string, string> _childRoots = new ListDictionary<string, string>(); // ChildUuid => RootUuids
        private readonly HashSet<string> _emptyParent = new HashSet<string>(); // ParentUuid
        private readonly HashSet<string> _emptyRoot = new HashSet<string>(); // RootUuid
        private readonly HashSet<string> _emptyChild = new HashSet<string>(); // ChildUuid

        private readonly ListDictionary<string, string> _types = new ListDictionary<string, string>();
        private readonly HashSet<string> _emptyTypes = new HashSet<string>();


        public DefinitionDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        #region "Query"

        public async Task<TrakHoundResponse<string>> Query(string pattern, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<string>.RouteNotConfigured(Id, pattern);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByType(IEnumerable<string> types, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            if (!types.IsNullOrEmpty())
            {
                foreach (var type in types)
                {
                    if (type != null)
                    {
                        var queryKey = type.ToLower();

                        var uuids = _types.Get(queryKey);
                        if (!uuids.IsNullOrEmpty())
                        {
                            foreach (var uuid in uuids)
                            {
                                var queryResult = new TrakHoundDefinitionQueryResult(type, uuid);
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.Ok, queryResult, uuid));
                            }
                        }
                        else
                        {
                            if (_emptyTypes.Contains(queryKey))
                            {
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, type, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<ITrakHoundDefinitionQueryResult>.RouteNotConfigured(Id, null);

            //var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            //lock (_lock)
            //{
            //    if (!parentUuids.IsNullOrEmpty())
            //    {
            //        foreach (var parentUuid in parentUuids)
            //        {
            //            var key = parentUuid != null ? parentUuid : "$ROOT$";

            //            var childUuids = _parentChildren.Get(key);
            //            if (!childUuids.IsNullOrEmpty())
            //            {
            //                foreach (var childUuid in childUuids)
            //                {
            //                    var queryResult = new TrakHoundDefinitionQueryResult(parentUuid, childUuid);
            //                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Ok, queryResult, childUuid));
            //                }
            //            }
            //            else
            //            {
            //                if (_emptyParent.Contains(key))
            //                {
            //                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.Empty));
            //                }
            //                else
            //                {
            //                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, parentUuid, TrakHoundResultType.NotFound));
            //                }
            //            }
            //        }
            //    }
            //}

            //return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundDefinitionQueryResult>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return TrakHoundResponse<ITrakHoundDefinitionQueryResult>.RouteNotConfigured(Id, null);

            //var results = new List<TrakHoundResult<ITrakHoundDefinitionQueryResult>>();

            //lock (_lock)
            //{
            //    if (!childUuids.IsNullOrEmpty())
            //    {
            //        foreach (var childUuid in childUuids)
            //        {
            //            var childObject = _entities.GetValueOrDefault(childUuid);
            //            if (childObject != null)
            //            {
            //                var queryResult = new TrakHoundDefinitionQueryResult(childUuid, childObject.ParentUuid);
            //                results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.Ok, queryResult, childObject.ParentUuid));
            //            }
            //            else
            //            {
            //                if (_emptyChild.Contains(childUuid))
            //                {
            //                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.Empty));
            //                }
            //                else
            //                {
            //                    results.Add(new TrakHoundResult<ITrakHoundDefinitionQueryResult>(Id, childUuid, TrakHoundResultType.NotFound));
            //                }
            //            }
            //        }
            //    }
            //}

            //return new TrakHoundResponse<ITrakHoundDefinitionQueryResult>(results);
        }

        public async Task<TrakHoundResponse<bool>> StoreResults(IEnumerable<ITrakHoundDefinitionQueryResult> queryResults)
        {
            if (!queryResults.IsNullOrEmpty())
            {
                lock (_lock)
                {
                    foreach (var queryResult in queryResults)
                    {
                        if (!string.IsNullOrEmpty(queryResult.Query))
                        {
                            var queryKey = queryResult.Query.ToLower();

                            if (queryResult.Uuid != null)
                            {
                                _types.Add(queryKey, queryResult.Uuid);
                            }
                            else
                            {
                                _emptyTypes.Add(queryKey);
                            }
                        }
                    }
                }
            }

            return TrakHoundResponse<bool>.RouteNotConfigured(Id, "StoreResults");
        }

        #endregion


        protected override bool PublishCompare(ITrakHoundDefinitionEntity newEntity, ITrakHoundDefinitionEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }
    }
}
