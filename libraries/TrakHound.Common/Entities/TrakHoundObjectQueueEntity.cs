// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Queue Entities are used to implement an editable FIFO queue of Objects.
    /// </summary>
    public struct TrakHoundObjectQueueEntity : ITrakHoundObjectQueueEntity
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

        public string QueueUuid { get; set; }

        public string MemberUuid { get; set; }

        public int Index { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Queue;

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
                       !string.IsNullOrEmpty(QueueUuid) &&
                       !string.IsNullOrEmpty(MemberUuid) &&
                       !string.IsNullOrEmpty(SourceUuid) &&
                       Timestamp > 0;
            }
        }


        public TrakHoundObjectQueueEntity()
        {
            var now = UnixDateTime.Now;

            QueueUuid = null;
            MemberUuid = null;
            Index = 0;
            Timestamp = now;
            SourceUuid = null;
            Created = now;
        }

        public TrakHoundObjectQueueEntity(ITrakHoundObjectQueueEntity entity)
        {
            var now = UnixDateTime.Now;

            QueueUuid = null;
            MemberUuid = null;
            Index = 0;
            Timestamp = now;
            SourceUuid = null;
            Created = now;

            if (entity != null)
            {
                QueueUuid = entity.QueueUuid;
                MemberUuid = entity.MemberUuid;
                Index = entity.Index;
                Timestamp = entity.Timestamp;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundObjectQueueEntity(string queueUuid, string memberUuid, int index = -1, string sourceUuid = null, long timestamp = 0, long created = 0)
        {
            var now = UnixDateTime.Now;

            QueueUuid = queueUuid;
            MemberUuid = memberUuid;
            Index = index;
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }


        public static string GenerateUuid(string queueUuid, string memberUuid)
        {
            return $"{queueUuid}:{memberUuid}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectQueueEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.QueueUuid, entity.MemberUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectQueueEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.QueueUuid}:{entity.MemberUuid}:{entity.Index}:{entity.Timestamp}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectQueueEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundObjectQueueEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 6)
            {
                return new TrakHoundObjectQueueEntity
                {
                    QueueUuid = obj[0]?.ToString(),
                    MemberUuid = obj[1]?.ToString(),
                    Index = obj[2].ToInt(),
                    Timestamp = obj[3].ToLong(),
                    SourceUuid = obj[4]?.ToString(),
                    Created = obj[5].ToLong()
                };
            }

            return new TrakHoundObjectQueueEntity { };
        }

        public static IEnumerable<ITrakHoundObjectQueueEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectQueueEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectQueueEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectQueueEntity obj) => new object[] {
            obj.QueueUuid,
            obj.MemberUuid,
            obj.Index,
            obj.Timestamp,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => new object[] {
            QueueUuid,
            MemberUuid,
            Index,
            Timestamp,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
