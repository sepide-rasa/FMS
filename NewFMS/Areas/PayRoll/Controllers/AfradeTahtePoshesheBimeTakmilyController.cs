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
using System.Data;
using NewFMS.Models;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class AfradeTahtePoshesheBimeTakmilyController : Controller
    {
        //
        // GET: /PayRoll/AfradeTahtePoshesheBimeTakmily/
        WCF_Personeli.PersoneliService servicP = new WCF_Personeli.PersoneliService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Personeli.ClsError ErrP = new WCF_Personeli.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.AfradTahtePoosheshTakmili(); ;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int id, int PersonalId, int PrsPersonalId,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.AfradTahtePooshesh();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.PrsPersonalId = PrsPersonalId;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult CopyTakmili()
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
          
            return PartialView;
        }
        public ActionResult UploadExcel()
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.AfradTahtePooshesh();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HistoryAfradeTahtePoshesh(int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult PrintHistoryAfradeTahtePoshesh(int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult GeneratePDFHistory(int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_HistoryAfradTahtePoshesheBimeTakmilyTableAdapter SooratHesab = new DataSet.DataSet1TableAdapters.spr_Pay_HistoryAfradTahtePoshesheBimeTakmilyTableAdapter();

                dt.EnforceConstraints = false;
                var User = servic_Com.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err_Com).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);

                SooratHesab.Fill(dt.spr_Pay_HistoryAfradTahtePoshesheBimeTakmily, PersonalId, OrganId);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptHistoryAfradTahtePoshesheBimeTakmily.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Save(WCF_PayRoll.OBJ_AfradeTahtePoshesheBimeTakmily BimeTakmily, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (BimeTakmily.fldDesc == null)
                    BimeTakmily.fldDesc = "";
                if (BimeTakmily.fldId == 0)
                { //ذخیره
                    servic.InsertAfradeTahtePoshesheBimeTakmily(BimeTakmily.fldPersonalId, BimeTakmily.fldTedadAsli, BimeTakmily.fldTedadTakafol60Sal,
                        BimeTakmily.fldTedadTakafol70Sal, BimeTakmily.fldGHarardadBimeId, BimeTakmily.fldAfradTahtePoshehsId, BimeTakmily.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);

                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش

                    servic.UpdateAfradeTahtePoshesheBimeTakmily(BimeTakmily.fldId, BimeTakmily.fldPersonalId, BimeTakmily.fldTedadAsli, BimeTakmily.fldTedadTakafol60Sal,
                        BimeTakmily.fldTedadTakafol70Sal, BimeTakmily.fldGHarardadBimeId, BimeTakmily.fldAfradTahtePoshehsId, BimeTakmily.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);

                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
        public ActionResult Copy(int GHarardadBimeId_To, int GHarardadBimeId_From, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                
                 //ذخیره
                servic.CopyAfradeTahtePoshesheBimeTakmily(GHarardadBimeId_From, GHarardadBimeId_To, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);

                    Msg = "عملیات با موفقیت انجام شد.";
                    MsgTitle = "عملیات موفق";
                
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
        public ActionResult CheckCopy()
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه کپی            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            try
            {
                var p = servic.GetGHarardadBimeWithFilter("ToCopy", "", 1, IP, out Err).FirstOrDefault();
                if (p == null)
                    Flag = 0;
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

                Flag = Flag,
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
        public ActionResult Delete(int id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteAfradeTahtePoshesheBimeTakmily(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAfradeTahtePoshesheBimeTakmilyDetail(Id, OrganId, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldGHarardadBimeId = q.fldGHarardadBimeId,
                fldPersonalId = q.fldPersonalId,
                fldTedadAsli = q.fldTedadAsli,
                fldTedadTakafol60Sal = q.fldTedadTakafol60Sal,
                fldTedadTakafol70Sal = q.fldTedadTakafol70Sal,
                fldNameBime = q.fldNameBime,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
       /* public ActionResult RloadAfrad(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_AfradeTahtePoshesheBimeTakmily_Details> data = null;

            data = servic.GetAfradeTahtePoshesheBimeTakmily_DetailsWithFilter("fldPersonalId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }*/
        public ActionResult GetFromBime()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetGHarardadBimeWithFilter("FromCopy", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameBime });
            return this.Store(q);
        }
        public ActionResult GetToBime()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetGHarardadBimeWithFilter("ToCopy", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameBime });
            return this.Store(q);
        }
        public ActionResult RloadAfrad(int PersonalId, int OrganId,string Id)
        {

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_AfradeTahtePoshesheBimeTakmily_Details> data = null;
            data = servic.GetAfradeTahtePoshesheBimeTakmily_DetailsWithFilter("fldPersonalId", PersonalId.ToString(), Id, OrganId,0, IP, out Err).ToList();
            var check = data.Where(l => l.fldId != 0).ToList();

            int[] checkId = new int[check.Count()];

            for (int i = 0; i < check.Count(); i++)
            {
                checkId[i] = check[i].fldId;
            }
            return Json(new
            {
                data = data,
                checkId = checkId
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Read(StoreRequestParameters parameters,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_PayPersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_PayPersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId_BimeTakmili";
                            break;
                        case "fldName_Father":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_Father_BimeTakmili";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode_BimeTakmili";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali_BimeTakmili";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPayPersonalInfoWithFilter("", "", OrganId, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_PayPersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadHistory(StoreRequestParameters parameters, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_HistoryAfradTahtePoshesheBimeTakmily> data = null;

            data = servic.GetHistoryAfradTahtePoshesheBimeTakmily(PersonalId, OrganId, IP, out Err).ToList();
            
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_PayRoll.OBJ_HistoryAfradTahtePoshesheBimeTakmily> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_AfradeTahtePoshesheBimeTakmily> data = null;

            data = servic.GetAfradeTahtePoshesheBimeTakmilyWithFilter("fldPersonalId", PersonalId.ToString(), 0, OrganId, 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    System.IO.File.Delete(physicalPath);
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var FileNamee = Path.GetFileNameWithoutExtension(Request.Files[0].FileName) + extension;
                var IsExcel = FileInfoExtensions.IsExcel(Request.Files[0]);
                if (IsExcel == true/*extension == ".xls" || extension == ".xlsx"*/)
                {
                    if (Request.Files[0].ContentLength <= 1048576)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + FileNamee);
                        file.SaveAs(savePath);
                        Session["savePath"] = savePath;
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
                       
                    }
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
                if (Session["savePath"] != null)
                {
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
                }
                return null;
            }
        }
       
        public ActionResult SaveExcel(int GharadadBime)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Msg = ""; var MsgTitle = ""; var Er = 0; string CodeMeli="",c="";
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                
                try
                {
                    CodeMeli = ProcessXlsLoc(Session["savePath"].ToString());
                    if (CodeMeli!="")
                    {
                         c=servic.GetAfradTahtePoosheshSelect_FromExcelWithFilter(CodeMeli, GharadadBime, Session["Username"].ToString(), (Session["Password"].ToString()),  IP, out Err).FirstOrDefault().fldCodemeli;
                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                            if (Err.ErrorType == true)
                            {
                                Msg = Err.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;
                            }
                    }
                    else
                    {
                        Er = 1;
                        Msg = "خطا در خواندن فایل Excel";
                        MsgTitle = "خطا";
                    }
                    System.IO.File.Delete(Session["savePath"].ToString());
                    Session.Remove("savePath");
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
                    Er = Er,
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    CodeMeli=c
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private string ProcessXlsLoc(string fileName)
        {
            string CodeMeli = "";
            try
            {
                
               
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(fileName);
                var sheet = wb.Worksheets[0];
                Regex r = new Regex(@"\d{4}/\d{2}/\d{2}");
               
                for (int i = sheet.Cells.MinDataRow + 1; i < sheet.Cells.MaxDataRow + 1; i++)
                {
                   
                    for (int j = sheet.Cells.MinDataColumn; j < sheet.Cells.MaxDataColumn + 1; j++)
                    {
                        var str = sheet.Cells[i, j].StringValue;
                        CodeMeli = CodeMeli + str+";";
                    }
                    
                }
                return CodeMeli;
             
            }
            catch (Exception x)
            {
                return CodeMeli;
            }
        }
       
    }
}
