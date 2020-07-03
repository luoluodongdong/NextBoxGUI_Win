using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NextBox.TestModel.TestDataModel
{
    public sealed class TestDataManager
    {
        #region TestDataManager Singleton
        private static TestDataManager instance = null;
        private static readonly object padlock = new object();
        private object _syncOperateLock = new object();
        TestDataManager()
        {

        }

        public static TestDataManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TestDataManager();
                    }
                    return instance;
                }
            }
        }
        #endregion

        
        public DataSet myDataSet = new DataSet();
        public Dictionary<string, object> dataDict = new Dictionary<string, object>();
        public void initDataModel()
        {
            _syncOperateLock = new object();
            if (myDataSet.Tables.Count > 0)
            {
                myDataSet.Clear();
                //dataDict.Clear();
            }
        }

        public void removedOldData()
        {
            foreach (string name in dataDict.Keys)
            {
                BindingList<TestDataRow> dict = (BindingList<TestDataRow>)dataDict[name];
                dict.Clear();
            }
            GC.Collect();
        }

        public void insertDataTable(string name)
        {
            lock (_syncOperateLock)
            {
                if (myDataSet.Tables.Contains(name) == false)
                {
                    myDataSet.Tables.Add(name);
                    /*
                    DataTable table = myDataSet.Tables[name];
                    table.Columns.Add("NO", typeof(int));
                    table.Columns.Add("Group", typeof(string));
                    table.Columns.Add("TID", typeof(string));
                    table.Columns.Add("Status", typeof(TestStatus));
                    table.Columns.Add("Value", typeof(string));
                    table.Columns.Add("Low", typeof(string));
                    table.Columns.Add("Up", typeof(string));
                    table.Columns.Add("Units", typeof(string));
                    table.Columns.Add("Duration", typeof(double));
                    table.Columns.Add("Info", typeof(string));*/
                    BindingList<TestDataRow> bList = new BindingList<TestDataRow>();
                    dataDict.Add(name, bList);
                }
            }
        }

        public void insertDataRow(string tableName, TestDataRow dataRow)
        {
            lock (_syncOperateLock)
            {
                /*if (myDataSet.Tables.Contains(tableName))
                {
                    DataTable table = myDataSet.Tables[tableName];
                    DataRow row = table.NewRow();
                    row["NO"] = dataRow.NO;
                    row["Group"] = dataRow.Group;
                    row["TID"] = dataRow.TID;
                    row["Status"] = dataRow.Status;
                    row["Value"] = dataRow.Value;
                    row["Low"] = dataRow.Low;
                    row["Up"] = dataRow.Up;
                    row["Units"] = dataRow.Units;
                    row["Duration"] = dataRow.Duration;
                    row["Info"] = dataRow.Info;
                    table.Rows.Add(row);
                }*/
                if (dataDict.Keys.Contains(tableName) == false)
                {
                    BindingList<TestDataRow> bList = new BindingList<TestDataRow>();
                    dataDict.Add(tableName, bList);
                    bList.Add(dataRow);

                }
                else
                {
                    BindingList<TestDataRow> bList2 = (BindingList<TestDataRow>)dataDict[tableName];
                    
                    bList2.Add(dataRow);
                }
            }
        }

        public DataTable getDataTable(string name)
        {
            return myDataSet.Tables[name];
        }

        public BindingList<TestDataRow> getBindingList(string name)
        {
            return (BindingList<TestDataRow>)dataDict[name];
        }

    }
}
