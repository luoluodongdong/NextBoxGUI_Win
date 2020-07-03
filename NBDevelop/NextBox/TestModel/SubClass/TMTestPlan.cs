using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NextBox.TestModel.SubClass
{
    public class TMTestPlan
    {
        public List<TMTestItem> itemList;
        public bool loadTestplan(string testplanFile)
        {
            itemList = new List<TMTestItem>();
            if (testplanFile == "") return false;
            if (!File.Exists(testplanFile)) return false;
            FileStream fs = null;
            StreamReader sr = null;
            bool bResult = true;
            try
            {
                fs = new FileStream(testplanFile, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs, Encoding.Default);
                string lineData = sr.ReadLine(); //line 0
                lineData = sr.ReadLine(); //line 1
                while (lineData != null)
                {
                    Console.WriteLine(lineData);
                    string[] data = lineData.Split(',');
                    
                    TMTestItem item = new TMTestItem(data[0],data[1],data[2],data[3], data[4],data[5],data[6],data[7],data[8],data[9],data[10],data[11], data[12]);
                    itemList.Add(item);

                    lineData = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("load csv testplan err:" + ex.ToString());
                bResult = false;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();

            }

            return bResult;

        }
    }
}
