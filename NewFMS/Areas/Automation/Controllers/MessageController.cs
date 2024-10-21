using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Web.Configuration;
namespace NewFMS.Areas.Automation.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Automation/Message/
        WCF_Common.CommonService Comservic = new WCF_Common.CommonService();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        public ActionResult Index(int CommisionId, int HeaderId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.CommisionId = CommisionId;
            result.ViewBag.HeaderId = HeaderId;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SaveFiles(string title, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
     
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {
                if (Session["savePathMessagefile"] != null)
                {
                   
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathMessagefile"].ToString()));
                        var file = stream.ToArray();
                        var Pasvand = Path.GetExtension(Session["savePathMessagefile"].ToString());
                        var q = servic.GetMessageAttachmentWithFilter("fldTitle", title,  Convert.ToInt32(Session["OrganId"]),0, IP, out Err).Where(l => l.fldMessageId == HeaderId).FirstOrDefault();
                        if (q == null)
                        {
                            servic.InsertMessageAttachment(title,HeaderId,  file, Pasvand, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                         
                            MsgTitle = "ذخیره موفق";
                            Msg = "ذخیره با موفقیت انجام شد.";
                        }
                        else
                        {
                            MsgTitle = "خطا";
                            Msg = "عنوان فایل وارد شده تکراری است.";
                            Er = 1;
                        }
                
                    if (Session["savePathMessagefile"] != null)
                    {
                        string physicalPath = System.IO.Path.Combine(Session["savePathMessagefile"].ToString());
                        Session.Remove("savePathMessagefile");
                        System.IO.File.Delete(physicalPath);
                    }
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "لطفا فایل مورد نظر را آپلود کنید.",
                        Er = 1
                    });
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
        public ActionResult ReadFile_Detail(int HeaderID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Automation.OBJ_MessageAttachment> data = null;
            data = servic.GetMessageAttachmentWithFilter("fldMessageId", HeaderID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            return this.Store(data);
       
        }
        public ActionResult CheckFormatSize(string Passvand)
        {
            var FileSize = 0;
            var Valid = false;
            var IsValidFormat = servic.GetLetterFileMojazWithFilter("fldPassvand", "%" + Passvand + "%", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l=>l.fldType==2).FirstOrDefault();
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
        public FileContentResult DownloadFile(int FileDetailId)
        {

            var q = servic.GetMessageAttachmentWithFilter("fldId", FileDetailId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            var q2 = Comservic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 0, IP, out ComErr).FirstOrDefault();
            return File((byte[])q2.fldImage, MimeType.Get(q.fldPasvand), q.fldTitle + "." + q.fldPasvand);
        }
        public ActionResult CheckDeleteFile(int FileDetailId, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; bool ExistFile = false; 
            try
            {//حذف

                    //var point = m.prs_tblPOintSelect("fldFileDetailId", FileDetailId.ToString(), 0).ToList();
                    //if (point.Count == 0)
                    //{
                       
                            MsgTitle = "حذف موفق";
                            Msg = "حذف با موفقیت انجام شد.";
                            servic.DeleteMessageAttachment(FileDetailId, Session["Username"].ToString(), Session["Password"].ToString(),  IP, out Err);
                   
                    //}
                    //else
                    //{
                    //    return Json(new
                    //    {
                    //        Msg = "پیام موردنظر قبلا ارسال شده و قادر به حذف پیوست آن نمی باشید.",
                    //        MsgTitle = "خطا",
                    //        Er = 1
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
       
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveHeaders(WCF_Automation.OBJ_Message Header, List<WCF_Automation.OBJ_MessageAttachment> Lessons)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var IdHeader = 0;
            try
            {
                if (Header.fldDesc == null)
                    Header.fldDesc = "";
                if (Header.fldId == 0)
                {
                    //ذخیره

                    //var q = servic.GetMessageWithFilter("fldTitle", Header.fldTitle, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    //    if (q == null)
                    //    {
                            IdHeader = servic.InsertMessage(Header.fldCommisionId, Header.fldTitle, Header.fldMatn, Header.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            var box = servic.GetBoxWithFilter("fldComisionID", Header.fldCommisionId.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l=>l.fldBoxTypeID==4).FirstOrDefault();
                            servic.InsertLetterBox(box.fldID, null, IdHeader, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            MsgTitle = "ذخیره موفق";
                            Msg = "ذخیره با موفقیت انجام شد.";
                        //}
                        //else
                        //{
                        //    return Json(new
                        //    {
                        //        Msg = "عنوان وارد شده تکراری است.",
                        //        MsgTitle = "خطا",
                        //        Er = 1,
                        //        IdHeader = IdHeader
                        //    }, JsonRequestBehavior.AllowGet);
                        //}
                  
                }
                else
                {
                    //ویرایش
                  
                        IdHeader = Header.fldId;
                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                        // var q = servic.GetMessageWithFilter("fldTitle", Header.fldTitle, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldId != Header.fldId).FirstOrDefault();
                        //if (q == null)
                        //{
                            servic.UpdateMessage(Header.fldId, Header.fldCommisionId, Header.fldTitle, Header.fldMatn, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                         
                        //}
                        //else
                        //{
                        //    return Json(new
                        //    {
                        //        Msg = "عنوان وارد شده تکراری است.",
                        //        MsgTitle = "خطا",
                        //        Er = 1,
                        //        IdHeader = Header.fldId
                        //    }, JsonRequestBehavior.AllowGet);
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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                IdHeader = IdHeader,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int HeaderId)
        {
            var q = servic.GetMessageWithFilter("fldId", HeaderId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldMatn = q.fldMatn
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePathMessagefile"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathMessagefile"].ToString());
                    Session.Remove("savePathMessagefile");
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
                        var FileNamee = Path.GetFileNameWithoutExtension(Request.Files[0].FileName) + Session["UserId"] + extension;
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + FileNamee);
                        file.SaveAs(savePath);
                        Session["savePathMessagefile"] = savePath;
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
                            Message = "حجم فایل انتخابی غیرمجاز است."
                        });
                        DirectResult result = new DirectResult();
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
                        Message = "فایل انتخاب شده غیرمجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (Session["savePathMessagefile"] != null)
                {
                    System.IO.File.Delete(Session["savePathMessagefile"].ToString());
                    Session.Remove("savePathMessagefile");
                }
                return null;
            }
        }
    }
}
