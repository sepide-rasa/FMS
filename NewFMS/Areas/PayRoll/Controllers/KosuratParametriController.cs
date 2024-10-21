using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class KosuratParametriController : Controller
    {
        //
        // GET: /PayRoll/KosuratParametri/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();
            //int Permission_Parametr = 0;
            //if (servic.Permission(289, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err) &&
            //servic.Permission(634, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //    Permission_Parametr = 1;
            //else if (servic.Permission(289, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //    Permission_Parametr = 2;
            //else if (servic.Permission(634, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //    Permission_Parametr = 3;
            //result.ViewBag.Permission_Parametr = Permission_Parametr;
            return result;
        }
        public ActionResult CheckPermission(int OrganId)
        {
            int Permission_Parametr = 0;
            if (servic.Permission(289, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err) &&
            servic.Permission(634, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_Parametr = 1;
            else if (servic.Permission(289, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_Parametr = 2;
            else if (servic.Permission(634, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_Parametr = 3;

            return Json(new { Permission_Parametr = Permission_Parametr }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Deactive()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();
            //int Permission_Parametr = 0;
            //if (servic.Permission(1221, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err) &&
            //servic.Permission(1220, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //    Permission_Parametr = 1;
            //else if (servic.Permission(1221, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //    Permission_Parametr = 2;
            //else if (servic.Permission(1220, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
            //    Permission_Parametr = 3;
            //result.ViewBag.Permission_Parametr = Permission_Parametr;
            return result;
        }
        public ActionResult CheckPermissionDeactive(int OrganId)
        {
            int Permission_Parametr = 0;
            if (servic.Permission(1221, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err) &&
            servic.Permission(1220, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_Parametr = 1;
            else if (servic.Permission(1221, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_Parametr = 2;
            else if (servic.Permission(1220, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_Parametr = 3;

            return Json(new { Permission_Parametr = Permission_Parametr }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult KosuratParametriSingle(string containerId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.KosorateParametri_Personal(); 
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.OrganId = OrganId.ToString();
            return result;
        }
        public ActionResult ChangeStatus(int id, int State, string LastStatus, byte NoePardakht,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            int Tarikh=0;
            var Sal = "";
            var Month = "";
            var TarikhPardakht = "";
            var Tedad = 0;
            if (LastStatus == "0")
            {
                if (State == 1)
                {
                    var q = PayServic.GetKosorateParametri_PersonalDetail(id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    Tarikh = q.fldDateDeactive;
                    TarikhPardakht = q.fldTarikhePardakht;
                }
                else if (State == 2)
                {
                    var q = PayServic.GetMotalebateParametri_PersonalDetail(id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    Tarikh = q.fldDateDeactive;
                    TarikhPardakht = q.fldTarikhPardakht;
                }
                Sal = Tarikh.ToString().Substring(0, 4);
                Month = Tarikh.ToString().Substring(4);
            }
            else
            {
                if (State == 1)
                {
                    var q = PayServic.GetKosorateParametri_PersonalDetail(id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    TarikhPardakht = q.fldTarikhePardakht;
                    Tedad = q.fldTedad;
                }
                else if (State == 2)
                {
                    var q = PayServic.GetMotalebateParametri_PersonalDetail(id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    TarikhPardakht = q.fldTarikhPardakht;
                    Tedad = q.fldTedad;
                }
            }


            PartialView.ViewBag.id = id;
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.LastStatus = LastStatus;
            PartialView.ViewBag.Sal = Sal;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.TarikhPardakht = TarikhPardakht;
            PartialView.ViewBag.Tedad = Tedad;
            PartialView.ViewBag.NoePardakht = NoePardakht;
            PartialView.ViewBag.OrganId = OrganId.ToString();

            return PartialView;
        }
        public ActionResult PrintSooratHesabKosurat(int KosouratId, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.KosouratId = KosouratId;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult GeneratePDFSooratHesab(int KosouratId, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_PaySooratHesabKosuratParametriTableAdapter SooratHesab = new DataSet.DataSet1TableAdapters.spr_PaySooratHesabKosuratParametriTableAdapter();

                dt.EnforceConstraints = false;
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                if (KosouratId != 0)
                    SooratHesab.Fill(dt.spr_PaySooratHesabKosuratParametri, KosouratId, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                else
                    SooratHesab.Fill(dt.spr_PaySooratHesabKosuratParametri, 0, PersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\SooratHesabKosurat.frx");
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
        public ActionResult TavighKosurat(int PersonalId, int id, int ParametrId,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            var Sal = "";
            var Month = "";

            var SalTavigh = Convert.ToInt32(NewFMS.Models.CurrentDate.Year);
            var MonthTavigh = Convert.ToInt32(NewFMS.Models.CurrentDate.Month);

            var NoePardakht = 1;
            var q = PayServic.GetKosorateParametri_PersonalDetail(id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
            
            if (q.fldNoePardakht == false)
            {
                var Tarikh = q.fldDateDeactive;

                NoePardakht = 0;
                Sal = Tarikh.ToString().Substring(0, 4);
                Month = Tarikh.ToString().Substring(4);

                if(Convert.ToInt32(Month)==12)
                {
                    Month = "01";
                    Sal = (Convert.ToInt32(Sal) + 1).ToString();
                }
                else
                {
                    Month = (Convert.ToInt32(Month) + 1).ToString();
                    if (Month.Count() != 2)
                    {
                        Month = Month.PadLeft('0', 2);
                    }
                }

                //string tt = Sal + "/" + Month + "/01";
                //var kk = MyLib.Shamsi.Shamsi2miladiDateTime(tt);
                //kk = kk.AddMonths(1);
                //var Shamsi = MyLib.Shamsi.Miladi2ShamsiString(kk);

                //Sal = Shamsi.Substring(0, 4);
                //Month = Shamsi.Substring(5, 2);
            }
            else
            {
                var q2 = PayServic.GetTavighKosuratWithFilter("fldKosuratId", id.ToString(), 0, IP, out ErrPay).FirstOrDefault();
                if (q2 != null)
                {
                    if (q2.fldMonth == 12)
                    {
                        Month = "01";
                        Sal = (q2.fldYear + 1).ToString();
                    }
                    else
                    {
                        Month = (q2.fldMonth + 1).ToString();
                        if (Month.Count() != 2)
                        {
                            Month = Month.PadLeft('0', 2);
                        }
                        Sal = q2.fldYear.ToString();
                    }
                }
                else
                {
                    var Date = servic.GetDate(IP, out Err);
                    Sal = Date.fldTarikh.Substring(0, 4);
                    Month = Date.fldTarikh.Substring(5, 2);
                }
            }
            
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.SalTavigh = SalTavigh.ToString();
            PartialView.ViewBag.MonthTavigh = MonthTavigh.ToString();
            PartialView.ViewBag.Sal = Sal;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.ParametrId = ParametrId;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.NoePardakht = NoePardakht;
            PartialView.ViewBag.OrganId = OrganId.ToString();
            return PartialView;
        }
        public ActionResult ReloadKosuratParametriGroupDeactive(string Status,string Parametr, string Mablagh, string TarikhePardakht,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //List<NewFMS.WCF_PayRoll.OBJ_KosuratParametriGroup> data = null;
            List<NewFMS.WCF_PayRoll.OBJ_KosorateParametri_Personal> data = null;
            data = PayServic.GetKosorateParametri_PersonalWithFilter("RealoadDeactive", Status, Parametr
                       , Mablagh, TarikhePardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out ErrPay).ToList();
          //  data = PayServic.GetKosuratParametriGroupWithFilter("Deactive","",Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadKosuratParametriGroup(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.WCF_PayRoll.OBJ_KosuratParametriGroup> data = null;
            data = PayServic.GetKosuratParametriGroupWithFilter("", "",OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult KosuratParametriGroup(string containerId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var Date = servic.GetDate(IP, out Err);
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(Date.fldDateTime));
            result.ViewBag.OrganId = OrganId.ToString();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult KosuratParametriGroup_Deactive(string containerId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var Date = servic.GetDate(IP, out Err);
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(Date.fldDateTime));
            result.ViewBag.OrganId = OrganId.ToString();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult NewKosorateParametriSingle(int PersonalId, int Id, byte? DelCalc, int OrganId)
        {//باز شدن تب

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var Date = servic.GetDate(IP, out Err);
          /*  if (Id != 0)
            {
                var type = PayServic.GetCheckKosorat_Motalebat("Kosorat", Id, IP, out ErrPay).FirstOrDefault().fldType;
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
            }*/
            result.ViewBag.Id = Id;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.DelCalc = DelCalc;
            result.ViewBag.OrganId = OrganId.ToString();
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(Date.fldDateTime));
            return result;
        }

        public ActionResult GetKosouratParametri(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetParametrsWithFilter("Kosurat", "", OrganId.ToString()/*(Session["OrganId"]).ToString()*/, 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldParametrName = c.fldTitle });
            return this.Store(q);
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

        public ActionResult DetailsHeader(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetPayPersonalInfoWithFilter("fldId", Id.ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out ErrPay).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = servic.GetDate(IP, out Err);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4)) - 7;
            for (int i = fldsal; i < fldsal + 16; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();

                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckStatusFlag(int id)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 5;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = "";
            try
            {
                var ghatei = PayServic.GetCheckKosorat_Motalebat("KosoratGhatei", id, IP, out ErrPay).FirstOrDefault().fldType;
                if (ghatei == "1")
                    Flag = 3;
                else
                {
                    var type = PayServic.GetCheckKosorat_Motalebat("Kosorat", id, IP, out ErrPay).FirstOrDefault().fldType;
                    if (type == "1")
                    {
                        Flag = 4;
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
                Er = 1;
            }
            return Json(new
            {

                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id, int? fldPayPersonalId, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف

                if (DelCalc == 1)
                {
                    PayServic.MohasebatDelete("AllMohasebat_Personal_Kosorat", id, Convert.ToByte(0), Convert.ToInt16(0), Convert.ToByte(0), "", "",
                        Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out ErrPay);
                    if (ErrPay.ErrorType == true)
                    {
                        Msg = ErrPay.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;

                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                PayServic.DeleteKosorateParametri_Personal(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                if (ErrPay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrPay.ErrorMsg;
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetKosorateParametri_PersonalDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
            var q1 = PayServic.GetMohasebat_kosorat_MotalebatParamWithFilter("fldKosoratId", Id.ToString(), 0, IP, out ErrPay);
            string NoePardakht = "0";
            if (q.fldNoePardakht)
                NoePardakht = "1";
            string Status = "0";
            if (q.fldStatus)
                Status = "1";
            int Tarikh = q.fldDateDeactive;
           var Sal = Tarikh.ToString().Substring(0, 4);
            var Month = Tarikh.ToString().Substring(4);
            var use = false;
            if (q1.Count() > 0)
            {
                use = true;
            }
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                Sal = Sal,
                Month = Month,
                fldMablagh = q.fldMablagh,
                fldMondeFish = q.fldMondeFish,
                fldMondeGHabl = q.fldMondeGHabl,
                fldNoePardakht = NoePardakht,
                fldNoePardakhtName = q.fldNoePardakhtName,
                fldParametrId = q.fldParametrId.ToString(),
                fldStatus = Status,
                fldstatusName = q.fldstatusName,
                fldSumPardakhtiGHabl = q.fldSumPardakhtiGHabl,
                fldSumFish = q.fldSumFish,
                fldTarikhPardakht = q.fldTarikhePardakht,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldTedad = q.fldTedad,
                use=use,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveSingle(WCF_PayRoll.OBJ_KosorateParametri_Personal Kosurat, string fldYear, string fldMonth,byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";byte Er=0;

            try
            {
                
                if (Kosurat.fldDesc == null)
                    Kosurat.fldDesc = "";
                var fldDateDeactive = "";
                var Month = "";
                if (fldMonth.Count() != 2)
                {
                    fldMonth = fldMonth.PadLeft('0', 2);
                }
                if (Kosurat.fldNoePardakht == true && Kosurat.fldTedad != 0)
                {
                    Kosurat.fldTedad = 0;
                }
                fldDateDeactive = fldYear + fldMonth;

                if (Kosurat.fldId == 0)
                { //ذخیره
                    PayServic.InsertKosorateParametri_Personal(Kosurat.fldPersonalId, Kosurat.fldParametrId, Kosurat.fldNoePardakht,Kosurat.fldMablagh
                        , Kosurat.fldTedad, Kosurat.fldTarikhePardakht, Kosurat.fldSumFish, Kosurat.fldMondeFish,
                        Kosurat.fldSumPardakhtiGHabl, Kosurat.fldMondeGHabl, Kosurat.fldStatus, Convert.ToInt32(fldDateDeactive), Kosurat.fldDesc,
                        Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                    if (ErrPay.ErrorType)
                    {
                        Msg = ErrPay.ErrorMsg;
                        MsgTitle = "خطا";
                        Er=1;
                    }
                }
                else
                { //ویرایش
                    if (DelCalc == 1)
                    {
                        PayServic.MohasebatDelete("AllMohasebat_Personal_Kosorat", Kosurat.fldId, Convert.ToByte(0), Convert.ToInt16(0), Convert.ToByte(0), "", "",
                            Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out ErrPay);
                        if (ErrPay.ErrorType == true)
                        {
                            Msg = ErrPay.ErrorMsg;
                            MsgTitle = "خطا";
                            Er = 1;

                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    PayServic.UpdateKosorateParametri_Personal(Kosurat.fldId, Kosurat.fldPersonalId, Kosurat.fldParametrId, Kosurat.fldNoePardakht, Kosurat.fldMablagh
                        , Kosurat.fldTedad, Kosurat.fldTarikhePardakht, Kosurat.fldSumFish, Kosurat.fldMondeFish,
                        Kosurat.fldSumPardakhtiGHabl, Kosurat.fldMondeGHabl, Kosurat.fldStatus, Convert.ToInt32(fldDateDeactive), Kosurat.fldDesc,
                        Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
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
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
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
                Msg = PayServic.UpdateKosurat_Motalebat("Kosurat", fldStatus, id, Convert.ToInt32(fldDateDeactive), Session["Username"].ToString(),
                    Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                if (ErrPay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrPay.ErrorMsg;
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
        public ActionResult SaveTavigh(WCF_PayRoll.OBJ_TavighKosurat tavigh, int NoePardakht, short Year, byte Month,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var fldDateDeactive = "";
                string mah = Month.ToString();
                if (mah.Count() != 2)
                {
                    mah = mah.PadLeft('0', 2);
                }
                fldDateDeactive = Year.ToString() + mah;
                //ذخیره
                MsgTitle = "ذخیره موفق";
                if (NoePardakht == 0)
                {
                    PayServic.UpdateKosurat_Motalebat("Kosurat_DateActive", false, tavigh.fldKosuratId, Convert.ToInt32(fldDateDeactive),
                        Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    if (ErrPay.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrPay.ErrorMsg,
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                Msg = PayServic.InsertTavighKosurat(tavigh.fldKosuratId, tavigh.fldYear, tavigh.fldMonth, "", Session["Username"].ToString(),
                    Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                if (ErrPay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrPay.ErrorMsg;
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

        public ActionResult SaveGroup( WCF_PayRoll.OBJ_KosorateParametri_Personal KP, string PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; var exist = "";

            try
            {
                /*foreach (var item in KosorateParametri)
                {
                    var q=PayServic.GetKosorateParametri_PersonalWithFilter("CheckPersonal", item.fldPersonalId.ToString(), KP.fldParametrId.ToString()
                        , KP.fldMablagh.ToString(), KP.fldTarikhePardakht.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).FirstOrDefault();
                    if (q == null)
                    {
                        PayServic.InsertKosorateParametri_Personal(item.fldPersonalId,KP.fldParametrId,KP.fldNoePardakht,KP.fldMablagh, KP.fldTedad, KP.fldTarikhePardakht
                             , KP.fldSumFish,KP.fldMondeFish, KP.fldSumPardakhtiGHabl, KP.fldMondeGHabl, KP.fldStatus, KP.fldDateDeactive,"",
                             Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                        
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        exist = exist + q.fldName_Father + '،';
                    }
                }*/
                exist = PayServic.InsertGroupKosorateParametri_Personal(PersonalId, KP.fldParametrId, KP.fldNoePardakht, KP.fldMablagh, KP.fldTedad, KP.fldTarikhePardakht
                            , KP.fldSumFish, KP.fldMondeFish, KP.fldSumPardakhtiGHabl, KP.fldMondeGHabl, KP.fldStatus, KP.fldDateDeactive, "",
                            Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                if (ErrPay.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = ErrPay.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "ذخیره با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";
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
                exist=exist,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveGroupDeactive(WCF_PayRoll.OBJ_KosorateParametri_Personal KosorateParametri, string PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; var exist = "";

            try
            {


                PayServic.UpdateDeactiveKosorateParametri_Personal(PersonalId, KosorateParametri.fldParametrId,  KosorateParametri.fldMablagh, KosorateParametri.fldTarikhePardakht
                             , KosorateParametri.fldStatus, KosorateParametri.fldDateDeactive, "",
                             Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                   
                Msg = "عملیات با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";
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
                exist = exist,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
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
                        data1 = PayServic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetPayPersonalInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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
        public ActionResult ReadKosorateParametri(StoreRequestParameters parameters, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KosorateParametri_Personal> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_KosorateParametri_Personal> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldPersonalId_Id";
                            break;
                        case "fldParamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_ParamTitle";
                            break;
                        case "fldTedad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Tedad";
                            break;
                        case "fldTarikhePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_TarikhePardakht";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Mablagh";
                            break;
                        case "fldstatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_statusName";
                            break;
                        case "fldNoePardakhtName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NoePardakhtName";
                            break;
                    }
                    if (data != null)
                        data1 = PayServic.GetKosorateParametri_PersonalWithFilter(field, searchtext, PersonalId.ToString(), "", "",OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetKosorateParametri_PersonalWithFilter(field, searchtext, PersonalId.ToString(), "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetKosorateParametri_PersonalWithFilter("fldPersonalId", PersonalId.ToString(), "", "", "",OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<WCF_PayRoll.OBJ_KosorateParametri_Personal> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Reload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_KosorateParametri_Personal> data = null;
            data = PayServic.GetKosorateParametri_PersonalWithFilter("fldPersonalId", PersonalId.ToString(), "", "", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
