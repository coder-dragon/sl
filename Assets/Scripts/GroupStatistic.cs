using System.Collections.Generic;
using log4net;
using SL.Models;

namespace SL
{
    class GroupStatistic
    {
        public class Record
        {
            public int TotalCost;
            public int TotalPayout;

            public override string ToString()
            {
                return $"{(TotalCost-TotalPayout)/100:F2}";
            }
        }

        public Dictionary<string, Record> Records
        {
            get
            {
                lock (mLock)
                {
                    return mRecords;
                }
            }
        }

        public GroupStatistic(Assistant assistant)
        {
            mAssistant = assistant;
        }

        public void Reset()
        {
            lock (mLock)
            {
                mRecords.Clear();
            }
        }

        public void Append(RollMessage[] messages)
        {
            lock (mLock)
            {
                foreach (var msg in messages)
                {
                    var totalValue = msg.number * msg.price;
                    var groupConfig = mAssistant.Config.gift.GetGroup(msg.name);
                    if (groupConfig == null)
                    {
                        mLog.Error($"分组未定义的礼物：{msg.name}");
                        continue;
                    }

                    if (Records.TryGetValue(groupConfig.name, out var record))
                    {
                        record.TotalCost += msg.number * groupConfig.cost;
                        record.TotalPayout += totalValue;
                    }
                    else
                    {
                        record = new Record
                        {
                            TotalCost = msg.number * groupConfig.cost,
                            TotalPayout = totalValue
                        };
                        Records.Add(groupConfig.name, record);
                    }
                }
            }
        }

        private readonly object mLock = new object();
        private Assistant mAssistant;
        private readonly Dictionary<string, Record> mRecords = new Dictionary<string, Record>();
        private readonly ILog mLog = LogManager.GetLogger(typeof(Assistant));
    }
}