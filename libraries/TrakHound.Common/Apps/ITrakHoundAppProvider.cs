// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Apps
{
    public interface ITrakHoundAppProvider
    {
        IEnumerable<string> PackageStylesheets { get; }

        IEnumerable<string> PackageScripts { get; }

        public event TrakHoundAppLogHandler AppLogUpdated;


        bool IsRouteValid(string route);


        IEnumerable<TrakHoundAppInformation> GetInformation();

        TrakHoundAppInformation GetInformation(string appId);

        TrakHoundAppPageInformation GetPageInformation(string route);


        IEnumerable<string> GetAppStylesheets(string appId);

        IEnumerable<string> GetAppScripts(string appId);
    }
}
