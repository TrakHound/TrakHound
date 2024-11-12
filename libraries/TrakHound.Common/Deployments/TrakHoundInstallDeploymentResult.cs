// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Deployments
{
    public struct TrakHoundInstallDeploymentResult
    {
        public bool Success { get; }

        public string[] Messages { get; }

        public string[] Errors { get; }


        public TrakHoundInstallDeploymentResult(bool success, string[] messages = null, string[] errors = null)
        {
            Success = success;
            Messages = messages;
            Errors = errors;
        }
    }
}
