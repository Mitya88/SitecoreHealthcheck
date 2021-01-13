
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.ExternalChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

            var data = redis.GetCounters();

            InitializeQueueClient.Init();
            InitializeSubscriptionClient.Init();

            Console.ReadKey();
        }
    }
}
