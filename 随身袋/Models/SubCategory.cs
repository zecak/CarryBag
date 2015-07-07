using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 随身袋.Models
{
    public class SubCategory
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public Guid PID { get; set; }

        public string Path { get; set; }
        public string FileName { get; set; }

        public string ImgSrc { get; set; }

    }
}
