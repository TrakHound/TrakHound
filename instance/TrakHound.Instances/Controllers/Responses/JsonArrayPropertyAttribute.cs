// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Controllers.Http.Responses
{
    public class JsonArrayPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }


        public JsonArrayPropertyAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
