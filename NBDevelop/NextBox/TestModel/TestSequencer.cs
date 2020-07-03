using NextBox.TestModel.SubClass;
using NextBox.TestModel.TestDataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NextBox.TestModel
{
    public delegate void EventInsertDataRow(TestDataRow row);
    public class TestSequencer
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int index;
        public TestEngine engine;
        public TestStatus testStatus = TestStatus.NotSet;
        public event EventFromTestSequencerDelegate eventFromSequencer;
        public event EventInsertDataRow insterDataRow;
        public DateTime startTime;
        public DateTime endTime;
        public List<TMRecord> failList = new List<TMRecord>();
        public int itemFinishedCount = 0;

        private TestCoreManager coreManager = TestCoreManager.Instance;
        private Dictionary<string, string> FOMs = new Dictionary<string, string>();

        #region 开始主测试序列
        public void start()
        {
            TMReport report = new TMReport();
            report.slotID = "Unit-" + (index + 1).ToString();
            report.recordsList = new List<TMRecord>();
            report.start = DateTime.Now;
            startTime = DateTime.Now;
            printLog("start test");
            testStatus = TestStatus.Testing;
            failList = new List<TMRecord>();
            itemFinishedCount = 0;
            FOMs.Clear();
            FOMs.Add("serialnumber", coreManager.snList[index]);
            FOMs.Add("channel", "12");
            FOMs.Add("factory_name", "ITKS");
            FOMs.Add("line_number", "SMT-SENSOR");
            FOMs.Add("test_str", "th");
            
            engine.syncTaskFinishSignal = false;
            
            List<TMTestItem> testItemsArr = coreManager.testplan.itemList;
            for (int i = 0; i < testItemsArr.Count; i++)
            {
                
                TMTestItem item =(TMTestItem)testItemsArr[i];
                printLog(string.Format("item tid:{0} function:{1}",item.tid,item.function));

                TMRecord record = new TMRecord();
                record.item = item;
                Thread.Sleep(20);
                if (itemShouldSkip(item) == true)
                {
                    record.result = TestStatus.Skip;
                }
                else
                {
                    replaceParam1Value(ref item.param1);
                    printLog("param1 after replace:" + item.param1);

                    engine.executeTestRequest(item, ref record);
                    printLog(string.Format("record value:{0}", record.measurement));

                    checkParam2ToSave(item.param2, record.measurement);
                    

                }
                record.end = DateTime.Now;
                record.duration = Math.Round(record.end.Subtract(record.start).TotalSeconds,4);

                printLog(string.Format("item:{0} value:{1} result:{2}", item.tid, record.measurement, record.result));
                if (record.result == TestStatus.Fail || record.result == TestStatus.Error || record.result == TestStatus.NotSet)
                {
                    failList.Add(record);
                }
                report.recordsList.Add(record);

                //更新detail view
                TestDataRow dataRow = new TestDataRow();
                dataRow.initWith(i+1,item.group,item.tid,record.result,record.measurement,item.low,item.up,item.units,record.duration,record.failureInfo);
                insterDataRow(dataRow);
                itemFinishedCount += 1;
            }
            endTime = DateTime.Now;
            printLog("finished test");
            if (itemFinishedCount == coreManager.testplan.itemList.Count && failList.Count == 0)
            {
                testStatus = TestStatus.Pass;
            }
            else
            {
                testStatus = TestStatus.Fail;
            }

            foreach (string key in FOMs.Keys)
            {
                Console.WriteLine(string.Format("FOMs key: {0} value: {1}", key, FOMs[key]));
            }

            foreach(TMRecord record  in failList)
            {
                Console.WriteLine(string.Format("fail item:{0} value{1}", record.item.tid, record.measurement));
            }
            report.serialnumber = FOMs["serialnumber"];
            report.end = endTime;
            report.duration = Math.Round(endTime.Subtract(startTime).TotalSeconds, 2);
            report.failureList = failList;
            report.result = testStatus;
            Dictionary<string, object> resultDict = new Dictionary<string, object>();
            resultDict.Add("type", "finished");
            resultDict.Add("data", report);
            eventFromSequencer(index, resultDict);
        }
        #endregion

        #region 检查测试项是否被执行
        private bool itemShouldSkip(TMTestItem item)
        {
            if (item.testVAL == "")
            {
                return false;
            }
            if (item.testVAL.Equals("not work"))
            {
                return true;
            }
            if (item.testKEY.Contains("{{"))
            {
                string key = SubstringSingle(item.testKEY, "{{", "}}");
                if (FOMs.ContainsKey(key) && FOMs[key].Equals(item.testVAL))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 替换PARAM1中“{{vlue}}”的值
        private void replaceParam1Value(ref string param1)
        {
            if (param1.Contains("[[") == false) return;
            List<string> matchStrArr = SubstringMultiple(param1, "\\[\\[", "\\]\\]");
            foreach (string key in matchStrArr)
            {
                //printLog("replaceParam1 :" + key);
                if (FOMs.ContainsKey(key))
                {
                    param1 = param1.Replace(string.Format("[[{0}]]", key), FOMs[key]);
                }
            }
        }
        #endregion

        #region 储存PARAM2中“[[key]]”的值
        private void checkParam2ToSave(string param2, string value)
        {
            if (param2.Length == 0) return;
            value = value.Replace("\r", "");
            value = value.Replace("\n", "");
            if (param2.StartsWith("{{"))
            {
                string key = SubstringSingle(param2, "{{", "}}");
                if (FOMs.ContainsKey(key))
                {
                    FOMs[key] = value;
                }
                else
                {
                    FOMs.Add(key, value);
                }
            }
            else if (param2.StartsWith("<<"))
            {
                string key = SubstringSingle(param2, "<<", ">>");
                if (FOMs.ContainsKey(key))
                {
                    FOMs[key] = value;
                }
                else
                {
                    FOMs.Add(key, value);
                }
            }
        }
        #endregion

        #region 正则表达式处理字符串
        /**
         * "[[testKey]]" -> SubstringSingle(str, "\\[\\[", "\\]\\]")
         * "{{testKey}}" -> SubstringSingle(str, "{{", "}}")
         * "<<testKey>>" -> SubstringSingle(str, "<<", ">>")
         */
        /// <summary>
        /// 截取字符串中开始和结束字符串中间的字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>中间字符串</returns>
        public string SubstringSingle(string source, string startStr, string endStr)
        {
            Regex rg = new Regex("(?<=(" + startStr + "))[.\\s\\S]*?(?=(" + endStr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(source).Value;
        }
        /// <summary>
        /// （批量）截取字符串中开始和结束字符串中间的字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>中间字符串</returns>
        public List<string> SubstringMultiple(string source, string startStr, string endStr)
        {
            Regex rg = new Regex("(?<=(" + startStr + "))[.\\s\\S]*?(?=(" + endStr + "))", RegexOptions.Multiline | RegexOptions.Singleline);

            MatchCollection matches = rg.Matches(source);

            List<string> resList = new List<string>();

            foreach (Match item in matches)
                resList.Add(item.Value);

            return resList;
        }
        #endregion

        private void printLog(string txt)
        {
            log.Info(string.Format("[Sequencer] -{0}->{1}", index, txt));
            //Console.WriteLine(string.Format("[Sequencer] -{0}->{1}",index,txt));
        }

    }

   
}
