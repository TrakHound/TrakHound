// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using TrakHound.Buffers;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Routing
{
    public partial class TrakHoundRouterProvider
    {
         public void InitializeBuffers()
        {
            if (_bufferProvider != null && _driverProvider != null && !_driverProvider.Drivers.IsNullOrEmpty())
            {
                var driverConfigurationIds = _driverProvider.Drivers.Select(o => o.Configuration.Id).Distinct().ToList();
                foreach (var driverConfigurationId in driverConfigurationIds)
                {

                    AddEntityBuffers<ITrakHoundObjectEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectMetadataEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectAssignmentEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectBlobEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectBooleanEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectDurationEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectEventEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectGroupEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectHashEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectLogEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectMessageEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectMessageQueueEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectNumberEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectObservationEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectQueueEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectReferenceEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectSetEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectStateEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectStatisticEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectStringEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectTimeRangeEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectTimestampEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectVocabularyEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundObjectVocabularySetEntity>(driverConfigurationId);


                    AddEntityBuffers<ITrakHoundDefinitionEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundDefinitionMetadataEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundDefinitionDescriptionEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundDefinitionWikiEntity>(driverConfigurationId);


                    AddEntityBuffers<ITrakHoundSourceEntity>(driverConfigurationId);
                    AddEntityBuffers<ITrakHoundSourceMetadataEntity>(driverConfigurationId);
                }
            }
        }
    }
}
