namespace Healthcheck.Service.Core.Models.Event
{
    using Healthcheck.Service.Core.Messages;
    using Sitecore.Eventing;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(HealthcheckResultMessage))]
    public class HealthcheckFinishedRemoteEvent : IHasEventName
    {
        public HealthcheckFinishedRemoteEvent(string eventName, HealthcheckResultMessage componentData)
        {
            this.EventName = eventName;
            this.ComponentData = componentData;
        }

        [DataMember]
        public string InstanceName { get; protected set; }

        [DataMember]
        public string EventName { get; protected set; }

        [DataMember]
        public HealthcheckResultMessage ComponentData { get; protected set; }
    }
}