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
    public class ReferralRulesController : Controller
    {
        //
        // GET: /Automation/ReferralRules/
        WCF_Common.CommonService Comservic = new WCF_Common.CommonService();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

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
        public ActionResult GetPosts()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            var q = Comservic.GetOrganizationalPostsEjraeeWithFilter("fldOrganId", Session["OrganId"].ToString(), 0,Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            var q = Comservic.GetOrganizationWithFilter("", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).ToList().Select(n => new { ID = n.fldId, Name = n.fldName });
            return this.Store(q);
        }
        
        public ActionResult NodeLoad(string nod, string fldPostErjaDahandeId,int? OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
         

            if (nod == "0")
            {
                var q = Comservic.GetChartOrganEjraeeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).Where(l => l.fldOrganId == OrganId).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();
                    asyncNode.Icon = Ext.Net.Icon.Building;
                    asyncNode.Checked = false;
                    var f1 = servic.GetReferralRulesWithFilter("fldPostErjaDahandeId", fldPostErjaDahandeId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    if (f1.Count() != 0)
                    {
                        foreach (var _item in f1)
                        {
                            if (_item.fldChartEjraeeGirandeId == item.fldId)
                            {
                                asyncNode.Checked = true;
                            }
                        }
                    }

                    var child = Comservic.GetChartOrganEjraeeWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).Where(l => l.fldOrganId == OrganId).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        childNode.Checked = false;
                        var f = servic.GetReferralRulesWithFilter("fldPostErjaDahandeId", fldPostErjaDahandeId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        if (f.Count() != 0)
                        {
                            foreach (var _item in f)
                            {
                                if (_item.fldPostErjaGirandeId == ch.fldId)
                                {
                                    childNode.Checked = true;
                                }
                            }
                        }
                        asyncNode.Children.Add(childNode);
                    }

                    var childp = Comservic.GetOrganizationalPostsEjraeeWithFilter("fldChartOrganId", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).ToList();

                    foreach (var ch in childp)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = "P" + ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.User;
                        childNode.Checked = false;
                        var f = servic.GetReferralRulesWithFilter("fldPostErjaDahandeId", fldPostErjaDahandeId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        if (f.Count() != 0)
                        {
                            foreach (var _item in f)
                            {
                                if (_item.fldPostErjaGirandeId == ch.fldId)
                                {
                                    childNode.Checked = true;
                                }
                            }
                        }
                        asyncNode.Children.Add(childNode);

                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                if (nod.Substring(0, 1) == "P")
                {
                    nod = nod.Substring(1, nod.Length - 1);
                    var childp = Comservic.GetOrganizationalPostsEjraeeWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).Where(l => l.fldOrganId == OrganId).ToList();

                    foreach (var ch in childp)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = "P" + ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.User;
                        childNode.Checked = false;
                        var f = servic.GetReferralRulesWithFilter("fldPostErjaDahandeId", fldPostErjaDahandeId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        if (f.Count() != 0)
                        {
                            foreach (var _item in f)
                            {
                                if (_item.fldPostErjaGirandeId == ch.fldId)
                                {
                                    childNode.Checked = true;
                                }
                            }
                        }
                        nodes.Add(childNode);

                    }
                }
                else
                {
                    var child = Comservic.GetChartOrganEjraeeWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).Where(l => l.fldOrganId == OrganId).ToList();

                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        childNode.Checked = false;
                        var f = servic.GetReferralRulesWithFilter("fldPostErjaDahandeId", fldPostErjaDahandeId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        if (f.Count() != 0)
                        {
                            foreach (var _item in f)
                            {
                                if (_item.fldChartEjraeeGirandeId == ch.fldId)
                                {
                                    childNode.Checked = true;
                                }
                            }
                        }
                        nodes.Add(childNode);

                    }

                    var childp = Comservic.GetOrganizationalPostsEjraeeWithFilter("fldChartOrganId", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ComErr).ToList();

                    foreach (var ch in childp)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = "P" + ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.User;
                        childNode.Checked = false;
                        var f = servic.GetReferralRulesWithFilter("fldPostErjaDahandeId", fldPostErjaDahandeId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        if (f.Count() != 0)
                        {
                            foreach (var _item in f)
                            {
                                if (_item.fldPostErjaGirandeId == ch.fldId)
                                {
                                    childNode.Checked = true;
                                }
                            }
                        }
                        nodes.Add(childNode);

                    }
                }

            }
            return this.Direct(nodes);
        }
        public ActionResult SaveReferralRules(List<WCF_Automation.OBJ_ReferralRules> Rules, int PostErjaDahandeId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";
            try
            {

                servic.DeleteReferralRules(PostErjaDahandeId, OrganId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType == true)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                    }




                    if (Rules != null)
                {
                    foreach (var item in Rules)
                    {
                        Msg = servic.InsertReferralRules(item.fldPostErjaDahandeId, item.fldPostErjaGirandeId, item.fldChartEjraeeGirandeId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType == true)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
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
                //}
                //else
                //{
                //    return null;
                //}
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
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
