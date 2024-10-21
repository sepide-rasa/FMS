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
using Aspose.Words;

namespace NewFMS.Areas.Automation.Controllers
{
    public class VaredeLetterController : Controller
    {
        //
        // GET: /Automation/VaredeLetter/ 
        WCF_Common.CommonService Comservic = new WCF_Common.CommonService();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
        public ActionResult Index(int CommisionId, long LetterId, int SourceAssId)
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
                    var l = servic.GetLetterWithFilter("fldId", LetterId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    CommisionId = l.fldComisionID;
                }

                PartialView.ViewBag.CurrentComId = CurrentComId;
                PartialView.ViewBag.CommisionId = CommisionId;
                PartialView.ViewBag.LetterId = LetterId.ToString();
                PartialView.ViewBag.SourceAssId = SourceAssId;

               
                return PartialView;
            }
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetCreator(string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCommisionWithFilter("UserKartabls", Session["UserId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldActive == "1").ToList().Select(n => new { ID = n.fldID, Name = n.fldName + "(" + n.fldO_postEjraee_Title + ")" });
            if (LetterId != "0" && LetterId != "" && LetterId != null)
            {
                var let = servic.GetLetterDetail(Convert.ToInt64(LetterId), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                q = servic.GetCommisionWithFilter("fldId", let.fldComisionID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldID, Name = n.fldName + "(" + n.fldO_postEjraee_Title + ")" });
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
                        data1 = servic.GetLetterAttachmentWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldLetterId == Convert.ToInt64(LetterId)).ToList();
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
        public ActionResult CheckFormatSize(string Passvand)
        {
            var FileSize = 0;
            var Valid = false;
            var IsValidFormat = servic.GetLetterFileMojazWithFilter("fldPassvand", "%" + Passvand + "%", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldType == 1).FirstOrDefault();
            if (IsValidFormat != null)
            {
                Valid = true;
                FileSize = IsValidFormat.fldSize * 1048576;
            }
            return Json(new
            {
                FileSize = FileSize,
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
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
                            Message = "حجم فایل انتخابی می بایست کمتر از " + IsValidFormat.fldSize+" مگا بایت باشد",
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
        public ActionResult SaveFiles(string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; var Er = 0;
            byte[] fileLetter = null; string fileName = "", Pasvand = "";
            var LetterContentFileId = 0;
            try
            {
                if (LetterId == "0")
                {
                    return Json(new
                    {
                        Msg = "لطفا ابتدا نامه زا ذخیره نمایید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var IsDistribute = CheckIsDistribute(Convert.ToInt32(LetterId));
                    if (IsDistribute == 0)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFileLetter"].ToString()));
                        fileLetter = stream.ToArray();
                        Pasvand = Path.GetExtension(Session["savePathFileLetter"].ToString());
                        fileName = Session["FileNameFileLetter"].ToString();

                        var HaveFile = servic.GetContentFileWithFilter("MatnLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                        if (HaveFile == null)
                        {
                            servic.InsertContentFile(fileName, fileLetter, Convert.ToInt64(LetterId), Pasvand, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            MsgTitle = "ذخیره موفق";
                            Msg = "ذخیره با موفقیت انجام شد.";
                            if (Err.ErrorType)
                            {
                                Msg = Err.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;

                            }

                            var H = servic.GetContentFileWithFilter("MatnLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                           if (H != null)
                                LetterContentFileId = H.fldId;
                            
                        }
                        else
                        {
                            servic.UpdateContentFile(HaveFile.fldId, fileName, fileLetter, Convert.ToInt64(LetterId), Pasvand, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            MsgTitle = "ذخیره موفق";
                            Msg = "ذخیره با موفقیت انجام شد.";

                            LetterContentFileId = HaveFile.fldId;

                            if (Err.ErrorType)
                            {
                                Msg = Err.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;
                            }
                        }
                       
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "نامه مورد نظر توزیع شده است لذا امکان تغییر وجود ندارد..",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);

                    }
                }
                


                if (Session["savePathFileLetter"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathFileLetter"].ToString());
                    Session.Remove("savePathFileLetter");
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
                Er = Er,
                LetterContentFileId = LetterContentFileId
            }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult UploadFileLetter()
        {
            Models.AutomationEntities m=new AutomationEntities();
            try
            {

                if (Session["savePathFileLetter"] != null)
                {
                    System.IO.File.Delete(Session["savePathFileLetter"].ToString());
                    Session.Remove("savePathFileLetter");
                    Session.Remove("FileNameFileLetter");
                    Session.Remove("PasvandFileLetter");
                }


                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var IsPDF = FileInfoExtensions.IsPDF(Request.Files[0]);
                var IsImage = FileInfoExtensions.IsImage(Request.Files[0]);
                if (IsImage == true || IsPDF == true)
                {
                    if (Request.Files[0].ContentLength <= 10485760)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["savePathFileLetter"] = savePath;
                        Session["PasvandFileLetter"] = file.FileName.Split('.').Last();
                        Session["FileNameFileLetter"] = Path.GetFileNameWithoutExtension(Request.Files[0].FileName);

                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName,
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 10 مگابایت باشد.",
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
                if (Session["savePathFileLetter"] != null)
                {
                    System.IO.File.Delete(Session["savePathFileLetter"].ToString());
                    Session.Remove("savePathFileLetter");
                    Session.Remove("PasvandFileLetter");
                    Session.Remove("FileNameFileLetter");
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
        public ActionResult ReadRunevesht(string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Automation.OBJ_Ronevesht> data = null;
            data = servic.GetRoneveshtWithFilter("fldLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SaveLetter(WCF_Automation.OBJ_Letter letter, List<WCF_Automation.OBJ_Ronevesht> runevesht, string Senders, string Recievers)
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
                if (letter.fldID == 0)
                {//ثبت رکورد جدید


                    if (Recievers == "")
                        return Json(new
                        {
                            Msg = "لطفا گیرنده نامه را مشخص کنید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);

                    if (Senders == "")
                        return Json(new
                        {
                            Msg = "لطفا فرستنده نامه را مشخص کنید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);

                    var g = Recievers.Split(';');
                    var sender = Senders.Split(';');
                    int sal = Convert.ToInt32(Comservic.GetDate(IP, out ComErr).fldTarikh.Substring(0, 4));
              
                    var k = servic.InsertLetter(sal, letter.fldSubject, letter.fldLetterNumber, letter.fldLetterDate, letter.fldKeywords, 1, letter.fldComisionID, letter.fldImmediacyID, letter.fldSecurityTypeID, 2, letter.fldSignType,"",null,"", letter.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP);
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
                    for (int j = 0; j < sender.Length - 1; j++)
                    {
                        var senders = sender[j].Split('|');
                        if (senders[1] == "2")
                        servic.InsertExternalLetterSender(_Letterid, null, Convert.ToInt32(senders[0]), "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (runevesht != null)
                    {
                        foreach (var item in runevesht)
                        {
                            if (item.fldText == null)
                                item.fldText = "";
                            if (item.fldAshkhasHoghoghiTitlesId == 0)
                                item.fldAshkhasHoghoghiTitlesId = null;
                            if (item.fldCommisionId == 0)
                                item.fldCommisionId = null;
                            servic.InsertRonevesht(_Letterid, item.fldAshkhasHoghoghiTitlesId, item.fldCommisionId, Convert.ToInt32(item.fldAssignmentTypeId), item.fldText, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                    }
                    var q = servic.GetLetterWithFilter("fldId", _Letterid.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                   // ViewBag.LetterId_Upload = _Letterid;
                    return Json(new
                    {
                        Msg = "ذخیره با موفقیت انجام شد.",
                        MsgTitle = "عملیات موفق",
                        Err = 0,
                        LetterID = _Letterid,
                        
                        LetterOrderId = _LetterOrderid,
                        LetterCreateDate = q.fldCreatedDate.Substring(0, 10),
                        CreatorId = q.fldComisionID,
                        LetterDate = q.fldLetterDate,
                        LetterNumber = q.fldLetterNumber
                    });
                }
                else
                {//ویرایش رکورد ارسالی
                    var IsTozi = servic.GetLetterWithFilter("fldID", letter.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    if (IsTozi.fldLetterStatusID != 4)
                    {
                        if (Recievers == "")
                            return Json(new
                            {
                                Msg = "لطفا گیرنده نامه را مشخص کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);

                        if (Senders == "")
                            return Json(new
                            {
                                Msg = "لطفا فرستنده نامه را مشخص کنید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);

                        var g = Recievers.Split(';');
                        var sender = Senders.Split(';');



                        servic.UpdateLetter(letter.fldID, letter.fldSubject, letter.fldLetterNumber, letter.fldLetterDate, letter.fldKeywords, letter.fldComisionID, letter.fldImmediacyID, letter.fldSecurityTypeID, 2, letter.fldSignType, "", null,"", letter.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        servic.DeleteExternalLetterReceiver(letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        for (int i = 0; i < g.Count() - 1; i++)
                        {
                            var recivers = g[i].Split('|');
                            if (recivers[1] == "2")
                                servic.InsertExternalLetterReceiver(Convert.ToInt64(letter.fldID), null, Convert.ToInt32(recivers[0]), "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            else
                                servic.InsertInternalLetterReceiver(Convert.ToInt64(letter.fldID), null, Convert.ToInt32(recivers[0]), null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                        servic.DeleteExternalLetterSender(letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        for (int j = 0; j < sender.Length - 1; j++)
                        {
                            var senders = sender[j].Split('|');
                            if (senders[1] == "2")
                            servic.InsertExternalLetterSender(letter.fldID, null, Convert.ToInt32(senders[0]), "",Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                        servic.DeleteRonevesht("fldLetterId", letter.fldID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (runevesht != null)
                        {
                            foreach (var item in runevesht)
                            {
                                if (item.fldText == null)
                                    item.fldText = "";
                                if (item.fldAshkhasHoghoghiTitlesId == 0)
                                    item.fldAshkhasHoghoghiTitlesId = null;
                                if (item.fldCommisionId == 0)
                                    item.fldCommisionId = null;
                                servic.InsertRonevesht(Convert.ToInt64(letter.fldID), item.fldAshkhasHoghoghiTitlesId, item.fldCommisionId, Convert.ToInt32(item.fldAssignmentTypeId), item.fldText, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                        }
                        var q = servic.GetLetterWithFilter("fldId", letter.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                      //  ViewBag["LetterId_Upload"] = letter.fldID;
                        return Json(new
                        {
                            Msg = "ویرایش با موفقیت انجام شد.",
                            MsgTitle = "عملیات موفق",
                            Err = 0,
                            LetterID = letter.fldID,
                            LetterOrderId = q.fldOrderId,
                            LetterCreateDate = q.fldCreatedDate.Substring(0, 10),
                            CreatorId = q.fldComisionID
                        });
                    }
                    else
                        return Json(new
                        {
                            Msg = "نامه توزیع شده و قابل تغییر نمی باشد.",
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
            var H = servic.GetContentFileWithFilter("MatnLetterId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
            var HaveFile = 0;
            if (H != null)
                HaveFile = H.fldId;
            return Json(new
            {
                fldID = q.fldID,
                fldComisionID = q.fldComisionID.ToString(),
                fldCreatedDate = q.fldCreatedDate.Substring(0, 10),
                fldCreatedDateEn = q.fldCreatedDateEn,
                fldImmediacyID = q.fldImmediacyID.ToString(),
                fldKeywords = q.fldKeywords,
                fldLetterDate = q.fldLetterDate,
                fldLetterNumber = q.fldLetterNumber,
                fldLetterStatusID = q.fldLetterStatusID,
                fldLetterTypeID = q.fldLetterTypeID,
                fldOrderId = q.fldOrderId,
                fldReceiver = q.fldReceiver,
                fldRecieverName = q.fldRecieverName,
                fldRoonevesht = q.fldRoonevesht,
                fldSecurityTypeID = q.fldSecurityTypeID.ToString(),
                fldSenderName = q.fldSenderName,
                fldSigner = q.fldSigner,
                fldSubject = q.fldSubject,
                fldSignType = q.fldSignType.ToString(),
                fldYear = q.fldYear,
                fldSignerName = q.fldSignerName,
                fldDesc = q.fldDesc,
                fldExternalSenderName = q.fldExternalSenderName,
                fldExternalSender = q.fldExternalSender,
                HaveFileLetter = HaveFile
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAssignmentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(l => new { fldId = l.fldID, fldName = l.fldType });
            return this.Store(q);
        }
        public int CheckIsDistribute(int LetterID)
        {
            Models.AutomationEntities p = new Models.AutomationEntities();
            var sign = servic.GetLetterWithFilter("fldId", LetterID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            int IsDistribute = 0;
            if (sign.fldLetterStatusID == 4)
                IsDistribute = 1;

            return IsDistribute;
        }
        [HttpPost]
        public FileContentResult Download(int ContentFileId)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {

                var q = servic.GetContentFileWithFilter("fldId", ContentFileId.ToString(), Convert.ToInt32(Session["OrganId"]),1, IP, out Err).FirstOrDefault();
                if (q != null && q.fldLetterText != null)
                {
                    MemoryStream st = new MemoryStream(q.fldLetterText);
                    return File(st.ToArray(), MimeType.Get(q.fldType), "DownloadFile" + q.fldType);
                }
                return null;
            }
        }
        public ActionResult ScanLetWin(string LetterId)
        {
            //return View();
            //if (Session["Username"] == null)
            //    return RedirectToAction("logon", "Account", new { area = "" });
            //else
            //{
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            PartialView.ViewBag.LetterId = LetterId;


            return PartialView;
            //}
        }
        public ActionResult ScanLetWin1(string LetterId)
        {
            ViewBag.LetterId = LetterId;
            return View();

            //Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.LetterId = LetterId;
            //return PartialView;

        }
        public ActionResult ScanLetWin2(string LetterId)
        {
            ViewBag.LetterId = LetterId;
            return View();

            //Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.LetterId = LetterId;
            //return PartialView;
            
        }
        public ActionResult ScanLetWin3(string LetterId)
        {
            ViewBag.LetterId = LetterId;
            return View();

            //Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.LetterId = LetterId;
            //return PartialView;

        }
        public ActionResult ScanLetWin4(string LetterId)
        {
            ViewBag.LetterId = LetterId;
            return View();

            //Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //PartialView.ViewBag.LetterId = LetterId;
            //return PartialView;

        }
        [HttpPost]
        public ActionResult SaveToFile()
        {
            try
            {
                String strImageName;
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                HttpPostedFile uploadfile = files["RemoteFile"];
                strImageName = uploadfile.FileName;

                uploadfile.SaveAs(Server.MapPath("/") + "\\UploadedImages\\" + strImageName);

            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public ActionResult SaveScanedFile(string LetterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; var Er = 0;
            byte[] fileLetter = null; string fileName = "", Pasvand = ""; ;
            try
            {
                string savePath = Server.MapPath(@"~\UploadedImages\WebTWAINImage.pdf");

                if (LetterId == "0")
                {
                    return Json(new
                    {
                        Msg = "لطفا ابتدا نامه زا ذخیره نمایید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    var IsDistribute = CheckIsDistribute(Convert.ToInt32(LetterId));
                    if (IsDistribute == 0)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                        fileLetter = stream.ToArray();
                        Pasvand = Path.GetExtension(savePath);
                        fileName = "فایل اسکن شده";

                        var HaveFile = servic.GetContentFileWithFilter("MatnLetterId", LetterId, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                        if (HaveFile == null)
                        {
                            servic.InsertContentFile(fileName, fileLetter, Convert.ToInt32(LetterId), Pasvand, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            MsgTitle = "ذخیره موفق";
                            Msg = "ذخیره با موفقیت انجام شد.";
                            if (Err.ErrorType)
                            {
                                Msg = Err.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;

                            }

                        }
                        else
                        {
                            servic.UpdateContentFile(HaveFile.fldId, fileName, fileLetter, Convert.ToInt32(LetterId), Pasvand, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            MsgTitle = "ذخیره موفق";
                            Msg = "ذخیره با موفقیت انجام شد.";
                            if (Err.ErrorType)
                            {
                                Msg = Err.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;
                            }
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "نامه مورد نظر توزیع شده است لذا امکان تغییر وجود ندارد..",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);

                    }
                }



                if (savePath != null)
                {
                    string physicalPath = System.IO.Path.Combine(savePath);
                    Session.Remove(savePath);
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
