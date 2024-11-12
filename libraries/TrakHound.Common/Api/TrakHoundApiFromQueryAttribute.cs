// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public class FromQueryAttribute : Attribute
    {
        public string ParameterName { get; set; }


        public FromQueryAttribute() { }

        public FromQueryAttribute(string name)
        {
            ParameterName = name;
        }
    }
}
