// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectGroupEntity> _groups = new Dictionary<string, ITrakHoundObjectGroupEntity>();

        private readonly ListDictionary<string, string> _groupsByGroupUuid = new ListDictionary<string, string>(); // GroupUuid => Uuid


        private readonly ListDictionary<string, string> _groupsByMemberUuid = new ListDictionary<string, string>(); // MemberUuid => Uuid



        public IEnumerable<ITrakHoundObjectGroupEntity> Groups => _groups.Values;
        public void AddGroup(ITrakHoundObjectGroupEntity entity)
        {
            if (entity != null)
            {
                var x = _groups.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _groups.Remove(entity.Uuid);
                    _groups.Add(entity.Uuid, new TrakHoundObjectGroupEntity(entity));


                    _groupsByGroupUuid.Add(entity.GroupUuid, entity.Uuid);

                    _groupsByMemberUuid.Add(entity.MemberUuid, entity.Uuid);

                }
            }
        }

        public void AddGroups(IEnumerable<ITrakHoundObjectGroupEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _groups.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _groups.Remove(entity.Uuid);
                            _groups.Add(entity.Uuid, new TrakHoundObjectGroupEntity(entity));


                            _groupsByGroupUuid.Add(entity.GroupUuid, entity.Uuid);
                            _groupsByMemberUuid.Add(entity.MemberUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectGroupEntity GetGroup(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _groups.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectGroupEntity> GetGroups(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectGroupEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _groups.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectGroupEntity> QueryGroupsByGroupUuid(string groupUuid)
        {
            if (!string.IsNullOrEmpty(groupUuid))
            {
                var uuids = _groupsByGroupUuid.Get(groupUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectGroupEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _groups.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }





        public IEnumerable<ITrakHoundObjectGroupEntity> QueryGroupsByMemberUuid(string memberUuid)
        {
            if (!string.IsNullOrEmpty(memberUuid))
            {
                var uuids = _groupsByMemberUuid.Get(memberUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectGroupEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _groups.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetGroupArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _groups.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
