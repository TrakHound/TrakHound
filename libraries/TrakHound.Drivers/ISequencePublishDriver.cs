// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;
using TrakHound.Sequences;

namespace TrakHound.Drivers
{
    public interface ISequencePublishDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<TrakHoundSequenceResponse>> Publish(string sequenceId, double value, long created);
    }
}
