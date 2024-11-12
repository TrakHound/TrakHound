// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to represent a Horizontal Relationship of one Object to another Object
    /// </summary>
    public struct TrakHoundObjectGroupEntity : ITrakHoundObjectGroupEntity
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

        public string GroupUuid { get; set; }

        public string MemberUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Group;

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
                       !string.IsNullOrEmpty(GroupUuid) &&
                       !string.IsNullOrEmpty(MemberUuid) &&
                       !string.IsNullOrEmpty(SourceUuid);
            }
        }


        public TrakHoundObjectGroupEntity()
        {
            GroupUuid = null;
            MemberUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectGroupEntity(ITrakHoundObjectGroupEntity obj)
        {
            GroupUuid = null;
            MemberUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (obj != null)
            {
                GroupUuid = obj.GroupUuid;
                MemberUuid = obj.MemberUuid;
                SourceUuid = obj.SourceUuid;
                Created = obj.Created;
            }
        }

        public TrakHoundObjectGroupEntity(string groupUuid, string memberUuid, string sourceUuid = null, long created = 0)
        {
            GroupUuid = groupUuid;
            MemberUuid = memberUuid;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(string groupUuid, string memberUuid)
        {
            return $"{groupUuid}:{memberUuid}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectGroupEntity group)
        {
            if (group != null)
            {
                return GenerateUuid(group.GroupUuid, group.MemberUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectGroupEntity group)
        {
            if (group != null)
            {
                return $"{group.GroupUuid}:{group.MemberUuid}:{group.SourceUuid}:{group.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectGroupEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundObjectGroupEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundObjectGroupEntity
                {
                    GroupUuid = obj[0]?.ToString(),
                    MemberUuid = obj[1]?.ToString(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong()
                };
            }

            return new TrakHoundObjectGroupEntity { };
        }

        public static IEnumerable<ITrakHoundObjectGroupEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectGroupEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectGroupEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectGroupEntity obj) => new object[] {
            obj.GroupUuid,
            obj.MemberUuid,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => new object[] {
            GroupUuid,
            MemberUuid,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
