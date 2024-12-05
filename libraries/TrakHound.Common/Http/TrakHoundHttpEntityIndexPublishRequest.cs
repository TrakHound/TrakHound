// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpEntityIndexPublishRequest
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("dataType")]
        public int DataType { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("sourceUuid")]
        public string SourceUuid { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }


        public TrakHoundHttpEntityIndexPublishRequest() { }

        public TrakHoundHttpEntityIndexPublishRequest(EntityIndexPublishRequest request)
        {
            Uuid = request.Uuid;
            Target = request.Target;
            DataType = request.DataType;
            Value = request.Value;
            Subject = request.Subject;
            SourceUuid = request.SourceUuid;
            Created = request.Created;
        }


        public EntityIndexPublishRequest ToRequest()
        {
            return new EntityIndexPublishRequest(Target, (EntityIndexDataType)DataType, Value, Subject, SourceUuid, Created);
        }


        public static IEnumerable<TrakHoundHttpEntityIndexPublishRequest> Create(EntityIndexPublishRequest request)
        {
            if (!string.IsNullOrEmpty(request.Target) && request.Created > 0)
            {
                var results = new List<TrakHoundHttpEntityIndexPublishRequest>();
                results.Add(new TrakHoundHttpEntityIndexPublishRequest(request));
                return results;
            }

            return null;
        }

        public static IEnumerable<TrakHoundHttpEntityIndexPublishRequest> Create(IEnumerable<EntityIndexPublishRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var x = new List<TrakHoundHttpEntityIndexPublishRequest>();
                foreach (var request in requests)
                {
                    x.Add(new TrakHoundHttpEntityIndexPublishRequest(request));
                }
                return x;
            }

            return null;
        }
    }

    public static class TrakHoundHttpEntityIndexPublishRequestExtensions
    {
        public static IEnumerable<EntityIndexPublishRequest> ToRequests(this IEnumerable<TrakHoundHttpEntityIndexPublishRequest> requests)
        {
            var queries = new List<EntityIndexPublishRequest>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    queries.Add(request.ToRequest());
                }
            }

            return queries;
        }
    }
}
