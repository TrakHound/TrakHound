// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrakHoundPathParameterAttribute : Attribute
    {
        public string Name { get; set; }


        public TrakHoundPathParameterAttribute() { }

        public TrakHoundPathParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
