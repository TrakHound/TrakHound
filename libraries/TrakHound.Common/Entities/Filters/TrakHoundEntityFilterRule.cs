// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;

namespace TrakHound.Entities.Filters
{
    public class TrakHoundEntityFilterRule : IDisposable
    {
        private readonly string _id;
        private readonly ITrakHoundSystemEntitiesClient _client;
        private TrakHoundEntityPatternFilter _patternFilter;
        private TrakHoundEntityOnChangeFilter _changeFilter;
        private TrakHoundEntityDeltaFilter _deltaFilter;
        private TrakHoundEntityPeriodFilter _periodFilter;


        public string Id => _id;


        public TrakHoundEntityFilterRule(ITrakHoundSystemEntitiesClient client, string id = null)
        {
            _client = client;
            _id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            if (_patternFilter != null) _patternFilter.Dispose();
        }


        public void SetAllowPattern(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                if (_patternFilter == null) _patternFilter = new TrakHoundEntityPatternFilter(_client);
                _patternFilter.Allow(pattern);
            }
        }

        public void SetAllowPattern(IEnumerable<string> patterns)
        {
            if (!patterns.IsNullOrEmpty())
            {
                if (_patternFilter == null) _patternFilter = new TrakHoundEntityPatternFilter(_client);
                _patternFilter.Allow(patterns);
            }
        }

        public void SetDenyPattern(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                if (_patternFilter == null) _patternFilter = new TrakHoundEntityPatternFilter(_client);
                _patternFilter.Deny(pattern);
            }
        }

        public void SetDenyPattern(IEnumerable<string> patterns)
        {
            if (!patterns.IsNullOrEmpty())
            {
                if (_patternFilter == null) _patternFilter = new TrakHoundEntityPatternFilter(_client);
                _patternFilter.Deny(patterns);
            }
        }

        public void SetOnChange(bool filterOnChange)
        {
            if (filterOnChange) _changeFilter = new TrakHoundEntityOnChangeFilter();
            //if (filterOnChange) _changeFilter = new TrakHoundEntityOnChangeFilter($"{_id}.on-change");
            else _changeFilter = null;
        }

        public void SetMinimumPeriod(TimeSpan minimumPeriod)
        {
            if (_periodFilter == null) _periodFilter = new TrakHoundEntityPeriodFilter($"{_id}.period", minimumPeriod);
            else _periodFilter = null;
        }

        public void SetMinimumDelta(double minimumDelta)
        {
            if (_deltaFilter == null) _deltaFilter = new TrakHoundEntityDeltaFilter($"{_id}.delta", minimumDelta);
            else _deltaFilter = null;
        }


        public async Task<IEnumerable<ITrakHoundEntity>> Process(IEnumerable<ITrakHoundEntity> entities)
        {
            var results = new Dictionary<string, ITrakHoundEntity>();

            if (!entities.IsNullOrEmpty())
            {
                if (_patternFilter != null)
                {
                    // Filter by Pattern
                    var matchResults = await _patternFilter.Match(entities);
                    if (!matchResults.IsNullOrEmpty())
                    {
                        foreach (var matchResult in matchResults)
                        {
                            if (matchResult.Entity.Uuid != null) results.Add(matchResult.Entity.Uuid, matchResult.Entity);
                        }
                    }
                }
                else
                {
                    foreach (var entity in entities)
                    {
                        if (entity.Uuid != null) results.Add(entity.Uuid, entity);
                    }
                }

                // Filter by OnChange
                if (_changeFilter != null)
                {
                    foreach (var uuid in results.Keys.ToList())
                    {
                        var entity = results.GetValueOrDefault(uuid);
                        if (!_changeFilter.Filter(entity))
                        {
                            // Remove from Results if not changed
                            results.Remove(uuid);
                        }
                    }
                }

                // Filter by Delta
                if (_deltaFilter != null)
                {
                    foreach (var uuid in results.Keys.ToList())
                    {
                        var entity = results.GetValueOrDefault(uuid);
                        if (!_deltaFilter.Filter(entity))
                        {
                            // Remove from Results if not exceeds Minimum Delta
                            results.Remove(uuid);
                        }
                    }
                }

                // Filter by Period
                if (_periodFilter != null)
                {
                    foreach (var uuid in results.Keys.ToList())
                    {
                        var entity = results.GetValueOrDefault(uuid);
                        if (!_periodFilter.Filter(entity))
                        {
                            // Remove from Results if not exceeds Minimum Period
                            results.Remove(uuid);
                        }
                    }
                }
            }

            return results.Values;
        }
    }
}
