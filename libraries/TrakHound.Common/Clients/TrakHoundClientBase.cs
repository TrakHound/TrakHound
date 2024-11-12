// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Clients
{
    public abstract class TrakHoundClientBase
    {
        protected ITrakHoundClient Client { get; set; }

        public IEnumerable<ITrakHoundClientMiddleware> Middleware
        {
            get
            {
                var middleware = new List<ITrakHoundClientMiddleware>();
                middleware.AddRange(Client.Entities.Middleware);
                middleware.AddRange(Client.System.Entities.Middleware);
                return middleware;
            }
        }



        public void AddMiddleware(ITrakHoundClientMiddleware middleware)
        {
            if (Client != null && Client.Entities != null)
            {
                // Add special Client (removes same middleware that can cause endless loop)
                middleware.Client = new TrakHoundMiddlewareClient(Client);

                // Add existing Middleware
                if (!Middleware.IsNullOrEmpty())
                {
                    foreach (var existingMiddleware in Middleware) middleware.Client.AddMiddleware(existingMiddleware);
                }

                // Add to Entities Middleware
                Client.Entities.AddMiddleware(middleware);

                if (typeof(ITrakHoundEntitiesClientMiddleware).IsAssignableFrom(middleware.GetType()))
                {
                    // Add to System Entities Middleware
                    Client.System.Entities.AddMiddleware((ITrakHoundEntitiesClientMiddleware)middleware);
                }
            }
        }

        public void AddMiddleware(ITrakHoundEntitiesClientMiddleware middleware)
        {
            if (Client != null && Client.Entities != null && Client.System != null && Client.System.Entities != null)
            {
                // Add special Client (removes same middleware that can cause endless loop)
                middleware.Client = new TrakHoundMiddlewareClient(Client);

                // Add existing Middleware
                if (!Middleware.IsNullOrEmpty())
                {
                    foreach (var existingMiddleware in Middleware) middleware.Client.AddMiddleware(existingMiddleware);
                }

                // Add to Entities Middleware
                Client.Entities.AddMiddleware(middleware);

                // Add to System Entities Middleware
                Client.System.Entities.AddMiddleware(middleware);
            }
        }
    }
}
