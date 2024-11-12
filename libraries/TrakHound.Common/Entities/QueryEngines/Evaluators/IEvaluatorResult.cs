// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities.QueryEngines.Evaluators
{
    public interface IEvaluatorResult
    {
        public string Id { get; set; }

        public string ConditionId { get; set; }

        public string GroupId { get; set; }

        public bool Success { get; set; }
    }
}
