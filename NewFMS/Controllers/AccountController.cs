using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using NewFMS.Filters;
using NewFMS.Models;
using System.IO;

namespace NewFMS.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        [AllowAnonymous]
        public FileContentResult generateCaptcha(string dc)
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("tahoma");
            CaptchaImage img = new CaptchaImage(90, 40, family);
            string text = img.CreateRandomText(5);
            text = text.ToUpper();
            text = text.Replace("O", "P").Replace("0", "2").Replace("1", "3").Replace("I", "M");
            img.SetText(text);
            img.GenerateImage();
            MemoryStream stream = new MemoryStream();
            img.Image.Save(stream,
            System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaLogin"] = text;
            return File(stream.ToArray(), "jpg");        }
        [AllowAnonymous]
        public ActionResult LogOn()
        {
            if (Session["captchahave"] == null)
                Session["captchahave"] = 0;
            ViewBag.Title = "ورود به سامانه";
            ViewBag.captchahave = Convert.ToInt32(Session["captchahave"]);
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Vorod(string UserName, string Password, string Capthalogin)
        {
            string Msg1 = ""; var Er = 0; var flag = false; string MsgTitle = "";
            try
            {
                //if (ModelState.IsValid)
                //{
                if (Convert.ToInt32(Session["captchahave"]) > 1)
                {
                    if (Capthalogin == "")
                    {
                        Session["captchaLogin"] = "Error";
                        MsgTitle = "خطا";
                        Msg1 = "لطفا کد امنیتی را وارد نمایید.";
                        Er = 1;
                        Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                        return Json(new
                        {
                            Msg = Msg1,
                            MsgTitle = MsgTitle,
                            flag = flag,
                            captchahave = Session["captchahave"]
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        if (Capthalogin.ToLower() != Session["captchaLogin"].ToString().ToLower())
                        {
                            Session["captchaLogin"] = "Error";
                            MsgTitle = "خطا";
                            Msg1 = "لطفا کد امنیتی را صحیح وارد نمایید.";
                            Er = 1;
                            Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                            return Json(new
                            {
                                Msg = Msg1,
                                MsgTitle = MsgTitle,
                                flag = flag,
                                captchahave = Session["captchahave"]
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                var UserId = servic.ExistUser(UserName, servic.HashPass(Password), IP, out Err);
                var k = servic.HashPass(Password);
                if (Err.ErrorType == false)
                {
                    Session["Password"] = servic.HashPass(Password);
                    string user = UserName;
                    Session["Username"] = user;
                    Session["UserId"] = UserId;
                    var date = servic.GetDate(IP, out Err).fldDateTime;

                    /***از کار افتادنه سمنان**/
                    if (servic.GetDate(IP, out Err).fldTarikh == "1399/01/01")
                        servic.ChangeLetter(IP, out Err);

                    servic.InsertInputInfo(date, IP, "", true, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    var h = servic.GetInputInfoWithFilter("fldUserId", Session["UserId"].ToString(), true,0,IP, out Err).FirstOrDefault();

                    if (UserLoginCount.userObj.Where(item => item.sessionId == System.Web.HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value.ToString()).Count() > 0)
                        UserLoginCount.userObj.Remove(UserLoginCount.userObj.Where(item => item.sessionId == System.Web.HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value.ToString()).FirstOrDefault());

                    UserLoginCount.AddOnlineUser(Session["UserId"].ToString(), Session["Username"].ToString(), IP, h.Name_Family, h.fldDateTime + "_" + h.fldTime, System.Web.HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value.ToString());

                  /*  var OrganId = servic.GetUserDetail(Convert.ToInt32(UserId), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldOrganId;
                    
                    Session["OrganId"] = OrganId;*/
                    //var OrganName = servic.GetOrganizationDetail(OrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldName;
                    //Session["OrganName"] = OrganName;        
                    
                    
                    //servic.InsertInputInfo(date, IP, "", true, null, Convert.ToInt32(Session["FristRegisterId"]), "", out Err);
                    //var l = servic.GetInputInfoFilter("fldFirstRegisterUser", Session["FristRegisterId"].ToString(), 0, true, out Err).FirstOrDefault();
                    //UserLoginCount.AddOnlineClient(Session["FristRegisterId"].ToString(), UserName, IP, Name, n.fldGroupName, l.fldDateTime + "_" + l.fldTime);
                    //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    flag = true;
                    MsgTitle = "ورود موفق";
                    //    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    //        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    //    {
                    //        return Redirect(returnUrl);
                    //    }
                    //    else
                    //    {
                    //        return RedirectToAction("index", "Home");
                    //    }

                }
                else
                {
                    servic.InsertLoginFailed(UserName, IP, out Err);
                    if (Err.ErrorMsg != "")
                        Msg1 = Err.ErrorMsg;
                    else
                        Msg1 = "نام کاربری یا رمز عبور اشتباه است";
                    MsgTitle = "ورود ناموفق";
                    Session["captchahave"] = Convert.ToInt32(Session["captchahave"]) + 1;
                }

                //else
                //{
                //ModelState.AddModelError("", Err.ErrorMsg);
                //return View(model);

                // }

                // If we got this far, something failed, redisplay form
                //return View(model);
            }
            catch (Exception x)
            {
                //System.IO.File.WriteAllText("d:\\rasaco\\a.txt", x.Message);
                //throw;
                 DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep =  x.InnerException.Message ;
                else excep=x.Message;
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
            }
            return Json(new
            {
                Msg = Msg1,
                MsgTitle = MsgTitle,
                flag = flag,
                captchahave = Session["captchahave"]
            }, JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        
    }
}
