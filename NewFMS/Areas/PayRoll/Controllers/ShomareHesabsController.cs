using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class ShomareHesabsController : Controller
    {
        //
        // GET: /PayRoll/ShomareHesabs/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        public ActionResult Index(string containerId)
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
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetTypeHesab()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetHesabTypeWithFilter("", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetBankWithFilter("BankFix", "", 0, IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetShobe(string BankId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetSHobeWithFilter("fldBankId", BankId, 0,IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GeneratePDF(string BankId, string TypeHesabId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                Boolean Type = false;
                if (TypeHesabId == "1")
                    Type = true;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblShomareHesabGroupSelectTableAdapter ShomareHesab = new DataSet.DataSet1TableAdapters.spr_tblShomareHesabGroupSelectTableAdapter();

                var User = Cservic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out CErr).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId,OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                Date.Fill(dt_Com.spr_GetDate);
                ShomareHesab.Fill(dt.spr_tblShomareHesabGroupSelect, Type, Convert.ToInt32(BankId), OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptShomareHesabGroup.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Print(string BankId, string TypeHesabId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var result = new Ext.Net.MVC.PartialViewResult
            //{
            //    WrapByScriptTag = true,
            //    ContainerId = containerId,
            //    RenderMode = RenderMode.AddTo

            //};
            //this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            //result.ViewBag.BankId = BankId;
            //result.ViewBag.TypeHesabId = TypeHesabId;
            //return result;
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.BankId = BankId;
            PartialView.ViewBag.TypeHesabId = TypeHesabId;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult Save(List<WCF_PayRoll.OBJ_ShomareHesabs> ShomareHesabVal, int BankId, Boolean TypeHesab, byte fldHesabTypeId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var fldShomareHesab = "";
                var fldShomareKart = "";
                foreach (var item in ShomareHesabVal)
                {
                    //ذخیره
                    if (item.fldShomareKart == null)
                        item.fldShomareKart = "";
                    if (item.fldShomareHesab == null)
                        item.fldShomareHesab = "";
                    if (item.fldShomareHesabId == 0)
                    {
                        if (item.fldShomareHesab.Length < 10 || item.fldShomareKart.Length < 10)
                        {
                            fldShomareHesab = item.fldShomareHesab.PadLeft('0', 10);
                            fldShomareKart = item.fldShomareKart.PadLeft('0', 10);
                        }
                        else { fldShomareHesab = item.fldShomareHesab; fldShomareKart = item.fldShomareKart; }
                        Msg = servic.InsertShomareHesabs(item.fldPersonalId, item.fldShobeId, fldShomareHesab, fldShomareKart, TypeHesab, fldHesabTypeId, "", Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                        MsgTitle = "ذخیره موفق";
                    }
                    else
                    {
                        //ویرایش
                            if (item.fldShomareHesab.Length < 10 || item.fldShomareKart.Length < 10)
                            {
                                fldShomareHesab = item.fldShomareHesab.PadLeft('0', 10);
                                { fldShomareKart = item.fldShomareKart.PadLeft('0', 10); }
                            }
                            else { fldShomareHesab = item.fldShomareHesab; fldShomareKart = item.fldShomareKart; }
                            Msg = servic.UpdateShomareHesabs(item.fldShomareHesabId, item.fldPersonalId, item.fldShobeId, fldShomareHesab, fldShomareKart, TypeHesab, fldHesabTypeId, "", Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            MsgTitle = "ذخیره موفق";
                    }
                    if (Err.ErrorType == true)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er=1
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadHesab(string TypeHesabId, string BankId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                List<WCF_PayRoll.OBJ_ShomareHesabs> data = null;
                //Boolean Type = false;
                if (TypeHesabId != "null" && BankId != "null")
                {
                    //if (TypeHesabId == "1")
                    //    Type = true;
                    //data = servic.GetShomareHesabGroupWithFilter(Type, Convert.ToInt32(BankId), Convert.ToInt32(Session["OrganId"]),IP, out Err).ToList();
                    data = servic.GetShomareHesabsWithFilter("fldBankId_HesabTypeId", BankId, TypeHesabId, "",OrganId/* Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).ToList();
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult Delete(int id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                Msg = Cservic.DeleteShomareHesabeOmoomi(id, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out CErr);
                MsgTitle = "حذف موفق";
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = CErr.ErrorMsg;
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
    }
}
