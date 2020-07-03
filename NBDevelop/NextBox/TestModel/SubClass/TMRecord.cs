using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.SubClass
{
    public class TMRecord
    {
        public TMTestItem item;
        public double duration = 0.0;
        public DateTime start = DateTime.Now;
        public DateTime end;
        public TestStatus result = TestStatus.NotSet;
        public string measurement = "";
        public string failureInfo = "";
    }
}
