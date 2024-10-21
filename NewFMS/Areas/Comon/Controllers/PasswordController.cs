using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Comon.Controllers
{
    public class PasswordController : Controller
    {
        //
        // GET: /Comon/Password/
        /// <summary>
        /// این متد پنجره مربوط به رمز عبور را نمایش میدهد
        /// </summary>
        /// <param name="State"></param>
      
        public ActionResult Index(int State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;

        }
      /// <summary>
      ///. این متد صحیح یا اشتباه بودن رمز عبور را چک میکند
      /// </summary>
      /// <param name="Password">مقدار رمز عبور</param>
    
        public ActionResult Vorod(string Password)
        {
            var Resualt = false;
            if (Password == "رسا")
                Resualt = true;
            return Json(new
            {
                Resualt = Resualt
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
