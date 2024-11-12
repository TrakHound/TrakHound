// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrakHound.Entities;

namespace TrakHound.Http
{
    public class TrakHoundHttpEntitySubscriptionRequest
    {
        [JsonPropertyName("entityCategory")]
        public string EntityCategory { get; set; }

        [JsonPropertyName("entityClass")]
        public string EntityClass { get; set; }

        [JsonPropertyName("expression")]
        public string Expression { get; set; }


        public TrakHoundHttpEntitySubscriptionRequest() { }

        public TrakHoundHttpEntitySubscriptionRequest(TrakHoundEntitySubscriptionRequest request)
        {
            EntityCategory = request.EntityCategory;
            EntityClass = request.EntityClass;
            Expression = request.Expression;
        }

        public TrakHoundEntitySubscriptionRequest ToSubscriptionRequest()
        {
            var request = new TrakHoundEntitySubscriptionRequest();
            request.EntityCategory = EntityCategory;
            request.EntityClass = EntityClass;
            request.Expression = Expression;
            return request;
        }


        public static IEnumerable<TrakHoundHttpEntitySubscriptionRequest> Create(IEnumerable<TrakHoundEntitySubscriptionRequest> requests)
        {
            var httpRequests = new List<TrakHoundHttpEntitySubscriptionRequest>();

            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    httpRequests.Add(new TrakHoundHttpEntitySubscriptionRequest(request));
                }
            }

            return httpRequests;
        }

        public static IEnumerable<TrakHoundEntitySubscriptionRequest> ToSubscriptionRequests(IEnumerable<TrakHoundHttpEntitySubscriptionRequest> httpRequests)
        {
            var requests = new List<TrakHoundEntitySubscriptionRequest>();

            if (!httpRequests.IsNullOrEmpty())
            {
                foreach (var httpRequest in httpRequests)
                {
                    requests.Add(httpRequest.ToSubscriptionRequest());
                }
            }

            return requests;
        }
    }
}
