// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Entities
{
	[Flags]
	public enum TrakHoundEntityNotificationType
	{
		Created = 1,

		Changed = 2,

		Deleted = 4,

		ComponentChanged = 5,

		All = Created | Changed | Deleted | ComponentChanged
	}
}
