// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound.Clients.Collections
{
    public class TrakHoundCollectionEntityClient<TEntity> where TEntity : ITrakHoundEntity
    {
        private readonly TrakHoundEntityCollection _collection;

        public TrakHoundCollectionEntityClient(TrakHoundEntityCollection collection)
        {
            _collection = collection;
        }


        protected IEnumerable<string> GetObjectUuids(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return GetObjectUuids(new string[] { path });
            }

            return null;
        }

        protected IEnumerable<string> GetObjectUuids(IEnumerable<string> paths)
        {
            if (!paths.IsNullOrEmpty())
            {
                var expressionQueries = new List<string>();
                var pathQueries = new List<string>();
                var objectUuids = new List<string>();

                foreach (var path in paths)
                {
                    if (TrakHoundPath.GetType(path) == TrakHoundPathType.Expression) expressionQueries.Add(path);
                    else pathQueries.Add(path);
                }

                if (!expressionQueries.IsNullOrEmpty())
                {
                    foreach (var expressionQuery in expressionQueries)
                    {
                        var objs = _collection.Objects.QueryObjects(expressionQuery);
                        if (!objs.IsNullOrEmpty()) objectUuids.AddRange(objs.Select(o => o.Uuid));
                    }
                }

                if (!pathQueries.IsNullOrEmpty())
                {
                    var objs = _collection.Objects.QueryObjects(pathQueries);
                    if (!objs.IsNullOrEmpty()) objectUuids.AddRange(objs.Select(o => o.Uuid));
                }

                return objectUuids;
            }

            return null;
        }


        #region "Read"

        public async Task<TEntity> ReadByUuid(string uuid, string routerId = null) => _collection.GetEntity<TEntity>(uuid);

        public async Task<IEnumerable<TEntity>> ReadByUuid(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<TEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _collection.GetEntity<TEntity>(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }

        #endregion

        #region "Subscribe"

        public async Task<ITrakHoundConsumer<IEnumerable<TEntity>>> Subscribe(int interval = 0, int limit = 1000, string consumerId = null, string routerId = null) => null;

        #endregion

        #region "Publish"

        public async Task<bool> Publish(TEntity entity, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            _collection.Add(entity);
            return true;
        }

        public async Task<bool> Publish(IEnumerable<TEntity> entities, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities) _collection.Add(entity);
            }
            return true;
        }

        #endregion

        #region "Delete"

        public async Task<bool> Delete(string uuid, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null) => false;

        public async Task<bool> Delete(IEnumerable<string> uuids, TrakHoundOperationMode mode = TrakHoundOperationMode.Async, string routerId = null) => false;

        #endregion

        #region "Expire"

        public async Task<EntityDeleteResult> Expire(EntityDeleteRequest request, string routerId = null) => default;

        public async Task<IEnumerable<EntityDeleteResult>> Expire(IEnumerable<EntityDeleteRequest> requests, string routerId = null) => null;

        public async Task<EntityDeleteResult> ExpireByUpdate(EntityDeleteRequest request, string routerId = null) => default;

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByUpdate(IEnumerable<EntityDeleteRequest> requests, string routerId = null) => null;

        public async Task<EntityDeleteResult> ExpireByAccess(EntityDeleteRequest request, string routerId = null) => default;

        public async Task<IEnumerable<EntityDeleteResult>> ExpireByAccess(IEnumerable<EntityDeleteRequest> requests, string routerId = null) => null;

        #endregion

    }
}
