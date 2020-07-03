using Newtonsoft;
using NextBox.TestModel.SubClass;
using NextBox.TestModel.TestDataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NextBox.TestModel
{
    #region Test Delegate
    /// <summary>
    /// unit level nonUI task delegate
    /// 
    /// unitSetting <-> unitSettingView
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public delegate  TMRecord UnitNonUITaskDelegate(TMTestItem item);
    /// <summary>
    /// station level nonUI task delegate
    /// 
    /// stationSetting <-> stationSettingView
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public delegate TMRecord StationNonUITaskDelegate(TMRequest request);
    /// <summary>
    /// unit level call station Task delegate
    /// 
    /// engine <-> TestCoreManager
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public delegate TMRecord UnitCallStationTaskDelegate(TMRequest request);
    /// <summary>
    /// station level UI Task delegate
    /// 
    /// TestCoreManager <-> ContainerView
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public delegate TMRecord StationUITaskDelegate(TMRequest request);



    public delegate void EventFromTestCoreManagerDelegate(Dictionary<string, object> content);
    public delegate void EventFromTestSequencerDelegate(int index, Dictionary<string, object> content);

    #endregion

    public sealed class TestCoreManager
    {
        #region TestCoreManager Singleton
        private static TestCoreManager instance = null;
        private static readonly object padlock = new object();

        TestCoreManager()
        {
            
        }

        public static TestCoreManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TestCoreManager();
                    }
                    return instance;
                }
            }
        }
        #endregion
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventFromTestCoreManagerDelegate eventFromTestCoreManager;
        public event StationUITaskDelegate stationUITaskEvent;

        public List<TestSequencer> sequencersList = new List<TestSequencer>();
        public List<TestEngine> engineList = new List<TestEngine>();
        public List<TestUnitSetting> unitSettingList = new List<TestUnitSetting>();
        public Dictionary<string, object> stationConfigDict = new Dictionary<string, object>();

        public List<string> snList = new List<string>();
        public TMTestPlan testplan = new TMTestPlan();
        public int unitCount;
        public int enabledUnitsCount;
        public int finishedUnitsCount;
        //public TMReport testReport = new TMReport();
        public string coreVersion;
        public TestStationSetting stationSetting = new TestStationSetting();

        public List<bool> unitSelectedArr = new List<bool>();
        public TMRecord syncTaskRecord = new TMRecord();

        private int syncSignalCount = 0;
        private readonly object _syncEventFromSeq = new object();
        private readonly object _syncUnitCallTask = new object();

        public JsonHelper json_helper = new JsonHelper();
        private TestDataManager dataManager = TestDataManager.Instance;
        
        
        #region Init TestCoreManager
        public bool initTestCoreManager()
        {

            json_helper = new JsonHelper();
            json_helper.jsonFile = Application.StartupPath + @"\NBArchive\cfg.json";
            if (json_helper.loadCfg())
            {
                string swName = json_helper.cfgContent.Station_Settings.SoftwareName;
                printLog("software name:" + swName);
                JsonHelper.UnitSettings unit1Setting = json_helper.cfgContent.Units_Settings[0];
                JsonHelper.Device unit1_dev1 = unit1Setting.Devices[0];
                //unit1_dev1.Port = "portTest";
                //json_helper.saveCfg();
            }
            else
            {
                printLog("load cfg.json failure!");
                return false;
            }

            unitCount = json_helper.cfgContent.Units_Settings.Count;
            enabledUnitsCount = unitCount;
            finishedUnitsCount = 0;
            coreVersion = "WIN_TCM_200616_231";
            string testplanFile = Application.StartupPath + @"\NBArchive\Testplan\HS_Dummy_test_plan.csv";
            bool status = testplan.loadTestplan(testplanFile);
            if(status == false)
            {
                printLog("load testplan failure!");
                return false;
            }
            printLog("testplan item 0 ->" + testplan.itemList[0].description);

            for(int i = 0; i < unitCount; i++)
            {
                TestSequencer sequencer = new TestSequencer();
                sequencer.index = i;
                sequencer.testStatus = TestStatus.NotSet;
                sequencer.eventFromSequencer += new EventFromTestSequencerDelegate(eventFromSequencer);
                

                TestEngine engine = new TestEngine();
                engine.index = i;
                engine.unitCallStationTaskEvent += new UnitCallStationTaskDelegate(unitCallStationTaskEvent);
                

                TestUnitSetting unitSetting = new TestUnitSetting();
                unitSetting.index = i;

                engine.unitSet = unitSetting;

                sequencer.engine = engine;

                sequencersList.Add(sequencer);
                engineList.Add(engine);
                unitSettingList.Add(unitSetting);

                unitSelectedArr.Add(true);
            }
            dataManager.initDataModel();
            for (int i = 0; i < unitCount; i++)
            {
                dataManager.insertDataTable(string.Format("Unit-{0}", i));
            }
            return true;
        }
        #endregion

        #region  Sub Model I/0 OutputEvent
        public void outputEvent(Dictionary<string,object> content){
            string type = (string)content["type"];
            if (type.Equals("unitSelected")) //from unit view
            {
                int index = (int)content["index"];
                bool status = (bool)content["status"];
                unitSelectedArr[index] = status;

                enabledUnitsCount += status ? 1 : -1;

                for(int i = 0; i < unitSelectedArr.Count; i++)
                {
                    printLog(string.Format("unit[{0}] selected status:{1}",i,unitSelectedArr[i]));
                }
            }
        }

        #endregion

        #region Start Test
        public void startTest()
        {
            finishedUnitsCount = 0;

            Dictionary<string, object> dict1 = new Dictionary<string, object>();
            dict1.Add("type", "start");
            eventFromTestCoreManager(dict1);

            for(int i = 0; i < sequencersList.Count; i++)
            {
                if((bool)unitSelectedArr[i] == true)
                {
                    TestSequencer sequencer = (TestSequencer)sequencersList[i];

                    Thread t1 = new Thread(new ThreadStart(sequencer.start));
                    t1.IsBackground = true;
                    t1.Start();
                }
                else
                {
                    printLog(string.Format("unit[{0}] disabled",i+1));
                }
                //sequencer.start();
            }
        }
        #endregion

        #region Event Form Sequencer
        public void eventFromSequencer(int index, Dictionary<string, object> content)
        {
            lock (_syncEventFromSeq)
            {
                string type = (string)content["type"];
                if (type.Equals("finished"))
                {
                    TMReport report = (TMReport)content["data"];
                    TestStatus result = report.result;
                    printLog(string.Format("sequencer[{0}] test result:{1}", index, result));
                    //testReport.unitResultArr[index] = result;

                    SaveSlotReport(report);

                    Dictionary<string, object> dict1 = new Dictionary<string, object>();
                    dict1.Add("type", "unitFinished");
                    dict1.Add("index", index);
                    dict1.Add("result", result);
                    eventFromTestCoreManager(dict1);

                    finishedUnitsCount += 1;
                    if(finishedUnitsCount == enabledUnitsCount)
                    {
                        printLog("all sequencers finished");

                        Dictionary<string, object> dict2 = new Dictionary<string, object>();
                        dict2.Add("type", "allFinished");
                        dict2.Add("report", "");
                        eventFromTestCoreManager(dict2);

                    }
                }
            }

        }
        #endregion

        #region Station Level 测试请求管理(UI/NonUI, Async/Sync)

        private TMRecord unitCallStationTaskEvent(TMRequest request)
        {
            lock (_syncUnitCallTask)
            {
                TMTestItem item = request.testItem;
                printLog("unit call station task func:" + item.function);
                //Thread.Sleep(1000);
                TMRecord record = new TMRecord();
                record.result = TestStatus.NotSet;

                if (item.function.Equals(Function.StationNonUIFuncAsync))
                {
                    record = stationSetting.executeNonUITask(request);
                }
                else if (item.function.Equals(Function.StationUIFuncAsync))
                {
                    record = stationUITaskEvent(request);
                }
                else if (item.function.Equals(Function.StationNonUIFuncSync))
                {
                    //同步信号累加
                    syncSignalCount += 1;
                    //同步信号数量达到测试状态unit数量（过程中某Unit提前达到结束状态）
                    if (syncSignalCount >= enabledUnitsCount - finishedUnitsCount)
                    {
                        //同步信号清零
                        syncSignalCount = 0;
                        syncTaskRecord = new TMRecord();
                        syncTaskRecord.result = TestStatus.NotSet;
                        //执行同步任务
                        Thread t1 = new Thread(new ParameterizedThreadStart(executeSyncTask));
                        t1.IsBackground = true;
                        t1.Start(request);
                    }
                }
                else if (item.function.Equals(Function.StationUIFuncSync))
                {
                    //同步信号累加
                    syncSignalCount += 1;
                    //同步信号数量与测试状态unit数量相同
                    if (syncSignalCount == enabledUnitsCount - finishedUnitsCount)
                    {
                        //同步信号清零
                        syncSignalCount = 0;
                        syncTaskRecord = new TMRecord();
                        syncTaskRecord.result = TestStatus.NotSet;
                        //执行同步任务
                        Thread t1 = new Thread(new ParameterizedThreadStart(executeSyncTask));
                        t1.IsBackground = true;
                        t1.Start(request);
                    }
                }
                //------------------dummy testplan function-------------------
                else if (item.function.Equals(Function.AsyncDialog))
                {
                    record = stationUITaskEvent(request);
                }
                else if (item.function.Equals(Function.SyncDialog)) //StationSyncUI
                {
                    //同步信号累加
                    syncSignalCount += 1;
                    //同步信号数量与测试状态unit数量相同
                    if (syncSignalCount == enabledUnitsCount - finishedUnitsCount)
                    {
                        //同步信号清零
                        syncSignalCount = 0;
                        syncTaskRecord = new TMRecord();
                        syncTaskRecord.result = TestStatus.NotSet;
                        //执行同步任务
                        Thread t1 = new Thread(new ParameterizedThreadStart(executeSyncTask));
                        t1.IsBackground = true;
                        t1.Start(request);
                    }
                }
                else if (item.function.Equals(Function.Fixture)) //StationSyncNonUI
                {
                    //同步信号累加
                    syncSignalCount += 1;
                    //同步信号数量达到测试状态unit数量（过程中某Unit提前达到结束状态）
                    if (syncSignalCount >= enabledUnitsCount - finishedUnitsCount)
                    {
                        //同步信号清零
                        syncSignalCount = 0;
                        syncTaskRecord = new TMRecord();
                        syncTaskRecord.result = TestStatus.NotSet;
                        //执行同步任务
                        Thread t1 = new Thread(new ParameterizedThreadStart(executeSyncTask));
                        t1.IsBackground = true;
                        t1.Start(request);
                    }
                }
                return record;

            }
            
        }

        private void executeSyncTask(object request)
        {
            TMRequest theRequest = (TMRequest)request;
            TMTestItem testItem = theRequest.testItem;
            //执行同步任务
            if (testItem.function.Equals(Function.StationNonUIFuncSync))
            {
                syncTaskRecord = stationSetting.executeNonUITask(theRequest);
            }
            else if (testItem.function.Equals(Function.StationUIFuncSync))
            {
                syncTaskRecord = stationUITaskEvent(theRequest);
            }
            //---------------dummy testplan function-------
            else if (testItem.function.Equals(Function.SyncDialog))
            {
                syncTaskRecord = stationUITaskEvent(theRequest);
            }
            else if (testItem.function.Equals(Function.Fixture))
            {
                syncTaskRecord = stationSetting.executeNonUITask(theRequest);
            }

            //释放任务完成信号
            for (int i = 0; i < sequencersList.Count; i++)
            {
                if ((bool)unitSelectedArr[i] == true)
                {
                    TestSequencer sequencer = (TestSequencer)sequencersList[i];
                    if (sequencer.testStatus == TestStatus.Testing)
                    {
                        TestEngine engine = (TestEngine)engineList[i];
                        engine.syncTaskFinishSignal = true;
                    }
                }
                else
                {
                    printLog(string.Format("executeSyncTask - unit[{0}] disabled", i + 1));
                }
                //sequencer.start();
            }

        }
        #endregion

        #region Save Test Reports
        private void SaveSlotReport(TMReport report)
        {
            string csvData = "";
            csvData += report.serialnumber + ",";
            csvData += report.result + ",";
            foreach (TMRecord record in report.failureList)
            {
                csvData += record.item.tid + "@";
            }
            csvData += ",";
            csvData += report.slotID + ",";
            csvData += report.start.ToString() + ",";
            csvData += report.end.ToString() + ",";

            foreach(TMRecord record  in report.recordsList)
            {
                string value = record.measurement;
                value = value.Replace("\r", "");
                value = value.Replace("\n", " ");
                value = value.Replace(",", "@");
                csvData += value + ",";

            }
            csvData += "\n";
            string logPath = Application.StartupPath + @"\Log\Csv";
            Save2OneCSV(logPath, csvData);
        }

        private void Save2OneCSV(string Path, string data)
        {
            //1.创建文件夹
            string dateTime = DateTime.Now.ToString("yyyy-MM");
            Path = Path + "\\" + dateTime;
            if (Directory.Exists(Path))
            {
                printLog(Path + " 此文件夹已经存在，无需创建！");
            }
            else
            {
                Directory.CreateDirectory(Path);
                printLog(Path + " 创建成功!");
            }
            //2.创建CSV文件
            dateTime = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = Path + "\\" + dateTime + ".csv";

            if (!File.Exists(fileName))
            {
                string firstLine = "SerialNumber,Result,FailList,SlotID,Test Start Time,Test End Time,";
                string secondLine = "Uppers---->,,,,,,";
                string thirdLine = "Lower---->,,,,,,";
                string fourthLine = "Units---->,,,,,,";

                for (int i = 0; i < testplan.itemList.Count; i++)
                {
                    firstLine += testplan.itemList[i].tid + ",";
                    secondLine += testplan.itemList[i].up + ",";
                    thirdLine += testplan.itemList[i].low + ",";
                    fourthLine += testplan.itemList[i].units + ",";
                }
                try
                {
                    FileStream fsTemp = new FileStream(fileName, FileMode.Append);
                    StreamWriter swTemp = new StreamWriter(fsTemp);
                    //swTemp.Write()
                    swTemp.WriteLine(firstLine);
                    swTemp.WriteLine(secondLine);
                    swTemp.WriteLine(thirdLine);
                    swTemp.WriteLine(fourthLine);
                    swTemp.Close();
                    fsTemp.Close();
                }
                catch (Exception ex)
                {
                    printLog("Save Log CSV ERROR:" + ex.ToString());
                    MessageBox.Show("Save Log CSV ERROR:" + ex.ToString());
                }
            }
            //3.写入测试数据           
            FileStream fs = new FileStream(fileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(data);
            sw.Close();
            fs.Close();
        }
        #endregion

        private void printLog(string txt)
        {
            log.Info("[TCM] ->" + txt);
            //Console.WriteLine("[TCM] ->" + txt);
        }
    }
}
