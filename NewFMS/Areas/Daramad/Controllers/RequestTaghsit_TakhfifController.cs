using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Text.RegularExpressions;
using System.Globalization;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class RequestTaghsit_TakhfifController : Controller
    {
        //
        // GET: /Daramad/RequestTaghsit_Takhfif/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            var k = servic.GetRequestTaghsit_TakhfifWithFilter("fldElamAvarezId_NotEbtal", Id.ToString(), 0, IP, out Err).ToList();
            if (Err.ErrorType)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Err.ErrorMsg
                });
                DirectResult result1 = new DirectResult();
                return result1;
            }
            var HaveTakhfif = 0;
            var HaveTaghsit = 0;
            var ReplyTaghsit = 0;
            var ReplyTakhfif = 0;
            foreach (var item in k)
            {
                if (item.fldRequestType == 2)
                    HaveTakhfif = 1;
                else if (item.fldRequestType == 1)
                    HaveTaghsit = 1;

                var kk = servic.GetStatusTaghsit_TakhfifWithFilter("fldRequestId", item.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                if (kk != null)
                {
                    if (kk.fldTypeMojavez == 1)//موافقت
                    {
                        if (item.fldRequestType == 2)
                            ReplyTakhfif = 1;
                        else if (item.fldRequestType == 1)
                            ReplyTaghsit = 1;
                    }
                    else if (kk.fldTypeMojavez == 2)//مخالفت
                    {
                        if (item.fldRequestType == 2)
                            ReplyTakhfif = 2;
                        else if (item.fldRequestType == 1)
                            ReplyTaghsit = 2;
                    }
                }
            }
            var Permission_TypeRequest = 0;

            if (servic_Com.Permission(504, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err_Com) &&
          servic_Com.Permission(687, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err_Com))
                Permission_TypeRequest = 1;

            else if (servic_Com.Permission(504, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err_Com))
                Permission_TypeRequest = 2;
            else if (servic_Com.Permission(687, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err_Com))
                Permission_TypeRequest = 3;
            result.ViewBag.HaveTakhfif = HaveTakhfif;
            result.ViewBag.HaveTaghsit = HaveTaghsit;
            result.ViewBag.ReplyTaghsit = ReplyTaghsit;
            result.ViewBag.ReplyTakhfif = ReplyTakhfif;
            result.ViewBag.Permission_TypeRequest = Permission_TypeRequest;

            //if (ReplyTaghsit == 1 && HaveTakhfif == 0)
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.ERROR,
            //        Title = "خطا",
            //        Message = "با درخواست تقسیط موافقت شده، امکان ابطال آن و ثبت تخفیف وجود ندارد."
            //    });
            //    DirectResult result1 = new DirectResult();
            //    return result1;
            //}
            //else
                return result;
        }
        bool invalid = false;
        public ActionResult checkEmail(string Email)
        {

            if (String.IsNullOrEmpty(Email))
                invalid = false;

            else
            {
                Email = Regex.Replace(Email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);

                invalid = Regex.IsMatch(Email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
            }
            return Json(new { valid = invalid }, JsonRequestBehavior.AllowGet);
        }
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        public ActionResult Details(byte RequestType, int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetRequestTaghsit_TakhfifWithFilter("fldElamAvarezId_NotEbtal", ElamAvarezId.ToString(), 0, IP, out Err).Where(l => l.fldRequestType == RequestType).FirstOrDefault();
            if (q != null)
                return Json(new
                {
                    HaveReq = 1,
                    fldId = q.fldId,
                    fldEmployeeId = q.fldEmployeeId,
                    fldEmployeeName=q.fldName_Family,
                    fldAddress = q.fldAddress,
                    fldRequestType = q.fldRequestType.ToString(),
                    fldCodePosti = q.fldCodePosti,
                    fldElamAvarezId = q.fldElamAvarezId,
                    fldEmail = q.fldEmail,
                    fldMobile = q.fldMobile,
                    fldStatusId = q.fldStatusId,
                    fldDesc=q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            else {
                var fldEmployeeId=0;
                var fldEmployeeName="";
                var s = servic.GetElamAvarezDetail(ElamAvarezId,(Session["OrganId"]).ToString(), IP, out Err);
                var h = servic_Com.GetAshkhasDetail(s.fldAshakhasID, IP, out Err_Com);

                if(h.fldHaghighiId!=null){
                var k= servic_Com.GetEmployeeDetail(Convert.ToInt32(h.fldHaghighiId),IP, out Err_Com);
                    fldEmployeeId=k.fldId;
                    fldEmployeeName=k.fldName+" "+k.fldFamily;
                }
                return Json(new
                {
                    HaveReq = 0,
                    fldEmployeeId=fldEmployeeId,
                    fldEmployeeName = fldEmployeeName,
                    fldRequestType = RequestType.ToString(),
                    fldElamAvarezId=ElamAvarezId,
                    fldId = 0,
                    /*fldAddress = "",
                    fldCodePosti = "",
                    fldEmail = "",
                    fldMobile = "",*/
                    fldStatusId ="2",
                    fldDesc = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Save(WCF_Daramad.OBJ_RequestTaghsit_Takhfif req)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var IDReq = 0; var HaveTakhfif = 0; var HaveTaghsit = 0;
            try
            {
                if (req.fldAddress == "")
                    req.fldAddress = null;
                if (req.fldEmail == "")
                    req.fldEmail = null;
                if (req.fldCodePosti == "")
                    req.fldCodePosti = null;

                if (req.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "عملیات موفق";
                    Msg = "ثبت درخواست با موفقیت انجام شد.";
                    IDReq = servic.InsertRequestTaghsit_Takhfif(req.fldElamAvarezId, req.fldRequestType, req.fldEmployeeId, req.fldAddress, req.fldEmail, req.fldCodePosti,
                        req.fldMobile, req.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "عملیات موفق";

                    //var q = servic.GetRequestTaghsit_TakhfifWithFilter("fldElamAvarezId", req.fldElamAvarezId.ToString(), 0, IP, out Err).Where(l => l.fldRequestType == req.fldRequestType).FirstOrDefault();
                    //if(q)
                    Msg = servic.UpdateRequestTaghsit_Takhfif(req.fldId, req.fldElamAvarezId, req.fldRequestType, req.fldEmployeeId, req.fldAddress, req.fldEmail, req.fldCodePosti,
                        req.fldMobile, req.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    IDReq = req.fldId;
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }

                var k = servic.GetRequestTaghsit_TakhfifWithFilter("fldElamAvarezId_NotEbtal", req.fldElamAvarezId.ToString(), 0, IP, out Err).ToList();
                foreach (var item in k)
                {
                    if (item.fldRequestType == 2)
                        HaveTakhfif = 1;
                    if (item.fldRequestType == 1)
                        HaveTaghsit = 1;
                }
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
                Err = Er,
                IDReq = IDReq,
                HaveTakhfif = HaveTakhfif,
                HaveTaghsit=HaveTaghsit
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EbtalRequest(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                    MsgTitle = "عملیات موفق";
                    Msg = servic.InsertEbtal(null, Id, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                
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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
