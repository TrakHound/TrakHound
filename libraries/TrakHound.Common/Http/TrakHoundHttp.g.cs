// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Http
{
    public static partial class TrakHoundHttp
    {
        public static string GetSourceEntityPath<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity))) return "sources";

            else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity))) return "sources/metadata";


            return null;
        }

        public static string GetDefinitionEntityPath<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity))) return "definitions";

            else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity))) return "definitions/metadata";

            else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity))) return "definitions/description";

            else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity))) return "definitions/wiki";


            return null;
        }

        public static string GetObjectEntityPath<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity))) return "objects";

            else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity))) return "objects/metadata";

            else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity))) return "objects/assignment";

            else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity))) return "objects/blob";

            else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity))) return "objects/boolean";

            else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity))) return "objects/duration";

            else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity))) return "objects/event";

            else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity))) return "objects/group";

            else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity))) return "objects/hash";

            else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity))) return "objects/log";

            else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity))) return "objects/message";

            else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity))) return "objects/message-queue";

            else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity))) return "objects/number";

            else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity))) return "objects/observation";

            else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity))) return "objects/queue";

            else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity))) return "objects/reference";

            else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity))) return "objects/set";

            else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity))) return "objects/state";

            else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity))) return "objects/statistic";

            else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity))) return "objects/string";

            else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity))) return "objects/time-range";

            else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity))) return "objects/timestamp";

            else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity))) return "objects/vocabulary";

            else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity))) return "objects/vocabulary-set";


            return null;
        }
    }
}
