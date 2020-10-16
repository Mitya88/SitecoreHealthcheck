namespace Healthcheck.Service.Remote.EventQueue
{
    using Healthcheck.Service.Remote.EventQueue.Models.Event;
    using Sitecore.Data.Events;
    using Sitecore.Eventing;
    using Sitecore.Events;
    using Sitecore.Pipelines;
    using System;

    public class HealthcheckEventMap
    {
        /// <summary>
        /// Initializes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Initialize(PipelineArgs args)
        {
            EventManager.Subscribe<HealthcheckStartedRemoteEvent>(new Action<HealthcheckStartedRemoteEvent>(OnGenericRemoteEvent<HealthcheckStartedRemoteEvent>));
            EventManager.Subscribe<HealthcheckFinishedRemoteEvent>(new Action<HealthcheckFinishedRemoteEvent>(OnGenericRemoteEvent<HealthcheckFinishedRemoteEvent>));
        }

        /// <summary>
        /// Called when [generic remote event].
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event.</param>
        private static void OnGenericRemoteEvent<TEvent>(TEvent @event) where TEvent : IHasEventName
        {
            RemoteEventArgs<TEvent> remoteEventArgs = new RemoteEventArgs<TEvent>(@event);
            Event.RaiseEvent(@event.EventName, remoteEventArgs);
        }
    }
}