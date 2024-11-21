// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Blazored.LocalStorage;
using Microsoft.Extensions.Hosting.WindowsServices;
using NLog;
using NLog.Web;
using Radzen;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Blazor;
using TrakHound.Blazor.Services;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Http;
using TrakHound.Instance.Configurations;
using TrakHound.Instance.Security;
using TrakHound.Instance.Services;
using TrakHound.Instances;
using TrakHound.Logging;
using TrakHound.Security;

#if !DEBUG
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
#endif

namespace TrakHound.Instance
{
    [CommandGroup]
    public class InstanceApplication
    {
        private static readonly TrakHoundFileCache _packageFileCache = new TrakHoundFileCache();

        private static Logger _logger;
        private static TrakHoundInstance _instance;
        private static TrakHoundHttpWebSocketManager _webSocketManager;

        [Command]
        public static async Task Run(
            [CommandParameter] string configurationProfileId = TrakHoundConfigurationProfile.Default,
            [CommandOption] string configurationPath = null,
            [CommandOption] string packagesPath = null
            )
        {
            PrintConsoleHeader();

            TrakHoundTemp.Clear();
            _logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            var configurationsPath = Path.Combine(AppContext.BaseDirectory, "config");

            try
            {
                // Read Instance Configuration File
                var instanceConfigurationPath = Path.Combine(configurationsPath, TrakHoundInstanceConfiguration.Filename);
                var instanceConfiguration = TrakHoundInstanceConfiguration.Read(instanceConfigurationPath);
                if (instanceConfiguration != null)
                {
                    _logger.Info($"Instance Configuration File Read : {instanceConfigurationPath}");
                }
                else
                {
                    _logger.Info("No Instance Configuration File Found. Loading Default..");
                    instanceConfiguration = new TrakHoundInstanceConfiguration();
                    instanceConfiguration.Save(instanceConfigurationPath, false);
                }


                var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
                {
                    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
                });

                if (!IsRunningInProcessIIS() && !InRunningInDocker())
                {
                    builder.Host.UseWindowsService();
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                }

                // Add Logging
                //builder.Logging.ClearProviders();
                builder.Host.UseNLog();
                var logProvider = TrakHoundLogProvider.Get();
                TrakHoundLogProvider.MinimumLogLevel = TrakHoundLogLevel.Debug;
                logProvider.LogEntryReceived += LogEntryReceived;

                // Add Local Storage (using Blazored)
                builder.Services.AddBlazoredLocalStorage();
                builder.Services.AddTransient<ITrakHoundAppLocalStorageService, TrakHoundLocalStorageService>();

                // Add Blazor
                builder.Services.AddRazorPages();
                builder.Services.AddRadzenComponents();
                builder.Services.AddRazorComponents((options) => options.DetailedErrors = builder.Environment.IsDevelopment()).AddInteractiveServerComponents();
                if (instanceConfiguration.AdminInterfaceEnabled) builder.AddTrakHoundExplorer();

                // Create TrakHound Instance
                _instance = new TrakHoundInstance(instanceConfiguration);
                await _instance.Start();
                builder.Services.AddSingleton<TrakHoundInstanceConfiguration>(instanceConfiguration);
                builder.Services.AddSingleton<ITrakHoundInstance>(_instance);
                builder.Services.AddSingleton<ITrakHoundClientProvider>(_instance.ClientProvider);


                // Add Instance Manager (for Explorer)
                builder.Services.AddSingleton<Pages.Explorer.InstanceManager>();

                // Add HttpContext to Blazor Pages
                builder.Services.AddHttpContextAccessor();

                // Add Sessions
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession();

                // Add Security Services
                builder.Services.AddSingleton<ITrakHoundSecurityManager>(_instance.SecurityManager);
                builder.Services.AddSingleton<TrakHoundAuthenticationSessionService>();
                builder.Services.AddScoped<TrakHoundAuthenticationService>();

                // Add Admin Authentication Services
                builder.Services.AddSingleton<AdminTokenService>();
                builder.Services.AddScoped<AdminAuthenticationService>();


                // DEBUG ONLY
                var entityClipboardService = new EntityClipboardService(null);
                builder.Services.AddSingleton<EntityClipboardService>(entityClipboardService);

                // Read Remotes Configuration File
                var remotesConfigurationPath = Path.Combine(configurationsPath, RemotesConfiguration.Filename);
                var remotesConfiguration = RemotesConfiguration.Read(remotesConfigurationPath);
                if (remotesConfiguration != null)
                {
                    _logger.Info($"Remotes Configuration File Read : {remotesConfigurationPath}");
                    
                }
                else
                {
                    _logger.Info("No Remotes Configuration File Found. Loading Default..");
                    remotesConfiguration = RemotesConfiguration.GetDefault();
                    remotesConfiguration.Save(remotesConfigurationPath);
                }
                builder.Services.AddSingleton(remotesConfiguration);

                // Add TrakHound Blazor Services(Apps)
                builder.Services.AddTrakHoundHosting(_instance);

                // Set Default Admin Theme
                TrakHoundThemeService.SetDefaultTheme(AdminThemes.Key, instanceConfiguration.AdminDefaultTheme.ConvertEnum<TrakHoundThemes>());

                // Set Blazor Upload File Size Limit
                builder.Services.AddSignalR(options =>
                {
                    options.MaximumReceiveMessageSize = 102400000;
                });

                // Add WebSocket Manager
                _webSocketManager = new TrakHoundHttpWebSocketManager();
                builder.Services.AddSingleton<TrakHoundHttpWebSocketManager>(_webSocketManager);

                //builder.Services.AddControllers();
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.WriteIndented = true;
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    });

