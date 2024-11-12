// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;

namespace TrakHound.Memory
{
    public abstract class MemoryDriver<T> : TrakHoundDriver
    {
        private readonly string _instanceId;


        public override bool IsAvailable => true;


        public MemoryDriver(ITrakHoundDriverConfiguration configuration) : base(configuration)
        {
            _instanceId = Guid.NewGuid().ToString();
        }
    }
}
