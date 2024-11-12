// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace TrakHound.Clients
{
    public interface ITrakHoundClientMiddleware
    {
        string Id { get; }

        ITrakHoundClient Client { get; set; }


        Task OnBeforePublish();

        Task OnAfterPublish();
    }
}
