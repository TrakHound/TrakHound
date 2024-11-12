// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Definitions Entity Class Names in the TrakHound Framework
    /// </summary>
    public static class TrakHoundDefinitionsEntityClassName
    {

        public const string Definition = "Definition";
        public const string Metadata = "Metadata";
        public const string Description = "Description";
        public const string Wiki = "Wiki";


        public static string Get(byte entityClassId)
        {
            switch (entityClassId)
            {

                case TrakHoundDefinitionsEntityClassId.Definition: return Definition;
                case TrakHoundDefinitionsEntityClassId.Metadata: return Metadata;
                case TrakHoundDefinitionsEntityClassId.Description: return Description;
                case TrakHoundDefinitionsEntityClassId.Wiki: return Wiki;
            }

            return null;
        }

        public static string Get(TrakHoundDefinitionsEntityClass entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundDefinitionsEntityClass.Definition: return Definition;
                case TrakHoundDefinitionsEntityClass.Metadata: return Metadata;
                case TrakHoundDefinitionsEntityClass.Description: return Description;
                case TrakHoundDefinitionsEntityClass.Wiki: return Wiki;
            }

            return null;
        }
    }
}
