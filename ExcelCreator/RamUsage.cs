using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelCreator
{
    public class RamUsage
    {
        public int min { get; set; }
        public int max { get; set; }
        public int avg { get; set; }

        public RamUsage()
        {
            min = 0;
            max = 0;
            avg = 0;
        }
    }
}
