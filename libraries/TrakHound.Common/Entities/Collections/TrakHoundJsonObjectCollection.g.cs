// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TrakHound.Entities.Collections
{
    public class TrakHoundJsonObjectCollection
    {
        [JsonPropertyName("object")]
        public IEnumerable<object[]> Object { get; set; }

        [JsonPropertyName("metadata")]
        public IEnumerable<object[]> Metadata { get; set; }

        [JsonPropertyName("assignment")]
        public IEnumerable<object[]> Assignment { get; set; }

        [JsonPropertyName("blob")]
        public IEnumerable<object[]> Blob { get; set; }

        [JsonPropertyName("boolean")]
        public IEnumerable<object[]> Boolean { get; set; }

        [JsonPropertyName("duration")]
        public IEnumerable<object[]> Duration { get; set; }

        [JsonPropertyName("event")]
        public IEnumerable<object[]> Event { get; set; }

        [JsonPropertyName("group")]
        public IEnumerable<object[]> Group { get; set; }

        [JsonPropertyName("hash")]
        public IEnumerable<object[]> Hash { get; set; }

        [JsonPropertyName("log")]
        public IEnumerable<object[]> Log { get; set; }

        [JsonPropertyName("message")]
        public IEnumerable<object[]> Message { get; set; }

        [JsonPropertyName("messagequeue")]
        public IEnumerable<object[]> MessageQueue { get; set; }

        [JsonPropertyName("number")]
        public IEnumerable<object[]> Number { get; set; }

        [JsonPropertyName("observation")]
        public IEnumerable<object[]> Observation { get; set; }

        [JsonPropertyName("queue")]
        public IEnumerable<object[]> Queue { get; set; }

        [JsonPropertyName("reference")]
        public IEnumerable<object[]> Reference { get; set; }

        [JsonPropertyName("set")]
        public IEnumerable<object[]> Set { get; set; }

        [JsonPropertyName("state")]
        public IEnumerable<object[]> State { get; set; }

        [JsonPropertyName("statistic")]
        public IEnumerable<object[]> Statistic { get; set; }

        [JsonPropertyName("string")]
        public IEnumerable<object[]> String { get; set; }

        [JsonPropertyName("timerange")]
        public IEnumerable<object[]> TimeRange { get; set; }

        [JsonPropertyName("timestamp")]
        public IEnumerable<object[]> Timestamp { get; set; }

        [JsonPropertyName("vocabulary")]
        public IEnumerable<object[]> Vocabulary { get; set; }

        [JsonPropertyName("vocabularyset")]
        public IEnumerable<object[]> VocabularySet { get; set; }



        public TrakHoundJsonObjectCollection() { }

        public TrakHoundJsonObjectCollection(TrakHoundObjectCollection collection)
        {
            if (collection != null)
            {
                Object = collection.GetObjectArrays();
                Metadata = collection.GetMetadataArrays();
                Assignment = collection.GetAssignmentArrays();
                Blob = collection.GetBlobArrays();
                Boolean = collection.GetBooleanArrays();
                Duration = collection.GetDurationArrays();
                Event = collection.GetEventArrays();
                Group = collection.GetGroupArrays();
                Hash = collection.GetHashArrays();
                Log = collection.GetLogArrays();
                Message = collection.GetMessageArrays();
                MessageQueue = collection.GetMessageQueueArrays();
                Number = collection.GetNumberArrays();
                Observation = collection.GetObservationArrays();
                Queue = collection.GetQueueArrays();
                Reference = collection.GetReferenceArrays();
                Set = collection.GetSetArrays();
                State = collection.GetStateArrays();
                Statistic = collection.GetStatisticArrays();
                String = collection.GetStringArrays();
                TimeRange = collection.GetTimeRangeArrays();
                Timestamp = collection.GetTimestampArrays();
                Vocabulary = collection.GetVocabularyArrays();
                VocabularySet = collection.GetVocabularySetArrays();
            }
        }


        public IEnumerable<ITrakHoundEntity> GetEntities()
        {
            var entities = new List<ITrakHoundEntity>();

            if (!Object.IsNullOrEmpty())
            {
                foreach (var a in Object) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectEntity>(a));
            }

            if (!Metadata.IsNullOrEmpty())
            {
                foreach (var a in Metadata) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectMetadataEntity>(a));
            }

            if (!Assignment.IsNullOrEmpty())
            {
                foreach (var a in Assignment) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectAssignmentEntity>(a));
            }

            if (!Blob.IsNullOrEmpty())
            {
                foreach (var a in Blob) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectBlobEntity>(a));
            }

            if (!Boolean.IsNullOrEmpty())
            {
                foreach (var a in Boolean) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectBooleanEntity>(a));
            }

            if (!Duration.IsNullOrEmpty())
            {
                foreach (var a in Duration) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectDurationEntity>(a));
            }

            if (!Event.IsNullOrEmpty())
            {
                foreach (var a in Event) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectEventEntity>(a));
            }

            if (!Group.IsNullOrEmpty())
            {
                foreach (var a in Group) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectGroupEntity>(a));
            }

            if (!Hash.IsNullOrEmpty())
            {
                foreach (var a in Hash) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectHashEntity>(a));
            }

            if (!Log.IsNullOrEmpty())
            {
                foreach (var a in Log) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectLogEntity>(a));
            }

            if (!Message.IsNullOrEmpty())
            {
                foreach (var a in Message) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectMessageEntity>(a));
            }

            if (!MessageQueue.IsNullOrEmpty())
            {
                foreach (var a in MessageQueue) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectMessageQueueEntity>(a));
            }

            if (!Number.IsNullOrEmpty())
            {
                foreach (var a in Number) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectNumberEntity>(a));
            }

            if (!Observation.IsNullOrEmpty())
            {
                foreach (var a in Observation) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectObservationEntity>(a));
            }

            if (!Queue.IsNullOrEmpty())
            {
                foreach (var a in Queue) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectQueueEntity>(a));
            }

            if (!Reference.IsNullOrEmpty())
            {
                foreach (var a in Reference) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectReferenceEntity>(a));
            }

            if (!Set.IsNullOrEmpty())
            {
                foreach (var a in Set) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectSetEntity>(a));
            }

            if (!State.IsNullOrEmpty())
            {
                foreach (var a in State) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectStateEntity>(a));
            }

            if (!Statistic.IsNullOrEmpty())
            {
                foreach (var a in Statistic) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectStatisticEntity>(a));
            }

            if (!String.IsNullOrEmpty())
            {
                foreach (var a in String) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectStringEntity>(a));
            }

            if (!TimeRange.IsNullOrEmpty())
            {
                foreach (var a in TimeRange) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectTimeRangeEntity>(a));
            }

            if (!Timestamp.IsNullOrEmpty())
            {
                foreach (var a in Timestamp) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectTimestampEntity>(a));
            }

            if (!Vocabulary.IsNullOrEmpty())
            {
                foreach (var a in Vocabulary) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectVocabularyEntity>(a));
            }

            if (!VocabularySet.IsNullOrEmpty())
            {
                foreach (var a in VocabularySet) entities.Add(TrakHoundEntity.FromArray<ITrakHoundObjectVocabularySetEntity>(a));
            }

            return entities;
        }
    }
}
