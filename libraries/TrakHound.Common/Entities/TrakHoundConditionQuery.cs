// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrakHound.Entities
{
    public static class TrakHoundConditionQuery
    {
        private const string EqualPattern = "=(.*)";
        private const string GreaterThanPattern = @"^>([0-9]*\.?[0-9]*)$";
        private const string LessThanPattern = @"^<([0-9]*\.?[0-9]*)$";
        private const string BetweenPattern = @">([0-9]*\.?[0-9]*)<([0-9]*\.?[0-9]*)";

        private static readonly Regex _equalRegex = new Regex(EqualPattern);
        private static readonly Regex _lessThanRegex = new Regex(LessThanPattern);
        private static readonly Regex _greaterThanRegex = new Regex(GreaterThanPattern);
        private static readonly Regex _betwenRegex = new Regex(BetweenPattern);


        public static IEnumerable<ITrakHoundObjectObservationEntity> Match(string query, IEnumerable<ITrakHoundObjectObservationEntity> objs)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (_equalRegex.IsMatch(query)) return MatchEquals(query, objs);

                if (_greaterThanRegex.IsMatch(query)) return MatchGreaterThan(query, objs);

                if (_lessThanRegex.IsMatch(query)) return MatchLessThan(query, objs);

                if (_betwenRegex.IsMatch(query)) return MatchBetween(query, objs);
            }

            return objs;
        }

        private static IEnumerable<ITrakHoundObjectObservationEntity> MatchEquals(string query, IEnumerable<ITrakHoundObjectObservationEntity> objs)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = _equalRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var value = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return objs.Where(o => o.Value == value);
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundObjectObservationEntity>();
        }

        private static IEnumerable<ITrakHoundObjectObservationEntity> MatchGreaterThan(string query, IEnumerable<ITrakHoundObjectObservationEntity> objs)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = _greaterThanRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var value = match.Groups[1].Value.ToDouble();
                    return objs.Where(o => o.Value.IsNumeric() && o.Value.ToDouble() > value);
                }
            }

            return Enumerable.Empty<ITrakHoundObjectObservationEntity>();
        }

        private static IEnumerable<ITrakHoundObjectObservationEntity> MatchLessThan(string query, IEnumerable<ITrakHoundObjectObservationEntity> objs)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = _lessThanRegex.Match(query);
                if (match.Success && match.Groups.Count > 1)
                {
                    var value = match.Groups[1].Value.ToDouble();
                    return objs.Where(o => o.Value.IsNumeric() && o.Value.ToDouble() < value);
                }
            }

            return Enumerable.Empty<ITrakHoundObjectObservationEntity>();
        }


        private static IEnumerable<ITrakHoundObjectObservationEntity> MatchBetween(string query, IEnumerable<ITrakHoundObjectObservationEntity> objs)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var match = _betwenRegex.Match(query);
                if (match.Success && match.Groups.Count > 2)
                {
                    var greaterThan = match.Groups[1].Value.ToDouble();
                    var lessThan = match.Groups[2].Value.ToDouble();

                    return objs.Where(o => o.Value.IsNumeric() && o.Value.ToDouble() > greaterThan && o.Value.ToDouble() < lessThan);
                }
            }

            return Enumerable.Empty<ITrakHoundObjectObservationEntity>();
        }

    }
}
