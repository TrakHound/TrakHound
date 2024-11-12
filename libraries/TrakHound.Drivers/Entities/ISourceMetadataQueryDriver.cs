// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Source Metadata Entities
    /// </summary>
    public interface ISourceMetadataQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Read TrakHound Source Metadata Entities for the specified Source UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>> Query(IEnumerable<string> sourceUuids);
    }
}
