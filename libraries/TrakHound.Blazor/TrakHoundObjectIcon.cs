// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;

namespace TrakHound.Blazor
{
    public static class TrakHoundObjectIcon
    {
        public static string Get(TrakHoundObjectContentType contentType)
        {
            switch (contentType)
            {
                case TrakHoundObjectContentType.Directory: return "fa-sharp fa-solid fa-folder";

                case TrakHoundObjectContentType.Assignment: return "fa-regular fa-arrow-down-to-square";
                case TrakHoundObjectContentType.Blob: return "fa-regular fa-file-binary";
                case TrakHoundObjectContentType.Boolean: return "fa-regular fa-toggle-on";
                case TrakHoundObjectContentType.Duration: return "fa-regular fa-hourglass";
                case TrakHoundObjectContentType.Event: return "fa-sharp fa-solid fa-bolt";
                //case TrakHoundObjectContentType.Feed: return "fa-sharp fa-solid fa-rss";
                case TrakHoundObjectContentType.Group: return "fa-sharp fa-solid fa-layer-group";
                case TrakHoundObjectContentType.Hash: return "fa-sharp fa-regular fa-list";
                case TrakHoundObjectContentType.Log: return "fa-sharp fa-regular fa-triangle-exclamation";
                case TrakHoundObjectContentType.Message: return "fa-sharp fa-regular fa-envelope";
                case TrakHoundObjectContentType.MessageQueue: return "fa-sharp fa-solid fa-inbox";
                case TrakHoundObjectContentType.Number: return "fa-regular fa-hashtag";
                case TrakHoundObjectContentType.Observation: return "fa-regular fa-signal-stream";
                case TrakHoundObjectContentType.Queue: return "fa-sharp fa-regular fa-arrow-down-1-9";
                case TrakHoundObjectContentType.Reference: return "fa-sharp fa-regular fa-bullseye-pointer";
                case TrakHoundObjectContentType.Set: return "fa-sharp fa-regular fa-bars";
                case TrakHoundObjectContentType.State: return "fa-sharp fa-solid fa-signal-bars";
                case TrakHoundObjectContentType.Statistic: return "fa-sharp fa-solid fa-chart-column";
                case TrakHoundObjectContentType.String: return "fa-regular fa-pen-line";
                case TrakHoundObjectContentType.TimeRange: return "fa-sharp fa-regular fa-calendar-day";
                case TrakHoundObjectContentType.Timestamp: return "fa-sharp fa-regular fa-clock";
                case TrakHoundObjectContentType.Vocabulary: return "fa-sharp fa-regular fa-book";
                case TrakHoundObjectContentType.VocabularySet: return "fa-sharp fa-regular fa-books";
            }

            return null;
        }
    }
}
