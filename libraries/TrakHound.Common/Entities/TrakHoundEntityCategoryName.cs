// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public static class TrakHoundEntityCategoryName
    {
        public const string Objects = "Objects";
        public const string Definitions = "Definitions";
        public const string Sources = "Sources";


        public static string Get(byte categoryId)
        {
            switch (categoryId)
            {
                case TrakHoundEntityCategoryId.Objects: return Objects;
                case TrakHoundEntityCategoryId.Definitions: return Definitions;
                case TrakHoundEntityCategoryId.Sources: return Sources;
            }

            return null;
        }
    }
}
