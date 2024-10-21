using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using System.Xml;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class SaveReportHokmController : Controller
    {
        //
        // GET: /Personeli/SaveReportHokm/
        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common_Pay.Comon_PayService Pservic = new WCF_Common_Pay.Comon_PayService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        WCF_Common_Pay.ClsError PErr = new WCF_Common_Pay.ClsError();
        public ActionResult IndexNew(string containerId)
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
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            HttpPostedFileBase file = Request.Files[0];
            string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
            file.SaveAs(savePath);
            Session["savePath"] = savePath;
            object r = new
            {
                success = true,
                name = Request.Files[0].FileName
            };
            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
        }
        public FileContentResult Download(int FileId)
        {
            var q = Cservic.GetFileWithFilter("fldId", FileId.ToString(), 1,IP, out CErr).FirstOrDefault();

            if (q != null)
            {
                MemoryStream st = new MemoryStream(q.fldImage);
                return File(st.ToArray(), MimeType.Get(q.fldPasvand), "DownloadFile" + q.fldPasvand);
            }
            return null;
        }
        public ActionResult GetEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Pservic.GetAnvaEstekhdamWithFilter("", "", 0, IP,out PErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Personeli.OBJ_HokmReport HokmReport)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            byte[] report_file = null; var e = "";
            try
            {
                var q = Cservic.GetDate(IP, out CErr);
                if (Session["savePath"] != null)
                {
                    var path = Session["savePath"].ToString();
                    e = Path.GetExtension(Session["savePath"].ToString());
                    var Name = Path.GetFileNameWithoutExtension(path);
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));

                    report_file = stream.ToArray();
                }
                else if (HokmReport.fldId == 0)
                {
                    return Json(new
                    {
                        Msg = "لطفا فایل گزارش را وارد کنید.",
                        MsgTitle = "خطا",
                        Er=1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (HokmReport.fldDesc == null)
                    HokmReport.fldDesc = "";

                if (HokmReport.fldId == 0)
                { //ذخیره
                    servic.InsertHokmReport(HokmReport.fldName, report_file, e, HokmReport.fldAnvaEstekhdamId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";

                }
                else
                { //ویرایش
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";

                    if (Session["savePath"] == null)
                    {
                        servic.UpdateHokmReport(HokmReport.fldId, HokmReport.fldName, HokmReport.fldFileId, null, e, HokmReport.fldAnvaEstekhdamId, "", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        servic.UpdateHokmReport(HokmReport.fldId, HokmReport.fldName, HokmReport.fldFileId, report_file, e, HokmReport.fldAnvaEstekhdamId, "", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePath"] != null)
                {
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
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
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteHokmReport(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetHokmReportDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldAnvaEstekhdamId = q.fldAnvaEstekhdamId.ToString(),
                fldName = q.fldName,
                fldDesc = q.fldDesc,
                fldFileId = q.fldFileId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_HokmReport> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_HokmReport> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldAnvaEstekhdamName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAnvaEstekhdamName";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetHokmReportWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetHokmReportWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetHokmReportWithFilter("", "", 100, IP, out Err).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Personeli.OBJ_HokmReport> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public static string XmlUnescape(string escaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }
        public ActionResult SaveDesignedReport(string reportUUID, string reportID)
        {
            //if (Session["Username"] == null)
            //    return RedirectToAction("logon", "Account", new { area = "" });
            var s = System.IO.File.ReadAllText(Server.MapPath(@"\App_Data\" + reportUUID));
            s = s.Replace("Border.Color=\"0, 0, 0, 0\"", "Border.Color=\"#000000\"");
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(s);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            FastReport.Report a = new FastReport.Report();
            a.ReportResourceString = returnValue;
            MemoryStream stream = new MemoryStream();
            a.Save(stream);
            var q = reportID.Split(';');
            var HokmReport = servic.GetHokmReportWithFilter("fldId", q[0], 0, IP, out Err).FirstOrDefault();

            servic.UpdateHokmReport(HokmReport.fldId, HokmReport.fldName, HokmReport.fldFileId, stream.ToArray(), ".frx", HokmReport.fldAnvaEstekhdamId, "", Convert.ToInt32(q[1]), Convert.ToInt32(q[2]), IP, out Err);
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + HokmReport.fldName + ".frx");
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\" + reportUUID);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportIndex(int Id)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var result = new Ext.Net.MVC.PartialViewResult
            //{
            //    WrapByScriptTag = true,
            //    ContainerId = containerId,
            //    RenderMode = RenderMode.AddTo
            //};

            //this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            var q = servic.GetHokmReportWithFilter("fldId", Id.ToString(), 0, IP,out Err).FirstOrDefault();
            var q2 = Cservic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 0,IP, out CErr).FirstOrDefault();
            //string path = Server.MapPath(@"\Reports\" + q.fldName + ".frx");
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + q.fldName + ".frx";
            System.IO.File.WriteAllBytes(path, q2.fldImage);
            ViewBag.Path = path;
            ViewBag.RId = q.fldId.ToString() + ";" + Session["UserId"].ToString() + ";" +(Session["OrganId"]).ToString();
            return View();
        }
    }
}
