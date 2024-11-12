// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrakHound
{
    public interface ITrakHoundConsumer : IDisposable
    {
        string Id { get; }

        CancellationToken CancellationToken { get; }

        Action OnDisposed { get; }


        event EventHandler<string> Disposed;
    }

    public interface ITrakHoundConsumer<T> : ITrakHoundConsumer
    {
        T InitialValue { get; }

        /// <summary>
        /// Event handler for when a new <typeparamref name="T"/> is received from the stream
        /// </summary>
        event EventHandler<T> Received;
    }

    public interface ITrakHoundConsumer<TInput, TOutput> : ITrakHoundConsumer<TOutput>
    {
        Func<TInput, TOutput> OnReceived { get; set; }

        Func<TInput, Task<TOutput>> OnReceivedAsync { get; set; }
    }
}
