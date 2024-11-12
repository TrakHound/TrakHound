// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using TrakHound.Apps;
using TrakHound.Clients;
using TrakHound.Packages;
using TrakHound.Security;
using TrakHound.Volumes;

namespace TrakHound.Blazor
{
    public abstract class TrakHoundAppBase : ComponentBase, ITrakHoundAppComponent
    {
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

        [Inject]
        public ITrakHoundAppInjectionServices Services { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        protected string GetUrl()
        {
            return Url.Combine(NavigationManager.BaseUri, BaseUrl);
        }

        protected string GetUrl(string destination)
        {
            var baseUrl = Url.Combine(NavigationManager.BaseUri, BaseUrl);
            return Url.Combine(baseUrl, destination);
        }

        protected string GetPath()
        {
            return BasePath;
        }

        protected string GetPath(string destination)
        {
            return Url.Combine(BasePath, destination);
        }

        protected string GetLocation()
        {
            return BaseLocation;
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
    }
}
