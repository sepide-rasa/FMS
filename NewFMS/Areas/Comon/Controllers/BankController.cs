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
using System.Configuration;

namespace NewFMS.Areas.Comon.Controllers
{
    public class BankController : Controller
    {
        //
        // GET: /Comon/Bank/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
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
            var Permission_TypeB = 0;
            if (servic.Permission(85, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err) &&
          servic.Permission(165,Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_TypeB = 1;
            else if (servic.Permission(85,Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_TypeB = 2;
            else if (servic.Permission(165,Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_TypeB = 3;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Permission_TypeB = Permission_TypeB;
            
            return result;
        }
        public ActionResult DeleteFile()
        {
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Session["FileName"] = "Blank.jpg";
                Session["savePath"] = Server.MapPath("~/Content/Blank.jpg");
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Upload()
        {
            if (Request.ContentLength <= 102400)
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }
                HttpPostedFileBase file = Request.Files[0];
                if ((file.ContentType).Substring(0, 5) == "image")
                {
                    string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    file.SaveAs(savePath);
                    Session["FileName"] = file.FileName;
                    Session["savePath"] = savePath;
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
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "حجم فایل بیشتر از حد مجاز است."
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
                //return null;
            }
        }
        public FileContentResult ShowPic(int FileId, string dc)
        {//برگرداندن عکس 
            var q = servic.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out Err).FirstOrDefault();
            if (q != null)
            {
                if (q.fldImage != null)
                {
                    //return File(PDF(q.fldPic),".pdf");
                    return File((q.fldImage), ".jpg");
                }

            }
            return null;

        }
        public ActionResult ShowPicUpload(string dc)
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }
        public ActionResult Save(WCF_Common.OBJ_Bank Bank, int IsDel)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                byte[] report_file = null; string FileName = "", e = "";
                if (Session["savePath"] != null && IsDel==0)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    report_file = stream.ToArray();
                    FileName = Session["FileName"].ToString();
                    e = Path.GetExtension(Session["savePath"].ToString());
                    var lenght = stream.Length;
                    if (lenght > 102400)
                    {
                        Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                        MsgTitle = "اخطار";

                        return Json(new
                        {
                            Msg = Msg,
                            MsgTitle = MsgTitle,
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var Image = Server.MapPath("~/Content/Blank.jpg");
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                    report_file = stream.ToArray();
                    FileName = "Blank.jpg";
                }
                if (Bank.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertBank(Bank.fldBankName,report_file,e,Bank.fldCentralBankCode,Bank.fldInfinitiveBank,Bank.fldFix, Bank.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    if (Session["savePath"] == null)
                    {
                        byte[] picFile = null;
                        if (Bank.fldFileId == 0)
                            picFile = report_file;
                        else
                        {
                            var pic = servic.GetFileWithFilter("fldId", Bank.fldFileId.ToString(), 1,IP, out Err).FirstOrDefault();
                            if (pic.fldImage == null || IsDel ==1)
                                picFile = report_file;
                        }
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateBank(Bank.fldId, Bank.fldBankName, Bank.fldFileId, picFile, e, Bank.fldCentralBankCode, Bank.fldInfinitiveBank, Bank.fldFix, Bank.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateBank(Bank.fldId, Bank.fldBankName, Bank.fldFileId, report_file, e, Bank.fldCentralBankCode, Bank.fldInfinitiveBank, Bank.fldFix, Bank.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    System.IO.File.Delete(physicalPath);
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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteBank(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetBankDetail(Id,  IP, out Err);

            var Image = Server.MapPath("~/Content/Blank.jpg");
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
            var Img = Convert.ToBase64String(stream.ToArray());
            int? FileId = 0;
            if (q.fldFileId != null)
            {
                var pic = servic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, IP, out Err).FirstOrDefault();
                if (pic.fldImage != null)
                    Img = Convert.ToBase64String(pic.fldImage);
                 FileId = q.fldFileId;
            }
            var fix="0";
            if (q.fldFix)
                fix = "1";

            return Json(new
            {
                fldId = q.fldId,
                fldBankName = q.fldBankName,
                fldInfinitiveBank=q.fldInfinitiveBank,
                fldCentralBankCode=q.fldCentralBankCode,
                fldDesc = q.fldDesc,
                fldImage = Img,
                fldFileId = q.fldFileId,
                fldFix = fix
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Bank> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_Bank> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldFixTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFixTitle";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetBankWithFilter(field, searchtext, 100,IP, out Err).ToList();
                    else
                        data = servic.GetBankWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetBankWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_Bank> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
