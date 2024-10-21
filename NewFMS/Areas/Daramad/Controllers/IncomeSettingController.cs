using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;
using System.Data.SqlClient;
using System.Data;
using NewFMS.DataSet;
using System.Collections;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class IncomeSettingController : Controller
    {
        //
        // GET: /Daramad/IncomeSetting/
        WCF_Common.CommonService servic = new  WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();

        public ActionResult Index(int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.FiscalYearId = FiscalYearId;

            return result;
        }

        /*public ActionResult GetSazman()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationWithFilter("fldTreeOrgan", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }*/
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Details(int FiscalYearId)
        {
            if (Session["Username"] == null || Session["OrganId"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var organ = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
            var q = servicD.GetTanzimateDaramadWithFilter("fldOrganId", organ.fldId.ToString(),FiscalYearId, 1, IP, out ErrD).FirstOrDefault();
            if (q != null)
            {
                return Json(new
                {
                    fldAvarezId = q.fldAvarezId,
                    fldMaliyatId = q.fldMaliyatId,
                    fldTakhirId = q.fldTakhirId,
                    fldMablaghGerdKardan = q.fldMablaghGerdKardan,
                    fldDesc = q.fldDesc,
                    fldTakhirTitle = q.fldTitle_CodeTakhir,
                    fldMaliyatTitle = q.fldTitle_CodeMaliyat,
                    fldAvarezTitle = q.fldTitle_CodeAvarez,
                    fldShomareHesab = q.fldShomareHesab,
                    fldShomareHesabIdPishfarz = q.fldShomareHesabIdPishfarz,
                    fldNerkh = q.fldNerkh,
                    fldChapShenaseGhabz_Pardakht = q.fldChapShenaseGhabz_Pardakht,
                    fldShorooshenaseGhabz=q.fldShorooshenaseGhabz,
                    organId=organ.fldId,
                    fldOrganName=organ.fldName,
                    fldSumMaliyat_Avarez = q.fldSumMaliyat_Avarez
                  //  fldDefaultReportFish = q.fldDefaultReportFish
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                fldAvarezId = 0,
                fldMaliyatId = 0,
                fldTakhirId = 0,
                fldMablaghGerdKardan = "",
                fldDesc = "",
                fldTakhirTitle = "",
                fldMaliyatTitle = "",
                fldAvarezTitle = "",
                fldShomareHesab = "",
                fldShomareHesabIdPishfarz = 0,
                fldNerkh = 0,
                fldChapShenaseGhabz_Pardakht = false,
                fldSumMaliyat_Avarez=false,
                organId = organ.fldId,
                fldOrganName = organ.fldName
                //fldDefaultReportFish=0
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Session["savePathFile"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePathFile"].ToString());
                Session.Remove("savePathFile");
                Session.Remove("FileNameReport");
                System.IO.File.Delete(physicalPath);
            }
            HttpPostedFileBase file = Request.Files[0];
            if ((file.ContentType) == "application/octet-stream")
            {
                string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                file.SaveAs(savePath);
                Session["FileNameReport"] = file.FileName;
                Session["savePathFile"] = savePath;
                object r = new
                {
                    success = true,
                    name = Request.Files[0].FileName
                };
                return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "فایل غیرمجاز"
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
        }
        public ActionResult Save(WCF_Daramad.OBJ_TanzimateDaramad setting)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;int ReportFileID = 0;
            try
            {
                //byte[] report_file = null; string FileName = "", e = ""; 
                //if (Session["savePathFile"] != null)
                //{
                //    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFile"].ToString()));
                //    report_file = stream.ToArray();
                //    FileName = Session["FileNameReport"].ToString();
                //    e = Path.GetExtension(Session["savePathFile"].ToString());
                //    var lenght = stream.Length;
                //}
                //else if (setting.fldDefaultReportFish == null || setting.fldDefaultReportFish == 0)
                //{
                //    var f = Server.MapPath("~/Reports/Daramad/DaramadFish.frx");
                //    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(f.ToString()));
                //    report_file = stream.ToArray();
                //    FileName = "DaramadFish.frx";
                //    e = ".frx";
                //}
                //ذخیره
                MsgTitle = "ذخیره موفق";
                Msg =  servicD.InsertTanzimateDaramad(null, null, setting.fldTakhirId,setting.fldMablaghGerdKardan,
                    setting.fldOrganId, setting.fldNerkh, setting.fldChapShenaseGhabz_Pardakht, setting.fldShorooshenaseGhabz, setting.fldShomareHesabIdPishfarz,
                    setting.fldSumMaliyat_Avarez, setting.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);

                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
                    Er = 1;
                }
                //if (Session["savePathFile"] != null)
                //{
                //    string physicalPath = System.IO.Path.Combine(Session["savePathFile"].ToString());
                //    System.IO.File.Delete(physicalPath);
                //    Session.Remove("savePathFile");
                //}
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
                ReportFileID = ReportFileID,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
