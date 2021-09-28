using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Models
{
    class RollMessage
    {
        /// <summary>
        /// 道具名称。
        /// </summary>
        public string name;
        public string image;
        public string nickname;
        public int user_id;
        public int number;
        public int aperture_color;
        public int type;
        public int price;
        public int No { get; set; }

        public override string ToString()
        {
            return $"{No,-6} {nickname} {name}x{number}";
        }

        public override int GetHashCode()
        {
            var full = $"{user_id}{aperture_color}{name}{number}";
            return full.GetHashCode();
        }

        public string DebugString => $"{No,-6} {nickname} {user_id} {name}x{number} (color={aperture_color})";
    }
}
