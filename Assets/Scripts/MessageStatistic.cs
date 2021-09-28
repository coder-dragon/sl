using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using SL.Models;

namespace SL
{
    class MessageStatistic
    {
        public Dictionary<string, int> Items
        {
            get
            {
                lock (mLock)
                {
                    return new Dictionary<string, int>(mItems);
                }
            }
        }

        public int LastTotal { get; private set; }

        public int Total { get; private set; }

        public RollMessage[] Append(RollMessage[] messages)
        {
            lock (mLock)
            {
                var newMessages = extractNew(messages);

                foreach (var msg in newMessages)
                {
                    msg.No = Total + 1;
                    Total += msg.number;
                    if (mItems.TryGetValue(msg.name, out int count))
                        mItems[msg.name] = count + msg.number;
                    else
                        mItems[msg.name] = msg.number;
                    mMessageLog.Info("计数=" + Total + " " + msg.DebugString);
                }
                mList.AddRange(newMessages);

                return newMessages;
            }
        }

        public RollMessage GetLastRollMessage(string name)
        {
            lock (mLock)
            {
                for (var i = mList.Count - 1; i >= 0; i--)
                {
                    var item = mList[i];
                    if (item.name == name)
                        return item;
                }
                return null;
            }
        }

        public RollMessage[] GetTop(int count = 30)
        {
            lock (mLock)
            {
                var beginIndex = Math.Max(0, mList.Count - count);
                count = Math.Min(count, mList.Count);
                var ret = new RollMessage[count];
                for (var i = 0; i < count; i++)
                    ret[i] = mList[beginIndex + i];
                Array.Reverse(ret);
                return ret;
            }
        }

        public void Reset()
        {
            lock (mLock)
            {
                mMessageLog.Info("计数重置");
                LastTotal = Total;
                Total = 0;
                if (mList.Count > 100)
                    mList.RemoveRange(0, mList.Count - 100);
                mItems.Clear();
            }
        }

        private bool checkListTail(int startIndex, RollMessage[] message)
        {
            var count = mList.Count - startIndex;
            for (var i = 0; i < count; i++)
            {
                var inList = mList[startIndex + i];
                var target = message[i];
                if (inList.GetHashCode() != target.GetHashCode())
                    return false;
            }
            return true;
        }

        private RollMessage[] extractNew(RollMessage[] messages)
        {
            if (messages.Length > mList.Count)
                return messages;


            var startIndex = mList.Count - messages.Length;
            for (var i = 0; i < messages.Length; i++)
            {
                if (checkListTail(startIndex + i, messages))
                {
                    var ret = new List<RollMessage>();
                    for (var j = messages.Length - i; j < messages.Length; j++)
                        ret.Add(messages[j]);
                    return ret.ToArray();
                }
            }
            return messages;
        }

        private readonly Dictionary<string, int> mItems = new Dictionary<string, int>();
        private readonly List<RollMessage> mList = new List<RollMessage>();
        private readonly object mLock = new object();
        private readonly ILog mMessageLog = LogManager.GetLogger("Messages");
    }
}
