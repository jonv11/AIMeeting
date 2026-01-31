namespace AIMeeting.Core.Events
{
    /// <summary>
    /// Pub/sub event notification system for meeting events.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to publish</typeparam>
        /// <param name="event">The event instance</param>
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;

        /// <summary>
        /// Subscribes to events of a specific type.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to subscribe to</typeparam>
        /// <param name="handler">Handler to call when event is published</param>
        /// <returns>Subscription that can be disposed to unsubscribe</returns>
        IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
    }
}
