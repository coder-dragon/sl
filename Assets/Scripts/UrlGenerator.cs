using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SL
{
    class UrlGenerator
    {
        public static string GenerateSignature(string method, string url, Dictionary<string,string> hashMap, string ts)
        {
            if (mIt == null)
            {
                var length = mItOrigin.Length / 2;
                mIt = mItOrigin.Substring(length - (mSecretKey.Length / 2), mSecretKey.Length);
            }
            string path = new Uri(url).LocalPath;
            var query = from pair in hashMap
                orderby pair.Key
                select $"{pair.Key}={pair.Value}";
            var args = string.Concat(query);
            var buffer = new StringBuilder();
            buffer.Append(method);
            buffer.Append(":");
            buffer.Append(path);
            buffer.Append(":");
            buffer.Append(args);
            buffer.Append(":");
            buffer.Append(ts);
            buffer.Append(":");
            buffer.Append(mIt);
            return MD5Util.Md5(buffer.ToString());
        }

        public static string GenerateUrl(string url, Dictionary<string, string> hashMap, string ts)
        {
            if(mIt == null)
            {
                var length = mItOrigin.Length / 2;
                mIt = mItOrigin.Substring(length - (mSecretKey.Length / 2), length + (mSecretKey.Length /2));
            }
            var buffer = new StringBuilder(url);
            if(hashMap.Count > 0)
            {
                var generateSignature = GenerateSignature("GET", url, hashMap, ts);
                buffer.Append("?");
                foreach (var pair in hashMap)
                    buffer.Append($"{pair.Key}={pair.Value}");
                buffer.Append($"_t_={ts}");
                buffer.Append($"_s_={generateSignature}");
            }
            return buffer.ToString();
        }

        private static string mIt;
        private static string mItOrigin = "aviapao[av*_)&po1-0UH_()&bi13iv9ash(*&nnzhfajaslv!sdiomvaopsJPj)jnbajnasjndjasn;kasnda;lsv;sjb;lwenn:Ajfwevn;sajhb;aslnefqw;lvlsaeg;ajsviasdnv;las;asdjlasn ;aiajFLVAS;LNAivn;ljf;laf30371feec4b4433cf2bcab252ab8ce2viapao[av*_)&po1-0UH_()&bi13iv9ash(*&nnz zhfajaslv!sdiomvaopsJPj)J&YQmfd9sopzxb7968798n8uiqkIJDS;AIJF;IFJ;AFAS'JJ&YQmfd9sopzxb7968798n8uiqkfuvhldkjshgubnalkjbhaskngjklxjnzbaks-qf98";
        private static string mSecretKey = "ahuivavhse143lk2ul1k23hbl1hu1fuh";
    }
}