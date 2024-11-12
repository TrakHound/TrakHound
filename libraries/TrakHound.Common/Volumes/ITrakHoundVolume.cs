// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace TrakHound.Volumes
{
    public interface ITrakHoundVolume : IDisposable
    {
        string Id { get; }


        Task<string[]> ListFiles(string path = null, bool includeSubdirectories = false);


        Task<string> ReadString(string path);

        Task<T> ReadJson<T>(string path);

        Task<byte[]> ReadBytes(string path);



        Task<bool> WriteString(string path, string content);

        Task<bool> WriteJson(string path, object content);

        Task<bool> WriteBytes(string path, byte[] content);


        Task<bool> Delete(string path);


        ITrakHoundVolumeListener CreateListener(string path, string filter = null);
    }
}
