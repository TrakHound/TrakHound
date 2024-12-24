// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Http;
using TrakHound.Sequences;

namespace TrakHound.Clients
{
    internal class TrakHoundHttpSystemSequencesClient : ITrakHoundSystemSequencesClient
    {
        private readonly TrakHoundHttpClient _baseClient;


        public string BaseUrl => _baseClient != null ? _baseClient.HttpBaseUrl : null;

        public string RouterId => _baseClient != null ? _baseClient.RouterId : null;


        public TrakHoundHttpSystemSequencesClient(TrakHoundHttpClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundSequenceResponse>> Read(IEnumerable<string> sequenceIds, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.SequencesPrefix);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponses = await RestRequest.Post<IEnumerable<TrakHoundHttpSequenceResponse>>(url, sequenceIds);
            if (!httpResponses.IsNullOrEmpty())
            {
                return httpResponses.ToResponses();
            }

            return default;
        }

        public async Task<TrakHoundSequenceResponse> Publish(string sequenceId, double value, long created = 0, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.SequencesPrefix);
            url = Url.Combine(url, "publish");
            url = Url.AddQueryParameter(url, "sequenceId", sequenceId);
            url = Url.AddQueryParameter(url, "value", value);
            url = Url.AddQueryParameter(url, "created", created);
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.Post<TrakHoundHttpSequenceResponse>(url, (string)null);
            return httpResponse.ToSequence();
        }

        public async Task<bool> Delete(IEnumerable<string> sequenceIds, string routerId = null)
        {
            var url = Url.Combine(BaseUrl, HttpConstants.SequencesPrefix);
            url = Url.Combine(url, "delete");
            url = Url.AddQueryParameter(url, "routerId", _baseClient.GetRouterId(routerId));

            var httpResponse = await RestRequest.PostResponse(url, sequenceIds);
            return httpResponse.StatusCode >= 200 && httpResponse.StatusCode < 300;
        }
    }
}
