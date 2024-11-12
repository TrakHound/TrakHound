// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound
{
    public struct TrakHoundResult<T>
    {
        public string Source { get; set; }

        public TrakHoundResultType Type { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }

        public T Content { get; set; }


        public TrakHoundResult(string source, string request, TrakHoundResultType type, T content = default, string response = null)
        {
            Source = source;
            Type = type;
            Request = request;
            Response = response;
            Content = content;
        }


        public static TrakHoundResult<T> Ok(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.Ok, content);

        public static IEnumerable<TrakHoundResult<T>> OkArray(string request, string source, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();
            
            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.Ok, content));
                }
            }

            return results;
        }


        public static TrakHoundResult<T> NotFound(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.NotFound, content);

        public static IEnumerable<TrakHoundResult<T>> NotFoundArray(string request, string source, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();

            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.NotFound, content));
                }
            }

            return results;
        }


        public static TrakHoundResult<T> NotAvailable(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.NotAvailable, content);

        public static IEnumerable<TrakHoundResult<T>> NotAvailableArray(string source, string request, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();

            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.NotAvailable, content));
                }
            }

            return results;
        }


        public static TrakHoundResult<T> RouteNotConfigured(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.RouteNotConfigured, content);

        public static IEnumerable<TrakHoundResult<T>> RouteNotConfiguredArray(string source, string request, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();

            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.RouteNotConfigured, content));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.RouteNotConfigured));
            }

            return results;
        }


        public static TrakHoundResult<T> BadRequest(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.BadRequest, content);

        public static IEnumerable<TrakHoundResult<T>> BadRequestArray(string source, string request, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();

            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.BadRequest, content));
                }
            }

            return results;
        }


        public static TrakHoundResult<T> Timeout(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.Timeout, content);

        public static IEnumerable<TrakHoundResult<T>> TimeoutArray(string source, string request, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();

            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.Timeout, content));
                }
            }

            return results;
        }


        public static TrakHoundResult<T> InternalError(string source, string request = null, T content = default) => new TrakHoundResult<T>(source, request, TrakHoundResultType.InternalError, content);

        public static IEnumerable<TrakHoundResult<T>> InternalErrorArray(string source, string request, IEnumerable<T> contents = default)
        {
            var results = new List<TrakHoundResult<T>>();

            if (!contents.IsNullOrEmpty())
            {
                foreach (var content in contents)
                {
                    results.Add(new TrakHoundResult<T>(source, request, TrakHoundResultType.InternalError, content));
                }
            }

            return results;
        }
    }
}
