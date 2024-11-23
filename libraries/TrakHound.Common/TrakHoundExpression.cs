// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Entities.Collections;

namespace TrakHound
{
    public static class TrakHoundExpression
    {
        private const string Wildcard = "*";
        private const string WildcardLevel = "**";
        private const string PreviousLevel = "..";
        private const string PathSeparator = "/";
        private const string Delimiter = ";";

        private const string AndPattern = "&?([^&]+)";
        private const string OrPattern = @"\|?([^\|]+)";

        private const string _uuidPrefix = "uuid=";
        private const string _contentTypePrefix = "content-type=";
        private const string _typePrefix = "type=";
        private const string _metaPrefix = "meta@";

        private const string NameLikePrefixPattern = @"\~(.*)";
        private const string UuidPrefixPattern = $"{_uuidPrefix}(.*)";
        private const string ContentTypePrefixPattern = $"{_contentTypePrefix}([^>]*)>?(.*)";
        private const string TypePrefixPattern = $"{_typePrefix}([^>]*)>?(.*)";
        private const string MetadataPrefixPattern = @$"{_metaPrefix}(.*)\=(.*)";
        private const string MetadataLikePrefixPattern = @$"{_metaPrefix}(.*)\~(.*)";

        private static readonly Regex _andRegex = new Regex(AndPattern);
        private static readonly Regex _orRegex = new Regex(OrPattern);

        private static readonly Regex _nameLikeRegex = new Regex(NameLikePrefixPattern);
        private static readonly Regex _uuidRegex = new Regex(UuidPrefixPattern);
        private static readonly Regex _contentTypeRegex = new Regex(ContentTypePrefixPattern);
        private static readonly Regex _typeRegex = new Regex(TypePrefixPattern);
        private static readonly Regex _metadataRegex = new Regex(MetadataPrefixPattern);
        private static readonly Regex _metadataLikeRegex = new Regex(MetadataLikePrefixPattern);

        private struct NormalizeResult
        {
            public string[] BaseUuids { get; set; }
            public string[] Segments { get; set; }
        }


        public static async Task<IEnumerable<TrakHoundExpressionResult>> Evaluate(ITrakHoundSystemEntitiesClient client, string expression, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (client != null && !string.IsNullOrEmpty(expression))
            {
                var subExpressions = expression.Split(Delimiter);
                if (!subExpressions.IsNullOrEmpty())
                {
                    var results = new List<TrakHoundExpressionResult>();

                    foreach (var subExpression in subExpressions)
                    {
                        var uuids = await EvaluateSingle(client, subExpression, null, 0, 0, sortOrder);
                        if (!uuids.IsNullOrEmpty())
                        {
                            var ns = TrakHoundPath.GetNamespace(subExpression);

                            foreach (var uuid in uuids)
                            {
                                results.Add(new TrakHoundExpressionResult(ns, subExpression, uuid));
                            }
                        }
                    }

                    if (skip > 0 || take > 0)
                    {
                        return results.Skip(skip > int.MaxValue ? int.MaxValue : (int)skip).Take(take > int.MaxValue ? int.MaxValue : (int)take);
                    }
                    else
                    {
                        return results;
                    }
                }
            }

            return null;
        }

        public static async Task<IEnumerable<TrakHoundExpressionResult>> Evaluate(ITrakHoundSystemEntitiesClient client, IEnumerable<string> expressions, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (client != null && !expressions.IsNullOrEmpty())
            {
                var results = new List<TrakHoundExpressionResult>();

                foreach (var expression in expressions)
                {
                    var subExpressions = expression.Split(Delimiter);
                    if (!subExpressions.IsNullOrEmpty())
                    {
                        foreach (var subExpression in subExpressions)
                        {
                            var uuids = await EvaluateSingle(client, subExpression, null, 0, 0, sortOrder);
                            if (!uuids.IsNullOrEmpty())
                            {
                                var ns = TrakHoundPath.GetNamespace(subExpression);

                                foreach (var uuid in uuids)
                                {
                                    results.Add(new TrakHoundExpressionResult(ns, subExpression, uuid));
                                }
                            }
                        }
                    }
                }

                if (skip > 0 || take > 0)
                {
                    return results.Skip(skip > int.MaxValue ? int.MaxValue : (int)skip).Take(take > int.MaxValue ? int.MaxValue : (int)take);
                }
                else
                {
                    return results;
                }
            }

            return null;
        }

        public static async Task<IEnumerable<TrakHoundExpressionResult>> Evaluate(ITrakHoundSystemEntitiesClient client, string expression, IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (client != null && !string.IsNullOrEmpty(expression))
            {
                var subExpressions = expression.Split(Delimiter);
                if (!subExpressions.IsNullOrEmpty())
                {
                    var results = new List<TrakHoundExpressionResult>();

                    foreach (var subExpression in subExpressions)
                    {
                        var uuids = await EvaluateSingle(client, subExpression, parentUuids, 0, 0, sortOrder);
                        if (!uuids.IsNullOrEmpty())
                        {
                            var ns = TrakHoundPath.GetNamespace(subExpression);

                            foreach (var uuid in uuids)
                            {
                                results.Add(new TrakHoundExpressionResult(ns, subExpression, uuid));
                            }
                        }
                    }

                    if (skip > 0 || take > 0)
                    {
                        return results.Skip(skip > int.MaxValue ? int.MaxValue : (int)skip).Take(take > int.MaxValue ? int.MaxValue : (int)take);
                    }
                    else
                    {
                        return results;
                    }
                }
            }

            return null;
        }

