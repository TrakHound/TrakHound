// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundAssignment>> GetAssignments(string assigneePath, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);
        
        Task<IEnumerable<TrakHoundAssignment>> GetAssignments(IEnumerable<string> assigneePaths, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);
        
        Task<IEnumerable<TrakHoundAssignment>> GetAssignments(string assigneePath, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);
        
        Task<IEnumerable<TrakHoundAssignment>> GetAssignments(IEnumerable<string> assigneePaths, DateTime start, DateTime stop, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);
       
        Task<IEnumerable<TrakHoundAssignment>> GetAssignments(string assigneePath, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);
        
        Task<IEnumerable<TrakHoundAssignment>> GetAssignments(IEnumerable<string> assigneePaths, string startExpression, string stopExpression, int skip = _defaultSkip, int take = _defaultTake, SortOrder sortOrder = _defaultSortOrder, string routerId = null);


        Task<IEnumerable<TrakHoundAssignment>> GetCurrentAssignments(string assigneePath, string routerId = null);

        Task<IEnumerable<TrakHoundAssignment>> GetCurrentAssignments(IEnumerable<string> assigneePaths, string routerId = null);


        Task<ITrakHoundConsumer<IEnumerable<TrakHoundAssignment>>> SubscribeAssignments(string assigneePath, string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<TrakHoundAssignment>>> SubscribeAssignments(IEnumerable<string> assigneePaths, string routerId = null);


        Task<bool> PublishAssignment(string assigneePath, string member, DateTime? addTimestamp = null, DateTime? removeTimestamp = null, bool async = false, string routerId = null);

        Task<bool> PublishAssignments(IEnumerable<TrakHoundAssignmentEntry> entries, bool async = false, string routerId = null);


        Task<bool> RemoveAssignment(string assigneePath, string memberPath, DateTime? removeTimestamp = null, bool async = false, string routerId = null);
    }
}
