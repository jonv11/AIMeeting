namespace AIMeeting.Core.Tests.Events
{
    using AIMeeting.Core.Events;
    using AIMeeting.Core.Models;
    using Xunit;

    /// <summary>
    /// Tests for orchestrator events.
    /// </summary>
    public class OrchestratorEventsTests
    {
        [Fact]
        public void OrchestratorTurnRequestEvent_CanBeCreated()
        {
            // Arrange & Act
            var evt = new OrchestratorTurnRequestEvent
            {
                MeetingId = "test-meeting",
                CurrentTurnNumber = 5,
                CurrentPhase = MeetingPhase.OptionGeneration
            };

            // Assert
            Assert.Equal("test-meeting", evt.MeetingId);
            Assert.Equal(5, evt.CurrentTurnNumber);
            Assert.Equal(MeetingPhase.OptionGeneration, evt.CurrentPhase);
            Assert.True(evt.Timestamp <= DateTime.UtcNow);
        }

        [Fact]
        public void OrchestratorDecisionEvent_ContinueMeeting_CanBeCreated()
        {
            // Arrange & Act
            var evt = new OrchestratorDecisionEvent
            {
                MeetingId = "test-meeting",
                Type = DecisionType.ContinueMeeting,
                NextAgentId = "engineer-001",
                Rationale = "Engineer has expertise"
            };

            // Assert
            Assert.Equal("test-meeting", evt.MeetingId);
            Assert.Equal(DecisionType.ContinueMeeting, evt.Type);
            Assert.Equal("engineer-001", evt.NextAgentId);
            Assert.Equal("Engineer has expertise", evt.Rationale);
        }

        [Fact]
        public void OrchestratorDecisionEvent_ChangePhase_CanBeCreated()
        {
            // Arrange & Act
            var evt = new OrchestratorDecisionEvent
            {
                MeetingId = "test-meeting",
                Type = DecisionType.ChangePhase,
                NewPhase = MeetingPhase.Evaluation,
                Rationale = "Sufficient options generated"
            };

            // Assert
            Assert.Equal(DecisionType.ChangePhase, evt.Type);
            Assert.Equal(MeetingPhase.Evaluation, evt.NewPhase);
        }

        [Fact]
        public void OrchestratorDecisionEvent_EndMeeting_CanBeCreated()
        {
            // Arrange & Act
            var evt = new OrchestratorDecisionEvent
            {
                MeetingId = "test-meeting",
                Type = DecisionType.EndMeeting,
                EndReason = "Consensus reached",
                Rationale = "All agents agree"
            };

            // Assert
            Assert.Equal(DecisionType.EndMeeting, evt.Type);
            Assert.Equal("Consensus reached", evt.EndReason);
        }

        [Fact]
        public void PhaseChangeRequestedEvent_CanBeCreated()
        {
            // Arrange & Act
            var evt = new PhaseChangeRequestedEvent
            {
                OldPhase = MeetingPhase.OptionGeneration,
                NewPhase = MeetingPhase.Evaluation,
                Reason = "Three viable options proposed"
            };

            // Assert
            Assert.Equal(MeetingPhase.OptionGeneration, evt.OldPhase);
            Assert.Equal(MeetingPhase.Evaluation, evt.NewPhase);
            Assert.Equal("Three viable options proposed", evt.Reason);
        }

        [Fact]
        public void PhaseChangedEvent_CanBeCreated()
        {
            // Arrange & Act
            var evt = new PhaseChangedEvent
            {
                OldPhase = MeetingPhase.Evaluation,
                NewPhase = MeetingPhase.Decision
            };

            // Assert
            Assert.Equal(MeetingPhase.Evaluation, evt.OldPhase);
            Assert.Equal(MeetingPhase.Decision, evt.NewPhase);
        }

        [Fact]
        public void DecisionType_HasExpectedValues()
        {
            // Assert all expected decision types exist
            Assert.Equal(0, (int)DecisionType.ContinueMeeting);
            Assert.Equal(1, (int)DecisionType.ChangePhase);
            Assert.Equal(2, (int)DecisionType.EndMeeting);
        }
    }
}
