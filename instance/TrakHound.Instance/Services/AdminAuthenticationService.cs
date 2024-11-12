// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using TrakHound.Instances;

namespace TrakHound.Instance.Services
{
    public class AdminAuthenticationService
    {
        private const string _userFilename = ".user";
        //private const string _tokenKey = "trakhound.instance.admin.authentication.token";
        private const int _usernameMinLength = 4;
        private const int _passwordMinLength = 4;

        private readonly ITrakHoundInstance _instance;
        private readonly AdminTokenService _tokenService;
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorageService;
        private readonly TimeSpan _ttl = TimeSpan.FromDays(7);
        private readonly object _lock = new object();
        private bool _isSetup;


        class User
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("salt")]
            public string Salt { get; set; }

            [JsonPropertyName("hash")]
            public string Hash { get; set; }
        }

        public struct AuthenticationResult
        {
            public bool Success { get; set; }
            public string Username { get; set; }
            public string Token { get; set; }
            public string Message { get; set; }
        }


        public AdminAuthenticationService(ITrakHoundInstance instance, AdminTokenService tokenService, Blazored.LocalStorage.ILocalStorageService localStorageService)
        {
            _instance = instance;
            _tokenService = tokenService;
            _localStorageService = localStorageService;
        }


        public async Task<AuthenticationResult> Login(string username, string password)
        {
            var result = new AuthenticationResult();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var userFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", _userFilename);
                var user = await Json.ReadAsync<User>(userFilePath);
                if (user != null)
                {
                    var hash = (password + user.Salt).ToSHA256Hash();

                    if (user.Hash == hash)
                    {
                        var token = Guid.NewGuid().ToString();

                        var storedToken = new AdminAuthenticationToken();
                        storedToken.Token = token;
                        storedToken.Username = user.Username;
                        storedToken.Timestamp = UnixDateTime.Now;

                        _tokenService.Add(storedToken);

                        await SetToken(token);

                        result.Success = true;
                        result.Username = user.Username;
                        result.Token = token;
                    }
                    else
                    {
                        result.Message = "Invalid Password";
                    }
                }
                else
                {
                    result.Message = "Login Error";
                }
            }
            else
            {
                result.Message = "Invalid Username or Password";
            }

            return result;
        }

        public async Task Logout(string token)
        {
            await DeleteToken(token);
            _tokenService.Remove(token);
        }

        public async Task<bool> IsAuthenticated()
        {
            if (_instance.Configuration.AdminAuthenticationEnabled)
            {
                var token = await GetToken();
                if (token != null)
                {
                    lock (_lock)
                    {
                        var storedToken = _tokenService.Validate(token);
                        if (storedToken != null)
                        {
                            var now = DateTime.UtcNow;
                            var timestamp = storedToken.Timestamp.ToDateTime();
                            if (now - timestamp < _ttl)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public async Task<string> GetToken()
        {
            try
            {
                return await _localStorageService.GetItemAsync<string>(GetTokenKey());
            }
            catch { }

            return null;
        }

        public async Task SetToken(string token)
        {
            try
            {
                await _localStorageService.SetItemAsync(GetTokenKey(), token);
            }
            catch { }
        }

        public async Task DeleteToken(string token)
        {
            try
            {
                await _localStorageService.RemoveItemAsync(GetTokenKey());
            }
            catch { }
        }

        private string GetTokenKey()
        {
            return $"trakhound.instance.{_instance.Id}.admin.authentication.token";
        }


        public bool IsSetup()
        {
            if (_isSetup)
            {
                return true;
            }
            else
            {
				var userFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", _userFilename);
                return File.Exists(userFilePath);
			}
        }

        public async Task<AuthenticationResult> Setup(string username, string password)
        {
            var result = new AuthenticationResult();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (username.Length < _usernameMinLength)
                {
                    result.Message = "Username must be a minimum of 4 characters";
                    return result;
                }

                if (password.Length < _passwordMinLength)
                {
                    result.Message = "Password must be a minimum of 4 characters";
                    return result;
                }

                var salt = StringFunctions.RandomString(10);
                var hash = (password + salt).ToSHA256Hash();

                var user = new User();
                user.Username = username;
                user.Salt = salt;
                user.Hash = hash;

                var userFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", _userFilename);
                await Json.WriteAsync(user, userFilePath, CancellationToken.None);

                var token = Guid.NewGuid().ToString();

                var storedToken = new AdminAuthenticationToken();
                storedToken.Token = token;
                storedToken.Username = user.Username;
                storedToken.Timestamp = UnixDateTime.Now;
                _tokenService.Add(storedToken);

                await SetToken(token);

                _isSetup = true;

                result.Success = true;
                result.Username = username;
                result.Token = token;
            }

            return result;
        }
    }
}