        private static async Task<IEnumerable<string>> EvaluateSingle(ITrakHoundSystemEntitiesClient client, string expression, IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var results = new List<string>();

            if (client != null && !string.IsNullOrEmpty(expression))
            {
                var ns = TrakHoundPath.GetNamespace(expression);
                var partialExpression = TrakHoundPath.GetPartialPath(expression);

                if (partialExpression == PathSeparator)
                {
                    // Query Root Objects
                    IEnumerable<ITrakHoundObjectQueryResult> rootUuids;
                    if (!string.IsNullOrEmpty(ns)) rootUuids = await client.Objects.QueryRootUuids(ns, skip, take, sortOrder);
                    else rootUuids = await client.Objects.QueryRootUuids(skip, take, sortOrder);

                    if (!rootUuids.IsNullOrEmpty()) results.AddRange(rootUuids.Select(o => o.Uuid));
                }
                else
                {
                    var normalizeResult = Normalize(ns, partialExpression);
                    if (!normalizeResult.Segments.IsNullOrEmpty())
                    {
                        IEnumerable<string> targetUuids = !normalizeResult.BaseUuids.IsNullOrEmpty() ? normalizeResult.BaseUuids : parentUuids;
                        var i = 0;
                        var m = normalizeResult.Segments.Length;

                        int parentLevel = 0;

                        while (i < m)
                        {
                            var querySegment = normalizeResult.Segments[i];

                            if (querySegment == PathSeparator)
                            {
                                targetUuids = new string[] { null }; // No Parent (Root)
                                //var uuids = await client.Objects.QueryRootUuids(skip, take, sortOrder);
                                //if (i == m - 1) results.AddRange(targetUuids);

                                parentLevel = 0;
                            }
                            else if (querySegment == Wildcard)
                            {
                                var uuids = await client.Objects.QueryUuidsByParentUuid(targetUuids, skip, take);
                                targetUuids = uuids?.Select(o => o.Uuid);
                                if (i == m - 1) results.AddRange(targetUuids);

                                parentLevel = 1;
                            }
                            else if (querySegment == WildcardLevel)
                            {
                                if (i == m - 1)
                                {
                                    var uuids = await client.Objects.QueryChildUuidsByRootUuid(targetUuids, skip, take);
                                    targetUuids = uuids?.Select(o => o.Uuid);
                                    results.AddRange(targetUuids);
                                }

                                parentLevel = 2;
                            }
                            else if (querySegment == PreviousLevel)
                            {
                                var uuids = await client.Objects.QueryByChildUuid(targetUuids, 0, 0);
                                targetUuids = uuids?.Select(o => o.Uuid);
                                if (i == m - 1) results.AddRange(targetUuids);

                                parentLevel = 0;
                            }
                            else
                            {
                                if (i == m - 1) targetUuids = await EvaluateSegment(client, ns, querySegment, targetUuids, parentLevel, false, skip, take);
                                else targetUuids = await EvaluateSegment(client, ns, querySegment, targetUuids, parentLevel, false, skip, take);

                                if (targetUuids.IsNullOrEmpty()) break;
                                if (i == m - 1) results.AddRange(targetUuids);

                                parentLevel = 0;
                            }

                            i++;
                        }
                    }
                    else if (!normalizeResult.BaseUuids.IsNullOrEmpty())
                    {
                        results.AddRange(normalizeResult.BaseUuids);
                    }
                }
            }

            if (skip > 0 || take > 0)
            {
                return results.Skip(skip > int.MaxValue ? int.MaxValue : (int)skip).Take(take > int.MaxValue ? int.MaxValue : (int)take);
            }
            else
            {
                return results;
            }
        }

        private static async Task<IEnumerable<string>> EvaluateSegment(ITrakHoundSystemEntitiesClient client, string ns, string segment, IEnumerable<string> parentUuids, int parentLevel, bool isRoot, long skip = 0, long take = 0)
        {
            IEnumerable<string> results = null;

            if (!string.IsNullOrEmpty(segment))
            {
                // Test OR Patterns
                if (segment.Contains('|'))
                {
                    return await EvaluateOrSegment(client, ns, segment, parentUuids, parentLevel, isRoot, skip, take);
                }

                // Test AND Patterns
                else if (segment.Contains('&'))
                {
                    return await EvaluateAndSegment(client, ns, segment, parentUuids, parentLevel, isRoot, skip, take);
                }

                else
                {
                    return await EvaluatePart(client, ns, segment, parentUuids, parentLevel, isRoot, skip, take);
                }
            }

            return results;
        }

