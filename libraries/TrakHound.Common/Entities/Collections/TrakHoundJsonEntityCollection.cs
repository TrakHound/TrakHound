// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Entities.Collections
{
    public class TrakHoundJsonEntityCollection
    {
        [JsonPropertyName("targetUuids")]
        public IEnumerable<string> TargetUuids { get; set; }

        [JsonPropertyName("objects")]
        public TrakHoundJsonObjectCollection Objects { get; set; }

        [JsonPropertyName("definitions")]
        public TrakHoundJsonDefinitionCollection Definitions { get; set; }

        [JsonPropertyName("sources")]
        public TrakHoundJsonSourceCollection Sources { get; set; }


        public TrakHoundJsonEntityCollection() { }

        public TrakHoundJsonEntityCollection(TrakHoundEntityCollection collection) 
        { 
            if (collection != null)
            {
                if (!collection.TargetUuids.IsNullOrEmpty()) TargetUuids = collection.TargetUuids;
                if (!collection.Objects.IsEmpty) Objects = new TrakHoundJsonObjectCollection(collection.Objects);
                if (!collection.Definitions.IsEmpty) Definitions = new TrakHoundJsonDefinitionCollection(collection.Definitions);
                if (!collection.Sources.IsEmpty) Sources = new TrakHoundJsonSourceCollection(collection.Sources);
            }
        }


        public TrakHoundEntityCollection ToCollection()
        {
            var collection = new TrakHoundEntityCollection();
            collection.AddTargetIds(TargetUuids);
            collection.Add(GetEntities(), false);
            return collection;
        }


        public IEnumerable<ITrakHoundEntity> GetEntities()
        {
            var entities = new List<ITrakHoundEntity>();

            IEnumerable<ITrakHoundEntity> categoryEntities;

            // Objects
            categoryEntities = Objects?.GetEntities();
            if (!categoryEntities.IsNullOrEmpty()) entities.AddRange(categoryEntities);

            // Definitions
            categoryEntities = Definitions?.GetEntities();
            if (!categoryEntities.IsNullOrEmpty()) entities.AddRange(categoryEntities);

            // Sources
            categoryEntities = Sources?.GetEntities();
            if (!categoryEntities.IsNullOrEmpty()) entities.AddRange(categoryEntities);

            return entities;
        }
    }
}
