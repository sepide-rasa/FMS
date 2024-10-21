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
using System.Text.RegularExpressions;
using System.Globalization;
using NewFMS.Models;

namespace NewFMS.Areas.Automation.Controllers
{
    public class SettingController : Controller
    {
        //
        // GET: /Automation/Setting/

       WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        WCF_Common.CommonService service_com=new WCF_Common.CommonService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int OrganId = Convert.ToInt32(Session["OrganId"]);
            var q = service_com.GetOrganizationWithFilter("fldID", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com).FirstOrDefault();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.OrganId = OrganId;
            result.ViewBag.OrganName = q.fldName;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
           // return PartialView;
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
        bool invalid = false;

        public ActionResult checkEmail(string Email)
        {

            if (String.IsNullOrEmpty(Email))
                invalid = false;

            else
            {
                Email = Regex.Replace(Email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);

                invalid = Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
            }
            return Json(new { valid = invalid }, JsonRequestBehavior.AllowGet);
        }
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        //public ActionResult Upload()
        //{
        //    if (Request.ContentLength <= 102400)
        //    {
        //        if (Session["savePath"] != null)
        //        {
        //            string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
        //            Session.Remove("savePath");
        //            Session.Remove("FileName");
        //            System.IO.File.Delete(physicalPath);
        //        }
        //        HttpPostedFileBase file = Request.Files[0];
        //        if ((file.ContentType).Substring(0, 5) == "image")
        //        {
        //            string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
        //            file.SaveAs(savePath);
        //            Session["FileName"] = file.FileName;
        //            Session["savePath"] = savePath;
        //            object r = new
        //            {
        //                success = true,
        //                name = Request.Files[0].FileName
        //            };
        //            return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
        //        }
        //        else
        //        {
        //            X.Msg.Show(new MessageBoxConfig
        //            {
        //                Buttons = MessageBox.Button.OK,
        //                Icon = MessageBox.Icon.ERROR,
        //                Title = "خطا",
        //                Message = "فایل غیرمجاز"
        //            });
        //            DirectResult result = new DirectResult();
        //            result.IsUpload = true;
        //            return result;
        //        }
        //    }
        //    else
        //    {
        //        X.Msg.Show(new MessageBoxConfig
        //        {
        //            Buttons = MessageBox.Button.OK,
        //            Icon = MessageBox.Icon.ERROR,
        //            Title = "خطا",
        //            Message = "حجم فایل بیشتر از حد مجاز است."
        //        });
        //        DirectResult result = new DirectResult();
        //        result.IsUpload = true;
        //        return result;
        //        //return null;
        //    }
        //}
        public ActionResult DeleteSessionFile()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "";
            if (Session["savePath"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                Session.Remove("savePath");
                Session.Remove("FileName");
                System.IO.File.Delete(physicalPath);
                Msg = "حذف فایل با موفقیت انجام شد";
            }
            return Json(new
            {
                Msg = Msg
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; int Er = 0;
            try
            {
                if (Session["savePath"] != null)
                {
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("FileName");
                    Session.Remove("savePath");
                }

                var IsImage = FileInfoExtensions.IsImage(Request.Files[0]);
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (IsImage == true/*extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff" || extension == ".gif"*/)
                {
                    if (Request.Files[0].ContentLength <= 1024000)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["savePath"] = savePath;
                        Session["FileName"] = Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
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
                            Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد.",
                            IsValid = false
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        //X.Msg.Show(new MessageBoxConfig
                        //{
                        //    Buttons = MessageBox.Button.OK,
                        //    Icon = MessageBox.Icon.ERROR,
                        //    Title = "خطا",
                        //    Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد."
                        //});
                        //DirectResult result = new DirectResult();
                        //return result;
                    }
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده غیر مجاز است.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //X.Msg.Show(new MessageBoxConfig
                    //{
                    //    Buttons = MessageBox.Button.OK,
                    //    Icon = MessageBox.Icon.ERROR,
                    //    Title = "خطا",
                    //    Message = "فایل انتخاب شده غیر مجاز است."
                    //});
                    //DirectResult result = new DirectResult();
                    //return result;
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
                return Json(new
                {
                    MsgTitle = MsgTitle,
                    Msg = Msg,
                    Er = 1
                });
            }
        }

         public FileContentResult ShowPic(int FileId, string dc)
        {//برگرداندن عکس 
            int OrganId = Convert.ToInt32(Session["OrganId"]);

            var q = servic.GetSettingWithFilter("fldId", FileId.ToString(),OrganId, 1, IP, out Err).FirstOrDefault();
            //if (q != null)
            //{
            //    if (q.fldFileId != null)
            //    {
            //        //return File(PDF(q.fldPic),".pdf");
            //        return File((q.fldFileId), ".jpg");
            //    }

            //}
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

         public ActionResult Save(WCF_Automation.OBJ_Setting Setting/*,bool IsDel*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                byte[] report_file = null; string FileName = "", e = "";

                if (Setting.fldID == 0)
                {//ذخیره
                    //if (Session["savePath"] != null)
                    //{
                    //    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    //    report_file = stream.ToArray();
                    //    FileName = Session["FileName"].ToString();
                    //    e = Path.GetExtension(Session["savePath"].ToString());
                    //    var lenght = stream.Length;
                    //    if (lenght > 102400)
                    //    {
                    //        Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                    //        MsgTitle = "اخطار";

                    //        return Json(new
                    //        {
                    //            Msg = Msg,
                    //            MsgTitle = MsgTitle,
                    //            Er = 1
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else
                    //{
                    //    report_file = null;
                    //    FileName = "";
                    //    e = "";
                    //}
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSetting(Setting.fldEmailAddress, Setting.fldEmailPassword, Setting.fldRecieveServer, Setting.fldSendServer, Setting.fldSendPort, Setting.fldSSL, Setting.fldDelFax, Setting.fldIsSigner, Setting.fldFaxPath, Setting.fldRecievePort, Setting.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    //if (Session["savePath"] != null)
                    //{
                    //    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    //    e = Path.GetExtension(Session["savePath"].ToString());
                    //    report_file = stream.ToArray();
                    //    FileName = Session["FileName"].ToString();
                    //    var lenght = stream.Length;
                    //    if (lenght > 102400)
                    //    {
                    //        Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                    //        MsgTitle = "اخطار";

                    //        return Json(new
                    //        {
                    //            Msg = Msg,
                    //            MsgTitle = MsgTitle,
                    //            Er = 1
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else
                    //{
                    //    if (IsDel == true)
                    //    {
                    //        report_file = null;
                    //        e = "";
                    //        FileName = "";
                    //    }
                    //    else
                    //    {

                    //        var pic = servic.GetSettingWithFilter("fldID", Setting.fldID.ToString(), Setting.fldOrganID, 1, IP, out Err).FirstOrDefault();
                    //        if (pic != null && pic.fldFileId != null)
                    //        {
                    //            var ff = service_com.GetFileWithFilter("fldId", pic.fldFileId.ToString(), 0, IP, out Err_com).FirstOrDefault();
                    //            report_file = ff.fldImage;
                    //            e = ff.fldPasvand;
                    //        }
                    //    }
                    //}

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSetting(Setting.fldID, Setting.fldEmailAddress, Setting.fldEmailPassword, Setting.fldRecieveServer, Setting.fldSendServer, Setting.fldSendPort, Setting.fldSSL, Setting.fldDelFax, Setting.fldIsSigner, Setting.fldFaxPath, Setting.fldRecievePort, Setting.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    System.IO.File.Delete(physicalPath);
                    Session.Remove("savePath");
                }
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }


         public ActionResult Details(int OrganId)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             var q = servic.GetSettingWithFilter("fldOrganID", OrganId.ToString(),0, 1, IP, out Err).FirstOrDefault();
             //byte[] file = null; string FileName = "";
             //int? FileId = 0;
             //if (q!=null &&q.fldFileId != null)
             //{
             //    var pic = service_com.GetFileWithFilter("fldID", q.fldFileId.ToString(), 1, IP, out Err_com).FirstOrDefault();
             //    if (pic.fldImage != null)
             //        file = pic.fldImage;
             //    FileId = q.fldFileId;
             //    FileName = "Pic." + pic.fldPasvand;
             //}
             //string Im = "";
             //if (file != null)
             //{
             //    Im = Convert.ToBase64String(file);
             //}
             if (q != null)
             {
                 return Json(new
                 {
                     fldID = q.fldID,
                     NameOrgan=CodeDecode.stringDecode(""),
                     fldOrganID = q.fldOrganID,
                     fldEmailAddress = q.fldEmailAddress,
                     fldEmailPassword = q.fldEmailPassword,
                     fldRecieveServer = q.fldRecieveServer,
                     fldSendServer = q.fldSendServer,
                     fldSendPort = q.fldSendPort,
                     fldSSL = q.fldSSL,
                     fldDelFax = q.fldDelFax,
                     fldIsSigner = q.fldIsSigner,
                     fldFaxPath = q.fldFaxPath,
                     fldRecievePort = q.fldRecievePort,
                     fldDesc = q.fldDesc,
                    // Image = Im,
                     //FileName = FileName
                 }, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 return Json(new
                 {
                     fldID = "",
                     fldOrganID="",
                     fldEmailAddress = "",
                     fldEmailPassword = "",
                     fldRecieveServer ="",
                     fldSendServer = "",
                     fldSendPort = "",
                     fldSSL ="",
                     fldDelFax = "",
                     fldIsSigner ="",
                     fldFaxPath = "",
                     fldFileId = "",
                     fldRecievePort = "",
                     fldDesc ="",
                    // FileName = FileName
                 }, JsonRequestBehavior.AllowGet);
             }
         }
        //public ActionResult Read(StoreRequestParameters parameters)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

        //    List<WCF_Automation.OBJ_Setting> data = null;
        //    //if (filterHeaders.Conditions.Count > 0)
        //    //{
        //    //    string field = "";
        //    //    string searchtext = "";
        //    //    List<WCF_Automation.OBJ_Setting> data1 = null;
        //    //    foreach (var item in filterHeaders.Conditions)
        //    //    {
        //    //        var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

        //    //        switch (item.FilterProperty.Name)
        //    //        {
        //    //            case "fldID":
        //    //                searchtext = ConditionValue.Value.ToString();
        //    //                field = "fldID";
        //    //                break;
        //    //            case "fldSecurityType":
        //    //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
        //    //                field = "fldSecurityType";
        //    //                break;
        //    //            case "fldDesc":
        //    //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
        //    //                field = "fldDesc";
        //    //                break;
        //    //        }
        //    //        if (data != null)
        //    //            data1 = servic.GetSettingWithFilter(field, searchtext, 0, 100, IP, out Err).ToList();
        //    //        else
        //    //            data = servic.GetSettingWithFilter(field, searchtext, 0, 100, IP, out Err).ToList();
        //    //    }
        //    //    if (data != null && data1 != null)
        //    //        data.Intersect(data1);
        //    //}
        //    //else
        //    //{
        //    //    data = servic.GetSettingWithFilter("", "", 0, 100, IP, out Err).ToList();
        //    //}

        //    //var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

        //    //FilterConditions fc = parameters.GridFilters;

        //    //-- start filtering ------------------------------------------------------------
        //    if (fc != null)
        //    {
        //        foreach (var condition in fc.Conditions)
        //        {
        //            string field = condition.FilterProperty.Name;
        //            var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

        //            data.RemoveAll(
        //                item =>
        //                {
        //                    object oValue = item.GetType().GetProperty(field).GetValue(item, null);
        //                    return !oValue.ToString().Contains(value.ToString());
        //                }
        //            );
        //        }
        //    }
        //    //-- end filtering ------------------------------------------------------------

        //    //-- start paging ------------------------------------------------------------
        //    int limit = parameters.Limit;

        //    if ((parameters.Start + parameters.Limit) > data.Count)
        //    {
        //        limit = data.Count - parameters.Start;
        //    }

        //    List<WCF_Automation.OBJ_Setting> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
        //    //-- end paging ------------------------------------------------------------

        //    return this.Store(rangeData, data.Count);
        //}
    }
}
