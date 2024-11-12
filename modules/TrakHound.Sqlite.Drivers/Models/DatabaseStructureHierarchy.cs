// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Sqlite.Drivers.Models
{
    public class DatabaseStructureHierarchy
    {
        public string RequestedId { get; set; }

        public string Uuid { get; set; }

        public int TypeId { get; set; }

        public HierarchyType Type
        {
            get
            {
                switch (TypeId)
                {
                    case -1: return HierarchyType.Root;
                    case 1: return HierarchyType.Child;
                    default: return HierarchyType.Target;
                }
            }
        }
    }
}
