// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Objects Entities in the TrakHound Framework
    /// </summary>
    public static class TrakHoundObjectsEntity
    {

        public const string Object = "Object";
        public const string Metadata = "Metadata";
        public const string Assignment = "Assignment";
        public const string Blob = "Blob";
        public const string Boolean = "Boolean";
        public const string Duration = "Duration";
        public const string Event = "Event";
        public const string Group = "Group";
        public const string Hash = "Hash";
        public const string Log = "Log";
        public const string Message = "Message";
        public const string MessageQueue = "MessageQueue";
        public const string Number = "Number";
        public const string Observation = "Observation";
        public const string Queue = "Queue";
        public const string Reference = "Reference";
        public const string Set = "Set";
        public const string State = "State";
        public const string Statistic = "Statistic";
        public const string String = "String";
        public const string TimeRange = "TimeRange";
        public const string Timestamp = "Timestamp";
        public const string Vocabulary = "Vocabulary";
        public const string VocabularySet = "VocabularySet";


        public static bool IsObjectsEntity<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity))) return true;


            return false;
        }

        public static bool IsObjectsEntity(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(type)) return true;

            }

            return false;
        }


        public static string GetEntityClass<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity)))
                return Object;

            else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return Metadata;

            else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity)))
                return Assignment;

            else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity)))
                return Blob;

            else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity)))
                return Boolean;

            else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity)))
                return Duration;

            else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity)))
                return Event;

            else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity)))
                return Group;

            else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity)))
                return Hash;

            else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity)))
                return Log;

            else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity)))
                return Message;

            else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity)))
                return MessageQueue;

            else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity)))
                return Number;

            else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity)))
                return Observation;

            else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity)))
                return Queue;

            else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity)))
                return Reference;

            else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity)))
                return Set;

            else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity)))
                return State;

            else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity)))
                return Statistic;

            else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity)))
                return String;

            else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity)))
                return TimeRange;

            else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity)))
                return Timestamp;

            else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity)))
                return Vocabulary;

            else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity)))
                return VocabularySet;


            return default;
        }

        public static byte GetEntityClassId<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Object;

            else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Metadata;

            else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Assignment;

            else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Blob;

            else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Boolean;

            else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Duration;

            else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Event;

            else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Group;

            else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Hash;

            else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Log;

            else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Message;

            else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.MessageQueue;

            else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Number;

            else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Observation;

            else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Queue;

            else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Reference;

            else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Set;

            else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.State;

            else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Statistic;

            else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.String;

            else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.TimeRange;

            else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Timestamp;

            else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.Vocabulary;

            else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundObjectsEntityClassId.VocabularySet;


            return 0;
        }

        public static IEnumerable<string> GetEntityClasses()
        {
            var classes = new List<string>();

            foreach (var entityClass in Enum.GetValues(typeof(TrakHoundObjectsEntityClass)))
            {
                classes.Add(entityClass.ToString());
            }

            return classes;
        }

        public static IEnumerable<byte> GetEntityClassIds()
        {
            var classes = new List<byte>();

            foreach (TrakHoundObjectsEntityClass entityClass in Enum.GetValues(typeof(TrakHoundObjectsEntityClass)))
            {
                classes.Add(TrakHoundObjectsEntityClassId.Get(entityClass));
            }

            return classes;
        }

        public static TEntity FromJson<TEntity>(string json) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(json))
            {

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectMetadataEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectAssignmentEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectBlobEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectBooleanEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectDurationEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectEventEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectGroupEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectHashEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectLogEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectMessageEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectMessageQueueEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectNumberEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectObservationEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectQueueEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectReferenceEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectSetEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectStateEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectStatisticEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectStringEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectTimeRangeEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectTimestampEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectVocabularyEntity.FromJson(json);

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectVocabularySetEntity.FromJson(json);

            }

            return default;
        }

        public static ITrakHoundEntity FromJson(byte entityClass, string json)
        {
            if (entityClass > 0 && !string.IsNullOrEmpty(json))
            {
                switch (entityClass)
                {

                    case TrakHoundObjectsEntityClassId.Object: return TrakHoundObjectEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Metadata: return TrakHoundObjectMetadataEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Assignment: return TrakHoundObjectAssignmentEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Blob: return TrakHoundObjectBlobEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Boolean: return TrakHoundObjectBooleanEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Duration: return TrakHoundObjectDurationEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Event: return TrakHoundObjectEventEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Group: return TrakHoundObjectGroupEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Hash: return TrakHoundObjectHashEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Log: return TrakHoundObjectLogEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Message: return TrakHoundObjectMessageEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.MessageQueue: return TrakHoundObjectMessageQueueEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Number: return TrakHoundObjectNumberEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Observation: return TrakHoundObjectObservationEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Queue: return TrakHoundObjectQueueEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Reference: return TrakHoundObjectReferenceEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Set: return TrakHoundObjectSetEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.State: return TrakHoundObjectStateEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Statistic: return TrakHoundObjectStatisticEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.String: return TrakHoundObjectStringEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.TimeRange: return TrakHoundObjectTimeRangeEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Timestamp: return TrakHoundObjectTimestampEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.Vocabulary: return TrakHoundObjectVocabularyEntity.FromJson(json);
                    case TrakHoundObjectsEntityClassId.VocabularySet: return TrakHoundObjectVocabularySetEntity.FromJson(json);
                }
            }

            return default;
        }


        public static string ToJson(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectEntity((ITrakHoundObjectEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectMetadataEntity((ITrakHoundObjectMetadataEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectAssignmentEntity((ITrakHoundObjectAssignmentEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectBlobEntity((ITrakHoundObjectBlobEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectBooleanEntity((ITrakHoundObjectBooleanEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectDurationEntity((ITrakHoundObjectDurationEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectEventEntity((ITrakHoundObjectEventEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectGroupEntity((ITrakHoundObjectGroupEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectHashEntity((ITrakHoundObjectHashEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectLogEntity((ITrakHoundObjectLogEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectMessageEntity((ITrakHoundObjectMessageEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectMessageQueueEntity((ITrakHoundObjectMessageQueueEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectNumberEntity((ITrakHoundObjectNumberEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectObservationEntity((ITrakHoundObjectObservationEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectQueueEntity((ITrakHoundObjectQueueEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectReferenceEntity((ITrakHoundObjectReferenceEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectSetEntity((ITrakHoundObjectSetEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectStateEntity((ITrakHoundObjectStateEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectStatisticEntity((ITrakHoundObjectStatisticEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectStringEntity((ITrakHoundObjectStringEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectTimeRangeEntity((ITrakHoundObjectTimeRangeEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectTimestampEntity((ITrakHoundObjectTimestampEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectVocabularyEntity((ITrakHoundObjectVocabularyEntity)entity).ToJson();

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectVocabularySetEntity((ITrakHoundObjectVocabularySetEntity)entity).ToJson();

            }

            return null;
        }


        public static TEntity FromArray<TEntity>(object[] a) where TEntity : ITrakHoundEntity
        {
            if (a != null)
            {

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectMetadataEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectAssignmentEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectBlobEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectBooleanEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectDurationEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectEventEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectGroupEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectHashEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectLogEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectMessageEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectMessageQueueEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectNumberEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectObservationEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectQueueEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectReferenceEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectSetEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectStateEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectStatisticEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectStringEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectTimeRangeEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectTimestampEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectVocabularyEntity.FromArray(a);

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundObjectVocabularySetEntity.FromArray(a);

            }

            return default;
        }

        public static ITrakHoundEntity FromArray(string entityClass, object[] a)
        {
            if (!string.IsNullOrEmpty(entityClass) && !a.IsNullOrEmpty())
            {
                var x = entityClass.ToPascalCase().ConvertEnum<TrakHoundObjectsEntityClass>();
                switch (x)
                {

                    case TrakHoundObjectsEntityClass.Object: return TrakHoundObjectEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Metadata: return TrakHoundObjectMetadataEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Assignment: return TrakHoundObjectAssignmentEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Blob: return TrakHoundObjectBlobEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Boolean: return TrakHoundObjectBooleanEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Duration: return TrakHoundObjectDurationEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Event: return TrakHoundObjectEventEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Group: return TrakHoundObjectGroupEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Hash: return TrakHoundObjectHashEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Log: return TrakHoundObjectLogEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Message: return TrakHoundObjectMessageEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.MessageQueue: return TrakHoundObjectMessageQueueEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Number: return TrakHoundObjectNumberEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Observation: return TrakHoundObjectObservationEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Queue: return TrakHoundObjectQueueEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Reference: return TrakHoundObjectReferenceEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Set: return TrakHoundObjectSetEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.State: return TrakHoundObjectStateEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Statistic: return TrakHoundObjectStatisticEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.String: return TrakHoundObjectStringEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.TimeRange: return TrakHoundObjectTimeRangeEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Timestamp: return TrakHoundObjectTimestampEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.Vocabulary: return TrakHoundObjectVocabularyEntity.FromArray(a);
                    case TrakHoundObjectsEntityClass.VocabularySet: return TrakHoundObjectVocabularySetEntity.FromArray(a);
                }
            }

            return default;
        }


        public static object[] ToArray(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundObjectEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectEntity((ITrakHoundObjectEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectMetadataEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectMetadataEntity((ITrakHoundObjectMetadataEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectAssignmentEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectAssignmentEntity((ITrakHoundObjectAssignmentEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectBlobEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectBlobEntity((ITrakHoundObjectBlobEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectBooleanEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectBooleanEntity((ITrakHoundObjectBooleanEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectDurationEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectDurationEntity((ITrakHoundObjectDurationEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectEventEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectEventEntity((ITrakHoundObjectEventEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectGroupEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectGroupEntity((ITrakHoundObjectGroupEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectHashEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectHashEntity((ITrakHoundObjectHashEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectLogEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectLogEntity((ITrakHoundObjectLogEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectMessageEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectMessageEntity((ITrakHoundObjectMessageEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectMessageQueueEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectMessageQueueEntity((ITrakHoundObjectMessageQueueEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectNumberEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectNumberEntity((ITrakHoundObjectNumberEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectObservationEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectObservationEntity((ITrakHoundObjectObservationEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectQueueEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectQueueEntity((ITrakHoundObjectQueueEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectReferenceEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectReferenceEntity((ITrakHoundObjectReferenceEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectSetEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectSetEntity((ITrakHoundObjectSetEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectStateEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectStateEntity((ITrakHoundObjectStateEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectStatisticEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectStatisticEntity((ITrakHoundObjectStatisticEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectStringEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectStringEntity((ITrakHoundObjectStringEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectTimeRangeEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectTimeRangeEntity((ITrakHoundObjectTimeRangeEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectTimestampEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectTimestampEntity((ITrakHoundObjectTimestampEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectVocabularyEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectVocabularyEntity((ITrakHoundObjectVocabularyEntity)entity).ToArray();

                else if (typeof(ITrakHoundObjectVocabularySetEntity).IsAssignableFrom(type))
                    return new TrakHoundObjectVocabularySetEntity((ITrakHoundObjectVocabularySetEntity)entity).ToArray();

            }

            return null;
        }
    }
}
