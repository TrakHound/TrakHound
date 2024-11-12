// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Sources Entity Class Names in the TrakHound Framework
    /// </summary>
    public static class TrakHoundSourcesEntityClassName
    {

        public const string Source = "Source";
        public const string Metadata = "Metadata";


        public static string Get(byte entityClassId)
        {
            switch (entityClassId)
            {

                case TrakHoundSourcesEntityClassId.Source: return Source;
                case TrakHoundSourcesEntityClassId.Metadata: return Metadata;
            }

            return null;
        }

        public static string Get(TrakHoundSourcesEntityClass entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundSourcesEntityClass.Source: return Source;
                case TrakHoundSourcesEntityClass.Metadata: return Metadata;
            }

            return null;
        }
    }
}
