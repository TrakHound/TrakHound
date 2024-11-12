// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Security
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TrakHoundPermissionAttribute : Attribute
    {
        public string Permission { get; set; }


        public TrakHoundPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
