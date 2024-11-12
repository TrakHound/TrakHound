// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to represent a Temporary Horizontal Relationship of one Object to another Object
    /// </summary>
    public struct TrakHoundObjectAssignmentEntity : ITrakHoundObjectAssignmentEntity
    {
        private string _uuid = null;
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(this);
                return _uuid;
            }
        }

        public string AssigneeUuid { get; set; }

        public string MemberUuid { get; set; }

        public long AddTimestamp { get; set; }

        public string AddSourceUuid { get; set; }

        public long RemoveTimestamp { get; set; }

        public string RemoveSourceUuid { get; set; }

        public long Created { get; set; }

        public TimeSpan Duration => GetDuration(this);


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Assignment;


        private byte[] _hash = null;
        public byte[] Hash
        {
            get
            {
                if (_hash == null) _hash = GenerateHash(this);
                return _hash;
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Uuid) &&
                       !string.IsNullOrEmpty(AssigneeUuid) &&
                       !string.IsNullOrEmpty(MemberUuid) &&
                       !string.IsNullOrEmpty(AddSourceUuid) &&
                       AddTimestamp > 0;
            }
        }


        public TrakHoundObjectAssignmentEntity()
        {
            var now = UnixDateTime.Now;

            AssigneeUuid = null;
            MemberUuid = null;
            AddTimestamp = now;
            AddSourceUuid = null;
            RemoveTimestamp = 0;
            RemoveSourceUuid = null;
            Created = now;
        }

        public TrakHoundObjectAssignmentEntity(ITrakHoundObjectAssignmentEntity assignment)
        {
            var now = UnixDateTime.Now;

            AssigneeUuid = null;
            MemberUuid = null;
            AddTimestamp = now;
            AddSourceUuid = null;
            RemoveTimestamp = 0;
            RemoveSourceUuid = null;
            Created = now;

            if (assignment != null)
            {
                AssigneeUuid = assignment.AssigneeUuid;
                MemberUuid = assignment.MemberUuid;
                AddTimestamp = assignment.AddTimestamp;
                AddSourceUuid = assignment.AddSourceUuid;
                RemoveTimestamp = assignment.RemoveTimestamp;
                RemoveSourceUuid = assignment.RemoveSourceUuid;
                Created = assignment.Created;
            }
        }

        public TrakHoundObjectAssignmentEntity(string assigneeUuid, string memberUuid, long addTimestamp = 0, string addSourceUuid = null, long removeTimestamp = 0, string removeSourceUuid = null, long created = 0)
        {
            var now = UnixDateTime.Now;

            AssigneeUuid = assigneeUuid;
            MemberUuid = memberUuid;
            AddTimestamp = addTimestamp > 0 ? addTimestamp : now;
            AddSourceUuid = addSourceUuid;
            RemoveTimestamp = removeTimestamp;
            RemoveSourceUuid = removeSourceUuid;
            Created = created > 0 ? created : now;
        }


        public static string GenerateUuid(string assigneeUuid, string memberUuid, long addTimestamp)
        {
            return $"{assigneeUuid}:{memberUuid}:{addTimestamp}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectAssignmentEntity assignment)
        {
            if (assignment != null)
            {
                return GenerateUuid(assignment.AssigneeUuid, assignment.MemberUuid, assignment.AddTimestamp);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectAssignmentEntity assignment)
        {
            if (assignment != null)
            {
                return $"{assignment.AssigneeUuid}:{assignment.MemberUuid}:{assignment.AddTimestamp}:{assignment.AddSourceUuid}:{assignment.RemoveTimestamp}:{assignment.RemoveSourceUuid}:{assignment.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectAssignmentEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static TimeSpan GetDuration(ITrakHoundObjectAssignmentEntity assignment)
        {
            if (assignment != null)
            {
                var start = assignment.AddTimestamp;
                var end = assignment.RemoveTimestamp > 0 ? assignment.RemoveTimestamp : UnixDateTime.Now;

                return TimeSpan.FromTicks(end - start);
            }

            return TimeSpan.Zero;
        }

        public static bool IsBetween(ITrakHoundObjectAssignmentEntity entity, long from, long to)
        {
            if (entity != null && to > from)
            {
                if (entity.AddTimestamp >= from && entity.AddTimestamp < to && entity.RemoveTimestamp < 1) return true;
                if (entity.AddTimestamp >= from && entity.AddTimestamp < to) return true;
                if (entity.AddTimestamp <= from && entity.RemoveTimestamp >= to) return true;
                if (entity.AddTimestamp >= from && entity.RemoveTimestamp > from && entity.RemoveTimestamp < to) return true;
            }

            return false;
        }

        public static ITrakHoundObjectAssignmentEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 7)
            {
                var x = new TrakHoundObjectAssignmentEntity();
                x.AssigneeUuid = obj[0]?.ToString();
                x.MemberUuid = obj[1]?.ToString();
                x.AddTimestamp = obj[2].ToLong();
                x.AddSourceUuid = obj[3]?.ToString();
                x.RemoveTimestamp = obj[4].ToLong();
                x.RemoveSourceUuid = obj[5]?.ToString();
                x.Created = obj[6].ToLong();
                return x;
            }

            return new TrakHoundObjectAssignmentEntity { };
        }

        public static IEnumerable<ITrakHoundObjectAssignmentEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectAssignmentEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectAssignmentEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectAssignmentEntity assignment) => new object[] {
            assignment.AssigneeUuid,
            assignment.MemberUuid,
            assignment.AddTimestamp,
            assignment.AddSourceUuid,
            assignment.RemoveTimestamp,
            assignment.RemoveSourceUuid,
            assignment.Created
        };

        public object[] ToArray() => new object[] {
            AssigneeUuid,
            MemberUuid,
            AddTimestamp,
            AddSourceUuid,
            RemoveTimestamp,
            RemoveSourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(AddSourceUuid) || overwrite) AddSourceUuid = sourceUuid;
            if ((string.IsNullOrEmpty(RemoveSourceUuid) && RemoveTimestamp > 0) || overwrite) RemoveSourceUuid = sourceUuid;
        }
    }
}
