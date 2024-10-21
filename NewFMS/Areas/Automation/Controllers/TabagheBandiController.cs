using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net; 
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Automation.Controllers
{
    public class TabagheBandiController : Controller
    {
        //
        // GET: /Automation/TabagheBandi/

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
        public ActionResult Save(WCF_Automation.OBJ_TabagheBandi TabagheBandi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (TabagheBandi.fldID == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertTabagheBandi(TabagheBandi.fldName, TabagheBandi.fldComisionID, TabagheBandi.fldPID, TabagheBandi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateTabagheBandi(TabagheBandi.fldID, TabagheBandi.fldName, TabagheBandi.fldComisionID, TabagheBandi.fldPID, TabagheBandi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                var q1 = servic.GetTabagheBandiWithFilter("fldPID", id.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
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
                    servic.DeleteTabagheBandi(id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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
            var q = servic.GetTabagheBandiDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                var q = servic.GetTabagheBandiWithFilter("fldComisionID", ComisionID,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldName;
                    asyncNode.NodeID = item.fldID.ToString();

                    var child = servic.GetTabagheBandiWithFilter("fldPID", item.fldID.ToString(),"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
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
                var child = servic.GetTabagheBandiWithFilter("fldPID", nod,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Store(nodes);
        }
        public ActionResult LetterTabagheBandi(string fldLetterId, string fldMessageId, string fldCommisionId)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.LetterId = fldLetterId;
            result.ViewBag.MessageId = fldMessageId;
            result.ViewBag.CommisionId = fldCommisionId;
            
            return result;
        }
        public ActionResult NodeLoadLetterTabagheBandi(string nod, string ComisionID, string fldLetterId, string fldMessageId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            
                var child = servic.GetTabagheBandiWithFilter("fldPID", nod,ComisionID, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldID.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    childNode.Checked = false;
                    if (fldLetterId != "")
                    {
                        fldLetterId = fldLetterId.Remove(fldLetterId.Length - 1);
                        var Ids = fldLetterId.Split(';');
                        foreach (var letter in Ids)
                        {
                            var f = servic.GetLetterTabagheBandiWithFilter("fldLetterId", letter.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                            if (f.Count() != 0)
                            {
                                foreach (var _item in f)
                                {
                                    if (_item.fldTabagheBandiId == ch.fldID)
                                    {
                                        childNode.Checked = true;
                                    }
                                }
                            }
                        }
                        
                    }
                    if (fldMessageId != "")
                    {
                        fldMessageId = fldMessageId.Remove(fldMessageId.Length - 1);
                        var Ids = fldMessageId.Split(';');
                        foreach (var mess in Ids)
                        {
                            var f = servic.GetLetterTabagheBandiWithFilter("fldMessageId", mess, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                            if (f.Count() != 0)
                            {
                                foreach (var _item in f)
                                {
                                    if (_item.fldTabagheBandiId == ch.fldID)
                                    {
                                        childNode.Checked = true;
                                    }
                                }
                            }
                        }
                        
                    }
                    nodes.Add(childNode);

                }

            
            return this.Store(nodes);
        }
        public ActionResult SaveLetterTabagheBandi(List<WCF_Automation.OBJ_LetterTabagheBandi> TabagheBandi, string fldLetterId, string fldMessageId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {
                
                //ذخیره
              
                if (TabagheBandi != null)
                {
                    if (fldLetterId != "")//طبقه بندی نامه های انتخاب شده
                    {
                        fldLetterId = fldLetterId.Remove(fldLetterId.Length - 1);
                        var Ids = fldLetterId.Split(';');
                        foreach (var letter in Ids)
                        {
                            var tabagheId = "";
                            servic.DeleteLetterTabagheBandi("LettreId", Convert.ToInt64(letter), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                            if (Err.ErrorType == true)
                            {
                                MsgTitle = "خطا";
                                Msg = Err.ErrorMsg;
                                Er = 1;
                            }
                            foreach (var item in TabagheBandi)
                            {
                                Msg = servic.InsertLetterTabagheBandi(item.fldTabagheBandiId, Convert.ToInt64(letter), null, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                tabagheId = tabagheId + item.fldTabagheBandiId + ",";
                                if (Err.ErrorType)
                                {
                                    MsgTitle = "خطا";
                                    Msg = Err.ErrorMsg;
                                    Er = 1;
                                }
                            }
                            m.spr_tblLetterActionsInsert(Convert.ToInt64(letter), 1, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), "بایگانی در طبقه بندی:" + tabagheId, IP);
                        }
                    }

                    if (fldMessageId != "")//طبقه بندی پیام های انتخاب شده
                    {
                        fldMessageId = fldMessageId.Remove(fldMessageId.Length - 1);
                        var Ids = fldMessageId.Split(';');
                        foreach (var mess in Ids)
                        {
                            servic.DeleteLetterTabagheBandi("MessageId", Convert.ToInt32(mess), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                            if (Err.ErrorType == true)
                            {
                                MsgTitle = "خطا";
                                Msg = Err.ErrorMsg;
                                Er = 1;
                            }
                            foreach (var item in TabagheBandi)
                            {
                                Msg = servic.InsertLetterTabagheBandi(item.fldTabagheBandiId, null, Convert.ToInt32(mess), "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                if (Err.ErrorType)
                                {
                                    MsgTitle = "خطا";
                                    Msg = Err.ErrorMsg;
                                    Er = 1;
                                }
                            }
                        }
                    }
                    
                }
                

                MsgTitle = "ذخیره موفق";
                Msg = "ذخیره با موفقیت انجام شد.";
                if (Err.ErrorType == true)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
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
