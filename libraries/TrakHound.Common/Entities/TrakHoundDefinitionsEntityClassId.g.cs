// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Definitions Entity Class IDs in the TrakHound Framework
    /// </summary>
    public static class TrakHoundDefinitionsEntityClassId
    {

        public const byte Definition = 1;
        public const byte Metadata = 2;
        public const byte Description = 3;
        public const byte Wiki = 4;


        public static byte Get(string entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundDefinitionsEntityClassName.Definition: return Definition;
                case TrakHoundDefinitionsEntityClassName.Metadata: return Metadata;
                case TrakHoundDefinitionsEntityClassName.Description: return Description;
                case TrakHoundDefinitionsEntityClassName.Wiki: return Wiki;
            }

            return 0;
        }

        public static byte Get(TrakHoundDefinitionsEntityClass entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundDefinitionsEntityClass.Definition: return Definition;
                case TrakHoundDefinitionsEntityClass.Metadata: return Metadata;
                case TrakHoundDefinitionsEntityClass.Description: return Description;
                case TrakHoundDefinitionsEntityClass.Wiki: return Wiki;
            }

            return 0;
        }
    }
}
