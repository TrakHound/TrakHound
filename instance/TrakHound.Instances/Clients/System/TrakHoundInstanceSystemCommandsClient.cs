// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Commands;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemCommandsClient : ITrakHoundSystemCommandsClient
    {
        private readonly TrakHoundInstanceClient _baseClient;


        public TrakHoundInstanceSystemCommandsClient(TrakHoundInstanceClient baseClient)
        {
            _baseClient = baseClient;
        }


        public async Task<TrakHoundCommandResponse> Run(string commandId, IReadOnlyDictionary<string, string> parameters = null, string routerId = null)
        {
            if (!string.IsNullOrEmpty(commandId))
            {
                var router = _baseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router.Commands.Run(commandId, parameters);
                    if (response.IsSuccess)
                    {
                        return response.Content.FirstOrDefault();
                    }
                }
            }

            return default;
        }
	}
}
