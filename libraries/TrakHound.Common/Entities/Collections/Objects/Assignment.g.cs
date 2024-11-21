// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectAssignmentEntity> _assignments = new Dictionary<string, ITrakHoundObjectAssignmentEntity>();

        private readonly ListDictionary<string, string> _assignmentsByAssigneeUuid = new ListDictionary<string, string>(); // AssigneeUuid => Uuid


        private readonly ListDictionary<string, string> _assignmentsByMemberUuid = new ListDictionary<string, string>(); // MemberUuid => Uuid



        public IEnumerable<ITrakHoundObjectAssignmentEntity> Assignments => _assignments.Values;
        public void AddAssignment(ITrakHoundObjectAssignmentEntity entity)
        {
            if (entity != null)
            {
                var x = _assignments.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _assignments.Remove(entity.Uuid);
                    _assignments.Add(entity.Uuid, new TrakHoundObjectAssignmentEntity(entity));


                    _assignmentsByAssigneeUuid.Add(entity.AssigneeUuid, entity.Uuid);

                    _assignmentsByMemberUuid.Add(entity.MemberUuid, entity.Uuid);

                }
            }
        }

        public void AddAssignments(IEnumerable<ITrakHoundObjectAssignmentEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _assignments.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _assignments.Remove(entity.Uuid);
                            _assignments.Add(entity.Uuid, new TrakHoundObjectAssignmentEntity(entity));


                            _assignmentsByAssigneeUuid.Add(entity.AssigneeUuid, entity.Uuid);
                            _assignmentsByMemberUuid.Add(entity.MemberUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectAssignmentEntity GetAssignment(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _assignments.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectAssignmentEntity> GetAssignments(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectAssignmentEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _assignments.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectAssignmentEntity> QueryAssignmentsByAssigneeUuid(string assigneeUuid)
        {
            if (!string.IsNullOrEmpty(assigneeUuid))
            {
                var uuids = _assignmentsByAssigneeUuid.Get(assigneeUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectAssignmentEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _assignments.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }





        public IEnumerable<ITrakHoundObjectAssignmentEntity> QueryAssignmentsByMemberUuid(string memberUuid)
        {
            if (!string.IsNullOrEmpty(memberUuid))
            {
                var uuids = _assignmentsByMemberUuid.Get(memberUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectAssignmentEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _assignments.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetAssignmentArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _assignments.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
