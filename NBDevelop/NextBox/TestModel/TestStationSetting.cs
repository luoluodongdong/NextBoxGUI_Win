using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextBox.TestModel.SubClass;

namespace NextBox.TestModel
{
    public class TestStationSetting
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event StationNonUITaskDelegate stationNonUITaskEvent;
        public TMRecord executeNonUITask(TMRequest request)
        {
            printLog("execute NonUI task func:" + request.testItem.function);
            return stationNonUITaskEvent(request);
        }

        private void printLog(string txt)
        {
            log.Info("[TSS] ->" + txt);
            //Console.WriteLine("[TSS] ->" + txt);
        }
    }
}
