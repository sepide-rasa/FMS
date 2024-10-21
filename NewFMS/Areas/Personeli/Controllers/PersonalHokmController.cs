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

namespace NewFMS.Areas.Personeli.Controllers
{
    public class PersonalHokmController : Controller
    {
        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService servic_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();
        // GET: PersonalHokm
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();
        }

        public ActionResult IndexNew(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.PersonalHokm(); 
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
        public ActionResult IndexGroup()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var m = servic_Com.GetDate(IP, out Err_Com);
            result.ViewBag.fldTarikh = m.fldDateTime.ToString("yyyy-MM-dd");
            return result;
        }
        public ActionResult HelpHokmGroup()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        [DirectMethod]
        public ActionResult New(int id, string containerId, int PersonalId, int Status, int? length,byte? HokmType)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPersoneliInfoDetail( PersonalId,Convert.ToInt32(Session["OrganId"]), IP, out Err);
           
            if (Err.ErrorType)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Err.ErrorMsg
                });
                DirectResult Dresult = new DirectResult();
                return Dresult;
            }
            if (q == null)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "لطفا ابتدا شخص مورد نظر را در ماژول حقوق و دستمزد ثبت نموده و سپس اقدام به تعریف حکم جدید نمایید. "
                });
                DirectResult Dresult = new DirectResult();
                return Dresult;
            }
            var p1 = servic_Com.GetPermissionWithFilter("HaveAcces", "1217", 1, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err_Com).Any();
            var p2 = servic_Com.GetPermissionWithFilter("HaveAcces", "1218", 1, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err_Com).Any();
            if (id != 0 && HokmType == 2)
            {
               
                if (p1 == false)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "شما مجاز به دسترسی ویرایش حکم اولیه نمی باشید."
                    });
                    DirectResult Dresult = new DirectResult();
                    return Dresult;
                }
            }
            if (id != 0 && HokmType == 3)
            {
                
                if (p2 == false)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "شما مجاز به دسترسی ویرایش حکم دستی نمی باشید."
                    });
                    DirectResult Dresult = new DirectResult();
                    return Dresult;
                }
            }
            int Gender = 0;
            int TedadFarzand = 0;
            string TarikhShamsi;
            if (q.fldJensiyat==true)
                Gender = 1;
            var T = servic.GetAfradTahtePoosheshWithFilter("fldPersonalId", PersonalId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP,out Err).Where(k => k.fldMashmul == true && k.fldNesbat==1).ToList();
            TedadFarzand = T.Count();
            var Org = servic_Com.GetOrganizationalPostsDetail(q.fldOrganPostId, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Com);

            var m = servic_Com.GetDate(IP, out Err_Com);
          
            TarikhShamsi = m.fldTarikh;
            /*var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();*/
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var Tarikh2=MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(m.fldTarikh,1)).ToString("yyyy-MM-dd");
            result.ViewBag.Id = id;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.PersonalName = q.fldName+" "+q.fldFamily + "(" + q.fldFatherName + ")";;
            result.ViewBag.SDate = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(m.fldDateTime));
            //result.ViewBag.AnvaeEstekhdamId = AnvaeEstekhdamId;
            //result.ViewBag.txtAnvaeEstekhdam = txtAnvaeEstekhdam;
            result.ViewBag.Gender = Gender;
            //result.ViewBag.TypeEstekhdam = TypeEstekhdam;
            result.ViewBag.OrgPostCode = Org.fldOrgPostCode;
            result.ViewBag.TedadFarzand = TedadFarzand;
            result.ViewBag.OrganId = q.fldOrganId;
            result.ViewBag.fldTarikh = m.fldDateTime.ToString("yyyy-MM-dd");
            result.ViewBag.fldTarikh2 =Tarikh2;
            result.ViewBag.TarikhShamsi = TarikhShamsi;
            //result.ViewBag.count = count;
            result.ViewBag.fldTaaholId = q.fldTaaholId.ToString();
            result.ViewBag.length = length;
            result.ViewBag.Status = Status;
            result.ViewBag.PerAvali = p1;
            result.ViewBag.PerDasti = p2;
            return result;
        }

          public ActionResult GetTypeEstekhdam(int id,int PersonalId,string TarikhEjra)
        
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int AnvaeEstekhdamId=0;
            string txtAnvaeEstekhdam = "";
            int TypeEstekhdam = 0;

               var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("Tarikh",PersonalId,TarikhEjra, IP,  out Err_Com_Pay).FirstOrDefault();
               if (f == null)
               {
                   f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", PersonalId, TarikhEjra,IP, out Err_Com_Pay).FirstOrDefault();
               }
              AnvaeEstekhdamId=f.fldAnvaEstekhdamId;
            txtAnvaeEstekhdam = f.fldTitle;

          //  var z = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Com_Pay);
           
                TypeEstekhdam = f.fldNoeEstekhdamId;//نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
           
            var Items_Estekhdam = servic_Com_Pay.GetItems_EstekhdamWithFilter("fldTypeEstekhdamId", TypeEstekhdam.ToString(), id, 0,IP, out Err_Com_Pay).ToList();
            var count = Items_Estekhdam.Count();

            return Json(new
            {
                AnvaeEstekhdamId = AnvaeEstekhdamId,
                txtAnvaeEstekhdam = txtAnvaeEstekhdam,
                TypeEstekhdam = TypeEstekhdam,
                count = count
            });
         }
          public ActionResult GeneratePDF(string PersonalHokmId, string PersonalId, int AnvaeEstekhdamId, string TarikhEjra, byte Details)
          {
              if (Session["Username"] == null)
                  return RedirectToAction("logon", "Account", new { area = "" });
              try
              {
                  var Person = servic.GetPersonalHokmDetail(Convert.ToInt32(PersonalHokmId), Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), IP, out Err);
                  if (Person.fldStatusHokm == true && Person.fldFileId != null && Details==0)
                  {
                      var f = servic_Com.GetFileWithFilter("fldId", Person.fldFileId.ToString(), 1, IP, out Err_Com).FirstOrDefault();
                      var FileHokm = servic_Com.GetFileWithFilter("fldId", Person.fldFileId.ToString(), 1, IP,out Err_Com).FirstOrDefault().fldImage;
                      return File(FileHokm, "application/pdf");
                  }
                  int CountTarikh = 0;
                  NewFMS.DataSet.DataSet1 dt = new DataSet.DataSet1();
                  dt.EnforceConstraints = false;
                  NewFMS.DataSet.DataSet_Com dt_Com= new DataSet.DataSet_Com();
                  NewFMS.DataSet.DataSet1TableAdapters.spr_tblPersonalHokmSelectTableAdapter PersonalHokm = new NewFMS.DataSet.DataSet1TableAdapters.spr_tblPersonalHokmSelectTableAdapter();
                  NewFMS.DataSet.DataSet1TableAdapters.spr_Prs_tblPersonalInfoSelectTableAdapter PersonalInfo = new NewFMS.DataSet.DataSet1TableAdapters.spr_Prs_tblPersonalInfoSelectTableAdapter();
                  NewFMS.DataSet.DataSet1TableAdapters.spr_PrsReportPersonalHokmTableAdapter Personal_History = new NewFMS.DataSet.DataSet1TableAdapters.spr_PrsReportPersonalHokmTableAdapter();
                  NewFMS.DataSet.DataSet1TableAdapters.spr_tblHokm_ItemSelectTableAdapter Hokm_Item = new NewFMS.DataSet.DataSet1TableAdapters.spr_tblHokm_ItemSelectTableAdapter();
                  //NewFMS.DataSet.DataSet_ComTableAdapters.spr_tblSettingSelectTableAdapter Setting = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_tblSettingSelectTableAdapter();
                  NewFMS.DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                  var User = servic_Com.GetUserWithFilter("CheckPass", Session["Username"].ToString(), (Session["Password"].ToString()), 1, IP, out Err_Com).FirstOrDefault();
                  var UserId = User.fldId;
                  PersonalInfo.Fill(dt.spr_Prs_tblPersonalInfoSelect, "fldId", PersonalId, Convert.ToInt32(Session["OrganId"]), 0);
                  PersonalHokm.Fill(dt.spr_tblPersonalHokmSelect, "fldId", PersonalHokmId, "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 0);
                  Personal_History.Fill(dt.spr_PrsReportPersonalHokm,Convert.ToInt32(PersonalHokmId));
                  if (Details == 0)
                      Hokm_Item.Fill(dt.spr_tblHokm_ItemSelect, "fldPersonalHokmId", PersonalHokmId, 0);
                  else
                      Hokm_Item.Fill(dt.spr_tblHokm_ItemSelect, "fldPersonalHokmId_Details", PersonalHokmId, 0);
                  //Setting.Fill(dt_Com.spr_tblSettingSelect, "", "", 0);
                  Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                  var Module_Organ = servic_Com.GetModule_OrganWithFilter("fldOrganId",Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err_Com).Where(l => l.fldModuleId == 2).FirstOrDefault();
                  var signers = servic_Com.GetSignersWithFilter(Module_Organ.fldId, TarikhEjra, 16,IP, out Err_Com).FirstOrDefault();
                  var SignerName="";
                  var PostName="";
                  if(signers!=null)
                  {
                      SignerName = signers.SignerName;
                      PostName = signers.Post;
                  }
                  FastReport.Report Report = new FastReport.Report();
                 // var AnvaeEstekhdam = servic_Com_Pay.GetAnvaEstekhdamDetail( AnvaeEstekhdamId, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Com_Pay);
                  var Sanavat = servic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", PersonalId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP,  out Err).ToList();
                  foreach (var item in Sanavat)
                  {
                      CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(item.fldAzTarikh, item.fldTaTarikh);
                  }
                  
                  CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(Person.fldTarikhEstekhdam, Person.fldTarikhEjra);

                  var DayMahSal = servic_Com_Pay.GetDiffDayMahSalWithFilter(CountTarikh, IP,  out Err_Com_Pay).FirstOrDefault();
                  var SanavatRasmi = DayMahSal.CountString;
          
                  var q =servic.GetHokmReportWithFilter ("fldAnvaEstekhdamId",Person.fldAnvaeEstekhdamId.ToString(),1,IP,  out Err).FirstOrDefault();
                  var user = servic_Com.GetUserWithFilter("fldUserName", Session["Username"].ToString(), "", 0, IP, out Err_Com).FirstOrDefault().fldNameEmployee;
                  if (q != null)
                  {
                      var s = servic_Com.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, IP, out Err_Com).FirstOrDefault();
                      MemoryStream m = new MemoryStream(s.fldImage);
                      Report.Load(m);
                      Report.RegisterData(dt, "rasaFMSPayRoll");
                      Report.RegisterData(dt_Com, "rasaFMSCommon");
                      FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                      MemoryStream stream = new MemoryStream();
                      Report.SetParameterValue("SanavatKhedmat", SanavatRasmi);
                      Report.SetParameterValue("SignerName",SignerName );
                      Report.SetParameterValue("PostName", PostName);
                      Report.SetParameterValue("UserName", user);
                      Report.Prepare();
                      Report.Export(pdf,stream);

                      if (Person.fldStatusHokm == true && Person.fldFileId == null)
                      {
                          servic.InsertFileForHokm(Person.fldId, stream.ToArray(), ".pdf", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                      }
                      return File(stream.ToArray(), "application/pdf");
                  }
                 else
                  {
                     /* if (AnvaeEstekhdam.fldNoeEstekhdamId == 1)
                      {

                          Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKargarRasmi.frx");
                      }
                      else if (AnvaeEstekhdam.fldNoeEstekhdamId == 2)
                      {

                          Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptHokm.frx");
                      }
                      else if (AnvaeEstekhdam.fldNoeEstekhdamId == 3)
                      {
                          Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKarmandPiymani.frx");
                      }
                      Report.RegisterData(dt, "rasaFMSPayRoll");
                      Report.RegisterData(dt_Com, "rasaFMSCommon");
                      FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                      MemoryStream stream = new MemoryStream();
                      Report.SetParameterValue("SanavatKhedmat", SanavatRasmi);
                      Report.SetParameterValue("SignerName", SignerName);
                      Report.SetParameterValue("PostName", PostName);
                      Report.SetParameterValue("UserName", user);
                      Report.Prepare();
                      Report.Export(pdf, stream);
                      if(Person.fldStatusHokm==true && Person.fldFileId==null)
                      {
                          servic.InsertFileForHokm(Person.fldId, stream.ToArray(), ".pdf", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                      }
                      return File(stream.ToArray(), "application/pdf");*/
                      return null;
                  }
              }
              catch (Exception x)
              {
                  var Msg = "";
                  if (x.InnerException != null)
                      Msg = x.InnerException.Message;
                  else
                      Msg = x.Message;
                  servic_Com.InsertError(Msg, servic_Com.GetDate(IP, out Err_Com).fldDateTime, IP, "", Session["Username"].ToString(), (Session["Password"].ToString()), out Err_Com);
                  return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet); 
              }
          }
          public ActionResult Print(string containerId, string PersonalHokmId, string PersonalId, string AnvaeEstekhdamId, string TarikhEjra, byte Details)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            /*var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo

            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();*/
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.PersonalHokmId = PersonalHokmId;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.AnvaeEstekhdamId = AnvaeEstekhdamId;
            result.ViewBag.TarikhEjra = TarikhEjra;
            result.ViewBag.Details = Details;
            return result;
        }

        public ActionResult Rload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalHokm> data = null;

            data = servic.GetPersonalHokmWithFilter("fldPrs_PersonalInfoId", PersonalId.ToString(), "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }        
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";int Er = 0;

            try
            {
                //حذف
                //m.sp_Prs_tblHokm_InfoPersonal_HistoryHokmIdDelete(id, 1);
                //m.sp_Prs_tblHokm_ItemHokmIdDelete(id, 1);
                servic.DeletePersonalHokm(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                //تصویر شخص و اشخاص پاک نشده
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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }

        public byte[] GetPDFHokm(string PersonalHokmId, string PersonalId, int AnvaeEstekhdamId, string TarikhEjra, byte Details            )
        {
            try
            {
                int CountTarikh = 0;
                NewFMS.DataSet.DataSet1 dt = new DataSet.DataSet1();
                NewFMS.DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                NewFMS.DataSet.DataSet1TableAdapters.spr_tblPersonalHokmSelectTableAdapter PersonalHokm = new NewFMS.DataSet.DataSet1TableAdapters.spr_tblPersonalHokmSelectTableAdapter();
                NewFMS.DataSet.DataSet1TableAdapters.spr_Prs_tblPersonalInfoSelectTableAdapter PersonalInfo = new NewFMS.DataSet.DataSet1TableAdapters.spr_Prs_tblPersonalInfoSelectTableAdapter();
                NewFMS.DataSet.DataSet1TableAdapters.spr_PrsReportPersonalHokmTableAdapter Personal_History = new NewFMS.DataSet.DataSet1TableAdapters.spr_PrsReportPersonalHokmTableAdapter();
                NewFMS.DataSet.DataSet1TableAdapters.spr_tblHokm_ItemSelectTableAdapter Hokm_Item = new NewFMS.DataSet.DataSet1TableAdapters.spr_tblHokm_ItemSelectTableAdapter();
                //NewFMS.DataSet.DataSet_ComTableAdapters.spr_tblSettingSelectTableAdapter Setting = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_tblSettingSelectTableAdapter();
                NewFMS.DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                var User = servic_Com.GetUserWithFilter("CheckPass", Session["Username"].ToString(), (Session["Password"].ToString()), 1, IP, out Err_Com).FirstOrDefault();
                var UserId = User.fldId;
                dt.EnforceConstraints = false;
                PersonalInfo.Fill(dt.spr_Prs_tblPersonalInfoSelect, "fldId", PersonalId, Convert.ToInt32(Session["OrganId"]), 0);
                PersonalHokm.Fill(dt.spr_tblPersonalHokmSelect, "fldId", PersonalHokmId, "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 0);
                Personal_History.Fill(dt.spr_PrsReportPersonalHokm, Convert.ToInt32(PersonalHokmId));
                if (Details == 0)
                    Hokm_Item.Fill(dt.spr_tblHokm_ItemSelect, "fldPersonalHokmId", PersonalHokmId, 0);
                else
                    Hokm_Item.Fill(dt.spr_tblHokm_ItemSelect, "fldPersonalHokmId_Details", PersonalHokmId, 0);
                //Setting.Fill(dt_Com.spr_tblSettingSelect, "", "", 0);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId,Convert.ToInt32(Session["OrganId"]));
                var Module_Organ = servic_Com.GetModule_OrganWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err_Com).Where(l => l.fldModuleId == 2).FirstOrDefault();
                var signers = servic_Com.GetSignersWithFilter(Module_Organ.fldId, TarikhEjra, 16,IP, out Err_Com).FirstOrDefault();
                var SignerName = "";
                var PostName = "";
                if (signers != null)
                {
                    SignerName = signers.SignerName;
                    PostName = signers.Post;
                }
                FastReport.Report Report = new FastReport.Report();
              //  var AnvaeEstekhdam = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com_Pay);
                var Sanavat = servic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                foreach (var item in Sanavat)
                {
                    CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(item.fldAzTarikh, item.fldTaTarikh);
                }
                var Person = servic.GetPersonalHokmDetail(Convert.ToInt32(PersonalHokmId), Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), IP, out Err);

                CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(Person.fldTarikhEstekhdam, Person.fldTarikhEjra);

                var DayMahSal = servic_Com_Pay.GetDiffDayMahSalWithFilter(CountTarikh, IP,out Err_Com_Pay).FirstOrDefault();
                var SanavatRasmi = DayMahSal.CountString;
         
                var q = servic.GetHokmReportWithFilter("fldAnvaEstekhdamId", Person.fldAnvaeEstekhdamId.ToString(), 1, IP, out Err).FirstOrDefault();
                var user = servic_Com.GetUserWithFilter("fldUserName", Session["Username"].ToString(), "", 0, IP, out Err_Com).FirstOrDefault().fldNameEmployee;
                if (q != null)
                {
                    var s = servic_Com.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, IP, out Err_Com).FirstOrDefault();
                    MemoryStream m = new MemoryStream(s.fldImage);
                    Report.Load(m);
                    Report.RegisterData(dt, "rasaFMSPayRoll");
                    Report.RegisterData(dt_Com, "rasaFMSCommon");
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.SetParameterValue("SanavatKhedmat", SanavatRasmi);
                    Report.SetParameterValue("SignerName", SignerName);
                    Report.SetParameterValue("PostName", PostName);
                    Report.SetParameterValue("UserName", user);
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    return stream.ToArray();
                }
                else
                {
                    
                    return null;
                }
            }
            catch (Exception x)
            {
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'HaghMaskan " + x.InnerException.Message + "'";
                else
                    excep = "'HaghMaskan " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
                return null;
            }
        }
        public ActionResult ConfirmHokm(int HokmId, string Prs_PersonalInfoId, int AnvaeEstekhdamId, string TarikhEjra, string fldNoeEstekhdamName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                var CheckHokm = servic.GetPersonalHokmWithFilter("CheckTaiidHokm", HokmId.ToString(), TarikhEjra, 0, Convert.ToInt32(Session["UserId"]), 0, IP, out Err).FirstOrDefault();
               if (CheckHokm != null)
               {
                   return Json(new
                   {
                       Er = 1,
                       Msg = "این حکم از حکم های قبل تاثیر گرفته است، لطفا قبل از تایید، یک بار ویرایش نمایید.",
                       MsgTitle = "خطا"
                   }, JsonRequestBehavior.AllowGet);
               }

                byte[] report_file = null;
                report_file = GetPDFHokm(HokmId.ToString(), Prs_PersonalInfoId, AnvaeEstekhdamId, TarikhEjra,0);
                if (report_file==null)
                {

                    return Json(new
                    {
                        Er = 1,
                        Msg = "برای نوع استخدام "+fldNoeEstekhdamName+" گزارش حکم تعریف نشده است؛ لطفا ابتدا گزارش مربوطه را تعریف کنید.",
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
                servic.InsertFileForHokm(HokmId, report_file, ".pdf", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                        if (Err.ErrorType)
                        {
                          
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                      return Json(new
                            {
                                Er = 0,
                                Msg = "تایید حکم با موفقیت انجام شد.",
                                MsgTitle = "عملیات موفق"
                            }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                return Json(new
                {
                    Er = 0,
                    Msg = Msg,
                    MsgTitle = MsgTitle
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Save(List<Models.ItemVal> ItemVal, WCF_Personeli.OBJ_PersonalHokm Hokm)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            byte[] report_file = null; string FileName = "", e = ""; int FileId = 0;/*فایل حکم که باید ذخیره بشههههههههههههههه*/
            string TarikhEtmam = null;
            string TarikhShoroo = null;
            try
            {
               // System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
                var CheckHokm=servic.GetPersonalHokmWithFilter("CheckHokm", Hokm.fldPrs_PersonalInfoId.ToString(), Hokm.fldTarikhEjra.ToString(), Hokm.fldId, 0,0, IP, out Err).ToList();
                var HokmToEdit = "";
                var txtHokmToEdit = "";
                foreach (var item in CheckHokm)
                {
                    HokmToEdit = HokmToEdit + item.fldId + "،";
                }
                if (HokmToEdit != "")
                    txtHokmToEdit = "(حکم با کدهای " + HokmToEdit + " نیازمند ویرایش می باشد.)";
             

                int _id = 0;string a;
                if (Hokm.fldDesc == null)
                    Hokm.fldDesc = "";
                if (Hokm.fldTarikhEtmam != null)
                    TarikhEtmam = Hokm.fldTarikhEtmam;
                if (Hokm.fldTarikhShoroo != null)
                    TarikhShoroo = Hokm.fldTarikhShoroo;
                if (Hokm.fldId == 0)
                { //ذخیره

                    _id=servic.InsertPersonalHokm(Hokm.fldPrs_PersonalInfoId, Hokm.fldTarikhEjra, Hokm.fldTarikhSodoor,
                        TarikhEtmam, Hokm.fldAnvaeEstekhdamId, Hokm.fldGroup, Hokm.fldMoreGroup, Hokm.fldShomarePostSazmani, Hokm.fldTedadFarzand,
                        Hokm.fldTedadAfradTahteTakafol, Hokm.fldTypehokm, Hokm.fldShomareHokm, Hokm.fldStatusHokm, Hokm.fldDescriptionHokm, Hokm.fldCodeShoghl, Hokm.fldStatusTaaholId, report_file, e
                        ,Hokm.fldMashmooleBime,Hokm.fldTatbigh1,Hokm.fldTatbigh2,Hokm.fldHasZaribeTadil,Hokm.fldZaribeSal1,Hokm.fldZaribeSal2,TarikhShoroo, Hokm.fldDesc,Hokm.fldHokmType,
                         Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), 0, out Err);
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
                    //var q = servic.GetPersoneliInfoDetail(Hokm.fldPrs_PersonalInfoId, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                     foreach (var item in ItemVal)
                         servic.InsertHokm_Item(_id, item.fldItems_EstekhdamId, item.fldMablagh,Convert.ToDecimal(item.fldZarib), "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);

                     Msg = "ذخیره با موفقیت انجام شد." + txtHokmToEdit;
                    MsgTitle = "ذخیره موفق";
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
                    if (Hokm.fldStatusHokm == true)
                    {
                        report_file = GetPDFHokm(_id.ToString(), Hokm.fldPrs_PersonalInfoId.ToString(), Hokm.fldAnvaeEstekhdamId, Hokm.fldTarikhSodoor,0);
                        servic.InsertFileForHokm(_id, report_file, ".pdf", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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
                    }
                   
                }
                else
                { //ویرایش

                    servic.UpdatePersonalHokm(Hokm.fldId, Hokm.fldPrs_PersonalInfoId, Hokm.fldTarikhEjra, Hokm.fldTarikhSodoor,
                       TarikhEtmam, Hokm.fldAnvaeEstekhdamId, Hokm.fldGroup, Hokm.fldMoreGroup, Hokm.fldShomarePostSazmani, Hokm.fldTedadFarzand,
                       Hokm.fldTedadAfradTahteTakafol, Hokm.fldTypehokm, Hokm.fldShomareHokm, Hokm.fldStatusHokm, Hokm.fldDescriptionHokm, Hokm.fldCodeShoghl, Hokm.fldStatusTaaholId, FileId, report_file,
                       e, Hokm.fldMashmooleBime, Hokm.fldTatbigh1, Hokm.fldTatbigh2, Hokm.fldHasZaribeTadil, Hokm.fldZaribeSal1, Hokm.fldZaribeSal2, TarikhShoroo, Hokm.fldDesc, Hokm.fldHokmType
                       , Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

                    servic.DeleteHokm_Item("fldHokmId", Hokm.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);

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

                    foreach (var item in ItemVal)
                        servic.InsertHokm_Item(Hokm.fldId, item.fldItems_EstekhdamId, item.fldMablagh, Convert.ToDecimal(item.fldZarib), "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                    Msg = "ویرایش با موفقیت انجام شد." + txtHokmToEdit;
                    MsgTitle = "ویرایش موفق";
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
                    if (Hokm.fldStatusHokm == true)
                    {
                        report_file = GetPDFHokm(Hokm.fldId.ToString(), Hokm.fldPrs_PersonalInfoId.ToString(), Hokm.fldAnvaeEstekhdamId, Hokm.fldTarikhSodoor,0);
                        servic.InsertFileForHokm(Hokm.fldId,report_file,".pdf","", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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
                    }
                   
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
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult Details(int Id)
        
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPersonalHokmDetail(Id, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), IP, out Err);
            string StatusHokm = "0";
            if (q.fldStatusHokm)
                StatusHokm = "1";
            var q2 = servic_Com_Pay.GetAnvaEstekhdamDetail(q.fldAnvaeEstekhdamId, IP, out Err_Com_Pay);

            return Json(new
            {
                TypeEstekhdam=q2.fldNoeEstekhdamId,
                fldId = q.fldId,
                fldAnvaeEstekhdamId = q.fldAnvaeEstekhdamId.ToString(),
                txtAnvaeEstekhdam = q.fldNoeEstekhdamName,
                fldCodeShoghl = q.fldCodeShoghl,
                fldGroup = q.fldGroup,
                fldMoreGroup = q.fldMoreGroup,
                fldPrs_PersonalInfoId = q.fldPrs_PersonalInfoId,
                PersonalKargoziniName = q.fldNameEmployee,
                fldShomareHokm = q.fldShomareHokm,
                fldShomarePostSazmani = q.fldShomarePostSazmani,
                fldDescriptionHokm = q.fldDescriptionHokm,
                fldStatusHokm = StatusHokm,
                fldStatusTaaholId = q.fldStatusTaaholId.ToString(),
                fldTarikhEjra = q.fldTarikhEjra,
                fldTarikhEtmam = q.fldTarikhEtmam,
                fldTarikhShoroo=q.fldTarikhShoroo,
                fldTarikhSodoor = q.fldTarikhSodoor,
                fldTedadAfradTahteTakafol = q.fldTedadAfradTahteTakafol,
                fldTedadFarzand = q.fldTedadFarzand,
                fldTypehokm = q.fldTypehokm,
                fldDesc = q.fldDesc,
                fldMashmooleBime=q.fldMashmooleBime,
                fldHasZaribeTadil=q.fldHasZaribeTadil,
                fldTatbigh1=q.fldTatbigh1,
                fldTatbigh2=q.fldTatbigh2,
                fldZaribeSal1=q.fldZaribeSal1,
                fldZaribeSal2=q.fldZaribeSal2,
                fldHokmType=q.fldHokmType.ToString(),
                fldHokmTypeName=q.fldHokmTypeName
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters, string PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalHokm> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalHokm> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "Personal_Id";
                            break;
                        case "fldNoeEstekhdamName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_NoeEstekhdamName";
                            break;
                        case "fldTarikhEjra":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_TarikhEjra";
                            break;
                        case "fldTarikhSodoor":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_TarikhSodoor";
                            break;
                        case "fldShomareHokm":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_ShomareHokm";
                            break;
                        case "fldStatusHokmName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_StatusHokmName";
                            break;
                        case "fldStatusTaaholName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_StatusTaaholName";
                            break;
                        case "fldTypehokm":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_Typehokm";
                            break;
                        case "fldSumItem":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_SumItem";
                            break;

                

                    }
                    if (data != null)
                        data1 = servic.GetPersonalHokmWithFilter(field, PersonalId, searchtext, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetPersonalHokmWithFilter(field, PersonalId, searchtext, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersonalHokmWithFilter("fldPrs_PersonalInfoId", PersonalId, "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalHokm> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadAshkhas(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameEmployee";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamilyEmployee";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetAnvaeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com_Pay.GetAnvaEstekhdamWithFilter("", "", 0, IP, out Err_Com_Pay).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetNoeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com_Pay.GetAnvaEstekhdamWithFilter("fldNoeEstekhdamId", "1", 0, IP, out Err_Com_Pay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetStatusTaahol()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetStatusTaaholWithFilter("", "", 0, IP, out Err_Com).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult ReloadItem(int Estekhdam, int HokmId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            //Boolean EsType = true;

            //var Est = servic.GetAnvaEstekhdamWithFilter("fldId", Estekhdam.ToString(), 0).FirstOrDefault();
            //if (Est.fldNoeEstekhdamId == 1)
            //    EsType = false;


            List<WCF_Common_Pay.OBJ_Items_Estekhdam> data = null;

            data = servic_Com_Pay.GetItems_EstekhdamWithFilter("fldTypeEstekhdamId", Estekhdam.ToString(), HokmId, 0, IP, out Err_Com_Pay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Formul(int PersonalId,string OrganId, int ItemHoghoghi, int TypeEstekhdam, double Zarib,
            int TaaholStatus, string Tarikh_Ejra, int Group, int TopGroup, byte TedadFarzandan
            , int Gender, List<Models.ClsHokmItems> HokmItem, string Type, string TarikhSodoor, int AnvaEstekhdam
            , int ZaribHoghAval,  int Tatbigh, bool HasZarib, double ZaribTadil,int mashmol)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                int j = 0;
                System.Data.DataSet dtFormul = new System.Data.DataSet();
                List<Models.ClsHokmItems> HokmItems_dt = new List<Models.ClsHokmItems>();
                if (HokmItem == null) return null;
                foreach (var item in HokmItem)
                {
                    HokmItems_dt.Add(new Models.ClsHokmItems
                    {
                        ItemHoghughiId = item.ItemHoghughiId,
                        Mablagh = item.Mablagh,
                        Zarib = item.Zarib,
                        ItemEstekhdamId = item.ItemEstekhdamId
                    });
                    //HokmItems_dt.Rows[j]["ItemHoghughiId"] = item.ItemHoghughiId;
                    //HokmItems_dt.Rows[j]["fldMablagh"] = item.Mablagh;
                    //HokmItems_dt.Rows[j]["fldZarib"] = item.Zarib;
                    //j++;
                }
                //dtFormul.Tables.Add(HokmItems_dt);
                //var Count = HokmItems_dt.Count();
                var fldPatternNoeEstekhdamId = servic_Com_Pay.GetAnvaEstekhdamDetail( AnvaEstekhdam, IP,out Err_Com_Pay).fldPatternNoeEstekhdamId;
                var q = servic_Com_Pay.GetComputationFormulaWithFilter("fldAzTarikh", Session["OrganId"].ToString(), Type.ToString(), Tarikh_Ejra.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_Com_Pay).FirstOrDefault();
                var PersonalInfo = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1,IP, out Err_Pay).FirstOrDefault();
                var TypeBimeId= PersonalInfo.fldTypeBimeId;
                string Formula = q.fldFormule;
                string[] ReferenceSplit = q.fldLibrary.Split(';').Reverse().Skip(1).Reverse().ToArray();

                ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
                CompilerParameters loParameters = new CompilerParameters();

                for (int i = 0; i < ReferenceSplit.Length; i++)
                {
                    if (ReferenceSplit[i] == "System.dll" || ReferenceSplit[i] == "System.Data.dll" || ReferenceSplit[i] == "System.Xml.dll" || ReferenceSplit[i] == "System.Core.dll")
                    {
                        loParameters.ReferencedAssemblies.Add(ReferenceSplit[i]);
                    }
                    else
                    {
                        var Path = Server.MapPath(@"~\Reference\" + ReferenceSplit[i]);
                        loParameters.ReferencedAssemblies.Add(Path);
                    }
                }

                loParameters.GenerateInMemory = true;
                CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Formula);

                Assembly loAssembly = loCompiled.CompiledAssembly;
                object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");
                /*ParameterModifier[] a = new ParameterModifier[2];
                string[] namee=new string[3];
                namee[0]="ZaribHoghNew";
                namee[1]="MashmulKosurat";
                namee[2] = "Parameters";*/
                object[] loCodeParms = new object[19];
                loCodeParms[0] = ItemHoghoghi;
                loCodeParms[1] = TypeEstekhdam;
                loCodeParms[2] = Zarib;
                loCodeParms[3] = TaaholStatus;
                loCodeParms[4] = Tarikh_Ejra;
                loCodeParms[5] = Group;
                loCodeParms[6] = TopGroup;
                loCodeParms[7] = TedadFarzandan;
                loCodeParms[8] = Gender;
                loCodeParms[9] = HokmItems_dt;
                loCodeParms[10] = PersonalId;
                loCodeParms[11] = TypeBimeId;
                loCodeParms[12] = AnvaEstekhdam;
                loCodeParms[13] = ZaribHoghAval;
                loCodeParms[14] = Tatbigh;
                loCodeParms[15] = HasZarib;
                loCodeParms[16] = ZaribTadil;
                loCodeParms[17] = mashmol;
                loCodeParms[18] = fldPatternNoeEstekhdamId;
                
                object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                List<Models.ClsHokmItems> data = (List<Models.ClsHokmItems>)loResult;

                //NewFMS.Controllers.TestFormula.FormulHokmController k = new NewFMS.Controllers.TestFormula.FormulHokmController();
                //k.Formulator(Convert.ToInt32(PersonalId), Convert.ToInt32(ItemHoghoghi), TypeEstekhdam, Zarib, TaaholStatus, Tarikh_Ejra, Group, TopGroup, TedadFarzandan, Gender, TypeBimeId, AnvaEstekhdam,
                //    HokmItems_dt, ZaribHoghAval, Tatbigh, HasZarib, ZaribTadil, mashmol);
                return Json(data, JsonRequestBehavior.AllowGet);
                
                //return RedirectToAction("Formulator", "FormulHokm", new
                //{
                //    PersonalId = PersonalId,
                //    ItemHoghoghi = ItemHoghoghi,
                //    TypeEstekhdam = TypeEstekhdam,
                //    Zarib = Zarib,
                //    TaaholStatus = TaaholStatus,
                //    Tarikh_Ejra = Tarikh_Ejra,
                //    Group = Group,
                //    TopGroup = TopGroup,
                //    TedadFarzandan = TedadFarzandan,
                //    Gender = Gender,
                //    dt = HokmItems_dt,
                //    TypeBimeId = TypeBimeId,
                //    AnvaEstekhdam = AnvaEstekhdam,
                //    ZaribHoghAval = ZaribHoghAval,
                //    Tatbigh = Tatbigh,
                //    HasZarib = HasZarib,
                //    ZaribTadil = ZaribTadil,
                //    mashmol = mashmol
                //});
                return null;
            }
            catch (Exception x)
            {
                
                    var m = x.Message.ToString();
                    if (x.InnerException != null)
                        m = x.InnerException.Message;
                    return Json(m, JsonRequestBehavior.AllowGet);
               
            }
        }
        public ActionResult ReloadHoghughMabna(int Group, int Estekhdam,string TarikhEjra)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int HMabna = 0;
            int HavHMabna = 0;
            Boolean EsType = true;
            int Year =Convert.ToInt32(TarikhEjra.Substring(0, 4));

          //  var Est = servic_Com_Pay.GetAnvaEstekhdamWithFilter("fldId", Estekhdam.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err_Com_Pay).FirstOrDefault();
            if (Estekhdam == 1)
                EsType = false;

            if (Group != 0)
            {
                var q = servic.GetDetailHoghoghMabnaWithFilter("fldGroh_Type", Group.ToString(), EsType, 0, IP, out Err).Where(k => k.fldYear == Year).FirstOrDefault();
                if (q != null)
                {
                    HavHMabna = 1;
                    HMabna = q.fldMablagh;
                }
            }
            return Json(new
            {
                HoghughMabna = HMabna,
                HavHMabna = HavHMabna
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MoteghayereAhkam(string TarikhEjra, int Estekhdam,string StatusTaahol)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string year = TarikhEjra.Substring(0, 4);
            Boolean EsType = true;
 
            int HaveAhkam = 0;
            //var Est = servic_Com_Pay.GetAnvaEstekhdamDetail(Estekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com_Pay);
            //if (Est != null)
            //{
            if (Estekhdam == 1)
                    EsType = false;
            else if (Estekhdam == 2 || Estekhdam == 3)
                    EsType = true;
            //}

            var q = servic.GetMoteghayerhaAhkamFilter("fldYear", year, 0 ,IP, out Err).Where(k => k.fldType == EsType).FirstOrDefault();
            if (q != null)
            {
                HaveAhkam = 1;
                //fldHadaghalDaryafti = q.fldHadaghalDaryafti;
                //fldHagheAeleMandi = q.fldHagheAeleMandi;
                //fldHagheOlad = q.fldHagheOlad;
                ////fldBarjestegi = q.fldBarjestegi;
                ////fldTakhasosi = q.fldTakhasosi;
                //fldMaskan = q.fldMaskan;
                //fldHaghBon = q.fldHaghBon;
                //if (StatusTaahol == "1")
                //    fldKharoBar = q.fldKharoBarMojarad;
                //else
                //    fldKharoBar = q.fldKharoBar;
            }

            return Json(new
            {
                HaveAhkam = HaveAhkam,
                year = year
                //fldHadaghalDaryafti = fldHadaghalDaryafti,
                //fldHagheOlad = fldHagheOlad,
                //fldHagheAeleMandi = fldHagheAeleMandi,
                ////fldBarjestegi = fldBarjestegi,
                ////fldTakhasosi=fldTakhasosi,
                //fldKharoBar = fldKharoBar,
                //fldMaskan = fldMaskan,
                //fldHaghBon = fldHaghBon
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ZaribHoghAval(int PersonalId,string TarikhEjra, int AnvaEstkhdam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string year = TarikhEjra.Substring(0, 4);
         var PersonalInfo = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                var TypeBimeId= PersonalInfo.fldTypeBimeId;
                var h = 0;
            var q = servic_Pay.GetMoteghayerhayeHoghopghi_Zarib(AnvaEstkhdam, TypeBimeId, year, IP, out Err_Pay).FirstOrDefault();
            if (q != null)
            {
                h = q.fldZaribHoghoghiSal;
            }

            return Json(new
            {
                ZaribHoghAval = h
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckPayPrsonel(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var PersonalInfo = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
            
            var h = 0;
            if (PersonalInfo != null)
            {
                h = 1;
            }

            return Json(new
            {
                h = h
            }, JsonRequestBehavior.AllowGet);
        }
        /*public ActionResult ZaribHoghNew(int ZaribHoghAval, decimal ZaribTadil)
        {
            int  h = 0;
            h =Convert.ToInt32(Math.Round(ZaribHoghAval + (ZaribHoghAval * ZaribTadil / 100)));

            return Json(new
            {
                ZaribHoghNew = h
            }, JsonRequestBehavior.AllowGet);
        }*/
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReloadPayeSanavati(int Group, int PersonalInfoKargoziniId, string TarikhEjra)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int PSanavati = 0;
            int HavPSanavati = 0;
            int PLastYear = 0;
            int Year = Convert.ToInt32(TarikhEjra.Substring(0, 4));
            int LastYear = Year - 1;
            var L = servic.GetAkharinHokmSalFilter(PersonalInfoKargoziniId, LastYear.ToString(), IP, out Err).FirstOrDefault();
            if (L != null)
                PLastYear = Hokm_Item_Mablagh(3, L.fldAnvaeEstekhdamId, L.fldId);

            if (Group != 0)
            {
                var q = servic.GetDetailPayeSanavatiFilter("fldGroh", Group.ToString(), 0, IP, out Err).Where(k => k.fldYear == Year).FirstOrDefault();
                if (q != null)
                {
                    HavPSanavati = 1;
                    PSanavati = q.fldMablagh;
                }
            }
            return Json(new
            {
                PayeSanavati = PSanavati,
                HavPSanavati = HavPSanavati,
                PayeShahrud = PLastYear + PSanavati
            }, JsonRequestBehavior.AllowGet);
        }
        public int Hokm_Item_Mablagh(int ItemID,int AnvaeEstekhdam,int HokmID)
        {
            var Mablagh = 0;
            var z = servic_Com_Pay.GetItems_EstekhdamWithFilter("fldAnvaeEstekhdamId", AnvaeEstekhdam.ToString(), HokmID, 0, IP, out Err_Com_Pay).Where(k => k.fldItemsHoghughiId == ItemID).FirstOrDefault();
            if (z != null)
                Mablagh = z.fldMablagh;
            return Mablagh;
        }
        public ActionResult Sanavat(string TarikhEjra, int PersonalInfoKargoziniId, int TypeEstekhdam,int zarib)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int fldSanavat = 0;
            string Year = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(servic_Com.GetDate(IP,  out Err_Com).fldDateTime)).Substring(0, 4);
            var q = servic.GetAkharinHokmSalFilter(PersonalInfoKargoziniId, Year.ToString() ,IP, out Err).FirstOrDefault();
            if (q != null)
            {
                if (q.fldYear == Year)
                {
                    fldSanavat = Hokm_Item_Mablagh(2, q.fldAnvaeEstekhdamId, q.fldId);
                }
                else
                {
                    if (TypeEstekhdam == 2)//کارمندی
                    {
                        fldSanavat = zarib * (Hokm_Item_Mablagh(1, q.fldAnvaeEstekhdamId, q.fldId) + Hokm_Item_Mablagh(2, q.fldAnvaeEstekhdamId, q.fldId) +
                                                Hokm_Item_Mablagh(4, q.fldAnvaeEstekhdamId, q.fldId) + Hokm_Item_Mablagh(5, q.fldAnvaeEstekhdamId, q.fldId))
                                    + Hokm_Item_Mablagh(2, q.fldAnvaeEstekhdamId, q.fldId);
                    }
                    else if (TypeEstekhdam == 1)//کارگری
                    {
                        fldSanavat = zarib * (Hokm_Item_Mablagh(2, q.fldAnvaeEstekhdamId, q.fldId)); //shahrood
                        //fldSanavat = zarib * (Hokm_Item_Mablagh(2, q.fldAnvaeEstekhdamId, q.fldId) + Hokm_Item_Mablagh(3, q.fldAnvaeEstekhdamId, q.fldId)); //maraghe
                    }
                }
            }
            return Json(new
            {
                fldSanavat = fldSanavat
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveHokmGroup(string TarikhEjra, string TarikhSodoor, string TarikhEtmam, string TarikhShoroo, bool StatusHokm, string Zarib, string DescriptionHokm, int AnvaeEstekhdamId, string Typehokm)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            byte[] report_file = null; string FileName = "", e = ""; int FileId = 0;

            int _id = 0; string a;
            try
            {
                var datainf = servic.GetPersoneliInfoWithFilter("PersonalActive", AnvaeEstekhdamId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                var fldPatternNoeEstekhdamId = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, IP, out Err_Com_Pay).fldPatternNoeEstekhdamId;
                foreach (var item in datainf)
                {
                    var Hokm = servic.GetPersonalHokmWithFilter("LastPersonalHokm", (Convert.ToInt32(TarikhEjra.Substring(0, 4)) - 1).ToString(), item.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 100, IP, out Err).FirstOrDefault();

                    if (Hokm != null)
                    {
                        _id = servic.InsertPersonalHokm(Hokm.fldPrs_PersonalInfoId, TarikhEjra, TarikhSodoor,
                            TarikhEtmam, AnvaeEstekhdamId, Hokm.fldGroup, Hokm.fldMoreGroup, Hokm.fldShomarePostSazmani, Hokm.fldTedadFarzand,
                            Hokm.fldTedadAfradTahteTakafol, Typehokm, "", StatusHokm, DescriptionHokm, Hokm.fldCodeShoghl, Hokm.fldStatusTaaholId, report_file, e
                            , Hokm.fldMashmooleBime, Hokm.fldTatbigh1, Hokm.fldTatbigh2, Hokm.fldHasZaribeTadil, Hokm.fldZaribeSal1, Hokm.fldZaribeSal2, TarikhShoroo, Hokm.fldDesc, Hokm.fldHokmType,
                             Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), 0, out Err);

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

                        var HokmItem = servic.GetHokm_ItemFilter("fldPersonalHokmIdGroupo", Hokm.fldId.ToString(), Convert.ToInt32(TarikhEjra.Substring(0,4)), IP, out Err).ToList();

                        int j = 0;
                        System.Data.DataSet dtFormul = new System.Data.DataSet();
                        List<Models.ClsHokmItems> HokmItems_dt = new List<Models.ClsHokmItems>();
                        foreach (var _item in HokmItem)
                        {
                            if (_item.fldItemsHoghughiId == 2)/*مزد سنوات ماهیانه*/
                                _item.fldZarib = Convert.ToDecimal(Zarib);

                            HokmItems_dt.Add(new Models.ClsHokmItems
                            {
                                ItemHoghughiId = _item.fldItemsHoghughiId,
                                Mablagh = _item.fldMablagh,
                                Zarib = Convert.ToDouble(_item.fldZarib),
                                ItemEstekhdamId = _item.fldItems_EstekhdamId
                            });

                        }

                        var q = servic_Com_Pay.GetComputationFormulaWithFilter("fldAzTarikh", Session["OrganId"].ToString(), "0", TarikhSodoor.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_Com_Pay).FirstOrDefault();
                        var PersonalInfo = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", item.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        if (PersonalInfo != null)
                        {
                            var TypeBimeId = PersonalInfo.fldTypeBimeId;
                            string Formula = q.fldFormule;
                            string[] ReferenceSplit = q.fldLibrary.Split(';').Reverse().Skip(1).Reverse().ToArray();

                            ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
                            CompilerParameters loParameters = new CompilerParameters();

                            for (int i = 0; i < ReferenceSplit.Length; i++)
                            {
                                if (ReferenceSplit[i] == "System.dll" || ReferenceSplit[i] == "System.Data.dll" || ReferenceSplit[i] == "System.Xml.dll" || ReferenceSplit[i] == "System.Core.dll")
                                {
                                    loParameters.ReferencedAssemblies.Add(ReferenceSplit[i]);
                                }
                                else
                                {
                                    var Path = Server.MapPath(@"~\Reference\" + ReferenceSplit[i]);
                                    loParameters.ReferencedAssemblies.Add(Path);
                                }
                            }

                            loParameters.GenerateInMemory = true;
                            CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Formula);

                            Assembly loAssembly = loCompiled.CompiledAssembly;
                            object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");

                            int Gender = 0;
                            if (item.fldJensiyat == true)
                                Gender = 1;

                            var HokmItem2 = servic.GetHokm_ItemFilter("fldPersonalHokmId", Hokm.fldId.ToString(), 0, IP, out Err).Where(l => l.fldItemsHoghughiId == 39).FirstOrDefault();
                            decimal z = 0;
                            if (HokmItem2 != null)
                                z = (decimal)HokmItem2.fldZarib;

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

                            object[] loCodeParms = new object[19];
                            loCodeParms[0] = "0";
                            loCodeParms[1] = "1";//کارگری
                            loCodeParms[2] = "0";
                            loCodeParms[3] = Hokm.fldStatusTaaholId;
                            loCodeParms[4] = TarikhEjra;
                            loCodeParms[5] = Hokm.fldGroup;
                            loCodeParms[6] = Hokm.fldMoreGroup;
                            loCodeParms[7] = Hokm.fldTedadFarzand;
                            loCodeParms[8] = Gender;
                            loCodeParms[9] = HokmItems_dt;
                            loCodeParms[10] = item.fldId;
                            loCodeParms[11] = TypeBimeId;
                            loCodeParms[12] = AnvaeEstekhdamId;
                            loCodeParms[13] = Hokm.fldZaribeSal1;
                            loCodeParms[14] = Hokm.fldTatbigh1;
                            loCodeParms[15] = Hokm.fldHasZaribeTadil;
                            loCodeParms[16] = z;
                            loCodeParms[17] = Hokm.fldMashmooleBime;
                            loCodeParms[18] = fldPatternNoeEstekhdamId;

                            object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                            List<Models.ClsHokmItems> data = (List<Models.ClsHokmItems>)loResult;

                            foreach (var item2 in data)
                                servic.InsertHokm_Item(_id, item2.ItemEstekhdamId, item2.Mablagh, Convert.ToDecimal(item2.Zarib), "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);

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

                            if (StatusHokm == true)
                            {
                                report_file = GetPDFHokm(_id.ToString(), item.fldId.ToString(), AnvaeEstekhdamId, TarikhSodoor,0);
                                servic.InsertFileForHokm(_id, report_file, ".pdf", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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
                            }
                        }
                    }

                    Msg = "ذخیره احکام با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}