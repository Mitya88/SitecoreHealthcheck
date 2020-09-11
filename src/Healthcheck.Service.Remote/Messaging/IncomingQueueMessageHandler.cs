using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcheck.Service.Remote.Messaging
{
    public class IncomingQueueMessageHandler
    {
        public static Task ReceiveMessage(Message message, CancellationToken token)
        {
            // TODO, PROCESS and RUN Healthcheck!
            Sitecore.Diagnostics.Log.Info(Encoding.UTF8.GetString(message.Body), "MessageHandler");

            return Task.CompletedTask;
        }

        public static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Sitecore.Diagnostics.Log.Info(string.Format("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath), "MessageHandler");
            return Task.CompletedTask;
        }
    }
}
