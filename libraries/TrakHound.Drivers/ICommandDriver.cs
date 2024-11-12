// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Commands;

namespace TrakHound.Drivers
{
    public interface ICommandDriver : ITrakHoundDriver 
    {
        Task<TrakHoundResponse<TrakHoundCommandResponse>> Run(string commandId, IReadOnlyDictionary<string, string> parameters = null);
    }
}
