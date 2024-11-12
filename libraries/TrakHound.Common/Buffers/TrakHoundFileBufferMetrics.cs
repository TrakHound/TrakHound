// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Buffers
{
    public class TrakHoundFileBufferMetrics
    {
        [JsonPropertyName("isEnabled")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool IsEnabled { get; set; }

        [JsonPropertyName("isReadActive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool IsReadActive { get; set; }

        [JsonPropertyName("isWriteActive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool IsWriteActive { get; set; }

        [JsonPropertyName("pageCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int PageCount { get; set; }

        [JsonPropertyName("readPageSequence")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ulong ReadPageSequence { get; set; }

        [JsonPropertyName("writePageSequence")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ulong WritePageSequence { get; set; }

        [JsonPropertyName("nextPageSequence")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ulong NextPageSequence { get; set; }

        [JsonPropertyName("lastPageSequence")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ulong LastPageSequence { get; set; }

        [JsonPropertyName("pageSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long PageSize { get; set; }

        [JsonPropertyName("remainingSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long RemainingSize { get; set; }

        [JsonPropertyName("totalReadCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long TotalReadCount { get; set; }

        [JsonPropertyName("totalWriteCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long TotalWriteCount { get; set; }

        [JsonPropertyName("totalBytesRead")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long TotalBytesRead { get; set; }

        [JsonPropertyName("totalBytesWritten")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long TotalBytesWritten { get; set; }

        [JsonPropertyName("itemReadRate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public double ItemReadRate { get; set; }

        [JsonPropertyName("byteReadRate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public double ByteReadRate { get; set; }

        [JsonPropertyName("itemWriteRate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public double ItemWriteRate { get; set; }

        [JsonPropertyName("byteWriteRate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public double ByteWriteRate { get; set; }

        [JsonPropertyName("lastUpdated")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long LastUpdated { get; set; }
    }
}
