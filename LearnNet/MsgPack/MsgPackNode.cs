using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace LearnNet
{
    /// <summary>
    /// MsgPack 메세지에 대한 Subscription과 호출 관리 
    /// 내부 큐를 사용하여 비동기로 처리되도록 한다.
    /// 
    /// </summary>
    public class MsgPackNode
    {
        private ConcurrentQueue<Msg> recvQ = new ConcurrentQueue<Msg>();
        private Dictionary<Guid, MsgPackProtocol> protocols = new Dictionary<Guid, MsgPackProtocol>();
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        private Logger logger = LogManager.GetCurrentClassLogger();

        private MsgPackProtocol acceptor;

        public MsgPackNode()
        {
            this.acceptor = new MsgPackProtocol(this);

            acceptor.Subscribe(this, (uint)MsgInternal.AcceptedForNode, (m) => { OnAccepted(m); });
        }

        public Result Listen(string address, int backLog)
        {
            logger.Debug($"Listening. Address:{address}, Backlog:{backLog}");

            return acceptor.Listen(address, backLog);
        }

        public Result Connect(string address)
        {
            var c = new MsgPackProtocol(this);

            // subscribe : connected, disconnected

            logger.Debug($"Connecting. Address:{address}");

            using (var wlock = new WriteLock(rwLock))
            {
                protocols[c.ProtocolId] = c;
            }

            c.Subscribe(this, (uint)MsgInternal.Connected, (m) => { OnConnected(m); });
            c.Subscribe(this, (uint)MsgInternal.Disconnected, (m) => { OnDisconnected(m); });

            return c.Connect(address);
        }

        public void Notify(Msg m)
        {
            recvQ.Enqueue(m);
        }

        // process internal message
        public void Process()
        {
            Msg m;

            while ( recvQ.TryDequeue(out m) )
            {
                m.Protocol.Post(m);
            }
        }

        public void Finish()
        {
            // what to do?
            // 
        }

        public void Broadcast(Msg m)
        {
            using (var rlock = new ReadLock(rwLock))
            {
                foreach ( var kv in protocols )
                {
                    kv.Value.Send(m);
                }
            }
        }

        public MsgPackProtocol Get(Guid guid)
        {
            using (var rlock = new ReadLock(rwLock))
            {
                if (protocols.ContainsKey(guid))
                {
                    return protocols[guid];
                }
            }

            return null;
        }

        public bool Has(Guid guid)
        {
            using (var rlock = new ReadLock(rwLock))
            {
                return protocols.ContainsKey(guid);
            }
        }

        /// <summary>
        /// 테스트 용도로 작성한 함수. 
        /// </summary>
        /// <returns></returns>
        public Msg Next()
        {
            Msg m;
            recvQ.TryDequeue(out m);
            return m;
        }


        private void OnAccepted(Msg m)
        {
            // m의 소켓을 갖고 새로운 프로토콜을 만든다. 

            var ma = (MsgAcceptedNode)m;
            var p = new MsgPackProtocol(this, ma.Socket);

            logger.Debug($"Accepted. New:{p.ProtocolId}, Acceptor:{m.Protocol.ProtocolId}");

            using (var wlock = new WriteLock(rwLock))
            {
                protocols[p.ProtocolId] = p;
            }

            p.Subscribe(this, (uint)MsgInternal.Disconnected, (x) => { OnDisconnected(x); });
            p.BeginRecvInternalFromNode();

        }

        private void OnConnected(Msg m)
        {
            var mc = (MsgConnected)m;

            logger.Debug($"Connected. Result:{mc.Result}, Protocol: {mc.Protocol.ProtocolId}");

            if (mc.Result != Result.Success && mc.Result != Result.Success_ActiveClose)
            {
                using (var rlock = new ReadLock(rwLock))
                {
                    protocols.Remove(mc.Protocol.ProtocolId);
                }
            }
        }

        private void OnDisconnected(Msg m)
        {
            logger.Debug($"Disconnected. Protocol: {m.Protocol.ProtocolId}");

            using (var rlock = new ReadLock(rwLock))
            {
                protocols.Remove(m.Protocol.ProtocolId);
            }
        } 
    }
}
