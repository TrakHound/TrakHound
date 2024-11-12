// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundTypeAttribute : Attribute
    {
        public string Pattern { get; set; }


        public TrakHoundTypeAttribute() { }

        public TrakHoundTypeAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}
