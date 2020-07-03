using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.SubClass
{
    public class TMRequest
    {
        public TMTestItem testItem;
        public string name;
        public int index;
        public string param;

        public TMRequest(TMTestItem item, string na, int ind, string para)
        {
            testItem = item;
            name = na;
            index = ind;
            param = para;
        }

    }
}
