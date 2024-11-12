// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundStringAttribute : TrakHoundEntityEntryAttribute
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string BasePath { get; set; }


        public TrakHoundStringAttribute()
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.String.ToString();
        }

        public TrakHoundStringAttribute(string name)
        {
            Category = TrakHoundEntityCategory.Objects.ToString();
            Class = TrakHoundObjectsEntityClass.String.ToString();
            Name = name;
        }
    }
}
