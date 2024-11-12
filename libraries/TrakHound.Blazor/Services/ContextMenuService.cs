// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Blazor.Services
{
    public class ContextMenuService
    {
        private readonly Dictionary<string, IContextMenu> _menus = new Dictionary<string, IContextMenu>();
        private readonly object _lock = new object();
        private IContextMenu _shownMenu;


        public void Add(IContextMenu menu)
        {
            if (menu != null && !string.IsNullOrEmpty(menu.Id))
            {
                lock (_lock)
                {
                    _menus.Remove(menu.Id);
                    _menus.Add(menu.Id, menu);
                }
            }
        }

        public void Remove(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                lock (_lock)
                {
                    _menus.Remove(id);
                    if (_shownMenu != null && _shownMenu.Id == id) _shownMenu = null;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _menus.Clear();
                _shownMenu = null;
            }
        }

        public void Show(string id, double x, double y)
        {
            if (!string.IsNullOrEmpty(id))
            {
                lock (_lock)
                {
                    var menu = _menus.GetValueOrDefault(id);
                    if (menu != null)
                    {
                        menu.Show(x, y);
                        _shownMenu = menu;
                    }
                }
            }
        }

        public void Hide(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                lock (_lock)
                {
                    var menu = _menus.GetValueOrDefault(id);
                    if (menu != null)
                    {
                        menu.Hide();
                        _shownMenu = null;
                    }
                }
            }
        }
    }
}
