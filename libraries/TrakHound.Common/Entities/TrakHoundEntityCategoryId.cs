// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public static class TrakHoundEntityCategoryId
    {
        public const byte Objects = 1;
        public const byte Definitions = 2;
        public const byte Sources = 3;


        public static byte Get(string category)
        {
            switch (category)
            {
                case TrakHoundEntityCategoryName.Objects: return Objects;
                case TrakHoundEntityCategoryName.Definitions: return Definitions;
                case TrakHoundEntityCategoryName.Sources: return Sources;
            }

            return 0;
        }
    }
}
