// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundTimestamp> GetTimestamp(string objectPath, string routerId = null);

        Task<DateTime?> GetTimestampValue(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundTimestamp>> GetTimestamps(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundTimestamp>> GetTimestamps(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundTimestamp>>> SubscribeTimestamps(string objectPath, string routerId = null);


        Task<bool> PublishTimestamp(string objectPath, long value, bool async = false, string routerId = null);

        Task<bool> PublishTimestamp(string objectPath, DateTime value, bool async = false, string routerId = null);

        Task<bool> PublishTimestamp(string objectPath, string value, bool async = false, string routerId = null);
    }
}
