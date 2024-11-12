import clr
clr.AddReference("TrakHound.Common")
import TrakHound.Functions
import time

def _run(request):

    transaction = TrakHound.Requests.TrakHoundEntityTransaction()

    transaction.Add(TrakHound.Requests.TrakHoundAssignmentEntry("Debug:/Entities/Assignment", "Debug:/Entities"))
    transaction.Add(TrakHound.Requests.TrakHoundBooleanEntry("Debug:/Entities/Boolean", True))
    transaction.Add(TrakHound.Requests.TrakHoundDurationEntry("Debug:/Entities/Duration", "5m"))
    transaction.Add(TrakHound.Requests.TrakHoundGroupEntry("Debug:/Entities/Group", "Debug:/Assignment"))
    transaction.Add(TrakHound.Requests.TrakHoundGroupEntry("Debug:/Entities/Group", "Debug:/Boolean"))
    transaction.Add(TrakHound.Requests.TrakHoundHashEntry("Debug:/Entities/Hash", "FirstName", "John"))
    transaction.Add(TrakHound.Requests.TrakHoundHashEntry("Debug:/Entities/Hash", "LastName", "Doe"))
    transaction.Add(TrakHound.Requests.TrakHoundLogEntry("Debug:/Entities/Log", "This is a Log Message", TrakHound.TrakHoundLogLevel.Debug))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberByte", "1", TrakHound.Entities.TrakHoundNumberDataType.Byte))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberInt16", "1234", TrakHound.Entities.TrakHoundNumberDataType.Int16))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberInt32", "123456", TrakHound.Entities.TrakHoundNumberDataType.Int32))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberInt64", "123456789", TrakHound.Entities.TrakHoundNumberDataType.Int64))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberFloat", "1.001", TrakHound.Entities.TrakHoundNumberDataType.Float))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberDouble", "1.001", TrakHound.Entities.TrakHoundNumberDataType.Double))
    transaction.Add(TrakHound.Requests.TrakHoundNumberEntry("Debug:/Entities/NumberDecimal", "1.001", TrakHound.Entities.TrakHoundNumberDataType.Decimal))
    transaction.Add(TrakHound.Requests.TrakHoundReferenceEntry("Debug:/Entities/Reference", "Debug:/Entities"))
    transaction.Add(TrakHound.Requests.TrakHoundSetEntry("Debug:/Entities/Set", "This is the first Set value"))
    transaction.Add(TrakHound.Requests.TrakHoundSetEntry("Debug:/Entities/Set", "This is the second Set value"))
    transaction.Add(TrakHound.Requests.TrakHoundStateEntry("Debug:/Entities/State", "Testing.Started"))
    transaction.Add(TrakHound.Requests.TrakHoundStringEntry("Debug:/Entities/String", "This is a STRING value"))  
    transaction.Add(TrakHound.Requests.TrakHoundTimeRangeEntry("Debug:/Entities/TimeRange", '1/1/1900', '1/1/2000'))
    transaction.Add(TrakHound.Requests.TrakHoundTimestampEntry("Debug:/Entities/Timestamp", '7/4/1776'))
    transaction.Add(TrakHound.Requests.TrakHoundVocabularyEntry("Debug:/Entities/Vocabulary", 'Testing.Test'))
    transaction.Add(TrakHound.Requests.TrakHoundVocabularySetEntry("Debug:/Entities/VocabularySet", 'Testing.Test'))

    request.Client.Entities.Publish(transaction)

    response = TrakHound.Functions.TrakHoundFunctionResponse()
    response.StatusCode = 200
    return response