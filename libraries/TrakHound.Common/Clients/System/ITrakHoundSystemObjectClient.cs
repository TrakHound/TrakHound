// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectClient
    {

        #region "Query"

        Task<IEnumerable<ITrakHoundObjectEntity>> Query(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> Query(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(string path, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(IEnumerable<string> paths, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundObjectEntity>> Query(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuids(TrakHoundObjectQueryRequest queryRequest, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<string>> QueryMatch(string path, IEnumerable<string> objectUuids, string routerId = null);

        Task<IEnumerable<TrakHoundTargetResult>> QueryMatch(IEnumerable<string> paths, IEnumerable<string> objectUuids, string routerId = null);


        Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(string ns, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> QueryRoot(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(string ns, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuids(IEnumerable<string> namespaces, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> QueryByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(string parentUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByParentUuid(IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> QueryByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> QueryChildrenByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(string rootUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryChildUuidsByRootUuid(IEnumerable<string> rootUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);


        Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEntity>> QueryRootByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(string childUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        Task<IEnumerable<ITrakHoundObjectQueryResult>> QueryRootUuidsByChildUuid(IEnumerable<string> childUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        #endregion

        #region "Namespaces"

        Task<IEnumerable<string>> ListNamespaces(long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending, string routerId = null);

        #endregion

        #region "Subscribe"

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>> Subscribe(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>> SubscribeByUuid(
            IEnumerable<string> uuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        #endregion

        #region "Count"

        Task<TrakHoundCount> QueryChildCount(string uuid, string routerId = null);

        Task<IEnumerable<TrakHoundCount>> QueryChildCount(IEnumerable<string> uuids, string routerId = null);

        #endregion

        #region "Notify"

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>> Notify(string path, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All, string routerId = null);

        #endregion

        #region "Index"

        Task<IEnumerable<string>> QueryIndex(IEnumerable<EntityIndexRequest> requests, long skip, long take, SortOrder sortOrder, string routerId = null);

        Task<bool> UpdateIndex(IEnumerable<EntityIndexPublishRequest> requests, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null);

        #endregion

        #region "Delete"

        Task<bool> DeleteByRootUuid(string rootUuid, string routerId = null);

        Task<bool> DeleteByRootUuid(IEnumerable<string> rootUuids, string routerId = null);

        #endregion

        #region "Expire"

        Task<long> Expire(string pattern, long created, string routerId = null);

        Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<string> patterns, long created, string routerId = null);


        Task<long> ExpireByUpdate(string pattern, long lastUpdated, string routerId = null);

        Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<string> patterns, long lastUpdated, string routerId = null);


        Task<long> ExpireByAccess(string pattern, long lastAccessed, string routerId = null);

        Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<string> patterns, long lastAccessed, string routerId = null);

        #endregion

    }
}
