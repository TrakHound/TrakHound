// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Radzen;
using TrakHound.Apps;

namespace TrakHound.Blazor.Services
{
    public class TrakHoundThemeService
    {
        private readonly ITrakHoundAppLocalStorageService _localStorage;
        private TrakHoundThemes _theme;
        private bool _themeSet;


        public TrakHoundThemes Theme => _theme;

        public event EventHandler<TrakHoundThemes> ThemeChanged;


        public TrakHoundThemeService(ITrakHoundAppLocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task Load(string key)
        {
            if (!_themeSet && !string.IsNullOrEmpty(key))
            {
                _themeSet = true;

                var value = await _localStorage.GetItem<string>(key);
                if (value != null)
                {
                    _theme = value.ConvertEnum<TrakHoundThemes>();

                    if (ThemeChanged != null) ThemeChanged.Invoke(this, _theme);
                }
            }
        }

        public async Task SetTheme(string key, TrakHoundThemes theme)
        {
            if (_theme != theme && !string.IsNullOrEmpty(key))
            {
                _theme = theme;

                await _localStorage.SetItem<string>(key, theme.ToString());

                if (ThemeChanged != null) ThemeChanged.Invoke(this, theme);
            }
        }
    }
}
