// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Entities;

namespace TrakHound.Routing.Routers
{
    public class TrakHoundObjectNotifySubscription
	{
		public string Id { get; set; }

		public string Query { get; set; }

		public TrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>> Consumer { get; set; }


		public TrakHoundObjectNotifySubscription() 
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
