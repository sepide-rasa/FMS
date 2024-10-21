using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class HoghoghMabnaController : Controller
    {
        //
        // GET: /Personeli/HoghoghMabna/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        public ActionResult IndexNew(string containerId)
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
        public ActionResult Reload(int Group, int Sal, Boolean Estekhdam)
        {//جستجو
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDetailHoghoghMabnaWithFilter("fldYear_Type", Sal.ToString(), Estekhdam, 0, IP, out Err).ToList();
            byte k = 0;
            List<Models.Mabna_G> groups = new List<Models.Mabna_G>();
            foreach (var Item in q)
            {

                Models.Mabna_G m = new Models.Mabna_G();
                m.fldGroh = Item.fldGroh;
                m.fldMablagh = Item.fldMablagh;
                groups.Add(m);
                k++;
            }
            for (byte i = k; i < Convert.ToInt32(Group); i++)
            {
                Models.Mabna_G m = new Models.Mabna_G();
                m.fldGroh = Convert.ToByte(i + 1);
                m.fldMablagh = 0;
                groups.Add(m);
            }
            return Json(groups, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadGrid(int Sal, Boolean Estekhdam)
        {//جستجو
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDetailHoghoghMabnaWithFilter("fldYear_Type", Sal.ToString(), Estekhdam, 0, IP, out Err).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(List<WCF_Personeli.OBJ_DetailHoghoghMabna> HoghVal, WCF_Personeli.OBJ_HoghoghMabna Hogh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var q2 = servic.GetDetailHoghoghMabnaWithFilter("fldYear_Type", Hogh.fldYear.ToString(), Hogh.fldType, 0, IP, out Err).FirstOrDefault();

                if (q2 == null)
                { //ذخیره
                    var HoghId = servic.InsertHoghoghMabna(Hogh.fldYear, Hogh.fldType, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    foreach (var item in HoghVal)
                    {
                        servic.InsertDetailHoghoghMabna(HoghId, item.fldGroh, item.fldMablagh, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                        Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش
                    var q = servic.GetHoghoghMabnaDetail(q2.fldHoghoghMabnaId, IP, out Err);
                    if (q != null)
                        servic.DeleteDetailHoghoghMabna("fldHoghoghMabnaId", q2.fldHoghoghMabnaId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    foreach (var item in HoghVal)
                    {
                        servic.InsertDetailHoghoghMabna(q2.fldHoghoghMabnaId, item.fldGroh, item.fldMablagh, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
