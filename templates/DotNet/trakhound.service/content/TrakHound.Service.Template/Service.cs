using TrakHound;
using TrakHound.Clients;
using TrakHound.Services;
using TrakHound.Volumes;

public class Service : TrakHoundService
{
    public Service(ITrakHoundServiceConfiguration configuration, ITrakHoundClient client, ITrakHoundVolume volume) : base(configuration, client, volume) { }


    protected override void OnStart()
    {
        Log(TrakHoundLogLevel.Information, "UserPackageId Service STARTED.");
    }

    protected override void OnStop()
    {
        Log(TrakHoundLogLevel.Information, "UserPackageId Service STOPPED.");
    }
}
