// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Entities.Filters
{
    public class TrakHoundEntityFilter : IDisposable
    {
        private readonly Dictionary<string, TrakHoundEntityFilterRule> _rules = new Dictionary<string, TrakHoundEntityFilterRule>();


        public void Dispose()
        {
            foreach (var rule in _rules.Values) rule.Dispose();
        }


        public void AddRule(TrakHoundEntityFilterRule rule)
        {
            if (rule != null && rule.Id != null)
            {
                _rules.Remove(rule.Id);
                _rules.Add(rule.Id, rule);
            }
        }


        public async Task<bool> IsMatch(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var results = await Match(new ITrakHoundEntity[] { entity });
                return !results.IsNullOrEmpty();
            }

            return false;
        }

        public async Task<IEnumerable<ITrakHoundEntity>> Match(IEnumerable<ITrakHoundEntity> entities)
        {
            if (!entities.IsNullOrEmpty() && !_rules.IsNullOrEmpty())
            {
                IEnumerable<ITrakHoundEntity> results = entities;

                foreach (var rule in _rules)
                {
                    results = await rule.Value.Process(results);
                    if (results.IsNullOrEmpty()) break;
                }

                return results;
            }

            return entities;
        }
    }
}
