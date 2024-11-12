import clr
clr.AddReference("TrakHound.Common")
import TrakHound.Functions

def _run(request):

    path = "Main:/Test/Observation"
    observation = request.Client.Entities.GetLatestObservation(path).Result

    response = TrakHound.Functions.TrakHoundFunctionResponse()
    response.StatusCode = 200
    response.AddParameter("message", observation.Value)
    return response