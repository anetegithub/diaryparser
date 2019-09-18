using System;
using System.Collections.Generic;
using System.Text;

namespace DiaryParser
{
    public class DiaryRow
    {
        public string Text { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
        
        public DiaryRowType Type { get; set; }
    }

    public enum DiaryRowType
    {
        Action=0,
        Food=1
    }
}
