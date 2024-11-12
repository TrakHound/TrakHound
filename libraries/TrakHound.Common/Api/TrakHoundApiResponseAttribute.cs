// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Api
{
    public class TrakHoundApiResponseAttribute : Attribute
    {
        public int StatusCode { get; set; }

        public Type ReturnType { get; set; }


        public TrakHoundApiResponseAttribute(int statusCode, Type returnType)
        {
            StatusCode = statusCode;
            ReturnType = returnType;
        }
    }
}
