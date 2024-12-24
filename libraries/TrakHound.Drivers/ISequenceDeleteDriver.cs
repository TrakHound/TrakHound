// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers
{
    public interface ISequenceDeleteDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<bool>> Delete(IEnumerable<string> sequenceIds);
    }
}
