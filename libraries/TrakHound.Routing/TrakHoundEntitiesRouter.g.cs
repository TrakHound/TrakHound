// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;
using TrakHound.Routing.Routers;

namespace TrakHound.Routing
{
    public partial class TrakHoundEntitiesRouter
    {
        public ITrakHoundEntityRouter<TEntity> GetEntityRouter<TEntity>() where TEntity : ITrakHoundEntity
        {
            var entityCategory = TrakHoundEntity.GetEntityCategory<TEntity>().ConvertEnum<TrakHoundEntityCategory>();

            switch (entityCategory)
            {
                case TrakHoundEntityCategory.Sources:
                    switch (TrakHoundEntity.GetEntityClass<TEntity>().ConvertEnum<TrakHoundSourcesEntityClass>())
                    {

                        case TrakHoundSourcesEntityClass.Source: return (ITrakHoundEntityRouter<TEntity>)_sources.Sources;
                        case TrakHoundSourcesEntityClass.Metadata: return (ITrakHoundEntityRouter<TEntity>)_sources.Metadata;
                    }
                    break;

                case TrakHoundEntityCategory.Definitions:
                    switch (TrakHoundEntity.GetEntityClass<TEntity>().ConvertEnum<TrakHoundDefinitionsEntityClass>())
                    {

                        case TrakHoundDefinitionsEntityClass.Definition: return (ITrakHoundEntityRouter<TEntity>)_definitions.Definitions;
                        case TrakHoundDefinitionsEntityClass.Metadata: return (ITrakHoundEntityRouter<TEntity>)_definitions.Metadata;
                        case TrakHoundDefinitionsEntityClass.Description: return (ITrakHoundEntityRouter<TEntity>)_definitions.Descriptions;
                        case TrakHoundDefinitionsEntityClass.Wiki: return (ITrakHoundEntityRouter<TEntity>)_definitions.Wikis;
                    }
                    break;

                case TrakHoundEntityCategory.Objects:
                    switch (TrakHoundEntity.GetEntityClass<TEntity>().ConvertEnum<TrakHoundObjectsEntityClass>())
                    {

                        case TrakHoundObjectsEntityClass.Object: return (ITrakHoundEntityRouter<TEntity>)_objects.Objects;
                        case TrakHoundObjectsEntityClass.Metadata: return (ITrakHoundEntityRouter<TEntity>)_objects.Metadata;
                        case TrakHoundObjectsEntityClass.Assignment: return (ITrakHoundEntityRouter<TEntity>)_objects.Assignments;
                        case TrakHoundObjectsEntityClass.Blob: return (ITrakHoundEntityRouter<TEntity>)_objects.Blobs;
                        case TrakHoundObjectsEntityClass.Boolean: return (ITrakHoundEntityRouter<TEntity>)_objects.Booleans;
                        case TrakHoundObjectsEntityClass.Duration: return (ITrakHoundEntityRouter<TEntity>)_objects.Durations;
                        case TrakHoundObjectsEntityClass.Event: return (ITrakHoundEntityRouter<TEntity>)_objects.Events;
                        case TrakHoundObjectsEntityClass.Group: return (ITrakHoundEntityRouter<TEntity>)_objects.Groups;
                        case TrakHoundObjectsEntityClass.Hash: return (ITrakHoundEntityRouter<TEntity>)_objects.Hashes;
                        case TrakHoundObjectsEntityClass.Log: return (ITrakHoundEntityRouter<TEntity>)_objects.Logs;
                        case TrakHoundObjectsEntityClass.Message: return (ITrakHoundEntityRouter<TEntity>)_objects.Messages;
                        case TrakHoundObjectsEntityClass.MessageQueue: return (ITrakHoundEntityRouter<TEntity>)_objects.MessageQueues;
                        case TrakHoundObjectsEntityClass.Number: return (ITrakHoundEntityRouter<TEntity>)_objects.Numbers;
                        case TrakHoundObjectsEntityClass.Observation: return (ITrakHoundEntityRouter<TEntity>)_objects.Observations;
                        case TrakHoundObjectsEntityClass.Queue: return (ITrakHoundEntityRouter<TEntity>)_objects.Queues;
                        case TrakHoundObjectsEntityClass.Reference: return (ITrakHoundEntityRouter<TEntity>)_objects.References;
                        case TrakHoundObjectsEntityClass.Set: return (ITrakHoundEntityRouter<TEntity>)_objects.Sets;
                        case TrakHoundObjectsEntityClass.State: return (ITrakHoundEntityRouter<TEntity>)_objects.States;
                        case TrakHoundObjectsEntityClass.Statistic: return (ITrakHoundEntityRouter<TEntity>)_objects.Statistics;
                        case TrakHoundObjectsEntityClass.String: return (ITrakHoundEntityRouter<TEntity>)_objects.Strings;
                        case TrakHoundObjectsEntityClass.TimeRange: return (ITrakHoundEntityRouter<TEntity>)_objects.TimeRanges;
                        case TrakHoundObjectsEntityClass.Timestamp: return (ITrakHoundEntityRouter<TEntity>)_objects.Timestamps;
                        case TrakHoundObjectsEntityClass.Vocabulary: return (ITrakHoundEntityRouter<TEntity>)_objects.Vocabularies;
                        case TrakHoundObjectsEntityClass.VocabularySet: return (ITrakHoundEntityRouter<TEntity>)_objects.VocabularySets;
                    }
                    break;
            }

            return null;
        }
    }
}
