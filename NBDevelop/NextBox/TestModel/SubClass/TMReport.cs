using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.SubClass
{
    public class TMReport
    {
        public string slotID;
        public TestStatus result = TestStatus.NotSet;
        public DateTime start;
        public DateTime end;
        public double duration = 0;
        public List<TMRecord> recordsList;
        public List<TMRecord> failureList;
        public string serialnumber;


    }
}
