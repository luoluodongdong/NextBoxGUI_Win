using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextBox.TestModel.SubClass
{
    public class TMSnIsOK
    {
        private List<string> snList = new List<string>();

        public void init()
        {
            snList = new List<string>();
        }
        public bool checkSN(string sn)
        {
            if (sn.Length != 6)
            {
                return false;
            }
            if (snList.Count>0 && snList.Contains(sn))
            {
                return false;
            }
            //string err = "";
            //string response = SFCQuery.WebRequst("", "", "POST", ref err);
            snList.Add(sn);
            return true;
        }
    }
}
