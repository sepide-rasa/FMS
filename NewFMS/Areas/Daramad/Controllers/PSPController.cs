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

namespace NewFMS.Areas.Daramad.Controllers
{
    public class PSPController : Controller
    {
        //
        // GET: /Daramad/PSP/

        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();
        WCF_Common.CommonService servic = new WCF_Common.CommonService();

        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
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
        public ActionResult Save(WCF_Daramad.OBJ_Psp psp, int IsDel)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                byte[] report_file = null; string FileName = "", e = "";
                if (Session["savePath"] != null && IsDel == 0)
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
                if (psp.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servicD.InsertPsp(psp.fldTitle, report_file, e, psp.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    if (Session["savePath"] == null)
                    {
                        byte[] picFile = null;
                        if (psp.fldFileId == 0)
                            picFile = report_file;
                        else
                        {
                            var pic = servic.GetFileWithFilter("fldId", psp.fldFileId.ToString(), 1, IP, out Err).FirstOrDefault();
                            if (pic.fldImage == null || IsDel == 1)
                                picFile = report_file;
                        }
                        MsgTitle = "ویرایش موفق";
                        Msg = servicD.UpdatePsp(psp.fldId, psp.fldTitle, psp.fldFileId, picFile, e, psp.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                        
                    }
                    else
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = servicD.UpdatePsp(psp.fldId, psp.fldTitle, psp.fldFileId, report_file, e, psp.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                        
                    }
                }
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
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
                Msg = servicD.DeletePsp(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
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
            var q = servicD.GetPspDetail(Id, IP, out ErrD);

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
            

            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldDesc = q.fldDesc,
                fldImage = Img,
                fldFileId = q.fldFileId
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_Psp> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_Psp> data1 = null;
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
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servicD.GetPspWithFilter(field, searchtext, 100,IP, out ErrD).ToList();
                    else
                        data = servicD.GetPspWithFilter(field, searchtext, 100,IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetPspWithFilter("", "", 100, IP, out ErrD).ToList();
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

            List<WCF_Daramad.OBJ_Psp> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
