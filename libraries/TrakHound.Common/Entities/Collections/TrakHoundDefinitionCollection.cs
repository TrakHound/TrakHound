// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundDefinitionCollection
    {
        private readonly ListDictionary<string, string> _definitionParents = new ListDictionary<string, string>(); // ParentUuid => Uuid[]


        public IEnumerable<ITrakHoundDefinitionEntity> QueryDefinitionsByChildUuid(string childUuid)
        {
            if (!string.IsNullOrEmpty(childUuid))
            {
                var childDefinition = GetDefinition(childUuid);
                if (childDefinition != null)
                {
                    var definitions = new List<ITrakHoundDefinitionEntity>();
                    definitions.Add(childDefinition);

                    if (!string.IsNullOrEmpty(childDefinition.ParentUuid))
                    {
                        var parentDefinitions = QueryDefinitionsByChildUuid(childDefinition.ParentUuid);
                        if (!parentDefinitions.IsNullOrEmpty()) definitions.AddRange(parentDefinitions);
                    }

                    return definitions;
                }
            }

            return null;
        }


        public void OnAddDefinition(ITrakHoundDefinitionEntity entity) { }

        public partial void OnClear() { }
    }
}
