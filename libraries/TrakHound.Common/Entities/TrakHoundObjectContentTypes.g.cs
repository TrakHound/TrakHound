// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public static class TrakHoundObjectContentTypes
    {

        public const string Directory = "Directory";
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

        public static string GetString(TrakHoundObjectContentType contentType)
        {
            switch (contentType)
            {

                case TrakHoundObjectContentType.Directory: return Directory;
                case TrakHoundObjectContentType.Assignment: return Assignment;
                case TrakHoundObjectContentType.Blob: return Blob;
                case TrakHoundObjectContentType.Boolean: return Boolean;
                case TrakHoundObjectContentType.Duration: return Duration;
                case TrakHoundObjectContentType.Event: return Event;
                case TrakHoundObjectContentType.Group: return Group;
                case TrakHoundObjectContentType.Hash: return Hash;
                case TrakHoundObjectContentType.Log: return Log;
                case TrakHoundObjectContentType.Message: return Message;
                case TrakHoundObjectContentType.MessageQueue: return MessageQueue;
                case TrakHoundObjectContentType.Number: return Number;
                case TrakHoundObjectContentType.Observation: return Observation;
                case TrakHoundObjectContentType.Queue: return Queue;
                case TrakHoundObjectContentType.Reference: return Reference;
                case TrakHoundObjectContentType.Set: return Set;
                case TrakHoundObjectContentType.State: return State;
                case TrakHoundObjectContentType.Statistic: return Statistic;
                case TrakHoundObjectContentType.String: return String;
                case TrakHoundObjectContentType.TimeRange: return TimeRange;
                case TrakHoundObjectContentType.Timestamp: return Timestamp;
                case TrakHoundObjectContentType.Vocabulary: return Vocabulary;
                case TrakHoundObjectContentType.VocabularySet: return VocabularySet;
            }

            return null;
        }
    }
}
