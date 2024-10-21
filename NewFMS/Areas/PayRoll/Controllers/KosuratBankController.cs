using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;
using System.IO;
using System.Data;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class KosuratBankController : Controller
    {
        //
        // GET: /PayRoll/KosuratBank/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();

        WCF_PayRoll.PayRolService service_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult IndexNew(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.KosuratBank(); ;
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult PrintSooratHesabKosuratBank(int KosouratId, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.KosouratId = KosouratId;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.OrganId = OrganId;

            return PartialView;
        }
        public ActionResult GeneratePDFSooratHesabBank(int KosouratId, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_PaySooratHesabKosuratBankTableAdapter SooratHesab = new DataSet.DataSet1TableAdapters.spr_PaySooratHesabKosuratBankTableAdapter();

                dt.EnforceConstraints = false;
                var User = servic_Com.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err_Com).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                if (KosouratId != 0)
                    SooratHesab.Fill(dt.spr_PaySooratHesabKosuratBank, KosouratId, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                else
                    SooratHesab.Fill(dt.spr_PaySooratHesabKosuratBank, 0, PersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\SooratHesabKosuratBank.frx");
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
        public ActionResult New(int PersonalId, int id, string containerId,int OrganId)
        {//باز شدن تب

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var q = servic_Com.GetDate(IP,  out Err_Com);
            if (id != 0)
            {
                var type = service_Pay.GetCheckKosorat_Motalebat("Kosorat_Bank", id, IP, out Err_Pay).FirstOrDefault().fldType;
                if (type == "1")
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "آیتم مورد نظر در محاسبات استفاده شده و امکان ویرایش وجود ندارد."
                    });
                    DirectResult result2 = new DirectResult();
                    return result2;
                }
            }
            result.ViewBag.Id = id;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.OrganId = OrganId;
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(q.fldDateTime));
            return result;
        }
        public ActionResult ChangeStatus(int id, string LastStatus,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var Sal = "";
            var Month = "";
            var q = servic_Com.GetDate(IP, out Err_Com);
            Sal = q.fldTarikh.Substring(0, 4);
            Month = q.fldTarikh.Substring(5, 2);

            PartialView.ViewBag.id = id;
            PartialView.ViewBag.LastStatus = LastStatus;
            PartialView.ViewBag.Sal = Sal;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.OrganId = OrganId;

            return PartialView;
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic_Com.GetBankWithFilter("", "", 0, IP,  out Err_Com).ToList().Select(c => new { ID = c.fldId, Name = c.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetShobe(string ID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic_Com.GetSHobeWithFilter("fldBankId", ID, 0,IP,  out Err_Com).ToList().Select(c => new { fldId = c.fldId.ToString(), fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult EditDesc(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult LoadDescKosurat(int Id,int OrganId)
        {
            var q = service_Pay.GetKosuratBankDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            return Json(new {
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveDescKosurat(int Id, string Desc, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                MsgTitle = "ذخیره موفق";
                Msg = service_Pay.UpdateDescKosuratBank(Id, Desc, Session["Username"].ToString(), (Session["Password"].ToString()),
                    OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                if (Err_Pay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err_Pay.ErrorMsg;
                    Er = 1;
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_KosuratBank Kosurat , string fldYear, string fldMonth,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                if (Kosurat.fldDesc == null)
                    Kosurat.fldDesc = "";
                if (Kosurat.fldShomareSheba == null)
                    Kosurat.fldShomareSheba = "";
                var fldDateDeactive = ""; var fldShomareHesab = "";
                var Month = "";
                if (fldMonth.Count() != 2)
                {
                    Month = fldMonth.PadLeft('0', 2);
                    fldDateDeactive = fldYear + Month;
                }
                else
                {
                    fldDateDeactive = fldYear + fldMonth;
                }
                if (Kosurat.fldShomareHesab.Length < 10)
                {
                    fldShomareHesab = Kosurat.fldShomareHesab.PadLeft('0', 10);
                }
                else { fldShomareHesab = Kosurat.fldShomareHesab; }
                if (Kosurat.fldId == 0)
                { //ذخیره

                    service_Pay.InsertKosuratBank(Kosurat.fldPersonalId, Kosurat.fldShobeId, Kosurat.fldMablagh, Kosurat.fldCount,
                        Kosurat.fldTarikhPardakht, fldShomareHesab, Kosurat.fldStatus, Convert.ToInt32(fldDateDeactive), Kosurat.fldMandeAzGhabl, Kosurat.fldMandeDarFish,Kosurat.fldShomareSheba, Kosurat.fldDesc
                        , Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";

                }
                else
                { //ویرایش

                    service_Pay.UpdateKosuratBank(Kosurat.fldId, Kosurat.fldPersonalId, Kosurat.fldShobeId, Kosurat.fldMablagh, Kosurat.fldCount,
                        Kosurat.fldTarikhPardakht, fldShomareHesab, Kosurat.fldStatus, Convert.ToInt32(fldDateDeactive), Kosurat.fldMandeAzGhabl, Kosurat.fldMandeDarFish, Kosurat.fldShomareSheba, Kosurat.fldDesc
                        , Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";

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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف

                service_Pay.DeleteKosuratBank(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";

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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var BankId = 0;
            var q = service_Pay.GetKosuratBankDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            if (q != null)
            {
                var Shobe = servic_Com.GetSHobeDetail( q.fldShobeId, IP, out Err_Com);
                BankId = Shobe.fldBankId;
            }
            string Status = "0";
            if (q.fldStatus)
                Status = "1";
            string Tarikh = q.fldDeactiveDate.ToString();
            var Sal = Tarikh.ToString().Substring(0, 4);
            var Month = Tarikh.ToString().Substring(4, 2);
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldShobeId = q.fldShobeId.ToString(),
                BankId = BankId.ToString(),
                Sal = Sal,
                Month = Month,
                fldShomareHesab = q.fldShomareHesab,
                fldMablagh = q.fldMablagh,
                fldStatus = Status,
                fldTarikhPardakht = q.fldTarikhPardakht,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldTedad = q.fldCount,
                fldMandeAzGhabl = q.fldMandeAzGhabl,
                fldMandeDarFish = q.fldMandeDarFish,
                fldDesc=q.fldDesc,
                fldShomareSheba = q.fldShomareSheba
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsHeader(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = service_Pay.GetPayPersonalInfoDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DateDeactive(string TrikhePardakht, int Tedad)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            int Sal = Convert.ToInt32(TrikhePardakht.Substring(0, 4));
            int Mah = Convert.ToInt32(TrikhePardakht.Substring(5, 2));
            Mah = Mah + Tedad - 1;
            while (Mah > 12)
            {
                Sal += 1;
                Mah = Mah - 12;
            }
            return Json(new { Year = Sal, Mah = Mah.ToString().PadLeft('0', 2) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters,int OrganId)
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
                            field = "fldId";
                            break;
                        case "fldName_Father":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_Father";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomarePersoneli";
                            break;
                        case "fldJobeCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldJobeCode";
                            break;
                    }
                    if (data != null)
                        data1 = service_Pay.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                    else
                        data = service_Pay.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service_Pay.GetPayPersonalInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
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
        public ActionResult ReadKosuratBank(StoreRequestParameters parameters, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KosuratBank> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_KosuratBank> data1 = null;
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
                            field = "fldPersonalId_BankName";
                            break;
                        case "ShobeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_ShobeName";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Mablagh";
                            break;
                        case "fldCount":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Count";
                            break;
                        case "fldTarikhPardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_TarikhPardakht";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_ShomareHesab";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_StatusName";
                            break;
                        case "fldDeactiveDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_DeactiveDate";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Desc";
                            break;
                    }
                    if (data != null)
                        data1 = service_Pay.GetKosuratBankWithFilter(field, searchtext, PersonalId.ToString(), "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                    else
                        data = service_Pay.GetKosuratBankWithFilter(field, searchtext, PersonalId.ToString(), "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service_Pay.GetKosuratBankWithFilter("PersonalId", PersonalId.ToString(), "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
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

            List<WCF_PayRoll.OBJ_KosuratBank> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Reload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KosuratBank> data = null;

            data = service_Pay.GetKosuratBankWithFilter("PersonalId", PersonalId.ToString(),"","", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err_Pay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveStatus(int id, bool fldStatus, string fldYear, string fldMonth,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var fldDateDeactive = "";
                if (fldMonth.Count() != 2)
                {
                    fldMonth = fldMonth.PadLeft('0', 2);
                }
                fldDateDeactive = fldYear + fldMonth;
                //ذخیره
                MsgTitle = "ذخیره موفق";
                Msg = service_Pay.UpdateKosuratBankStatus(fldStatus, id, Convert.ToInt32(fldDateDeactive), Session["Username"].ToString(),
                    Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);

                if (Err_Pay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err_Pay.ErrorMsg;
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
    }
}
