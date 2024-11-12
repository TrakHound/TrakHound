// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundPublishResult<TResult>
    {
        public TrakHoundPublishResultType Type { get; set; }

        public TResult Result { get; set; }


        public TrakHoundPublishResult(TrakHoundPublishResultType type, TResult result)
        {
            Type = type;
            Result = result;
        }
    }
}
