using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class SharhCodDaramdController : Controller
    {
        //
        // GET: /Daramad/SharhCodDaramd/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        public ActionResult Index(int id, int FiscalYearId)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldId", id.ToString(), "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.ShomareHesabCodeDaramadId = id.ToString();
            PartialView.ViewBag.FiscalYearId = FiscalYearId.ToString();
            PartialView.ViewBag.DaramadTitle = q.fldDaramadTitle;// +"(" + q.fldDaramadCode + ")";
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetParameter(string Type, string ShomareHesabCodeDaramadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetParametreSabetWithFilter("fldNoe", Type,ShomareHesabCodeDaramadId, 0, IP, out Err).ToList().Select(c => new { fldId = c.fldNameParametreEn, fldTitle = c.fldNameParametreFa });
            return this.Store(q);
        }
        public ActionResult Save(int id, string SharhCodDaramd)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = "ذخیره موفق"; var Er = 0;
            try
            {
                servic.UpdateSharhecodeDaramd(id, SharhCodDaramd, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id, int FiscalYearId)
        {
            var q = servic.GetShomareHesabCodeDaramadDetail(Id,FiscalYearId, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldShomareHesabCodeDaramadId = q.fldId,
                fldSharhCodDaramd = q.fldSharhCodDaramd
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
