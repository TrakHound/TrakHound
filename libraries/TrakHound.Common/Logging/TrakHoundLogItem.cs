// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Logging
{
    public struct TrakHoundLogItem
    {
        public string Sender { get; set; }

        public TrakHoundLogLevel LogLevel { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public long Timestamp { get; set; }


        public TrakHoundLogItem(string sender, TrakHoundLogLevel logLevel, string message, string code = null, long timestamp = 0)
        {
            Sender = sender;
            LogLevel = logLevel;
            Message = message;
            Code = code;
            Timestamp = timestamp > 0 ? timestamp : UnixDateTime.Now;
        }
    }
}
