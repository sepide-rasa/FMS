using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using System.IO;
using NewFMS.Models;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class ExcelTemplateController : Controller
    {
        // GET: Accounting/ExcelTemplate
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
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
        public ActionResult GetBank(int? HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (HeaderId == null)
                HeaderId = 0;
            var data = servic_Com.GetBankWithFilter("BankTemplate_DetailsId", HeaderId.ToString(), 0, IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldBankName, fldDetailsId = c.fldFileId });
            return this.Store(data);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePathTemplate"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathTemplate"].ToString());
                    Session.Remove("savePathTemplate");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var FileNamee = Path.GetFileNameWithoutExtension(Request.Files[0].FileName) + Session["UserId"] + extension;
                var IsExcel = FileInfoExtensions.IsExcel(Request.Files[0]);
                if (IsExcel == true)
                {
                    //if (Request.Files[0].ContentLength <= 1048576)
                    //{
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + FileNamee);
                        file.SaveAs(savePath);
                        Session["savePathTemplate"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName,
                            Message = "فایل با موفقیت آپلود شد.",
                            IsValid = true
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                    //else
                    //{
                    //    object r = new
                    //    {
                    //        success = true,
                    //        name = Request.Files[0].FileName,
                    //        Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد.",
                    //        IsValid = false
                    //    };
                    //    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده معتبر نمی باشد.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                }
            }
            catch (Exception x)
            {
                if (Session["savePathTemplate"] != null)
                {
                    System.IO.File.Delete(Session["savePathTemplate"].ToString());
                    Session.Remove("savePathTemplate");
                }
                return null;
            }
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_Accounting.OBJ_BankTemplate_Header> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_BankTemplate_Header> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNamePattern":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNamePattern";
                            break;
                        case "fldStartRow":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartRow";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetBankTemplate_HeaderWithFilter(field, searchtext, 0, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetBankTemplate_HeaderWithFilter(field, searchtext, 0, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetBankTemplate_HeaderWithFilter("", "",0, IP, out Err).ToList();
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
            List<WCF_Accounting.OBJ_BankTemplate_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Details(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetBankTemplate_HeaderDetail(HeaderId, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldDesc=q.fldDesc,
                fldFileId=q.fldFileId,
                fldNamePattern=q.fldNamePattern,
                fldStartRow=q.fldStartRow,
                fldBankId=q.fldBankId.Remove(q.fldBankId.Length - 1),
                fldDetailsId=q.fldDetailsId.Remove(q.fldDetailsId.Length - 1)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public FileContentResult Download(int fldFileId)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                var q = servic_Com.GetFileWithFilter("fldId", fldFileId.ToString(), 1, IP, out Err_Com).FirstOrDefault();
                if (q != null && q.fldImage != null)
                {
                    MemoryStream st = new MemoryStream(q.fldImage);
                    return File(st.ToArray(), MimeType.Get(q.fldPasvand), "DownloadFile" + q.fldPasvand);
                }
                return null;
            }
        }
        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteBankTemplate(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Save(WCF_Accounting.OBJ_BankTemplate_Header Header, List<Models.BankTemplate_Details> BankTemplate_DetailsArray)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "",Passvand=""; var Er = 0;  byte[] file = null;
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "acc.tblBankTemplate_Details" };
                using (var reader = FastMember.ObjectReader.Create(BankTemplate_DetailsArray))
                {
                    dt.Load(reader);
                }
                if (Header.fldId == 0)
                {
                    //ذخیره
                    if (Session["savePathTemplate"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathTemplate"].ToString()));
                        file = stream.ToArray();
                        Passvand = Path.GetExtension(Session["savePathTemplate"].ToString()).ToLower();
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg ="لطفا فایل الگو را بارگزاری نمایید.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertBankTemplate(Header.fldNamePattern, Header.fldStartRow,file,Passvand, dt,Header.fldDesc,Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        Msg = Err.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                }
                else
                {
                    //ویرایش
                    if (Session["savePathTemplate"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathTemplate"].ToString()));
                        file = stream.ToArray();
                        Passvand = Path.GetExtension(Session["savePathTemplate"].ToString()).ToLower();
                    }
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateBankTemplate(Header.fldId, Header.fldNamePattern, Header.fldStartRow,Header.fldFileId,file,Passvand, dt, Header.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        Msg = Err.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                }
                if (Session["savePathTemplate"] != null)
                {
                    System.IO.File.Delete(Session["savePathTemplate"].ToString());
                    Session.Remove("savePathTemplate");
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
                if (Session["savePathTemplate"] != null)
                {
                    System.IO.File.Delete(Session["savePathTemplate"].ToString());
                    Session.Remove("savePathTemplate");
                }
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