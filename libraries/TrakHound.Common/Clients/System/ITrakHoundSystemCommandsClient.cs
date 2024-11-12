// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Commands;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemCommandsClient
    {
        Task<TrakHoundCommandResponse> Run(string commandId, IReadOnlyDictionary<string, string> parameters = null, string routerId = null);
    }
}
