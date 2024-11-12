// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Deployments
{
    public struct TrakHoundCreateDeploymentResult
    {
        public bool Success { get; }

        public TrakHoundDeploymentManifest Manifest { get; }

        public byte[] Content { get; }

        public string[] Messages { get; }

        public string[] Errors { get; }


        public TrakHoundCreateDeploymentResult(bool success, TrakHoundDeploymentManifest manifest, byte[] content, string[] messages = null, string[] errors = null)
        {
            Success = success;
            Manifest = manifest;
            Content = content;
            Messages = messages;
            Errors = errors;
        }
    }
}
