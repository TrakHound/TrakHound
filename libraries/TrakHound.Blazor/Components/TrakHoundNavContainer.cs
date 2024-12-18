// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using System.Diagnostics;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace TrakHound.Blazor.Components;

/// <summary>
/// A component that renders an anchor tag, automatically toggling its 'active'
/// class based on whether its 'href' matches the current URI.
/// </summary>
public class TrakHoundNavContainer : ComponentBase, IDisposable
{
    private const string DefaultHideClass = "d-none";

    private bool _isHidden;
    private string _hrefAbsolute;
    private string _class;

    /// <summary>
    /// Gets or sets the CSS class name applied to the NavLink when the
    /// current route matches the NavLink href.
    /// </summary>
    [Parameter]
    public string HideClass { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be added to the generated
    /// <c>a</c> element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets the computed CSS class based on whether or not the link is active.
    /// </summary>
    protected string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the child content of the component.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a value representing the URL matching behavior.
    /// </summary>
    [Parameter]
    public NavLinkMatch Match { get; set; }

    [Inject] 
    private NavigationManager NavigationManager { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        // We'll consider re-rendering on each location change
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        // Update computed state
        var href = (string)null;
        if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("href", out var obj))
        {
            href = Convert.ToString(obj, CultureInfo.InvariantCulture);
        }

        var absoluteUri = NavigationManager.ToAbsoluteUri(href).AbsoluteUri;
        var currentUri = NavigationManager.Uri;

        _hrefAbsolute = href == null ? null : RemoveUriQuery(absoluteUri);
        _isHidden = !ShouldMatch(RemoveUriQuery(currentUri));

        _class = (string)null;
        if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("class", out obj))
        {
            _class = Convert.ToString(obj, CultureInfo.InvariantCulture);
        }

        UpdateCssClass();
    }

    private static string RemoveUriQuery(string uri)
    {
        if (!string.IsNullOrEmpty(uri))
        {
            var i = uri.IndexOf("?");
            if (i > 0) return uri.Substring(0, i);
        }

        return uri;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // To avoid leaking memory, it's important to detach any event handlers in Dispose()
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    private void UpdateCssClass()
    {
        CssClass = _isHidden ? CombineWithSpace(_class, HideClass ?? DefaultHideClass) : _class;
    }

    private void OnLocationChanged(object sender, LocationChangedEventArgs args)
    {
        // We could just re-render always, but for this component we know the
        // only relevant state change is to the _isActive property.
        var shouldBeActiveNow = !ShouldMatch(RemoveUriQuery(args.Location));
        if (shouldBeActiveNow != _isHidden)
        {
            _isHidden = shouldBeActiveNow;
            UpdateCssClass();
            StateHasChanged();
        }
    }

    private bool ShouldMatch(string currentUriAbsolute)
    {
        if (_hrefAbsolute == null)
        {
            return false;
        }

        if (EqualsHrefExactlyOrIfTrailingSlashAdded(currentUriAbsolute))
        {
            return true;
        }

        if (Match == NavLinkMatch.Prefix
            && IsStrictlyPrefixWithSeparator(currentUriAbsolute, _hrefAbsolute))
        {
            return true;
        }

        return false;
    }

    private bool EqualsHrefExactlyOrIfTrailingSlashAdded(string currentUriAbsolute)
    {
        Debug.Assert(_hrefAbsolute != null);

        if (string.Equals(currentUriAbsolute, _hrefAbsolute, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (currentUriAbsolute.Length == _hrefAbsolute.Length - 1)
        {
            // Special case: highlight links to http://host/path/ even if you're
            // at http://host/path (with no trailing slash)
            //
            // This is because the router accepts an absolute URI value of "same
            // as base URI but without trailing slash" as equivalent to "base URI",
            // which in turn is because it's common for servers to return the same page
            // for http://host/vdir as they do for host://host/vdir/ as it's no
            // good to display a blank page in that case.
            if (_hrefAbsolute[_hrefAbsolute.Length - 1] == '/'
                && _hrefAbsolute.StartsWith(currentUriAbsolute, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!_isHidden)
        {
            builder.AddContent(0, ChildContent);
        }



        //builder.OpenElement(0, "div");

        //builder.AddMultipleAttributes(1, AdditionalAttributes);
        //builder.AddAttribute(2, "class", CssClass);
        //if (_isHidden)
        //{
        //    builder.AddAttribute(3, "aria-current", "page");
        //}
        //builder.AddContent(0, ChildContent);

        //builder.CloseElement();
    }

    private static string? CombineWithSpace(string str1, string str2)
        => str1 == null ? str2 : $"{str1} {str2}";

    private static bool IsStrictlyPrefixWithSeparator(string value, string prefix)
    {
        var prefixLength = prefix.Length;
        if (value.Length > prefixLength)
        {
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && (
                    // Only match when there's a separator character either at the end of the
                    // prefix or right after it.
                    // Example: "/abc" is treated as a prefix of "/abc/def" but not "/abcdef"
                    // Example: "/abc/" is treated as a prefix of "/abc/def" but not "/abcdef"
                    prefixLength == 0
                    || !char.IsLetterOrDigit(prefix[prefixLength - 1])
                    || !char.IsLetterOrDigit(value[prefixLength])
                );
        }
        else
        {
            return false;
        }
    }
}
