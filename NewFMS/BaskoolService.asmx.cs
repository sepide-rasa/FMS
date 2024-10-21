using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace NewFMS
{
    /// <summary>
    /// Summary description for BaskoolService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BaskoolService : System.Web.Services.WebService
    {
        WCF_Weigh.WeighService servic = new WCF_Weigh.WeighService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Weigh.ClsError Err = new WCF_Weigh.ClsError();

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        
        [WebMethod]
        public string GetOnlineWeight(int WeighbridgeId, int Weight, string Password)
        {
            try
            {
                //string Pass = CodeDecode.GenerateHash(Password);
                //var q = servic.GetWeighbridgeWithFilter("fldPassword", Pass, 0, IP, out Err).Where(l => l.fldId == WeighbridgeId).FirstOrDefault();
                //if (q != null)
                //{
                
                    SignalrHub r = new SignalrHub();
                
                      r.SendWight(Weight.ToString());
                      System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\baskoolOut.txt", " Weight: " + Weight + "  " + servicCommon.GetDate(IP, out ErrCommon).fldDateTime + " ****** ");
                    return Weight.ToString();
                //}
                //else
                //{
                //    return "شما مجاز به دسترسی نمی باشد.";
                //}
            }
            catch (Exception x)
            {
                string Er = x.Message;
                if (x.InnerException != null)
                    Er += " " + x.InnerException.Message;
                return Er;
            }

        }
        [WebMethod]
        public string GetMaxWeight(int WeighbridgeId, /*string Pelak,*/ double STime, double MTime, int WeightMax, string Password)
        {
            try
            {
                //string Pass = CodeDecode.GenerateHash(Password);
                //var q = servic.GetWeighbridgeWithFilter("fldPassword", Pass, 0, IP, out Err).Where(l => l.fldId == WeighbridgeId).FirstOrDefault();
                //if (q != null)
                //{
                    //string[] pelakS = Pelak.Split('-');

                    //var q2=pelakS[0].Substring(0, 2);
                    //var Harf = pelakS[0].Substring(2, 1);
                    //var q3 = pelakS[0].Substring(3, 3);

                    //var PelakId = servicCommon.InsertPlaque_WebService(Harf, q2, q3, Convert.ToByte(pelakS[1]), IP, out ErrCommon);
                    //if (ErrCommon.ErrorType)
                    //{
                    //    return ErrCommon.ErrorMsg;
                    //}
                    DateTime dtNow = DateTime.Now;
                    DateTime Houre = dtNow.AddMilliseconds(MTime);
                    DateTime Start = dtNow.AddMilliseconds(STime);
                    servic.InsertTozin(WeighbridgeId, WeightMax, null, Houre, Start, dtNow, IP, out Err);
                    return "ذخیره با موفقیت انجام شد.";
                //}
                //else
                //{
                //    return "شما مجاز به دسترسی نمی باشد.";
                //}
            }
            catch (Exception x)
            {
                string Er = x.Message;
                if (x.InnerException != null)
                    Er += " " + x.InnerException.Message;
                return Er;
            }
        }
    }
}
