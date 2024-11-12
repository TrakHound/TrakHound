// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
	public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundHash>> GetHashes(string objectPath, string routerId = null);
        Task<IEnumerable<TrakHoundHash>> GetHashes(IEnumerable<string> objectPaths, string routerId = null);


        Task<Dictionary<string, Dictionary<string, string>>> GetHashValues(string objectPath, string routerId = null);
        Task<Dictionary<string, Dictionary<string, string>>> GetHashValues(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundHash>>> SubscribeHashes(string objectPath, string routerId = null);


        Task<bool> PublishHash(string objectPath, string key, string value, bool async = false, string routerId = null);

        Task<bool> PublishHash(string objectPath, Dictionary<string, string> values, bool async = false, string routerId = null);

        Task<bool> PublishHashes(IEnumerable<TrakHoundHashEntry> entries, bool async = false, string routerId = null);


        Task<bool> DeleteHash(string objectPath, bool async = false, string routerId = null);

        Task<bool> DeleteHash(string objectPath, string key, bool async = false, string routerId = null);
    }
}
