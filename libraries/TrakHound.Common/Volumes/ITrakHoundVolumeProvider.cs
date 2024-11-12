// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace TrakHound.Volumes
{
    public interface ITrakHoundVolumeProvider
    {
        ITrakHoundVolume GetVolume(string id);

        bool DeleteVolume(string id);
    }
}
