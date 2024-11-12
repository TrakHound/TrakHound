// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to Assign a Message entry for a TrakHound Object Entity
    /// </summary>
    public struct TrakHoundObjectMessageEntity : ITrakHoundObjectMessageEntity
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
        /// 
        /// </summary>
        public string BrokerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The MIME type of the Payload
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Qos { get; set; }

        /// <summary>
        /// The ID of the Source of the Message entry
        /// </summary>
        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Message;

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
                    !string.IsNullOrEmpty(ObjectUuid) &&
                    !string.IsNullOrEmpty(BrokerId) &&
                    !string.IsNullOrEmpty(Topic) &&
                    !string.IsNullOrEmpty(SourceUuid);
            }
        }


        public TrakHoundObjectMessageEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            BrokerId = null;
            Topic = null;
            SourceUuid = null;
            Created = now;
        }

        public TrakHoundObjectMessageEntity(string objectUuid, string brokerId, string topic, string contentType, bool retain = false, int qos = 0, string sourceUuid = null, long created = 0)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            BrokerId = brokerId;
            Topic = topic;
            ContentType = contentType;
            Retain = retain;
            Qos = qos;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectMessageEntity(ITrakHoundObjectMessageEntity entity)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            BrokerId = null;
            Topic = null;
            SourceUuid = null;
            Created = now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                BrokerId = entity.BrokerId;
                Topic = entity.Topic;
                ContentType = entity.ContentType;
                Retain = entity.Retain;
                Qos = entity.Qos;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created > 0 ? entity.Created : now;
            }
        }


        public static string GenerateUuid(ITrakHoundObjectMessageEntity entity)
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
                return $"{objectUuid}:message".ToSHA256Hash();
            }

            return null;
        }


        public static byte[] GenerateHash(ITrakHoundObjectMessageEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.Uuid}:{entity.ObjectUuid}:{entity.BrokerId}:{entity.Topic}:{entity.ContentType}:{entity.Retain}:{entity.Qos}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectMessageEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));

        public static object[] ToArray(ITrakHoundObjectMessageEntity obj) => new object[] {
            obj.ObjectUuid,
            obj.BrokerId,
            obj.Topic,
            obj.ContentType,
            obj.Retain,
            obj.Qos,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectMessageEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 8)
            {
                return new TrakHoundObjectMessageEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    BrokerId = obj[1]?.ToString(),
                    Topic = obj[2]?.ToString(),
                    ContentType = obj[3]?.ToString(),
                    Retain = obj[4].ToBoolean(),
                    Qos = obj[5].ToInt(),
                    SourceUuid = obj[6]?.ToString(),
                    Created = obj[7].ToLong()
                };
            }

            return new TrakHoundObjectMessageEntity { };
        }

        public static IEnumerable<ITrakHoundObjectMessageEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectMessageEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectMessageEntity>();
        }


        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (overwrite || string.IsNullOrEmpty(SourceUuid)) SourceUuid = sourceUuid;
        }
    }
}
