using AIMeeting.Core.Events;
using Xunit;

namespace AIMeeting.Core.Tests.Events
{
    public class InMemoryEventBusTests
    {
        [Fact]
        public async Task PublishAsync_NotifiesSubscribers()
        {
            var eventBus = new InMemoryEventBus();
            var received = new List<string>();

            eventBus.Subscribe<TurnCompletedEvent>(evt =>
            {
                received.Add(evt.AgentId);
                return Task.CompletedTask;
            });

            await eventBus.PublishAsync(new TurnCompletedEvent
            {
                AgentId = "agent-1",
                Message = "hello"
            });

            Assert.Single(received);
            Assert.Equal("agent-1", received[0]);
        }

        [Fact]
        public async Task PublishAsync_DisposedSubscription_DoesNotReceive()
        {
            var eventBus = new InMemoryEventBus();
            var received = new List<string>();

            var subscription = eventBus.Subscribe<TurnStartedEvent>(evt =>
            {
                received.Add(evt.AgentId);
                return Task.CompletedTask;
            });

            subscription.Dispose();

            await eventBus.PublishAsync(new TurnStartedEvent
            {
                AgentId = "agent-1",
                TurnNumber = 1
            });

            Assert.Empty(received);
        }

        [Fact]
        public async Task PublishAsync_MultipleSubscribers_AllReceive()
        {
            var eventBus = new InMemoryEventBus();
            var first = 0;
            var second = 0;

            eventBus.Subscribe<MeetingEndedEvent>(evt =>
            {
                first++;
                return Task.CompletedTask;
            });

            eventBus.Subscribe<MeetingEndedEvent>(evt =>
            {
                second++;
                return Task.CompletedTask;
            });

            await eventBus.PublishAsync(new MeetingEndedEvent
            {
                EndReason = "done"
            });

            Assert.Equal(1, first);
            Assert.Equal(1, second);
        }
    }
}
