// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Clients
{
    public abstract class TrakHoundClientProviderBase
    {
        private readonly List<ITrakHoundClientMiddleware> _middleware = new List<ITrakHoundClientMiddleware>();


        public IEnumerable<ITrakHoundClientMiddleware> Middleware => _middleware;


        public void AddMiddleware(ITrakHoundClientMiddleware middleware)
        {
            if (middleware != null)
            {
                _middleware.Add(middleware);
            }
        }

        public void AddMiddleware(ITrakHoundEntitiesClientMiddleware middleware)
        {
            if (middleware != null)
            {
                _middleware.Add(middleware);
            }
        }
    }
}
