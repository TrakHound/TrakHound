// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Volumes
{
    public enum TrakHoundVolumeOnChangeType
    {
        Create,
        Modify,
        Delete
    }

    public delegate void TrakHoundVolumeOnChangeHandler(string path, TrakHoundVolumeOnChangeType changeType);

    public interface ITrakHoundVolumeListener : IDisposable
    {
        event TrakHoundVolumeOnChangeHandler Changed;

        void Start();

        void Stop();
    }
}
