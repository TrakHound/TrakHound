// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Memory
{
    public class MemoryConsumer<T> : ITrakHoundConsumer<T>
    {
        private readonly string _id;
        private readonly CancellationToken _cancellationToken;
        private readonly Func<IEnumerable<string>, T, T> _processFuncion;
        private readonly IEnumerable<string> _keys;


        public string Id => _id;

        public CancellationToken CancellationToken => _cancellationToken;

        public T InitialValue { get; set; }

        public Func<T, T> OnReceived { get; set; }

        public Func<T, Task<T>> OnReceivedAsync { get; set; }

        public Action OnDisposed { get; set; }


        /// <summary>
        /// Event handler for when a new <typeparamref name="T"/> is received from the stream
        /// </summary>
        public event EventHandler<T> Received;

        public event EventHandler<string> Disposed;


        public MemoryConsumer(Func<IEnumerable<string>, T, T> processFuncion) 
        {
            _id = Guid.NewGuid().ToString();
            _processFuncion = processFuncion;
        }

        public MemoryConsumer(IEnumerable<string> keys, Func<IEnumerable<string>, T, T> processFuncion)
        {
            _id = Guid.NewGuid().ToString();
            _processFuncion = processFuncion;
            _keys = keys;
        }


        public void Dispose() 
        {
            if (Disposed != null) Disposed.Invoke(this, Id);
        }

        public virtual bool Push(T item)
        {
            if (Received != null && item != null)
            {
                if (_processFuncion != null)
                {
                    var sendItem = _processFuncion(_keys, item);
                    if (sendItem != null)
                    {
                        Received.Invoke(this, sendItem);
                    }
                }
                else
                {
                    Received.Invoke(this, item);
                }
            }

            return true;
        }
    }
}
