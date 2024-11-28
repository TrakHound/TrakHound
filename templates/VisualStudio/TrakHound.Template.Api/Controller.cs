using System.Threading.Tasks;
using TrakHound;
using TrakHound.Api;

namespace $projectname$
{
    public class Controller : TrakHoundApiController
    {
        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> Query()
        {
            return Ok("$projectname$");
        }
    }
}
