// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Buffers
{
    /// <summary>
    /// A Configuration used to configure a Buffer
    /// </summary>
    public class TrakHoundBufferConfiguration
    {
        /// <summary>
        /// The interval in milliseconds that asyncrhronous operations are handled
        /// </summary>
        public int Interval { get; set; }

        public int RetryInterval { get; set; }

        public int MaxItemsPerInterval { get; set; }

        public int QueuedItemLimit { get; set; }

        /// <summary>
        /// Determines whether the "Store and Forward" File Buffer is enabled
        /// </summary>
        public bool FileBufferEnabled { get; set; }

        /// <summary>
        /// Determines whether new Items are forced to the File Buffer instead of the Working Queue
        /// </summary>
        public bool FileBufferForceEnabled { get; set; }

        /// <summary>
        /// The size (in bytes) of the Maximum Page Size to use for the File Buffer
        /// </summary>
        public int FileBufferPageSize { get; set; }

        /// <summary>
        /// The interval at which the File Buffer is read
        /// </summary>
        public int FileBufferReadInterval { get; set; }

        /// <summary>
        /// The interval at which the File Buffer is written
        /// </summary>
        public int FileBufferWriteInterval { get; set; }

        /// <summary>
        /// Determines whether messages are verified after successfully sending
        /// </summary>
        public bool AcknowledgeSent { get; set; }


        public TrakHoundBufferConfiguration()
        {
            Interval = -1;
            RetryInterval = -1;
            MaxItemsPerInterval = 0;
            QueuedItemLimit = 0;
            FileBufferEnabled = false;
            FileBufferForceEnabled = false;
            FileBufferPageSize = 0;
            FileBufferReadInterval = -1;
            FileBufferWriteInterval = -1;
            AcknowledgeSent = false;
        }
    }
}
