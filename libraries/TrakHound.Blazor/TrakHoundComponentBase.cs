// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using System.Text.Json;
using TrakHound.Apps;
using TrakHound.Clients;
using TrakHound.Logging;
using TrakHound.Packages;
using TrakHound.Security;
using TrakHound.Volumes;

namespace TrakHound.Blazor
{
    public abstract class TrakHoundComponentBase : ComponentBase, ITrakHoundAppComponent
    {
        private Dictionary<string, object> _parameters;


        [CascadingParameter(Name = "AppId")]
        public string AppId { get; set; }

        [CascadingParameter(Name = "AppName")]
        public string AppName { get; set; }

        [CascadingParameter(Name = "Client")]
        public ITrakHoundClient Client { get; set; }

        [CascadingParameter(Name = "Package")]
        public TrakHoundPackage Package { get; set; }

        [CascadingParameter(Name = "Volume")]
        public ITrakHoundVolume Volume { get; set; }

        [CascadingParameter(Name = "Logger")]
        public ITrakHoundLogger Logger { get; set; }

        [CascadingParameter(Name = "Session")]
        public ITrakHoundSession Session { get; set; }

        public ITrakHoundAuthenticatedSession AuthenticatedSession
        {
            get
            {
                if (Session != null && typeof(ITrakHoundAuthenticatedSession).IsAssignableFrom(Session.GetType()))
                {
                    return (ITrakHoundAuthenticatedSession)Session;
                }

                return null;
            }
        }

        public ITrakHoundAnnonymousSession AnnonymousSession
        {
            get
            {
                if (Session != null && typeof(ITrakHoundAnnonymousSession).IsAssignableFrom(Session.GetType()))
                {
                    return (ITrakHoundAnnonymousSession)Session;
                }

                return null;
            }
        }

        [CascadingParameter(Name = "BaseUrl")]
        public string BaseUrl { get; set; }

        [CascadingParameter(Name = "BasePath")]
        public string BasePath { get; set; }

        [CascadingParameter(Name = "BaseLocation")]
        public string BaseLocation { get; set; }

        [CascadingParameter(Name = "Parameters")]
        public Dictionary<string, object> Parameters
        {
            get
            {
                if (_parameters == null) _parameters = new Dictionary<string, object>();
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        [Inject]
        public ITrakHoundAppInjectionServices Services { get; set; }


        protected string GetUrl(string destination)
        {
            return Url.Combine(BaseUrl, destination);
        }

        protected string GetPath(string destination)
        {
            return Url.Combine(BasePath, destination);
        }

        protected string GetLocation(string destination)
        {
            return Url.Combine(BaseLocation, destination);
        }

        protected TService GetService<TService>() where TService : class, ITrakHoundAppInjectionService
        {
            try
            {
                return Services?.Get<TService>(AppId);
            }
            catch { }

            return null;
        }


        public bool ParameterExists(string name)
        {
            return !Parameters.IsNullOrEmpty() && Parameters.ContainsKey(name);
        }

        public string GetParameter(string name)
        {
            if (!Parameters.IsNullOrEmpty())
            {
                Parameters.TryGetValue(name, out var obj);
                if (obj != null) return obj.ToString();
            }

            return null;
        }

        public T GetParameter<T>(string name)
        {
            if (!Parameters.IsNullOrEmpty() && !string.IsNullOrEmpty(name))
            {
                Parameters.TryGetValue(name, out var obj);
                if (obj != null)
                {
                    var x = obj;
                    if (obj.GetType() == typeof(JsonElement) && ((JsonElement)x).ValueKind == JsonValueKind.Number) x = x.ToInt();


                    try
                    {
                        return (T)Convert.ChangeType(obj, typeof(T));
                    }
                    catch { }
                }
            }

            return default(T);
        }

        public void SetParameter(string name, object value)
        {
            if (!string.IsNullOrEmpty(name) && value != null)
            {
                if (Parameters == null) Parameters = new Dictionary<string, object>();
                Parameters.Remove(name);
                Parameters.Add(name, value.ToString());
            }
        }
    }
}
