using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

namespace NextBox
{
    public class JsonHelper
    {
        /*
         * 根据cfg.json文件内容建立数据结构模型
         *包含station settings,units settings节点
         */
        public class CfgModel
        {
            public StationSettings Station_Settings { get; set; }
            public List<UnitSettings> Units_Settings { get; set; }
        }
        /*
         * 建立station settings数据结构
         * 
         * **/
        public class StationSettings
        {
            public string SoftwareName { get; set; }
            public string SoftwareVersion { get; set; }
            public List<Device> Devices { get; set; }

        }
        /*
         * 建立device数据结构
         * **/
        public class Device
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string ID { get; set; }
            public string Load { get; set; }
            public string Type { get; set; }
            public string Port { get; set; }
            public string BaudRate { get; set; }

        }
        /*
         * 建立unit settings数据结构
         * **/
        public class UnitSettings
        {
            public string UnitID { get; set; }
            public List<Device> Devices { get; set; }
        }

        //cfg.json文件路径
        public string jsonFile = "";
        //模型实例对象
        public CfgModel cfgContent = null;

        public bool loadCfg()
        {
            cfgContent = null;
            try
            {
                StreamReader streamReader = new StreamReader(File.OpenRead(jsonFile));
                string str = "";
                string jsonstr;
                while ((jsonstr = streamReader.ReadLine()) != null)
                {
                    str += jsonstr;
                }
                streamReader.Close();
                Console.WriteLine(str);

                cfgContent = JsonConvert.DeserializeObject<CfgModel>(str);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            
            return true;
        }

        public bool saveCfg()
        {
            try
            {
                string output = JsonConvert.SerializeObject(cfgContent, Formatting.Indented);
                File.WriteAllText(jsonFile, output);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            
            return true;
        }
    }
}
