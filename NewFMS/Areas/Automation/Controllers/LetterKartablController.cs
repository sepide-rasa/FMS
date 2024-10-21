using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Automation.Controllers
{
    public class LetterKartablController : Controller
    {
        //
        // GET: /Automation/LetterKartabl/
        WCF_Common.CommonService Comservic = new WCF_Common.CommonService();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                ViewData.Model = new NewFMS.Models.LetterList();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                    ViewData = ViewData
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
            }
        }
        public ActionResult IndexBaygani(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                ViewData.Model = new NewFMS.Models.LetterList();
                var result = new Ext.Net.MVC.PartialViewResult
                {
                    WrapByScriptTag = true,
                    ContainerId = containerId,
                    RenderMode = RenderMode.AddTo,
                    ViewData = ViewData
                };
                this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
                return result;
            }
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetKartabl()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {

                string Msg = ""; string MsgTitle = ""; byte Er = 0;
                int[] KartablId = new int[1];
                string[] KartablName = new string[1];
                string[] KartablIcon = new string[1];
                string[] Active = new string[1];

                int[,] BoxId = new int[1,1];
                string[,] BoxName = new string[1,1];
                string[,] BoxIcon = new string[1,1];
                int[,] BoxType = new int[1,1];
                string[,] ChildId = new string[1,1];
                string[,] ChildName = new string[1,1];

                
               // List<Models.KartablAutoList> dataKartablAutoList = new List<Models.KartablAutoList>();

                try
                {
                    var q = servic.GetCommisionWithFilter("UserKartabls", Session["UserId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                    KartablId = new int[q.Count()];
                    KartablName = new string[q.Count()];
                    KartablIcon = new string[q.Count()];
                    Active = new string[q.Count()];

                    BoxId = new int[q.Count(), 5];
                    BoxName = new string[q.Count(), 5];
                    BoxIcon = new string[q.Count(), 5];
                    BoxType = new int[q.Count(),5];
                    ChildId = new string[q.Count(), 5];
                    ChildName = new string[q.Count(), 5];

                    var Icon = "";
                    for (int j = 0; j < q.Count(); j++)
                    {
                        KartablId[j] = q[j].fldID;
                        KartablName[j] = q[j].fldO_postEjraee_Title;
                        Active[j] = q[j].fldActive;
                        //if (q[j].fldFile != null)
                        //{
                        //    Icon = Convert.ToBase64String(q[j].fldFile);
                        //}
                        KartablIcon[j] = Icon;


                        /*var q2 = servic.GetBoxWithFilter("fldComisionID", q[j].fldID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();


                        List<Models.KartablAuto> dataKartablAuto = new List<Models.KartablAuto>();
                        Models.KartablAutoList ChList = new Models.KartablAutoList();

                        for (int i = 0; i < q2.Count(); i++)
                        {
                            var Iconb = "";


                            Models.KartablAuto Ch = new Models.KartablAuto();
                            
                           
                            Ch.BoxName= q2[i].tedad;

                            Ch.BoxId = q2[i].fldID.ToString();
                            Ch.BoxType = q2[i].fldBoxTypeID.ToString();
                            Ch.BoxIcon = Iconb;
                            Ch.ChildId = q2[i].chidId;
                            Ch.ChildName = q2[i].childName;

                                dataKartablAuto.Add(Ch);

                        }
                        ChList.ListKartabl = dataKartablAuto;
                        dataKartablAutoList.Add(ChList);*/
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
                    KartablId = KartablId,
                    KartablName = KartablName,
                    KartablIcon = KartablIcon,
                    Active=Active,
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er
                    //,
                    //dataKartablAutoList = dataKartablAutoList.ToList()
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBox(int CommisionId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                Models.AutomationEntities m = new Models.AutomationEntities();
                string Msg = ""; string MsgTitle = ""; byte Er = 0;
                int[] BoxId = new int[1];
                string[] BoxName = new string[1];
                string[] BoxIcon = new string[1];
                int[] BoxType = new int[1];
                string[] ChildId = new string[1];
                string[] ChildName = new string[1];


                try
                {
                    var q = servic.GetBoxWithFilter("fldComisionID", CommisionId.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                    BoxId = new int[q.Count()];
                    BoxName = new string[q.Count()];
                    BoxIcon = new string[q.Count()];
                    BoxType = new int[q.Count()];
                    ChildId = new string[q.Count()];
                    ChildName = new string[q.Count()];

                    for (int j = 0; j < q.Count(); j++)
                    {
                        var Icon = "";
                        //if (q[j].fldBoxTypeID == 1)
                        //{
                           // var q1 = m.spr_selectCountUnReadLetter(q[j].fldID, CommisionId, Convert.ToInt32(Session["OrganId"])).FirstOrDefault();
                        //    BoxName[j] = q[j].fldName + "(" + q[j].tedad + ")";
                        //}
                        //else
                            //BoxName[j] = q[j].fldName;

                        BoxName[j] = q[j].tedad;

                        BoxId[j] = q[j].fldID;
                        BoxType[j] = q[j].fldBoxTypeID;
                        BoxIcon[j] = Icon;
                        ChildId[j] = q[j].chidId;
                        ChildName[j] = q[j].childName;
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
                    BoxId = BoxId,
                    BoxName = BoxName,
                    BoxType=BoxType,
                    ChildId=ChildId,
                    ChildName=ChildName,
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetTabagheBandi(int CommisionId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                Models.AutomationEntities m = new Models.AutomationEntities();
                string Msg = ""; string MsgTitle = ""; byte Er = 0;
                int[] BoxId = new int[1];
                string[] BoxName = new string[1];
                string[] BoxIcon = new string[1];
                int[] BoxType = new int[1];
                string[] ChildId = new string[1];
                string[] ChildName = new string[1];


                try
                {
                    var q = servic.GetTabagheBandiWithFilter("fldComisionID", CommisionId.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                    BoxId = new int[q.Count()];
                    BoxName = new string[q.Count()];
                    BoxIcon = new string[q.Count()];
                    BoxType = new int[q.Count()];
                    ChildId = new string[q.Count()];
                    ChildName = new string[q.Count()];

                    for (int j = 0; j < q.Count(); j++)
                    {
                        var Icon = "";
                       

                        BoxName[j] = q[j].fldName;

                        BoxId[j] = q[j].fldID;
                       // BoxType[j] = q[j].fldBoxTypeID;
                        BoxIcon[j] = Icon;
                        ChildId[j] = q[j].chidId;
                        ChildName[j] = q[j].childName;
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
                    BoxId = BoxId,
                    BoxName = BoxName,
                    BoxType = BoxType,
                    ChildId = ChildId,
                    ChildName = ChildName,
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult NodeLoad(string nod, string CommId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();

            if (nod == null)
            {
                var q = servic.GetBoxWithFilter("fldComisionID", CommId,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldName;
                    asyncNode.NodeID = item.fldID.ToString();

                    var child = servic.GetBoxWithFilter("fldPID", item.fldID.ToString(),"",Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
            var child = servic.GetBoxWithFilter("fldPID", nod,"", Convert.ToInt32(Session["OrganId"]),0, IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
        public ActionResult ErjaWin(string fldLetterId, string fldMessageId, string commId, string SourceAssId, int state, int LetterTypeId)
        {//باز شدن پنجره جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
          
            result.ViewBag.fldLetterId = fldLetterId;
            result.ViewBag.fldMessageId = fldMessageId;
            result.ViewBag.commId = commId;
            result.ViewBag.SourceAssId = SourceAssId;
            result.ViewBag.state = state;
            result.ViewBag.LetterTypeId = LetterTypeId;

            var s = servic.GetCommisionWithFilter("fldId", commId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            result.ViewBag.CommName = s.fldName + "(" + s.fldO_postEjraee_Title + ")";

            //if (fldLetterId != "" && fldLetterId != "null" && fldLetterId != null)
            //{
            //    var k = servic.GetLetterWithFilter("fldId", fldLetterId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            //    result.ViewBag.Tarikh = k.fldLetterDate;
            //}
            //else if (fldMessageId != "" && fldMessageId != "null" && fldMessageId != null)
            //{
            //    var k = servic.GetMessageWithFilter("fldId", fldMessageId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            //    result.ViewBag.Tarikh = k.fldTarikhShamsi;
            //}
            result.ViewBag.Tarikh = MyLib.Shamsi.ShamsiAddDay(Comservic.GetDate(IP,out ComErr).fldTarikh,2);
            return result;
        }
        public ActionResult ErjaWinShow(string fldLetterId, string fldMessageId, string commId, string SourceAssId, int state, int LetterTypeId)
        {//باز شدن پنجره نمایش
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.fldLetterId = fldLetterId;
            result.ViewBag.fldMessageId = fldMessageId;
            result.ViewBag.commId = commId;
            result.ViewBag.SourceAssId = SourceAssId;
            result.ViewBag.state = state;
            result.ViewBag.LetterTypeId = LetterTypeId;

            var s = servic.GetCommisionWithFilter("fldId", commId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            result.ViewBag.CommName = s.fldName + "(" + s.fldO_postEjraee_Title + ")";

            //if (fldLetterId != "" && fldLetterId != "null" && fldLetterId != null)
            //{
            //    var k = servic.GetLetterWithFilter("fldId", fldLetterId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            //    result.ViewBag.Tarikh = k.fldLetterDate;
            //}
            //else if (fldMessageId != "" && fldMessageId != "null" && fldMessageId != null)
            //{
            //    var k = servic.GetMessageWithFilter("fldId", fldMessageId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            //    result.ViewBag.Tarikh = k.fldTarikhShamsi;
            //}
            result.ViewBag.Tarikh = MyLib.Shamsi.ShamsiAddDay(Comservic.GetDate(IP, out ComErr).fldTarikh, 2);
            return result;
        }
        public ActionResult GetAssType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAssignmentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(l => new { fldId = l.fldID, fldName = l.fldType });
                return this.Store(q);
        }
        public ActionResult SaveAssignment(List<WCF_Automation.OBJ_InternalAssignmentReceiver> recievers, WCF_Automation.OBJ_Assignment LetterAssignment, int fldSenderComisionID, string LetterID, string MessageId)
        {
            string Msg = "", MsgTitle = ""; var Er = 0; int state = 0;
            try
            {

                if (LetterAssignment.fldDesc == null)
                    LetterAssignment.fldDesc = "";

                var Fieldname = "fldLetterID";
                string value = LetterID;

                if (LetterID != "")//ارجاع نامه های انتخاب شده
                {

                    string[] Ids = value.Split(';');

                    foreach (var Letters in Ids)
                    {
                        if (Letters != "" && Letters != null && Letters != "0")
                        {
                            LetterAssignment.fldMessageId = null;
                            LetterAssignment.fldLetterID = Convert.ToInt64(Letters);


                            int idAssignment = 0;
                            var IsLetterID = servic.GetAssignmentWithFilter(Fieldname, Letters, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                            //sabte erja
                            if (IsLetterID != null)
                            {// در صورتیکه این نامه قبلا ارجاع داده شده است
                                int? ParentAssignmentID = LetterAssignment.fldSourceAssId;
                                if (ParentAssignmentID == 0)
                                    ParentAssignmentID = null;
                                idAssignment = servic.InsertAssignment(LetterAssignment.fldLetterID, LetterAssignment.fldMessageId, LetterAssignment.fldAnswerDate, ParentAssignmentID, LetterAssignment.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                                var BoxSendID = servic.GetBoxWithFilter("fldComisionID", fldSenderComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                                //ذخیره نامه در پوشه ارسال شده
                                servic.InsertInternalAssignmentSender(idAssignment, fldSenderComisionID, BoxSendID.fldID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            else
                            {// در صورتیکه برای اولین بار نامه ارجاع داده می شود
                                idAssignment = servic.InsertAssignment(LetterAssignment.fldLetterID, LetterAssignment.fldMessageId, LetterAssignment.fldAnswerDate, null, LetterAssignment.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                                //دریافت کد باکس جهت ذخیره در کارتابل ارسال شده
                                var BoxSendID = servic.GetBoxWithFilter("fldComisionID", fldSenderComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                                //ذخیره نامه در پوشه ارسال شده
                                // Fieldname = "fldLetterId";
                                // value = LetterAssignment.fldLetterID;
                                //if (LetterAssignment.fldLetterID == null)
                                //{
                                //    Fieldname = "fldMessageId";
                                //    value = LetterAssignment.fldMessageId;
                                //}
                                var LetterBox = servic.GetLetterBoxWithFilter(Fieldname, Letters, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(LetterBox.fldId, BoxSendID.fldID, LetterAssignment.fldLetterID, LetterAssignment.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                servic.InsertInternalAssignmentSender(idAssignment, fldSenderComisionID, BoxSendID.fldID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            //بررسی فیلد  تفویض جهت ارجاع نامه



                            foreach (var _item in recievers)
                            {
                                var BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", _item.fldReceiverComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();

                                servic.InsertInternalAssignmentReceiver(idAssignment, _item.fldReceiverComisionID, 1, _item.fldAssignmentTypeID, BoxCurrentID.fldID, null, true, _item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                var subStatiut = servic.GetSubstituteWithFilter("fldSenderComisionID", _item.fldReceiverComisionID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                                foreach (var item in subStatiut)
                                {
                                    BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", item.fldReceiverComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                                    servic.InsertInternalAssignmentReceiver(idAssignment, item.fldReceiverComisionID, 1, 1, BoxCurrentID.fldID, null, false, "تفویض شده", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }

                                //p.sp_tblInternalLetterReceiverInsert(LetterAssignment.fldLetterID, Convert.ToInt32(s[i]), 1, 1, "", "");
                            }
                        }
                    }
                }
                if (MessageId != "")//ارجاع پیام های انتخاب شده
                {
                    Fieldname = "fldMessageId";
                    value = MessageId;
                    string[] Ids = value.Split(';');

                    foreach (var Letters in Ids)
                    {
                        if (Letters != "" && Letters != null && Letters != "0")
                        {
                            LetterAssignment.fldMessageId = Convert.ToInt32(Letters);
                            LetterAssignment.fldLetterID = null;


                            int idAssignment = 0;
                            var IsLetterID = servic.GetAssignmentWithFilter(Fieldname, Letters, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                            //sabte erja
                            if (IsLetterID != null)
                            {// در صورتیکه این نامه قبلا ارجاع داده شده است
                                int? ParentAssignmentID = LetterAssignment.fldSourceAssId;
                                if (ParentAssignmentID == 0)
                                    ParentAssignmentID = null;
                                idAssignment = servic.InsertAssignment(LetterAssignment.fldLetterID, LetterAssignment.fldMessageId, LetterAssignment.fldAnswerDate, ParentAssignmentID, LetterAssignment.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                                var BoxSendID = servic.GetBoxWithFilter("fldComisionID", fldSenderComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                                //ذخیره نامه در پوشه ارسال شده
                                servic.InsertInternalAssignmentSender(idAssignment, fldSenderComisionID, BoxSendID.fldID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            else
                            {// در صورتیکه برای اولین بار نامه ارجاع داده می شود
                                idAssignment = servic.InsertAssignment(LetterAssignment.fldLetterID, LetterAssignment.fldMessageId, LetterAssignment.fldAnswerDate, null, LetterAssignment.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                                //دریافت کد باکس جهت ذخیره در کارتابل ارسال شده
                                var BoxSendID = servic.GetBoxWithFilter("fldComisionID", fldSenderComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 2).FirstOrDefault();
                                //ذخیره نامه در پوشه ارسال شده
                                // Fieldname = "fldLetterId";
                                // value = LetterAssignment.fldLetterID;
                                //if (LetterAssignment.fldLetterID == null)
                                //{
                                //    Fieldname = "fldMessageId";
                                //    value = LetterAssignment.fldMessageId;
                                //}
                                var LetterBox = servic.GetLetterBoxWithFilter(Fieldname, Letters, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(LetterBox.fldId, BoxSendID.fldID, LetterAssignment.fldLetterID, LetterAssignment.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                servic.InsertInternalAssignmentSender(idAssignment, fldSenderComisionID, BoxSendID.fldID, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            //بررسی فیلد  تفویض جهت ارجاع نامه



                            foreach (var _item in recievers)
                            {
                                var BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", _item.fldReceiverComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();

                                servic.InsertInternalAssignmentReceiver(idAssignment, _item.fldReceiverComisionID, 1, _item.fldAssignmentTypeID, BoxCurrentID.fldID, null, true, _item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                var subStatiut = servic.GetSubstituteWithFilter("fldSenderComisionID", _item.fldReceiverComisionID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                                foreach (var item in subStatiut)
                                {
                                    BoxCurrentID = servic.GetBoxWithFilter("fldComisionID", item.fldReceiverComisionID.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 1).FirstOrDefault();
                                    servic.InsertInternalAssignmentReceiver(idAssignment, item.fldReceiverComisionID, 1, 1, BoxCurrentID.fldID, null, false, "تفویض شده", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }

                                //p.sp_tblInternalLetterReceiverInsert(LetterAssignment.fldLetterID, Convert.ToInt32(s[i]), 1, 1, "", "");
                            }
                        }
                    }
                }


                Msg = "ارسال با موفقیت انجام شد.";
                Er = 0;
                MsgTitle = "عملیات موفق";

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
        public ActionResult Search(StoreRequestParameters parameters, int BoxId, string ComId,int BoxType,string SearchFilter)    
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            try
            {
                var end = Comservic.GetDate(IP, out ComErr).fldTarikh;
                var start = MyLib.Shamsi.ShamsiAddDay(end, -7);
                List<WCF_Automation.OBJ_Inbox> ListInbox = new List<WCF_Automation.OBJ_Inbox>();
                List<WCF_Automation.OBJ_Sent> ListSent = new List<WCF_Automation.OBJ_Sent>();
                List<WCF_Automation.OBJ_SavedLetter> ListSavedLetter = new List<WCF_Automation.OBJ_SavedLetter>();
                List<WCF_Automation.OBJ_SavedMessage> ListSavedMsg = new List<WCF_Automation.OBJ_SavedMessage>();
                List<WCF_Automation.OBJ_Trash> ListDeleted = new List<WCF_Automation.OBJ_Trash>();
              //  SearchFilter = SearchFilter.Replace('|', '''');

                if (BoxType == 1)
                    ListInbox = servic.SelectInbox("", start, end, BoxId,0, SearchFilter, Convert.ToInt32(Session["OrganId"]),50, IP, out Err).ToList();
                else if (BoxType == 2)
                    ListSent = servic.SelectSent("", start, end, BoxId, 0, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 3)
                    ListSavedLetter = servic.SelectSavedLetter("", start, end, BoxId.ToString(), 0, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 4)
                    ListSavedMsg = servic.GetSavedMessageWithFilter("", start, end, BoxId.ToString(), 0, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 5)
                    ListDeleted = servic.SelectTrash("", start, end, BoxId, 0, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();

                  
               return new JsonResult()
                {
                    Data = new
                    {
                        Er = 0,
                        ListInbox=ListInbox,
                        ListSent=ListSent,
                        ListSavedLetter=ListSavedLetter,
                        ListSavedMsg = ListSavedMsg,
                        ListDeleted = ListDeleted
                    },
                    MaxJsonLength = Int32.MaxValue,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception x)
            {
                var Msg = "";
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = Msg,
                    Er = 1
                });
            }
        }
        public JsonResult SaveDeleted(int BoxType, string SelectedLetterId,int CommId)
        {
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {
                if (SelectedLetterId != "")
                {
                    string[] letterAssId = SelectedLetterId.Split(';');
                    var TrashBox = servic.GetBoxWithFilter("fldComisionID", CommId.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 5).FirstOrDefault();
                    for (int i = 0; i < letterAssId.Count() - 1; i++)
                    {
                        if (BoxType == 1)//دریافتی
                            {
                                var Assignment = servic.GetInternalAssignmentReceiverWithFilter("fldAssignmentID", letterAssId[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldReceiverComisionID == CommId).FirstOrDefault();
                                if (Assignment != null)
                                {
                                    servic.UpdateInternalAssignmentReceiverBox(Assignment.fldID, TrashBox.fldID,  Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                        else if (BoxType == 2)//ارسالی
                            {
                                var Assignment = servic.GetInternalAssignmentSenderWithFilter("fldAssignmentID", letterAssId[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldSenderComisionID == CommId).FirstOrDefault();
                                if (Assignment != null)
                                {
                                    servic.UpdateInternalAssignmentSenderBox(Assignment.fldId, TrashBox.fldID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                        else if (BoxType == 3)//نامه ذخیره شده
                            {
                                var Assignment = servic.GetLetterWithFilter("fldId", letterAssId[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                if (Assignment != null)
                                {
                                    var BoxSendID = servic.GetBoxWithFilter("fldComisionID", Assignment.fldComisionID.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 5).FirstOrDefault();
                                    var LetterBox = servic.GetLetterBoxWithFilter("fldLetterID", letterAssId[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                    servic.UpdateLetterBox(LetterBox.fldId, BoxSendID.fldID, Convert.ToInt64(letterAssId[i]), null,"", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                        else if (BoxType == 4)//پیام ذخیره شده
                            {
                                var Assignment = servic.GetMessageWithFilter("fldId", letterAssId[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                if (Assignment != null)
                                {
                                    var BoxSendID = servic.GetBoxWithFilter("fldComisionID", Assignment.fldCommisionId.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldBoxTypeID == 5).FirstOrDefault();
                                    var LetterBox = servic.GetLetterBoxWithFilter("fldMessageId", letterAssId[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                    servic.UpdateLetterBox(LetterBox.fldId, BoxSendID.fldID, null,Convert.ToInt32(letterAssId[i]),  "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                        else if (BoxType == 5)//حذف شده ها
                        {
                            string[] ss = letterAssId[i].Split('|');
                            if (ss[1] == "1")//نامه
                            {
                                var sign = servic.GetSignerWithFilter("fldLetterId", (ss[0]).ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldFirstSigner != null).FirstOrDefault();

                                var si = servic.GetLetterWithFilter("fldID", (ss[0]).ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                var Ass = servic.GetAssignmentWithFilter("fldLetterID", (ss[0]).ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                if (sign != null)
                                    return Json(new
                                       {
                                           Msg = "نامه شماره " + si.fldOrderId.ToString() + "دارای امضا می باشد و قابل حذف شدن نیست.",
                                           MsgTitle = "خطا",
                                           Err = 1
                                       }, JsonRequestBehavior.AllowGet);
                                                            else if (Ass != null)
                                                                return Json(new
                                        {
                                            Msg = "نامه شماره " + si.fldOrderId.ToString() + "ارجاع شده و قابل حذف شدن نیست.",
                                            MsgTitle = "خطا",
                                            Err = 1
                                        }, JsonRequestBehavior.AllowGet);
                                else
                                {
                                    servic.DeleteRonevesht("fldLetterId", Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteLetterFollowLetterID(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteLetterBoxLetterID("Letter",Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                                    servic.DeleteLetterAttachmentLetterID(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteContentFile(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                  //  servic.DeleteLetterArchiveLetterID(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteInternalLetterReceiver(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteExternalLetterReceiver(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteExternalLetterSender(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteSigner(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                                    servic.DeleteAssignmentLetterID(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                    servic.DeleteLetter(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                            else//پیام
                            {
                                var Ass = servic.GetAssignmentWithFilter("fldMessageId", (ss[0]).ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                if (Ass != null)
                                    return Json(new
                                    {
                                        Msg = "پیام موردنظر ارجاع شده و قابل حذف شدن نیست.",
                                        MsgTitle = "خطا",
                                        Err = 1
                                    }, JsonRequestBehavior.AllowGet);
                                else
                                {
                                    servic.DeleteLetterBoxLetterID("Mesage", Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                                    servic.DeleteMessage(Convert.ToInt32(ss[0]), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                                }
                            }
                        }
                    }
                    Msg = "نامه(ها) با موفقیت حذف گردید.";
                    MsgTitle = "عملیات موفق";
                    Er = 0;
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
        public ActionResult Andicator(int LetterId, int CommisionId)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var comision = servic.GetCommisionWithFilter("fldid", CommisionId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                var organicRol = Comservic.GetOrganizationalPostsEjraeeWithFilter("fldid", comision.fldOrganizPostEjraeeID.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ComErr).FirstOrDefault();
                var Secretariat = servic.GetSecretariat_OrganizationUnitWithFilter("fldOrganizationUnitID", organicRol.fldChartOrganId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                if (Secretariat != null)
                {
                    var Letter = servic.GetLetterWithFilter("fldId", LetterId.ToString(),Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                    var ss = servic.GetSignerWithFilter("fldLetterId_HaveEmza", LetterId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Any();
                    if (ss==true/*Letter.fldLetterStatusID == 2*/)
                    {
                        if (Letter.fldLetterDate != null || Letter.fldLetterNumber != null)
                            return Json("نامه قبلا ثبت اندیکاتور شده است.", JsonRequestBehavior.AllowGet);
                        var SecretariatFormat = servic.GetNumberingFormatWithFilter("fldSecretariatID", Secretariat.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                        if (SecretariatFormat != null)
                        {
                            string LetterNumber = "";
                            var Format = SecretariatFormat.fldNumberFormat.Split('*');
                            for (int i = 0; i < Format.Count() - 1; i++)
                            {
                                switch (Format[i])
                                {
                                    case "shomaresabt":
                                        LetterNumber += Letter.fldOrderId;
                                        break;
                                    case "shomarande":
                                        var Number = servic.InsertLetterNumber(Letter.fldID, SecretariatFormat.fldStartNumber, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                        LetterNumber += Number;
                                        break;
                                    case "sal2":
                                        LetterNumber += Letter.fldYear.ToString().Substring(2, 2);
                                        break;
                                    case "sal4":
                                        LetterNumber += Letter.fldYear.ToString();
                                        break;
                                    case "shomarehokm":
                                        var commistion = servic.GetCommisionWithFilter("fldid", Letter.fldComisionID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();

                                        LetterNumber += commistion.fldOrganicNumber.ToString();
                                        break;
                                    default:
                                        LetterNumber += Format[i];
                                        break;
                                }
                            }
                            servic.UpdateLetterNumDate(Letter.fldID, LetterNumber, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]),IP,out Err);
                            servic.UpdateLetterStatusId(Letter.fldID, 3, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);

                            return Json(new
                            {
                                Msg = "نامه باموفقیت ثبت اندیکاتور شد. شماره نامه: " + LetterNumber,
                                MsgTitle = "عملیات موفق",
                                Err = 0
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new
                            {
                                Msg = "فرمت شماره گذاری مشخص نشده و شما نمی توانید آن را ثبت اندیکاتور نمایید.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new
                        {
                            Msg = "نامه امضا نشده و شما نمی توانید آن را ثبت اندیکاتور نمایید.",
                            MsgTitle = "خطا",
                            Err = 1
                        }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        Msg = "شما کاربر دبیرخانه نمی باشید و مجوز ثبت اندیکاتور ندارید.",
                        MsgTitle = "خطا",
                        Err = 1
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateReadFlag(int AssignmentID, int ComisionId)
        {
            try
            {
                Models.AutomationEntities p = new Models.AutomationEntities();

                var Assignment = servic.GetAssignmentWithFilter("fldId", AssignmentID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                if (Assignment != null)
                {
                    var AssStatus = servic.GetInternalAssignmentReceiverWithFilter("fldAssignmentID", Assignment.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(h => h.fldReceiverComisionID == ComisionId).FirstOrDefault();
                    if (AssStatus != null)
                        if (AssStatus.fldAssignmentStatusID == 1)
                        {
                            servic.UpdateInternalAssignmentReceiverStatus(AssStatus.fldID, 2, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]),IP,out Err);
                        }
                }
                return Json(new
                {
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
        }
        public ActionResult GetImmediacy()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetImmediacyWithFilter("fldOrganID", Session["OrganId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldID, Name = n.fldName });
            return this.Store(q);
        }
        public ActionResult GetSecurityType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSecurityTypeWithFilter("fldOrganID", Session["OrganId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { ID = n.fldID, Name = n.fldSecurityType });
            return this.Store(q);
        }
        public ActionResult ReadAssReciever(StoreRequestParameters parameters, string AssId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_InternalAssignmentReceiver> data = null;


            data = servic.GetInternalAssignmentReceiverWithFilter("fldAssignmentID", AssId, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
         

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Automation.OBJ_InternalAssignmentReceiver> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ShowBaygani(int ComId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.ComId = ComId;
            return result;
        }
        public ActionResult GetBoxType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {

                var q = servic.GetBoxTypeWithFilter("", "", 0, IP, out Err).ToList().Where(l=>l.fldId!=4 && l.fldId!=6).Select(c => new { fldId = c.fldId, fldName = c.fldName });
                return this.Store(q);
            }
        }
        public ActionResult SearchBaygani(StoreRequestParameters parameters, int TabagheId, string ComId, int BoxType, string SearchFilter)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            try
            {
                var end = Comservic.GetDate(IP, out ComErr).fldTarikh;
                var start = MyLib.Shamsi.ShamsiAddDay(end, -7);
                List<WCF_Automation.OBJ_Inbox> ListInbox = new List<WCF_Automation.OBJ_Inbox>();
                List<WCF_Automation.OBJ_Sent> ListSent = new List<WCF_Automation.OBJ_Sent>();
                List<WCF_Automation.OBJ_SavedLetter> ListSavedLetter = new List<WCF_Automation.OBJ_SavedLetter>();
                List<WCF_Automation.OBJ_SavedMessage> ListSavedMsg = new List<WCF_Automation.OBJ_SavedMessage>();
                List<WCF_Automation.OBJ_Trash> ListDeleted = new List<WCF_Automation.OBJ_Trash>();
                SearchFilter = SearchFilter.Replace('|', '"');

                if (BoxType == 1)
                    ListInbox = servic.SelectInbox("TabagheBandi", start, end,BoxType, TabagheId, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 2)
                    ListSent = servic.SelectSent("TabagheBandi", start, end, BoxType, TabagheId, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 3)
                    ListSavedLetter = servic.SelectSavedLetter("TabagheBandi", start, end, BoxType.ToString(), TabagheId, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 4)
                    ListSavedMsg = servic.GetSavedMessageWithFilter("TabagheBandi", start, end, BoxType.ToString(), TabagheId, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();
                else if (BoxType == 5)
                    ListDeleted = servic.SelectTrash("TabagheBandi", start, end, BoxType, TabagheId, SearchFilter, Convert.ToInt32(Session["OrganId"]), 50, IP, out Err).ToList();


                return new JsonResult()
                {
                    Data = new
                    {
                        Er = 0,
                        ListInbox = ListInbox,
                        ListSent = ListSent,
                        ListSavedLetter = ListSavedLetter,
                        ListSavedMsg = ListSavedMsg,
                        ListDeleted = ListDeleted
                    },
                    MaxJsonLength = Int32.MaxValue,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception x)
            {
                var Msg = "";
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = Msg,
                    Er = 1
                });
            }
        }
        
    }
}
