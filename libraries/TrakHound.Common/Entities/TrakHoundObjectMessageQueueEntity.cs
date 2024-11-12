// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Represents an Object that is added to a FIFO Stack of another Object
    /// </summary>
    public struct TrakHoundObjectMessageQueueEntity : ITrakHoundObjectMessageQueueEntity
    {
        private string _uuid = null;
        public string Uuid
        {
            get
            {
                if (_uuid == null) _uuid = GenerateUuid(ObjectUuid);
                return _uuid;
            }
        }

        /// <summary>
        /// The ID of the Object that this Message Entry is associated with
        /// </summary>
        public string ObjectUuid { get; set; }

        /// <summary>
        /// The ID of the Message Queue
        /// </summary>
        public string QueueId { get; set; }

        /// <summary>
        /// The MIME type of the Payload
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The ID of the Source of the Message entry
        /// </summary>
        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.MessageQueue;

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
                       !string.IsNullOrEmpty(QueueId) &&
                       !string.IsNullOrEmpty(SourceUuid);
            }
        }


        public TrakHoundObjectMessageQueueEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            QueueId = null;
            SourceUuid = null;
            Created = now;
        }

        public TrakHoundObjectMessageQueueEntity(string objectUuid, string queueId, string contentType, string sourceUuid = null, long created = 0)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            QueueId = queueId;
            ContentType = contentType;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectMessageQueueEntity(ITrakHoundObjectMessageQueueEntity entity)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            QueueId = null;
            SourceUuid = null;
            Created = now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                QueueId = entity.QueueId;
                ContentType = entity.ContentType;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created > 0 ? entity.Created : now;
            }
        }


        public static string GenerateUuid(ITrakHoundObjectMessageQueueEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid);
            }

            return null;
        }

        public static string GenerateUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                return $"{objectUuid}:message-queue".ToSHA256Hash();
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectMessageQueueEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.Uuid}:{entity.ObjectUuid}:{entity.QueueId}:{entity.ContentType}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectMessageQueueEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectMessageQueueEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.QueueId,
            entity.ContentType,
            entity.SourceUuid,
            entity.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectMessageQueueEntity FromArray(object[] entity)
        {
            if (entity != null && entity.Length >= 5)
            {
                return new TrakHoundObjectMessageQueueEntity
                {
                    ObjectUuid = entity[0]?.ToString(),
                    QueueId = entity[1]?.ToString(),
                    ContentType = entity[2]?.ToString(),
                    SourceUuid = entity[3]?.ToString(),
                    Created = entity[4].ToLong()
                };
            }

            return new TrakHoundObjectMessageQueueEntity { };
        }

        public static IEnumerable<ITrakHoundObjectMessageQueueEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectMessageQueueEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectMessageQueueEntity>();
        }


        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (overwrite || string.IsNullOrEmpty(SourceUuid)) SourceUuid = sourceUuid;
        }
    }
}
