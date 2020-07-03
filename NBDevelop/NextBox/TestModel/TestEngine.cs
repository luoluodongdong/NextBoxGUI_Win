using NextBox.TestModel.SubClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace NextBox.TestModel
{
    public class TestEngine
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int index;
        public TestUnitSetting unitSet;
        public event UnitCallStationTaskDelegate unitCallStationTaskEvent;
        //同步任务完成的信号
        public bool syncTaskFinishSignal = false;
        private TestCoreManager coreManager = TestCoreManager.Instance;

        #region 执行具体的测试function
        public void executeTestRequest(TMTestItem item, ref TMRecord record)
        {
            printLog("execute test request func:" + item.function);

            if (item.function.Equals(Function.UnitTestFunc))
            {
                record = unitSet.executeNonUITask(item);
            }
            else if (item.function.Equals(Function.UnitTestDeviceFunc))
            {
                record = unitSet.executeNonUITask(item);
            }
            else if (item.function.Equals(Function.StationNonUIFuncAsync))
            {
                TMRequest request = new TMRequest(item, "Unit", index, "");
                record = unitCallStationTaskEvent(request);
            }
            else if (item.function.Equals(Function.StationUIFuncAsync))
            {
                TMRequest request = new TMRequest(item, "Unit", index, "");
                record = unitCallStationTaskEvent(request);
            }
            else if (item.function.Equals(Function.StationNonUIFuncSync))
            {
                syncTaskFinishSignal = false;
                //发送同步请求
                TMRequest request = new TMRequest(item, "Unit", index, "");
                unitCallStationTaskEvent(request);
                //等待任务完成
                while (syncTaskIsFinished() == false)
                {
                    Thread.Sleep(50);

                }
                record = coreManager.syncTaskRecord;
            }
            else if (item.function.Equals(Function.StationUIFuncSync))
            {
                syncTaskFinishSignal = false;
                //发送同步请求
                TMRequest request = new TMRequest(item, "Unit", index, "");
                unitCallStationTaskEvent(request);
                //等待任务完成
                while (syncTaskIsFinished() == false)
                {
                    Thread.Sleep(50);

                }
                record = coreManager.syncTaskRecord;
            }
            //----------------dummy testplan function-----------------------------
            else if (item.function.Equals(Function.CalculateFunc))
            {
                string cmd = Application.StartupPath + @"\NBArchive\ExePyCmd\ExePyCmd.exe -c " + item.param1;
                string output = "";
                string errMsg = "";
                callCMD(cmd, out output, out errMsg);
                printLog("calculate output:" + output);
                output = output.Replace("\r", "");
                string[] outputData = output.Split('\n');
                record.measurement = "";
                if (outputData.Length > 0)
                {
                    int index = 0;
                    foreach (string line in outputData)
                    {
                        if (line.Contains("[ACK]"))
                        {
                            record.measurement = outputData[index - 1];
                        }
                        index += 1;
                    }

                }

                if (compareMeasurement(item, record.measurement))
                {
                    record.result = TestStatus.Pass;
                }
                else
                {
                    record.result = TestStatus.Fail;
                    record.failureInfo = "out of limit";
                }

            }
            else if (item.function.Equals(Function.AsyncDialog)) //StationAsyncUI
            {
                TMRequest request = new TMRequest(item, "Unit", index, "");
                TMRecord tempRecord = unitCallStationTaskEvent(request);
                record.measurement = tempRecord.measurement;
                record.result = TestStatus.Pass;
            }
            else if (item.function.Equals(Function.BoardVersion)) //StationAsyncNonUI
            {
                record.measurement = "boardversion.value";
            }
            else if (item.function.Equals(Function.DMM)) //UnitNonUI
            {
                TMRecord tempRecord = unitSet.executeNonUITask(item);
                record.measurement = tempRecord.measurement;
                if (compareMeasurement(item, record.measurement))
                {
                    record.result = TestStatus.Pass;
                }
                else
                {
                    record.result = TestStatus.Fail;
                    record.failureInfo = "out of limit";
                }
            }
            else if (item.function.Equals(Function.SyncDialog)) //StationSyncUI
            {
                syncTaskFinishSignal = false;
                //发送同步请求
                TMRequest request = new TMRequest(item, "Unit", index, "");
                unitCallStationTaskEvent(request);
                //等待任务完成
                while (syncTaskIsFinished() == false)
                {
                    Thread.Sleep(50);

                }
                TMRecord tempRecord = coreManager.syncTaskRecord;
                record.measurement = tempRecord.measurement;
                record.result = TestStatus.Pass;
            }
            else if (item.function.Equals(Function.Fixture)) //StationSyncNonUI
            {
                syncTaskFinishSignal = false;
                //发送同步请求
                TMRequest request = new TMRequest(item, "Unit", index, "");
                unitCallStationTaskEvent(request);
                //等待任务完成
                while (syncTaskIsFinished() == false)
                {
                    Thread.Sleep(50);

                }
                TMRecord tempRecord = coreManager.syncTaskRecord;

                record.measurement = tempRecord.measurement;
                record.result = TestStatus.Pass;
            }
            else if (item.function.Equals(Function.Arduino)) //UnitNonUI
            {
                TMRecord tempRecord = unitSet.executeNonUITask(item);
                record.measurement = tempRecord.measurement;
                if (compareMeasurement(item, record.measurement))
                {
                    record.result = TestStatus.Pass;
                }
                else
                {
                    record.result = TestStatus.Fail;
                    record.failureInfo = "out of limit";
                }
            }
            else
            {
                record.failureInfo = "Unknown Func";
            }

        }
        #endregion
        
        #region 检查测试值是否符合limit
        private bool compareMeasurement(TMTestItem item, string value)
        {
            if (item.low == "" && item.up == "") return true;
            if (value == "") return false;
            //string
            if(item.low == item.up)
            {
                return item.low.Equals(value);
            }
            //digital value
            double valueDou = 0;
            if(double.TryParse(value, out valueDou))
            {
                if (item.low == "" && item.up != "")
                {
                    double upDou = 0;
                    if (double.TryParse(item.up,out upDou))
                    {
                        return valueDou <= upDou;
                    }
                }
                else if (item.low != "" && item.up == "")
                {
                    double lowDou = 0;
                    if (double.TryParse(item.low,out lowDou))
                    {
                        return valueDou >= lowDou;
                    }
                }
                else
                {
                    double lowDou = 0;
                    double upDou = 0;
                    if (double.TryParse(item.low,out lowDou) && double.TryParse(item.up,out upDou))
                    {
                        return valueDou >= lowDou && valueDou <= upDou;
                    }
                }
            }
            return false;
        }
        #endregion

        #region 检测同步任务信号（线程安全）
        private bool syncTaskIsFinished()
        {
            lock (this)
            {
                return syncTaskFinishSignal;
            }
        }
        #endregion

        #region Execute Terminal Commands
        private void callCMD(string cmd, out string data, out string errMsg)
        {
            data = "";
            errMsg = "";
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(cmd + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            //获取cmd窗口的输出信息
            //string output = p.StandardOutput.ReadToEnd();
            StreamReader readerERR = p.StandardError;
            string line2 = readerERR.ReadLine();
            while (!readerERR.EndOfStream)
            {
                //    str += line + "  ";
                line2 = readerERR.ReadLine();
                // Console.WriteLine("ERROR:" + line2);
            }
            StreamReader reader = p.StandardOutput;
            string line = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                //    str += line + "  ";
                //line = reader.ReadLine();
                line = reader.ReadToEnd();

            }
            // Console.WriteLine("data:\r\n"+line);
            data = line;
            errMsg = line2;
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
        }
        #endregion

        private void printLog(string txt)
        {
            log.Info(string.Format("[Engine] -{0}->{1}", index, txt));
            //Console.WriteLine(string.Format("[Engine] -{0}->{1}",index,txt));
        }
    }
}
