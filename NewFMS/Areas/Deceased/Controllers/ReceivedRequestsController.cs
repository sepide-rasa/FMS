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
    public class ReceivedRequestsController : Controller
    {
        //
        // GET: /Deceased/ReceivedRequests/
        WCF_Deceased.DeceasedService service = new WCF_Deceased.DeceasedService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();

        WCF_Common.CommonService service_com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();

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
        public ActionResult NewStep(int RequestId,int kartablId,string KartablTitle)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.RequestId = RequestId;
            result.ViewBag.kartablId = kartablId.ToString();
            result.ViewBag.KartablTitle = KartablTitle;

            var HaveEtmam=false;
            var HaveEbtal=false;
            var q = service.GetKartablDetail(kartablId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            if (q.fldEtmam == true)
                HaveEtmam = true;
            if (q.fldEbtal == true)
                HaveEbtal = true;

            result.ViewBag.HaveEtmam = HaveEtmam;
            result.ViewBag.HaveEbtal = HaveEbtal;
            return result;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Deceased.OBJ_RequestAmanat> data = null;
            
                data = service.GetRequestAmanatWithFilter("NotExistsAmanat", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();

                return this.Store(data, data.Count);
        }
        public ActionResult Details(int RequestId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service.GetRequestAmanatDetail(RequestId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName+" "+q.fldFamily,
                fldFatherName=q.fldFatherName,
                fldMeli_Moshakhase=q.fldMeli_Moshakhase,
               fldSh_Shenasname= q.fldSh_Shenasname,
                fldNameVadiSalam=q.fldNameVadiSalam,
                fldNameGhete=q.fldNameGhete,
                fldNameRadif=q.fldNameRadif,
               fldShomare= q.fldShomare,
                fldTarikhRequest = q.fldTarikhRequest
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAction(int kartablId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service.GetAction_KartablWithFilter("fldKartablId", kartablId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldActionId, fldTitle = c.fldTitleAction });
            return this.Store(q);
        }
        public ActionResult SaveStep(WCF_Deceased.OBJ_Kartabl_Request req)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var s = service.GetNextKartablWithFilter("fldActionId", req.fldActionId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = service.InsertKartabl_Request(req.fldKartablId, req.fldActionId, req.fldRequestId,s.fldKartablNextId, req.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
               
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EndStep(int RequestId, string Desc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
               //ذخیره
                MsgTitle = "ذخیره موفق";
                var r = service.GetRequestAmanatDetail(RequestId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
                var q = service.GetGhabreAmanatWithFilter("fldEmployeeId", r.fldEmployeeId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                if (q != null)
                {
                    Msg = "برای یک شخص نمی توان بیش از یه قبر به امانت گرفت.";
                    MsgTitle = "اخطار";
                    Er = 1;
                }
                else
                {
                    var tarikh=service_com.GetDate(IP, out Err_com);
                    Msg = service.InsertGhabreAmanat(r.fldShomareId, r.fldEmployeeId, tarikh.fldTarikh, Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }

                //Msg = service.InsertKartabl_Request(req.fldKartablId, req.fldActionId, req.fldRequestId,s.fldKartablNextId, req.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
               
                //if (Err.ErrorType)
                //{
                //    MsgTitle = "خطا";
                //    Msg = Err.ErrorMsg;
                //    Er = 1;
                //}

                Msg = service.UpdateEtmamCharkhe(RequestId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NextKartabl(int ActionId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var s = service.GetNextKartablWithFilter("fldActionId", ActionId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            var NameKartabl = "";
            if (s != null)
                NameKartabl = s.fldTitleKartabl;
            return Json(new
            {
                NameKartabl = NameKartabl
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TimeLine(int RequestId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var End = false;

            List<WCF_Deceased.OBJ_TimeLine> q = new List<WCF_Deceased.OBJ_TimeLine>();

            q = service.SelectTimeLine(RequestId, IP,out Err).ToList();
            

                if (q.Count != 0)
                {
                    
                    Session["RequestId"] = RequestId;
                    PartialView.ViewBag.RequestId = RequestId;
                    PartialView.ViewBag.End = End;
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "تاکنون هیچ عملیاتی روی این درخواست انجام نگرفته است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            
            return PartialView;
        }
        public JsonResult TimeLineDetails(int RequestId)
        {//نمایش اطلاعات جهت رویت کاربر
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                List<WCF_Deceased.OBJ_TimeLine> q = new List<WCF_Deceased.OBJ_TimeLine>();
                q = service.SelectTimeLine(RequestId, IP, out Err).ToList();

                var tarikh = "";
                var TitleKartabl = "";
                var TitleAction = "";
                var Name_familyUser = "";

                int i = 0;
                foreach (var item in q)
                {
                    Name_familyUser = Name_familyUser + item.fldName_familyUser + ',';
                    TitleAction = TitleAction + item.fldTitleAction + ',';
                    TitleKartabl = TitleKartabl + item.fldTitleKartabl + ',';
                    tarikh = tarikh + item.fldTarikh + "_" + item.fldTime + ',';

                    i++;
                }
                return Json(new
                {
                    tarikh = tarikh,
                    Name_familyUser = Name_familyUser,
                    TitleAction = TitleAction,
                    TitleKartabl = TitleKartabl
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult EbtalReq(int RequestId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Msg = service.UpdateRequestForEbtal(RequestId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
