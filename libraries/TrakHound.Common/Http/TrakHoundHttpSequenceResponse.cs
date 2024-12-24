// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Sequences;

namespace TrakHound.Http
{
    public class TrakHoundHttpSequenceResponse
    {
        [JsonPropertyName("sequenceId")]
        public string SequenceId { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }


        public TrakHoundHttpSequenceResponse() { }

        public TrakHoundHttpSequenceResponse(TrakHoundSequenceResponse response)
        {
            SequenceId = response.SequenceId;
            Value = response.Value;
            Created = response.Created;
        }


        public TrakHoundSequenceResponse ToSequence() => new TrakHoundSequenceResponse(SequenceId, Value, Created);


        public static IEnumerable<TrakHoundHttpSequenceResponse> Create(IEnumerable<TrakHoundSequenceResponse> sequences)
        {
            var requests = new List<TrakHoundHttpSequenceResponse>();
            if (!sequences.IsNullOrEmpty())
            {
                foreach (var sequence in sequences)
                {
                    requests.Add(new TrakHoundHttpSequenceResponse(sequence));
                }
            }

            return requests;
        }
    }

    public static class TrakHoundHttpSequenceResponseExtensions
    {
        public static IEnumerable<TrakHoundSequenceResponse> ToResponses(this IEnumerable<TrakHoundHttpSequenceResponse> requests)
        {
            var queries = new List<TrakHoundSequenceResponse>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToSequence());
                }
            }

            return queries;
        }
    }
}
