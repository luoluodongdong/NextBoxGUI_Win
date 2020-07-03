using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.TestDataModel
{
    public class TestDataRow
    {
        public int NO { get; set; }
        public string Group { get; set; }
        public string TID { get; set; }
        public TestStatus Status { get; set; }
        public string Value { get; set; }
        public string Low { get; set; }
        public string Up { get; set; }
        public string Units { get; set; }
        public double Duration { get; set; }
        public string Info { get; set; }
        public void initWith(int nu, string gr, string tid, TestStatus st, string mea, string lo, string u, string un, double dur, string inf)
        {
            NO = nu;
            Group = gr;
            TID = tid;
            Status = st;
            Value = mea;
            Low = lo;
            Up = u;
            Units = un;
            Duration = dur;
            Info = inf;
        }

    }
}
