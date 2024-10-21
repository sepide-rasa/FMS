using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Hogaf;
using Hogaf.ExtNet.UX;
using Hogaf.ExtNet;
using Controller = System.Web.Mvc.Controller;
using System.Web.Configuration;

using NewFMS.Models;

namespace NewFMS.Controllers
{
    public class ChangingPositionController : Controller
    {
        //
        // GET: /ChangingPosition/
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();            
            return PartialView;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Save(int fldOrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            Session["OrganId"] = fldOrganId;

            var desktopp = X.GetCmp<Desktop>("Desktop1");
            var q = servic_Com.GetModule_OrganWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err_Com).Where(l => l.fldOrganId == fldOrganId).ToList();
            List<int> allowModule = new List<int>();
            foreach (var item in q)
	        {
                allowModule.Add(item.fldModuleId);
	        }

            foreach (var item in q)
            {
                switch (item.fldModuleId)
                {
                    case 1:
                        {
                            desktopp.SetShortcutHidden("PublicSetting", false);
                        }
                        break;
                    case 2:
                        {
                            desktopp.SetShortcutHidden("Personeli", false);
                        }
                        break;
                    case 3:
                        {
                            desktopp.SetShortcutHidden("PayRoll", false);
                        }
                        break;
                    case 5:
                        {
                            desktopp.SetShortcutHidden("Daramad", false);
                        }
                        break;
                    case 4:
                        {
                            desktopp.SetShortcutHidden("_Hesabdari", false);
                        }
                        break;
                    case 6:
                        {
                            desktopp.SetShortcutHidden("create-Archive", false);
                        }
                        break;
                    case 7:
                        {
                            desktopp.SetShortcutHidden("_Budje", false);
                        }
                        break;
                    case 8:
                        {
                            desktopp.SetShortcutHidden("_FaniShahrsazi", false);
                        }
                        break;
                    case 9:
                        {
                            desktopp.SetShortcutHidden("_AnbarAmval", false);
                        }
                        break;
                    //case 10:
                    //    {
                    //        desktopp.SetShortcutHidden("_Amval", false);
                    //    }
                    //    break;
                    case 10:
                        {
                            desktopp.SetShortcutHidden("_Khazanedari", false);
                        }
                        break;
                    case 11:
                        {
                            desktopp.SetShortcutHidden("Create-Automation", false);
                        }
                        break;
                    case 12:
                        {
                            desktopp.SetShortcutHidden("_Chek", false);
                        }
                        break;
                    case 13:
                        {
                            desktopp.SetShortcutHidden("_OmmoorGharadad", false);
                        }
                        break;
                    case 14:
                        {
                            desktopp.SetShortcutHidden("_Motevafiat", false);
                        }
                        break;
                    case 15:
                        {
                            desktopp.SetShortcutHidden("_Kartabl", false);
                        }
                        break;
                    case 16:
                        {
                            desktopp.SetShortcutHidden("_Tasisat", false);
                        }
                        break;
                }
            }
            return this.Direct(allowModule);
        }
        public ActionResult SessionRemove()
        {
            Session.Remove("OrganId");

            return RedirectToAction("Index", "Home");
          //  return null;
        }
    }
}
