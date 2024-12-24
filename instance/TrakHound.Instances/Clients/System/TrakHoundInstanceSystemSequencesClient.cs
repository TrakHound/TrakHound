// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Sequences;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemSequencesClient : ITrakHoundSystemSequencesClient
    {
        private readonly TrakHoundInstanceClient _baseClient;


        public TrakHoundInstanceSystemSequencesClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<IEnumerable<TrakHoundSequenceResponse>> Read(IEnumerable<string> sequenceIds, string routerId = null)
        {
            if (!sequenceIds.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Sequences.Read(sequenceIds);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return default;
        }

        public async Task<TrakHoundSequenceResponse> Publish(string sequenceId, double value, long created, string routerId = null)
        {
            if (!string.IsNullOrEmpty(sequenceId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Sequences.Publish(sequenceId, value, created);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }

        public async Task<bool> Delete(IEnumerable<string> sequenceIds, string routerId = null)
        {
            if (!sequenceIds.IsNullOrEmpty())
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Sequences.Delete(sequenceIds);
                    if (response.IsSuccess)
                    {
                        return response.Content.All(o => o);
                    }
                }
            }

            return default;
        }
    }
}
