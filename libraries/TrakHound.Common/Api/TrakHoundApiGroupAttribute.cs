// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public class TrakHoundApiGroupAttribute : Attribute
    {
        public string Name { get; set; }


        public TrakHoundApiGroupAttribute(string name)
        {
            Name = name;
        }
    }
}
