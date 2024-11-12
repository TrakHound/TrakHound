// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrakHoundMetadataAttribute : Attribute
    {
        public string Name { get; set; }


        public TrakHoundMetadataAttribute() { }

        public TrakHoundMetadataAttribute(string name)
        {
            Name = name;
        }
    }
}
