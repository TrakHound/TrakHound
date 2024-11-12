// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.Services
{
    public interface IContextMenu
    {
        string Id { get; set; }

        void Show(double x, double y);

        void Hide();
    }
}
