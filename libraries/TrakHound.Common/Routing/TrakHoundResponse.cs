// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound
{
    public struct TrakHoundResponse<T>
    {
        private readonly IEnumerable<TrakHoundResult<T>> _results;
        private readonly IEnumerable<TrakHoundResult<T>> _successResults;
        private readonly IEnumerable<TrakHoundResult<T>> _notFoundResults;
        private readonly IEnumerable<TrakHoundResult<T>> _internalErrorResults;
        private readonly bool _hasResults;
        private readonly bool _hasSuccessResults;
        private readonly bool _hasNotFoundResults;
        private readonly bool _hasInternalErrorResults;


        public IEnumerable<T> Content => _successResults?.Select(o => o.Content);

        public IEnumerable<TrakHoundResult<T>> Results => _results;

        public bool HasResults => _hasResults;

        public long Duration { get; set; }


        public IEnumerable<TrakHoundResult<T>> SuccessResults => _successResults;

        public IEnumerable<TrakHoundResult<T>> NotFoundResults => _notFoundResults;

        public IEnumerable<TrakHoundResult<T>> InternalErrorResults => _internalErrorResults;

        public bool IsSuccess => _hasSuccessResults;

        public bool IsNotFound => _hasNotFoundResults;

        public bool IsInternalError => _hasInternalErrorResults;


        public TrakHoundResponse(IEnumerable<TrakHoundResult<T>> results, long duration = 0)
        {
            if (!results.IsNullOrEmpty())
            {
                _results = results;
                _successResults = results?.Where(o => o.Type == TrakHoundResultType.Ok).ToList();
                _notFoundResults = results?.Where(o => o.Type == TrakHoundResultType.NotFound).ToList();
                _internalErrorResults = results?.Where(o => o.Type == TrakHoundResultType.InternalError).ToList();
                _hasSuccessResults = !_successResults.IsNullOrEmpty();
                _hasNotFoundResults = !_notFoundResults.IsNullOrEmpty();
                _hasInternalErrorResults = !_internalErrorResults.IsNullOrEmpty();
                _hasResults = true;
            }
            else
            {
                _results = null;
                _successResults = null;
                _notFoundResults = null;
                _internalErrorResults = null;
                _hasResults = false;
                _hasSuccessResults = false;
                _hasNotFoundResults = false;
                _hasInternalErrorResults = false;
            }

            Duration = duration;
        }


        public static TrakHoundResponse<T> Ok(string source, string request, T content, long duration = 0)
        {
            var a = new List<TrakHoundResult<T>> { TrakHoundResult<T>.Ok(source, request, content) };
            return new TrakHoundResponse<T>(a, duration);
        }

        public static TrakHoundResponse<T> NotFound(string source, string request, long duration = 0)
        {
            return new TrakHoundResponse<T>(TrakHoundResult<T>.NotFoundArray(source, request), duration);
        }

        public static TrakHoundResponse<T> NotAvailable(string source, string request, long duration = 0)
        {
            return new TrakHoundResponse<T>(TrakHoundResult<T>.NotAvailableArray(source, request), duration);
        }

        public static TrakHoundResponse<T> RouteNotConfigured(string source, string request, long duration = 0)
        {
            return new TrakHoundResponse<T>(TrakHoundResult<T>.RouteNotConfiguredArray(source, request), duration);
        }

        public static TrakHoundResponse<T> Timeout(string source, string request, long duration = 0)
        {
            return new TrakHoundResponse<T>(TrakHoundResult<T>.TimeoutArray(source, request), duration);
        }

        public static TrakHoundResponse<T> InternalError(string source, string request = null, long duration = 0)
        {
            return new TrakHoundResponse<T>(TrakHoundResult<T>.InternalErrorArray(source, request), duration);
        }
    }
}
