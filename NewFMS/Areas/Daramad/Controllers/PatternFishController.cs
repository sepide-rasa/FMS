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

namespace NewFMS.Areas.Daramad.Controllers
{
    public class PatternFishController : Controller
    {
        //
        // GET: /Daramad/PatternFish/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();

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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
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
            var q = Cservic.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out CErr).FirstOrDefault();

            if (q != null)
            {
                MemoryStream st = new MemoryStream(q.fldImage);
                return File(st.ToArray(), MimeType.Get(q.fldPasvand), "DownloadFile" + q.fldPasvand);
            }
            return null;
        }
        public ActionResult Save(WCF_Daramad.OBJ_PatternFish PatternFish)
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
                else if (PatternFish.fldId == 0)
                {
                    return Json(new
                    {
                        Msg = "لطفا فایل گزارش را وارد کنید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (PatternFish.fldDesc == null)
                    PatternFish.fldDesc = "";

                if (PatternFish.fldId == 0)
                { //ذخیره
                    servic.InsertPatternFish(PatternFish.fldName, report_file, e,"", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";

                }
                else
                { //ویرایش
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";

                    if (Session["savePath"] == null)
                    {
                        servic.UpdatePatternFish(PatternFish.fldId, PatternFish.fldName, PatternFish.fldFileId, null, e,  "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        servic.UpdatePatternFish(PatternFish.fldId, PatternFish.fldName, PatternFish.fldFileId, report_file, e,  "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er = Er
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
                servic.DeletePatternFish(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPatternFishDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
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

            List<WCF_Daramad.OBJ_PatternFish> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PatternFish> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetPatternFishWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetPatternFishWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPatternFishWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_PatternFish> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
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
            var PatternFish = servic.GetPatternFishWithFilter("fldId", q[0], 0, IP, out Err).FirstOrDefault();

            servic.UpdatePatternFish(PatternFish.fldId, PatternFish.fldName, PatternFish.fldFileId, stream.ToArray(), ".frx",  "", q[1], q[2], Convert.ToInt32(q[3]), IP, out Err);
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + PatternFish.fldName + ".frx");
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
            var q = servic.GetPatternFishWithFilter("fldId", Id.ToString(), 0, IP, out Err).FirstOrDefault();
            var q2 = Cservic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 0, IP, out CErr).FirstOrDefault();
            //string path = Server.MapPath(@"\Reports\" + q.fldName + ".frx");
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + q.fldName + ".frx";
            System.IO.File.WriteAllBytes(path, q2.fldImage);
            ViewBag.Path = path;
            ViewBag.RId = q.fldId.ToString() + ";" + Session["Username"].ToString() + ";" + Session["Password"].ToString() + ";" + Session["OrganId"].ToString();
            return View();
        }

    }
}
