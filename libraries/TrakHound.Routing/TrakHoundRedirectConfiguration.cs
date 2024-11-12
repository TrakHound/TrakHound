// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Routing
{
    /// <summary>
    /// Configuration used to configure Route Target Redirects
    /// </summary>
    public class TrakHoundRedirectConfiguration
    {
        public string Id { get; set; }

        /// <summary>
        /// The result of the Router that will trigger this redirect
        /// </summary>
        public IEnumerable<string> Conditions { get; set; }

        /// <summary>
        /// List of Targets to use when the Redirect is triggered
        /// </summary>
        public IEnumerable<TrakHoundTargetConfiguration> Targets { get; set; }

        /// <summary>
        /// List of Options to use when the Redirect is triggered
        /// </summary>
        public IEnumerable<RouteRedirectOptions> Options { get; set; }


        public TrakHoundRedirectConfiguration()
        {
            Id = Guid.NewGuid().ToString();
        }


        public void AddCondition(string condition)
        {
            if (condition != null)
            {
                var conditions = Conditions?.ToList();
                if (conditions == null) conditions = new List<string>();
                conditions.Add(condition);
                Conditions = conditions;
            }
        }

        public void AddCondition(TrakHoundResultType condition)
        {
            var conditions = Conditions?.ToList();
            if (conditions == null) conditions = new List<string>();
            conditions.Add(condition.ToString());
            Conditions = conditions;
        }

        public void RemoveCondition(string condition)
        {
            if (!string.IsNullOrEmpty(condition) && !Conditions.IsNullOrEmpty())
            {
                var conditions = Conditions.ToList();
                conditions.RemoveAll(o => o == condition);
                if (conditions.Count > 0)
                {
                    Conditions = conditions;
                }
                else Conditions = null;
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


        public void AddOption(string option)
        {
            var options = Options?.ToList();
            if (options == null) options = new List<RouteRedirectOptions>();
            options.Add(option.ConvertEnum<RouteRedirectOptions>());
            Options = options;
        }

        public void AddOption(RouteRedirectOptions option)
        {
            var options = Options?.ToList();
            if (options == null) options = new List<RouteRedirectOptions>();
            options.Add(option);
            Options = options;
        }

        public void RemoveOption(string option)
        {
            if (!string.IsNullOrEmpty(option) && !Options.IsNullOrEmpty())
            {
                var options = Options.ToList();
                options.RemoveAll(o => o.ToString() == option);
                if (options.Count > 0)
                {
                    Options = options;
                }
                else Options = null;
            }
        }
    }
}
