// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Instances;

namespace TrakHound.Http.Entities
{
    public partial class TrakHoundDefinitionMetadataController : TrakHoundDefinitionMetadataControllerBase
    {
        public TrakHoundDefinitionMetadataController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryByDefinitionUuid(
            ITrakHoundClient client,
            string definitionUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(
            definitionUuid);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryByDefinitionUuid(
            ITrakHoundClient client,
            IEnumerable<string> definitionUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Definitions.Metadata.QueryByDefinitionUuid(
            definitionUuids);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
