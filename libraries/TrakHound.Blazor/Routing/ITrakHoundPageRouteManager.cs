// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Apps;

namespace TrakHound.Blazor.Routing
{
    public interface ITrakHoundPageRouteManager
    {
        IEnumerable<Route> Routes { get; }

        event EventHandler<string> AppUpdated; // AppId as argument


        void AddApp(ITrakHoundAppConfiguration configuration);

        void RemoveApp(string configurationId);


        void ClearPages();

        void Initialise();

        MatchResult Match(string url);
    }
}
