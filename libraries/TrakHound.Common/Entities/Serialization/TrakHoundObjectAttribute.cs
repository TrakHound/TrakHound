// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundObjectAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string BasePath { get; set; }

        public TrakHoundObjectContentType ContentType { get; set; }

        public byte Priority { get; set; }


        public TrakHoundObjectAttribute()
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Object.ToString();
        }

        public TrakHoundObjectAttribute(string name, TrakHoundObjectContentType contentType = TrakHoundObjectContentType.Directory)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Object.ToString();
            Name = name;
            ContentType = contentType;
        }
    }
}
