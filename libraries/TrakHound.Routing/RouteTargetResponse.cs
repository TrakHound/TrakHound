// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Routing
{
    struct RouteTargetResponse<T>
    {
        private IEnumerable<TrakHoundResult<T>> _results;
        private bool _hasResults;

        public IEnumerable<TrakHoundResult<T>> Results => _results;

        public bool HasResults => _hasResults;

        public long Duration { get; set; }

        public IEnumerable<RouteRedirectOption<T>> Options { get; set; }        
        

        public IEnumerable<TrakHoundResult<T>> SuccessResults => GetResults(TrakHoundResultType.Ok);

        public IEnumerable<TrakHoundResult<T>> EmptyResults => GetResults(TrakHoundResultType.Empty);

        public IEnumerable<TrakHoundResult<T>> NotFoundResults => GetResults(TrakHoundResultType.NotFound);

        public IEnumerable<TrakHoundResult<T>> InternalErrorResults => GetResults(TrakHoundResultType.InternalError);


        public bool IsSuccess => !SuccessResults.IsNullOrEmpty();

        public bool IsEmpty => !EmptyResults.IsNullOrEmpty();

        public bool IsNotFound => !NotFoundResults.IsNullOrEmpty();

        public bool IsInternalError => !InternalErrorResults.IsNullOrEmpty();


        public IEnumerable<T> Content => SuccessResults?.Select(o => o.Content);


        public RouteTargetResponse(
            IEnumerable<TrakHoundResult<T>> results,
            long duration = 0, 
            IEnumerable<RouteRedirectOption<T>> options = null
            )
        {
            if (!results.IsNullOrEmpty())
            {
                _results = results;
                _hasResults = true;
            }
            else
            {
                _results = null;
                _hasResults = false;
            }

            Duration = duration;
            Options = options;
        }


        private IEnumerable<TrakHoundResult<T>> GetResults(TrakHoundResultType type)
        {
            var results = new List<TrakHoundResult<T>>();

            if (_hasResults && !_results.IsNullOrEmpty())
            {
                var x = _results?.Where(o => o.Type == type);
                if (!x.IsNullOrEmpty()) results.AddRange(x);
            }

            return results;
        }
    }
}
