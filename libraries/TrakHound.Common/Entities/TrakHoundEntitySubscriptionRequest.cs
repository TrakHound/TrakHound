// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public struct TrakHoundEntitySubscriptionRequest
    {
        public string EntityCategory { get; set; }

        public string EntityClass { get; set; }

        public string Expression { get; set; }


        public TrakHoundEntitySubscriptionRequest(string entityCategory, string entityClass, string expression)
        {
            EntityCategory = entityCategory;
            EntityClass = entityClass;
            Expression = expression;
        }
    }
}