        private static async Task<IEnumerable<string>> EvaluateOrSegment(ITrakHoundSystemEntitiesClient client, string ns, string segment, IEnumerable<string> parentUuids, int parentLevel, bool isRoot, long skip = 0, long take = 0)
        {
            IEnumerable<string> results = null;

            if (!string.IsNullOrEmpty(segment))
            {
                var sResults = new List<string>();

                var matches = _orRegex.Matches(segment);
                foreach (Match match in matches)
                {
                    if (match.Success && match.Groups.Count > 1)
                    {
                        var expression = match.Groups[1].Value;
                        var expressionResults = await EvaluatePart(client, ns, expression, parentUuids, parentLevel, isRoot, skip, take);
                        if (!expressionResults.IsNullOrEmpty())
                        {
                            sResults.AddRange(expressionResults);
                        }
                    }
                }

                results = sResults.Distinct();
            }

            return results;
        }

        private static async Task<IEnumerable<string>> EvaluateAndSegment(ITrakHoundSystemEntitiesClient client, string ns, string segment, IEnumerable<string> parentUuids, int parentLevel, bool isRoot, long skip = 0, long take = 0)
        {
            IEnumerable<string> results = null;

            if (!string.IsNullOrEmpty(segment))
            {
                var keys = new List<string>();
                var uuids = new List<string>();
                var sResults = new ListDictionary<string, string>();
                var isMatched = true;

                var matches = _andRegex.Matches(segment);
                foreach (Match match in matches)
                {
                    if (match.Success && match.Groups.Count > 1)
                    {
                        var expression = match.Groups[1].Value;
                        var expressionResults = await EvaluatePart(client, ns, expression, parentUuids, parentLevel, isRoot, skip, take);
                        if (!expressionResults.IsNullOrEmpty())
                        {
                            keys.Add(expression);
                            uuids.AddRange(expressionResults);
                            sResults.Add(expression, expressionResults);
                        }
                        else
                        {
                            isMatched = false;
                            break;
                        }                        
                    }
                }

                if (isMatched)
                {
                    var x = new List<string>();

                    var dUuids = uuids.Distinct();
                    foreach (var uuid in dUuids)
                    {
                        if (sResults.AllKeysContains(uuid))
                        {
                            x.Add(uuid);
                        }
                    }

                    results = x.Distinct();
                }
            }

            return results;
        }

        private static async Task<IEnumerable<string>> EvaluatePart(ITrakHoundSystemEntitiesClient client, string ns, string segment, IEnumerable<string> parentUuids, int parentLevel, bool isRoot, long skip = 0, long take = 0)
        {
            IEnumerable<string> results = null;

            if (client != null)
            {
                var pUuids = isRoot ? new string[] { null } : parentUuids;

                var queryRequest = new TrakHoundObjectQueryRequest();
                queryRequest.Type = GetRequestType(segment);
                queryRequest.Namespace = ns;

                switch (queryRequest.Type)
                {
                    case TrakHoundObjectQueryRequestType.Metadata:

                        var metadataQueryType = GetMetadataQueryType(segment);
                        var metadataName = GetMetadataName(metadataQueryType, segment);
                        var metadataValue = GetMetadataValue(metadataQueryType, segment);

                        if (!parentUuids.IsNullOrEmpty())
                        {
                            IEnumerable<string> objectUuids;
                            if (parentLevel > 0) objectUuids = (await client.Objects.QueryByParentUuid(pUuids, skip, take))?.Select(o => o.Uuid);
                            else objectUuids = (await client.Objects.QueryChildrenByRootUuid(pUuids, skip, take))?.Select(o => o.Uuid);
                            if (!objectUuids.IsNullOrEmpty())
                            {
                                return (await (client.Objects.Metadata.QueryByEntityUuid(objectUuids, metadataName, metadataQueryType, metadataValue)))?.Select(o => o.EntityUuid).Distinct();
                            }
                        }
                        else
                        {
                            // Need to filter by NAMESPACE
                            return (await (client.Objects.Metadata.QueryByName(metadataName, metadataQueryType, metadataValue)))?.Select(o => o.EntityUuid).Distinct();
                        }

                        break;

                    default:

                        if (queryRequest.Type == TrakHoundObjectQueryRequestType.DefinitionUuid)
                        {
                            var definitionType = FormatSegment(segment);
                            var definitionUuids = (await client.Definitions.QueryIdsByType(definitionType, 0, 0))?.Select(o => o.Uuid);
                            if (!definitionUuids.IsNullOrEmpty())
                            {
                                var queryDefinitionUuids = new List<string>();
                                queryDefinitionUuids.AddRange(definitionUuids);

                                var definitions = await client.Definitions.QueryByParentUuid(definitionUuids);
                                if (!definitions.IsNullOrEmpty())
                                {
                                    queryDefinitionUuids.AddRange(definitions.Select(o => o.Uuid).Distinct());
                                }

                                queryRequest.Queries = queryDefinitionUuids;
                            }
                        }
                        else
                        {
                            queryRequest.Queries = new string[] { FormatSegment(segment) };
                        }

                        queryRequest.ParentLevel = parentLevel;
                        queryRequest.ParentUuids = pUuids;

                        results = (await client.Objects.QueryUuids(queryRequest, skip, take))?.Select(o => o.Uuid);
                        break;
                }             
            }

            return results;
        }

