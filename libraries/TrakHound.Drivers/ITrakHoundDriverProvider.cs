// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Drivers
{
    public interface ITrakHoundDriverProvider : IDisposable
    {
        IEnumerable<ITrakHoundDriver> Drivers { get; }

        EventHandler<ITrakHoundDriver> DriverAdded { get; set; }

        EventHandler<string> DriverRemoved { get; set; }

        EventHandler<Exception> DriverLoadError { get; set; }


        IEnumerable<TrakHoundDriverInformation> GetInformation();

        TrakHoundDriverInformation GetInformation(string configurationId);


        TDriver GetDriver<TDriver>(string configurationId) where TDriver : ITrakHoundDriver;
    }
}
