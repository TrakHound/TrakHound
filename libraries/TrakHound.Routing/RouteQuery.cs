// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Routing
{
    struct RouteQuery
    {
        public string Query { get; set; }

        public IEnumerable<RouteQueryParameter> Parameters { get; set; }


        public RouteQuery(string query, IEnumerable<RouteQueryParameter> parameters = null)
        {
            Query = query;
            Parameters = parameters;
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
