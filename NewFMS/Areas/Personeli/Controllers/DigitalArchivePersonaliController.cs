using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class DigitalArchivePersonaliController : Controller
    {
        //
        // GET: /Personeli/DigitalArchivePersonali/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_Personeli.PersoneliService servicPersoneli = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();
       
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //if (servic.Permission(44, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //{
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
            //}
            //else
            //{
            //    return null;
            //}

        }
        public ActionResult GetArchiveTree()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDigitalArchiveTreeStructureWithFilter("fldAttacheFile", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult UploadFile(string id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.TreeId = id;
            Session["TreeId"] = id;
            return PartialView;
        }
        public ActionResult UploadContent()
        {
            if (Request.ContentLength <= 3000000)
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
                    //string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    string savePath = Server.MapPath(@"~\Uploaded" + Session["TreeId"]);
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
        public ActionResult ContentSave(int PersonalId, int TreeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";
            string savePath = Session["savePath"].ToString();

            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(savePath));

            byte[] _File = stream.ToArray();
            MsgTitle = "ذخیره موفق";
          //  Msg = servicPersoneli.InsertDigitalArchive(PersonalId, TreeId, _File, (Session["UserName"]).ToString(), Session["Password"].ToString(), IP, out ErrPersoneli);
            Session.Remove("savePath");
            if (Err.ErrorType)
            {
                MsgTitle = "خطا";
                Msg = Err.ErrorMsg;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckFileUpload(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDigitalArchiveTreeStructureWithFilter("fldId", Id.ToString(), 0,IP, out Err).FirstOrDefault();
            var UplodFile = 0;

            if (q.fldAttachFile)
                UplodFile = 1;
            return Json(new { UplodFile = UplodFile }, JsonRequestBehavior.AllowGet);

        }
        
    }
}
