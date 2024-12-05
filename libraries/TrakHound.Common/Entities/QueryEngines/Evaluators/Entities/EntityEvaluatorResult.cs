// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.QueryEngines.Evaluators.Entities
{
    public class EntityEvaluatorResult : IEvaluatorResult
    {
        public string Id { get; set; }

        public string ConditionId { get; set; }

        public string GroupId { get; set; }

        public bool Success { get; set; }

        public ConditionObjectQuery Query { get; set; }

        public EntityEvaluator Evaluator { get; set; }

        public ITrakHoundEntity Entity { get; set; }
    }
}
