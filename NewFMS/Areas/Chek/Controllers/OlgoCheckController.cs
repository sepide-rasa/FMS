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

namespace NewFMS.Areas.Chek.Controllers
{
    public class OlgoCheckController : Controller
    {
        //
        // GET: /Chek/OlgoCheck/
        WCF_Chek.ChekService servic = new WCF_Chek.ChekService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Chek.ClsError Err = new WCF_Chek.ClsError();
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
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetBankWithFilter("", "", 0, IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldBankName });
            return this.Store(q);
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
            if ((file.ContentType) == "application/octet-stream")
            {
                string savePathOlguChek = Server.MapPath(@"~\Uploaded\" + file.FileName);
                file.SaveAs(savePathOlguChek);
                Session["savePathOlguChek"] = savePathOlguChek;
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
                    Message = "فایل غیرمجاز است. (پسوند قابل قبول frx)"
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
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
        public ActionResult Save(WCF_Chek.OBJ_OlgoCheck OlgoCheck)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            byte[] report_file = null; var e = "";
            try
            {
                var q = Cservic.GetDate(IP, out CErr);
                if (Session["savePathOlguChek"] != null)
                {
                    var path = Session["savePathOlguChek"].ToString();
                    e = Path.GetExtension(Session["savePathOlguChek"].ToString());
                    var Name = Path.GetFileNameWithoutExtension(path);
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathOlguChek"].ToString()));

                    report_file = stream.ToArray();
                }
                else if (OlgoCheck.fldId == 0)
                {
                    return Json(new
                    {
                        Msg = "لطفا فایل گزارش را وارد کنید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (OlgoCheck.fldDesc == null)
                    OlgoCheck.fldDesc = "";

                if (OlgoCheck.fldId == 0)
                { //ذخیره
                    servic.InsertOlgoCheck(OlgoCheck.fldTitle, report_file, e, OlgoCheck.fldIdBank,"", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";

                }
                else
                { //ویرایش
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";

                    if (Session["savePathOlguChek"] == null)
                    {
                        servic.UpdateOlgoCheck(OlgoCheck.fldId, OlgoCheck.fldTitle, null, e,OlgoCheck.fldIdFile,  OlgoCheck.fldIdBank, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        servic.UpdateOlgoCheck(OlgoCheck.fldId, OlgoCheck.fldTitle, report_file, e, OlgoCheck.fldIdFile, OlgoCheck.fldIdBank, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePathOlguChek"] != null)
                {
                    System.IO.File.Delete(Session["savePathOlguChek"].ToString());
                    Session.Remove("savePathOlguChek");
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
                servic.DeleteOlgoCheck(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetOlgoCheckDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldDesc = q.fldDesc,
                fldFileId = q.fldIdFile,
                fldIdBank = q.fldIdBank.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Chek.OBJ_OlgoCheck> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Chek.OBJ_OlgoCheck> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetOlgoCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetOlgoCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetOlgoCheckWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Chek.OBJ_OlgoCheck> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
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
            var OlgoCheck = servic.GetOlgoCheckWithFilter("fldId", q[0], Convert.ToInt32(q[3]), 0, IP, out Err).FirstOrDefault();

            servic.UpdateOlgoCheck(OlgoCheck.fldId, OlgoCheck.fldTitle,  stream.ToArray(), ".frx",OlgoCheck.fldIdFile,OlgoCheck.fldIdBank, "", q[1], q[2], Convert.ToInt32(q[3]), IP, out Err);
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + OlgoCheck.fldTitle + ".frx");
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
            var q = servic.GetOlgoCheckWithFilter("fldId", Id.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            var q2 = Cservic.GetFileWithFilter("fldId", q.fldIdFile.ToString(), 0, IP, out CErr).FirstOrDefault();
            //string path = Server.MapPath(@"\Reports\" + q.fldName + ".frx");
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Uploaded\" + q.fldTitle + ".frx";
            System.IO.File.WriteAllBytes(path, q2.fldImage);
            ViewBag.Path = path;
            ViewBag.RId = q.fldId.ToString() + ";" + Session["Username"].ToString() + ";" + Session["Password"].ToString() + ";" + Session["OrganId"].ToString();
            return View();
        }

    }
}
