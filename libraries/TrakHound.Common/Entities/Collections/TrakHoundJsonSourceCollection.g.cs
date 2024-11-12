// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Entities.Collections
{
    public class TrakHoundJsonSourceCollection
    {
        [JsonPropertyName("source")]
        public IEnumerable<object[]> Source { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<object[]> Metadata { get; set; }



        public TrakHoundJsonSourceCollection() { }

        public TrakHoundJsonSourceCollection(TrakHoundSourceCollection collection)
        {
            if (collection != null)
            {
                Source = collection.GetSourceArrays();
                Metadata = collection.GetMetadataArrays();
            }
        }


        public IEnumerable<ITrakHoundEntity> GetEntities()
        {
            var entities = new List<ITrakHoundEntity>();

            if (!Source.IsNullOrEmpty())
            {
                foreach (var a in Source) entities.Add(TrakHoundEntity.FromArray<ITrakHoundSourceEntity>(a));
            }

            if (!Metadata.IsNullOrEmpty())
            {
                foreach (var a in Metadata) entities.Add(TrakHoundEntity.FromArray<ITrakHoundSourceMetadataEntity>(a));
            }

            return entities;
        }
    }
}
