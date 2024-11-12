// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundNumber> GetNumber(string objectPath, string routerId = null);

        Task<string> GetNumberValue(string objectPath, string routerId = null);

        Task<T> GetNumberValue<T>(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundNumber>> GetNumbers(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundNumber>> GetNumbers(IEnumerable<string> objectPaths, string routerId = null);

        Task<Dictionary<string, string>> GetNumberValues(string objectPath, string routerId = null);

        Task<Dictionary<string, string>> GetNumberValues(IEnumerable<string> objectPaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundNumber>>> SubscribeNumbers(string objectPath, string routerId = null);


        Task<bool> PublishNumber(string objectPath, string value, TrakHoundNumberDataType dataType = TrakHoundNumberDataType.Float, bool async = false, string routerId = null);
        Task<bool> PublishNumber(string objectPath, byte value, bool async = false, string routerId = null);
        Task<bool> PublishNumber(string objectPath, short value, bool async = false, string routerId = null);
        Task<bool> PublishNumber(string objectPath, int value, bool async = false, string routerId = null);
        Task<bool> PublishNumber(string objectPath, long value, bool async = false, string routerId = null);
        Task<bool> PublishNumber(string objectPath, double value, bool async = false, string routerId = null);
        Task<bool> PublishNumber(string objectPath, decimal value, bool async = false, string routerId = null);
    }
}