                // Don't add Compression when debugging. Messes up the CSS Hot Reload
#if !DEBUG
                // Add Compression Services
                builder.Services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.Providers.Add<GzipCompressionProvider>();
                });

                builder.Services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Optimal;
                });
#endif

                if (!IsRunningInProcessIIS() && !InRunningInDocker())
                {
                    builder.WebHost.UseKestrel(o =>
                    {
                        if (!string.IsNullOrEmpty(instanceConfiguration.HttpAddress))
                        {
                            if (IPAddress.TryParse(instanceConfiguration.HttpAddress, out var httpAddress))
                            {
                                o.Listen(httpAddress, instanceConfiguration.HttpPort);
                            }
                            else
                            {
                                // ERROR
                            }
                        }
                        else
                        {
                            o.ListenAnyIP(instanceConfiguration.HttpPort);
                        }

                        o.Limits.MaxRequestBodySize = null;
                    });
                }


                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/_error", createScopeForErrors: true);
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                // Set AppProvider for TrakHound Injection Services
                var appProvider = app.Services.GetService<ITrakHoundAppProvider>();
                if (appProvider != null)
                {
                    _instance.AppProvider = appProvider;
                    ((TrakHoundInstanceClientProvider)_instance.ClientProvider).AppProvider = appProvider;
                }

                // Set AppProvider for TrakHound Injection Services
                var apiProvider = app.Services.GetService<ITrakHoundApiProvider>();

                // Set ServiceProvider for TrakHound Injection Services
                var injectionServiceManager = app.Services.GetService<ITrakHoundAppInjectionServiceManager>();
                if (injectionServiceManager != null)
                {
                    ((TrakHoundAppInjectionServiceManager)injectionServiceManager).ServiceProvider = app.Services;
                }

#if !DEBUG
                app.UseResponseCompression();
