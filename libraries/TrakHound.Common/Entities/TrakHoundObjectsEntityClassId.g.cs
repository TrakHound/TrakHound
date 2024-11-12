// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Objects Entity Class IDs in the TrakHound Framework
    /// </summary>
    public static class TrakHoundObjectsEntityClassId
    {

        public const byte Object = 1;
        public const byte Metadata = 2;
        public const byte Assignment = 3;
        public const byte Blob = 4;
        public const byte Boolean = 5;
        public const byte Duration = 6;
        public const byte Event = 7;
        public const byte Group = 8;
        public const byte Hash = 9;
        public const byte Log = 10;
        public const byte Message = 11;
        public const byte MessageQueue = 12;
        public const byte Number = 13;
        public const byte Observation = 14;
        public const byte Queue = 15;
        public const byte Reference = 16;
        public const byte Set = 17;
        public const byte State = 18;
        public const byte Statistic = 19;
        public const byte String = 20;
        public const byte TimeRange = 21;
        public const byte Timestamp = 22;
        public const byte Vocabulary = 23;
        public const byte VocabularySet = 24;


        public static byte Get(string entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundObjectsEntityClassName.Object: return Object;
                case TrakHoundObjectsEntityClassName.Metadata: return Metadata;
                case TrakHoundObjectsEntityClassName.Assignment: return Assignment;
                case TrakHoundObjectsEntityClassName.Blob: return Blob;
                case TrakHoundObjectsEntityClassName.Boolean: return Boolean;
                case TrakHoundObjectsEntityClassName.Duration: return Duration;
                case TrakHoundObjectsEntityClassName.Event: return Event;
                case TrakHoundObjectsEntityClassName.Group: return Group;
                case TrakHoundObjectsEntityClassName.Hash: return Hash;
                case TrakHoundObjectsEntityClassName.Log: return Log;
                case TrakHoundObjectsEntityClassName.Message: return Message;
                case TrakHoundObjectsEntityClassName.MessageQueue: return MessageQueue;
                case TrakHoundObjectsEntityClassName.Number: return Number;
                case TrakHoundObjectsEntityClassName.Observation: return Observation;
                case TrakHoundObjectsEntityClassName.Queue: return Queue;
                case TrakHoundObjectsEntityClassName.Reference: return Reference;
                case TrakHoundObjectsEntityClassName.Set: return Set;
                case TrakHoundObjectsEntityClassName.State: return State;
                case TrakHoundObjectsEntityClassName.Statistic: return Statistic;
                case TrakHoundObjectsEntityClassName.String: return String;
                case TrakHoundObjectsEntityClassName.TimeRange: return TimeRange;
                case TrakHoundObjectsEntityClassName.Timestamp: return Timestamp;
                case TrakHoundObjectsEntityClassName.Vocabulary: return Vocabulary;
                case TrakHoundObjectsEntityClassName.VocabularySet: return VocabularySet;
            }

            return 0;
        }

        public static byte Get(TrakHoundObjectsEntityClass entityClass)
        {
            switch (entityClass)
            {

                case TrakHoundObjectsEntityClass.Object: return Object;
                case TrakHoundObjectsEntityClass.Metadata: return Metadata;
                case TrakHoundObjectsEntityClass.Assignment: return Assignment;
                case TrakHoundObjectsEntityClass.Blob: return Blob;
                case TrakHoundObjectsEntityClass.Boolean: return Boolean;
                case TrakHoundObjectsEntityClass.Duration: return Duration;
                case TrakHoundObjectsEntityClass.Event: return Event;
                case TrakHoundObjectsEntityClass.Group: return Group;
                case TrakHoundObjectsEntityClass.Hash: return Hash;
                case TrakHoundObjectsEntityClass.Log: return Log;
                case TrakHoundObjectsEntityClass.Message: return Message;
                case TrakHoundObjectsEntityClass.MessageQueue: return MessageQueue;
                case TrakHoundObjectsEntityClass.Number: return Number;
                case TrakHoundObjectsEntityClass.Observation: return Observation;
                case TrakHoundObjectsEntityClass.Queue: return Queue;
                case TrakHoundObjectsEntityClass.Reference: return Reference;
                case TrakHoundObjectsEntityClass.Set: return Set;
                case TrakHoundObjectsEntityClass.State: return State;
                case TrakHoundObjectsEntityClass.Statistic: return Statistic;
                case TrakHoundObjectsEntityClass.String: return String;
                case TrakHoundObjectsEntityClass.TimeRange: return TimeRange;
                case TrakHoundObjectsEntityClass.Timestamp: return Timestamp;
                case TrakHoundObjectsEntityClass.Vocabulary: return Vocabulary;
                case TrakHoundObjectsEntityClass.VocabularySet: return VocabularySet;
            }

            return 0;
        }
    }
}
