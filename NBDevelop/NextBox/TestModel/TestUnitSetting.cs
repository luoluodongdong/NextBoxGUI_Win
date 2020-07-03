using NextBox.TestModel.SubClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel
{
    public class TestUnitSetting
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int index;
        public event UnitNonUITaskDelegate unitNonUITaskEvent;

        public TMRecord executeNonUITask(TMTestItem item)
        {
            return unitNonUITaskEvent(item);
        }
        private void printLog(string txt)
        {
            log.Info(string.Format("[TUS] -{0}->{1}", index, txt));
            //Console.WriteLine(string.Format("[TUS] -{0}->{1}",index,txt));
        }
    }
}
