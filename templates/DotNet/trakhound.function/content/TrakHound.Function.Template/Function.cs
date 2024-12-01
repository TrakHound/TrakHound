using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound;
using TrakHound.Clients;
using TrakHound.Functions;
using TrakHound.Volumes;

public class Function : TrakHoundFunction
{
    public Function(ITrakHoundFunctionConfiguration configuration, ITrakHoundClient client, ITrakHoundVolume volume) : base(configuration, client, volume) { }

    protected async override Task<TrakHoundFunctionResponse> OnRun(IReadOnlyDictionary<string, string> parameters)
    {
        Log(TrakHoundLogLevel.Information, "Running UserPackageId Function..");

        return Ok("UserPackageId Function Completed");
    }
}
