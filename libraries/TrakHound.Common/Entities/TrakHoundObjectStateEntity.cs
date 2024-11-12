// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YamlDotNet.Core.Tokens;

namespace TrakHound.Entities
{
    /// <summary>
    /// The State of an Object at a specific time
    /// </summary>
    public struct TrakHoundObjectStateEntity : ITrakHoundObjectStateEntity
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

        public string DefinitionUuid { get; set; }

        public long TTL { get; set; }

        public string SourceUuid { get; set; }

        public long Timestamp { get; set; }

        public long Created { get; set; }


        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.State;

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
                       !string.IsNullOrEmpty(DefinitionUuid) &&
                       !string.IsNullOrEmpty(SourceUuid) &&
                       Timestamp > 0;
            }
        }


        public TrakHoundObjectStateEntity()
        {
            var now = UnixDateTime.Now;

            ObjectUuid = null;
            DefinitionUuid = null;
            TTL = 0;
            Timestamp = now;
            SourceUuid = null;
            Created = now;
        }

        public TrakHoundObjectStateEntity(string objectUuid, string definitionUuid, int ttl = 0, string sourceUuid = null, long timestamp = 0, long created = 0)
        {
            var now = UnixDateTime.Now;

            ObjectUuid = objectUuid;
            DefinitionUuid = definitionUuid;
            TTL = ttl;
            SourceUuid = sourceUuid;
            Timestamp = timestamp > 0 ? timestamp : now;
            Created = created > 0 ? created : now;
        }

        public TrakHoundObjectStateEntity(ITrakHoundObjectStateEntity entity)
        {
            ObjectUuid = null;
            DefinitionUuid = null;
            TTL = 0;
            Timestamp = 0;
            SourceUuid = null;
            Created = 0;

            if (entity != null)
            {
                ObjectUuid = entity.ObjectUuid;
                DefinitionUuid = entity.DefinitionUuid;
                TTL = entity.TTL;
                SourceUuid = entity.SourceUuid;
                Timestamp = entity.Timestamp;
                Created = entity.Created;
            }
        }


        public static string GenerateUuid(string objectUuid, long timestamp)
        {
            return $"{objectUuid}:{timestamp}".ToSHA256Hash();
        }

        public static string GenerateUuid(ITrakHoundObjectStateEntity state)
        {
            if (state != null)
            {
                return GenerateUuid(state.ObjectUuid, state.Timestamp);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundObjectStateEntity state)
        {
            if (state != null)
            {
                return $"{state.ObjectUuid}:{state.DefinitionUuid}:{state.TTL}:{state.SourceUuid}:{state.Timestamp}:{state.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectStateEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundObjectStateEntity state) => new object[] {
            state.ObjectUuid,
            state.DefinitionUuid,
            state.TTL,
            state.SourceUuid,
            state.Timestamp,
            state.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectStateEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 6)
            {
                return new TrakHoundObjectStateEntity
                {
                    ObjectUuid = obj[0]?.ToString(),
                    DefinitionUuid = obj[1]?.ToString(),
                    TTL = obj[2].ToInt(),
                    SourceUuid = obj[3]?.ToString(),
                    Timestamp = obj[4].ToLong(),
                    Created = obj[5].ToLong()
                };
            }

            return new TrakHoundObjectStateEntity { };
        }

        public static IEnumerable<ITrakHoundObjectStateEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectStateEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectStateEntity>();
        }

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
