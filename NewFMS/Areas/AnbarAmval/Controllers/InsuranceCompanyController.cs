using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.AnbarAmval.Controllers
{
    public class InsuranceCompanyController : Controller
    {
        //
        // GET: /AnbarAmval/InsuranceCompany/

        WCF_AnbarAmval.AnbarAmvalService servic = new WCF_AnbarAmval.AnbarAmvalService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_AnbarAmval.ClsError Err = new WCF_AnbarAmval.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();
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
            var q = servic_C.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out Err_C).FirstOrDefault();
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
        public ActionResult Save(WCF_AnbarAmval.OBJ_InsuranceCompany InsuranceCompany)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                byte[] report_file = null; string FileName = "", e = "";
                if (Session["savePath"] != null)
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
                if (InsuranceCompany.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertInsuranceCompany(InsuranceCompany.fldName, report_file, e, InsuranceCompany.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateInsuranceCompany(InsuranceCompany.fldId, InsuranceCompany.fldName, InsuranceCompany.fldFileId, report_file, e, InsuranceCompany.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Msg = servic.DeleteInsuranceCompany(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetInsuranceCompanyDetail(Id, IP, out Err);

            var Image = Server.MapPath("~/Content/Blank.jpg");
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
            var Img = Convert.ToBase64String(stream.ToArray());
            int? FileId = 0;
            if (q.fldFileId != null)
            {
                var pic = servic_C.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, IP, out Err_C).FirstOrDefault();
                if (pic.fldImage != null)
                    Img = Convert.ToBase64String(pic.fldImage);
                FileId = q.fldFileId;
            }
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldFileId = q.fldFileId,
                fldImage = Img,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_AnbarAmval.OBJ_InsuranceCompany> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_AnbarAmval.OBJ_InsuranceCompany> data1 = null;
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
                        data1 = servic.GetInsuranceCompanyWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetInsuranceCompanyWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetInsuranceCompanyWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_AnbarAmval.OBJ_InsuranceCompany> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
