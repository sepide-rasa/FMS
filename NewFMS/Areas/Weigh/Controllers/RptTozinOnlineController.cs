using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Weigh.Controllers
{
    public class RptTozinOnlineController : Controller
    {
        //
        // GET: /Weigh/RptTozinOnline/
        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();

        WCF_Weigh.WeighService service = new WCF_Weigh.WeighService();
        WCF_Weigh.ClsError Err = new WCF_Weigh.ClsError();

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
        public ActionResult GetNoeMasraf()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service.GetNoeMasrafWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetOrganization()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            var q = servicCommon.GetOrganizationWithFilter("Baskool", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCommon).ToList().Select(n => new { ID = n.fldId, Name = n.fldName });
            return this.Store(q);
        }
        public ActionResult GeneratePdfRptTozinOnline(string AzTarikh, string TaTarikh, int OrganId, byte NoeMasrafId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptVazn_BaskoolInfoTableAdapter Vazn_Baskool = new DataSet.DataSet1TableAdapters.spr_RptVazn_BaskoolInfoTableAdapter();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Vazn_Baskool.Fill(dt.spr_RptVazn_BaskoolInfo, AzTarikh, TaTarikh, OrganId,NoeMasrafId);
                GetDate.Fill(dt_Com.spr_GetDate);
                var User = servicCommon.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDBDataSet");
                Report.RegisterData(dt, "rasaFMSDBDataSet");
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Baskool\RptTozinOnline.frx");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", AzTarikh);
                Report.SetParameterValue("TaTarikh", TaTarikh);
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
    }
}
