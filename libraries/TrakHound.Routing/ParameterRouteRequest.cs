// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Routing
{
    class ParameterRouteRequest : IParameterRouteRequest
    {
        protected readonly List<RouteQuery> _queries = new List<RouteQuery>();
        private readonly object _lock = new object();


        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<RouteQueryParameter> Parameters { get; set; }

        public IEnumerable<RouteQuery> Queries => _queries;


        public ParameterRouteRequest()
        {
            Id = null;
            Name = null;
            Parameters = null;
        }

        public ParameterRouteRequest(string name, string id, string query = null, IEnumerable<RouteQueryParameter> parameters = null)
        {
            Name = name;
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            Parameters = parameters;

            AddQuery(query);
        }

        public ParameterRouteRequest(string name, string id, IEnumerable<string> queries, IEnumerable<RouteQueryParameter> parameters = null)
        {
            Name = name;
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            Parameters = parameters;

            AddQueries(queries);
        }

        public ParameterRouteRequest(string name, string id, IEnumerable<RouteQuery> queries, IEnumerable<RouteQueryParameter> parameters = null)
        {
            Name = name;
            Id = !string.IsNullOrEmpty(id) ? id : Guid.NewGuid().ToString();

            Parameters = parameters;

            if (queries != null)
            {
                lock (_lock) _queries.AddRange(queries);
            }
        }


        public void AddQuery(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                lock (_lock) _queries.Add(new RouteQuery(query));
            }
        }

        public void AddQuery(RouteQuery query)
        {
            lock (_lock) _queries.Add(query);
        }

        public void AddQueries(IEnumerable<string> queries)
        {
            if (!queries.IsNullOrEmpty())
            {
                var l = new List<RouteQuery>();
                foreach (var query in queries)
                {
                    l.Add(new RouteQuery(query));
                }
                _queries.AddRange(l);
            }
        }

        public void AddQueries(IEnumerable<RouteQuery> queries)
        {
            if (!queries.IsNullOrEmpty())
            {
                _queries.AddRange(queries);
            }
        }


        public string GetParameter(string key)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var parameter = Parameters.FirstOrDefault(o => o.Key == key);
                if (parameter.Value != null)
                {
                    return parameter.Value;
                }
            }

            return null;
        }

        public IEnumerable<string> GetParameters(string key)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var parameters = Parameters.Where(o => o.Key == key);
                if (!parameters.IsNullOrEmpty())
                {
                    var values = new List<string>();

                    foreach (var parameter in parameters)
                    {
                        if (parameter.Value != null)
                        {
                            values.Add(parameter.Value);
                        }
                    }

                    return values;
                }
            }

            return null;
        }


        public T GetParameter<T>(string key)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var parameter = Parameters.FirstOrDefault(o => o.Key == key);
                if (parameter.Value != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(parameter.Value, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }

        public IEnumerable<T> GetParameters<T>(string key)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                var parameters = Parameters.Where(o => o.Key == key);
                if (!parameters.IsNullOrEmpty())
                {
                    var values = new List<T>();

                    foreach (var parameter in parameters)
                    {
                        if (parameter.Value != null)
                        {
                            try
                            {
                                var value = (T)Convert.ChangeType(parameter.Value, typeof(T));
                                values.Add(value);
                            }
                            catch { }
                        }
                    }

                    return values;
                }
            }

            return null;
        }
    }
}
