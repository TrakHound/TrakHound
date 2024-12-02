using System.Threading.Tasks;
using TrakHound;
using TrakHound.Api;

public class Controller : TrakHoundApiController
{
    [TrakHoundApiQuery]
    public async Task<TrakHoundApiResponse> Query([FromQuery] string name)
    {
        return Ok("UserPackageId : Query : " + System.DateTime.Now.ToString("o") + " : " + name);
    }

    [TrakHoundApiQuery("{name}")]
    public async Task<TrakHoundApiResponse> QueryWithRouteParameter([FromRoute] string name)
    {
        return Ok("UserPackageId : QueryWithRouteParameter : " + System.DateTime.Now.ToString("o") + " : " + name);
    }
}