        private static TrakHoundObjectQueryRequestType GetRequestType(string segment)
        {
            if (segment != null)
            {
                if (_uuidRegex.IsMatch(segment)) return TrakHoundObjectQueryRequestType.Uuid;
                if (_contentTypeRegex.IsMatch(segment)) return TrakHoundObjectQueryRequestType.ContentType;
                if (_typeRegex.IsMatch(segment)) return TrakHoundObjectQueryRequestType.DefinitionUuid;
                if (_metadataRegex.IsMatch(segment)) return TrakHoundObjectQueryRequestType.Metadata;

                return TrakHoundObjectQueryRequestType.Name;
            }

            return default;
        }

        private static string FormatSegment(string segment)
        {
            if (segment != null)
            {
                if (_uuidRegex.IsMatch(segment)) return segment.Substring(_uuidPrefix.Length);
                if (_contentTypeRegex.IsMatch(segment)) return segment.Substring(_contentTypePrefix.Length);
                if (_typeRegex.IsMatch(segment)) return segment.Substring(_typePrefix.Length);

                return segment;
            }

            return default;
        }

        private static TrakHoundMetadataQueryType GetMetadataQueryType(string segment)
        {
            if (segment != null)
            {
                if (_metadataLikeRegex.IsMatch(segment)) return TrakHoundMetadataQueryType.Equal;
            }

            return default;
        }

        private static string GetMetadataName(TrakHoundMetadataQueryType queryType, string segment)
        {
            if (segment != null)
            {
                Match match;
                switch (queryType)
                {
                    case TrakHoundMetadataQueryType.Like: match = _metadataLikeRegex.Match(segment); break;
                    default: match = _metadataRegex.Match(segment); break;
                }
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }

                return segment;
            }

            return default;
        }

        private static string GetMetadataValue(TrakHoundMetadataQueryType queryType, string segment)
        {
            if (segment != null)
            {
                Match match;
                switch (queryType)
                {
                    case TrakHoundMetadataQueryType.Like: match = _metadataLikeRegex.Match(segment); break;
                    default: match = _metadataRegex.Match(segment); break;
                }
                if (match.Success)
                {
                    return match.Groups[2].Value;
                }

                return segment;
            }

            return default;
        }


        public static IEnumerable<ITrakHoundObjectEntity> Match(string query, TrakHoundEntityCollection collection)
        {
            var matches = new List<ITrakHoundObjectEntity>();

            if (!string.IsNullOrEmpty(query) && collection != null)
            {
                var queries = query.Split(Delimiter);
                if (!queries.IsNullOrEmpty())
                {
                    foreach (var q in queries)
                    {
                        if (!string.IsNullOrEmpty(q))
                        {
                            matches.AddRange(MatchQuery(q, collection));
                        }
                    }
                }
            }

            return matches.ToDistinct();
        }

        //public static IEnumerable<ITrakHoundObjectEntity> Match(ITrakHoundSystemEntitiesClient client, IEnumerable<string> queries, ITrakHoundObjectEntity obj)
        //{
        //    if (obj != null)
        //    {
        //        return Match(client, queries, new ITrakHoundObjectEntity[] { obj });
        //    }

        //    return Enumerable.Empty<ITrakHoundObjectEntity>();
        //}

        //public static IEnumerable<ITrakHoundObjectEntity> Match(ITrakHoundSystemEntitiesClient client, IEnumerable<string> queries, IEnumerable<ITrakHoundObjectEntity> objs)
        //{
        //    var matches = new List<ITrakHoundObjectEntity>();

        //    if (!queries.IsNullOrEmpty())
        //    {
        //        var querySingles = new List<string>();
        //        foreach (var query in queries)
        //        {
        //            querySingles.AddRange(query.Split(Delimiter));
        //        }

        //        if (!querySingles.IsNullOrEmpty())
        //        {
        //            //var fObjs = await client.Objects.QueryChildrenByRootUuid(querySingles);
        //            ////var fObjs = objs.ToFlatList();
        //            //if (!fObjs.IsNullOrEmpty())
        //            //{
        //            //    foreach (var q in querySingles)
        //            //    {
        //            //        if (!string.IsNullOrEmpty(q))
        //            //        {
        //            //            matches.AddRange(MatchQuery(q, fObjs));
        //            //        }
        //            //    }
        //            //}
        //        }
        //    }

        //    return matches.ToDistinct();
        //}


        //public static bool IsMatch(string query, ITrakHoundObjectEntityModel obj)
        //{
        //    if (!string.IsNullOrEmpty(query) && obj != null)
        //    {
        //        return !Match(query, obj).IsNullOrEmpty();
        //    }

        //    return false;
        //}

        //public static IEnumerable<ITrakHoundObjectEntityModel> IsMatch(string query, IEnumerable<ITrakHoundObjectEntityModel> objs)
        //{
        //    var matched = new List<ITrakHoundObjectEntityModel>();

        //    if (!string.IsNullOrEmpty(query) && !objs.IsNullOrEmpty())
        //    {
        //        foreach (var obj in objs)
        //        {
        //            var matches = Match(query, obj);
        //            if (!matches.IsNullOrEmpty())
        //            {
        //                matched.AddRange(matches);
        //            }
        //        }
        //    }

        //    return matched.ToDistinct();
        //}

