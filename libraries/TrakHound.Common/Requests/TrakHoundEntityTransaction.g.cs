// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public class TrakHoundEntityTransaction
    {
        [JsonPropertyName("publish")]
        public TrakHoundEntityPublishTransaction PublishOperations { get; set; }

        [JsonPropertyName("delete")]
        public TrakHoundEntityDeleteTransaction DeleteOperations { get; set; }

        [JsonPropertyName("source")]
        public TrakHoundSourceEntry Source { get; set; }


        public void Add(TrakHoundEntityTransaction transaction)
        {
            if (transaction != null)
            {
                Add(transaction.PublishOperations);
                Add(transaction.DeleteOperations);
            }
        }

        public void Add(TrakHoundEntityPublishTransaction transaction)
        {
            if (transaction != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(transaction);
            }
        }

        public void Add(TrakHoundEntityDeleteTransaction transaction)
        {
            if (transaction != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(transaction);
            }
        }



        public void Add(TrakHoundObjectEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundDefinitionEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundSourceEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundAssignmentEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundBlobEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundBooleanEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundDurationEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundEventEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundGroupEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundHashEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundLogEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundMessageMappingEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundMessageQueueMappingEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundNumberEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundObservationEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundQueueEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundReferenceEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundSetEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundStateEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundStatisticEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundStringEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundTimeRangeEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundTimestampEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundVocabularyEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }

        public void Add(TrakHoundVocabularySetEntry entry)
        {
            if (entry != null)
            {
                if (PublishOperations == null) PublishOperations = new TrakHoundEntityPublishTransaction();
                PublishOperations.Add(entry);
            }
        }



        public void Add(TrakHoundObjectDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundDefinitionDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundSourceDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundAssignmentDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundBlobDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundBooleanDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundDurationDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundEventDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundGroupDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundHashDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundLogDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundNumberDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundObservationDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundQueueDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundReferenceDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundSetDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundStateDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundStatisticDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundStringDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundTimeRangeDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundTimestampDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundVocabularyDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }

        public void Add(TrakHoundVocabularySetDeleteOperation operation)
        {
            if (operation != null)
            {
                if (DeleteOperations == null) DeleteOperations = new TrakHoundEntityDeleteTransaction();
                DeleteOperations.Add(operation);
            }
        }


    }
}
