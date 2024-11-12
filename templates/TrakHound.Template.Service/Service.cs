using TrakHound.Clients;
using TrakHound.Services;
using TrakHound.Volumes;

namespace $projectname$
{
    public class Service : TrakHoundService
    {
        public Service(ITrakHoundServiceConfiguration configuration, ITrakHoundClient client, ITrakHoundVolume volume) : base(configuration, client, volume) { }


        protected override void OnStart()
        {
            Log(TrakHoundLogLevel.Information, "$projectname$ Service STARTED.");
        }

        protected override void OnStop()
        {
            Log(TrakHoundLogLevel.Information, "$projectname$ Service STOPPED.");
        }
    }
}
