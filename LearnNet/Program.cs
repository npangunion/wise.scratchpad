using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using MessagePack;
using MessagePack.Resolvers;

namespace LearnNet
{
    class Program
    {
        [MessagePackObject]
        public class MsgEcho : Msg
        {
            [Key(0)]
            public string Hello { get; set; }

            public MsgEcho()
            {
                Type = (uint)MsgInternal.End + 100;
            }
        }

        class Client
        {
            public int MessageCount { get; private set; }

            public Client()
            {
                MessageCount = 0;
            }

            public void OnConnected(Msg m)
            {
                var echo = new MsgEcho();
                m.Protocol.Subscribe(this, echo.Type, OnEcho);

                echo.Hello = $"Hello {MessageCount}";
                m.Protocol.Send(echo);
            }

            public void OnDisconnected(Msg m)
            {
            }

            private void OnEcho(Msg m)
            {
                ++MessageCount;
            }
            
        }

        class Server
        {
            public void OnAccepted(Msg m)
            {

                m.Protocol.Subscribe(this, new MsgEcho().Type, OnEcho);
            }

            public void OnEcho(Msg m)
            {
                m.Protocol.Send(m);
            }
        }

        static void Main(string[] args)
        {

            var t = DynamicObjectResolver.Instance.GetFormatter<MsgEcho>();

            // MessagePack.Formatters.TestObjectFormatter
            Console.WriteLine(t.GetType().FullName);

            // MessagePack.Resolvers.DynamicObjectResolver, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
            Console.WriteLine(t.GetType().Assembly.FullName);

            MsgSerializerFactory.Instance().Set(new MsgEcho().Type, typeof(MsgEcho));

            if (args[0] == "client")
            {
                Client client = new Client();
                MsgPackNode node = new MsgPackNode();

                node.Connect(
                    "127.0.0.1:5000", 
                    client, 
                    client.OnConnected, 
                    client.OnDisconnected);


                while ( client.MessageCount < 1000 )
                {
                    node.Process();

                    Thread.Sleep(1);                    
                } 
            }
            else
            {
                Server server = new Server();

                MsgPackNode node = new MsgPackNode();

                node.Listen("127.0.0.1:5000", 100, server, server.OnAccepted);

                while (true)
                {
                    node.Process();

                    System.Threading.Thread.Sleep(10);
                }
            } 
        }
    }
}
