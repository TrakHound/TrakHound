// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Modules
{
    public interface ITrakHoundModuleProvider
    {
        ITrakHoundModuleManager Get<TModule>(string packageCategory, TrakHoundModuleContext context = null, bool loadByDependencies = true);

        ITrakHoundModuleManager Get<TModule>(IEnumerable<string> packageCategories, TrakHoundModuleContext context = null, bool loadByDependencies = true);
    }
}
