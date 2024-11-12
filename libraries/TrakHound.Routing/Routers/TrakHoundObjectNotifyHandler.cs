// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Entities.Filters;

namespace TrakHound.Routing.Routers
{
    internal class TrakHoundObjectNotifyHandler
	{
		private readonly ITrakHoundClient _client;
		private readonly int _interval;
		private readonly int _slowInterval;
		private readonly int _limit;
		private readonly Dictionary<string, TrakHoundObjectNotifySubscription> _subscriptions = new Dictionary<string, TrakHoundObjectNotifySubscription>();
		private readonly TrakHoundEntityPatternFilter _createdFilter;
		private readonly TrakHoundEntityPatternFilter _changedFilter;
		private readonly TrakHoundEntityPatternFilter _componentChangedFilter;
		private readonly ItemIntervalQueue<string> _createdQueue;
		private readonly ItemIntervalQueue<string> _createdSlowQueue;
		private readonly ItemIntervalQueue<string> _changedQueue;
		private readonly ItemIntervalQueue<string> _changedSlowQueue;
        private readonly ItemIntervalQueue<string> _componentChangedQueue;
        private readonly ItemIntervalQueue<string> _componentChangedSlowQueue;
        private readonly ItemIntervalQueue<ITrakHoundObjectEntity> _deletedQueue;
		private readonly ItemIntervalQueue<ITrakHoundObjectEntity> _deletedSlowQueue;
		private readonly object _lock = new object();


		struct QueryNotification
		{
			public string Query { get; }

			public TrakHoundEntityNotification Notification { get; }

			public QueryNotification(string query, TrakHoundEntityNotification notification)
			{
				Query = query;
				Notification = notification;
			}	
		}


		public TrakHoundObjectNotifyHandler(ITrakHoundClient client, int interval = 500, int limit = 500, int slowInterval = 2500)
		{
			_client = client;
			_interval = interval;
			_slowInterval = slowInterval;
			_limit = limit;

			_createdFilter = new TrakHoundEntityPatternFilter(_client.System.Entities, interval, limit);
			_createdFilter.MatchesReceived += CreatedFilterMatchReceived;

            _changedFilter = new TrakHoundEntityPatternFilter(_client.System.Entities, interval, limit);
            _changedFilter.MatchesReceived += ChangedFilterMatchReceived;

            _componentChangedFilter = new TrakHoundEntityPatternFilter(_client.System.Entities, interval, limit);
            _componentChangedFilter.MatchesReceived += ComponentChangedFilterMatchReceived;

            _createdQueue = new ItemIntervalQueue<string>(interval, limit);
			_createdQueue.ItemsReceived += ProcessCreatedQueue;
            _createdSlowQueue = new ItemIntervalQueue<string>(slowInterval, limit);
            _createdSlowQueue.ItemsReceived += ProcessCreatedQueue;

            _changedQueue = new ItemIntervalQueue<string>(interval, limit);
			_changedQueue.ItemsReceived += ProcessChangedQueue;
            _changedSlowQueue = new ItemIntervalQueue<string>(slowInterval, limit);
            _changedSlowQueue.ItemsReceived += ProcessChangedQueue;

            _componentChangedQueue = new ItemIntervalQueue<string>(interval, limit);
            _componentChangedQueue.ItemsReceived += ProcessComponentChangedQueue;
            _componentChangedSlowQueue = new ItemIntervalQueue<string>(slowInterval, limit);
            _componentChangedSlowQueue.ItemsReceived += ProcessComponentChangedQueue;

            _deletedQueue = new ItemIntervalQueue<ITrakHoundObjectEntity>(interval, limit);
			_deletedQueue.ItemsReceived += ProcessDeletedQueue;
            _deletedSlowQueue = new ItemIntervalQueue<ITrakHoundObjectEntity>(slowInterval, limit);
            _deletedSlowQueue.ItemsReceived += ProcessDeletedQueue;
        }


		public TrakHoundObjectNotifySubscription CreateSubscription(string objectQuery, TrakHoundEntityNotificationType notificationType = TrakHoundEntityNotificationType.All)
		{
			if (!string.IsNullOrEmpty(objectQuery))
			{
				var subscription = new TrakHoundObjectNotifySubscription();
				subscription.Query = objectQuery;
				subscription.Consumer = new TrakHoundConsumer<IEnumerable<TrakHoundEntityNotification>>();

				lock (_lock)
				{
					_subscriptions.Add(subscription.Id, subscription);
				}

				switch (notificationType)
				{
					case TrakHoundEntityNotificationType.All: 
						_createdFilter.Allow(objectQuery);
                        _changedFilter.Allow(objectQuery);
                        _componentChangedFilter.Allow(objectQuery);
                        //_deletedFilter.AddQuery(objectQuery);
						break;

					case TrakHoundEntityNotificationType.Created:
						_createdFilter.Allow(objectQuery);
						break;

					case TrakHoundEntityNotificationType.Changed:
						_changedFilter.Allow(objectQuery);
						break;

                    case TrakHoundEntityNotificationType.ComponentChanged:
                        _componentChangedFilter.Allow(objectQuery);
                        break;

                        //case TrakHoundEntityNotificationType.Deleted: _deletedFilter.AddQuery(objectQuery); break;			
                }

				return subscription;
			}

			return null;
		}