        //public static bool IsMatch(IEnumerable<string> queries, ITrakHoundObjectEntityModel obj)
        //{
        //    if (!queries.IsNullOrEmpty() && obj != null)
        //    {
        //        return !Match(queries, obj).IsNullOrEmpty();
        //    }

        //    return false;
        //}

        //public static IEnumerable<ITrakHoundObjectEntityModel> IsMatch(IEnumerable<string> queries, IEnumerable<ITrakHoundObjectEntityModel> objs)
        //{
        //    var matched = new List<ITrakHoundObjectEntityModel>();

        //    if (!queries.IsNullOrEmpty() && !objs.IsNullOrEmpty())
        //    {
        //        foreach (var obj in objs)
        //        {
        //            if (!Match(queries, obj).IsNullOrEmpty())
        //            {
        //                matched.Add(obj);
        //            }
        //        }
        //    }

        //    return matched;
        //}

        private static IEnumerable<ITrakHoundObjectEntity> MatchQuery(string query, TrakHoundEntityCollection collection)
        {
            var matches = new List<ITrakHoundObjectEntity>();

            var objs = collection?.Objects.Objects;
            if (!string.IsNullOrEmpty(query) && !objs.IsNullOrEmpty())
            {
                if (query == Wildcard)
                {
                    matches.AddRange(objs);
                }
                else if (query == PathSeparator)
                {
                    matches.AddRange(objs.Where(o => o.ParentUuid == null));
                }
                else
                {
                    var segmentObjs = objs;

                    var ns = TrakHoundPath.GetNamespace(query);
                    var partialPath = TrakHoundPath.GetPartialPath(query);

                    // Filter by Namespace
                    if (!string.IsNullOrEmpty(ns)) segmentObjs = segmentObjs.Where(o => o.Namespace.ToLower() == ns.ToLower());

                    var parts = partialPath.Split(PathSeparator);
                    if (!parts.IsNullOrEmpty())
                    {
                        var path = parts[0];
                        var reversePath = false;

                        for (var i = 0; i < parts.Length; i++)
                        {
                            var part = parts[i];

                            if (part == WildcardLevel)
                            {
                                var allChildren = new List<ITrakHoundObjectEntity>();
                                allChildren.AddRange(segmentObjs);
                                foreach (var obj in segmentObjs)
                                {
                                    //allChildren.AddRange(obj.GetAllChildren());
                                }
                                segmentObjs = allChildren;
                                reversePath = false;
                            }
                            else if (part == "" && i == 0) // Detect Root
                            {
                                segmentObjs = segmentObjs.Where(o => o.ParentUuid == null);
                                reversePath = false;
                            }
                            else if (part == PreviousLevel)
                            {
                                var parents = new List<ITrakHoundObjectEntity>();
                                foreach (var obj in segmentObjs)
                                {
                                    //if (obj.Parent != null) parents.Add(obj.Parent);
                                }
                                segmentObjs = parents;
                                reversePath = true;
                            }
                            else
                            {
                                //if (reversePath) segmentObjs = segmentObjs.GetChildren();

                                var partObjs = segmentObjs.Where(o => IsPartMatch(part, collection, o));

                                //if (!partObjs.IsNullOrEmpty() && i < parts.Length - 1)
                                if (!partObjs.IsNullOrEmpty() && i < parts.Length - 1 && parts[i + 1] != PreviousLevel)
                                {
                                    var childObjects = new List<ITrakHoundObjectEntity>();
                                    foreach (var partObj in partObjs)
                                    {
                                        var partChildObjects = collection.Objects.QueryObjectsByParentUuid(partObj.Uuid);
                                        if (!partChildObjects.IsNullOrEmpty())
                                        {
                                            childObjects.AddRange(partChildObjects);
                                        }
                                    }
                                    segmentObjs = childObjects;
                                    //segmentObjs = partObjs.GetChildren();
                                }
                                else segmentObjs = partObjs;

                                reversePath = false;
                            }

                            if (segmentObjs.IsNullOrEmpty()) break;

                            if (i > 0) path += "/" + part;
                        }
                    }

                    matches.AddRange(segmentObjs);
                }

            }

            return matches;
        }


        //private static async Task<IEnumerable<string>> MatchQuery(ITrakHoundSystemEntitiesClient client, string expression, IEnumerable<string> parentUuids, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    var results = new List<string>();

        //    if (client != null && !string.IsNullOrEmpty(expression))
        //    {
        //        if (expression == PathSeparator)
        //        {
        //            // Query Root Objects
        //            var rootUuids = await client.Objects.QueryUuidsByParentUuid((string)null, 0, int.MaxValue, sortOrder);
        //            if (!rootUuids.IsNullOrEmpty()) results.AddRange(rootUuids.Select(o => o.Uuid));
        //        }
        //        else
        //        {
        //            // Detect if Root
        //            var isRoot = expression.StartsWith(PathSeparator);

        //            // Remove leading PathSeparator
        //            var exp = isRoot ? expression.Substring(1) : expression;

        //            var querySegments = exp.Split(PathSeparator);
        //            if (!querySegments.IsNullOrEmpty())
        //            {
        //                IEnumerable<string> targetUuids = parentUuids;
        //                var i = 0;
        //                var m = querySegments.Length;

        //                int parentLevel = 1;

