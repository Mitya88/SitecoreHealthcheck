using Healthcheck.Service.Core.Messages;
using Healthcheck.Service.Core.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.Service.Core.Senders
{
    public static class EventQueueSender
    {
        public static void Send(string type, OutGoingMessage message)
        {
            var remoteEvent = new HealthcheckStartedRemoteEvent(type, "healthcheck:started:remote", message);
            var database = Sitecore.Configuration.Factory.GetDatabase("web");
            var eventQueue = database.RemoteEvents.EventQueue;
            eventQueue.QueueEvent<HealthcheckStartedRemoteEvent>(remoteEvent, true, false);
        }
    }
}
