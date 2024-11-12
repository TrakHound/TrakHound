// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Definition Metadata Entities
    /// </summary>
    public interface IDefinitionMetadataQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Read TrakHound Definition Metadata Entities for the specified Definition UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundDefinitionMetadataEntity>> Query(IEnumerable<string> definitionUuids);
    }
}
