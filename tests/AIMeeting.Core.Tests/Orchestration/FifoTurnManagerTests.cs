using AIMeeting.Core.Orchestration;
using Xunit;

namespace AIMeeting.Core.Tests.Orchestration
{
    public class FifoTurnManagerTests
    {
        [Fact]
        public void GetNextAgent_RotatesInOrder()
        {
            var manager = new FifoTurnManager();
            manager.RegisterAgent("agent-1");
            manager.RegisterAgent("agent-2");
            manager.RegisterAgent("agent-3");

            Assert.Equal("agent-1", manager.GetNextAgent());
            Assert.Equal("agent-2", manager.GetNextAgent());
            Assert.Equal("agent-3", manager.GetNextAgent());
            Assert.Equal("agent-1", manager.GetNextAgent());
        }

        [Fact]
        public void UnregisterAgent_RemovesFromRotation()
        {
            var manager = new FifoTurnManager();
            manager.RegisterAgent("agent-1");
            manager.RegisterAgent("agent-2");

            manager.UnregisterAgent("agent-1");

            Assert.Equal("agent-2", manager.GetNextAgent());
            Assert.Equal("agent-2", manager.GetNextAgent());
        }

        [Fact]
        public void RegisterAgent_DoesNotDuplicate()
        {
            var manager = new FifoTurnManager();
            manager.RegisterAgent("agent-1");
            manager.RegisterAgent("agent-1");

            Assert.Equal("agent-1", manager.GetNextAgent());
            Assert.Equal("agent-1", manager.GetNextAgent());
        }
    }
}
