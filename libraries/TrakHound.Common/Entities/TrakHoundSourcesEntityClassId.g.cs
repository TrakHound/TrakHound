// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Sources Entity Class IDs in the TrakHound Framework
    /// </summary>
    public static class TrakHoundSourcesEntityClassId
    {

        public const byte Source = 1;
        public const byte Metadata = 2;


        public static byte Get(string entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundSourcesEntityClassName.Source: return Source;
                case TrakHoundSourcesEntityClassName.Metadata: return Metadata;
            }

            return 0;
        }

        public static byte Get(TrakHoundSourcesEntityClass entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundSourcesEntityClass.Source: return Source;
                case TrakHoundSourcesEntityClass.Metadata: return Metadata;
            }

            return 0;
        }
    }
}
