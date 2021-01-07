
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
            InitializeQueueClient.Init();
            InitializeSubscriptionClient.Init();

            Console.ReadKey();
        }
    }
}
