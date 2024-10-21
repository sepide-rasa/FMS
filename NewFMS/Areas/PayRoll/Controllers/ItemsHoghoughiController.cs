using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;
using System.IO;
using System.Data;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class ItemsHoghoughiController : Controller
    {
        // GET: PayRoll/ItemsHoghoughi
        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_Common_Pay.Comon_PayService Com_PayServic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common.CommonService ComServic = new WCF_Common.CommonService();
        WCF_Personeli.PersoneliService PersonalServic = new WCF_Personeli.PersoneliService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
        WCF_Personeli.ClsError ErrPersonal = new WCF_Personeli.ClsError();
        WCF_Common_Pay.ClsError ErrCom_Pay = new WCF_Common_Pay.ClsError();


        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetHesabType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ComServic.GetHesabTypeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCom).ToList()
                .Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Common_Pay.OBJ_ItemsHoghughi> data = null;
            data = Com_PayServic.GetItemsHoghughiWithFilter("", "", 0, IP, out ErrCom_Pay).ToList(); 
            return this.Store(data, data.Count);
        }
        public ActionResult Save(List<WCF_Common_Pay.OBJ_ItemsHoghughi> ItemsHoghughi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "ذخیره موفق";
                foreach (var item in ItemsHoghughi)
                {
                    Msg = Com_PayServic.UpdateItemsHoghughi(item.fldId, Convert.ToByte(item.fldTypeHesabId), Convert.ToByte(item.fldMostamar), Session["Username"].ToString(),
                        Session["Password"].ToString(),Convert.ToInt32(Session["OrganId"]), IP, out ErrCom_Pay);
                    if (ErrCom_Pay.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrCom_Pay.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}