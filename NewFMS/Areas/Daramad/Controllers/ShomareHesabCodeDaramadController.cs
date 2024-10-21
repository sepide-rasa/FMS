using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class ShomareHesabCodeDaramadController : Controller
    {
        //
        // GET: /Daramad/ShomareHesabCodeDaramad/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult StatusCode(string id, bool Status)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.Status = Status;
            return PartialView;
        }
        public ActionResult UpdateStatus(int id, bool Status)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //ذخیره
                Msg = servic.UpdateStatus_CodeDaramad(id, Status, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                MsgTitle = "عملیات موفق";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
                    Er = 1;
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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadAllData(int OrganId, byte Type, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var Data = servic.ListCodeDaramad_ShomareHesabWithOrganId(OrganId, Type, FiscalYearId, IP, out Err).ToList();
            var check = servic.ListCodeDaramad_ShomareHesabWithOrganId(OrganId, Type, FiscalYearId, IP, out Err).Where(l => l.fldshomarehesabId != 0).ToList();
            int[] checkId = new int[check.Count];

            for (int i = 0; i < check.Count; i++)
            {
                checkId[i] = check[i].fldId;
            }
            return Json(new
            {
                Data = Data,
                checkId2 = checkId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("fldTreeOrgan", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> ShomareHesabVal,int FiscalYearId, string OrganId, string ShorooshenaseGhabz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; 

            try
            {
                foreach (var item in ShomareHesabVal)
                {
                    if (item.fldId == 0)
                    {
                        servic.InsertShomareHesabCodeDaramad(item.fldShomareHesadId, item.fldCodeDaramadId, Convert.ToInt32(OrganId), item.fldShorooshenaseGhabz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        servic.UpdateShomareHesabCodeDaramad(item.fldId, item.fldShomareHesadId, item.fldCodeDaramadId, Convert.ToInt32(OrganId), item.fldShorooshenaseGhabz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    
                }
                Msg = "ذخیره با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
