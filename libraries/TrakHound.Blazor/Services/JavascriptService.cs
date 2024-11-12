// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.JSInterop;
using TrakHound.Apps;

namespace TrakHound.Blazor.Services
{
    public class JavascriptService : ITrakHoundTransientAppInjectionService
    {
        private readonly IJSRuntime _jsRuntime;


        public JavascriptService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }


        public async Task InvokeVoid(string functionName, params object[] args)
        {
            if (!string.IsNullOrEmpty(functionName))
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync(functionName, args);
                }
                catch (Exception) 
                {
                    // Need to Log Exceptions
                }
            }
        }

		public async Task<T> Invoke<T>(string functionName, params object[] args)
		{
			if (!string.IsNullOrEmpty(functionName))
			{
				try
				{
					return await _jsRuntime.InvokeAsync<T>(functionName, args);
				}
				catch (Exception)
				{
					// Need to Log Exceptions
				}
			}

            return default;
		}


		public async Task FocusElement(string elementId)
        {
            await InvokeVoid("JsFunctions.focusElement", elementId);
        }

        public async Task CopyClipboard(string text)
        {
            await InvokeVoid("JsFunctions.copyClipboard", text);
        }

        public async Task<double> GetElementWidth(string elementId)
		{
			return await Invoke<double>("JsFunctions.getElementWidth", elementId);
		}

        public async Task ScrollIntoView(string elementId)
        {
            await InvokeVoid("JsFunctions.scrollIntoView", elementId);
        }

        public async Task StartTimer(string timerId, string elementId)
        {
            await InvokeVoid("JsFunctions.startTimer", timerId, elementId);
        }

        public async Task StopTimer(string timerId)
        {
            await InvokeVoid("JsFunctions.stopTimer", timerId);
        }
    }
}