        //                //// Get absolute segments (read as path instead of querying)
        //                //var absoluteParts = new List<string>();
        //                //if (isRoot)
        //                //{
        //                //    for (var j = 0; j < querySegments.Length; j++)
        //                //    {
        //                //        var querySegment = querySegments[j];
        //                //        if (TrakHoundObjectPath.GetType(querySegment) == TrakHoundObjectPathType.Path)
        //                //        {
        //                //            absoluteParts.Add(querySegment);
        //                //            i++;
        //                //        }
        //                //        else
        //                //        {
        //                //            break;
        //                //        }
        //                //    }

        //                //    if (!absoluteParts.IsNullOrEmpty())
        //                //    {
        //                //        if (!parent)
        //                //        targetUuids = 
        //                //    }
        //                //}

        //                while (i < m)
        //                {
        //                    var querySegment = querySegments[i];

        //                    if (querySegment == Wildcard)
        //                    {
        //                        if (parentLevel > 1) parentLevel++;
        //                        else parentLevel = 2;
        //                    }
        //                    else if (querySegment == WildcardLevel) parentLevel = 0;
        //                    else if (querySegment == PreviousLevel)
        //                    {
        //                        var uuids = await client.Objects.QueryByChildUuid(targetUuids, 0, int.MaxValue);
        //                        targetUuids = uuids?.Select(o => o.Uuid);
        //                        if (i == m - 1) results.AddRange(targetUuids);
        //                    }
        //                    else
        //                    {
        //                        targetUuids = await EvaluateSegment(client, querySegment, targetUuids, parentLevel, isRoot);
        //                        if (targetUuids.IsNullOrEmpty()) break;
        //                        if (i == m - 1) results.AddRange(targetUuids);
        //                        parentLevel = 1;
        //                    }

        //                    // If querySegment ends with wildcard
        //                    if (parentLevel != 1 && i == m - 1)
        //                    {
        //                        if (parentLevel > 0)
        //                        {
        //                            for (var j = 1; j < parentLevel; j++)
        //                            {
        //                                var uuids = await client.Objects.QueryUuidsByParentUuid(targetUuids, 0, int.MaxValue);
        //                                targetUuids = uuids?.Select(o => o.Uuid);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var uuids = await client.Objects.QueryChildUuidsByRootUuid(targetUuids, 0, int.MaxValue);
        //                            targetUuids = uuids?.Select(o => o.Uuid);
        //                        }

        //                        if (targetUuids.IsNullOrEmpty()) break;
        //                        results.AddRange(targetUuids);
        //                    }

        //                    isRoot = false;
        //                    i++;
        //                }
        //            }
        //        }
        //    }

        //    return results.Skip(skip > int.MaxValue ? int.MaxValue : (int)skip).Take(take > int.MaxValue ? int.MaxValue : (int)take);
        //}

        //private static IEnumerable<ITrakHoundObjectEntityModel> MatchQuery(string query, TrakHoundEntityCollection collection)
        //{
        //    var matches = new List<ITrakHoundObjectEntityModel>();

        //    var objs = collection?.Objects.GetObjectPartialModels(collection.Objects.Objects?.Select(o => o.Uuid));
        //    if (!string.IsNullOrEmpty(query) && !objs.IsNullOrEmpty())
        //    {
        //        if (query == Wildcard)
        //        {
        //            matches.AddRange(objs);
        //        }
        //        else if (query == PathSeparator)
        //        {
        //            matches.AddRange(objs.Where(o => o.ParentUuid == null));
        //        }
        //        else
        //        {
        //            var segmentObjs = objs;

        //            var parts = query.Split(PathSeparator);
        //            if (!parts.IsNullOrEmpty())
        //            {
        //                var path = parts[0];
        //                var reversePath = false;

        //                for (var i = 0; i < parts.Length; i++)
        //                {
        //                    var part = parts[i];

        //                    if (part == WildcardLevel)
        //                    {
        //                        var allChildren = new List<ITrakHoundObjectEntityModel>();
        //                        allChildren.AddRange(segmentObjs);

        //                        var objChildren = collection.Objects.QueryObjectModelsByParentUuid(segmentObjs.Select(o => o.Uuid));
        //                        if (!objChildren.IsNullOrEmpty()) allChildren.AddRange(objChildren);

        //                        segmentObjs = allChildren;
        //                        reversePath = false;
        //                    }                          
        //                    else if (part == "" && i == 0) // Detect Root
        //                    {
        //                        segmentObjs = segmentObjs.Where(o => o.ParentUuid == null);
        //                        reversePath = false;
        //                    }
        //                    else if (part == PreviousLevel)
        //                    {
        //                        var parents = new List<ITrakHoundObjectEntityModel>();
        //                        foreach (var obj in segmentObjs)
        //                        {
        //                            if (obj.Parent != null) parents.Add(obj.Parent);
        //                        }
        //                        segmentObjs = parents;
        //                        reversePath = true;
        //                    }
        //                    else
        //                    {
        //                        if (reversePath) segmentObjs = segmentObjs.GetChildren();

        //                        var partObjs = segmentObjs.Where(o => IsPartMatch(part, o));

