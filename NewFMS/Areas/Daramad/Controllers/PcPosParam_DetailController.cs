using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class PcPosParam_DetailController : Controller
    {
        //
        // GET: /Daramad/PcPosParam_Detail/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
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
        public ActionResult GetPcPosInfo()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPcPosInfoWithFilter("", "", Session["OrganId"].ToString(), 0, IP, out ErrD).ToList().Select(c => new { fldID = c.fldId, fldOrganId = c.fldOrganId, fldName = c.fldPspName + "(" + c.fldOrganName + ")" }).OrderBy(x => x.fldName);
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters,int PcPosInfoId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_PcPosParametr> data = null;
            data = servicD.GetPcPos_Param_Value(PcPosInfoId, IP, out ErrD).ToList();

            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }
            List<WCF_Daramad.OBJ_PcPosParametr> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(List<WCF_Daramad.OBJ_PcPosParam_Dtail> PcPosParam_DetailArray, int PcPosInfoId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var q=servicD.GetPcPosParam_DetailWithFilter("fldPcPosInfoId", PcPosInfoId.ToString(), 0, IP, out ErrD).ToList();
                if (q.Count!=0)/*داده دارد*/
                {
                    if (PcPosParam_DetailArray == null)
                    {
                        MsgTitle = "عملیات موفق";
                        Msg = "عملیات با موفقیت انجام شد.";
                        servicD.DeletePcPosParam_Detail(PcPosInfoId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                        if (ErrD.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrD.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                                {
                                    Msg = Msg,
                                    MsgTitle = MsgTitle,
                                    Er = Er
                                }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    if (PcPosParam_DetailArray==null)
                    {
                        return Json(new
                        {
                            Msg = "لطفا مقدار پارامترها را وارد نمایید.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                MsgTitle = "عملیات موفق";
                Msg = "عملیات با موفقیت انجام شد.";
                servicD.DeletePcPosParam_Detail(PcPosInfoId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
                    Er = 1;
                }
                foreach (var item in PcPosParam_DetailArray)
	            {
                    servicD.InsertPcPosParam_Detail(item.fldPcPosParamId, PcPosInfoId, item.fldValue, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrD.ErrorMsg;
                        Er = 1;
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
