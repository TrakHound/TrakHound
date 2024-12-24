// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Sequences;

namespace TrakHound.Drivers
{
    public interface ISequenceReadDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<TrakHoundSequenceResponse>> Read(IEnumerable<string> sequenceIds);
    }
}
