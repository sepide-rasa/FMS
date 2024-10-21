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
    public class PatternReportController : Controller
    {
        //
        // GET: /Daramad/PatternReport/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();

        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        public ActionResult IndexNew(string ShomareHesabCodeDaramadId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            
            var result = new Ext.Net.MVC.PartialViewResult();
            
            var isFile = 0;
            var c = servic.GetShomareHesabCodeDaramadWithFilter("fldId", ShomareHesabCodeDaramadId,"",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            if (c.fldReportFileId == null)
                isFile = 1;
            result.ViewBag.ShomareHesabCodeDaramadId = ShomareHesabCodeDaramadId;
            result.ViewBag.isFile = isFile;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Index(string ShomareHesabCodeDaramadId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();

            int? HaveFile = 0;
            var c = servic.GetShomareHesabCodeDaramadWithFilter("fldId", ShomareHesabCodeDaramadId, "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            if (c.fldReportFileId != null)
                HaveFile = c.fldReportFileId;
            result.ViewBag.ShomareHesabCodeDaramadId = ShomareHesabCodeDaramadId;
            result.ViewBag.HaveFile = HaveFile.ToString();
            return result;
        }
        public ActionResult GetPatternFish()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPatternFishWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Name = c.fldName });
            return this.Store(q);
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
        public ActionResult Save(int ShomareHesabCodeDaramadId, int PatternFishId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                byte[] report_file = null; string FileName = "", e = "";
                if (Session["savePathFile"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFile"].ToString()));
                    report_file = stream.ToArray();
                    FileName = Session["FileNameReport"].ToString();
                    e = Path.GetExtension(Session["savePathFile"].ToString());
                    var lenght = stream.Length;
                }
                else
                {
                    return Json(new
                    {
                        Msg = "فایل مورد نظر را انتخاب نمایید.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.UpdateFileForCodhayeDaramd(ShomareHesabCodeDaramadId, report_file, e, "", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePathFile"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathFile"].ToString());
                    System.IO.File.Delete(physicalPath);
                    Session.Remove("savePathFile");
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
        public ActionResult SaveDesignedReport(string reportUUID, string reportID, int FiscalYearId)
        {
            var s = System.IO.File.ReadAllText(Server.MapPath(@"\App_Data\" + reportUUID));
            s = s.Replace("Border.Color=\"0, 0, 0, 0\"", "Border.Color=\"#000000\"");
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(s);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            FastReport.Report a = new FastReport.Report();
            a.ReportResourceString = returnValue;
            MemoryStream stream = new MemoryStream();
            a.Save(stream);
            var q=reportID.Split(';');
            var ShomareHesabCodeDaramad = servic.GetShomareHesabCodeDaramadWithFilter("fldId", q[0], "",FiscalYearId, 0, IP, out Err).FirstOrDefault();

            string m = servic.UpdateFileForCodhayeDaramd(ShomareHesabCodeDaramad.fldId, stream.ToArray(), ".frx", "", Convert.ToInt32(q[1]), Convert.ToInt32(q[2]), IP, out Err);
           if (Err.ErrorType)
           {
               m = Err.ErrorMsg;
           }
           System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + ShomareHesabCodeDaramad.fldReportFileId + ".frx");
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\" + reportUUID);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportIndex(int Id, int FiscalYearId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldId", Id.ToString(), "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
          // var q = servic.GetPatternFishWithFilter("fldId", Id.ToString(), 0, IP, out Err).FirstOrDefault();
            var q2 = Cservic.GetFileWithFilter("fldId", q.fldReportFileId.ToString(), 0, IP, out CErr).FirstOrDefault();
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + q.fldReportFileId + ".frx";
            System.IO.File.WriteAllBytes(path, q2.fldImage);
            ViewBag.Path = path;
            ViewBag.RId = q.fldId.ToString() + ";" + Session["UserId"].ToString() + ";" + Session["OrganId"].ToString();
            return View();
        }
    }
}
