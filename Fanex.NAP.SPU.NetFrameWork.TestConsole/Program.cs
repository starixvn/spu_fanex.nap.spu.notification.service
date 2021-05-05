using Fanex.NAP.SPU.Notification.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.NetFrameWork.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 5; i++)
            {
                var message = $"Hello World! {i}";

                for (var j = 0; j < 5; j++)
                {
                    Console.WriteLine(DateTime.Now + "-" + message);

                    var telegramService = new NotificationService();
                    telegramService
                       .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001216907440")
                       .Message(new MessageModel("test", DateTime.Now, "test", message))
                       .SendWithDelay(10);

                    Thread.Sleep(TimeSpan.FromSeconds(10));                   
                }
            }
                

            Console.ReadKey();
        }
    }
}
