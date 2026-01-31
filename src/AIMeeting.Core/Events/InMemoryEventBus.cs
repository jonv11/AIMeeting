namespace AIMeeting.Core.Events
{
    /// <summary>
    /// In-memory implementation of the event bus.
    /// Provides thread-safe pub/sub event notification.
    /// </summary>
    public class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private readonly SemaphoreSlim _mutex = new(1, 1);

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            List<Func<TEvent, Task>> handlersSnapshot;

            await _mutex.WaitAsync();
            try
            {
                var eventType = typeof(TEvent);
                if (!_subscribers.TryGetValue(eventType, out var handlers))
                {
                    return;
                }

                handlersSnapshot = handlers
                    .Cast<Func<TEvent, Task>>()
                    .ToList();
            }
            finally
            {
                _mutex.Release();
            }

            if (handlersSnapshot.Count == 0)
            {
                return;
            }

            var tasks = handlersSnapshot.Select(h => h(@event));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Subscribes to events of a specific type.
        /// </summary>
        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(TEvent);
            
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }

            _subscribers[eventType].Add(handler);

            // Return a subscription that can be disposed to unsubscribe
            return new EventSubscription<TEvent>(this, handler);
        }

        /// <summary>
        /// Unsubscribes a handler from events.
        /// </summary>
        internal void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
        {
            if (handler == null)
                return;

            var eventType = typeof(TEvent);
            if (_subscribers.TryGetValue(eventType, out var handlers))
            {
                handlers.RemoveAll(h => h.Equals(handler));
                if (handlers.Count == 0)
                {
                    _subscribers.Remove(eventType);
                }
            }
        }

        /// <summary>
        /// Internal subscription wrapper for managing handler lifecycle.
        /// </summary>
        private class EventSubscription<TEvent> : IDisposable where TEvent : class
        {
            private readonly InMemoryEventBus _eventBus;
            private readonly Func<TEvent, Task> _handler;
            private bool _disposed;

            public EventSubscription(InMemoryEventBus eventBus, Func<TEvent, Task> handler)
            {
                _eventBus = eventBus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _eventBus.Unsubscribe(_handler);
                _disposed = true;
            }
        }
    }
}
