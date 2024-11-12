// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Logging;

namespace TrakHound.Services
{
    public delegate void TrakHoundServiceLogHandler(string serviceId, TrakHoundLogItem item);
}
