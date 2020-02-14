using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace LearnNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Starting server listening...");

            MsgPackNode node = new MsgPackNode();

            node.Listen("127.0.0.1:5000", 100);

            while ( true )
            {
                node.Process();

                System.Threading.Thread.Sleep(10);
            }

            node.Finish();
        }
    }
}
