using System;
using System.Security.Cryptography;
using System.Text;

namespace SL
{
    class MD5Util
    {
        public static string Md5(string input)
        {
            var buffer = Encoding.ASCII.GetBytes(input);
            var result = mMd5.ComputeHash(buffer);
            var ret = BitConverter.ToString(result).Replace("-",string.Empty).ToLower();
            return ret;
        }

        private static MD5 mMd5 = MD5.Create();
    }
}