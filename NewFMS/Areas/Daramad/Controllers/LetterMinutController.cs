using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class LetterMinutController : Controller
    {
        //
        // GET: /Daramad/LetterMinut/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();

        public ActionResult IndexWin(int id, int FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldId", id.ToString(),"",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.ShomareHesabCodeDaramadId = id.ToString();
            PartialView.ViewBag.DaramadTitle = q.fldDaramadTitle;/* +"(" + q.fldDaramadCode + ")";*/
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetParameter(string Type, string ShomareHesabCodeDaramadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetParametreSabetWithFilter("fldNoe", Type,ShomareHesabCodeDaramadId, 0, IP, out Err).ToList().Select(c => new { fldId = c.fldNameParametreEn, fldTitle = c.fldNameParametreFa });
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters, int ShomareHesabCodeDaramadId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_LetterMinut> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_LetterMinut> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldDescMinut":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDescMinut";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetLetterMinutWithFilter(field, searchtext, 0, IP, out Err).Where(k => k.fldShomareHesabCodeDaramadId == ShomareHesabCodeDaramadId).ToList();
                    else
                        data = servic.GetLetterMinutWithFilter(field, searchtext, 0, IP, out Err).Where(k => k.fldShomareHesabCodeDaramadId == ShomareHesabCodeDaramadId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetLetterMinutWithFilter("", "", 0, IP, out Err).Where(k => k.fldShomareHesabCodeDaramadId == ShomareHesabCodeDaramadId).ToList();
            }

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

            List<WCF_Daramad.OBJ_LetterMinut> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Save(WCF_Daramad.OBJ_LetterMinut Letter, List<WCF_Daramad.OBJ_LetterSigners> Posts)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = ""; var Er = 0;
            try
            {
                var Count1 = Letter.fldDescMinut.Count(l => l == '[');
                var Count2 = Letter.fldDescMinut.Count(l => l == ']');
                if (Count1 != Count2)
                {
                    return Json(new
                    {
                        Msg = "الگو نامه ثبت شده نامعتبر است.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                ////////چک کردن پارمترها
                var s = Letter.fldDescMinut.Split('[');
                var InvalidParam = "";
                for (byte i = 1; i < s.Length; i++)
                {
                    var z = s[i].Split(']');

                    var e = servic.GetParametreSabetWithFilter("fldNameParametreEn", z[0],Letter.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                    if (e == null)
                    {
                        if (!(z[0] == "ENumber" || z[0] == "EName" || z[0] == "EFamily" || z[0] == "EMeliCode" || z[0] == "EIdNum" || z[0] == "EPost" || z[0] == "ETel" || z[0] == "EAddress" || z[0] == "EFishNumber" || z[0] == "ECollectionDate" || z[0] == "EFishPrice" || z[0] == "EMaliatPrice" || z[0] == "EAvarezPrice"))
                            InvalidParam = InvalidParam + z[0] + ",";
                    }
                }
                if (InvalidParam != "")
                {
                    return Json(new
                       {
                           Msg = "پارامتر " + InvalidParam + " نامعتبر است.",
                           MsgTitle = "خطا",
                           Er = 1
                       }, JsonRequestBehavior.AllowGet);
                }
                //////////////////////

                if (Letter.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    int LetterId = servic.InsertLetterMinut(Letter.fldShomareHesabCodeDaramadId, Letter.fldTitle, Letter.fldDescMinut,Letter.fldSodoorBadAzVarizNaghdi,Letter.fldSodoorBadAzTaghsit,
                        Letter.fldTanzimkonande, "", Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    
                    if (Posts!=null)
                        foreach (var item in Posts)
                        {
                            servic.InsertLetterSigners(LetterId, item.fldOrganizationalPostsId, Session["Username"].ToString(), (Session["Password"].ToString()), "",Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                        }
                }
                else
                {
                    servic.DeleteLetterSigners(Letter.fldId, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateLetterMinut(Letter.fldId, Letter.fldShomareHesabCodeDaramadId, Letter.fldTitle, Letter.fldDescMinut, Letter.fldSodoorBadAzVarizNaghdi, Letter.fldSodoorBadAzTaghsit,
                        Letter.fldTanzimkonande, "", Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);

                    if (Posts != null)
                        foreach (var item in Posts)
                        {
                            servic.InsertLetterSigners(Letter.fldId, item.fldOrganizationalPostsId, Session["Username"].ToString(), (Session["Password"].ToString()), "",Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                        }
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
                MsgTitle = "حذف موفق";
                Msg=servic.DeleteLetterSigners(id, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                Msg = servic.DeleteLetterMinut(id, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Details(int Id)
        {
            var q = servic.GetLetterMinutDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldShomareHesabCodeDaramadId = q.fldShomareHesabCodeDaramadId,
                fldDescMinut = q.fldDescMinut,
                fldTitle = q.fldTitle,
                fldSodoorBadAzTaghsit=q.fldSodoorBadAzTaghsit,
                fldSodoorBadAzVarizNaghdi=q.fldSodoorBadAzVarizNaghdi,
                fldTanzimkonande=q.fldTanzimkonande,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NodeLoadPost(string nod, int OrganId, int LetterMinutId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();

            if (nod == "0")
            {
                var q = Cservic.GetOrganizationalPostsWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out CErr).ToList();
               
                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();
                    asyncNode.Checked = false;
                    var ff = servic.GetLetterSignersWithFilter("fldLetterMinutId", LetterMinutId.ToString(), 0,IP,out Err).ToList();
                    if (ff.Count() != 0)
                    {
                        foreach (var _item in ff)
                        {
                            if (_item.fldOrganizationalPostsId == item.fldId)
                            {
                                asyncNode.Checked = true;
                            }
                        }
                    }

                    var child = Cservic.GetOrganizationalPostsWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out CErr).ToList();
                 
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        childNode.Checked = false;
                        var f = servic.GetLetterSignersWithFilter("fldLetterMinutId", LetterMinutId.ToString(), 0, IP, out Err).ToList();
                        if (f.Count() != 0)
                        {
                            foreach (var _item in f)
                            {
                                if (_item.fldOrganizationalPostsId == ch.fldId)
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
                var child = Cservic.GetOrganizationalPostsWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out CErr).ToList();
               foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldTitle;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    childNode.Checked = false;
                    var ff = servic.GetLetterSignersWithFilter("fldLetterMinutId", LetterMinutId.ToString(), 0, IP, out Err).ToList();
                    if (ff.Count() != 0)
                    {
                        foreach (var _item in ff)
                        {
                            if (_item.fldOrganizationalPostsId == ch.fldId)
                            {
                                childNode.Checked = true;
                            }
                        }
                    }
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
    }
}
