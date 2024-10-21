using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Automation
{
    public class SendToSecretariatController : Controller
    {
        //
        // GET: /Automation/SendToSecretariat/
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        public ActionResult Index(int LetterID, string SourceAssId, int CurrentComId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.LetterID = LetterID;
            result.ViewBag.SourceAssId = SourceAssId;
            result.ViewBag.CurrentComId = CurrentComId;
            return result;
        }
        public ActionResult GetSecretariat()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSecretariat_OrganizationUnitWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldSecretariatID, Name = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult NodeLoadSendToSecretariat(string nod, string SecretariatID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            Models.AutomationEntities m = new Models.AutomationEntities();
            var child = m.spr_SendToSecretariat("fldId", SecretariatID, 0).ToList();

            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldStaffName;
                childNode.NodeID = ch.CommisionId.ToString();
                childNode.Icon = Ext.Net.Icon.Building;
                childNode.Checked = true;
                
               
                nodes.Add(childNode);

            }


            return this.Store(nodes);
        }
        public ActionResult Send(string secretriant, long LetterId, string SourceId, int SenderComId, List<Models.spr_SendToSecretariat> checkedNodes)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                Models.AutomationEntities p = new Models.AutomationEntities();
                var IdAssignment = 0;
                if (checkedNodes != null)
                {

                    var date = servicCommon.GetDate(IP, out ErrCommon);

                    //دریافت کد باکس جهت ذخیره در کارتابل ارسال شده
                    var BoxSendID = servic.GetBoxWithFilter("fldComisionID", SenderComId.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                    if (Convert.ToInt32(SourceId) == 0)
                    {
                        IdAssignment = servic.InsertAssignment(LetterId, null, date.fldTarikh, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                        var LetterBox = servic.GetLetterBoxWithFilter("fldLetterID", LetterId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                        servic.UpdateLetterBox(LetterBox.fldId, BoxSendID.fldID, LetterId, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        IdAssignment = servic.InsertAssignment(LetterId, null, date.fldTarikh, Convert.ToInt32(SourceId), "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }



                    servic.InsertInternalAssignmentSender(IdAssignment, SenderComId, BoxSendID.fldID, "ارسال به دبیرخانه", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    for (int i = 0; i < checkedNodes.Count(); i++)
                    {
                        var BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", checkedNodes[i].CommisionId.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                        servic.InsertInternalAssignmentReceiver(IdAssignment, checkedNodes[i].CommisionId,1,3, BoxCurrentID.fldID,null,true, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    MsgTitle = "عملیات موفق";
                    Msg = "ارسال با موفقیت انجام شد.";
                }
                return Json(new
                {
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er
                }, JsonRequestBehavior.AllowGet);

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
