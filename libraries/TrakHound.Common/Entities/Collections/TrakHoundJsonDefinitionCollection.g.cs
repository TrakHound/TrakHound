// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Entities.Collections
{
    public class TrakHoundJsonDefinitionCollection
    {
        [JsonPropertyName("definition")]
        public IEnumerable<object[]> Definition { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<object[]> Metadata { get; set; }

        [JsonPropertyName("description")]
        public IEnumerable<object[]> Description { get; set; }

        [JsonPropertyName("wiki")]
        public IEnumerable<object[]> Wiki { get; set; }



        public TrakHoundJsonDefinitionCollection() { }

        public TrakHoundJsonDefinitionCollection(TrakHoundDefinitionCollection collection)
        {
            if (collection != null)
            {
                Definition = collection.GetDefinitionArrays();
                Metadata = collection.GetMetadataArrays();
                Description = collection.GetDescriptionArrays();
                Wiki = collection.GetWikiArrays();
            }
        }


        public IEnumerable<ITrakHoundEntity> GetEntities()
        {
            var entities = new List<ITrakHoundEntity>();

            if (!Definition.IsNullOrEmpty())
            {
                foreach (var a in Definition) entities.Add(TrakHoundEntity.FromArray<ITrakHoundDefinitionEntity>(a));
            }

            if (!Metadata.IsNullOrEmpty())
            {
                foreach (var a in Metadata) entities.Add(TrakHoundEntity.FromArray<ITrakHoundDefinitionMetadataEntity>(a));
            }

            if (!Description.IsNullOrEmpty())
            {
                foreach (var a in Description) entities.Add(TrakHoundEntity.FromArray<ITrakHoundDefinitionDescriptionEntity>(a));
            }

            if (!Wiki.IsNullOrEmpty())
            {
                foreach (var a in Wiki) entities.Add(TrakHoundEntity.FromArray<ITrakHoundDefinitionWikiEntity>(a));
            }

            return entities;
        }
    }
}
