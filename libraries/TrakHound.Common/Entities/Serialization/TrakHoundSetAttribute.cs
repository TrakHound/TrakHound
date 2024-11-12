// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundSetAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string BasePath { get; set; }

        public TrakHoundEntryType EntryType { get; set; }


        public TrakHoundSetAttribute()
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Set.ToString();
        }

        public TrakHoundSetAttribute(TrakHoundEntryType entryType)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Set.ToString();
            EntryType = entryType;
        }

        public TrakHoundSetAttribute(string name)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Set.ToString();
            Name = name;
        }
    }
}
