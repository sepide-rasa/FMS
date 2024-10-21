using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Deceased.Controllers
{
    public class CodeDaramadAramestanController : Controller
    {
        //
        // GET: /Deceased/CodeDaramadAramestan/
        WCF_Deceased.DeceasedService servic = new WCF_Deceased.DeceasedService();
        WCF_Daramad.DaramadService servicDr = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();
        WCF_Daramad.ClsError ErrDr = new WCF_Daramad.ClsError();

        public ActionResult Index(string containerId)
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult LoadAllData()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var Data = servic.GetCodeDaramadAramestanWithFilter("ExistsInOrgan", (Session["OrganId"]).ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            var check = servic.GetCodeDaramadAramestanWithFilter("fldOrganid","", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            int[] checkId = new int[check.Count];

            for (int i = 0; i < check.Count; i++)
            {
                checkId[i] = check[i].fldCodeDaramadId;
            }
            return Json(new
            {
                Data = Data,
                checkId2 = checkId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(List<WCF_Deceased.OBJ_CodeDaramadAramestan> cods)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                var savedata = servic.GetCodeDaramadAramestanWithFilter("fldOrganid", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Select(c => new { fldId = c.fldId, fldCodeDaramadId = c.fldCodeDaramadId }).ToList();
                int[] SaveDataId = new int[savedata.Count];
                int[] codsInt = new int[cods.Count];

                for (int i = 0; i < savedata.Count; i++)
                    SaveDataId[i] = savedata[i].fldId;

                for (int i = 0; i < cods.Count; i++)
                    codsInt[i] = cods[i].fldId;
                
                var delData = SaveDataId.Except(codsInt);

               
               
                foreach (var item in cods)
                {
                    if (item.fldId == 0)
                    {
                        servic.InsertCodeDaramadAramestan(item.fldCodeDaramadId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        servic.UpdateCodeDaramadAramestan(item.fldId, item.fldCodeDaramadId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                Msg = "ذخیره با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";

                foreach (var item in delData)
                    servic.DeleteCodeDaramadAramestan(item, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا"
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
