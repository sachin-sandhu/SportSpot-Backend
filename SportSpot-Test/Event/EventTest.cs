using SportSpot.Events;
using SportSpot_Test.Event.Listener;

namespace SportSpot_Test.Event
{
    [TestClass()]
    public class EventTest
    {
        [TestMethod()]
        public async Task TestNormalEvent()
        {
            // Given: An instance of the EventService and TestEventListener
            // We register a simple TestEventListener to listen to events triggered by the EventService.
            EventService eventService = new();
            TestEventListener eventListener = new();
            eventService.RegisterListener(eventListener);

            // When: An event of type OnTestEvent is fired with data "Test"
            // The event is triggered, and we expect the listener to capture the event.
            await eventService.FireEvent(new OnTestEvent { Data = "Test" });

            // Then: The listener should have received the event
            // We check if the listener's count of received events is 1 and if the data it captured matches the event data.
            Assert.AreEqual(1, eventListener.Count, "The event listener count should be 1");
            Assert.AreEqual("Test", eventListener.Data, "The event listener data should be 'Test'");
        }

        [TestMethod()]
        public async Task TestPrioEventCall()
        {
            // Given: An EventService and a PrioEventListener that handles events by priority
            // We register a listener that handles events based on predefined priorities (HIGH, MEDIUM, LOW).
            EventService eventService = new();
            PrioEventListener eventListener = new();
            eventService.RegisterListener(eventListener);

            // When: An event of type OnTestEvent is fired
            // The listener should process the event and record the call stack based on the priority.
            await eventService.FireEvent(new OnTestEvent { Data = "Test" });

            // Then: The listener should process the event in the correct priority order
            // We verify that the event listener processed the event first with HIGH priority, then MEDIUM, and finally LOW.
            Assert.AreEqual(EventPriority.HIGH, eventListener.PrioritiesCallStack[0], "The first priority in the call stack should be HIGH");
            Assert.AreEqual(EventPriority.MEDIUM, eventListener.PrioritiesCallStack[1], "The second priority in the call stack should be MEDIUM");
            Assert.AreEqual(EventPriority.LOW, eventListener.PrioritiesCallStack[2], "The third priority in the call stack should be LOW");
        }

        [TestMethod()]
        public async Task TestAsyncEventCall()
        {
            // Given: An EventService and an AsyncEventListener for asynchronous event handling
            // We register an asynchronous event listener, which is designed to process events asynchronously.
            EventService eventService = new();
            AsyncEventListener eventListener = new();
            eventService.RegisterListener(eventListener);

            // When: An event of type OnTestEvent is fired with data "Test"
            // The event is triggered and should be handled asynchronously by the listener.
            await eventService.FireEvent(new OnTestEvent { Data = "Test" });

            // Then: The listener should have processed the event correctly
            // We check if the asynchronous listener captured the event data as expected.
            Assert.AreEqual("Test", eventListener.Data, "The event listener data should be 'Test'");
        }

        [TestMethod()]
        public async Task TestMultiEventCall()
        {
            // Given: An EventService and multiple listeners (AsyncEventListener, PrioEventListener, TestEventListener)
            // We register multiple listeners that handle the same event in different ways: one asynchronously, one by priority, and one simply.
            EventService eventService = new();
            AsyncEventListener eventListener = new();
            PrioEventListener prioEventListener = new();
            TestEventListener testEventListener = new();
            eventService.RegisterListener(eventListener);
            eventService.RegisterListener(prioEventListener);
            eventService.RegisterListener(testEventListener);

            // When: An event of type OnTestEvent is fired with data "Test"
            // The event is triggered, and each listener should handle the event based on its own implementation.
            await eventService.FireEvent(new OnTestEvent { Data = "Test" });

            // Then: Each listener should have processed the event appropriately
            // We verify that the asynchronous listener and the simple listener captured the correct data.
            Assert.AreEqual("Test", eventListener.Data, "The async event listener data should be 'Test'");
            Assert.AreEqual("Test", testEventListener.Data, "The test event listener data should be 'Test'");

            // We also verify that the priority-based listener processed the event in the correct priority order.
            Assert.AreEqual(EventPriority.HIGH, prioEventListener.PrioritiesCallStack[0], "The first priority in the call stack should be HIGH");
            Assert.AreEqual(EventPriority.MEDIUM, prioEventListener.PrioritiesCallStack[1], "The second priority in the call stack should be MEDIUM");
            Assert.AreEqual(EventPriority.LOW, prioEventListener.PrioritiesCallStack[2], "The third priority in the call stack should be LOW");
        }