#endif

                app.UseStaticFiles();

                // Handle App Package Static Files (ex. Stylesheets, scripts, images, etc.)
                app.Use(async (context, next) =>
                {
                    if (IsPackagesRoute(context.Request.Path))
                    {
                        await ProcessPackagesRoute(context);
                        return;
                    }
                    else
                    {
                        await next(context);
                    }
                });

                // Add WebSockets
                var webSocketOptions = new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(10)
                };
                app.UseWebSockets(webSocketOptions);

                app.UseSession();

                // Map Api Routes to TrakHoundHttpApiController.
                // Apps and Api's both use the same root route so this checks to see if the route matches an API first
                app.Use(async (context, next) =>
                {
                    if (IsApiRouteValid(context))
                    {
                        // Add Api RoutePrefix (used internally to route to TrakHoundHttpApiController)
                        //context.Request.Path = Url.Combine(TrakHoundHttpApiController.RoutePrefix, context.Request.Path);
                        context.Request.Path = "/" + Url.Combine(TrakHoundHttpApiController.RoutePrefix, context.Request.Path);
                    }
                    else if (!IsStaticRoute(context.Request.Path))
                    {
                        //Add for InteractiveServerMode(?)
                        context.Response.Headers.Append("Content-Security-Policy", "frame-ancestors 'self'");
                    }

                    if (IsSystemApi(context.Request.Path))
                    {
                        if (instanceConfiguration.SystemApiEnabled)
                        {
                            await next(context);
                        }
                    }
                    else
                    {
                        await next(context);
                    }
                });

                app.UseRouting();
                app.UseAntiforgery();
                app.UseMiddleware<TrakHoundAuthenticationMiddleware>();
                app.MapControllers();

                // Setup API Controllers
                app.MapControllerRoute("IdentityCallbackController", $"_identity/callback/{{**slug}}", defaults: new { controller = "IdentityCallback", action = "ProcessCallback" });
                app.MapControllerRoute("IdentityRevokationController", $"_identity/revoke", defaults: new { controller = "IdentityRevokation", action = "ProcessRevoke" });
                app.MapControllerRoute("TrakHoundHttpApiController", "_instances/information/host", defaults: new { controller = "TrakHoundHttpInstance", action = "GetHostInformation" });
                app.MapControllerRoute("TrakHoundHttpApiController", "_api/information", defaults: new { controller = "TrakHoundHttpApi", action = "GetRouteInformations" });
                app.MapControllerRoute("TrakHoundHttpApiController", "_api/information/{apiId}", defaults: new { controller = "TrakHoundHttpApi", action = "GetInformation" });
                app.MapControllerRoute("TrakHoundHttpApiController", "_api/package/{packageId}/{**slug}", defaults: new { controller = "TrakHoundHttpApi", action = "GetByPackage" });
                app.MapControllerRoute("TrakHoundHttpApiController", "_apps/information", defaults: new { controller = "TrakHoundHttpApp", action = "GetInformations" });
                app.MapControllerRoute("TrakHoundHttpApiController", "_apps/information/{appId}", defaults: new { controller = "TrakHoundHttpApp", action = "GetInformation" });
                app.MapControllerRoute("TrakHoundHttpApiController", $"{TrakHoundHttpApiController.RoutePrefix}/{{**slug}}", defaults: new { controller = "TrakHoundHttpApi", action = "GetByRoute" });

                if (instanceConfiguration.AdminInterfaceEnabled)
                {
                    app.MapRazorComponents<InstanceApp>().AddInteractiveServerRenderMode();
                }

                app.MapFallbackToPage("/_Host");

                var applicationLifetime = app.Services.GetService<IHostApplicationLifetime>();
                applicationLifetime.ApplicationStopping.Register(OnShutdown);

                app.Start();
                app.WaitForShutdown();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                _logger.Fatal(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                if (_instance != null) await _instance.Stop();

                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Check if this process is running on Windows in an in process instance in IIS
        /// </summary>
        /// <returns>True if Windows and in an in process instance on IIS, false otherwise</returns>
        private static bool IsRunningInProcessIIS()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            string processName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName);
            return (processName.Contains("w3wp", StringComparison.OrdinalIgnoreCase) ||
                processName.Contains("iisexpress", StringComparison.OrdinalIgnoreCase));
        }


        private static bool InRunningInDocker()
        {
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        }


        private static bool IsStaticRoute(PathString path)
        {
            var s = path.ToString();

            if (s.StartsWith("/_blazor")) return true;     // Blazor
            if (s.StartsWith("/_framework")) return true;  // Blazor
            if (s.StartsWith("/_content")) return true;    // Blazor
            if (s.StartsWith("/_packages")) return true;   // TrakHound Package (css, js, etc.)
            if (s.StartsWith("/_download")) return true;   // TrakHound Explorer Downloads Controller
            if (s.StartsWith("/img")) return true;         // Static Image Files
            if (s.StartsWith("/css")) return true;         // Static CSS Files

            return false;
        }

        private static bool IsErrorRoute(PathString path)
        {
            var s = path.ToString();
            if (s.StartsWith("/_error")) return true;

            return false;
        }

        private static bool IsPackagesRoute(PathString path)
        {
            var s = path.ToString();
            if (s.StartsWith("/_packages")) return true;

            return false;
        }

        private static bool IsSystemApi(PathString path)
        {
            var s = path.ToString();
            if (s.StartsWith("/_instances")) return true;
            if (s.StartsWith("/_api")) return true;
            if (s.StartsWith("/_apps")) return true;
            if (s.StartsWith("/_entities")) return true;
            if (s.StartsWith("/_functions")) return true;
            if (s.StartsWith("/_services")) return true;

            return false;
        }


        private static bool IsAppRouteValid(HttpContext context)
        {
            return _instance.AppProvider.IsRouteValid(context.Request.Path);
        }

        private static bool IsApiRouteValid(HttpContext context)
        {
            return _instance.ApiProvider.IsRouteValid(context.Request.Path);
        }


        private static async Task ProcessPackagesRoute(HttpContext context)
        {
            // Handle Packages wwwroot files manually due to .NET debug wwwroot folder.
            // Loading .js files causes the build to fail as it tries to map javascript exports to a .razor file

            var requestPath = context.Request.Path.ToString().TrimStart('/');

            var packagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp", "wwwroot");
            var responsePath = Path.Combine(packagesDir, requestPath);

            var content = _packageFileCache.Get(responsePath);
            if (content == null)
            {
                if (File.Exists(responsePath))
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = MimeTypes.GetMimeType(responsePath);

                    content = File.ReadAllBytes(responsePath);
                    if (content != null)
                    {
                        // Add File Content to Cache
                        _packageFileCache.Add(responsePath, content);

                        // Write Content to Response Stream
                        await context.Response.Body.WriteAsync(content, 0, content.Length);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = MimeTypes.GetMimeType(responsePath);

                // Write Cached Content to Response Stream
                await context.Response.Body.WriteAsync(content, 0, content.Length);
            }
        }

        private static async void OnShutdown()
        {
            if (_webSocketManager != null) await _webSocketManager.DisposeAsync();
            if (_instance != null) await _instance.Stop();
        }

        private static void LogEntryReceived(object sender, TrakHoundLogItem entry)
        {
            var logger = (ITrakHoundLogger)sender;

            var logEvent = new LogEventInfo();
            logEvent.LoggerName = logger.Name;
            logEvent.Message = entry.Message;

            switch (entry.LogLevel)
            {
                case TrakHoundLogLevel.Critical: logEvent.Level = NLog.LogLevel.Fatal; break;
                case TrakHoundLogLevel.Error: logEvent.Level = NLog.LogLevel.Error; break;
                case TrakHoundLogLevel.Warning: logEvent.Level = NLog.LogLevel.Warn; break;
                case TrakHoundLogLevel.Information: logEvent.Level = NLog.LogLevel.Info; break;
                case TrakHoundLogLevel.Debug: logEvent.Level = NLog.LogLevel.Debug; break;
                case TrakHoundLogLevel.Trace: logEvent.Level = NLog.LogLevel.Trace; break;
            }

            _logger.Log(logEvent);
        }

        private static void PrintConsoleHeader()
        {
            var assembly = Assembly.GetEntryAssembly();
            var version = assembly.GetName().Version;

            Console.WriteLine("--------------------");
            Console.WriteLine("Copyright 2024 TrakHound Inc., All Rights Reserved");
            Console.WriteLine("TrakHound Instance : Version " + version.ToString());
            Console.WriteLine("--------------------");
            Console.WriteLine("This application is licensed under the MIT License (https://choosealicense.com/licenses/mit/)");
            Console.WriteLine("Source code available at Github.com (https://github.com/TrakHound/MTConnect.NET)");
            Console.WriteLine("--------------------");
        }
    }
}
