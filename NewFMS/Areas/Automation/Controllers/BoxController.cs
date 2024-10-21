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
    public class BoxController : Controller
    {
        //
        // GET: /Automation/Box/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
        public ActionResult Index()
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetCommision()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {

                var q = servic.GetCommisionWithFilter("UserKartabls", Session["UserId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldID, fldName = c.fldO_postEjraee_Title });
                return this.Store(q);
            }
        }
        public ActionResult Save(WCF_Automation.OBJ_Box Box)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Box.fldPID == 0)
                    Box.fldPID = null;
                if (Box.fldID == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertBox(Box.fldName,Box.fldComisionID,Box.fldBoxTypeID,Box.fldPID, Box.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateBox(Box.fldID, Box.fldName, Box.fldComisionID, Box.fldBoxTypeID, Box.fldPID, Box.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                var q1 = servic.GetBoxWithFilter("fldPID", id.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                if (q1.Count != 0)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = "مورد انتخاب شده دارای زیرشاخه میباشد لطفا ابتدا انها را حذف نمایید.",
                        MsgTitle = "خطا",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    MsgTitle = "حذف موفق";
                    servic.DeleteBox(id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                    Msg = "حذف با موفقیت انجام شد.";


                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetBoxDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldID = q.fldID,
                fldComisionID = q.fldComisionID.ToString(),
                fldDesc = q.fldDesc,
                fldName = q.fldName,
                fldPID = q.fldPID
            }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult NodeLoad(string nod, string ComisionID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetBoxWithFilter("fldComisionID", ComisionID,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldName;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.Cls = item.fldBoxTypeID.ToString();

                    var child = servic.GetBoxWithFilter("fldPID", item.fldID.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        childNode.Cls = ch.fldBoxTypeID.ToString();
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetBoxWithFilter("fldPID", nod,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.Cls = ch.fldBoxTypeID.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Store(nodes);
        }
        public ActionResult LetterBox(string LetterId, string MessageId, string fldCommisionId, string BoxType, string assigmentIds)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.LetterId = LetterId;
            result.ViewBag.MessageId = MessageId;
            result.ViewBag.CommisionId = fldCommisionId;
            result.ViewBag.BoxTypeId = BoxType;
            result.ViewBag.assigmentIds = assigmentIds;
            return result;
        }
        public ActionResult NodeLoadLetterBox(string nod, string ComisionID, string BoxTypeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetBoxWithFilter("fldComisionID_Type", ComisionID, BoxTypeId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldName;
                    asyncNode.NodeID = item.fldID.ToString();
                    asyncNode.Cls = item.fldBoxTypeID.ToString();

                    var child = servic.GetBoxWithFilter("fldPID", item.fldID.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldName;
                        childNode.NodeID = ch.fldID.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        childNode.Cls = ch.fldBoxTypeID.ToString();
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetBoxWithFilter("fldPID", nod,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.Cls = ch.fldBoxTypeID.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Store(nodes);
        }
        public ActionResult SaveLetterBox(WCF_Automation.OBJ_LetterBox LetterBox, string LetterId, string MessageId, string assigmentIds, int BoxTypeId, int CommisionId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "ذخیره موفق";
                Msg = "انتقال به پوشه دیگر با موفقیت انجام شد";
                //if (LetterId != "")//انتقال نامه های انتخاب شده
                //{
                   // LetterId = LetterId.Remove(LetterId.Length - 1);
                    string[] Letters = LetterId.Split(';');
                    string[] assigments = assigmentIds.Split(';');
                    string[] Messages = MessageId.Split(';');

                   
                        //LetterBox.fldLetterId = Convert.ToInt64(Letters[i]);
                        //LetterBox.fldMessageId = null;
                       // Msg = servic.InsertLetterBox(LetterBox.fldBoxId, LetterBox.fldLetterId, LetterBox.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (BoxTypeId == 1)
                        {
                            for (int i = 0; i < assigments.Count() - 1; i++)
                            {
                                var Assignment = servic.GetInternalAssignmentReceiverWithFilter("fldAssignmentID", assigments[i].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldReceiverComisionID == CommisionId).FirstOrDefault();
                                if (Assignment != null)
                                {
                                    servic.UpdateInternalAssignmentReceiverBox(Assignment.fldID, LetterBox.fldBoxId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                        }
                        else if (BoxTypeId == 2)
                        {
                            for (int i = 0; i < assigments.Count() - 1; i++)
                            {
                                var Assignment = servic.GetInternalAssignmentSenderWithFilter("fldAssignmentID", assigments[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldSenderComisionID == CommisionId).FirstOrDefault();
                                if (Assignment != null)
                                {
                                    servic.UpdateInternalAssignmentSenderBox(Assignment.fldId, LetterBox.fldBoxId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                }
                            }
                        }
                        else if (BoxTypeId == 3)
                        {
                            for (int i = 0; i < Letters.Count() - 1; i++)
                            {
                                var D = servic.GetLetterBoxWithFilter("fldLetterId", Letters[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(D.fldId, LetterBox.fldBoxId, D.fldLetterId, null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                        }
                        else if (BoxTypeId == 4)
                        {
                            for (int i = 0; i < Messages.Count() - 1; i++)
                            {
                                var D = servic.GetLetterBoxWithFilter("fldMessageId", Messages[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(D.fldId, LetterBox.fldBoxId, null, D.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                        }
                        else if (BoxTypeId == 5)
                        {
                            for (int i = 0; i < Letters.Count() - 1; i++)
                            {
                                var D = servic.GetLetterBoxWithFilter("fldMessageId", Messages[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(D.fldId, LetterBox.fldBoxId, null, D.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                            for (int i = 0; i < Messages.Count() - 1; i++)
                            {
                                var D = servic.GetLetterBoxWithFilter("fldMessageId", Messages[i], Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                servic.UpdateLetterBox(D.fldId, LetterBox.fldBoxId, null, D.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            }
                        }
                    
                //}
                //if (MessageId != "")//انتقال پیام های انتخاب شده
                //{
                //    MessageId = MessageId.Remove(MessageId.Length - 1);
                //    string[] Ids = MessageId.Split(';');
                //    foreach (var Letters in Ids)
                //    {
                //        LetterBox.fldLetterId = null;
                //        LetterBox.fldMessageId = Convert.ToInt32(Letters);
                //        Msg = servic.InsertLetterBox(LetterBox.fldBoxId, null, LetterBox.fldMessageId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                //    }
                //}

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
