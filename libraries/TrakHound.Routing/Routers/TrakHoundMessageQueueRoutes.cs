// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    internal static class TrakHoundMessageQueueRoutes
    {
        public const string Subscribe = "MessageQueues.Read.Subscribe";
        public const string Pull = "MessageQueues.Read.Pull";
        public const string Publish = "MessageQueues.Write.Publish";


        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            { typeof(IMessageQueueSubscribeDriver), Subscribe },
            { typeof(IMessageQueuePullDriver), Pull },
            { typeof(IMessageQueuePublishDriver), Publish }
        };
    }
}
