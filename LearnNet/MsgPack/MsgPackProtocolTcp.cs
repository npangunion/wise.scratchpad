using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnNet
{
    public class MsgPackProtocol : ProtocolTcp
    {
        public MsgPackProtocol()
        {
        }

        public override void OnReceived(MemoryStream stream)
        {
        }

        public override void OnConnected(Result result, string address)
        {
        }

        public override void OnDisconnected(Result result, string address)
        {
        }
    }
}
