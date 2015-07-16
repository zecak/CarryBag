using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test_c1
{
    public class TextCName
    {
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public TextCName(string name1,string name2)
        {
            Name1 = name1;
            Name2 = name2;
        }
        public override string ToString()
        {
            return Name1+Name2;
        }
    }
}
