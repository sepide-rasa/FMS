using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class CurrentDate
    {
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public static string Year { get; set; }
        public  int fldYear { get; set; }
        public string fldMonth { get; set; }
        public string fldMonthName { get; set; }
        public static string Month { get; set; }
        public static byte nobatPardakht { get; set; }
        public static string CostCenter { get; set; }
        public static bool noeMostamar { get; set; }
    }
}