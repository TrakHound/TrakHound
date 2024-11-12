// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Routing
{
    /// <summary>
    /// Configuration used to configure Routes to Drivers
    /// </summary>
    public class TrakHoundRouteConfiguration
    {
        /// <summary>
        /// A unique Identifier for the Route
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// List of patterns used to match the desired route(s)
        /// </summary>
        public IEnumerable<string> Patterns { get; set; }

        public IEnumerable<string> Filters { get; set; }

        /// <summary>
        /// List of Targets to use when the Route pattern(s) are matched
        /// </summary>
        public IEnumerable<TrakHoundTargetConfiguration> Targets { get; set; }


        public TrakHoundRouteConfiguration()
        {
            Id = Guid.NewGuid().ToString();
        }


        public void AddPattern(string pattern)
        {
            if (pattern != null)
            {
                var patterns = Patterns?.ToList();
                if (patterns == null) patterns = new List<string>();
                patterns.Add(pattern);
                Patterns = patterns;
            }
        }

        public void RemovePattern(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern) && !Patterns.IsNullOrEmpty())
            {
                var patterns = Patterns.ToList();
                patterns.RemoveAll(o => o == pattern);
                if (patterns.Count > 0)
                {
                    Patterns = patterns;
                }
                else Patterns = null;
            }
        }



        public void AddFilter(string filter)
        {
            if (filter != null)
            {
                var filters = Filters?.ToList();
                if (filters == null) filters = new List<string>();
                filters.Add(filter);
                Filters = filters;
            }
        }

        public void RemoveFilter(string filter)
        {
            if (!string.IsNullOrEmpty(filter) && !Filters.IsNullOrEmpty())
            {
                var filters = Filters.ToList();
                filters.RemoveAll(o => o == filter);
                if (filters.Count > 0)
                {
                    Filters = filters;
                }
                else Filters = null;
            }
        }


        public void AddTarget(TrakHoundTargetConfiguration target)
        {
            if (target != null)
            {
                var targets = Targets?.ToList();
                if (targets == null) targets = new List<TrakHoundTargetConfiguration>();
                targets.Add(target);
                Targets = targets;
            }
        }

        public void RemoveTarget(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Targets.IsNullOrEmpty())
            {
                var targets = Targets.ToList();
                targets.RemoveAll(o => o.Id == id);
                if (targets.Count > 0)
                {
                    Targets = targets;
                }
                else Targets = null;
            }
        }

        public void RemoveRedirect(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Targets.IsNullOrEmpty())
            {
                foreach (var target in Targets)
                {
                    target.RemoveRedirect(id);
                }
            }
        }
    }
}