        //                        //if (!partObjs.IsNullOrEmpty() && i < parts.Length - 1)
        //                        if (!partObjs.IsNullOrEmpty() && i < parts.Length - 1 && parts[i + 1] != PreviousLevel)
        //                        {
        //                            segmentObjs = collection.Objects.QueryObjectModelsByParentUuid(segmentObjs.Select(o => o.Uuid));

        //                            //foreach (var obj in segmentObjs)
        //                            //{
        //                            //    var objChildren = collection.Objects.QueryObjectModelsByParentUuid(obj.Uuid);
        //                            //    if (!objChildren.IsNullOrEmpty()) allChildren.AddRange(objChildren);
        //                            //}

        //                            //segmentObjs = partObjs.GetChildren();
        //                        }
        //                        else segmentObjs = partObjs;

        //                        reversePath = false;
        //                    }

        //                    if (segmentObjs.IsNullOrEmpty()) break;

        //                    if (i > 0) path += "/" + part;
        //                }
        //            }

        //            matches.AddRange(segmentObjs);
        //        }

        //    }

        //    return matches;
        //}

        //private static IEnumerable<ITrakHoundObjectEntity> MatchQuery(string query, IEnumerable<ITrakHoundObjectEntity> objs)
        //{
        //    var matches = new List<ITrakHoundObjectEntity>();

        //    if (!string.IsNullOrEmpty(query) && !objs.IsNullOrEmpty())
        //    {
        //        if (query == Wildcard)
        //        {
        //            matches.AddRange(objs);
        //        }
        //        else if (query == PathSeparator)
        //        {
        //            matches.AddRange(objs.Where(o => o.ParentUuid == null));
        //        }
        //        else
        //        {
        //            var segmentObjs = objs;

        //            var parts = query.Split(PathSeparator);
        //            if (!parts.IsNullOrEmpty())
        //            {
        //                var path = parts[0];

        //                for (var i = 0; i < parts.Length; i++)
        //                {
        //                    var part = parts[i];

        //                    if (part == WildcardLevel)
        //                    {
        //                        var allChildren = new List<ITrakHoundObjectEntity>();
        //                        allChildren.AddRange(segmentObjs);
        //                        foreach (var obj in segmentObjs)
        //                        {
        //                            //allChildren.AddRange(obj.GetAllChildren());
        //                        }
        //                        segmentObjs = allChildren;
        //                    }
        //                    else if (part == "" && i == 0) // Detect Root
        //                    {
        //                        segmentObjs = segmentObjs.Where(o => o.ParentUuid == null);
        //                    }
        //                    else
        //                    {
        //                        var partObjs = segmentObjs.Where(o => IsPartMatch(part, o));

        //                        if (!partObjs.IsNullOrEmpty() && i < parts.Length - 1)
        //                        {
        //                            //segmentObjs = partObjs.GetChildren();
        //                        }
        //                        else segmentObjs = partObjs;
        //                    }

        //                    if (segmentObjs.IsNullOrEmpty()) break;

        //                    if (i > 0) path += "/" + part;
        //                }
        //            }

        //            matches.AddRange(segmentObjs);
        //        }

        //    }

        //    return matches;
        //}

        private static bool IsPartMatch(string part, TrakHoundEntityCollection collection, ITrakHoundObjectEntity obj)
        {
            var match = false;

            if (!string.IsNullOrEmpty(part) && obj != null)
            {
                // Test OR Patterns
                if (part.Contains('|'))
                {
                    var orQueries = _orRegex.Matches(part);
                    foreach (Match query in orQueries)
                    {
                        if (query.Success && query.Groups.Count > 1)
                        {
                            match = IsQueryMatch(query.Groups[1].Value, collection, obj);
                            if (match) break; // Only Require One Query to Match
                        }
                    }
                }

                // Test AND Patterns
                else if (part.Contains('&'))
                {
                    var andQueries = _andRegex.Matches(part);
                    foreach (Match query in andQueries)
                    {
                        if (query.Success && query.Groups.Count > 1)
                        {
                            match = IsQueryMatch(query.Groups[1].Value, collection, obj);
                            if (!match) break; // Require Every Query to Match
                        }
                    }
                }

                else
                {
                    match = IsQueryMatch(part, collection, obj);
                }
            }

            return match;
        }

        private static bool IsQueryMatch(string query, TrakHoundEntityCollection collection, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null)
            {
                var match = query == Wildcard;

                if (!match) match = IsNameMatch(query, obj);
                if (!match) match = IsUuidMatch(query, obj);
                if (!match) match = IsNamespaceMatch(query, obj);
                if (!match) match = IsContentTypeMatch(query, obj);
                if (!match) match = IsTypeMatch(query, collection, obj);
                if (!match) match = IsMetadataMatch(query, obj);

                return match;
            }

