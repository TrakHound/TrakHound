// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to Query TrakHound Vocabulary Sets
    /// </summary>
    public interface IObjectVocabularySetQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectVocabularySetEntity>> QueryByObjectUuid(IEnumerable<string> objectUuid, long skip = 0, long take = 1000, SortOrder sortOrder = SortOrder.Ascending);
    }
}
