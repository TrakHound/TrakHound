using System.Threading.Tasks;
using TrakHound.Tables;

namespace TrakHound.Drivers
{
    public interface ITimeSeriesPublishDriver
    {
        Task<bool> CreateCollection(string collectionUuid, TrakHoundTableColumn[] columns);

        Task<bool> Insert(string collectionUuid, TrakHoundTableRow[] rows);
    }
}
