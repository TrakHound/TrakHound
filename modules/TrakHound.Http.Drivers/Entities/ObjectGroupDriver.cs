// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectGroupDriver :
        HttpEntityDriver<ITrakHoundObjectGroupEntity>,
        IObjectGroupQueryDriver
    {
        public ObjectGroupDriver() { }

        public ObjectGroupDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByGroup(IEnumerable<string> groupUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectGroupEntity>>();

            if (!groupUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Group.QueryByGroupUuid(groupUuids);
                var dEntities = entities?.ToListDictionary(o => o.GroupUuid);

                foreach (var groupUuid in groupUuids)
                {
                    var targetEntities = dEntities?.Get(groupUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, groupUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, groupUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectGroupEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectGroupEntity>> QueryByMember(IEnumerable<string> memberUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectGroupEntity>>();

            if (!memberUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Group.QueryByMemberUuid(memberUuids);
                var dEntities = entities?.ToListDictionary(o => o.GroupUuid);

                foreach (var memberUuid in memberUuids)
                {
                    var targetEntities = dEntities?.Get(memberUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, memberUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, memberUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectGroupEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectGroupEntity>(results);
        }
    }
}
