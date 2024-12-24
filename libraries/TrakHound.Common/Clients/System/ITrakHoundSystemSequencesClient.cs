// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Sequences;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemSequencesClient
    {
        Task<IEnumerable<TrakHoundSequenceResponse>> Read(IEnumerable<string> sequenceIds, string routerId = null);

        Task<TrakHoundSequenceResponse> Publish(string sequenceId, double value, long created = 0, string routerId = null);

        Task<bool> Delete(IEnumerable<string> sequenceIds, string routerId = null);
    }
}
