using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionObjectObservationClient
    {
        public async Task<IEnumerable<ITrakHoundObjectObservationEntity>> QueryByRange(
            IEnumerable<TrakHoundRangeQuery> queries,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null)
        {
            return default;
        }
    }
}
