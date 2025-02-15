// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public struct TrakHoundEntityResponse<TResult>
    {
        public bool Success => StatusCode >= 200 && StatusCode < 400;

        public TResult Result { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; }
    }
}
