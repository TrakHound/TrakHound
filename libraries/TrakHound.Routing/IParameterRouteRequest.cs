// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Routing
{
    interface IParameterRouteRequest
    {
        string Id { get; }

        string Name { get; }

        IEnumerable<RouteQueryParameter> Parameters { get; }

        IEnumerable<RouteQuery> Queries { get; }


        void AddQuery(string query);

        void AddQuery(RouteQuery query);

        void AddQueries(IEnumerable<string> queries);

        void AddQueries(IEnumerable<RouteQuery> queries);


        string GetParameter(string key);

        IEnumerable<string> GetParameters(string key);


        T GetParameter<T>(string key);

        IEnumerable<T> GetParameters<T>(string key);
    }
}
