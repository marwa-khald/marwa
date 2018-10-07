using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mansour
{
    class AnalysisItem
    {
        public string word { get; set; }
        public string pattern { get; set; }
        public string suffix { get; set; }
        public string prefix { get; set; }
        public string root { get; set; }
        public string parsing { get; set; }
        public string analysis { get; set; }
        public AnalysisItem(string s1, string s2, string s3, string s4, string s5, string s6, string s7)
        {
            word = s1;
            pattern = s5;
            suffix = s2;
            prefix = s3;
            root = s4;
            parsing = s6;
            analysis = s7;
        }
    }
}
