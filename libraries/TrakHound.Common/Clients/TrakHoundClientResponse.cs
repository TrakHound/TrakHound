// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Clients
{
    public struct TrakHoundClientResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }
    }

    public struct TrakHoundClientResponse<TResult>
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public TResult Result { get; set; }
    }
}
