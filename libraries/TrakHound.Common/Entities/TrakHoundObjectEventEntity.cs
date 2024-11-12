// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectEventEntity : ITrakHoundObjectEventEntity
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

		public string ObjectUuid { get; set; }

        public string TargetUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Timestamp { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Event;

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
                return !string.IsNullOrEmpty(ObjectUuid) &&
                       !string.IsNullOrEmpty(TargetUuid) &&
                       !string.IsNullOrEmpty(SourceUuid) &&
                       Timestamp > 0;
            }
        }


        public TrakHoundObjectEventEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            TargetUuid = null;
            SourceUuid = null;
            Timestamp = now;
            Created = now;
        }

        public TrakHoundObjectEventEntity(string objectUuid, string targetUuid, string sourceUuid = null, long timestamp = 0, long created = 0)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            TargetUuid = targetUuid;
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectEventEntity(ITrakHoundObjectEventEntity entity)
        {
            ObjectUuid = null;
            TargetUuid = null;
            SourceUuid = null;
            Timestamp = 0;
            Created = 0;

            if (entity != null)
            {
                var now = UnixDateTime.Now;

                ObjectUuid = entity.ObjectUuid;
                TargetUuid = entity.TargetUuid;
                SourceUuid = entity.SourceUuid;
                Timestamp = entity.Timestamp > 0 ? entity.Timestamp : now;
                Created = entity.Created > 0 ? entity.Created : now;
            }
        }


        public static string GenerateUuid(string objectUuid, string targetUuid, long timestamp)
        {
            return $"{objectUuid}:{targetUuid}:{timestamp}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectEventEntity e)
        {
            if (e != null)
            {
                return GenerateUuid(e.ObjectUuid, e.TargetUuid, e.Timestamp);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectEventEntity e)
        {
            if (e != null)
            {
                return $"{e.ObjectUuid}:{e.TargetUuid}:{e.SourceUuid}:{e.Timestamp}:{e.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectEventEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundObjectEventEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.TargetUuid,
            obj.SourceUuid,
            obj.Timestamp,
            obj.Created
        };

        public object[] ToArray() => new object[] {
            ObjectUuid,
            TargetUuid,
            SourceUuid,
            Timestamp,
            Created
        };


        public static ITrakHoundObjectEventEntity FromArray(object[] objArray)
        {
            if (objArray != null && objArray.Length >= 5)
            {
                return new TrakHoundObjectEventEntity
                {
                    ObjectUuid = objArray[0]?.ToString(),
                    TargetUuid = objArray[1]?.ToString(),
                    SourceUuid = objArray[2]?.ToString(),
                    Timestamp = objArray[3].ToLong(),
                    Created = objArray[4].ToLong()
                };
            }

            return new TrakHoundObjectEventEntity();
        }

        public static IEnumerable<ITrakHoundObjectEventEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectEventEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectEventEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
