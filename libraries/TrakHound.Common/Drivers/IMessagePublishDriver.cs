// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers
{
    public interface IMessagePublishDriver : ITrakHoundDriver
    {
        Task<TrakHoundEntityResponse<bool>> Publish(string brokerId, string topic, Stream content, bool retain, int qos);
    }
}
