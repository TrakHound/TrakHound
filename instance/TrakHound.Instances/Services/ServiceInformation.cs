// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Serialization;

namespace TrakHound.Services
{
    [TrakHoundObject(BasePath = ".Instances/{instanceId}/Services/{packageId}/Engines")]
	internal class ServiceInformation
	{
        [TrakHoundPathParameter]
        public string InstanceId { get; set; }

        [TrakHoundName]
		public string Id { get; set; }

		[TrakHoundString]
		public string Name { get; set; }

		[TrakHoundString]
		public string Description { get; set; }

		[TrakHoundString]
        [TrakHoundPathParameter]
        public string PackageId { get; set; }

        [TrakHoundString]
        public string PackageVersion { get; set; }

        [TrakHoundString]
        public string PackageUuid { get; set; }

        [TrakHoundString]
        public string PackageHash { get; set; }

        [TrakHoundTimestamp]
        public DateTime? PackageBuildDate { get; set; }

        [TrakHoundState]
        public string Status { get; set; }

        [TrakHoundTimestamp]
        public DateTime LastUpdated { get; set; }
	}
}
