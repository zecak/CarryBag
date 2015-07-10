using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 随身袋.Models
{

    public class AppLink
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public Guid PID { get; set; }


        public string FileName { get; set; }

        /// <summary>
        /// 是否相对的
        /// </summary>
        public bool IsRelative { get; set; }

        /// <summary>
        /// 运行参数
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }


        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }
    }
}
