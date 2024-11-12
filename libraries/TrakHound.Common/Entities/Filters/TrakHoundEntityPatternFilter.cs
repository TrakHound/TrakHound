// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities.Collections;

namespace TrakHound.Entities.Filters
{
    public partial class TrakHoundEntityPatternFilter : IDisposable
    {
        private readonly ITrakHoundSystemEntitiesClient _client;
        private readonly ItemIntervalQueue<ITrakHoundEntity> _itemQueue;
        private readonly List<string> _allow = new List<string>();
        private readonly List<string> _deny = new List<string>();
        private readonly ListDictionary<string, string> _allowCache = new ListDictionary<string, string>(); // Query => Uuid[]
        private readonly ListDictionary<string, string> _denyCache = new ListDictionary<string, string>(); // Query => Uuid[]
        private readonly ListDictionary<string, string> _allowReverseCache = new ListDictionary<string, string>(); // Uuid => Query[]
        private readonly ListDictionary<string, string> _denyReverseCache = new ListDictionary<string, string>(); // Uuid => Query[]
        private readonly object _lock = new object();


        public event EventHandler<IEnumerable<TrakHoundEntityMatchResult>> MatchesReceived;


        public TrakHoundEntityPatternFilter(ITrakHoundSystemEntitiesClient client, int queueInterval = 0, int limit = 100)
        {
            _client = client;
            _itemQueue = new ItemIntervalQueue<ITrakHoundEntity>(queueInterval, limit);
            _itemQueue.ItemsReceived += ItemQueueRead;
        }

        public void Dispose()
        {
            if (_itemQueue != null) _itemQueue.Dispose();
        }


        public void Allow(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                lock (_lock)
                {
                    if (!_allow.Contains(pattern)) _allow.Add(pattern);
                }
            }
        }

        public void Allow(IEnumerable<string> patterns)
        {
            if (!patterns.IsNullOrEmpty())
            {
                foreach (var query in patterns) Allow(query);
            }
        }

