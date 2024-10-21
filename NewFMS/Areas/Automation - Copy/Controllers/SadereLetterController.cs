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
using NewFMS.Models;
using System.Drawing;
using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Layout;
using Aspose.Words.Fields;
using Aspose.Words.Tables;
using System.IO.Compression;

namespace NewFMS.Areas.Automation.Controllers
{
    public class SadereLetterController : Controller
    {
        //
        // GET: /Automation/SadereLetter/
        WCF_Common.CommonService Comservic = new WCF_Common.CommonService();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        public ActionResult Index(int CommisionId, long LetterId, int SourceAssId, int HistoryLetterID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                //ViewData.Model = new NewFMS.Models.LetterDetail();
                //var result = new Ext.Net.MVC.PartialViewResult
                //{
                //    WrapByScriptTag = true,
                    //ViewData = ViewData
                //};
                //result.ViewBag.CommisionId = CommisionId;
                //return result;
                var CurrentComId = CommisionId;
                if (LetterId != 0)
                {
                    var l=servic.GetLetterWithFilter("fldId", LetterId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    CommisionId=l.fldComisionID;
                }

                PartialView.ViewBag.CurrentComId = CurrentComId;
                PartialView.ViewBag.CommisionId = CommisionId;
                PartialView.ViewBag.LetterId = LetterId.ToString();
                PartialView.ViewBag.SourceAssId = SourceAssId;
                PartialView.ViewBag.HistoryLetterID = HistoryLetterID;
                if (HistoryLetterID != 0)
                {
                    var l = servic.GetLetterWithFilter("fldId", HistoryLetterID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    PartialView.ViewBag.Subject = "پاسخ به: "+l.fldSubject;
                    var ll = servic.GetInternalAssignmentSenderWithFilter("fldAssignmentID", SourceAssId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    var lll = servic.GetCommisionWithFilter("fldId", ll.fldSenderComisionID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    PartialView.ViewBag.reciverName = ll.fldName_Family+"("+lll.fldO_postEjraee_Title + ")،";
                    PartialView.ViewBag.recieverId = ll.fldSenderComisionID+"|1;";
                }
                return PartialView;
            }
        }
        public ActionResult LetterAttach(string containerId, string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            // this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            
            result.ViewBag.LetterId = LetterId;
            return result;
        }
        public ActionResult LetterHistory(string containerId, string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.LetterHistory(); ;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            // this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.LetterId = LetterId;
            return result;
        }
        public ActionResult LetterFollow(string containerId, string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            // this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.LetterId = LetterId;
            return result;
        }
        public ActionResult GetCreator(string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCommisionWithFilter("UserKartabls", Session["UserId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l=>l.fldActive=="1").ToList().Select(n => new { ID = n.fldID, Name = n.fldName+"("+n.fldO_postEjraee_Title+")" });
            if (LetterId != "0" && LetterId != "" && LetterId != null)
            {
                var let = servic.GetLetterDetail(Convert.ToInt64(LetterId), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                q = servic.GetCommisionWithFilter("fldId", let.fldComisionID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldActive == "1").ToList().Select(n => new { ID = n.fldID, Name = n.fldName + "(" + n.fldO_postEjraee_Title + ")" });
            }
            return this.Store(q);
        }
        public ActionResult GetImmediacy()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetImmediacyWithFilter("fldOrganID", Session["OrganId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldID, Name = n.fldName });
            return this.Store(q);
        }
        public ActionResult GetSecurityType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSecurityTypeWithFilter("fldOrganID", Session["OrganId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldID, Name = n.fldSecurityType });
            return this.Store(q);
        }
        public ActionResult GetLetterTemplate()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetLetterTemplateWithFilter("fldOrganId", Session["OrganId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldName });
            return this.Store(q);
        }
        public ActionResult GetHistoryType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities m = new AutomationEntities();
            var q = m.spr_tblHistoryTypeSelect("", "", Convert.ToInt32(Session["OrganId"]), 0).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldName });
            return this.Store(q);
        }
        
        public ActionResult ReadAttach(StoreRequestParameters parameters, string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_LetterAttachment> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_LetterAttachment> data1 = null;
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
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetLetterAttachmentWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l=>l.fldLetterId==Convert.ToInt64(LetterId)).ToList();
                    else
                        data = servic.GetLetterAttachmentWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldLetterId == Convert.ToInt64(LetterId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetLetterAttachmentWithFilter("fldLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_LetterAttachment> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public FileContentResult DownloadAttach(int fldContentFileId)
        {
            if (Session["Username"] == null)
                return null;

            var q = servic.GetContentFileDetail(fldContentFileId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            if (q != null && q.fldLetterText != null)
            {
                MemoryStream st = new MemoryStream(q.fldLetterText);
                return File(st.ToArray(), MimeType.Get(q.fldType), "DownloadFile" + q.fldType);
            }
            return null;
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "";
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {
                if (Session["savePathAttach"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathAttach"].ToString());
                    Session.Remove("savePathAttach");
                    Session.Remove("PasvandAttach");
                    Session.Remove("FileNameAttach");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var passvandd = "%" + extension.Substring(1) + "%";
                var IsValidFormat = servic.GetLetterFileMojazWithFilter("fldPassvand", passvandd, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                if (IsValidFormat != null)
                {
                    var FileSize = IsValidFormat.fldSize * 1048576;
                    if (Request.Files[0].ContentLength <= FileSize)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        string e = Path.GetExtension(savePath);
                        file.SaveAs(savePath);
                        Session["savePathAttach"] = savePath;
                        Session["PasvandAttach"] = file.FileName.Split('.').Last();
                        Session["FileNameAttach"] = file.FileName.Split('.').First();
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName,
                            Message = "فایل با موفقیت آپلود شد.",
                            IsValid = true
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName,
                            Message = "حجم فایل انتخابی می بایست کمتر از " + IsValidFormat.fldSize + " مگا بایت باشد",
                            IsValid = false
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));

                    }
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فرمت فایل انتخاب شده غیر مجاز است.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));

                }
            }
            catch (Exception x)
            {
                if (Session["savePathAttach"] != null)
                {
                    System.IO.File.Delete(Session["savePathAttach"].ToString());
                    Session.Remove("savePathAttach");
                    Session.Remove("PasvandAttach");
                    Session.Remove("FileNameAttach");
                }
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Err = 1
                });
            }
        }
        public ActionResult SaveLetterAttachment(WCF_Automation.OBJ_LetterAttachment Attachment)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                byte[] file = null; string fileName = "", Pasvand = ""; ;
                    //ذخیره
                if (Session["savePathAttach"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathAttach"].ToString()));
                        file = stream.ToArray();
                        Pasvand = Path.GetExtension(Session["savePathAttach"].ToString());
                        fileName = Session["FileNameAttach"].ToString();
                    }
                    else
                    {

                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "لطفا فایل مورد نظر را آپلود نمایید.",
                            Err = 1
                        });
                    }
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertLetterAttachment(Attachment.fldLetterId, Attachment.fldName, fileName, file, Pasvand, Attachment.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
               
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePathAttach"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathAttach"].ToString());
                    Session.Remove("savePathAttach");
                    Session.Remove("FileNameAttach");
                    System.IO.File.Delete(physicalPath);
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
        public ActionResult DeleteLetterAttachment(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                servic.DeleteLetterAttachment(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult GetAssType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAssignmentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(l => new { fldId = l.fldID, fldName = l.fldType });
            return this.Store(q);
        }
       
        
        public ActionResult ReadFollow(StoreRequestParameters parameters, string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_LetterFollow> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_LetterFollow> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldLetterText":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLetterText";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetLetterFollowWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldLetterId == Convert.ToInt64(LetterId)).ToList();
                    else
                        data = servic.GetLetterFollowWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldLetterId == Convert.ToInt64(LetterId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetLetterFollowWithFilter("fldLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_LetterFollow> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveLetterFollow(WCF_Automation.OBJ_LetterFollow Follow)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                
                //ذخیره
                
                MsgTitle = "ذخیره موفق";
                Msg = servic.InsertLetterFollow(Follow.fldLetterId, Follow.fldLetterText, Follow.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteLetterFollow(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                servic.DeleteLetterFollow(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult ReadHistory(StoreRequestParameters parameters, string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_HistoryLetter> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_HistoryLetter> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldSubject":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSubject";
                            break;
                        case "fldLetterNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLetterNumber";
                            break;
                        case "fldLetterDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLetterDate";
                            break;
                        case "fldHistoryTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldHistoryTypeName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetHistoryLetterWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldCreatedLetterId == Convert.ToInt64(LetterId)).ToList();
                    else
                        data = servic.GetHistoryLetterWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldCreatedLetterId == Convert.ToInt64(LetterId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetHistoryLetterWithFilter("fldCurrentLetter_Id", LetterId, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_HistoryLetter> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveHistoryLetter(WCF_Automation.OBJ_HistoryLetter History)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                //ذخیره

                MsgTitle = "ذخیره موفق";
                Msg = servic.InsertHistoryLetter(History.fldCurrentLetter_Id, History.fldHistoryType_Id,History.fldHistoryLetter_Id, History.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteLetterHistory(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                servic.DeleteHistoryLetter(id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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
        public ActionResult Readletter(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_Letter> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_Letter> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldOrderId":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrderId";
                            break;
                        case "fldSubject":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSubject";
                            break;
                        case "fldLetterNumber":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLetterNumber";
                            break;
                        case "fldCreatedDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCreatedDate";
                            break;
                        case "fldLetterDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldLetterDate";
                            break;
                        case "fldSenderName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSenderName";
                            break;
                        case "fldRecieverName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldRecieverName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetLetterWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetLetterWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetLetterWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_Letter> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveLetter(WCF_Automation.OBJ_Letter letter, List<WCF_Automation.OBJ_Ronevesht> runevesht, string Signers, string Recievers)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {
                //if (Permossions.haveAccess(Convert.ToInt32(Session["UserId"]), 84))
                //{
        

                Models.AutomationEntities p = new Models.AutomationEntities();
                if (letter.fldDesc == null)
                    letter.fldDesc = "";
                if (letter.fldKeywords == null)
                    letter.fldKeywords = "";

                letter.fldMatnLetter=System.Uri.UnescapeDataString(letter.fldMatnLetter);
                //letter.fldMatnLetter = letter.fldMatnLetter.Replace("text-align: right", "text-align: justify;direction: rtl");

              
                if (letter.fldFont != null && letter.fldFont != "")
                {
                    var oldlet = servic.GetLetterDetail(letter.fldID, Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (letter.fldMatnLetter.Contains("font-family:"))
                    {
                        letter.fldMatnLetter = letter.fldMatnLetter.Replace(oldlet.fldFont, letter.fldFont);
                    }
                    else
                        letter.fldMatnLetter = "<div style='font-family:" + letter.fldFont + ";'>" + letter.fldMatnLetter + "</div>";
                }

                if (letter.fldID == 0)
                {//ثبت رکورد جدید
                    if (Recievers == "")
                        return Json(new
                        {
                            Msg = "لطفا گیرنده نامه را مشخص کنید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);

                    if (Signers == "")
                        return Json(new
                        {
                            Msg = "لطفا امضا کننده نامه را مشخص کنید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);

                    var g = Recievers.Split(';');
                    var signer = Signers.Split(';');
                    int sal=Convert.ToInt32(Comservic.GetDate(IP, out ComErr).fldTarikh.Substring(0, 4));

                    var k = servic.InsertLetter(sal, letter.fldSubject, null, null, letter.fldKeywords, 1, letter.fldComisionID, letter.fldImmediacyID, letter.fldSecurityTypeID, 1, letter.fldSignType, letter.fldMatnLetter, letter.fldLetterTemplateId, letter.fldFont, letter.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP);
                    if (k.ErrorType == true)
                    {
                        return Json(new
                        {
                            Msg = k.Msg,
                            MsgTitle = k.ErrorMsg,
                            Err = 1,
                        });
                    }
                    long _Letterid = k.OutputId;
                    int _LetterOrderid = k.OutputId2;

           
                    for (int i = 0; i < g.Count() - 1; i++)
                    {
                        var recivers = g[i].Split('|');
                        if (recivers[1] == "2")
                            servic.InsertExternalLetterReceiver(_Letterid, null, Convert.ToInt32(recivers[0]), "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        else
                            servic.InsertInternalLetterReceiver(_Letterid, null, Convert.ToInt32(recivers[0]), null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    var BoxID = servic.GetBoxWithFilter("fldComisionID", letter.fldComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldBoxTypeID == 3).FirstOrDefault();
                    string _IDLetterBox = servic.InsertLetterBox(BoxID.fldID, _Letterid, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    for (int j = 0; j < signer.Length - 1; j++)
                    {
                        servic.InsertSigner(_Letterid, Convert.ToInt32(signer[j]), j + 1, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (runevesht != null)
                    {
                        foreach (var item in runevesht)
                        {
                            if (item.fldText == null)
                                item.fldText = "";
                            if (item.fldAshkhasHoghoghiId == 0)
                                item.fldAshkhasHoghoghiId = null;
                            if (item.fldCommisionId == 0)
                                item.fldCommisionId = null;
                            servic.InsertRonevesht(_Letterid, item.fldAshkhasHoghoghiId, item.fldCommisionId, Convert.ToInt32(item.fldAssignmentTypeId), item.fldText, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                    }

                    //zakhire matn
                    Aspose.Words.Document Doc = LetterContentDoc(_Letterid/*, TempId*/);

                    MemoryStream stream = new MemoryStream();
                    Doc.Save(stream, Aspose.Words.SaveFormat.Docx);
                    byte[] _File = stream.ToArray();
                    servic.InsertContentFile(null, _File, _Letterid, ".docx", "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);


                    var q = servic.GetLetterWithFilter("fldId", _Letterid.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    return Json(new
                    {
                        Msg = "ذخیره با موفقیت انجام شد.",
                        MsgTitle = "عملیات موفق",
                        Err = 0,
                        LetterID = _Letterid,
                        LetterOrderId = _LetterOrderid,
                        LetterCreateDate = q.fldCreatedDate,
                        CreatorId = q.fldComisionID
                    });

                   
                }
                else
                {//ویرایش رکورد ارسالی
                    var IsSign = servic.GetSignerWithFilter("fldLetterID", letter.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(h => h.fldFirstSigner != null).FirstOrDefault();
                    if (IsSign == null)
                    {
                        if (Recievers == "")
                            return Json(new
                            {
                                Msg = "لطفا گیرنده نامه را مشخص کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);

                        if (Signers == "")
                            return Json(new
                            {
                                Msg = "لطفا امضا کننده نامه را مشخص کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);

                        var g = Recievers.Split(';');
                        var signer = Signers.Split(';');


                        servic.UpdateLetter(letter.fldID, letter.fldSubject, letter.fldLetterNumber, letter.fldLetterDate, letter.fldKeywords, letter.fldComisionID, letter.fldImmediacyID, letter.fldSecurityTypeID, 1, letter.fldSignType, letter.fldMatnLetter, letter.fldLetterTemplateId,letter.fldFont, letter.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        servic.DeleteExternalLetterReceiver(letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        servic.DeleteInternalLetterReceiver(letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        for (int i = 0; i < g.Count() - 1; i++)
                        {
                            var recivers = g[i].Split('|');
                            if (recivers[1] == "2")
                                servic.InsertExternalLetterReceiver(Convert.ToInt64(letter.fldID), null, Convert.ToInt32(recivers[0]), "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            else
                                servic.InsertInternalLetterReceiver(Convert.ToInt64(letter.fldID),null, Convert.ToInt32(recivers[0]), null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                        servic.DeleteSigner(letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()),  IP, out Err);
                        for (int j = 0; j < signer.Length - 1; j++)
                        {
                            servic.InsertSigner(Convert.ToInt64(letter.fldID), Convert.ToInt32(signer[j]), j + 1, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }

                        servic.DeleteRonevesht(letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (runevesht != null)
                        {
                                foreach (var item in runevesht)
                                {
                                    if (item.fldText == null)
                                        item.fldText = "";
                                    if (item.fldAshkhasHoghoghiId == 0)
                                        item.fldAshkhasHoghoghiId = null;
                                    if (item.fldCommisionId == 0)
                                        item.fldCommisionId = null;
                                    servic.InsertRonevesht(Convert.ToInt64(letter.fldID), item.fldAshkhasHoghoghiId, item.fldCommisionId, Convert.ToInt32(item.fldAssignmentTypeId), item.fldText, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                        }
                        //zakhire matn
                        Aspose.Words.Document Doc = LetterContentDoc(letter.fldID/*, TempId*/);

                        MemoryStream stream = new MemoryStream();
                        Doc.Save(stream, Aspose.Words.SaveFormat.Docx);
                        byte[] _File = stream.ToArray();
                        if (letter.fldContentFileID == null || letter.fldContentFileID == 0)
                            servic.InsertContentFile(null, _File, letter.fldID, ".docx", "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        else
                            servic.UpdateContentFile(Convert.ToInt32(letter.fldContentFileID), null, _File, letter.fldID, ".docx", "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        var q = servic.GetLetterWithFilter("fldId", letter.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                        return Json(new
                        {
                            Msg = "ویرایش با موفقیت انجام شد.",
                            MsgTitle = "عملیات موفق",
                            Err = 0,
                            LetterID = letter.fldID,
                            LetterOrderId = q.fldOrderId,
                            LetterCreateDate = q.fldCreatedDate,
                            CreatorId = q.fldComisionID
                        });

                        
                    }
                    else
                    return Json(new
                    {
                        Msg = "نامه امضا شده و قابل تغییر نمی باشد.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                //}
                //else
                //{
                //    Session["ER"] = "شما مجاز به دسترسی نمی باشید.";
                //    return RedirectToAction("error", "Metro");
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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsLetter(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetLetterWithFilter("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldID = q.fldID,
                fldComisionID = q.fldComisionID.ToString(),
                fldCreatedDate=q.fldCreatedDate.Substring(0,10),
                fldCreatedDateEn=q.fldCreatedDateEn,
                fldImmediacyID=q.fldImmediacyID.ToString(),
                fldKeywords=q.fldKeywords,
                fldLetterDate=q.fldLetterDate,
                fldLetterNumber=q.fldLetterNumber,
                fldLetterStatusID=q.fldLetterStatusID,
                fldLetterTypeID=q.fldLetterTypeID,
                fldOrderId=q.fldOrderId,
                fldReceiver=q.fldReceiver,
                fldRecieverName= q.fldRecieverName,
                fldRoonevesht=q.fldRoonevesht,
                fldSecurityTypeID=q.fldSecurityTypeID.ToString(),
                fldSenderName=q.fldSenderName,
                fldSigner=q.fldSigner,
                fldSubject=q.fldSubject,
                fldSignType=q.fldSignType.ToString(),
                fldYear=q.fldYear,
                fldSignerName=q.fldSignerName,
                fldDesc = q.fldDesc,
                fldLetterTemplateId = q.fldLetterTemplateId.ToString(),
                fldContentFileID=q.fldContentFileID,
                fldMatnLetter=q.fldMatnLetter,
                fldFont = q.fldFont
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadRunevesht(string LetterId)
        
    {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Automation.OBJ_Ronevesht> data = null;
            data = servic.GetRoneveshtWithFilter("fldLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SignLetter(int LetterID, string SignerComisions, int CurrentComId)
        {//ذخیره آخرین پیگیری نامه
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = -1; 
            try
            {
                    Models.AutomationEntities p = new Models.AutomationEntities();
                    
                    var signer = SignerComisions.Split(';');
                  
                    for (int i = 0; i < signer.Count() - 1; i++)
                    {
                        //if (Signer.CreatorComId == Convert.ToInt32(signer[i]))
                        //{
                        var IsSign = servic.GetSignerWithFilter("fldLetterId", LetterID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(h => h.fldSignerComisionId == Convert.ToInt32(signer[i])).FirstOrDefault();
                        if (IsSign != null)
                        {
                            if (IsSign.fldFirstSigner == null)
                            {
                                if (signer[i] == CurrentComId.ToString())
                                {
                                    p.spr_SignLetter(IsSign.fldId, Convert.ToInt32(signer[i]), Convert.ToInt32(Session["UserId"]));
                                    servic.UpdateLetterStatusId(LetterID, 2, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                                    if (Er == -1)
                                    {
                                        MsgTitle = "عملیات موفق";
                                        Msg = "نامه با موفقیت امضا گردید.";
                                        Er = 0;
                                    }
                                }
                                else
                                {
                                    var Substitute = servic.GetSubstituteWithFilter("fldReceiverComisionID", CurrentComId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                    if (Substitute != null)
                                    {
                                        if (Substitute.fldIsSigner)
                                        {
                                            p.spr_SignLetter(IsSign.fldId, CurrentComId, Convert.ToInt32(Session["UserId"]));
                                            servic.UpdateLetterStatusId(LetterID, 2, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                                            if (Er == -1)
                                            {
                                                MsgTitle = "عملیات موفق";
                                                Msg = "نامه با موفقیت امضا گردید.";
                                                Er = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Er == -1)
                                        {
                                            MsgTitle = "خطا";
                                            Msg = "شما مجوز امضا نامه را ندارید.";
                                            Er = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Er == -1)
                                {
                                    MsgTitle = "خطا";
                                    Msg = "نامه قبلا امضا شده است.";
                                    Er = 1;
                                }
                            }
                        }
                        else
                        {
                            if (Er == -1)
                            {
                                MsgTitle = "خطا";
                                Msg = "شما مجوز امضا نامه را ندارید.";
                                Er = 1;
                            }
                        }
                        //}
                        //else
                        //{
                        //    if (state == -1)
                        //    {
                        //        data = "شما مجوز امضا نامه را ندارید.";
                        //        state = 1;
                        //    }
                        //}
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
        public ActionResult CheckInternalP(string fldReceiverComisionID, string fldRoneveshId)
        {
            Models.AutomationEntities p = new Models.AutomationEntities();
            if (fldRoneveshId == null)
                fldRoneveshId = "";
            var HaveIn = 0; string Msg = "";
            var Recivers = fldReceiverComisionID.Split(';');
            var Ronevesht = fldRoneveshId.Split(';');

            for (int i = 0; i < Recivers.Count() - 1; i++)
            {
                var R = Recivers[i].Split('|');
                if (R[1] == "2")
                {
                    Msg = "گیرنده نامه خارجی است لذا امکان توزیع نامه وجود ندارد.";
                }
                if (R[1] != "2")
                    HaveIn = 1;
            }
            for (int i = 0; i < Ronevesht.Count() - 1; i++)
            {
                var Ro = Ronevesht[i].Split('|');
                if (Ro[1] != "2")
                    HaveIn = 1;
            }

            return Json(new { In = HaveIn ,Msg=Msg}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Distribute(int LetterID, string ReceiverComisionID, List<WCF_Automation.OBJ_Ronevesht> runevesht, string AnswerDate, int SourceAssId, int ComisionID)
        {//توزیع نامه
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {
                    Models.AutomationEntities p = new Models.AutomationEntities();
                    var date = Comservic.GetDate(IP,out ComErr);

                   
                    if (AnswerDate == "")
                        AnswerDate = date.fldTarikh;

                    var Recivers = ReceiverComisionID.Split(';');
               



                    var status = servic.GetLetterWithFilter("fldId", LetterID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                        if (status.fldLetterStatusID != 4)//توزیع شده4
                        {

                            var IsLetterID = servic.GetAssignmentWithFilter("fldLetterID", LetterID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                            int idAssignment = 0;
                            //sabte erja
                            if (IsLetterID != null)
                            {// در صورتیکه این نامه قبلا ارجاع داده شده است
                                var ParentAssignmentID = SourceAssId;
                                idAssignment = servic.InsertAssignment(LetterID, null, AnswerDate, ParentAssignmentID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                                var BoxSendID = servic.GetBoxWithFilter("fldComisionID", ComisionID.ToString(), "",Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                                servic.InsertInternalAssignmentSender(idAssignment, ComisionID, BoxSendID.fldID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            else
                            {// در صورتیکه برای اولین بار نامه ارجاع داده می شود
                                idAssignment = servic.InsertAssignment(LetterID, null, AnswerDate, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                                //دریافت کد باکس جهت ذخیره در کارتابل ارسال شده
                                var BoxSendID = servic.GetBoxWithFilter("fldComisionID", ComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                                //ذخیره نامه در پوشه ارسال شده
                                var LetterBox = servic.GetLetterBoxWithFilter("fldLetterID", LetterID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(LetterBox.fldId, BoxSendID.fldID, LetterID, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                servic.InsertInternalAssignmentSender(idAssignment, ComisionID, BoxSendID.fldID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            servic.UpdateLetterStatusId(LetterID, 4, Session["Username"].ToString(), (Session["Password"].ToString()),IP,out Err);
                                  
                            for (int i = 0; i < Recivers.Count() - 1; i++)
                            {
                                var R = Recivers[i].Split('|');
                                if (R[1] != "2")
                                {
                                    var BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", R[0].ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                                    servic.InsertInternalAssignmentReceiver(idAssignment, Convert.ToInt32(R[0]), 1, 2, BoxCurrentID.fldID, null, true, "اصل", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    var subStatiut = servic.GetSubstituteWithFilter("fldSenderComisionID", R[0], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                                    foreach (var item in subStatiut)
                                    {
                                        BoxCurrentID = servic.GetBoxWithFilter("fldComisionID" ,item.fldReceiverComisionID.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                                        servic.InsertInternalAssignmentReceiver(idAssignment, item.fldReceiverComisionID, 1, 1, BoxCurrentID.fldID, null, false, "اصل-تفویض شده", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    }
                                }
                            }
                            foreach (var Ruitem in runevesht)
                            {
                                if (Ruitem.fldText == null)
                                    Ruitem.fldText = "";
                                if (Ruitem.fldCommisionId != 0 && Ruitem.fldCommisionId !=null)
                                {
                                    var BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", Ruitem.fldCommisionId.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                                    servic.InsertInternalAssignmentReceiver(idAssignment, Convert.ToInt32(Ruitem.fldCommisionId), 1, Convert.ToInt32(Ruitem.fldAssignmentTypeId), BoxCurrentID.fldID, null, true, "رونوشت", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    var subStatiut = servic.GetSubstituteWithFilter("fldSenderComisionID", Ruitem.fldCommisionId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                                    foreach (var item in subStatiut)
                                    {
                                        BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", item.fldReceiverComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                                        servic.InsertInternalAssignmentReceiver(idAssignment, item.fldReceiverComisionID, 1, 1, BoxCurrentID.fldID, null, false, "رونوشت-تفویض شده", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    }
                                }
                            }
                            return Json(new
                            {
                                Msg = "توزیع نامه با موفقیت انجام شد.",
                                MsgTitle = "عملیات موفق",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new
                        {
                            Msg = "نامه قبلا توزیع شده است.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    
                  

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
        public ActionResult DistributeWin(int LetterID, string ReceiverComisionID,  int SourceAssId, int ComisionID)
        {//باز شدن پنجره 
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.LetterID = LetterID;
            result.ViewBag.ReceiverComisionID = ReceiverComisionID;
            result.ViewBag.SourceAssId = SourceAssId;
            result.ViewBag.ComisionID = ComisionID;
            result.ViewBag.Tarikh = MyLib.Shamsi.ShamsiAddDay(Comservic.GetDate(IP, out ComErr).fldTarikh, 2);
            return result;
        }
        public Aspose.Words.Document LetterContentDoc(long LetterId/*, int TempId*/)
        {
            int TempId = 0;

            var let = servic.GetLetterDetail(LetterId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            TempId = Convert.ToInt32(let.fldLetterTemplateId);
            var tem = servic.GetLetterTemplateDetail(TempId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var fil = Comservic.GetFileWithFilter("fldId", tem.fldLetterFileId.ToString(), 0, IP, out ComErr).FirstOrDefault();


            //MemoryStream st = new MemoryStream(fil.fldImage);
            //var FilePath = Server.MapPath(@"~\Uploaded\TaiidMadrakNew.docx");
            //System.IO.File.WriteAllBytes(FilePath, st.ToArray());

            //MemoryStream Madrakstream = new MemoryStream(System.IO.File.ReadAllBytes(FilePath));

            MemoryStream Madrakstream = new MemoryStream(fil.fldImage);

            Aspose.Words.Document Doc = new Aspose.Words.Document(Madrakstream);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(Doc);

            Models.AutomationEntities p = new Models.AutomationEntities();
            var mergs = p.spr_tblMergeField_LetterTemplate_LetterIdSelect(TempId.ToString(), LetterId.ToString(), Convert.ToInt32(Session["OrganId"])).ToList();

            foreach (var item in mergs)
            {
                builder.MoveToBookmark(item.fldEnName);

                var valEnName = item.fldValue.Replace(";", "<br/>");
                if (let.fldFont != null && let.fldFont != "")
                    valEnName = "<span style=\"font-family:" + let.fldFont+";\">" + valEnName + "</span>";

                builder.InsertHtml(valEnName);
            }
            builder.MoveToBookmark("Matn");
            var CreateMatnHtml = let.fldMatnLetter;
            if (let.fldMatnLetter.Substring(0, 24) == "<div style=\"font-family:")
            {
                CreateMatnHtml = let.fldMatnLetter.Replace("<div style=\"font-family", "<span style=\"font-family");
                CreateMatnHtml = CreateMatnHtml.Remove(CreateMatnHtml.Length - 6) + "</span>";
            }
            builder.InsertHtml(CreateMatnHtml);
            //builder.MoveToBookmark("shomare");
            //builder.InsertHtml(let.fldLetterNumber);
            if (let.fldLetterStatusID == 2)//امضا شده
            {
                var signers = servic.GetSignerWithFilter("fldLetterId", LetterId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldFirstSigner != null).ToList();
                foreach (var item in signers)
                {
                    if (item.fldFileSignId != null)
                    {
                        var filSign = Comservic.GetFileWithFilter("fldId", item.fldFileSignId.ToString(), 0, IP, out ComErr).FirstOrDefault();
                        builder.MoveToBookmark("Emza");
                        builder.InsertImage(filSign.fldImage, 90, 70);
                    }
                }
            }

            //if (let.fldFont != null && let.fldFont != "")
            //{
            //    Aspose.Words.NodeCollection runs = Doc.GetChildNodes(NodeType.Run, true);
            //    foreach (Run run in runs)
            //    {
            //        run.Font.Name = let.fldFont;// "B Nazanin";
            //        // run.Font.SizeBi = 13;
            //    }
            //}

            return Doc;
        }
        public FileContentResult CreateLetterText(int LetterId/*, int TempId*/)
        {
            if (Session["Username"] == null)
                return null;

            var let = servic.GetLetterDetail(LetterId, Convert.ToInt32(Session["OrganId"]), IP, out Err);

            Aspose.Words.Document Doc = LetterContentDoc(LetterId/*, TempId*/);
            
            MemoryStream stream = new MemoryStream();
            Doc.Save(stream, Aspose.Words.SaveFormat.Docx);
            byte[] _File = stream.ToArray();

            return File(stream.ToArray(), MimeType.Get(".docx"), let.fldSubject+".docx");


        }
        public ActionResult PreviewLetter(int LetterId/*, int TempId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.LetterId = LetterId;
           /* result.ViewBag.TempId = TempId;*/
            return result;
        }
        public ActionResult GeneratePDFPreviewLetter(int LetterId/*, int TempId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var let = servic.GetLetterDetail(LetterId, Convert.ToInt32(Session["OrganId"]), IP, out Err);

                Aspose.Words.Document Doc = LetterContentDoc(LetterId/*, TempId*/);

                MemoryStream stream = new MemoryStream();
                Doc.Save(stream, Aspose.Words.SaveFormat.Pdf);

                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
