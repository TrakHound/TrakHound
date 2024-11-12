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
    public partial class TrakHoundSourceMetadataController : TrakHoundSourceMetadataControllerBase
    {
        public TrakHoundSourceMetadataController(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }



        protected async override Task<IActionResult> OnQueryBySourceUuid(
            ITrakHoundClient client,
            string sourceUuid,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Sources.Metadata.QueryBySourceUuid(
            sourceUuid);

            if (!IsNullOrEmpty(result))
            {
                return ProcessJsonContentResponse(result, indentOutput);
            }
            else
            {
                return NotFound();
            }
        }



        protected async override Task<IActionResult> OnQueryBySourceUuid(
            ITrakHoundClient client,
            IEnumerable<string> sourceUuids,
            bool indentOutput = false
        )
        {
            var result = await client.System.Entities.Sources.Metadata.QueryBySourceUuid(
            sourceUuids);

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
