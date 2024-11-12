// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrakHoundPropertyDefinitionAttribute : TrakHoundEntityEntryAttribute
    {
        public string Type { get; set; }

        public string Description { get; set; }

        public string LanguageCode { get; set; }

        public string ParentId { get; set; }


        public TrakHoundPropertyDefinitionAttribute()
        {
            Category = TrakHoundEntityCategoryName.Definitions;
            Class = TrakHoundDefinitionsEntityClassName.Definition;
            LanguageCode = TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode;
        }

        public TrakHoundPropertyDefinitionAttribute(string type, string description = null, string languageCode = TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode, string parentId = null)
        {
            Category = TrakHoundEntityCategoryName.Definitions;
            Class = TrakHoundDefinitionsEntityClassName.Definition;
            Type = type;
            Description = description;
            LanguageCode = languageCode;
            ParentId = parentId;
        }
    }
}
