import clr
clr.AddReference("TrakHound.Common")
import TrakHound.Functions

def _run(request):

    response = TrakHound.Functions.TrakHoundFunctionResponse()
    response.StatusCode = 200
    response.AddParameter("message", "HELLO WORLD")
    return response