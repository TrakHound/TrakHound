using Microsoft.AspNetCore.Components;
using TrakHound.Blazor.Components;
using TrakHound.Blazor.Diagrams;
using TrakHound.Blazor.Diagrams.Core.Models;
using TrakHound.Blazor.Diagrams.Core.PathGenerators;
using TrakHound.Blazor.Diagrams.Core.Routers;
using TrakHound.Blazor.Diagrams.Options;
using TrakHound.Blazor.Instance.InstanceConfigurationInternal.Components;
using TrakHound.Blazor.Services;
using TrakHound.Configurations;
using TrakHound.Drivers;
using TrakHound.Routing;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Routing
{
    public partial class Index
    {
        private readonly Dictionary<string, TrakHoundRouterConfiguration> _routerConfigurations = new Dictionary<string, TrakHoundRouterConfiguration>();
        private readonly Dictionary<string, ITrakHoundDriverConfiguration> _driverConfigurations = new Dictionary<string, ITrakHoundDriverConfiguration>();
        private NotificationService notificationService;
        private double _gridSize = 15;
        private bool _loading;
        private bool _saveLoading;


        [CascadingParameter(Name = "InstanceConfiguration")]
        public InstanceConfiguration InstanceConfiguration { get; set; }

        private BlazorDiagram Diagram { get; set; }


        public Index()
        {
            notificationService = new NotificationService();

            var options = new BlazorDiagramOptions
            {
                AllowMultiSelection = true,
                GridSize = 15,
                Zoom =
                {
                    ScaleFactor = 1.25,
                    Inverse = true,
                    Enabled = true,
                },
                Links =
                {
                    DefaultRouter = new NormalRouter(),
                    DefaultPathGenerator = new SmoothPathGenerator(75),
                    EnableSnapping = true,
                    DefaultColor = "var(--accentLight)",
                    DefaultSelectedColor = "var(--accentDark)"
                }
            };

            Diagram = new BlazorDiagram(options);
            Diagram.SetZoom(0.65);
        }

        protected async override Task OnInitializedAsync()
        {
            Diagram.Nodes.Removed += NodeRemoved;

            Diagram.RegisterComponent<RouterNode, RouterNodeWidget>();
            Diagram.RegisterComponent<RouteNode, RouteNodeWidget>();
            Diagram.RegisterComponent<RedirectNode, RedirectNodeWidget>();
            Diagram.RegisterComponent<TargetNode, TargetNodeWidget>();

            await Load();
        }


        private async Task Load()
        {
            _loading = true;
            await InvokeAsync(StateHasChanged);

            Diagram.Nodes.Clear();

            if (InstanceConfiguration != null)
            {
                // Get Router Configurations
                var routerConfigurations = InstanceConfiguration.ConfigurationGroup.ConfigurationProfile.Get<TrakHoundRouterConfiguration>(TrakHoundRouterConfiguration.ConfigurationCategory);
                if (!routerConfigurations.IsNullOrEmpty())
                {
                    foreach (var configuration in routerConfigurations)
                    {
                        if (!string.IsNullOrEmpty(configuration.Id))
                        {
                            _routerConfigurations.Remove(configuration.Id);
                            _routerConfigurations.Add(configuration.Id, configuration);
                        }
                    }
                }

                // Get Driver Configurations
                var driverConfigurations = InstanceConfiguration.ConfigurationGroup.ConfigurationProfile.Get<ITrakHoundDriverConfiguration>(TrakHoundDriverConfiguration.ConfigurationCategory);
                if (!driverConfigurations.IsNullOrEmpty())
                {
                    foreach (var configuration in driverConfigurations)
                    {
                        if (!string.IsNullOrEmpty(configuration.Id))
                        {
                            _driverConfigurations.Remove(configuration.Id);
                            _driverConfigurations.Add(configuration.Id, configuration);
                        }
                    }
                }

                // Load Diagram Configurations
                ((TrakHoundConfigurationProfile)InstanceConfiguration.ConfigurationGroup.ConfigurationProfile).Load<TrakHoundDiagramConfiguration>(TrakHoundDiagramConfiguration.ConfigurationCategory);

                var diagramConfiguration = InstanceConfiguration.ConfigurationGroup.ConfigurationProfile.Get<TrakHoundDiagramConfiguration>(TrakHoundDiagramConfiguration.ConfigurationCategory)?.FirstOrDefault();
                if (diagramConfiguration != null && !routerConfigurations.IsNullOrEmpty())
                {
                    if (!diagramConfiguration.Nodes.IsNullOrEmpty())
                    {
                        var dNodeConfigurations = diagramConfiguration.Nodes.ToDictionary(o => o.Id);

                        foreach (var routerConfiguration in routerConfigurations)
                        {
                            var routerNodeConfiguration = dNodeConfigurations.GetValueOrDefault(routerConfiguration.Id);
                            if (routerNodeConfiguration != null)
                            {
                                var routerNode = CreateRouterNode(routerConfiguration);
                                routerNode.Position = new Diagrams.Core.Geometry.Point(routerNodeConfiguration.X, routerNodeConfiguration.Y);
                                Diagram.Nodes.Add(routerNode);

                                if (!routerConfiguration.Routes.IsNullOrEmpty())
                                {
                                    foreach (var routeConfiguration in routerConfiguration.Routes)
                                    {
                                        var routeNodeConfiguration = dNodeConfigurations.GetValueOrDefault(routeConfiguration.Id);
                                        if (routeNodeConfiguration != null)
                                        {
                                            var routeNode = CreateRouteNode(routeConfiguration);
                                            routeNode.Position = new Diagrams.Core.Geometry.Point(routeNodeConfiguration.X, routeNodeConfiguration.Y);
                                            Diagram.Nodes.Add(routeNode);

                                            // Add Link
                                            Diagram.Links.Add(new LinkModel(routerNode.GetOutputPort(), routeNode.GetInputPort()));

                                            if (!routeConfiguration.Targets.IsNullOrEmpty())
                                            {
                                                foreach (var targetConfiguration in routeConfiguration.Targets)
                                                {
                                                    var targetNode = LoadTargetNode(ref dNodeConfigurations, targetConfiguration);

                                                    // Add Link
                                                    Diagram.Links.Add(new LinkModel(routeNode.GetOutputPort(), targetNode.GetInputPort()));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            await Task.Delay(500);

            _loading = false;
            await InvokeAsync(StateHasChanged);
        }

        private TargetNode LoadTargetNode(ref Dictionary<string, TrakHoundDiagramNodeConfiguration> dNodeConfigurations, TrakHoundTargetConfiguration targetConfiguration)
        {
            var targetNodeConfiguration = dNodeConfigurations.GetValueOrDefault(targetConfiguration.Id);
            if (targetNodeConfiguration != null)
            {
                var targetNode = CreateTargetNode(targetConfiguration);
                targetNode.Position = new Diagrams.Core.Geometry.Point(targetNodeConfiguration.X, targetNodeConfiguration.Y);
                Diagram.Nodes.Add(targetNode);

                if (!targetConfiguration.Redirects.IsNullOrEmpty())
                {
                    foreach (var redirectConfiguration in targetConfiguration.Redirects)
                    {
                        var redirectNodeConfiguration = dNodeConfigurations.GetValueOrDefault(redirectConfiguration.Id);
                        if (redirectNodeConfiguration != null)
                        {
                            var redirectNode = CreateRedirectNode(redirectConfiguration);
                            redirectNode.Position = new Diagrams.Core.Geometry.Point(redirectNodeConfiguration.X, redirectNodeConfiguration.Y);

                            if (!redirectConfiguration.Options.IsNullOrEmpty())
                            {
                                redirectNode.IsPublishOptionEnabled = redirectConfiguration.Options.Any(o => o == RouteRedirectOptions.Publish);
                                redirectNode.IsEmptyOptionEnabled = redirectConfiguration.Options.Any(o => o == RouteRedirectOptions.Empty);
                            }
                            
                            Diagram.Nodes.Add(redirectNode);

                            if (!redirectConfiguration.Targets.IsNullOrEmpty())
                            {
                                foreach (var redirectTargetConfiguration in redirectConfiguration.Targets)
                                {
                                    var redirectTargetNode = LoadTargetNode(ref dNodeConfigurations, redirectTargetConfiguration);

                                    // Add Link
                                    Diagram.Links.Add(new LinkModel(redirectNode.GetOutputPort(), redirectTargetNode.GetInputPort()));
                                }
                            }

                            // Add Link
                            Diagram.Links.Add(new LinkModel(targetNode.GetOutputPort(), redirectNode.GetInputPort()));
                        }
                    }
                }

                return targetNode;
            }

            return null;
        }


        private async Task AutoArrange()
        {
            if (!_routerConfigurations.IsNullOrEmpty())
            {
                var x = 0;
                var y = 0;

                foreach (var routerConfiguration in _routerConfigurations.Values)
                {
                    var routerNode = CreateRouterNode(routerConfiguration);
                    routerNode.Position = new Diagrams.Core.Geometry.Point(x, y);
                    Diagram.Nodes.Add(routerNode);

                    y += 300;
                }   
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task Save()
        {
            _saveLoading = true;
            await InvokeAsync(StateHasChanged);

            InstanceConfiguration.ConfigurationGroup.ConfigurationProfile.Clear(TrakHoundRouterConfiguration.ConfigurationCategory);

            if (!Diagram.Nodes.IsNullOrEmpty())
            {
                var nodeConfigurations = new List<TrakHoundDiagramNodeConfiguration>();
                var routerConfigurations = new List<TrakHoundRouterConfiguration>();

                nodeConfigurations.Clear();
                routerConfigurations.Clear();

                // Create Diagram Configurations
                foreach (var node in Diagram.Nodes)
                {
                    var nodeConfiguration = new TrakHoundDiagramNodeConfiguration();
                    nodeConfiguration.Id = node.Id;
                    nodeConfiguration.X = node.Position.X;
                    nodeConfiguration.Y = node.Position.Y;

                    if (!node.Ports.IsNullOrEmpty())
                    {
                        foreach (var nodePort in node.Ports)
                        {
                            if (!nodePort.Links.IsNullOrEmpty())
                            {
                                var nodeLinkConfigurations = new List<TrakHoundDiagramLinkConfiguration>();
                                foreach (var link in nodePort.Links)
                                {
                                    var nodeLinkConfiguration = new TrakHoundDiagramLinkConfiguration();
                                    nodeLinkConfiguration.InputId = ((PortModel)link.Source.Model).Id;
                                    nodeLinkConfiguration.OuputId = ((PortModel)link.Target.Model).Id;
                                    nodeLinkConfigurations.Add(nodeLinkConfiguration);
                                }
                                nodeConfiguration.Links = nodeLinkConfigurations;
                            }
                        }
                    }

                    nodeConfigurations.Add(nodeConfiguration);
                }

                var diagramConfiguration = new TrakHoundDiagramConfiguration();
                diagramConfiguration.Id = "routing-diagram";
                diagramConfiguration.Nodes = nodeConfigurations;

                InstanceConfiguration.ConfigurationGroup.ConfigurationProfile.Add(diagramConfiguration, true);


                // Create Router Configurations
                var routerNodes = Diagram.Nodes.OfType<RouterNode>();
                if (!routerNodes.IsNullOrEmpty())
                {
                    foreach (var routerNode in routerNodes)
                    {
                        var routerConfiguration = new TrakHoundRouterConfiguration();
                        routerConfiguration.Id = routerNode.Id;
                        routerConfiguration.Name = routerNode.Name;

                        // Create Route Configurations
                        var routeNodes = Diagram.Nodes.OfType<RouteNode>().Where(o => HasLink(o, routerNode.Id));
                        if (!routeNodes.IsNullOrEmpty())
                        {
                            foreach (var routeNode in routeNodes)
                            {
                                var routeConfiguration = new TrakHoundRouteConfiguration();
                                routeConfiguration.Id = routeNode.Id;

                                // Add Pattern
                                if (!string.IsNullOrEmpty(routeNode.Pattern))
                                {
                                    var patterns = routeNode.Pattern.Split(',', StringSplitOptions.TrimEntries);
                                    if (!patterns.IsNullOrEmpty())
                                    {
                                        foreach (var pattern in patterns)
                                        {
                                            routeConfiguration.AddPattern(pattern);
                                        }
                                    }
                                }

                                // Create Target Configurations
                                var targetNodes = Diagram.Nodes.OfType<TargetNode>().Where(o => HasLink(o, routeNode.Id));
                                if (!targetNodes.IsNullOrEmpty())
                                {
                                    foreach (var targetNode in targetNodes)
                                    {
                                        var targetConfiguration = SaveTarget(targetNode);
                                        routeConfiguration.AddTarget(targetConfiguration);
                                    }
                                }

                                routerConfiguration.AddRoute(routeConfiguration);
                            }
                        }

                        routerConfigurations.Add(routerConfiguration);
                    }
                }

                // Save Router Configurations
                foreach (var routerConfiguration in routerConfigurations)
                {
                    InstanceConfiguration.ConfigurationGroup.ConfigurationProfile.Add(routerConfiguration, true);
                }
            }

            await Task.Delay(500);

            notificationService.AddNotification(NotificationType.Information, "Save Successful", "Routing Configuration Saved Successfully");

            _saveLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private TrakHoundTargetConfiguration SaveTarget(TargetNode targetNode)
        {
            var targetConfiguration = new TrakHoundTargetConfiguration();
            targetConfiguration.Id = targetNode.Id;
            targetConfiguration.Source = targetNode.ConfigurationId;
            targetConfiguration.Type = targetNode.Type;

            // Create Redirect Configurations
            var redirectNodes = Diagram.Nodes.OfType<RedirectNode>().Where(o => HasLink(o, targetNode.Id));
            if (!redirectNodes.IsNullOrEmpty())
            {
                foreach (var redirectNode in redirectNodes)
                {
                    var redirectConfiguration = new TrakHoundRedirectConfiguration();
                    redirectConfiguration.Id = redirectNode.Id;
                    redirectConfiguration.Conditions = redirectNode.Conditions;

                    var options = new List<RouteRedirectOptions>();
                    if (redirectNode.IsPublishOptionEnabled) options.Add(RouteRedirectOptions.Publish);
                    if (redirectNode.IsEmptyOptionEnabled) options.Add(RouteRedirectOptions.Empty);
                    redirectConfiguration.Options = options;

                    var redirectTargetNodes = Diagram.Nodes.OfType<TargetNode>().Where(o => HasLink(o, redirectNode.Id));
                    if (!redirectTargetNodes.IsNullOrEmpty())
                    {
                        foreach (var redirectTargetNode in redirectTargetNodes)
                        {
                            var redirectTargetConfiguration = SaveTarget(redirectTargetNode);
                            redirectConfiguration.AddTarget(redirectTargetConfiguration);
                        }
                    }

                    targetConfiguration.AddRedirect(redirectConfiguration);
                }
            }

            return targetConfiguration;
        }


        private bool HasLink(NodeModel targetNode, string sourceId)
        {
            if (targetNode != null)
            {
                if (!targetNode.Ports.IsNullOrEmpty())
                {
                    var sourceFound = false;
                    var targetFound = false;

                    foreach (var port in targetNode.Ports)
                    {
                        if (!port.Links.IsNullOrEmpty())
                        {
                            foreach (var link in port.Links)
                            {
                                if (link.Source != null)
                                {
                                    var node = ((PortModel)link.Source.Model).Parent;
                                    if (node.Id == sourceId)
                                    {
                                        sourceFound = true;
                                    }
                                }

                                if (link.Target != null)
                                {
                                    var node = ((PortModel)link.Target.Model).Parent;
                                    if (node.Id == targetNode.Id)
                                    {
                                        targetFound = true;
                                    }
                                }
                            }
                        }
                    }

                    return sourceFound && targetFound;
                }
            }

            return false;
        }

        private async void NodeRemoved(NodeModel node)
        {
            if (node.GetType() == typeof(RouterNode))
            {
                var routerNode = (RouterNode)node;
                _routerConfigurations.Remove(routerNode.Id);

                await InvokeAsync(StateHasChanged);
            }
        }



        private async void AddRouter()
        {
            var routerConfiguration = new TrakHoundRouterConfiguration();
            routerConfiguration.Id = Guid.NewGuid().ToString();
            routerConfiguration.Name = TrakHoundRouter.Default;

            var node = new RouterNode(routerConfiguration.Id);
            node.Name = TrakHoundRouter.Default;

            node = Diagram.Nodes.Add(node);
            node.AddPort(new RouterInputPort(node, true, PortAlignment.Left));
            node.AddPort(new RouterOutputPort(node, true, PortAlignment.Right));

            var zoom = 1 / Diagram.Zoom;
            var relativePoint = Diagram.GetRelativePoint(0, 0);
            var mousePoint = Diagram.GetRelativeMousePoint(0, 0);
            var x = mousePoint.X - ((relativePoint.X - (Diagram.Container.Width / 2) + (271 / 2)) * zoom);
            var y = mousePoint.Y - ((relativePoint.Y - (Diagram.Container.Height / 2) + (85 / 2)) * zoom);
            node.Position = new Diagrams.Core.Geometry.Point(x, y);

            _routerConfigurations.Remove(routerConfiguration.Id);
            _routerConfigurations.Add(routerConfiguration.Id, routerConfiguration);

            node.Updated += RouterNodeUpdated;

            await InvokeAsync(StateHasChanged);
        }

        private RouterNode CreateRouterNode(TrakHoundRouterConfiguration routerConfiguration)
        {
            var node = new RouterNode(routerConfiguration.Id);
            node.Name = routerConfiguration.Name;
            node.AddPort(new RouterInputPort(node, true, PortAlignment.Left));
            node.AddPort(new RouterOutputPort(node, true, PortAlignment.Right));

            node.Updated += RouterNodeUpdated;

            return node;
        }

        private async void RouterNodeUpdated(object sender, RouterNode routerNode)
        {
            var routerConfiguration = _routerConfigurations.GetValueOrDefault(routerNode.Id);
            if (routerConfiguration != null)
            {
                routerConfiguration.Name = routerNode.Name;
            }

            await InvokeAsync(StateHasChanged);
        }

        private async void AddRoute()
        {
            var node = new RouteNode(Guid.NewGuid().ToString());
            node.Pattern = "*";
            node = Diagram.Nodes.Add(node);
            node.AddPort(new RouterInputPort(node, true, PortAlignment.Left));
            node.AddPort(new DriverOutputPort(node, true, PortAlignment.Right));
            node.AddPort(new RedirectPort(node, true, PortAlignment.Right));

            var zoom = 1 / Diagram.Zoom;
            var relativePoint = Diagram.GetRelativePoint(0, 0);
            var mousePoint = Diagram.GetRelativeMousePoint(0, 0);
            var x = mousePoint.X - ((relativePoint.X - (Diagram.Container.Width / 2) + (300 / 2)) * zoom);
            var y = mousePoint.Y - ((relativePoint.Y - (Diagram.Container.Height / 2) + (85 / 2)) * zoom);
            node.Position = new Diagrams.Core.Geometry.Point(x, y);

            await InvokeAsync(StateHasChanged);
        }

        private RouteNode CreateRouteNode(TrakHoundRouteConfiguration routeConfiguration)
        {
            var node = new RouteNode(routeConfiguration.Id);
            node.Pattern = !routeConfiguration.Patterns.IsNullOrEmpty() ? string.Join(',', routeConfiguration.Patterns) : null;
            node.AddPort(new RouterInputPort(node, true, PortAlignment.Left));
            node.AddPort(new DriverOutputPort(node, true, PortAlignment.Right));
            return node;
        }

        private async void AddTarget()
        {
            var configurationId = Guid.NewGuid().ToString();

            var node = new TargetNode(configurationId);
            node.ConfigurationId = configurationId;
            node.Name = "target";
            node = Diagram.Nodes.Add(node);
            node.AddPort(new DriverInputPort(node, true, PortAlignment.Left));
            node.AddPort(new RedirectPort(node, true, PortAlignment.Right));

            var zoom = 1 / Diagram.Zoom;
            var relativePoint = Diagram.GetRelativePoint(0, 0);
            var mousePoint = Diagram.GetRelativeMousePoint(0, 0);
            var x = mousePoint.X - ((relativePoint.X - (Diagram.Container.Width / 2) + (300 / 2)) * zoom);
            var y = mousePoint.Y - ((relativePoint.Y - (Diagram.Container.Height / 2) + (85 / 2)) * zoom);
            node.Position = new Diagrams.Core.Geometry.Point(x, y);

            await InvokeAsync(StateHasChanged);
        }

        private TargetNode CreateTargetNode(TrakHoundTargetConfiguration targetConfiguration)
        {
            var node = new TargetNode(targetConfiguration.Id);
            node.ConfigurationId = targetConfiguration.Source;
            node.Type = targetConfiguration.Type;

            if (!string.IsNullOrEmpty(targetConfiguration.Source))
            {
                switch (targetConfiguration.Type)
                {
                    case "Driver":
                        var driverConfiguration = _driverConfigurations.GetValueOrDefault(targetConfiguration.Source);
                        if (driverConfiguration != null)
                        {
                            node.Name = driverConfiguration.Name;
                        }
                        break;

                    case "Router":
                        var routerConfiguration = _routerConfigurations.GetValueOrDefault(targetConfiguration.Source);
                        if (routerConfiguration != null)
                        {
                            node.Name = routerConfiguration.Name;
                        }
                        break;
                }
            }       

            node.AddPort(new DriverInputPort(node, true, PortAlignment.Left));
            node.AddPort(new RedirectPort(node, true, PortAlignment.Right));
            return node;
        }

        private async void AddRedirect()
        {
            var configurationId = Guid.NewGuid().ToString();

            var node = new RedirectNode(configurationId);
            node.ConfigurationId = configurationId;
            node.Conditions = new string[] { "NotFound", "RouteNotConfigured", "NotAvailable" };
            node.IsPublishOptionEnabled = true;
            node.IsEmptyOptionEnabled = true;
            node = Diagram.Nodes.Add(node);
            node.AddPort(new RouterInputPort(node, true, PortAlignment.Left));
            node.AddPort(new DriverOutputPort(node, true, PortAlignment.Right));

            var zoom = 1 / Diagram.Zoom;
            var relativePoint = Diagram.GetRelativePoint(0, 0);
            var mousePoint = Diagram.GetRelativeMousePoint(0, 0);
            var x = mousePoint.X - ((relativePoint.X - (Diagram.Container.Width / 2) + (300 / 2)) * zoom);
            var y = mousePoint.Y - ((relativePoint.Y - (Diagram.Container.Height / 2) + (85 / 2)) * zoom);
            node.Position = new Diagrams.Core.Geometry.Point(x, y);

            await InvokeAsync(StateHasChanged);
        }

        private RedirectNode CreateRedirectNode(TrakHoundRedirectConfiguration redirectConfiguration)
        {
            var node = new RedirectNode(redirectConfiguration.Id);
            node.ConfigurationId = redirectConfiguration.Id;
            node.Conditions = redirectConfiguration.Conditions;
            node.IsPublishOptionEnabled = redirectConfiguration.Options != null && redirectConfiguration.Options.Contains(RouteRedirectOptions.Publish);
            node.IsEmptyOptionEnabled = redirectConfiguration.Options != null && redirectConfiguration.Options.Contains(RouteRedirectOptions.Empty);

            node.AddPort(new RouterInputPort(node, true, PortAlignment.Left));
            node.AddPort(new DriverOutputPort(node, true, PortAlignment.Right));
            return node;
        }

        private async void AddRouterTarget(TrakHoundRouterConfiguration routerConfiguration)
        {
            var targetId = Guid.NewGuid().ToString();

            var node = new TargetNode(targetId);
            node.ConfigurationId = routerConfiguration.Id;
            node.Name = routerConfiguration.Name;
            node.Type = "Router";
            node = Diagram.Nodes.Add(node);
            node.AddPort(new DriverInputPort(node, true, PortAlignment.Left));
            node.AddPort(new RedirectPort(node, true, PortAlignment.Right));

            var zoom = 1 / Diagram.Zoom;
            var relativePoint = Diagram.GetRelativePoint(0, 0);
            var mousePoint = Diagram.GetRelativeMousePoint(0, 0);
            var x = mousePoint.X - ((relativePoint.X - (Diagram.Container.Width / 2) + (300 / 2)) * zoom);
            var y = mousePoint.Y - ((relativePoint.Y - (Diagram.Container.Height / 2) + (85 / 2)) * zoom);
            node.Position = new Diagrams.Core.Geometry.Point(x, y);

            await InvokeAsync(StateHasChanged);
        }

        private async void AddDriverTarget(ITrakHoundDriverConfiguration driverConfiguration)
        {
            var targetId = Guid.NewGuid().ToString();

            var node = new TargetNode(targetId);
            node.ConfigurationId = driverConfiguration.Id;
            node.Name = driverConfiguration.Name;
            node.Type = "Driver";
            node = Diagram.Nodes.Add(node);
            node.AddPort(new DriverInputPort(node, true, PortAlignment.Left));
            node.AddPort(new RedirectPort(node, true, PortAlignment.Right));

            var zoom = 1 / Diagram.Zoom;
            var relativePoint = Diagram.GetRelativePoint(0, 0);
            var mousePoint = Diagram.GetRelativeMousePoint(0, 0);
            var x = mousePoint.X - ((relativePoint.X - (Diagram.Container.Width / 2) + (300 / 2)) * zoom);
            var y = mousePoint.Y - ((relativePoint.Y - (Diagram.Container.Height / 2) + (85 / 2)) * zoom);
            node.Position = new Diagrams.Core.Geometry.Point(x, y);

            await InvokeAsync(StateHasChanged);
        }
    }
}
