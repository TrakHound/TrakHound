// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// Sources are used to represent where data originated from
    /// </summary>
    public struct TrakHoundSourceEntity : ITrakHoundSourceEntity
    {
        private string _uuid;
        public string Uuid => _uuid;

        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                _uuid = GenerateUuid(_type, _sender, _parentUuid);
            }
        }

        private string _sender;
        public string Sender
        {
            get => _sender;
            set
            {
                _sender = value;
                _uuid = GenerateUuid(_type, _sender, _parentUuid);
            }
        }

        private string _parentUuid;
        public string ParentUuid
        {
            get => _parentUuid;
            set
            {
                _parentUuid = value;
                _uuid = GenerateUuid(_type, _sender, _parentUuid);
            }
        }

        public long Created { get; set; }

        public byte Category => TrakHoundEntityCategoryId.Sources;

        public byte Class => TrakHoundSourcesEntityClassId.Source;


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
                return !string.IsNullOrEmpty(Type) &&
                       !string.IsNullOrEmpty(Sender);
            }
        }


        public TrakHoundSourceEntity()
        {
            ParentUuid = null;
            Type = null;
            Sender = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundSourceEntity(ITrakHoundSourceEntity entity)
        {
            ParentUuid = null;
            Type = null;
            Sender = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                ParentUuid = entity.ParentUuid;
                Type = entity.Type;
                Sender = entity.Sender;
                Created = entity.Created;
            }
        }

        public TrakHoundSourceEntity(string type, string sender, string parentUuid = null, long created = 0)
        {
            ParentUuid = parentUuid;
            Type = type;
            Sender = sender;
            Created = created > 0 ? created : UnixDateTime.Now;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundSourceEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static string GenerateUuid(string type, string sender, string parentUuid)
        {
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(sender))
            {
                if (!string.IsNullOrEmpty(parentUuid))
                {
                    return $"{parentUuid}:{type}:{sender}".ToSHA256Hash();
                }
                else
                {
                    return $"{type}:{sender}".ToSHA256Hash();
                }
            }

            return null;
        }

        public static string GenerateUuid(ITrakHoundSourceEntity entity)
        {
            if (entity != null)
            {
                return GenerateUuid(entity.Type, entity.Sender, entity.ParentUuid);
            }

            return null;
        }

        public static byte[] GenerateHash(ITrakHoundSourceEntity entity)
        {
            if (entity != null)
            {
                return $"{entity.Uuid}:{entity.ParentUuid}:{entity.Type}:{entity.Sender}:{entity.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public static ITrakHoundSourceEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= 4)
            {
                return new TrakHoundSourceEntity
                {
                    Type = obj[0]?.ToString(),
                    Sender = obj[1]?.ToString(),
                    ParentUuid = obj[2]?.ToString(),
                    Created = obj[3].ToLong(),
                };
            }

            return new TrakHoundSourceEntity { };
        }

        public static IEnumerable<ITrakHoundSourceEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundSourceEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundSourceEntity>();
        }

        public static object[] ToArray(ITrakHoundSourceEntity entity) => new object[] {
            entity.Type,
            entity.Sender,
            entity.ParentUuid,
            entity.Created
        };

        public object[] ToArray() => new object[] {
            Type,
            Sender,
            ParentUuid,
            Created
        };
    }
}
