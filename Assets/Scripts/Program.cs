using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SL
{
    class Program
    {
        private static string getTimestamp()
        {
            return ((int)(DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds).ToString();
        }

        private static string getGuid()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-","");
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            var assistant = new Assistant();
            assistant.LoadConfig("配置.txt");
            assistant.Start();

            while (true)
            {
                Console.Clear();
                var buffer = new StringBuilder();
                buffer.AppendLine("最新播报：");
                buffer.AppendLine();
                if (!string.IsNullOrEmpty(assistant.LastError))
                    buffer.AppendLine(assistant.LastError);
                var messages = assistant.Statistic.GetTop();
                foreach (var message in messages)
                {
                    buffer.AppendLine(message.ToString());
                }
                Console.WriteLine(buffer.ToString());

                buffer.Clear();
                buffer.AppendLine("统计：" + assistant.Statistic.Total);
                buffer.AppendLine();
                foreach (var pair in assistant.Statistic.Items)
                {
                    buffer.AppendFormat("{0}={1}\r\n", pair.Key, pair.Value);
                }
                Console.WriteLine(buffer.ToString());

                buffer.Clear();
                buffer.AppendLine("分组：");
                buffer.AppendLine();
                foreach (var pair in assistant.Groups.Records)
                {
                    buffer.Append($"{pair.Key}={pair.Value}\r\n");
                }
                Console.WriteLine(buffer.ToString());

                var begin = DateTime.Now;
                while (DateTime.Now - begin < TimeSpan.FromSeconds(1))
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.R)
                    {
                        assistant.Statistic.Reset();
                        assistant.Groups.Reset();
                    }
                    Thread.Sleep(10);
                }
            }
        }
    }
}
