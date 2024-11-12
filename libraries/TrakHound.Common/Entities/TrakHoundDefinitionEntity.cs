// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundDefinitionEntity : ITrakHoundDefinitionEntity
    {

        private string _uuid;
        public string Uuid => _uuid;

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                _uuid = GenerateUuid(_id);
            }
        }

        public string ParentUuid { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }

        public byte Category => TrakHoundEntityCategoryId.Definitions;

        public byte Class => TrakHoundDefinitionsEntityClassId.Definition;

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
                return !string.IsNullOrEmpty(Uuid);
            }
        }

        public string Type => TrakHoundType.GetType(Id);


        public TrakHoundDefinitionEntity()
        {
            Id = null;
            ParentUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundDefinitionEntity(ITrakHoundDefinitionEntity entity)
        {
            Id = null;
            ParentUuid = null;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                Id = entity.Id;
                ParentUuid = entity.ParentUuid;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }

        public TrakHoundDefinitionEntity(string id, string parentUuid = null, string transactionUuid = null, long created = 0)
        {
            Id = id;
            ParentUuid = parentUuid;
            SourceUuid = transactionUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public static string GenerateUuid(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return id.ToLower().ToSHA256Hash();
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundDefinitionEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.Uuid}:{entity.ParentUuid}:{entity.SourceUuid}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundDefinitionEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static ITrakHoundDefinitionEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundDefinitionEntity
                {
                    Id = obj[0]?.ToString(),
                    ParentUuid = obj[1]?.ToString(),
                    SourceUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong(),
                };
            }

            return new TrakHoundDefinitionEntity { };
        }

        public static IEnumerable<ITrakHoundDefinitionEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundDefinitionEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundDefinitionEntity>();
        }

        public static object[] ToArray(ITrakHoundDefinitionEntity obj) => new object[] {
            obj.Id,
            obj.ParentUuid,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => new object[] {
            Id,
            ParentUuid,
            SourceUuid,
            Created
        };

        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