        public void Deny(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                lock (_lock)
                {
                    if (!_deny.Contains(pattern)) _deny.Add(pattern);
                }
            }
        }

        public void Deny(IEnumerable<string> patterns)
        {
            if (!patterns.IsNullOrEmpty())
            {
                foreach (var query in patterns) Deny(query);
            }
        }


        #region "Match"

        public async Task<bool> Match(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var x = await Match(new ITrakHoundEntity[] { entity });
                if (!x.IsNullOrEmpty()) return true;
            }

            return false;
        }

        public async Task<IEnumerable<TrakHoundEntityMatchResult>> Match(IEnumerable<ITrakHoundEntity> entities)
        {
            return await ProcessEntities(entities);
        }


        public void MatchAsync(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                _itemQueue.Add(entity);
            }
        }

        public void MatchAsync(IEnumerable<ITrakHoundEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        _itemQueue.Add(entity);
                    }
                }
            }
        }

        #endregion

        #region "Async"

        private async void ItemQueueRead(object sender, IEnumerable<ITrakHoundEntity> items)
        {
            await OnEntitiesReceived(items);
        }

        protected virtual async Task OnEntitiesReceived(IEnumerable<ITrakHoundEntity> entities)
        {
            var results = await ProcessEntities(entities);
            if (!results.IsNullOrEmpty())
            {
                if (MatchesReceived != null) MatchesReceived.Invoke(this, results);
            }
        }

        #endregion

        #region "Processing"

        private async Task<IEnumerable<TrakHoundEntityMatchResult>> ProcessEntities(IEnumerable<ITrakHoundEntity> entities)
        {
            var results = new List<TrakHoundEntityMatchResult>();

            if (!entities.IsNullOrEmpty())
            {
                var categoryEntities = entities.Where(o => o.Category == TrakHoundEntityCategoryId.Objects);
                if (!categoryEntities.IsNullOrEmpty())
                {
                    if (!_allow.IsNullOrEmpty())
                    {
                        // Create list of Results based on Allow patterns
                        var allowResults = await ProcessEntities(_allow, _allowCache, _allowReverseCache, categoryEntities);
                        if (!allowResults.IsNullOrEmpty())
                        {
                            results.AddRange(allowResults);

                            if (!_deny.IsNullOrEmpty())
                            {
                                // Remove from Results based on Deny patterns
                                var denyResults = await ProcessEntities(_deny, _denyCache, _denyReverseCache, categoryEntities);
                                if (!denyResults.IsNullOrEmpty())
                                {
                                    foreach (var denyResult in denyResults)
                                    {
                                        results.RemoveAll(o => o.Entity.Uuid == denyResult.Entity.Uuid);
                                    }
                                }
                            }
                        }
                    }
                    else if (!_deny.IsNullOrEmpty())
                    {
                        foreach (var entity in entities)
                        {
                            results.Add(new TrakHoundEntityMatchResult(null, entity));
                        }

                        // Remove from Results based on Deny patterns
                        var denyResults = await ProcessEntities(_deny, _denyCache, _denyReverseCache, categoryEntities);
                        if (!denyResults.IsNullOrEmpty())
                        {
                            foreach (var denyResult in denyResults)
                            {
                                results.RemoveAll(o => o.Entity.Uuid == denyResult.Entity.Uuid);
                            }
                        }
                    }
                }
            }

            return results;
        }


        private async Task<IEnumerable<TrakHoundEntityMatchResult>> ProcessEntities(
            IEnumerable<string> patterns,
            ListDictionary<string, string> patternCache,
            ListDictionary<string, string> patternReverseCache,
            IEnumerable<ITrakHoundEntity> categoryEntities
            )
        {
            var results = new List<TrakHoundEntityMatchResult>();

            if (!categoryEntities.IsNullOrEmpty())
            {
                // Get list of Objects
                var objectEntities = categoryEntities.Where(o => o.Class == TrakHoundObjectsEntityClassId.Object);
                if (!objectEntities.IsNullOrEmpty())
                {
                    var matchedEntities = await ProcessObjectEntities(patterns, patternCache, patternReverseCache, objectEntities);
                    if (!matchedEntities.IsNullOrEmpty())
                    {
                        foreach (var matchedEntity in matchedEntities)
                        {
                            results.Add(matchedEntity);
                        }
                    }
                }

                // Get list of Content Entities
                var contentEntities = categoryEntities.Where(o => o.Class != TrakHoundObjectsEntityClassId.Object && o.Class != TrakHoundObjectsEntityClassId.Metadata);
                if (!contentEntities.IsNullOrEmpty())
                {
                    var matchedEntities = await ProcessContentEntities(patterns, patternCache, patternReverseCache, contentEntities);
                    if (!matchedEntities.IsNullOrEmpty())
                    {
                        foreach (var matchedEntity in matchedEntities)
                        {
                            results.Add(matchedEntity);
                        }
                    }
                }
            }

            return results;
        }


        private async Task<IEnumerable<TrakHoundEntityMatchResult>> ProcessObjectEntities(
            IEnumerable<string> patterns, 
            ListDictionary<string, string> patternCache,
            ListDictionary<string, string> patternReverseCache,
            IEnumerable<ITrakHoundEntity> objectEntities
            )
        {
            var objects = objectEntities.ToDistinct();
            var objectUuids = objects.Select(o => o.Uuid);
            var dObjects = objects.ToDictionary(o => o.Uuid);

            var results = new List<TrakHoundEntityMatchResult>();
            var valid = new List<TrakHoundTargetResult>();
            var unknown = new List<string>();

            foreach (var objectUuid in objectUuids)
            {
                var objectQueries = patternReverseCache.Get(objectUuid);
                if (!objectQueries.IsNullOrEmpty())
                {
                    foreach (var objectQuery in objectQueries)
                    {
                        valid.Add(new TrakHoundTargetResult(objectQuery, objectUuid));
                    }
                }
                else
                {
                    unknown.Add(objectUuid);
                }
            }

            if (unknown.Count > 0)
            {
                // Find matches based on Filters
                var matches = await _client.Objects.QueryMatch(patterns, unknown);
                if (!matches.IsNullOrEmpty())
                {
                    foreach (var match in matches)
                    {
                        patternCache.Add(match.Target, match.Uuid);
                        patternReverseCache.Add(match.Uuid, match.Target);
                        valid.Add(new TrakHoundTargetResult(match.Target, match.Uuid));
                    }
                }
            }

            if (!valid.IsNullOrEmpty())
            {
                foreach (var result in valid)
                {
                    var entity = dObjects.GetValueOrDefault(result.Uuid);
                    if (entity != null)
                    {
                        results.Add(new TrakHoundEntityMatchResult(result.Target, entity));
                    }
                }
            }

            return results;
        }

        private async Task<IEnumerable<TrakHoundEntityMatchResult>> ProcessContentEntities(
            IEnumerable<string> patterns,
            ListDictionary<string, string> patternCache,
            ListDictionary<string, string> patternReverseCache,
            IEnumerable<ITrakHoundEntity> contentEntities
            )
        {
            var assemblyUuids = contentEntities.Select(o => GetContentObjectUuid(o)).Distinct();
            var dContentEntities = new ListDictionary<string, ITrakHoundEntity>();
            foreach (var componentEntity in contentEntities) dContentEntities.Add(GetContentObjectUuid(componentEntity), componentEntity);

            var results = new List<TrakHoundEntityMatchResult>();
            var valid = new List<TrakHoundTargetResult>();
            var unknown = new List<string>();

            foreach (var assemblyUuid in assemblyUuids)
            {
                var objectQueries = patternReverseCache.Get(assemblyUuid);
                if (!objectQueries.IsNullOrEmpty())
                {
                    foreach (var objectQuery in objectQueries)
                    {
                        valid.Add(new TrakHoundTargetResult(objectQuery, assemblyUuid));
                    }
                }
                else
                {
                    unknown.Add(assemblyUuid);
                }
            }

            if (unknown.Count > 0)
            {
                var targetObjects = await _client.Objects.ReadByUuid(unknown);
                if (!targetObjects.IsNullOrEmpty())
                {
                    var objects = new List<ITrakHoundObjectEntity>();
                    objects.AddRange(targetObjects);

                    // Read Root Objects (to use for Expression Evaluation)
                    var rootObjects = await _client.Objects.QueryRootByChildUuid(unknown);
                    if (!rootObjects.IsNullOrEmpty()) objects.AddRange(rootObjects);

                    var collection = new TrakHoundEntityCollection();
                    collection.Add(objects);

                    var definitionUuids = objects.Select(o => o.DefinitionUuid).Distinct();
                    if (!definitionUuids.IsNullOrEmpty())
                    {
                        collection.Add(await _client.Definitions.ReadByUuid(definitionUuids));
                        collection.Add(await _client.Definitions.QueryRootByChildUuid(definitionUuids));
                    }

                    foreach (var pattern in patterns)
                    {
                        var matchedObjects = TrakHoundExpression.Match(pattern, collection);
                        if (!matchedObjects.IsNullOrEmpty())
                        {
                            foreach (var matchedObject in matchedObjects)
                            {
                                patternCache.Add(pattern, matchedObject.Uuid);
                                patternReverseCache.Add(matchedObject.Uuid, pattern);
                                valid.Add(new TrakHoundTargetResult(pattern, matchedObject.Uuid));
                            }
                        }
                    }
                }
            }

            if (!valid.IsNullOrEmpty())
            {
                foreach (var result in valid)
                {
                    var validEntities = dContentEntities.Get(result.Uuid);
                    if (!validEntities.IsNullOrEmpty())
                    {
                        foreach (var entity in validEntities)
                        {
                            results.Add(new TrakHoundEntityMatchResult(result.Target, entity));
                        }
                    }
                }
            }

            return results;
        }

        #endregion

    }
}
