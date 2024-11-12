// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectLogEntity : ITrakHoundObjectLogEntity
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

        public int LogLevel { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public long Timestamp { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Log;


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
                       !string.IsNullOrEmpty(Message) &&
                       !string.IsNullOrEmpty(SourceUuid) &&
                       Timestamp > 0;
            }
        }


        public TrakHoundObjectLogEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            LogLevel = (int)TrakHoundLogLevel.Information;
            Message = null;
            Code = null;
            SourceUuid = null;
            Timestamp = now;
            Created = now;
        }

        public TrakHoundObjectLogEntity(ITrakHoundObjectLogEntity entity)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            LogLevel = (int)TrakHoundLogLevel.Information;
            Message = null;
            Code = null;
            SourceUuid = null;
            Timestamp = now;
            Created = now;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                LogLevel = entity.LogLevel;
                Message = entity.Message;
                Code = entity.Code;
                SourceUuid = entity.SourceUuid;
                Timestamp = entity.Timestamp > 0 ? entity.Timestamp : now;
                Created = entity.Created > 0 ? entity.Created : now;
            }
        }

        public TrakHoundObjectLogEntity(string objectUuid, TrakHoundLogLevel logLevel, string message, string code = null, string sourceUuid = null, long timestamp = 0, long created = 0)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            LogLevel = (int)logLevel;
            Message = message;
            Code = code;
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectLogEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static string GenerateUuid(ITrakHoundObjectLogEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.ObjectUuid, entity.LogLevel, entity.Message, entity.Timestamp);
            }

            return null;
        }

        public static string GenerateUuid(string objectUuid, int logLevel, string message, long timestamp)
        {
            if (!string.IsNullOrEmpty(objectUuid) && !string.IsNullOrEmpty(message))
            {
                return $"{objectUuid}:{logLevel}:{message}:{timestamp}".ToSHA256Hash();
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectLogEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.ObjectUuid}:{entity.LogLevel}:{entity.Message}:{entity.Code}:{entity.SourceUuid}:{entity.Timestamp}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public static ITrakHoundObjectLogEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 6)
            {
                return new TrakHoundObjectLogEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    LogLevel = obj[1].ToInt(),
                    Message = obj[2]?.ToString(),
                    Code = obj[3]?.ToString(),
                    SourceUuid = obj[4]?.ToString(),
                    Timestamp = obj[5].ToLong(),
                    Created = obj[6].ToLong(),
                };
            }

            return new TrakHoundObjectLogEntity { };
        }

        public static IEnumerable<ITrakHoundObjectLogEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectLogEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectLogEntity>();
        }

        public static object[] ToArray(ITrakHoundObjectLogEntity entity) => new object[] {
            entity.ObjectUuid,
            entity.LogLevel,
            entity.Message,
            entity.Code,
            entity.SourceUuid,
            entity.Timestamp,
            entity.Created
        };

        public object[] ToArray() => new object[] {
            ObjectUuid,
            LogLevel,
            Message,
            Code,
            SourceUuid,
            Timestamp,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (overwrite || string.IsNullOrEmpty(SourceUuid)) SourceUuid = sourceUuid;
        }
    }
}
