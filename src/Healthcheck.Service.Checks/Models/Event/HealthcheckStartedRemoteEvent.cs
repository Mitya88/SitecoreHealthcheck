namespace Healthcheck.Service.Core.Models.Event
{
    using Healthcheck.Service.Core.Messages;
    using Sitecore.Eventing;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(OutGoingMessage))]
    public class HealthcheckStartedRemoteEvent : IHasEventName
    {
        public HealthcheckStartedRemoteEvent(string type, string eventName, OutGoingMessage componentData)
        {
            this.Type = type;
            this.EventName = eventName;
            this.ComponentData = componentData;
        }

        [DataMember]
        public string EventName { get; protected set; }

        [DataMember]
        public OutGoingMessage ComponentData { get; protected set; }

        [DataMember]
        public string Type { get; set; }
    }
}