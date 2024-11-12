// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundBooleanAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string BasePath { get; set; }


        public TrakHoundBooleanAttribute()
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Boolean.ToString();
        }

        public TrakHoundBooleanAttribute(string name)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.Boolean.ToString();
            Name = name;
        }
    }
}
