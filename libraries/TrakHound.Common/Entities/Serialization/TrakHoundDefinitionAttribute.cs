// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Entities;

namespace TrakHound.Serialization
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property, AllowMultiple = true)]
    public class TrakHoundDefinitionAttribute : TrakHoundEntityEntryAttribute
    {
        public string Uuid => Id;

        public string Id { get; set; }

        public string Description { get; set; }

        public string LanguageCode { get; set; }

        public string ParentId { get; set; }


        public TrakHoundDefinitionAttribute()
        {
            Category = TrakHoundEntityCategoryName.Definitions;
            Class = TrakHoundDefinitionsEntityClassName.Definition;
        }

        public TrakHoundDefinitionAttribute(string id, string description = null, string languageCode = TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode, string parentId = null)
        {
            Category = TrakHoundEntityCategoryName.Definitions;
            Class = TrakHoundDefinitionsEntityClassName.Definition;
            Id = id;
            Description = description;
            LanguageCode = languageCode;
            ParentId = parentId;
        }
    }
}
