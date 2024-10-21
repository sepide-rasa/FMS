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

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class SettingController : Controller
    {
        //
        // GET: /PayRoll/Setting/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common_Pay.Comon_PayService CPservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Personeli.PersoneliService Pservic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        WCF_Common_Pay.ClsError CPErr = new WCF_Common_Pay.ClsError();
        WCF_Personeli.ClsError PErr = new WCF_Personeli.ClsError();
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult(); ;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetBankWithFilter("", "", 0,IP, out CErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetShobe(int ID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Shobe = Cservic.GetSHobeWithFilter("fldBankId", ID.ToString(), 0, IP, out CErr);
            return Json(Shobe.Select(p1 => new { fldID = p1.fldId, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Details(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSettingWithFilter("fldOrganId",OrganId.ToString()/* Session["OrganId"].ToString()*/,0, 1, IP,out Err).FirstOrDefault();
            //var pic = Cservic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(), out CErr).FirstOrDefault();
            //var Img = Convert.ToBase64String(pic.fldImage);
            return Json(new
            {
                Id=q.fldId,
                B_BankFixId=q.fldB_BankFixId.ToString(),
                B_ShHesabId = q.fldB_ShomareHesabId.ToString(),
                Sh_CheckId = q.fldSh_HesabCheckId.ToString(),
                B_CodeShenasaee=q.fldB_CodeShenasaee,
                B_NameShobe=q.fldB_NameShobe,
                B_ShomareHesab=q.fldB_ShomareHesab,
                BankName=q.fldBankName,
                NameBankHoghoogh=q.NameBankHoghoogh,
                CodeDastgah=q.fldCodeDastgah,
                CodeEghtesadi=q.fldCodeEghtesadi,
                Codemeli=q.fldCodemeli,
                CodeOrganPasAndaz=q.fldCodeOrganPasAndaz,
                CodeParvande=q.fldCodeParvande,
                Family=q.fldFamily,
                H_BankFixId=q.fldH_BankFixId.ToString(),
                H_CodeOrgan=q.fldH_CodeOrgan,
                H_CodeShobe=q.fldH_CodeShobe,
                H_NameShobe=q.fldH_NameShobe,
                fldName=q.fldName,
                PostOrganName=q.fldPostOrganName,
                Prs_PersonalId=q.fldPrs_PersonalId,
                Sh_HesabCheck=q.fldSh_HesabCheck,
                ShowBankLogo=q.fldShowBankLogo,
                fldP_BankFixName = q.fldP_BankFixName,
                fldP_BankFixId = q.fldP_BankFixId.ToString(),
                P_NameShobe = q.fldP_NameShobe,
                fldP_ShobeId = q.fldP_ShobeFixId.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WCF_PayRoll.OBJ_Setting Setting)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                if (Setting.fldDesc == null)
                    Setting.fldDesc = "";

                var q = servic.GetSettingWithFilter("fldId", Setting.fldId.ToString(),Setting.fldOrganId/* Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                //if (q.fldId == 0)
                //{
                //    servic.insertSetting(Setting.fldId, Setting.fldH_BankFixId, Setting.fldH_NameShobe, Setting.fldH_CodeOrgan, Setting.fldH_CodeShobe
                //                        , Setting.fldShowBankLogo, Convert.ToInt32(Session["OrganId"]), Setting.fldCodeEghtesadi, Setting.fldPrs_PersonalId, Setting.fldCodeParvande,
                //                        Setting.fldCodeOrganPasAndaz, Setting.fldSh_HesabCheckId, Setting.fldB_BankFixId, Setting.fldB_NameShobe, Setting.fldB_ShomareHesabId,
                //                        Setting.fldB_CodeShenasaee, Setting.fldCodeDastgah, Setting.fldP_BankFixId, Convert.ToInt32(Setting.fldP_ShobeFixId), Setting.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                //    Msg = "ذخیره با موفقیت انجام شد.";
                //    MsgTitle = "خیره موفق";
                //    if (Err.ErrorType)
                //    {
                //        Er = 1;
                //        Msg = Err.ErrorMsg;
                //        MsgTitle = "خطا";
                //    }
                //}
                //else
                //{
                    servic.UpdateSetting(Setting.fldId, Setting.fldH_BankFixId, Setting.fldH_NameShobe, Setting.fldH_CodeOrgan, Setting.fldH_CodeShobe
                        , Setting.fldShowBankLogo,Setting.fldOrganId/* Convert.ToInt32(Session["OrganId"])*/, Setting.fldCodeEghtesadi, Setting.fldPrs_PersonalId, Setting.fldCodeParvande,
                        Setting.fldCodeOrganPasAndaz, Setting.fldSh_HesabCheckId, Setting.fldB_BankFixId, Setting.fldB_NameShobe, Setting.fldB_ShomareHesabId,
                        Setting.fldB_CodeShenasaee, Setting.fldCodeDastgah, Setting.fldP_BankFixId, Convert.ToInt32(Setting.fldP_ShobeFixId), Setting.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(),Setting.fldOrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
                    if (Err.ErrorType)
                    {
                        Er = 1;
                        Msg = Err.ErrorMsg;
                        MsgTitle = "خطا";
                    }
                //}
                
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
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
