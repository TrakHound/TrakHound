// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    internal static class TrakHoundMessageRoutes
    {
        public const string BrokersQuery = "Messages.Brokers.Read.Query";
        public const string SendersQuery = "Messages.Senders.Read.Query";

        public const string Subscribe = "Messages.Read.Subscribe";
        public const string Publish = "Messages.Write.Publish";


        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            { typeof(IMessageBrokerDriver), BrokersQuery },
            { typeof(IMessageSenderDriver), SendersQuery },

            { typeof(IMessageSubscribeDriver), Subscribe },
            { typeof(IMessagePublishDriver), Publish }
        };
    }
}
