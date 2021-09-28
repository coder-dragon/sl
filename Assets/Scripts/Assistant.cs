using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SL.Configs;
using SL.Models;

namespace SL
{
    class Assistant
    {
        public MessageStatistic Statistic { get; } = new MessageStatistic();

        public GroupStatistic Groups { get; }

        public AppConfig Config { get; private set; }

        public string LastError { get; private set; }

        public bool Running { get; set; }

        public Assistant()
        {
            Groups = new GroupStatistic(this);
        }

        public void LoadConfig(string fileName)
        {
            try
            {
                var contents = File.ReadAllText(fileName);
                Config = JsonConvert.DeserializeObject<AppConfig>(contents);
                Config.Init();
                mLog.Info($"已加载{Config.gift.gifts.Length}个礼物配置");
            }
            catch (Exception e)
            {
                mLog.Error($"加载配置异常：{e.Message} {e.StackTrace}");
                throw e;
            }
        }

        public void Start()
        {
            if (mThread != null && mThread.IsAlive)
                return;
            Running = true;
            mThread = new Thread(work) { IsBackground = true };
            mThread.Start();
        }

        public void Stop()
        {
            if (mThread == null || !mThread.IsAlive)
                return;
            Running = false;
            mThread.Join(5000);
            mThread.Abort();
        }

        private void work()
        {
            mLog.Info("启动");
            while (Running)
            {
                pull();
                var delay = mRandom.Next(Config.gift.delay.min, Config.gift.delay.max);
                Thread.Sleep(delay);
            }
        }

        private void pull()
        {
            mLog.Debug("开始拉取播报");
            string text = null;
            try
            {
                var url = "https://sl-api.bianxiangapp.com:8866/hammer/winning_list";
                var ts = getTimestamp();
                var guid = getGuid();
                var hashMap = new Dictionary<string, string>
                {
                    { "guid", guid }
                };
                var sign = UrlGenerator.GenerateSignature("GET", url, hashMap, ts);
                var req = (HttpWebRequest)WebRequest.Create($"{url}?_s_={sign}&guid={guid}&_t_={ts}");
                req.ProtocolVersion = HttpVersion.Version10;
                var reader = new StreamReader(req.GetResponse().GetResponseStream());
                text = reader.ReadToEnd();
                var data = JObject.Parse(text)["data"];
                var messages = data.ToObject<RollMessage[]>();
                Array.Reverse(messages);
                var newMessages = Statistic.Append(messages);
                Groups.Append(newMessages);
                LastError = null;
            }
            catch (Exception e)
            {
                mLog.Error($"拉取播报失败：e={e} text={text}");
                LastError = "拉取播报失败" + e.Message;
            }
        }

        private static string getTimestamp()
        {
            return ((int)(DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds).ToString();
        }

        private static string getGuid()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

        private readonly Random mRandom = new Random();
        private readonly ILog mLog = LogManager.GetLogger(typeof(Assistant));
        private Thread mThread;
    }
}