        private void CreatedFilterMatchReceived(object sender, IEnumerable<TrakHoundEntityMatchResult> matches)
        {
            if (!matches.IsNullOrEmpty())
            {
				var queryNotifications = new List<QueryNotification>();

				foreach (var match in matches)
				{
                    // Create Notification
                    var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Created, match.Entity);

                    queryNotifications.Add(new QueryNotification(match.Query, notification));
				}

                // Push Notification to Subscription(s)
                PushNotification(queryNotifications);
            }
        }

        private void ChangedFilterMatchReceived(object sender, IEnumerable<TrakHoundEntityMatchResult> matches)
        {
            if (!matches.IsNullOrEmpty())
            {
                var queryNotifications = new List<QueryNotification>();

                foreach (var match in matches)
                {
                    // Create Notification
                    var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Changed, match.Entity);

                    queryNotifications.Add(new QueryNotification(match.Query, notification));
                }

                // Push Notification to Subscription(s)
                PushNotification(queryNotifications);
            }
        }

        private void ComponentChangedFilterMatchReceived(object sender, IEnumerable<TrakHoundEntityMatchResult> matches)
        {
            if (!matches.IsNullOrEmpty())
            {
                var queryNotifications = new List<QueryNotification>();

                foreach (var match in matches)
                {
                    // Create Notification
                    var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.ComponentChanged, match.Entity);

                    queryNotifications.Add(new QueryNotification(match.Query, notification));
                }

                // Push Notification to Subscription(s)
                PushNotification(queryNotifications);
            }
        }

        //private void CreatedFilterMatchReceived(string query, ITrakHoundEntity entity)
        //{
        //    if (query != null && entity != null)
        //    {
        //        // Create Notification
        //        var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Created, entity.Uuid);

        //        // Push Notification to Consumer
        //        //subscription.Consumer.Push(notification);
        //    }
        //}

        //private void ChangedFilterMatchReceived(string query, ITrakHoundEntity entity)
        //{
        //    if (query != null && entity != null)
        //    {
        //        // Create Notification
        //        var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Changed, entity.Uuid);

        //        // Push Notification to Consumer
        //        //subscription.Consumer.Push(notification);
        //    }
        //}


        //private void CreatedFilterEntityReceived(NotifySubscription subscription, ITrakHoundEntity entity)
        //{
        //	if (subscription != null && entity != null)
        //	{
        //		// Create Notification
        //		var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Created, entity.Uuid);

        //		// Push Notification to Consumer
        //		subscription.Consumer.Push(notification);
        //	}
        //}

        //private void ChangedFilterEntityReceived(NotifySubscription subscription, ITrakHoundEntity entity)
        //{
        //	if (subscription != null && entity != null)
        //	{
        //		// Create Notification
        //		var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Changed, entity.Uuid);

        //		// Push Notification to Consumer
        //		subscription.Consumer.Push(notification);
        //	}
        //}

        //      private void DeletedFilterEntityReceived(NotifySubscription subscription, ITrakHoundEntity entity)
        //{
        //	//if (subscription != null && entity != null)
        //	//{
        //	//	// Create Notification
        //	//	var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Deleted, entity.Uuid);

        //	//	// Push Notification to Consumer
        //	//	subscription.Consumer.Push(notification);
        //	//}
        //}


        private void PushNotification(IEnumerable<QueryNotification> queryNotifications)
		{
			if (!_subscriptions.IsNullOrEmpty())
			{
				foreach (var subscription in _subscriptions.Values)
				{
					var matchedNotifications = queryNotifications.Where(o => o.Query == subscription.Query)?.Select(o => o.Notification);
					if (!matchedNotifications.IsNullOrEmpty())
					{
                        subscription.Consumer.Push(matchedNotifications);
                    }
				}
			}
		}



		public void AddPublishNotification(TrakHoundEntityNotificationType notificationType, ITrakHoundEntity entity)
		{
			if (AnySubscriptions() && entity != null)
			{
				if (notificationType == TrakHoundEntityNotificationType.Created)
				{
					_createdQueue.Add(entity.Uuid);
				}
				else if (notificationType == TrakHoundEntityNotificationType.Changed)
				{
					_changedQueue.Add(entity.Uuid);
                }
                else if (notificationType == TrakHoundEntityNotificationType.ComponentChanged)
                {
                    _componentChangedQueue.Add(entity.Uuid);
                }
            }
		}

		public void AddDeleteNotification(ITrakHoundObjectEntity entityModel)
		{
			if (AnySubscriptions())
			{
				_deletedQueue.Add(entityModel);
			}
		}


		private TrakHoundObjectNotifySubscription GetSubscription(string subscriptionId)
		{
			if (!string.IsNullOrEmpty(subscriptionId))
			{
				lock (_lock)
				{
					if (_subscriptions.ContainsKey(subscriptionId))
					{
						return _subscriptions[subscriptionId];
					}
				}
			}

			return null;
		}

		private bool AnySubscriptions()
		{
			lock (_lock) return !_subscriptions.IsNullOrEmpty();
		}


		private async void ProcessCreatedQueue(object sender, IEnumerable<string> items)
		{
			var uuids = items.Select(o => o).Distinct();

			var entities = await _client.System.Entities.Objects.ReadByUuid(uuids);
			if (!entities.IsNullOrEmpty())
			{
				_createdFilter.MatchAsync(entities);

				//if (!_subscriptions.IsNullOrEmpty())
				//{
				//	foreach (var subscription in _subscriptions.Values)
				//	{
				//		//subscription.CreatedFilter.MatchAsync(entities);
				//	}
				//}
			}
			else
			{
                // Add to the Slow Queue and try again on the next interval
                foreach (var item in items)
                {
                    //Console.WriteLine($"Object Model NOT FOUND :: Item re-added to QUEUE :: {item}");
                    _createdSlowQueue.Add(item);
                }

                //// Add back to the Queue and try again on the next interval
                //foreach (var item in items)
                //{
                //	//Console.WriteLine($"Object Model NOT FOUND :: Item re-added to QUEUE :: {item}");
                //	_createdQueue.Add(item);
                //}
            }
		}

		private async void ProcessChangedQueue(object sender, IEnumerable<string> items)
		{
			var uuids = items.Select(o => o).Distinct();

			var entities = await _client.System.Entities.Objects.ReadByUuid(uuids);
			if (!entities.IsNullOrEmpty())
			{
                _changedFilter.MatchAsync(entities);

                //if (!_subscriptions.IsNullOrEmpty())
                //{
                //	foreach (var subscription in _subscriptions.Values)
                //	{
                //		//subscription.ChangedFilter.MatchAsync(entities);
                //	}
                //}
            }
			else
			{
                // Add to the Slow Queue and try again on the next interval
                foreach (var item in items)
                {
                    //Console.WriteLine($"Object Model NOT FOUND :: Item re-added to QUEUE :: {item}");
                    _changedSlowQueue.Add(item);
                }

                //// Add back to the Queue and try again on the next interval
                //foreach (var item in items)
                //{
                //	//Console.WriteLine($"Object Model NOT FOUND :: Item re-added to QUEUE :: {item}");
                //	_changedQueue.Add(item);
                //}
            }
		}

        private async void ProcessComponentChangedQueue(object sender, IEnumerable<string> items)
        {
            var uuids = items.Select(o => o).Distinct();

            var entities = await _client.System.Entities.Objects.ReadByUuid(uuids);
            if (!entities.IsNullOrEmpty())
            {
                _componentChangedFilter.MatchAsync(entities);

                //if (!_subscriptions.IsNullOrEmpty())
                //{
                //	foreach (var subscription in _subscriptions.Values)
                //	{
                //		//subscription.ChangedFilter.MatchAsync(entities);
                //	}
                //}
            }
            else
            {
                // Add to the Slow Queue and try again on the next interval
                foreach (var item in items)
                {
                    //Console.WriteLine($"Object Model NOT FOUND :: Item re-added to QUEUE :: {item}");
                    _componentChangedSlowQueue.Add(item);
                }

                //// Add back to the Queue and try again on the next interval
                //foreach (var item in items)
                //{
                //	//Console.WriteLine($"Object Model NOT FOUND :: Item re-added to QUEUE :: {item}");
                //	_changedQueue.Add(item);
                //}
            }
        }

        private void ProcessDeletedQueue(object sender, IEnumerable<ITrakHoundObjectEntity> items)
		{
			if (!_subscriptions.IsNullOrEmpty())
			{
				var collection = new TrakHoundEntityCollection();
				collection.Add(items);

				foreach (var subscription in _subscriptions.Values)
				{
                    var notifications = new List<TrakHoundEntityNotification>();

                    var matchedEntities = collection.Objects.QueryObjects(subscription.Query);
					//var matches = TrakQL.Match(subscription.Query, items);
					if (!matchedEntities.IsNullOrEmpty())
					{
						foreach (var entity in matchedEntities)
						{
                            // Create Notification
                            var notification = new TrakHoundEntityNotification(TrakHoundEntityNotificationType.Deleted, entity);
                            notifications.Add(notification);

                            //DeletedFilterEntityReceived(subscription, match);
                        }
					}

                    if (!notifications.IsNullOrEmpty())
                    {
                        // Push Notifications to Consumer
                        subscription.Consumer.Push(notifications);
                    }
                }
			}
		}
	}
}
