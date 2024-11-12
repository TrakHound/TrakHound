// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Routing
{
    /// <summary>
    /// Configuration used to configure Route Targets
    /// </summary>
    public class TrakHoundTargetConfiguration
    {
        /// <summary>
        /// A unique Identifier for the Target
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The ID (or Type) of the Driver or Router to direct Requests to
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The Type of Target (Driver or Router)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// List of Redirects to redirect requests upon the specified 'On' condition returned from the Target
        /// </summary>
        public IEnumerable<TrakHoundRedirectConfiguration> Redirects { get; set; }


        public TrakHoundTargetConfiguration()
        {
            Id = Guid.NewGuid().ToString();
        }


        public void AddRedirect(TrakHoundRedirectConfiguration redirect)
        {
            if (redirect != null)
            {
                var redirects = Redirects?.ToList();
                if (redirects == null) redirects = new List<TrakHoundRedirectConfiguration>();
                redirects.Add(redirect);
                Redirects = redirects;
            }
        }

        public void RemoveRedirect(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Redirects.IsNullOrEmpty())
            {
                var redirects = Redirects.ToList();
                redirects.RemoveAll(o => o.Id == id);
                if (redirects.Count > 0)
                {
                    Redirects = redirects;
                }
                else Redirects = null;
            }
        }
    }
}
