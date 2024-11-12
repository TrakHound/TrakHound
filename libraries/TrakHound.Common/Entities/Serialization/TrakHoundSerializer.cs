// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Serialization
{
    public static class TrakHoundSerializer
    {
        private static readonly Dictionary<string, PropertyInfo[]> _typeProperties = new Dictionary<string, PropertyInfo[]>();
        private static readonly ListDictionary<string, Attribute> _typeAttributes = new ListDictionary<string, Attribute>();
        private static readonly object _lock = new object();


        #region "Serialization"

        public static TrakHoundEntityTransaction Serialize(object obj)
        {
            if (obj != null)
            {
				if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()))
                {
                    var transaction = new TrakHoundEntityTransaction();
                    foreach (var cObj in (IEnumerable)obj)
                    {
                        transaction.Add(Serialize(cObj));
                    }
                    return transaction;
                }
                else
                {
					return SerializeObject(obj);
				}					
            }

            return null;
        }

        public static IEnumerable<T> Deserialize<T>(IEnumerable<ITrakHoundEntity> entities)
        {
            var results = DeserializeToResult<T>(entities, null);
            if (!results.IsNullOrEmpty())
            {
                return results.Select(o => o.Model);
            }

            return default;
        }

        public static IEnumerable<T> Deserialize<T>(IEnumerable<ITrakHoundEntity> entities, Dictionary<string, object> outputModels)
        {
            var results = DeserializeToResult<T>(entities, outputModels);
            if (!results.IsNullOrEmpty())
            {
                return results.Select(o => o.Model);
            }

            return default;
        }

        public static IEnumerable<T> Deserialize<T>(TrakHoundEntityCollection collection, IEnumerable<string> targetObjectUuids)
        {
            var results = DeserializeToResult<T>(collection, targetObjectUuids, null);
            if (!results.IsNullOrEmpty())
            {
                return results.Select(o => o.Model);
            }

            return default;
        }

        public static IEnumerable<T> Deserialize<T>(TrakHoundEntityCollection collection, IEnumerable<string> targetObjectUuids, Dictionary<string, object> outputModels)
        {
            var results = DeserializeToResult<T>(collection, targetObjectUuids, outputModels);
            if (!results.IsNullOrEmpty())
            {
                return results.Select(o => o.Model);
            }

            return default;
        }


        public static IEnumerable<TrakHoundDeserializationResult<T>> DeserializeToResult<T>(IEnumerable<ITrakHoundEntity> entities, Dictionary<string, object> outputModels)
        {
            if (!entities.IsNullOrEmpty())
            {
                var targetObjectUuids = entities.Select(o => o.Uuid);

                var collection = new TrakHoundEntityCollection();
                collection.Add(entities);

                return DeserializeToResult<T>(collection, targetObjectUuids, outputModels);
            }

            return default;
        }

        public static IEnumerable<TrakHoundDeserializationResult<T>> DeserializeToResult<T>(TrakHoundEntityCollection collection, IEnumerable<string> targetObjectUuids, Dictionary<string, object> outputModels)
        {
            if (collection != null)
            {
                return DeserializeObjectsToResult<T>(collection, targetObjectUuids, outputModels);
            }

            return default;
        }


        private static TrakHoundEntityTransaction SerializeObject(object obj, string name = null, string basePath = null, bool root = false)
        {
            var transaction = new TrakHoundEntityTransaction();

            if (obj != null)
            {
                var objectRequest = new TrakHoundObjectEntry();

                string objectBasePath = basePath;
                string objectPath = !string.IsNullOrEmpty(name) ? name : obj.GetType().Name;

                var childRequests = new List<ITrakHoundEntityEntryOperation>(); // Name => ContentRequest
                var contentRequests = new Dictionary<byte[], ITrakHoundEntityEntryOperation>(); // Name => ContentRequest

                // Root Object Attribute
                var rootObjectAttribute = GetAttribute<TrakHoundObjectAttribute>(obj.GetType());
                if (rootObjectAttribute != null)
                {
                    // Append Base Path
                    if (!string.IsNullOrEmpty(rootObjectAttribute.BasePath))
                    {
                        objectBasePath = TrakHoundPath.Combine(rootObjectAttribute.BasePath);
                    }

                    // Set Object ContentType
                    objectRequest.ContentType = rootObjectAttribute.ContentType.ToString();
                }

                // Root Definition Attribute
                var rootDefinitionAttributes = GetAttributes<TrakHoundDefinitionAttribute>(obj.GetType());
                if (!rootDefinitionAttributes.IsNullOrEmpty())
                {
                    string parentId = null;

                    foreach (var rootDefinitionAttribute in rootDefinitionAttributes)
                    {
                        // Create Definition Entry
                        var definitionRequest = new TrakHoundDefinitionEntry();
                        definitionRequest.Id = rootDefinitionAttribute.Id;
                        definitionRequest.ParentId = !string.IsNullOrEmpty(rootDefinitionAttribute.ParentId) ? rootDefinitionAttribute.ParentId : parentId;

                        // Set Description
                        if (!string.IsNullOrEmpty(rootDefinitionAttribute.Description))
                        {
                            definitionRequest.Descriptions.Add(rootDefinitionAttribute.LanguageCode, rootDefinitionAttribute.Description);
                        }

                        contentRequests.Remove(definitionRequest.EntryId);
                        contentRequests.Add(definitionRequest.EntryId, definitionRequest);

                        // Set Object Request Definition Properties
                        objectRequest.DefinitionId = definitionRequest.Id;
                        parentId = definitionRequest.Id;
                    }
                }

                // Process Properties
                var properties = obj.GetType().GetProperties();
                if (!properties.IsNullOrEmpty())
                {
                    var isPathSet = false;

                    // Set Object Name
                    var nameProperty = GetProperty<TrakHoundNameAttribute>(properties);
                    if (nameProperty != null)
                    {
                        var nameAttribute = GetAttribute<TrakHoundNameAttribute>(nameProperty);
                        if (nameAttribute != null)
                        {
                            if (!isPathSet) objectPath = nameProperty.GetValue(obj)?.ToString();
                        }
                    }

                    // Set Object Path
                    var pathProperty = GetProperty<TrakHoundPathAttribute>(properties);
                    if (pathProperty != null)
                    {
                        var pathAttribute = GetAttribute<TrakHoundPathAttribute>(pathProperty);
                        if (pathAttribute != null)
                        {
                            objectPath = pathProperty.GetValue(obj)?.ToString();
                            isPathSet = true;
                        }
                    }

                    // Set Object Path Parameter
                    var pathParameterProperty = GetProperty<TrakHoundPathParameterAttribute>(properties);
                    if (pathParameterProperty != null)
                    {
                        var pathParameterAttribute = GetAttribute<TrakHoundPathParameterAttribute>(pathParameterProperty);
                        if (pathParameterAttribute != null)
                        {
                            var pathParameterName = pathParameterProperty.Name.ToCamelCase();
                            if (!string.IsNullOrEmpty(pathParameterAttribute.Name)) pathParameterName = pathParameterAttribute.Name.ToCamelCase();

                            objectBasePath = TrakHoundPath.SetPathParameter(objectBasePath, objectBasePath, pathParameterName, pathParameterProperty.GetValue(obj)?.ToString());
                            objectPath = TrakHoundPath.SetPathParameter(objectPath, objectPath, pathParameterName, pathParameterProperty.GetValue(obj)?.ToString());
                        }
                    }

                    // Update Object Path
                    objectRequest.Path = TrakHoundPath.Combine(objectBasePath, objectPath);

                    foreach (var property in properties)
                    {
                        // Add Metadata
                        var metadataAttribute = GetAttribute<TrakHoundMetadataAttribute>(property);
                        if (metadataAttribute != null)
                        {
                            var metadataName = metadataAttribute.Name;
                            if (string.IsNullOrEmpty(metadataName)) metadataName = property.Name;

                            var metadataValue = property.GetValue(obj)?.ToString();

                            if (!string.IsNullOrEmpty(metadataName))
                            {
                                objectRequest.Metadata.Remove(metadataName);
                                objectRequest.Metadata.Add(metadataName, metadataValue);
                            }
                        }

                        //// Add Blobs
                        //var blobAttribute = GetAttribute<TrakHoundBlobAttribute>(property);
                        //if (blobAttribute != null)
                        //{
                        //    var blobObjectPath = property.Name;
                        //    if (!string.IsNullOrEmpty(blobAttribute.Name)) blobObjectPath = blobAttribute.Name;
                        //    if (!string.IsNullOrEmpty(blobAttribute.BasePath)) blobObjectPath = TrakHoundPath.Combine(blobAttribute.BasePath, blobObjectPath);
                        //    if (!string.IsNullOrEmpty(blobAttribute.Path)) blobObjectPath = blobAttribute.Path;

                        //    if (property.PropertyType == typeof(byte[]))
                        //    {
                        //        var blobValue = (byte[])property.GetValue(obj);
                        //        if (blobValue != null)
                        //        {
                        //            var blobRequest = new TrakHoundBlobEntry();
                        //            blobRequest.ObjectPath = blobObjectPath;
                        //            blobRequest.ContentType = blobAttribute.ContentType;
                        //            blobRequest.Content = TrakHoundObjectBlobEntity.CreateBase64String(blobValue);
                        //            contentRequests.Remove(blobRequest.EntryId);
                        //            contentRequests.Add(blobRequest.EntryId, blobRequest);
                        //        }
                        //    }
                        //}

                        // Add Durations
                        var durationAttribute = GetAttribute<TrakHoundDurationAttribute>(property);
                        if (durationAttribute != null)
                        {
                            var durationObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(durationAttribute.Name)) durationObjectPath = durationAttribute.Name;
                            if (!string.IsNullOrEmpty(durationAttribute.BasePath)) durationObjectPath = TrakHoundPath.Combine(durationAttribute.BasePath, durationObjectPath);
                            if (!string.IsNullOrEmpty(durationAttribute.Path)) durationObjectPath = durationAttribute.Path;

                            var durationRequest = new TrakHoundDurationEntry();
                            durationRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, durationObjectPath);
                            durationRequest.Value = property.GetValue(obj).ToString();

                            transaction.Add(durationRequest);
                        }

                        // Add Booleans
                        var booleanAttribute = GetAttribute<TrakHoundBooleanAttribute>(property);
                        if (booleanAttribute != null)
                        {
                            var booleanObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(booleanAttribute.Name)) booleanObjectPath = booleanAttribute.Name;
                            if (!string.IsNullOrEmpty(booleanAttribute.BasePath)) booleanObjectPath = TrakHoundPath.Combine(booleanAttribute.BasePath, booleanObjectPath);
                            if (!string.IsNullOrEmpty(booleanAttribute.Path)) booleanObjectPath = booleanAttribute.Path;

                            var booleanRequest = new TrakHoundBooleanEntry();
                            booleanRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, booleanObjectPath);
                            booleanRequest.Value = property.GetValue(obj).ToBoolean();

                            transaction.Add(booleanRequest);
                        }

                        // Add Events
                        var eventAttribute = GetAttribute<TrakHoundEventAttribute>(property);
                        if (eventAttribute != null)
                        {
                            var eventObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(eventAttribute.Name)) eventObjectPath = eventAttribute.Name;
                            if (!string.IsNullOrEmpty(eventAttribute.BasePath)) eventObjectPath = TrakHoundPath.Combine(eventAttribute.BasePath, eventObjectPath);
                            if (!string.IsNullOrEmpty(eventAttribute.Path)) eventObjectPath = eventAttribute.Path;

                            var eventType = property.GetValue(obj)?.ToString();
                            if (eventType != null)
                            {
                                var eventRequest = new TrakHoundEventEntry();
                                eventRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, eventObjectPath);

                                transaction.Add(eventRequest);
                            }
                        }

                        // Add Groups
                        var groupAttribute = GetAttribute<TrakHoundGroupAttribute>(property);
                        if (groupAttribute != null)
                        {
                            var groupObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(groupAttribute.Name)) groupObjectPath = groupAttribute.Name;
                            if (!string.IsNullOrEmpty(groupAttribute.BasePath)) groupObjectPath = TrakHoundPath.Combine(groupAttribute.BasePath, groupObjectPath);
                            if (!string.IsNullOrEmpty(groupAttribute.Path)) groupObjectPath = groupAttribute.Path;

                            var groupMemberPath = property.GetValue(obj)?.ToString();
                            if (groupMemberPath != null)
                            {
                                // Add Member Base Path
                                if (!string.IsNullOrEmpty(groupAttribute.MemberBasePath))
                                {
                                    groupMemberPath = TrakHoundPath.Combine(groupAttribute.MemberBasePath, groupMemberPath);
                                }

                                var groupRequest = new TrakHoundGroupEntry();
                                groupRequest.GroupPath = TrakHoundPath.Combine(objectRequest.Path, groupObjectPath);
                                groupRequest.MemberPath = groupMemberPath;

                                transaction.Add(groupRequest);
                            }
                        }

                        // Add Hashes
                        var hashAttribute = GetAttribute<TrakHoundHashAttribute>(property);
                        if (hashAttribute != null)
                        {
                            var hashObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(hashAttribute.Name)) hashObjectPath = hashAttribute.Name;
                            if (!string.IsNullOrEmpty(hashAttribute.BasePath)) hashObjectPath = TrakHoundPath.Combine(hashAttribute.BasePath, hashObjectPath);
                            if (!string.IsNullOrEmpty(hashAttribute.Path)) hashObjectPath = hashAttribute.Path;

                            var hashValue = property.GetValue(obj);
                            if (hashValue != null)
                            {
                                hashObjectPath = TrakHoundPath.Combine(objectRequest.Path, hashObjectPath);

                                if (property.PropertyType == typeof(string) && !string.IsNullOrEmpty(hashAttribute.Key))
                                {
                                    var hashKey = !string.IsNullOrEmpty(hashAttribute.KeyPrefix) ? hashAttribute.KeyPrefix + hashAttribute.Key : hashAttribute.Key;
                                    var hashRequest = new TrakHoundHashEntry(hashObjectPath, hashKey, (string)hashValue);

                                    transaction.Add(hashRequest);
                                }
                                else if (typeof(IDictionary<,>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) ||
                                    typeof(IDictionary).IsAssignableFrom(property.PropertyType))
                                {
                                    var hashValues = new Dictionary<string, string>();

                                    IDictionary hashDictionary = (IDictionary)hashValue;
                                    foreach (var hashDictionaryKeyObj in hashDictionary.Keys)
                                    {
                                        var hashDictionaryKey = hashDictionaryKeyObj != null ? hashDictionaryKeyObj.ToString() : null;
                                        if (hashDictionaryKey != null)
                                        {
                                            var hashDictionaryValue = hashDictionary[hashDictionaryKeyObj];
                                            if (hashDictionaryValue != null)
                                            {
                                                var hashKey = !string.IsNullOrEmpty(hashAttribute.KeyPrefix) ? hashAttribute.KeyPrefix + hashDictionaryKey : hashDictionaryKey;

                                                hashValues.Remove(hashKey);
                                                hashValues.Add(hashKey, hashDictionaryValue.ToString());
                                            }
                                        }
                                    }

                                    var hashRequest = new TrakHoundHashEntry(hashObjectPath, hashValues);

                                    transaction.Add(hashRequest);
                                }
                                else
                                {
                                    var hashKey = !string.IsNullOrEmpty(hashAttribute.KeyPrefix) ? hashAttribute.KeyPrefix + hashAttribute.Key : hashAttribute.Key;
                                    var hashRequest = new TrakHoundHashEntry(hashObjectPath, hashKey, hashValue.ToString());

                                    transaction.Add(hashRequest);
                                }
                            }
                        }

                        // Add Numbers
                        var numberAttribute = GetAttribute<TrakHoundNumberAttribute>(property);
                        if (numberAttribute != null)
                        {
                            var numberObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(numberAttribute.Name)) numberObjectPath = numberAttribute.Name;
                            if (!string.IsNullOrEmpty(numberAttribute.BasePath)) numberObjectPath = TrakHoundPath.Combine(numberAttribute.BasePath, numberObjectPath);
                            if (!string.IsNullOrEmpty(numberAttribute.Path)) numberObjectPath = numberAttribute.Path;

                            TrakHoundNumberDataType dataType = numberAttribute.DataType.HasValue ? numberAttribute.DataType.Value : TrakHoundNumberAttribute.GetDataType(property.PropertyType);

                            var numberValue = property.GetValue(obj)?.ToString();
                            if (numberValue != null)
                            {
                                var numberRequest = new TrakHoundNumberEntry();
                                numberRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, numberObjectPath);

                                // Set Definition
                                var propertyDefinitionAttribute = GetAttribute<TrakHoundPropertyDefinitionAttribute>(property);
                                if (propertyDefinitionAttribute != null)
                                {
                                    var propertyDefinitionType = !string.IsNullOrEmpty(propertyDefinitionAttribute.Type) ? propertyDefinitionAttribute.Type : property.Name;
                                    numberRequest.ObjectDefinitionId = GetPropertyDefinitionId(ref contentRequests, objectRequest.DefinitionId, propertyDefinitionType, propertyDefinitionAttribute.Description, propertyDefinitionAttribute.LanguageCode, propertyDefinitionAttribute.ParentId);
                                }

                                numberRequest.Value = numberValue;
                                numberRequest.DataType = dataType;

                                transaction.Add(numberRequest);
                            }
                        }

                        // Add Observations
                        var observationAttribute = GetAttribute<TrakHoundObservationAttribute>(property);
                        if (observationAttribute != null)
                        {
                            var observationObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(observationAttribute.Name)) observationObjectPath = observationAttribute.Name;
                            if (!string.IsNullOrEmpty(observationAttribute.BasePath)) observationObjectPath = TrakHoundPath.Combine(observationAttribute.BasePath, observationObjectPath);
                            if (!string.IsNullOrEmpty(observationAttribute.Path)) observationObjectPath = observationAttribute.Path;

                            var observationDataType = observationAttribute.DataType;

                            var observationValue = property.GetValue(obj)?.ToString();
                            if (observationValue != null)
                            {
                                var observationRequest = new TrakHoundObservationEntry();
                                observationRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, observationObjectPath);
                                observationRequest.Value = observationValue;
                                observationRequest.DataType = observationDataType;

                                transaction.Add(observationRequest);
                            }
                        }

                        // Add References
                        var referenceAttribute = GetAttribute<TrakHoundReferenceAttribute>(property);
                        if (referenceAttribute != null)
                        {
                            var referenceObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(referenceAttribute.Name)) referenceObjectPath = referenceAttribute.Name;
                            if (!string.IsNullOrEmpty(referenceAttribute.BasePath)) referenceObjectPath = TrakHoundPath.Combine(referenceAttribute.BasePath, referenceObjectPath);
                            if (!string.IsNullOrEmpty(referenceAttribute.Path)) referenceObjectPath = referenceAttribute.Path;

                            var referenceTargetPath = property.GetValue(obj)?.ToString();
                            if (referenceTargetPath != null)
                            {
                                // Add Target Base Path
                                if (!string.IsNullOrEmpty(referenceAttribute.TargetBasePath))
                                {
                                    referenceTargetPath = TrakHoundPath.Combine(referenceAttribute.TargetBasePath, referenceTargetPath);
                                }

                                var referenceRequest = new TrakHoundReferenceEntry();
                                referenceRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, referenceObjectPath);
                                referenceRequest.TargetPath = referenceTargetPath;

                                transaction.Add(referenceRequest);
                            }
                        }

                        // Add Sets
                        var setAttribute = GetAttribute<TrakHoundSetAttribute>(property);
                        if (setAttribute != null)
                        {
                            var setObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(setAttribute.Name)) setObjectPath = setAttribute.Name;
                            if (!string.IsNullOrEmpty(setAttribute.BasePath)) setObjectPath = TrakHoundPath.Combine(setAttribute.BasePath, setObjectPath);
                            if (!string.IsNullOrEmpty(setAttribute.Path)) setObjectPath = setAttribute.Path;

                            var setValue = property.GetValue(obj);
                            if (setValue != null)
                            {
                                setObjectPath = TrakHoundPath.Combine(objectRequest.Path, setObjectPath);

                                if (property.PropertyType == typeof(string))
                                {
                                    var setRequest = new TrakHoundSetEntry(setObjectPath, (string)setValue);
                                    setRequest.EntryType = setAttribute.EntryType;

                                    transaction.Add(setRequest);
                                }
                                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                {
                                    var setValues = new List<string>();

                                    IEnumerable setListValues = (IEnumerable)setValue;
                                    foreach (var setListValue in setListValues)
                                    {
                                        if (setListValue != null) setValues.Add(setListValue.ToString());
                                    }

                                    var setRequest = new TrakHoundSetEntry(setObjectPath, setValues);
                                    setRequest.EntryType = setAttribute.EntryType;

                                    transaction.Add(setRequest);
                                }
                                else
                                {
                                    var setRequest = new TrakHoundSetEntry(setObjectPath, setValue.ToString());
                                    setRequest.EntryType = setAttribute.EntryType;

                                    transaction.Add(setRequest);
                                }
                            }
                        }

                        // Add States
                        var stateAttribute = GetAttribute<TrakHoundStateAttribute>(property);
                        if (stateAttribute != null)
                        {
                            var stateObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(stateAttribute.Name)) stateObjectPath = stateAttribute.Name;
                            if (!string.IsNullOrEmpty(stateAttribute.BasePath)) stateObjectPath = TrakHoundPath.Combine(stateAttribute.BasePath, stateObjectPath);
                            if (!string.IsNullOrEmpty(stateAttribute.Path)) stateObjectPath = stateAttribute.Path;

                            var stateType = property.GetValue(obj)?.ToString();
                            if (stateType != null)
                            {
                                var stateRequest = new TrakHoundStateEntry();
                                stateRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, stateObjectPath);

                                stateRequest.DefinitionId = GetDefinitionId(objectPath, stateObjectPath, stateType);

                                transaction.Add(stateRequest);
                            }
                        }

                        // Add Strings
                        var stringAttribute = GetAttribute<TrakHoundStringAttribute>(property);
                        if (stringAttribute != null)
                        {
                            var stringObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(stringAttribute.Name)) stringObjectPath = stringAttribute.Name;
                            if (!string.IsNullOrEmpty(stringAttribute.BasePath)) stringObjectPath = TrakHoundPath.Combine(stringAttribute.BasePath, stringObjectPath);
                            if (!string.IsNullOrEmpty(stringAttribute.Path)) stringObjectPath = stringAttribute.Path;

                            var stringValue = property.GetValue(obj)?.ToString();
                            if (stringValue != null)
                            {
                                var stringRequest = new TrakHoundStringEntry();
                                stringRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, stringObjectPath);

                                // Set Definition
                                var propertyDefinitionAttribute = GetAttribute<TrakHoundPropertyDefinitionAttribute>(property);
                                if (propertyDefinitionAttribute != null)
                                {
                                    var propertyDefinitionType = !string.IsNullOrEmpty(propertyDefinitionAttribute.Type) ? propertyDefinitionAttribute.Type : property.Name;
                                    stringRequest.ObjectDefinitionId = GetPropertyDefinitionId(ref contentRequests, objectRequest.DefinitionId, propertyDefinitionType, propertyDefinitionAttribute.Description, propertyDefinitionAttribute.LanguageCode, propertyDefinitionAttribute.ParentId);
                                }

                                stringRequest.Value = stringValue;

                                transaction.Add(stringRequest);
                            }
                        }

                        // Add Timestamp
                        var timestampAttribute = GetAttribute<TrakHoundTimestampAttribute>(property);
                        if (timestampAttribute != null)
                        {
                            var timestampObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(timestampAttribute.Name)) timestampObjectPath = timestampAttribute.Name;
                            if (!string.IsNullOrEmpty(timestampAttribute.BasePath)) timestampObjectPath = TrakHoundPath.Combine(timestampAttribute.BasePath, timestampObjectPath);
                            if (!string.IsNullOrEmpty(timestampAttribute.Path)) timestampObjectPath = timestampAttribute.Path;

                            var timestampValue = property.GetValue(obj);
                            if (timestampValue != null)
                            {
                                var timestampRequest = new TrakHoundTimestampEntry();
                                timestampRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, timestampObjectPath);

                                if (timestampValue.GetType() == typeof(DateTime))
                                {
                                    timestampRequest.Value = ((DateTime)timestampValue).ToUnixTime().ToString();
                                }
                                else if (timestampValue.GetType() == typeof(long))
                                {
                                    timestampRequest.Value = timestampValue.ToString();
                                }
                                else
                                {
                                    timestampRequest.Value = timestampValue.ToString();
                                }

                                transaction.Add(timestampRequest);
                            }
                        }

                        // Add TimeRanges
                        var timeRangeAttribute = GetAttribute<TrakHoundTimeRangeAttribute>(property);
                        if (timeRangeAttribute != null)
                        {
                            var timeRangeObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(timeRangeAttribute.Name)) timeRangeObjectPath = timeRangeAttribute.Name;
                            if (!string.IsNullOrEmpty(timeRangeAttribute.BasePath)) timeRangeObjectPath = TrakHoundPath.Combine(timeRangeAttribute.BasePath, timeRangeObjectPath);
                            if (!string.IsNullOrEmpty(timeRangeAttribute.Path)) timeRangeObjectPath = timeRangeAttribute.Path;

                            var timeRangeValue = property.GetValue(obj)?.ToString();
                            if (timeRangeValue != null)
                            {                              
                                var timeRange = TimeRange.Parse(timeRangeValue);
                                if (timeRange.IsValid)
                                {
                                    var timeRangeRequest = new TrakHoundTimeRangeEntry();
                                    timeRangeRequest.ObjectPath = TrakHoundPath.Combine(objectRequest.Path, timeRangeObjectPath);
                                    timeRangeRequest.Start = timeRange.From;
                                    timeRangeRequest.End = timeRange.To;

                                    transaction.Add(timeRangeRequest);
                                }
                            }
                        }

                        // Set Child Objects
                        var objectAttribute = GetAttribute<TrakHoundObjectAttribute>(property);
                        if (objectAttribute != null)
                        {
                            var childObjectsValue = property.GetValue(obj);
                            if (childObjectsValue != null)
                            {
                                if (property.PropertyType == typeof(string))
                                {
                                    var childObjectName = property.GetValue(obj)?.ToString();
                                    if (childObjectName != null)
                                    {
                                        var childEntry = new TrakHoundObjectEntry();
                                        childEntry.Path = TrakHoundPath.Combine(objectRequest.Path, childObjectName);
                                        childEntry.ContentType = objectAttribute.ContentType.ToString();
                                        transaction.Add(childEntry);
                                    }
                                }
                                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                {
                                    var childPath = property.Name;
                                    if (!string.IsNullOrEmpty(objectAttribute.Name)) childPath = objectAttribute.Name;
                                    if (!string.IsNullOrEmpty(objectAttribute.BasePath)) childPath = TrakHoundPath.Combine(objectAttribute.BasePath, childPath);
                                    if (!string.IsNullOrEmpty(objectAttribute.Path)) childPath = objectAttribute.Path;

                                    childPath = TrakHoundPath.Combine(objectRequest.Path, childPath);

                                    var childParentPath = TrakHoundPath.GetParentPath(childPath);
                                    var childName = TrakHoundPath.GetObject(childPath);

                                    IEnumerable cObjs = (IEnumerable)childObjectsValue;
                                    foreach (var cObj in cObjs)
                                    {
                                        transaction.Add(SerializeObject(cObj, null, childPath));
                                    }
                                }
                                else
                                {
                                    var childPath = property.Name;
                                    if (!string.IsNullOrEmpty(objectAttribute.Name)) childPath = objectAttribute.Name;
                                    if (!string.IsNullOrEmpty(objectAttribute.BasePath)) childPath = TrakHoundPath.Combine(objectAttribute.BasePath, childPath);
                                    if (!string.IsNullOrEmpty(objectAttribute.Path)) childPath = objectAttribute.Path;

                                    childPath = TrakHoundPath.Combine(objectRequest.Path, childPath);

                                    var childParentPath = TrakHoundPath.GetParentPath(childPath);
                                    var childName = TrakHoundPath.GetObject(childPath);

                                    transaction.Add(SerializeObject(childObjectsValue, childName, childParentPath));
                                }
                            }
                        }
                    }
                }

                transaction.Add(objectRequest);
            }

            return transaction;
        }


        public static IEnumerable<TrakHoundDeserializationResult<T>> DeserializeObjectsToResult<T>(TrakHoundEntityCollection collection, IEnumerable<string> targetObjectUuids, Dictionary<string, object> outputModels)
        {
            var responses = DeserializeObjectsToResult(typeof(T), collection, targetObjectUuids, outputModels);
            if (!responses.IsNullOrEmpty())
            {
                var genericResponses = new List<TrakHoundDeserializationResult<T>>();
                foreach (var response in responses)
                {
                    var result = new TrakHoundDeserializationResult<T>(response.Key, (T)response.Value);
                    genericResponses.Add(result);
                }
                return genericResponses;
            }

            return null;
        }

        public static Dictionary<string, object> DeserializeObjectsToResult(Type responseType, TrakHoundEntityCollection collection, IEnumerable<string> targetObjectUuids, Dictionary<string, object> outputModels, int level = 0)
        {
            if (collection != null)
            {
                var responses = new Dictionary<string, object>();

                var targets = new List<string>();
                if (!targetObjectUuids.IsNullOrEmpty())
                {
                    targets.AddRange(targetObjectUuids);
                }
                else
                {
                    var targetPath = GetObjectPathAttribute(responseType);
                    if (!string.IsNullOrEmpty(targetPath))
                    {
                        var targetObjects = collection.Objects.QueryObjects(targetPath);
                        if (!targetObjects.IsNullOrEmpty()) targets.AddRange(targetObjects.Select(o => o.Uuid));
                    }
                    else
                    {
                        if (!collection.TargetUuids.IsNullOrEmpty()) targets.AddRange(collection.TargetUuids);
                        //if (!collection.ComponentTargets.IsNullOrEmpty()) targets.AddRange(collection.ComponentTargets.Select(o => o.AssemblyUuid));
                    }
                }


                if (!targets.IsNullOrEmpty())
                {
                    var objectEntities = new List<ITrakHoundObjectEntity>();
                    foreach (var targetId in targets)
                    {
                        var objectEntity = collection.Objects.GetObject(targetId);
                        if (objectEntity != null)
                        {
                            string basePath = null;
                            object response = null;
                            var responseKey = $"{responseType.AssemblyQualifiedName}:{objectEntity.Uuid}";

                            if (!outputModels.IsNullOrEmpty())
                            {
                                response = outputModels.GetValueOrDefault(responseKey);
                            }

                            if (response == null)
                            {
                                response = CreateInstance(responseType);
                                if (outputModels != null && response != null) outputModels.Add(responseKey, response);
                                //if (outputModels != null && response != null) outputModels.Add(objectEntity.Uuid, response);
                                //if (outputModels != null && level < 1) outputModels.Add(objectEntity.Uuid, response);
                            }

                            // Base Path
                            var basePathAttribute = GetAttribute<TrakHoundObjectAttribute>(responseType);
                            if (basePathAttribute != null)
                            {
                                basePath = basePathAttribute.BasePath;
                            }

                            var properties = GetProperties(responseType);
                            if (!properties.IsNullOrEmpty())
                            {
                                foreach (var property in properties)
                                {
                                    if (property.CanWrite)
                                    {
                                        // UUID
                                        var uuidAttribute = GetAttribute<TrakHoundUuidAttribute>(property);
                                        if (uuidAttribute != null)
                                        {
                                            SetProperty(property, response, ChangeType(objectEntity.Uuid, property.PropertyType));
                                        }

                                        // Type
                                        var typeAttributes = GetAttributes<TrakHoundTypeAttribute>(property);
                                        if (!typeAttributes.IsNullOrEmpty())
                                        {
                                            var objectDefinitions = GetDefinitions(collection, objectEntity.DefinitionUuid);
                                            if (!objectDefinitions.IsNullOrEmpty())
                                            {
                                                string type = null;

                                                foreach (var typeAttribute in typeAttributes)
                                                {
                                                    if (!string.IsNullOrEmpty(typeAttribute.Pattern))
                                                    {
                                                        type = objectDefinitions.FirstOrDefault(o => MatchDefinitionType(o.Id, typeAttribute.Pattern))?.Type;
                                                        if (!string.IsNullOrEmpty(type)) break;
                                                    }
                                                }

                                                SetProperty(property, response, ChangeType(type, property.PropertyType));
                                            }
                                        }

                                        // Name
                                        var nameAttribute = GetAttribute<TrakHoundNameAttribute>(property);
                                        if (nameAttribute != null)
                                        {
                                            SetProperty(property, response, ChangeType(objectEntity.Name, property.PropertyType));
                                        }

                                        // Path
                                        var pathAttribute = GetAttribute<TrakHoundPathAttribute>(property);
                                        if (pathAttribute != null)
                                        {
                                            SetProperty(property, response, ChangeType(objectEntity.GetAbsolutePath(), property.PropertyType));
                                        }

                                        // Path Parameter
                                        var pathParameterAttribute = GetAttribute<TrakHoundPathParameterAttribute>(property);
                                        if (pathParameterAttribute != null)
                                        {
                                            var pathParameterName = pathParameterAttribute.Name;
                                            if (string.IsNullOrEmpty(pathParameterName)) pathParameterName = property.Name;
                                            pathParameterName = pathParameterName.ToCamelCase();

                                            var value = Url.GetRouteParameter(objectEntity.GetAbsolutePath(), basePath, pathParameterName);
                                            SetProperty(property, response, ChangeType(value, property.PropertyType));
                                        }

                                        // Metadata
                                        var metadataAttribute = GetAttribute<TrakHoundMetadataAttribute>(property);
                                        if (metadataAttribute != null)
                                        {
                                            var metadataName = !string.IsNullOrEmpty(metadataAttribute.Name) ? metadataAttribute.Name : property.Name;
                                            var metadataUuid = TrakHoundObjectMetadataEntity.GenerateUuid(objectEntity.Uuid, metadataName);

                                            var metadataEntity = collection.Objects.GetMetadata(metadataUuid);
                                            if (metadataEntity != null)
                                            {
                                                SetProperty(property, response, ChangeType(metadataEntity.Value, property.PropertyType));
                                            }
                                        }


                                        // Assignments
                                        var assignmentAttribute = GetAttribute<TrakHoundAssignmentAttribute>(property);
                                        if (assignmentAttribute != null)
                                        {
                                            var assignmentName = property.Name;
                                            if (!string.IsNullOrEmpty(assignmentAttribute.Name)) assignmentName = assignmentAttribute.Name;

                                            var assignmentPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), assignmentName);
                                            if (!string.IsNullOrEmpty(assignmentAttribute.BasePath)) assignmentPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), assignmentAttribute.BasePath, assignmentName);
                                            if (!string.IsNullOrEmpty(assignmentAttribute.Path))
                                            {
                                                assignmentPath = TrakHoundPath.IsAbsolute(assignmentAttribute.Path) ? assignmentAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), assignmentAttribute.Path);
                                            }

                                            var assignmentObject = collection.Objects.QueryObjects(assignmentPath)?.FirstOrDefault();
                                            if (assignmentObject != null)
                                            {
                                                var assignmentEntity = collection.Objects.QueryAssignmentsByAssigneeUuid(assignmentObject.Uuid)?.FirstOrDefault();
                                                if (assignmentEntity != null)
                                                {
                                                    var memberObject = collection.Objects.GetObject(assignmentEntity.MemberUuid);

                                                    if (assignmentEntity.RemoveTimestamp > 0)
                                                    {
                                                        SetProperty(property, response, ChangeType(default, property.PropertyType));
                                                    }
                                                    else
                                                    {
                                                        SetProperty(property, response, ChangeType(memberObject?.Path, property.PropertyType));
                                                    }
                                                }
                                            }
                                        }

                                        //// Blobs
                                        //var blobAttribute = property.GetCustomAttribute<TrakHoundBlobAttribute>();
                                        //if (blobAttribute != null)
                                        //{
                                        //    if (property.PropertyType == typeof(byte[]))
                                        //    {
                                        //        var blobPath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //        if (!string.IsNullOrEmpty(blobAttribute.BasePath)) blobPath = TrakHoundPath.Combine(objectEntity.Path, blobAttribute.BasePath, property.Name);

                                        //        var blobObject = collection.Objects.QueryObjectsByPath(blobPath)?.FirstOrDefault();
                                        //        if (blobObject != null)
                                        //        {
                                        //            var blobEntity = collection.Objects.QueryBlobModels(blobObject.Uuid)?.FirstOrDefault();
                                        //            if (blobEntity != null)
                                        //            {
                                        //                property.SetValue(response, blobEntity.Bytes);
                                        //            }
                                        //        }
                                        //    }
                                        //}

                                        // Booleans
                                        var booleanAttribute = GetAttribute<TrakHoundBooleanAttribute>(property);
                                        if (booleanAttribute != null)
                                        {
                                            var booleanName = property.Name;
                                            if (!string.IsNullOrEmpty(booleanAttribute.Name)) booleanName = booleanAttribute.Name;

                                            var booleanPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), booleanName);
                                            if (!string.IsNullOrEmpty(booleanAttribute.BasePath)) booleanPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), booleanAttribute.BasePath, booleanName);
                                            if (!string.IsNullOrEmpty(booleanAttribute.Path))
                                            {
                                                booleanPath = TrakHoundPath.IsAbsolute(booleanAttribute.Path) ? booleanAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), booleanAttribute.Path);
                                            }

                                            var booleanObject = collection.Objects.QueryObjects(booleanPath)?.FirstOrDefault();
                                            if (booleanObject != null)
                                            {
                                                var booleanEntity = collection.Objects.QueryBooleanByObjectUuid(booleanObject.Uuid);
                                                if (booleanEntity != null)
                                                {
                                                    SetProperty(property, response, ChangeType(booleanEntity.Value, property.PropertyType));
                                                }
                                            }
                                        }

                                        // Durations
                                        var durationAttribute = GetAttribute<TrakHoundDurationAttribute>(property);
                                        if (durationAttribute != null)
                                        {
                                            var durationName = property.Name;
                                            if (!string.IsNullOrEmpty(durationAttribute.Name)) durationName = durationAttribute.Name;

                                            var durationPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), durationName);
                                            if (!string.IsNullOrEmpty(durationAttribute.BasePath)) durationPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), durationAttribute.BasePath, durationName);
                                            if (!string.IsNullOrEmpty(durationAttribute.Path))
                                            {
                                                durationPath = TrakHoundPath.IsAbsolute(durationAttribute.Path) ? durationAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), durationAttribute.Path);
                                            }

                                            var durationObject = collection.Objects.QueryObjects(durationPath)?.FirstOrDefault();
                                            if (durationObject != null)
                                            {
                                                var durationEntity = collection.Objects.QueryDurationByObjectUuid(durationObject.Uuid);
                                                if (durationEntity != null)
                                                {
                                                    if (property.PropertyType == typeof(TimeSpan))
                                                    {
                                                        var ticks = durationEntity.Value;
                                                        SetProperty(property, response, ChangeType(TimeSpan.FromTicks((long)ticks), property.PropertyType));
                                                    }
                                                    else
                                                    {
                                                        SetProperty(property, response, ChangeType(durationEntity.Value, property.PropertyType));
                                                    }
                                                }
                                            }
                                        }

                                        // Events
                                        var eventAttribute = property.GetCustomAttribute<TrakHoundEventAttribute>();
                                        if (eventAttribute != null)
                                        {
                                            var eventName = property.Name;
                                            if (!string.IsNullOrEmpty(eventAttribute.Name)) eventName = eventAttribute.Name;

                                            var eventPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), eventName);
                                            if (!string.IsNullOrEmpty(eventAttribute.BasePath)) eventPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), eventAttribute.BasePath, eventName);
                                            if (!string.IsNullOrEmpty(eventAttribute.Path))
                                            {
                                                eventPath = TrakHoundPath.IsAbsolute(eventAttribute.Path) ? eventAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), eventAttribute.Path);
                                            }

                                            var eventObject = collection.Objects.QueryObjects(eventPath)?.FirstOrDefault();
                                            if (eventObject != null)
                                            {
                                                //var eventEntity = collection.Objects.QueryEventModels(eventObject.Uuid)?.FirstOrDefault();
                                                //if (eventEntity != null)
                                                //{
                                                //    if (property.PropertyType == typeof(string))
                                                //    {
                                                //        var targetPath = eventEntity.Target?.Path;
                                                //        if (!string.IsNullOrEmpty(eventAttribute.TargetBasePath)) targetPath = TrakHoundPath.GetRelativeTo(eventAttribute.TargetBasePath, targetPath);

                                                //        SetProperty(property, response, ChangeType(targetPath, property.PropertyType));
                                                //    }
                                                //    else
                                                //    {
                                                //        var childEntity = collection.Objects.GetObject(eventEntity.TargetUuid);
                                                //        if (childEntity != null)
                                                //        {
                                                //            var childResponses = DeserializeObjectsToResult(property.PropertyType, collection, outputModels, new string[] { childEntity.Uuid }, level + 1);
                                                //            if (!childResponses.IsNullOrEmpty())
                                                //            {
                                                //                var childResponse = childResponses.FirstOrDefault();
                                                //                if (childResponse.Key != null)
                                                //                {
                                                //                    if (outputModels != null)
                                                //                    {
                                                //                        var childResponseKey = $"{property.PropertyType.AssemblyQualifiedName}:{childResponse.Key}";

                                                //                        outputModels.Remove(childResponseKey);
                                                //                        outputModels.Add(childResponseKey, childResponse.Value);
                                                //                    }

                                                //                    SetProperty(property, response, childResponse.Value);
                                                //                }
                                                //            }
                                                //        }
                                                //    }
                                                //}
                                            }
                                        }

                                        //// Events
                                        //var eventAttribute = GetAttribute<TrakHoundEventAttribute>(property);
                                        //if (eventAttribute != null)
                                        //{
                                        //    var eventPath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //    if (!string.IsNullOrEmpty(eventAttribute.BasePath)) eventPath = TrakHoundPath.Combine(objectEntity.Path, eventAttribute.BasePath, property.Name);
                                        //    if (!string.IsNullOrEmpty(eventAttribute.Path)) eventPath = TrakHoundPath.Combine(objectEntity.Path, eventAttribute.Path);

                                        //    var eventObject = collection.Objects.QueryObjects(eventPath)?.FirstOrDefault();
                                        //    if (eventObject != null)
                                        //    {
                                        //        var eventEntity = collection.Objects.QueryEventModels(eventObject.Uuid)?.FirstOrDefault();
                                        //        if (eventEntity != null)
                                        //        {
                                        //            property.SetValue(response, ChangeType(eventEntity.Type, property.PropertyType));
                                        //        }
                                        //    }
                                        //}

                                        //// Feeds
                                        //var feedAttribute = property.GetCustomAttribute<TrakHoundFeedAttribute>();
                                        //if (feedAttribute != null)
                                        //{
                                        //    var feedPath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //    if (!string.IsNullOrEmpty(feedAttribute.BasePath)) feedPath = TrakHoundPath.Combine(objectEntity.Path, feedAttribute.BasePath, property.Name);

                                        //    var feedObject = collection.Objects.QueryObjectsByPath(feedPath)?.FirstOrDefault();
                                        //    if (feedObject != null)
                                        //    {
                                        //        var feedEntity = collection.Objects.QueryFeedModels(feedObject.Uuid)?.FirstOrDefault();
                                        //        if (feedEntity != null)
                                        //        {
                                        //            property.SetValue(response, ChangeType(feedEntity.Message, property.PropertyType));
                                        //        }
                                        //    }
                                        //}

                                        //// Groups
                                        //var groupAttribute = property.GetCustomAttribute<TrakHoundGroupAttribute>();
                                        //if (groupAttribute != null)
                                        //{
                                        //    var groupPath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //    if (!string.IsNullOrEmpty(groupAttribute.BasePath)) groupPath = TrakHoundPath.Combine(objectEntity.Path, groupAttribute.BasePath, property.Name);

                                        //    var groupObject = collection.Objects.QueryObjectsByPath(groupPath)?.FirstOrDefault();
                                        //    if (groupObject != null)
                                        //    {
                                        //        var groupEntities = collection.Objects.QueryGroupModelsByGroup(groupObject.Uuid);
                                        //        if (!groupEntities.IsNullOrEmpty())
                                        //        {
                                        //            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                        //            {
                                        //                var childObjects = new List<object>();
                                        //                var childType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                                        //                if (childType != null)
                                        //                {
                                        //                    foreach (var groupEntity in groupEntities)
                                        //                    {
                                        //                        var childResults = DeserializeObjectsToResult(childType, collection, outputModels, new string[] { groupEntity.MemberUuid });
                                        //                        if (childResults != null)
                                        //                        {
                                        //                            foreach (var childResult in childResults) childObjects.Add(childResult.Value);
                                        //                        }
                                        //                    }

                                        //                    if (!childObjects.IsNullOrEmpty())
                                        //                    {
                                        //                        var genericListType = typeof(List<>).MakeGenericType(childType);
                                        //                        var genericList = Activator.CreateInstance(genericListType);

                                        //                        foreach (var childObject in childObjects)
                                        //                        {
                                        //                            ((IList)genericList).Add(ChangeType(childObject, childType));
                                        //                        }

                                        //                        property.SetValue(response, genericList);
                                        //                    }
                                        //                }
                                        //            }
                                        //            else
                                        //            {
                                        //                if (property.PropertyType == typeof(string))
                                        //                {
                                        //                    property.SetValue(response, ChangeType(groupEntities.FirstOrDefault()?.Member?.Path, property.PropertyType));
                                        //                }
                                        //                else
                                        //                {

                                        //                }
                                        //            }
                                        //        }
                                        //    }
                                        //}

                                        // Hashes
                                        var hashAttribute = GetAttribute<TrakHoundHashAttribute>(property);
                                        if (hashAttribute != null)
                                        {
                                            var hashName = property.Name;
                                            if (!string.IsNullOrEmpty(hashAttribute.Name)) hashName = hashAttribute.Name;

                                            var hashPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), hashName);
                                            if (!string.IsNullOrEmpty(hashAttribute.BasePath)) hashPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), hashAttribute.BasePath, hashName);
                                            if (!string.IsNullOrEmpty(hashAttribute.Path))
                                            {
                                                hashPath = TrakHoundPath.IsAbsolute(hashAttribute.Path) ? hashAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), hashAttribute.Path);
                                            }

                                            var hashObject = collection.Objects.QueryObjects(hashPath)?.FirstOrDefault();
                                            if (hashObject != null)
                                            {
                                                //var hashEntities = collection.Objects.QueryHashes(hashObject.Uuid);
                                                //if (!hashEntities.IsNullOrEmpty())
                                                //{
                                                //    SetProperty(property, response, hashEntities.ToDictionary(o => o.Key, o => o.Value));
                                                //}
                                            }
                                        }

                                        // Numbers
                                        var numberAttribute = GetAttribute<TrakHoundNumberAttribute>(property);
                                        if (numberAttribute != null)
                                        {
                                            var numberName = property.Name;
                                            if (!string.IsNullOrEmpty(numberAttribute.Name)) numberName = numberAttribute.Name;

                                            var numberPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), numberName);
                                            if (!string.IsNullOrEmpty(numberAttribute.BasePath)) numberPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), numberAttribute.BasePath, numberName);
                                            if (!string.IsNullOrEmpty(numberAttribute.Path))
                                            {
                                                numberPath = TrakHoundPath.IsAbsolute(numberAttribute.Path) ? numberAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), numberAttribute.Path);
                                            }

                                            var numberObject = collection.Objects.QueryObjects(numberPath)?.FirstOrDefault();
                                            if (numberObject != null)
                                            {
                                                var numberEntity = collection.Objects.QueryNumberByObjectUuid(numberObject.Uuid);
                                                if (numberEntity != null)
                                                {
                                                    SetProperty(property, response, ChangeType(numberEntity.Value, property.PropertyType));
                                                }
                                            }
                                        }

                                        // Observations
                                        var observationAttribute = GetAttribute<TrakHoundObservationAttribute>(property);
                                        if (observationAttribute != null)
                                        {
                                            var observationName = property.Name;
                                            if (!string.IsNullOrEmpty(observationAttribute.Name)) observationName = observationAttribute.Name;

                                            var observationPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), observationName);
                                            if (!string.IsNullOrEmpty(observationAttribute.BasePath)) observationPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), observationAttribute.BasePath, observationName);
                                            if (!string.IsNullOrEmpty(observationAttribute.Path))
                                            {
                                                observationPath = TrakHoundPath.IsAbsolute(observationAttribute.Path) ? observationAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), observationAttribute.Path);
                                            }

                                            var observationObject = collection.Objects.QueryObjects(observationPath)?.FirstOrDefault();
                                            if (observationObject != null)
                                            {
                                                //var observationEntity = collection.Objects.QueryObservationModels(observationObject.Uuid)?.FirstOrDefault();
                                                //if (observationEntity != null)
                                                //{
                                                //    SetProperty(property, response, ChangeType(observationEntity.Value, property.PropertyType));
                                                //}
                                            }
                                        }

                                        //// Queues
                                        //var queueAttribute = property.GetCustomAttribute<TrakHoundQueueAttribute>();
                                        //if (queueAttribute != null)
                                        //{
                                        //    var queuePath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //    if (!string.IsNullOrEmpty(queueAttribute.BasePath)) queuePath = TrakHoundPath.Combine(objectEntity.Path, queueAttribute.BasePath, property.Name);

                                        //    var queueObject = collection.Objects.QueryObjectsByPath(queuePath)?.FirstOrDefault();
                                        //    if (queueObject != null)
                                        //    {
                                        //        var queueEntities = collection.Objects.QueryQueueModels(queueObject.Uuid);
                                        //        if (!queueEntities.IsNullOrEmpty())
                                        //        {
                                        //            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                        //            {
                                        //                var childObjects = new List<object>();
                                        //                var childType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                                        //                if (childType != null)
                                        //                {
                                        //                    foreach (var queueEntity in queueEntities)
                                        //                    {
                                        //                        var childResults = DeserializeObjectsToResult(childType, collection, null, new string[] { queueEntity.MemberUuid });
                                        //                        if (childResults != null)
                                        //                        {
                                        //                            foreach (var childResult in childResults) childObjects.Add(childResult.Value);
                                        //                        }
                                        //                    }

                                        //                    if (!childObjects.IsNullOrEmpty())
                                        //                    {
                                        //                        var genericListType = typeof(List<>).MakeGenericType(childType);
                                        //                        var genericList = Activator.CreateInstance(genericListType);

                                        //                        foreach (var childObject in childObjects)
                                        //                        {
                                        //                            ((IList)genericList).Add(ChangeType(childObject, childType));
                                        //                        }

                                        //                        property.SetValue(response, genericList);
                                        //                    }
                                        //                }
                                        //            }
                                        //            else
                                        //            {
                                        //                if (property.PropertyType == typeof(string))
                                        //                {
                                        //                    property.SetValue(response, ChangeType(queueEntities.FirstOrDefault()?.Member?.Path, property.PropertyType));
                                        //                }
                                        //                else
                                        //                {

                                        //                }
                                        //            }
                                        //        }
                                        //    }
                                        //}

                                        //// Statistics
                                        //var statisticAttribute = property.GetCustomAttribute<TrakHoundStatisticAttribute>();
                                        //if (statisticAttribute != null)
                                        //{
                                        //    var statisticPath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //    if (!string.IsNullOrEmpty(statisticAttribute.BasePath)) statisticPath = TrakHoundPath.Combine(objectEntity.Path, statisticAttribute.BasePath, property.Name);

                                        //    var statisticObject = collection.Objects.QueryObjectsByPath(statisticPath)?.FirstOrDefault();
                                        //    if (statisticObject != null)
                                        //    {
                                        //        // Need to take into account TimeRange

                                        //        var statisticEntity = collection.Objects.QueryStatisticModels(statisticObject.Uuid)?.FirstOrDefault();
                                        //        if (statisticEntity != null)
                                        //        {
                                        //            property.SetValue(response, ChangeType(statisticEntity.Value, property.PropertyType));
                                        //        }
                                        //    }
                                        //}

                                        // References
                                        var referenceAttribute = property.GetCustomAttribute<TrakHoundReferenceAttribute>();
                                        if (referenceAttribute != null)
                                        {
                                            var referenceName = property.Name;
                                            if (!string.IsNullOrEmpty(referenceAttribute.Name)) referenceName = referenceAttribute.Name;

                                            var referencePath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), referenceName);
                                            if (!string.IsNullOrEmpty(referenceAttribute.BasePath)) referencePath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), referenceAttribute.BasePath, referenceName);
                                            if (!string.IsNullOrEmpty(referenceAttribute.Path))
                                            {
                                                referencePath = TrakHoundPath.IsAbsolute(referenceAttribute.Path) ? referenceAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), referenceAttribute.Path);
                                            }

                                            var referenceObject = collection.Objects.QueryObjects(referencePath)?.FirstOrDefault();
                                            if (referenceObject != null)
                                            {
                                                var referenceEntity = collection.Objects.QueryReferenceByObjectUuid(referenceObject.Uuid);
                                                if (referenceEntity != null)
                                                {
                                                    var targetObject = collection.Objects.GetObject(referenceEntity.TargetUuid);

                                                    if (property.PropertyType == typeof(string))
                                                    {
                                                        var targetPath = targetObject?.GetAbsolutePath();
                                                        if (!string.IsNullOrEmpty(referenceAttribute.TargetBasePath)) targetPath = TrakHoundPath.GetRelativeTo(referenceAttribute.TargetBasePath, targetPath);

                                                        SetProperty(property, response, ChangeType(targetPath, property.PropertyType));
                                                    }
                                                    else
                                                    {
                                                        var childEntity = collection.Objects.GetObject(referenceEntity.TargetUuid);
                                                        if (childEntity != null)
                                                        {
                                                            var childResponses = DeserializeObjectsToResult(property.PropertyType, collection, new string[] { childEntity.Uuid }, outputModels, level + 1);
                                                            if (!childResponses.IsNullOrEmpty())
                                                            {
                                                                var childResponse = childResponses.FirstOrDefault();
                                                                if (childResponse.Key != null)
                                                                {
                                                                    if (outputModels != null)
                                                                    {
                                                                        var childResponseKey = $"{property.PropertyType.AssemblyQualifiedName}:{childResponse.Key}";

                                                                        outputModels.Remove(childResponseKey);
                                                                        outputModels.Add(childResponseKey, childResponse.Value);
                                                                    }

                                                                    SetProperty(property, response, childResponse.Value);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // Sets
                                        var setAttribute = GetAttribute<TrakHoundSetAttribute>(property);
                                        if (setAttribute != null)
                                        {
                                            var setName = property.Name;
                                            if (!string.IsNullOrEmpty(setAttribute.Name)) setName = setAttribute.Name;

                                            var setPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), setName);
                                            if (!string.IsNullOrEmpty(setAttribute.BasePath)) setPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), setAttribute.BasePath, setName);
                                            if (!string.IsNullOrEmpty(setAttribute.Path))
                                            {
                                                setPath = TrakHoundPath.IsAbsolute(setAttribute.Path) ? setAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), setAttribute.Path);
                                            }

                                            var setObject = collection.Objects.QueryObjects(setPath)?.FirstOrDefault();
                                            if (setObject != null)
                                            {
                                                //var setEntities = collection.Objects.QuerySets(setObject.Uuid);
                                                //if (!setEntities.IsNullOrEmpty())
                                                //{
                                                //    SetProperty(property, response, setEntities.Select(o => o.Value));
                                                //}
                                            }
                                        }

                                        // States
                                        var stateAttribute = GetAttribute<TrakHoundStateAttribute>(property);
                                        if (stateAttribute != null)
                                        {
                                            var stateName = property.Name;
                                            if (!string.IsNullOrEmpty(stateAttribute.Name)) stateName = stateAttribute.Name;

                                            var statePath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), stateName);
                                            if (!string.IsNullOrEmpty(stateAttribute.BasePath)) statePath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), stateAttribute.BasePath, stateName);
                                            if (!string.IsNullOrEmpty(stateAttribute.Path))
                                            {
                                                statePath = TrakHoundPath.IsAbsolute(stateAttribute.Path) ? stateAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), stateAttribute.Path);
                                            }

                                            var stateObject = collection.Objects.QueryObjects(statePath)?.FirstOrDefault();
                                            if (stateObject != null)
                                            {
                                                var stateEntity = collection.Objects.QueryStatesByObjectUuid(stateObject.Uuid)?.FirstOrDefault();
                                                if (stateEntity != null)
                                                {
                                                    var stateDefinition = collection.Definitions.GetDefinition(stateEntity.DefinitionUuid);
                                                    if (stateDefinition != null)
                                                    {
                                                        SetProperty(property, response, ChangeType(stateDefinition.Type, property.PropertyType));
                                                    }
                                                }
                                            }
                                        }

                                        // Strings
                                        var stringAttribute = GetAttribute<TrakHoundStringAttribute>(property);
                                        if (stringAttribute != null)
                                        {
                                            var stringName = property.Name;
                                            if (!string.IsNullOrEmpty(stringAttribute.Name)) stringName = stringAttribute.Name;

                                            var stringPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), stringName);
                                            if (!string.IsNullOrEmpty(stringAttribute.BasePath)) stringPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), stringAttribute.BasePath, stringName);
                                            if (!string.IsNullOrEmpty(stringAttribute.Path))
                                            {
                                                stringPath = TrakHoundPath.IsAbsolute(stringAttribute.Path) ? stringAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), stringAttribute.Path);
                                            }

                                            var stringObject = collection.Objects.QueryObjects(stringPath)?.FirstOrDefault();
                                            if (stringObject != null)
                                            {
                                                var stringEntity = collection.Objects.QueryStringByObjectUuid(stringObject.Uuid);
                                                if (stringEntity != null)
                                                {
                                                    SetProperty(property, response, ChangeType(stringEntity.Value, property.PropertyType));
                                                }
                                            }
                                        }

                                        // Timestamps
                                        var timestampAttribute = GetAttribute<TrakHoundTimestampAttribute>(property);
                                        if (timestampAttribute != null)
                                        {
                                            var timestampName = property.Name;
                                            if (!string.IsNullOrEmpty(timestampAttribute.Name)) timestampName = timestampAttribute.Name;

                                            var timestampPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), timestampName);
                                            if (!string.IsNullOrEmpty(timestampAttribute.BasePath)) timestampPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), timestampAttribute.BasePath, timestampName);
                                            if (!string.IsNullOrEmpty(timestampAttribute.Path))
                                            {
                                                timestampPath = TrakHoundPath.IsAbsolute(timestampAttribute.Path) ? timestampAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), timestampAttribute.Path);
                                            }

                                            var timestampObject = collection.Objects.QueryObjects(timestampPath)?.FirstOrDefault();
                                            if (timestampObject != null)
                                            {
                                                var timestampEntity = collection.Objects.QueryTimestampByObjectUuid(timestampObject.Uuid);
                                                if (timestampEntity != null)
                                                {
                                                    if (property.PropertyType == typeof(DateTime))
                                                    {
                                                        SetProperty(property, response, timestampEntity.Value.ToDateTime());
                                                    }
                                                    else
                                                    {
                                                        SetProperty(property, response, ChangeType(timestampEntity.Value, property.PropertyType));
                                                    }
                                                }
                                            }
                                        }

                                        // Vocabularies
                                        var vocabularyAttribute = GetAttribute<TrakHoundVocabularyAttribute>(property);
                                        if (vocabularyAttribute != null)
                                        {
                                            var vocabularyName = property.Name;
                                            if (!string.IsNullOrEmpty(vocabularyAttribute.Name)) vocabularyName = vocabularyAttribute.Name;

                                            var vocabularyPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), vocabularyName);
                                            if (!string.IsNullOrEmpty(vocabularyAttribute.BasePath)) vocabularyPath = TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), vocabularyAttribute.BasePath, vocabularyName);
                                            if (!string.IsNullOrEmpty(vocabularyAttribute.Path))
                                            {
                                                vocabularyPath = TrakHoundPath.IsAbsolute(vocabularyAttribute.Path) ? vocabularyAttribute.Path : TrakHoundPath.Combine(objectEntity.GetAbsolutePath(), vocabularyAttribute.Path);
                                            }

                                            var vocabularyObject = collection.Objects.QueryObjects(vocabularyPath)?.FirstOrDefault();
                                            if (vocabularyObject != null)
                                            {
                                                //var vocabularyEntity = collection.Objects.QueryVocabulary(vocabularyObject.Uuid);
                                                //if (vocabularyEntity != null)
                                                //{
                                                //    SetProperty(property, response, ChangeType(vocabularyEntity.Definition?.Type, property.PropertyType));
                                                //}
                                            }
                                        }

                                        //// TimeRanges
                                        //var timeRangeAttribute = property.GetCustomAttribute<TrakHoundTimeRangeAttribute>();
                                        //if (timeRangeAttribute != null)
                                        //{
                                        //    var timeRangePath = TrakHoundPath.Combine(objectEntity.Path, property.Name);
                                        //    if (!string.IsNullOrEmpty(timeRangeAttribute.BasePath)) timeRangePath = TrakHoundPath.Combine(objectEntity.Path, timeRangeAttribute.BasePath, property.Name);

                                        //    var timeRangeObject = collection.Objects.QueryObjectsByPath(timeRangePath)?.FirstOrDefault();
                                        //    if (timeRangeObject != null)
                                        //    {
                                        //        var timeRangeEntity = collection.Objects.QueryTimeRangeModels(timeRangeObject.Uuid)?.FirstOrDefault();
                                        //        if (timeRangeEntity != null)
                                        //        {
                                        //            if (property.PropertyType == typeof(string))
                                        //            {
                                        //                var timeRange = new TimeRange(timeRangeEntity.Start, timeRangeEntity.End);
                                        //                property.SetValue(response, ChangeType(timeRange.ToString(), property.PropertyType));
                                        //            }
                                        //            else if (property.PropertyType == typeof(TimeRange))
                                        //            {
                                        //                var timeRange = new TimeRange(timeRangeEntity.Start, timeRangeEntity.End);
                                        //                property.SetValue(response, timeRange);
                                        //            }
                                        //        }
                                        //    }
                                        //}

                                        // Child Object
                                        var childObjectAttribute = GetAttribute<TrakHoundObjectAttribute>(property);
                                        if (childObjectAttribute != null)
                                        {
                                            var childObjectName = property.Name;
                                            if (!string.IsNullOrEmpty(childObjectAttribute.Name)) childObjectName = childObjectAttribute.Name;

                                            var childPath = childObjectName;
                                            if (!string.IsNullOrEmpty(childObjectAttribute.BasePath)) childPath = TrakHoundPath.Combine(childObjectAttribute.BasePath, childObjectName);
                                            if (!string.IsNullOrEmpty(childObjectAttribute.Path))
                                            {
                                                childPath = childObjectAttribute.Path;
                                                //childPath = TrakHoundObjectPath.IsAbsolute(childObjectAttribute.Path) ? childObjectAttribute.Path : TrakHoundPath.Combine(objectEntity.Path, childObjectAttribute.Path);
                                            }

                                            var childQuery = $"uuid={objectEntity.Uuid}/{childPath}";
                                            childQuery = childQuery.TrimEnd('/');

                                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                            {
                                                childQuery = $"{childQuery}/*";

                                                var childObjects = new Dictionary<string, object>();
                                                var childType = property.PropertyType.GetGenericArguments()?.FirstOrDefault();
                                                if (childType != null)
                                                {
                                                    var baseChildEntities = collection.Objects.QueryObjects(childQuery);
                                                    if (baseChildEntities != null)
                                                    {
                                                        foreach (var childEntity in baseChildEntities)
                                                        {
                                                            if (childEntity.Uuid != null)
                                                            {
                                                                // Initialize childObjects
                                                                var childOutputModel = outputModels?.GetValueOrDefault(childEntity.Uuid);
                                                                if (childOutputModel != null)
                                                                {
                                                                    childObjects.Remove(childEntity.Uuid);
                                                                    childObjects.Add(childEntity.Uuid, childOutputModel);
                                                                }

                                                                var childResponses = DeserializeObjectsToResult(childType, collection, new string[] { childEntity.Uuid }, outputModels, level + 1);
                                                                if (childResponses != null)
                                                                {
                                                                    foreach (var childResponse in childResponses)
                                                                    {
                                                                        if (childResponse.Key != null && childResponse.Value != null)
                                                                        {
                                                                            if (outputModels != null)
                                                                            {
                                                                                var childResponseKey = $"{childType.AssemblyQualifiedName}:{childResponse.Key}";

                                                                                outputModels.Remove(childResponseKey);
                                                                                outputModels.Add(childResponseKey, childResponse.Value);
                                                                            }

                                                                            childObjects.Remove(childEntity.Uuid);
                                                                            childObjects.Add(childEntity.Uuid, childResponse.Value);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (!childObjects.IsNullOrEmpty())
                                                    {
                                                        var genericListType = typeof(List<>).MakeGenericType(childType);
                                                        var genericList = Activator.CreateInstance(genericListType);

                                                        foreach (var childObject in childObjects)
                                                        {
                                                            ((IList)genericList).Add(childObject.Value);
                                                        }

                                                        SetProperty(property, response, genericList);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var childEntity = collection.Objects.QueryObjects(childQuery)?.FirstOrDefault();
                                                if (childEntity != null)
                                                {
                                                    var childResponses = DeserializeObjectsToResult(property.PropertyType, collection, new string[] { childEntity.Uuid }, outputModels, level + 1);
                                                    if (!childResponses.IsNullOrEmpty())
                                                    {
                                                        var childResponse = childResponses.FirstOrDefault();
                                                        if (childResponse.Key != null)
                                                        {
                                                            if (outputModels != null)
                                                            {
                                                                var childResponseKey = $"{property.PropertyType.AssemblyQualifiedName}:{childResponse.Key}";

                                                                outputModels.Remove(childResponseKey);
                                                                outputModels.Add(childResponseKey, childResponse.Value);
                                                            }

                                                            SetProperty(property, response, childResponse.Value);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                       
                                        if (GetAttribute<TrakHoundEntityEntryAttribute>(property) == null)
                                        {
                                            if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
                                            {
                                                var childResponses = DeserializeObjectsToResult(property.PropertyType, collection, new string[] { objectEntity.Uuid }, outputModels, level + 1);
                                                if (!childResponses.IsNullOrEmpty())
                                                {
                                                    var childResponse = childResponses.FirstOrDefault();
                                                    if (childResponse.Key != null)
                                                    {
                                                        if (outputModels != null)
                                                        {
                                                            var childResponseKey = $"{property.PropertyType.AssemblyQualifiedName}:{childResponse.Key}";

                                                            outputModels.Remove(childResponseKey);
                                                            outputModels.Add(childResponseKey, childResponse.Value);
                                                        }

                                                        SetProperty(property, response, childResponse.Value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (response != null) responses.Add(objectEntity.Uuid, response);
                        }
                    }
                }

                return responses;
            }

            return default;
        }

        #endregion

        #region "Properties"

        private static PropertyInfo[] GetProperties(Type type)
        {
            if (type != null)
            {
                var key = $"{type.Assembly.Location}::{type.AssemblyQualifiedName}".ToMD5Hash();

                PropertyInfo[] properties;
                lock (_lock) properties = _typeProperties.GetValueOrDefault(key);
                if (properties == null)
                {
                    properties = type.GetProperties();
                    if (properties != null)
                    {
                        lock (_lock)
                        {
                            _typeProperties.Remove(key);
                            _typeProperties.Add(key, properties);
                        }
                    }
                }

                return properties;
            }

            return null;
        }

        private static PropertyInfo GetProperty<TAttribute>(IEnumerable<PropertyInfo> properties) where TAttribute : Attribute
        {
            if (!properties.IsNullOrEmpty())
            {
                foreach (var property in properties)
                {
                    var match = GetAttribute<TAttribute>(property);
                    if (match != null) return property;
                }
            }

            return null;
        }

        #endregion

        #region "Attributes"

        private static TAttribute GetAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            return GetAttributes<TAttribute>(type)?.FirstOrDefault();
        }

        private static IEnumerable<TAttribute> GetAttributes<TAttribute>(Type type) where TAttribute : Attribute
        {
            if (type != null)
            {
                var key = $"{typeof(TAttribute).AssemblyQualifiedName}:{type.Assembly.Location}::{type.AssemblyQualifiedName}".ToSHA256Hash();

                var attributes = _typeAttributes.Get(key);
                if (attributes.IsNullOrEmpty())
                {
                    attributes = type.GetCustomAttributes(typeof(TAttribute));
                    if (!attributes.IsNullOrEmpty())
                    {
                        _typeAttributes.Add(key, attributes);
                    }
                }

                if (!attributes.IsNullOrEmpty())
                {
                    var results = new List<TAttribute>();
                    foreach (var attribute in attributes)
                    {
                        results.Add((TAttribute)attribute);
                    }
                    return results;
                }
            }

            return null;
        }


        private static TAttribute GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return GetAttributes<TAttribute>(propertyInfo)?.FirstOrDefault();
        }

        private static IEnumerable<TAttribute> GetAttributes<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            if (propertyInfo != null)
            {
                //var key = $"{typeof(TAttribute).AssemblyQualifiedName}:{propertyInfo.ReflectedType.Assembly.Location}:{propertyInfo.ReflectedType.FullName}::{propertyInfo.Name}".ToSHA256Hash();
                var key = $"{typeof(TAttribute).AssemblyQualifiedName}:{propertyInfo.DeclaringType.Assembly.Location}:{propertyInfo.DeclaringType.FullName}::{propertyInfo.Name}".ToSHA256Hash();

                var attributes = _typeAttributes.Get(key);
                if (attributes.IsNullOrEmpty())
                {
                    attributes = propertyInfo.GetCustomAttributes(typeof(TAttribute));
                    if (!attributes.IsNullOrEmpty())
                    {
                        _typeAttributes.Add(key, attributes);
                    }
                }

                if (!attributes.IsNullOrEmpty())
                {
                    var results = new List<TAttribute>();
                    foreach (var attribute in attributes)
                    {
                        results.Add((TAttribute)attribute);
                    }
                    return results;
                }
            }

            return null;
        }

        #endregion


        public static string CreateQuery<T>(string baseExpression)
        {
            return CreateQuery(typeof(T), baseExpression);
        }

        public static string CreateQuery(Type responseType, string baseExpression)
        {
            if (responseType != null && !string.IsNullOrEmpty(baseExpression))
            {
                var targetPathAttribute = GetAttribute<TrakHoundObjectAttribute>(responseType);
                if (targetPathAttribute != null)
                {
                    var query = "";                
                    var target = baseExpression;

                    var selects = CreateContentQueries(responseType);

                    query = "select\r\n";
                    query += string.Join(", \r\n", selects);
                    query += $"\r\nfrom {target}";

                    return query;
                }
            }

            return null;
        }

        public static IEnumerable<TrakHoundEntitySubscriptionRequest> CreateSubscriptionRequests<T>(string baseExpression)
        {
            return CreateSubscriptionRequests(typeof(T), baseExpression);
        }

        public static IEnumerable<TrakHoundEntitySubscriptionRequest> CreateSubscriptionRequests(Type responseType, string baseExpression)
        {
            if (responseType != null && !string.IsNullOrEmpty(baseExpression))
            {
                var targetPathAttribute = GetAttribute<TrakHoundObjectAttribute>(responseType);
                if (targetPathAttribute != null)
                {
                    return CreateContentSubscriptionRequests(responseType, baseExpression);
                }
            }

            return null;
        }


        internal static IEnumerable<string> CreateContentQueries(Type outputType, string baseExpression = null, int level = 0, Type parentType = null)
        {
            var queries = new List<string>();

            if (outputType != null)
            {
                var basePath = outputType == parentType ? TrakHoundPath.Combine(baseExpression.TrimEnd('*'), "**") : baseExpression;

                var properties = outputType.GetProperties();
                if (!properties.IsNullOrEmpty())
                {
                    foreach (var property in properties)
                    {
                        // Assignments
                        var assignmentAttribute = GetAttribute<TrakHoundAssignmentAttribute>(property);
                        if (assignmentAttribute != null)
                        {
                            var assignmentName = property.Name;
                            if (!string.IsNullOrEmpty(assignmentAttribute.Name)) assignmentName = assignmentAttribute.Name;

                            var assignmentPath = TrakHoundPath.Combine(basePath, assignmentName);
                            if (!string.IsNullOrEmpty(assignmentAttribute.BasePath)) assignmentPath = TrakHoundPath.Combine(basePath, assignmentAttribute.BasePath, assignmentName);
                            if (!string.IsNullOrEmpty(assignmentAttribute.Path))
                            {
                                assignmentPath = TrakHoundPath.IsAbsolute(assignmentAttribute.Path) ? assignmentAttribute.Path : TrakHoundPath.Combine(basePath, assignmentAttribute.Path);
                            }

                            queries.Add(assignmentPath);
                        }

                        // Booleans
                        var booleanAttribute = GetAttribute<TrakHoundBooleanAttribute>(property);
                        if (booleanAttribute != null)
                        {
                            var booleanName = property.Name;
                            if (!string.IsNullOrEmpty(booleanAttribute.Name)) booleanName = booleanAttribute.Name;

                            var booleanPath = TrakHoundPath.Combine(basePath, booleanName);
                            if (!string.IsNullOrEmpty(booleanAttribute.BasePath)) booleanPath = TrakHoundPath.Combine(basePath, booleanAttribute.BasePath, booleanName);
                            if (!string.IsNullOrEmpty(booleanAttribute.Path))
                            {
                                booleanPath = TrakHoundPath.IsAbsolute(booleanAttribute.Path) ? booleanAttribute.Path : TrakHoundPath.Combine(basePath, booleanAttribute.Path);
                            }

                            queries.Add(booleanPath);
                        }

                        // Durations
                        var durationAttribute = GetAttribute<TrakHoundDurationAttribute>(property);
                        if (durationAttribute != null)
                        {
                            var durationName = property.Name;
                            if (!string.IsNullOrEmpty(durationAttribute.Name)) durationName = durationAttribute.Name;

                            var durationPath = TrakHoundPath.Combine(basePath, durationName);
                            if (!string.IsNullOrEmpty(durationAttribute.BasePath)) durationPath = TrakHoundPath.Combine(basePath, durationAttribute.BasePath, durationName);
                            if (!string.IsNullOrEmpty(durationAttribute.Path))
                            {
                                durationPath = TrakHoundPath.IsAbsolute(durationAttribute.Path) ? durationAttribute.Path : TrakHoundPath.Combine(basePath, durationAttribute.Path);
                            }

                            queries.Add(durationPath);
                        }

                        // Events
                        var eventAttribute = GetAttribute<TrakHoundEventAttribute>(property);
                        if (eventAttribute != null)
                        {
                            var eventName = property.Name;
                            if (!string.IsNullOrEmpty(eventAttribute.Name)) eventName = eventAttribute.Name;

                            var eventPath = TrakHoundPath.Combine(basePath, eventName);
                            if (!string.IsNullOrEmpty(eventAttribute.BasePath)) eventPath = TrakHoundPath.Combine(basePath, eventAttribute.BasePath, eventName);
                            if (!string.IsNullOrEmpty(eventAttribute.Path))
                            {
                                eventPath = TrakHoundPath.IsAbsolute(eventAttribute.Path) ? eventAttribute.Path : TrakHoundPath.Combine(basePath, eventAttribute.Path);
                            }

                            queries.Add(eventPath);
                        }

                        // Hashes
                        var hashAttribute = GetAttribute<TrakHoundHashAttribute>(property);
                        if (hashAttribute != null)
                        {
                            var hashName = property.Name;
                            if (!string.IsNullOrEmpty(hashAttribute.Name)) hashName = hashAttribute.Name;

                            var hashPath = TrakHoundPath.Combine(basePath, hashName);
                            if (!string.IsNullOrEmpty(hashAttribute.BasePath)) hashPath = TrakHoundPath.Combine(basePath, hashAttribute.BasePath, hashName);
                            if (!string.IsNullOrEmpty(hashAttribute.Path))
                            {
                                hashPath = TrakHoundPath.IsAbsolute(hashAttribute.Path) ? hashAttribute.Path : TrakHoundPath.Combine(basePath, hashAttribute.Path);
                            }

                            queries.Add(hashPath);
                        }

                        // Numbers
                        var numberAttribute = GetAttribute<TrakHoundNumberAttribute>(property);
                        if (numberAttribute != null)
                        {
                            var numberName = property.Name;
                            if (!string.IsNullOrEmpty(numberAttribute.Name)) numberName = numberAttribute.Name;

                            var numberPath = TrakHoundPath.Combine(basePath, numberName);
                            if (!string.IsNullOrEmpty(numberAttribute.BasePath)) numberPath = TrakHoundPath.Combine(basePath, numberAttribute.BasePath, numberName);
                            if (!string.IsNullOrEmpty(numberAttribute.Path))
                            {
                                numberPath = TrakHoundPath.IsAbsolute(numberAttribute.Path) ? numberAttribute.Path : TrakHoundPath.Combine(basePath, numberAttribute.Path);
                            }

                            queries.Add(numberPath);
                        }

                        // Observations
                        var observationAttribute = GetAttribute<TrakHoundObservationAttribute>(property);
                        if (observationAttribute != null)
                        {
                            var observationName = property.Name;
                            if (!string.IsNullOrEmpty(observationAttribute.Name)) observationName = observationAttribute.Name;

                            var observationPath = TrakHoundPath.Combine(basePath, observationName);
                            if (!string.IsNullOrEmpty(observationAttribute.BasePath)) observationPath = TrakHoundPath.Combine(basePath, observationAttribute.BasePath, observationName);
                            if (!string.IsNullOrEmpty(observationAttribute.Path))
                            {
                                observationPath = observationAttribute.Path;
                                //observationPath = TrakHoundObjectPath.IsAbsolute(observationAttribute.Path) ? observationAttribute.Path : TrakHoundPath.Combine(basePath, observationAttribute.Path);
                            }

                            queries.Add(observationPath);
                        }

                        // References
                        var referenceAttribute = GetAttribute<TrakHoundReferenceAttribute>(property);
                        if (referenceAttribute != null)
                        {
                            var referenceName = property.Name;
                            if (!string.IsNullOrEmpty(referenceAttribute.Name)) referenceName = referenceAttribute.Name;

                            var referencePath = TrakHoundPath.Combine(basePath, referenceName);
                            if (!string.IsNullOrEmpty(referenceAttribute.BasePath)) referencePath = TrakHoundPath.Combine(basePath, referenceAttribute.BasePath, referenceName);
                            if (!string.IsNullOrEmpty(referenceAttribute.Path))
                            {
                                referencePath = TrakHoundPath.IsAbsolute(referenceAttribute.Path) ? referenceAttribute.Path : TrakHoundPath.Combine(basePath, referenceAttribute.Path);
                            }

                            queries.Add(referencePath);
                        }

                        // Sets
                        var setAttribute = GetAttribute<TrakHoundSetAttribute>(property);
                        if (setAttribute != null)
                        {
                            var setName = property.Name;
                            if (!string.IsNullOrEmpty(setAttribute.Name)) setName = setAttribute.Name;

                            var setPath = TrakHoundPath.Combine(basePath, setName);
                            if (!string.IsNullOrEmpty(setAttribute.BasePath)) setPath = TrakHoundPath.Combine(basePath, setAttribute.BasePath, setName);
                            if (!string.IsNullOrEmpty(setAttribute.Path))
                            {
                                setPath = TrakHoundPath.IsAbsolute(setAttribute.Path) ? setAttribute.Path : TrakHoundPath.Combine(basePath, setAttribute.Path);
                            }

                            queries.Add(setPath);
                        }

                        // States
                        var stateAttribute = GetAttribute<TrakHoundStateAttribute>(property);
                        if (stateAttribute != null)
                        {
                            var stateName = property.Name;
                            if (!string.IsNullOrEmpty(stateAttribute.Name)) stateName = stateAttribute.Name;

                            var statePath = TrakHoundPath.Combine(basePath, stateName);
                            if (!string.IsNullOrEmpty(stateAttribute.BasePath)) statePath = TrakHoundPath.Combine(basePath, stateAttribute.BasePath, stateName);
                            if (!string.IsNullOrEmpty(stateAttribute.Path))
                            {
                                statePath = TrakHoundPath.IsAbsolute(stateAttribute.Path) ? stateAttribute.Path : TrakHoundPath.Combine(basePath, stateAttribute.Path);
                            }

                            queries.Add(statePath);
                        }

                        // Strings
                        var stringAttribute = GetAttribute<TrakHoundStringAttribute>(property);
                        if (stringAttribute != null)
                        {
                            var stringName = property.Name;
                            if (!string.IsNullOrEmpty(stringAttribute.Name)) stringName = stringAttribute.Name;

                            var stringPath = TrakHoundPath.Combine(basePath, stringName);             
                            if (!string.IsNullOrEmpty(stringAttribute.BasePath)) stringPath = TrakHoundPath.Combine(basePath, stringAttribute.BasePath, stringName);
                            if (!string.IsNullOrEmpty(stringAttribute.Path))
                            {
                                stringPath = TrakHoundPath.IsAbsolute(stringAttribute.Path) ? stringAttribute.Path : TrakHoundPath.Combine(basePath, stringAttribute.Path);
                            }

                            queries.Add(stringPath);
                        }

                        // Timestamps
                        var timestampAttribute = GetAttribute<TrakHoundTimestampAttribute>(property);
                        if (timestampAttribute != null)
                        {
                            var timestampName = property.Name;
                            if (!string.IsNullOrEmpty(timestampAttribute.Name)) timestampName = timestampAttribute.Name;

                            var timestampPath = TrakHoundPath.Combine(basePath, timestampName);
                            if (!string.IsNullOrEmpty(timestampAttribute.BasePath)) timestampPath = TrakHoundPath.Combine(basePath, timestampAttribute.BasePath, timestampName);
                            if (!string.IsNullOrEmpty(timestampAttribute.Path))
                            {
                                timestampPath = TrakHoundPath.IsAbsolute(timestampAttribute.Path) ? timestampAttribute.Path : TrakHoundPath.Combine(basePath, timestampAttribute.Path);
                            }

                            queries.Add(timestampPath);
                        }

                        // Vocabularies
                        var vocabularyAttribute = GetAttribute<TrakHoundVocabularyAttribute>(property);
                        if (vocabularyAttribute != null)
                        {
                            var vocabularyName = property.Name;
                            if (!string.IsNullOrEmpty(vocabularyAttribute.Name)) vocabularyName = vocabularyAttribute.Name;

                            var vocabularyPath = TrakHoundPath.Combine(basePath, vocabularyName);
                            if (!string.IsNullOrEmpty(vocabularyAttribute.BasePath)) vocabularyPath = TrakHoundPath.Combine(basePath, vocabularyAttribute.BasePath, vocabularyName);
                            if (!string.IsNullOrEmpty(vocabularyAttribute.Path))
                            {
                                vocabularyPath = TrakHoundPath.IsAbsolute(vocabularyAttribute.Path) ? vocabularyAttribute.Path : TrakHoundPath.Combine(basePath, vocabularyAttribute.Path);
                            }

                            queries.Add(vocabularyPath);
                        }

                        // Child Objects
                        var childObjectAttribute = GetAttribute<TrakHoundObjectAttribute>(property);
                        if (childObjectAttribute != null)
                        {
                            var childObjectName = property.Name;
                            if (!string.IsNullOrEmpty(childObjectAttribute.Name)) childObjectName = childObjectAttribute.Name;

                            var childObjectPath = childObjectName;
                            if (!string.IsNullOrEmpty(childObjectAttribute.BasePath)) childObjectPath = TrakHoundPath.Combine(childObjectAttribute.BasePath, childObjectName);
                            if (!string.IsNullOrEmpty(childObjectAttribute.Path))
                            {
                                childObjectPath = childObjectAttribute.Path; ;
                                //childObjectPath = TrakHoundObjectPath.IsAbsolute(childObjectAttribute.Path) ? childObjectAttribute.Path : TrakHoundPath.Combine(basePath, childObjectAttribute.Path);
                            }

                            Type childObjectType;
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) childObjectType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                            else childObjectType = property.PropertyType;

                            if (childObjectType != outputType)
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                {
                                    childObjectPath = TrakHoundPath.Combine(basePath, childObjectPath, "*"); // Wildcard?
                                }
                                else
                                {
                                    childObjectPath = TrakHoundPath.Combine(basePath, childObjectPath);
                                }
                            }
                            else
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                {
                                    childObjectPath = TrakHoundPath.Combine(basePath.TrimEnd('*'), "**", childObjectPath, "*"); // Wildcard?
                                }
                                else
                                {
                                    childObjectPath = TrakHoundPath.Combine(basePath.TrimEnd('*'), "**", childObjectPath);
                                }
                            }


                            if (outputType != parentType || outputType != childObjectType)
                            {
                                queries.Add(childObjectPath);
                                queries.AddRange(CreateContentQueries(childObjectType, childObjectPath, level + 1, outputType));
                            }
                        }
                    }
                }
            }

            return queries;
        }

        private static IEnumerable<TrakHoundEntitySubscriptionRequest> CreateContentSubscriptionRequests(Type outputType, string baseExpression = null, int level = 0)
        {
            var requests = new List<TrakHoundEntitySubscriptionRequest>();

            if (outputType != null)
            {
                var properties = outputType.GetProperties();
                if (!properties.IsNullOrEmpty())
                {
                    foreach (var property in properties)
                    {
                        // Assignments
                        var assignmentAttribute = GetAttribute<TrakHoundAssignmentAttribute>(property);
                        if (assignmentAttribute != null)
                        {
                            var assignmentName = property.Name;
                            if (!string.IsNullOrEmpty(assignmentAttribute.Name)) assignmentName = assignmentAttribute.Name;

                            var assignmentPath = TrakHoundPath.Combine(baseExpression, assignmentName);
                            if (!string.IsNullOrEmpty(assignmentAttribute.BasePath)) assignmentPath = TrakHoundPath.Combine(baseExpression, assignmentAttribute.BasePath, assignmentName);
                            if (!string.IsNullOrEmpty(assignmentAttribute.Path))
                            {
                                assignmentPath = TrakHoundPath.IsAbsolute(assignmentAttribute.Path) ? assignmentAttribute.Path : TrakHoundPath.Combine(baseExpression, assignmentAttribute.Path);
                            }

                            requests.Add(new TrakHoundEntitySubscriptionRequest(TrakHoundEntityCategory.Objects.ToString(), TrakHoundObjectsEntityClass.Assignment.ToString(), assignmentPath));
                        }

                        // Events
                        var eventAttribute = GetAttribute<TrakHoundEventAttribute>(property);
                        if (eventAttribute != null)
                        {
                            var eventName = property.Name;
                            if (!string.IsNullOrEmpty(eventAttribute.Name)) eventName = eventAttribute.Name;

                            var eventPath = TrakHoundPath.Combine(baseExpression, eventName);
                            if (!string.IsNullOrEmpty(eventAttribute.BasePath)) eventPath = TrakHoundPath.Combine(baseExpression, eventAttribute.BasePath, eventName);
                            if (!string.IsNullOrEmpty(eventAttribute.Path))
                            {
                                eventPath = TrakHoundPath.IsAbsolute(eventAttribute.Path) ? eventAttribute.Path : TrakHoundPath.Combine(baseExpression, eventAttribute.Path);
                            }

                            requests.Add(new TrakHoundEntitySubscriptionRequest(TrakHoundEntityCategory.Objects.ToString(), TrakHoundObjectsEntityClass.Event.ToString(), eventPath));
                        }

                        // Observations
                        var observationAttribute = GetAttribute<TrakHoundObservationAttribute>(property);
                        if (observationAttribute != null)
                        {
                            var observationName = property.Name;
                            if (!string.IsNullOrEmpty(observationAttribute.Name)) observationName = observationAttribute.Name;

                            var observationPath = TrakHoundPath.Combine(baseExpression, observationName);
                            if (!string.IsNullOrEmpty(observationAttribute.BasePath)) observationPath = TrakHoundPath.Combine(baseExpression, observationAttribute.BasePath, observationName);
                            if (!string.IsNullOrEmpty(observationAttribute.Path))
                            {
                                observationPath = TrakHoundPath.IsAbsolute(observationAttribute.Path) ? observationAttribute.Path : TrakHoundPath.Combine(baseExpression, observationAttribute.Path);
                            }

                            requests.Add(new TrakHoundEntitySubscriptionRequest(TrakHoundEntityCategory.Objects.ToString(), TrakHoundObjectsEntityClass.Observation.ToString(), observationPath));
                        }

                        // States
                        var stateAttribute = GetAttribute<TrakHoundStateAttribute>(property);
                        if (stateAttribute != null)
                        {
                            var stateName = property.Name;
                            if (!string.IsNullOrEmpty(stateAttribute.Name)) stateName = stateAttribute.Name;

                            var statePath = TrakHoundPath.Combine(baseExpression, stateName);
                            if (!string.IsNullOrEmpty(stateAttribute.BasePath)) statePath = TrakHoundPath.Combine(baseExpression, stateAttribute.BasePath, stateName);
                            if (!string.IsNullOrEmpty(stateAttribute.Path))
                            {
                                statePath = TrakHoundPath.IsAbsolute(stateAttribute.Path) ? stateAttribute.Path : TrakHoundPath.Combine(baseExpression, stateAttribute.Path);
                            }

                            requests.Add(new TrakHoundEntitySubscriptionRequest(TrakHoundEntityCategory.Objects.ToString(), TrakHoundObjectsEntityClass.State.ToString(), statePath));
                        }

                        // Child Objects
                        var childObjectAttribute = GetAttribute<TrakHoundObjectAttribute>(property);
                        if (childObjectAttribute != null)
                        {
                            var childObjectPath = property.Name;
                            if (!string.IsNullOrEmpty(childObjectAttribute.BasePath)) childObjectPath = childObjectAttribute.BasePath;
                            //if (!string.IsNullOrEmpty(childObjectAttribute.BasePath)) childObjectPath = TrakHoundPath.Combine(childObjectAttribute.BasePath, property.Name);
                            childObjectPath = TrakHoundPath.Combine(baseExpression, childObjectPath);

                            Type childObjectType;
                            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) childObjectType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                            else childObjectType = property.PropertyType;

                            requests.AddRange(CreateContentSubscriptionRequests(childObjectType, childObjectPath, level + 1));
                        }
                    }
                }
            }

            return requests;
        }


        private static string GetObjectPathAttribute(Type type)
        {
            if (type != null)
            {
                var objectAttribute = GetAttribute<TrakHoundObjectAttribute>(type);
                if (objectAttribute != null)
                {
                    return objectAttribute.BasePath;
                }
            }

            return null;
        }

        private static TrakHoundDefinitionEntry CreateDefinition(PropertyInfo property, string basePath, string relativePath, ref Dictionary<byte[], ITrakHoundEntityEntryOperation> entries)
        {
            // Property Definition Attribute
            var definitionAttribute = GetAttribute<TrakHoundDefinitionAttribute>(property);
            if (definitionAttribute != null)
            {
                // Create Definition Entry
                var definitionRequest = new TrakHoundDefinitionEntry();

                // Set Description
                if (!string.IsNullOrEmpty(definitionAttribute.Description))
                {
                    definitionRequest.Descriptions.Add(TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode, definitionAttribute.Description);
                }

                entries.Remove(definitionRequest.EntryId);
                entries.Add(definitionRequest.EntryId, definitionRequest);

                return definitionRequest;
            }

            return null;
        }

        private static string GetDefinitionId(string basePath)
        {
            if (!string.IsNullOrEmpty(basePath))
            {
                return string.Join('.', basePath.Trim('.').Split('/').Select(o => o.UppercaseFirstCharacter()));
            }

            return null;
        }

        private static string GetDefinitionId(string basePath, string relativePath)
        {
            var definitionBasePath = "";
            if (!string.IsNullOrEmpty(basePath))
            {
                definitionBasePath = GetDefinitionId(basePath);
            }

            var definitionRelativePath = "";
            if (!string.IsNullOrEmpty(relativePath))
            {
                definitionRelativePath = GetDefinitionId(relativePath);
            }

            return TrakHoundType.Combine(definitionBasePath, definitionRelativePath);
        }

        private static string GetDefinitionId(string basePath, string relativePath, string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var definitionBasePath = "";
                if (!string.IsNullOrEmpty(basePath))
                {
                    definitionBasePath = GetDefinitionId(basePath);
                }

                var definitionRelativePath = "";
                if (!string.IsNullOrEmpty(relativePath))
                {
                    definitionRelativePath = GetDefinitionId(relativePath);
                }

                return TrakHoundType.Combine(definitionBasePath, definitionRelativePath, type);
            }

            return null;
        }

        private static string GetPropertyDefinitionId(
            ref Dictionary<byte[], ITrakHoundEntityEntryOperation> entries,
            string parentDefinitionId,
            string type,
            string description = null,
            string languageCode = TrakHoundDefinitionDescriptionEntity.DefaultLanguageCode,
            string parentId = null
            )
        {
            if (!string.IsNullOrEmpty(parentDefinitionId) && !string.IsNullOrEmpty(type))
            {
                var definitionId = TrakHoundType.Combine(parentDefinitionId, type);

                // Create Definition Entry
                var definitionRequest = new TrakHoundDefinitionEntry();
                definitionRequest.Id = definitionId;
                definitionRequest.ParentId = parentId;

                // Set Description
                if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(languageCode))
                {
                    definitionRequest.Descriptions.Add(languageCode, description);
                }

                entries.Remove(definitionRequest.EntryId);
                entries.Add(definitionRequest.EntryId, definitionRequest);

                return definitionId;
            }

            return null;
        }


        private static T CreateInstance<T>()
        {
            try
            {
                var constructor = typeof(T).GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    return (T)constructor.Invoke(new object[] { });
                }
            }
            catch (Exception ex)
            {

            }

            return default;
        }

        private static object CreateInstance(Type type)
        {
            try
            {
                var constructor = type.GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    return constructor.Invoke(new object[] { });
                }
            }
            catch (Exception ex)
            {

            }

            return default;
        }

        private static T ChangeType<T>(string str)
        {
            if (str != null)
            {
                try
                {
                    return (T)Convert.ChangeType(str, typeof(T));
                }
                catch { }
            }

            return default;
        }

        private static void SetProperty(PropertyInfo property, object obj, object value)
        {
            if (property != null && obj != null)
            {
                var debugType = obj.GetType();

                try
                {
                    property.SetValue(obj, value);
                }
                catch { }
            }
        }

        private static object ChangeType(object obj, Type type)
        {
            if (obj != null)
            {
                var convertType = type;

                if (type.IsValueType && Nullable.GetUnderlyingType(type) != null)
                {
                    convertType = Nullable.GetUnderlyingType(type);
                }

                try
                {
                    if (convertType == typeof(DateTime))
                    {
                        return obj.ToString().ToDateTime();
                    }
                    else if (typeof(Enum).IsAssignableFrom(convertType) && obj.GetType() == typeof(string))
                    {
                        if (Enum.TryParse(convertType, (string)obj, true, out var result))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return Convert.ChangeType(obj, convertType);
                    }
                }
                catch { }
            }

            return GetDefaultValue(type);
        }

        private static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch { }
            }

            return null;
        }

        private static IEnumerable<ITrakHoundDefinitionEntity> GetDefinitions(TrakHoundEntityCollection collection, string definitionUuid)
        {
            var definitions = new List<ITrakHoundDefinitionEntity>();

            if (collection != null && !string.IsNullOrEmpty(definitionUuid))
            {
                var definition = collection.Definitions.GetDefinition(definitionUuid);
                if (definition != null)
                {
                    definitions.Add(definition);

                    if (!string.IsNullOrEmpty(definition.ParentUuid))
                    {
                        definitions.AddRange(GetDefinitions(collection, definition.ParentUuid));
                    }
                }
            }

            return definitions;
        }

        public static bool MatchDefinitionType(string type, string pattern)
        {
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(pattern))
            {
                var typeParts = type.ToLower().Split('.');
                var patternParts = pattern.ToLower().Split('.');
                string previousPatternPart = null;

                if (patternParts.Length > typeParts.Length) return false;

                for (var i = 0; i < typeParts.Length; i++)
                {
                    if (i < patternParts.Length)
                    {
                        var typePart = typeParts[i];
                        var patternPart = patternParts[i];

                        if (typePart != patternPart && patternPart != "*")
                        {
                            return false;
                        }

                        previousPatternPart = patternPart;
                    }
                    else
                    {
                        if (previousPatternPart != "*") return false;
                        break;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
