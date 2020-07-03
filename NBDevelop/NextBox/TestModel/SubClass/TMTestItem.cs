using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.SubClass
{
    public class TMTestItem
    {
        public string group;
        public string description;
        public string tid;
        public string function;
        public string param1;
        public string param2;
        public string low;
        public string up;
        public string units;
        public string timeout;
        public string testKEY;
        public string testVAL;
        public string failCount;
        public TMTestItem (string gr, string de, string ti, string fu, string p1,string p2, string lo,string u,string un,string to,string tk,string tv,string fc)
        {
            group = gr;
            description = de;
            tid = ti;
            function = fu;
            param1 = p1;
            param2 = p2;
            low = lo;
            up = u;
            units = un;
            timeout = to;
            testKEY = tk;
            testVAL = tv;
            failCount = fc;
        }
    }
}
