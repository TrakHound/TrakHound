// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectEntity : ITrakHoundObjectEntity
    {
        public const string DefaultNamespace = TrakHoundNamespace.DefaultNamespace;
        public const byte DefaultPriority = 1;
        public const int ArraySize = 7;


        private string _uuid;
        public string Uuid => _uuid;


        //public string Namespace { get; set; }

        private string _namespace;
        public string Namespace
        {
            get => _namespace;
            set
            {
                _namespace = value;
                _uuid = TrakHoundPath.GetUuid(value, _path);
                _parentUuid = TrakHoundPath.GetUuid(value, TrakHoundPath.GetParentPath(_path));
            }
        }


        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = TrakHoundPath.ToRoot(value);
                _uuid = TrakHoundPath.GetUuid(Namespace, _path);
                _parentUuid = TrakHoundPath.GetUuid(Namespace, TrakHoundPath.GetParentPath(_path));
                _name = TrakHoundPath.GetObject(_path);
            }
        }

        private string _name;
        public string Name => _name;


        private string _parentUuid;
        public string ParentUuid => _parentUuid;


        public string ContentType { get; set; }

        public string DefinitionUuid { get; set; }

        public byte Priority { get; set; }

        public string SourceUuid { get; set; }

        public long Created { get; set; }

        public byte Category => TrakHoundEntityCategoryId.Objects;

        public byte Class => TrakHoundObjectsEntityClassId.Object;

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
                return !string.IsNullOrEmpty(Path) &&
                       !string.IsNullOrEmpty(Namespace) &&
                       !string.IsNullOrEmpty(ContentType) &&
                       !string.IsNullOrEmpty(SourceUuid);
            }
        }

        
        public TrakHoundObjectEntity()
        {
            Path = null;
            Namespace = DefaultNamespace;
            ContentType = null;
            DefinitionUuid = null;
            Priority = DefaultPriority;
            SourceUuid = null;
            Created = UnixDateTime.Now;
        }

        public TrakHoundObjectEntity(
            string path,
            TrakHoundObjectContentType contentType = TrakHoundObjectContentType.Directory,
            string definitionUuid = null,
            string ns = DefaultNamespace,
            byte priority = DefaultPriority,
            string sourceUuid = null,
            long created = 0
            )
        {
            _namespace = ns;
            _path = TrakHoundPath.ToRoot(path);
            _uuid = TrakHoundPath.GetUuid(ns, _path);
            _parentUuid = TrakHoundPath.GetUuid(ns, TrakHoundPath.GetParentPath(_path));
            _name = TrakHoundPath.GetObject(_path);
            ContentType = TrakHoundObjectContentTypes.GetString(contentType);
            DefinitionUuid = definitionUuid;
            Priority = priority;
            SourceUuid = sourceUuid;
            Created = created > 0 ? created : UnixDateTime.Now;
        }

        public TrakHoundObjectEntity(ITrakHoundObjectEntity entity)
        {
            _uuid = null;
            _namespace = DefaultNamespace;
            _path = null;
            _parentUuid = null;
            _name = null;
            ContentType = TrakHoundObjectContentTypes.Directory;
            DefinitionUuid = null;
            Priority = DefaultPriority;
            SourceUuid = null;
            Created = UnixDateTime.Now;

            if (entity != null)
            {
                _uuid = entity.Uuid;
                _namespace = entity.Namespace;
                _path = entity.Path;
                _parentUuid = entity.ParentUuid;
                _name = entity.Name;
                ContentType = entity.ContentType;
                DefinitionUuid = entity.DefinitionUuid;
                Priority = entity.Priority;
                SourceUuid = entity.SourceUuid;
                Created = entity.Created;
            }
        }


        public string GetAbsolutePath() => TrakHoundPath.SetNamespace(Namespace, _path);


        public static string GenerateUuid(string path)
        {
            return GenerateUuid(DefaultNamespace, path);
        }

        public static string GenerateUuid(string ns, string path)
        {
            if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(path))
            {
                return $"{ns.ToLower()}:{path.ToLower()}".ToSHA256Hash();
            }

            return null;
        }

        public static string GenerateUuid(string ns, string name, string parentUuid)
        {
            var bytes = GenerateUuidBytes(ns, name, parentUuid);
            return StringFunctions.ToSHA256HashString(bytes);
        }

        public static byte[] GenerateUuidBytes(string ns, string name, byte[] parentUuid)
        {
            if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(name) && !parentUuid.IsNullOrEmpty())
            {
                var bytesToHash = new byte[2][];
                bytesToHash[0] = StringFunctions.ToSHA256HashBytes(name.ToLower());
                bytesToHash[1] = parentUuid;
                return StringFunctions.ToSHA256HashBytes(bytesToHash);
            }
            else if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(name))
            {
                var bytesToHash = new byte[2][];
                bytesToHash[0] = StringFunctions.ToSHA256HashBytes(ns.ToLower());
                bytesToHash[1] = StringFunctions.ToSHA256HashBytes(name.ToLower());
                return StringFunctions.ToSHA256HashBytes(bytesToHash);
            }

            return null;
        }

        public static byte[] GenerateUuidBytes(string ns, string name, string parentUuid)
        {
            if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(name) && !parentUuid.IsNullOrEmpty())
            {
                var bytesToHash = new byte[2][];
                bytesToHash[0] = StringFunctions.ToSHA256HashBytes(name.ToLower());
                bytesToHash[1] = StringFunctions.ConvertHexidecimalToBytes(parentUuid);

                return StringFunctions.ToSHA256HashBytes(bytesToHash);
            }
            else if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(name))
            {
                var bytesToHash = new byte[2][];
                bytesToHash[0] = StringFunctions.ToSHA256HashBytes(ns.ToLower());
                bytesToHash[1] = StringFunctions.ToSHA256HashBytes(name.ToLower());
                return StringFunctions.ToSHA256HashBytes(bytesToHash);
            }

            return null;
        }


        public static byte[] GenerateHash(ITrakHoundObjectEntity obj)
        {
            if (obj != null)
            {
                return $"{obj.Namespace}:{obj.Path}:{obj.ContentType}:{obj.DefinitionUuid}:{obj.Priority}:{obj.SourceUuid}:{obj.Created}".ToSHA256HashBytes();
            }

            return null;
        }


        public override string ToString() => ToJson();

        public string ToJson() => ToArray().ToJson();

        public static ITrakHoundObjectEntity FromJson(string json) => FromArray(Json.Convert<object[]>(json));


        public static object[] ToArray(ITrakHoundObjectEntity obj) => new object[] {
            obj.Namespace,
            obj.Path,
            obj.ContentType,
            obj.DefinitionUuid,
            obj.Priority,
            obj.SourceUuid,
            obj.Created
        };

        public object[] ToArray() => ToArray(this);


        public static ITrakHoundObjectEntity FromArray(object[] obj)
        {
            if (obj != null && obj.Length >= ArraySize)
            {
                return new TrakHoundObjectEntity
                {
                    Namespace = obj[0]?.ToString(),
                    Path = obj[1]?.ToString(),
                    ContentType = obj[2]?.ToString(),
                    DefinitionUuid = obj[3]?.ToString(),
                    Priority = obj[4].ToByte(),
                    SourceUuid = obj[5]?.ToString(),
                    Created = obj[6].ToLong()
                };
            }

            return new TrakHoundObjectEntity { };
        }

        public static IEnumerable<ITrakHoundObjectEntity> FromArray(IEnumerable<object[]> objs)
        {
            if (!objs.IsNullOrEmpty())
            {
                var x = new List<ITrakHoundObjectEntity>();
                foreach (var obj in objs)
                {
                    var y = FromArray(obj);
                    if (y.IsValid) x.Add(y);
                }

                return x;
            }

            return Enumerable.Empty<ITrakHoundObjectEntity>();
        }


        public void SetSource(string sourceUuid, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(SourceUuid) || overwrite) SourceUuid = sourceUuid;
        }
    }
}
