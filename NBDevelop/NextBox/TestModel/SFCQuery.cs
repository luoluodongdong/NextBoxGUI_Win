using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace NextBox.TestModel
{
    public static class SFCQuery
    {
        /// <summary>
        /// SFC query
        /// </summary>
        /// <param name="url">http://172.17.32.29:5611/Bobcat/sfc_response.aspx</param>
        /// <param name="content">"c=QUERY_RECORD_PANEL&panel_sn=123&StationID=qwa&GET_SN=0&cmd=QUERY_PANEL_LOCATION"</param>
        /// <param name="method">"POST"/"GET"</param>
        /// <param name="error">ref param</param>
        /// <returns></returns>
        public static string WebRequst(string url, string content, string method, ref string error)
        {
            string result = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 15000; //15s
                req.Method = method;
                req.ContentType = "application/x-www-form-urlencoded";

                #region 添加Post 参数
                byte[] data = Encoding.UTF8.GetBytes(content);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }
            //调试输出
            Console.WriteLine("Info:SFC response:" + result);
            return result;
        }
    }
}
