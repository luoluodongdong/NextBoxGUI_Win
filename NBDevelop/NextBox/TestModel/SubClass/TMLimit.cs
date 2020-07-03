using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.SubClass
{
    class TMLimit
    {
        public string low;
        public string up;
        public string units;

        public TMLimit(string l, string u, string un)
        {
            low = l;
            up = u;
            units = un;
        }
    }
}
