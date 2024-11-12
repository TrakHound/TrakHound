// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines
{
    public class EntityConditionQuery
    {
        public TrakHoundObjectContentType ContentType { get; set; }

        public ITrakHoundObjectEntity TargetObject { get; set; }

        public ITrakHoundObjectEntity ConditionObject { get; set; }

        public QueryConditionGroup ConditionGroup { get; set; }

        public QueryCondition Condition { get; set; }


        //public EntityConditionQuery(TrakHoundObjectContentType contentType, ITrakHoundObjectEntityModel targetObject, ITrakHoundObjectEntityModel conditionObject, QueryCondition query)
        //{
        //    ContentType = contentType;
        //    TargetObject = targetObject;
        //    ConditionObject = conditionObject;
        //    Query = query;
        //}


        public void Evaluate(ITrakHoundClient client)
        {
            if (client != null)
            {

            }
        }
    }
}
