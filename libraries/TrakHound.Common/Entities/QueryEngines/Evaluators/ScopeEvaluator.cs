// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;

namespace TrakHound.Entities.QueryEngines.Evaluators
{
    public class ScopeEvaluator : IEvaluator
    {
        private readonly QueryScope _scope;
        private readonly ITrakHoundSystemEntitiesClient _client;


        public ScopeEvaluator(QueryScope scope, ITrakHoundSystemEntitiesClient client)
        {
            _scope = scope;
            _client = client;
        }


        public bool Evaluate()
        {
            return false;
        }
    }
}
