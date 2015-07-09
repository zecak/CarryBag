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

        /// <summary>
        /// 程序根目录
        /// </summary>
        public string AppRootPath { get; set; }
        /// <summary>
        /// 程序所在目录
        /// </summary>
        public string AppPath { get; set; }

        
        public string FileName { get; set; }
        public string Args { get; set; }

        public string ImgSrc { get; set; }

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
