// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Vocabulary Entities.
    /// </summary>
    public interface IObjectVocabularyQueryDriver : ITrakHoundDriver
    {
        /// <summary>
        /// Query the Vocabulary Entities with the specified Object UUID's
        /// </summary>
        Task<TrakHoundResponse<ITrakHoundObjectVocabularyEntity>> Query(IEnumerable<string> objectUuids);
    }
}
