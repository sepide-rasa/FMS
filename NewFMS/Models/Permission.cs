using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Models
{
    public class Permission
    {

        public static bool haveAccess(int RolId, string UserName, string Pass, int OrganId)
        {
            WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
            WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
            string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            var q = servic_Com.GetPermissionWithFilter("HaveAcces", RolId.ToString(), 0, UserName, Pass, OrganId, IP, out Err_Com).Any();
            return q;
            return true;
        }
        public static bool CheckPID(string OrganId, string UserName, string Pass)
        {
            WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
            WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
            string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            var q = servic_Com.GetOrganizationWithFilter("fldPId", OrganId, 0, UserName, Pass,IP, out Err_Com).Any();
            return q;
        }

    }
}