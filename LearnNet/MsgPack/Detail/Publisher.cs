using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LearnNet
{
    public class Publisher
    {
        public enum Result
        {
            Success, 
            Fail_Existing_Subscription, 
            Fail_MsgType_Mismatch,
        }

        private class Sub
        {
            public object Owner { get; set; }

            public Action<Msg> Action { get; set; }

            public uint MsgType { get; set; }
        }

        private class TypeSubs
        {
            private List<Sub> subs = new List<Sub>();
            private uint msgType = 0;

            public uint MsgType { get { return msgType;  } }

            public TypeSubs(uint msgType)
            {
                this.msgType = msgType;
            }

            public Result Subscribe(Sub sub)
            {
                if ( sub.MsgType != msgType )
                {
                    return Result.Fail_MsgType_Mismatch;
                }
                
                if ( IsDuplicate(sub) )
                {
                    return Result.Fail_Existing_Subscription;
                }

                subs.Add(sub);

                return Result.Success;
            }

            public int Post(Msg m)
            {
                int postCount = 0;

                if ( m.Type == msgType)
                {
                    var lst = new List<Action<Msg>>();

                    foreach ( var sub in subs )
                    {
                        lst.Add(sub.Action);
                    }

                    // 실행 중 Unsubscribe가 있을 수 있어 위와 같이 처리. 
                    // 더 나은 방법은??
                    foreach ( var action in lst )
                    {
                        action(m);
                        ++postCount;
                    }
                }

                return postCount;
            }

            public int GetSubscriptionCount()
            {
                return subs.Count;
            }

            public int GetSubscriptionCount(object o)
            {
                return subs.Count((x) => { return x.Owner == o; });
            }

            public void Unsubscribe(object owner, uint msgType)
            {
                subs.RemoveAll(x => x.Owner == owner && msgType == x.MsgType); 
            }

            public void Unsubscribe(object owner)
            {
                subs.RemoveAll(x => x.Owner == owner);
            }

            public bool IsDuplicate(Sub sub)
            {
                return subs.Find(x => x.Owner == sub.Owner && x.MsgType == sub.MsgType) != null;
            }
        }

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        private Dictionary<uint, TypeSubs> subs = new Dictionary<uint, TypeSubs>();

        public Result Subscribe(object o, uint msgType, Action<Msg> action)
        {
            using (var wlock = new WriteLock(rwLock))
            {
                if (subs.ContainsKey(msgType))
                {
                    var typeSubs = subs[msgType];

                    return typeSubs.Subscribe(
                        new Sub
                        {
                            Owner = o,
                            MsgType = msgType,
                            Action = action
                        });
                }
                else
                {
                    var typeSubs = new TypeSubs(msgType);
                    subs[msgType] = typeSubs;

                    return typeSubs.Subscribe(
                        new Sub
                        {
                            Owner = o,
                            MsgType = msgType,
                            Action = action
                        });
                }
            } 
        }

        public int Post(Msg m)
        {
            TypeSubs typeSubs = null;

            using (var rlock = new ReadLock(rwLock))
            {
                if (subs.ContainsKey(m.Type))
                {
                    typeSubs = subs[m.Type];
                }
            }

            if ( typeSubs != null )
            {
                typeSubs.Post(m);
            }

            return 0;
        }

        public int GetSubscriptionCount(uint msgType)
        {
            using (var rlock = new ReadLock(rwLock))
            {
                if (subs.ContainsKey(msgType))
                {
                    var typeSubs = subs[msgType];
                    return typeSubs.GetSubscriptionCount();
                }
            }

            return 0;
        }

        public int GetSubscriptionCount(object o)
        {
            int count = 0;

            using (var wlock = new WriteLock(rwLock))
            {
                foreach (var kv in subs)
                {
                    count += kv.Value.GetSubscriptionCount(o);
                }
            }

            return count;
        }

        public void Unsubscribe(object o, uint msgType)
        {
            using (var wlock = new WriteLock(rwLock))
            {
                if (subs.ContainsKey(msgType))
                {
                    var typeSubs = subs[msgType];

                    typeSubs.Unsubscribe(o, msgType);
                }
            }
        }

        public void Unsubscribe(object o)
        {
            using (var wlock = new WriteLock(rwLock))
            {
                foreach ( var kv in subs )
                {
                    kv.Value.Unsubscribe(o);
                }
            }
        }
    }
}
