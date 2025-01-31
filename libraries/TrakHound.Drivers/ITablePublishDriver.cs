using System.Threading.Tasks;
using TrakHound.Tables;

namespace TrakHound.Drivers
{
    public interface ITablePublishDriver
    {
        Task<bool> CreateTable(string tableUuid, TrakHoundTableColumn[] columns);

        Task<bool> Insert(string tableUuid, TrakHoundTableRow[] rows);
    }
}
