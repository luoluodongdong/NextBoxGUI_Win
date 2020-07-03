using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel
{

    public static class Function
    {
        //unit level - NonUI function
        //Engine发出NonUI的unit级别的测试请求，到达engine对应的unit setting，执行
        //engine <-> unit setting <-> unit setting view
        public static string UnitTestFunc = "unitTestFunc";
        public static string UnitTestDeviceFunc = "unitTestDeviceFunc";

        //station level - NonUI - async function
        //Engine发出NonUI异步的station级别的测试请求，到达station setting，按顺序依次执行
        //engine <-> test core manager <-> station setting <-> station setting view
        public static string StationNonUIFuncAsync = "stationNonUIFuncAsync";
        //station level - UI - async function
        //Engine发出UI异步的station级别的测试请求，到达container view，按顺序依次执行
        //engine <-> test core manager <-> conatiner view
        public static string StationUIFuncAsync = "stationUIFuncAsync";

        //station level - NonUI - sync function
        //Engine发出NonUI同步的station级别的测试请求，到达core manager，同步信号累加器触发测试请求，
        //到达station setting执行任务，完成后core manager向engine发送线程阻塞释放信号
        //engine <-> test core manager ----> station setting <-> station setting view
        public static string StationNonUIFuncSync = "stationNonUIFuncSync";
        //station level - UI - sync function
        //Engine发出NonUI同步的station级别的测试请求，到达core manager，同步信号累加器触发测试请求，
        //到达container view执行任务，完成后core manager向engine发送线程阻塞释放信号
        //engine <-> test core manager ----> container view
        public static string StationUIFuncSync = "stationUIFuncSync";

        //--------------dummy testplan function----------------------
        public static string CalculateFunc = "calculate";
        public static string AsyncDialog = "asyncDialog";
        public static string BoardVersion = "boardversion";
        public static string DMM = "dmm";
        public static string SyncDialog = "syncDialog";
        public static string Fixture = "fixture";
        public static string Arduino = "arduino";

    }

    public enum TestStatus
    {
        NotSet      = 0x00,
        Testing     = 0x01,
        Pass        = 0x02,
        Fail        = 0x03,
        Error       = 0x04,
        Skip        = 0x05,
    }

    class TestDefines
    {
    }
}