        // New Test Scenario: TestMultipleFiringOfEvent
        [TestMethod()]
        public async Task TestMultipleFiringOfEvent()
        {
            // Given: An EventService and a TestEventListener for monitoring multiple event firings
            // We register a listener to track how many times an event is triggered and whether it correctly handles multiple events.
            EventService eventService = new();
            TestEventListener eventListener = new();
            eventService.RegisterListener(eventListener);

            // When: The same event is fired multiple times
            // The event is triggered twice with different data ("First" and "Second").
            await eventService.FireEvent(new OnTestEvent { Data = "First" });
            await eventService.FireEvent(new OnTestEvent { Data = "Second" });

            // Then: The listener should correctly handle both events
            // We check that the listener received both events and captured the latest event data.
            Assert.AreEqual(2, eventListener.Count, "The event listener count should be 2");
            Assert.AreEqual("Second", eventListener.Data, "The event listener data should be 'Second', representing the latest event");
        }

        [TestMethod()]
        public async Task TestUnRegisterEventCall()
        {
            // Given: An EventService and multiple listeners (AsyncEventListener, PrioEventListener, TestEventListener)
            // We register multiple listeners that handle the same event in different ways: one asynchronously, one by priority, and one simply.
            EventService eventService = new();
            AsyncEventListener eventListener = new();
            PrioEventListener prioEventListener = new();
            TestEventListener testEventListener = new();
            eventService.RegisterListener(eventListener);
            eventService.RegisterListener(prioEventListener);
            eventService.RegisterListener(testEventListener);

            // Then: Assert that all listeners are registered
            // We check if the EventService correctly holds all three registered listeners.
            Assert.AreEqual(3, eventService.GetRegisteredListeners().Count, "There should be 3 registered listeners");
            Assert.IsTrue(eventService.GetRegisteredListeners().Contains(eventListener), "The async event listener should be registered");
            Assert.IsTrue(eventService.GetRegisteredListeners().Contains(prioEventListener), "The priority event listener should be registered");
            Assert.IsTrue(eventService.GetRegisteredListeners().Contains(testEventListener), "The test event listener should be registered");

            // When: We unregister the TestEventListener and PrioEventListener
            // We unregister both the TestEventListener and PrioEventListener from the EventService.
            eventService.UnRegisterListener(testEventListener);
            eventService.UnRegisterListener(prioEventListener);

            // Then: Assert that only AsyncEventListener remains registered
            // After unregistering, we check that only one listener (AsyncEventListener) remains registered.
            Assert.AreEqual(1, eventService.GetRegisteredListeners().Count, "There should be 1 registered listener");
            Assert.IsFalse(eventService.GetRegisteredListeners().Contains(testEventListener), "The test event listener should not be registered anymore");
            Assert.IsFalse(eventService.GetRegisteredListeners().Contains(prioEventListener), "The priority event listener should not be registered anymore");
            Assert.IsTrue(eventService.GetRegisteredListeners().Contains(eventListener), "The async event listener should still be registered");

            // Given: Register all listeners again
            // We re-register all listeners to ensure they will be triggered in the next event firing.
            eventService.RegisterListener(prioEventListener);
            eventService.RegisterListener(testEventListener);

            // When: An event of type OnTestEvent is fired with data "Test"
            // The event is triggered, and each listener should handle the event based on its own implementation.
            await eventService.FireEvent(new OnTestEvent { Data = "Test" });

            // Then: Each listener should have processed the event appropriately
            // We verify that the asynchronous listener and the simple listener captured the correct data.
            Assert.AreEqual("Test", eventListener.Data, "The async event listener data should be 'Test'");
            Assert.AreEqual("Test", testEventListener.Data, "The test event listener data should be 'Test'");

            // We also verify that the priority-based listener processed the event in the correct priority order.
            Assert.AreEqual(EventPriority.HIGH, prioEventListener.PrioritiesCallStack[0], "The first priority in the call stack should be HIGH");
            Assert.AreEqual(EventPriority.MEDIUM, prioEventListener.PrioritiesCallStack[1], "The second priority in the call stack should be MEDIUM");
            Assert.AreEqual(EventPriority.LOW, prioEventListener.PrioritiesCallStack[2], "The third priority in the call stack should be LOW");
        }
    }
}
