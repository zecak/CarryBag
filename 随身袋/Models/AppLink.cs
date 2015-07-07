using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 随身袋.Models
{
    public enum LinkType
    {
        Sys=0,
        App=1,
        Web=2,
        Oth=3,
    }
    public class AppLink
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public Guid PID { get; set; }

        public string AppRootPath { get; set; }
        public string AppPath { get; set; }
        public string FileName { get; set; }

        public string ImgSrc { get; set; }

        public int SortNum { get; set; }

        public LinkType AppType { get; set; }
    }
}
