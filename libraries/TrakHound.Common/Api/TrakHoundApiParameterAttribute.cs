// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakHound.Api
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TrakHoundApiParameterAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }


        public TrakHoundApiParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
