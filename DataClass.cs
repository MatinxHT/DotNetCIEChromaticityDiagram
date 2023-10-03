using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIEChromaticityDiagram
{
    public class DataClass
    {
        public struct DataGridValue
        {
            public byte address;
            public byte Channel;
            public double ciex;
            public double ciey;
            public double cieu;
            public double ciev;
            public string? TestTime;
        }
    }
}
