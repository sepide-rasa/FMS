using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using Ext.Net;
using Ext.Net.MVC;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Personeli/Admin/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        //WCF_Report.ReportService ReportService = new WCF_Report.ReportService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index()
        {
            Session["Username"] = "admin";
            Session["Password"] = "3781462CF954B7DAE340C624A8590B58FFF0B7E8";
            //if (Session["UserId"] == null)
            //    return RedirectToAction("Logon", "Account");D:\Fms\New Fms\NewFMS\NewFMS\NewFMS\Web References\WCF_FMS\CommonService.xsd
            return View();
        }
        public ActionResult image()
        {
            return null;
        }
        public ActionResult NewChangePass()
        {//باز شدن پنجره

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        /*   public ActionResult SaveChangePass(string fldPass, string fldNewPass, string fldConfirmPass)
           {
               string Msg = "", MsgTitle = "";
               try
               {
                   Models.MunPortalEntities p = new Models.MunPortalEntities();
                   var q = p.sp_tblUserSelect("fldId", Session["UserId"].ToString(), "", 1).FirstOrDefault();
                   var pass = fldPass.GetHashCode().ToString();
                   if (q.fldPassword == pass)
                   {
                       if (fldNewPass == fldConfirmPass)
                       {
                           p.sp_UserPassUpdate(q.fldId, fldNewPass.GetHashCode().ToString());
                           Msg = "ذخیره با موفقیت انجام شد.";
                           MsgTitle = "ذخیره موفق";

                       }
                       else
                       {
                           Msg = "رمز جدید با تکرار آن یکسان نیست.";
                           MsgTitle = "اخطار";
                       }
                   }
                   else
                   {
                       Msg = "رمز فعلی وارد شده معتبر نیست.";
                       MsgTitle = "اخطار";
                   }

               }
               catch (Exception x)
               {
                   return Json(new { data = x.InnerException.Message, state = 1 });
               }
               return Json(new
               {
                   Msg = Msg,
                   MsgTitle = MsgTitle
               }, JsonRequestBehavior.AllowGet);
           }*/

        public ActionResult Logout()
        {
            if (Session["UserId"] != null)
            {
                Session.RemoveAll();
            }
            FormsAuthentication.SignOut();
            return RedirectToAction("Logon", "Account");
        }

        public ActionResult Upload()
        {
            if (Session["savePath"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                Session.Remove("savePath");
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
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "فایل غیرمجاز"
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
        }


        //public FileContentResult ShowPicSetting(WCF_Common.OBJ_Setting Setting)
        //{//برگرداندن عکس 

        //    var q = servic.GetSettingWithFilter("", "", 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).FirstOrDefault();
        //    byte[] report_file = null;
        //    if (q != null)
        //    {

        //        return File((q.fldLogo), ".jpg");
        //    }

        //    else if (Session["savePath"] != null)
        //    {
        //        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
        //        report_file = stream.ToArray();
        //        return File((report_file), ".jpg");
        //    }

        //    return null;
        //}

    }
}
