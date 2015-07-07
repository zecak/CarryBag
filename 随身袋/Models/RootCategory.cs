using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 随身袋.Models
{
    public class RootCategory
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public Guid PID { get; set; }

        public int SortNum { get; set; }
    }
}