            return false;
        }

        private static bool IsNameMatch(string query, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null && !string.IsNullOrEmpty(obj.Name))
            {
                // Process Like match
                var match = _nameLikeRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var nameValue = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(nameValue))
                    {
                        var likePattern = nameValue.ToLower().Replace("*", ".*");
                        var regex = new Regex(likePattern);
                        return regex.IsMatch(obj.Name.ToLower());
                    }
                }
                else
                {
                    return obj.Name.ToLower() == query.ToLower();
                }
            }

            return false;
        }

        private static bool IsUuidMatch(string query, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null)
            {
                var match = _uuidRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var idValue = match.Groups[1].Value;
                    if (idValue != null && obj.Uuid != null)
                    {
                        return idValue.ToLower() == obj.Uuid.ToLower();
                    }
                }
            }

            return false;
        }

        private static bool IsNamespaceMatch(string query, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null)
            {
                var match = _contentTypeRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var namespaceValue = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(obj.ContentType) && !string.IsNullOrEmpty(namespaceValue))
                    {
                        return obj.Namespace.ToLower() == namespaceValue.ToLower();
                    }
                }
            }

            return false;
        }

        private static bool IsContentTypeMatch(string query, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null)
            {
                var match = _contentTypeRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var contentTypeValue = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(obj.ContentType) && !string.IsNullOrEmpty(contentTypeValue))
                    {
                        return obj.ContentType.ToLower() == contentTypeValue.ToLower();
                    }
                }
            }

            return false;
        }

        private static bool IsTypeMatch(string query, TrakHoundEntityCollection collection, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null)
            {
                var match = _typeRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var typeValue = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(typeValue))
                    {
                        typeValue = typeValue.ToLower();

                        var parentDefinitions = collection.Definitions.QueryDefinitionsByChildUuid(obj.DefinitionUuid);
                        if (!parentDefinitions.IsNullOrEmpty())
                        {
                            return parentDefinitions.Select(o => o.Type.ToLower()).Contains(typeValue);
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsMetadataMatch(string query, ITrakHoundObjectEntity obj)
        {
            if (!string.IsNullOrEmpty(query) && obj != null)
            {
                //// Process exact match
                //var match = _metadataRegex.Match(query);
                //if (match.Success && match.Groups.Count > 2)
                //{
                //    var nameValue = match.Groups[1].Value;
                //    var metadataValue = obj.GetMetadata(nameValue.ToPascalCase());
                //    if (metadataValue != null)
                //    {
                //        var valueValue = match.Groups[2].Value;
                //        if (valueValue != null)
                //        {
                //            return valueValue.ToLower() == metadataValue.ToLower();
                //        }
                //    }
                //}
                //else
                //{
                //    match = _metadataLikeRegex.Match(query);
                //    if (match.Success && match.Groups.Count > 2)
                //    {
                //        var nameValue = match.Groups[1].Value;
                //        var metadataValue = obj.GetMetadata(nameValue.ToPascalCase());
                //        if (metadataValue != null)
                //        {
                //            var valuePattern = match.Groups[2].Value;
                //            if (valuePattern != null)
                //            {
                //                var likePattern = valuePattern.ToLower().Replace("*", ".*");
                //                var regex = new Regex(likePattern);
                //                return regex.IsMatch(metadataValue.ToLower());
                //            }
                //        }
                //    }
                //}
            }

            return false;
        }

        /// <summary>
        /// Converts multiple TrakQL queries into a single string
        /// </summary>
        public static string Convert(IEnumerable<string> queries)
        {
            if (!queries.IsNullOrEmpty())
            {
                var q = "";

                foreach (var query in queries.Distinct())
                {
                    var x = query.Trim(';');
                    q += x + ";";
                }

                return q;
            }

            return null;
        }

        private static NormalizeResult Normalize(string ns, string expression)
        {
            var result = new NormalizeResult();

            if (!string.IsNullOrEmpty(expression))
            {
                //if (expression.StartsWith(PathSeparator))
                if (!string.IsNullOrEmpty(ns) && expression.StartsWith(PathSeparator))
                {
                    string absolutePath = null;
                    int expressionStartIndex = 0;

                    var segments = expression.TrimStart('/').Split(PathSeparator);
                    for (var i = 0; i < segments.Length; i++)
                    {
                        if (!IsSegmentExpression(segments[i]))
                        {
                            absolutePath = TrakHoundPath.Combine(absolutePath, segments[i]);
                            expressionStartIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(absolutePath))
                    {
                        result.BaseUuids = new string[] { TrakHoundPath.GetUuid(ns, absolutePath) };
                    }

                    var resultSegments = new List<string>();
                    for (var i = expressionStartIndex; i < segments.Length; i++)
                    {
                        resultSegments.Add(segments[i]);
                    }
                    result.Segments = resultSegments.ToArray();
                }
                else
                {
                    result.Segments = expression.Split(PathSeparator, System.StringSplitOptions.RemoveEmptyEntries);

                    // Add Root PathSeparater (if exists)
                    if (!result.Segments.IsNullOrEmpty() && expression.StartsWith(PathSeparator))
                    {
                        var segments = new string[result.Segments.Length + 1];
                        segments[0] = PathSeparator;
                        for (var i = 0; i < result.Segments.Length; i++)
                        {
                            segments[1 + i] = result.Segments[i];
                        }
                        result.Segments = segments;
                    }
                }               
            }

            return result;
        }

        private static bool IsSegmentExpression(string segment)
        {
            if (!string.IsNullOrEmpty(segment))
            {
                if (segment.Contains("*")) return true;
                if (segment.Contains("..")) return true;
                if (segment.Contains("type=")) return true;
                if (segment.Contains("meta@")) return true;
            }

            return false;
        }
    }
}
