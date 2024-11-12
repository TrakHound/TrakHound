// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Routing
{
    public class TrakHoundRouterInformation : ITrakHoundRouterInformation
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public static TrakHoundRouterInformation Create(TrakHoundRouter router)
        {
            if (router != null && router.Configuration != null)
            {
                var information = new TrakHoundRouterInformation();
                information.Id = router.Configuration.Id;
                information.Name = router.Configuration.Name;
                information.Description = router.Configuration.Description;

                return information;
            }

            return null;
        }
    }
}
