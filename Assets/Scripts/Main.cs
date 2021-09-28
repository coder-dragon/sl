using System;
using System.Collections;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SL
{
    public class Main : MonoBehaviour
    {

        public Text Output_Text;
        private void Start()
        {
            var assistant = new Assistant();
            assistant.LoadConfig(Application.streamingAssetsPath + "/Config/配置.txt");
            assistant.Start();
            //StartCoroutine(Refresh1(assistant));
        }

        private void InitTabHeaders(Assistant assistant)
        {
            for (int i = 0; i < assistant.Config.gift.groups.Length; i++)
            {
                var group = assistant.Config.gift.groups[i];
                //TODO:
            }
        }

        private IEnumerator Refresh(Assistant assistant)
        {
            while (true)
            {
                clearText();
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
                addText(buffer.ToString());

                buffer.Clear();
                buffer.AppendLine("统计：" + assistant.Statistic.Total);
                buffer.AppendLine();
                foreach (var pair in assistant.Statistic.Items)
                {
                    buffer.AppendFormat("{0}={1}\r\n", pair.Key, pair.Value);
                }
                addText(buffer.ToString());

                buffer.Clear();
                buffer.AppendLine("分组：");
                buffer.AppendLine();
                foreach (var pair in assistant.Groups.Records)
                {
                    buffer.Append($"{pair.Key}={pair.Value}\r\n");
                }
                addText(buffer.ToString());

                var begin = DateTime.Now;
                while (DateTime.Now - begin < TimeSpan.FromSeconds(1))
                {
                    if (Input.GetKey(KeyCode.R))
                    {
                        assistant.Statistic.Reset();
                        assistant.Groups.Reset();
                    }

                    yield return null;
                }
            }
        }
        
        private IEnumerator Refresh1(string groupName, Assistant assistant)
        {
            while (true)
            {
                clearText();
                
                var buffer = new StringBuilder();
                
                buffer.Clear();
                foreach (var pair in assistant.Groups.Records)
                {
                    buffer.Append($"{pair.Key}={pair.Value}\r\n");
                }
                addText(buffer.ToString());
                
                
                buffer.AppendLine("最新播报：");
                buffer.AppendLine();
                if (!string.IsNullOrEmpty(assistant.LastError))
                    buffer.AppendLine(assistant.LastError);
                var messages = assistant.Statistic.GetTop();
                foreach (var message in messages)
                {
                    buffer.AppendLine(message.ToString());
                }
                addText(buffer.ToString());

                buffer.Clear();
                buffer.AppendLine("统计：" + assistant.Statistic.Total);
                buffer.AppendLine();
                foreach (var pair in assistant.Statistic.Items)
                {
                    buffer.AppendFormat("{0}={1}\r\n", pair.Key, pair.Value);
                }
                addText(buffer.ToString());

                var begin = DateTime.Now;
                while (DateTime.Now - begin < TimeSpan.FromSeconds(1))
                {
                    if (Input.GetKey(KeyCode.R))
                    {
                        assistant.Statistic.Reset();
                        assistant.Groups.Reset();
                    }

                    yield return null;
                }
            }
        }

        private void addText(string s)
        {
            Output_Text.text = Output_Text.text + s;
        }
        
        private void clearText()
        {
            Output_Text.text = "";
        }
    }
}