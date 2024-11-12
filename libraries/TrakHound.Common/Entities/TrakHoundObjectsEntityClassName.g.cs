// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Objects Entity Class Names in the TrakHound Framework
    /// </summary>
    public static class TrakHoundObjectsEntityClassName
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


        public static string Get(byte entityClassId)
        {
            switch (entityClassId)
            {

                case TrakHoundObjectsEntityClassId.Object: return Object;
                case TrakHoundObjectsEntityClassId.Metadata: return Metadata;
                case TrakHoundObjectsEntityClassId.Assignment: return Assignment;
                case TrakHoundObjectsEntityClassId.Blob: return Blob;
                case TrakHoundObjectsEntityClassId.Boolean: return Boolean;
                case TrakHoundObjectsEntityClassId.Duration: return Duration;
                case TrakHoundObjectsEntityClassId.Event: return Event;
                case TrakHoundObjectsEntityClassId.Group: return Group;
                case TrakHoundObjectsEntityClassId.Hash: return Hash;
                case TrakHoundObjectsEntityClassId.Log: return Log;
                case TrakHoundObjectsEntityClassId.Message: return Message;
                case TrakHoundObjectsEntityClassId.MessageQueue: return MessageQueue;
                case TrakHoundObjectsEntityClassId.Number: return Number;
                case TrakHoundObjectsEntityClassId.Observation: return Observation;
                case TrakHoundObjectsEntityClassId.Queue: return Queue;
                case TrakHoundObjectsEntityClassId.Reference: return Reference;
                case TrakHoundObjectsEntityClassId.Set: return Set;
                case TrakHoundObjectsEntityClassId.State: return State;
                case TrakHoundObjectsEntityClassId.Statistic: return Statistic;
                case TrakHoundObjectsEntityClassId.String: return String;
                case TrakHoundObjectsEntityClassId.TimeRange: return TimeRange;
                case TrakHoundObjectsEntityClassId.Timestamp: return Timestamp;
                case TrakHoundObjectsEntityClassId.Vocabulary: return Vocabulary;
                case TrakHoundObjectsEntityClassId.VocabularySet: return VocabularySet;
            }

            return null;
        }

        public static string Get(TrakHoundObjectsEntityClass entityClass)
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

            return null;
        }
    }
}
