// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpDeleteRequest
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }


        public TrakHoundHttpDeleteRequest() { }

        public TrakHoundHttpDeleteRequest(EntityDeleteRequest request)
        {
            Target = request.Target;
            Timestamp = request.Timestamp;
        }


        public EntityDeleteRequest ToRequest()
        {
            var request = new EntityDeleteRequest();
            request.Target = Target;
            request.Timestamp = Timestamp;
            return request;
        }


        public static IEnumerable<TrakHoundHttpDeleteRequest> Create(EntityDeleteRequest request)
        {
            if (!string.IsNullOrEmpty(request.Target) && request.Timestamp > 0)
            {
                var results = new List<TrakHoundHttpDeleteRequest>();
                results.Add(new TrakHoundHttpDeleteRequest(request));
                return results;
            }

            return null;
        }

        public static IEnumerable<TrakHoundHttpDeleteRequest> Create(IEnumerable<EntityDeleteRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var x = new List<TrakHoundHttpDeleteRequest>();
                foreach (var request in requests)
                {
                    x.Add(new TrakHoundHttpDeleteRequest(request));
                }
                return x;
            }

            return null;
        }
    }

    public static class TrakHoundHttpDeleteRequestExtensions
    {
        public static IEnumerable<EntityDeleteRequest> ToRequests(this IEnumerable<TrakHoundHttpDeleteRequest> requests)
        {
            var queries = new List<EntityDeleteRequest>();

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
