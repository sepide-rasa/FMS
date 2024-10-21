using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.Utilities;
using Ext.Net;
using NewFMS.Models;
using Ext.Net.MVC;

namespace NewFMS.Areas.Automation.Controllers
{
    public class LetterFileMojazController : Controller
    {
        //
        // GET: /Automation/LetterFileMojaz/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
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


        public ActionResult GetFormatFile()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetFormatFileWithFilter("", "", 0, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldPassvand });
            return this.Store(q);
        }

        public ActionResult Save(Models.spr_tblLetterFileMojazSelect LetterFileMojaz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {
                if (LetterFileMojaz.fldDesc == null)
                    LetterFileMojaz.fldDesc = "";
                if (LetterFileMojaz.fldId == 0)
                {
                    //ذخیره
                    if (Permission.haveAccess(1074, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"])))
                    {
                        m.spr_tblLetterFileMojazInsert(LetterFileMojaz.fldFormatFileId,LetterFileMojaz.fldType, Convert.ToInt32(Session["UserId"]), LetterFileMojaz.fldDesc, Convert.ToInt32(Session["OrganId"]), IP);
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Err = 1
                        });
                    }
                }
                else
                {
                    //ویرایش
                    if (Permission.haveAccess(1075, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"])))
                    {
                        m.spr_tblLetterFileMojazUpdate(LetterFileMojaz.fldId, LetterFileMojaz.fldFormatFileId, LetterFileMojaz.fldType, Convert.ToInt32(Session["UserId"]), LetterFileMojaz.fldDesc, Convert.ToInt32(Session["OrganId"]), IP);
                        Msg = "ویرایش با موفقیت انجام شد.";
                        MsgTitle = "ویرایش موفق";
                    }
                    else
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = "شما مجاز به دسترسی نمی باشید.",
                            Err = 1
                        });
                    }
                }

            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Err = 1
                });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {
                //حذف
                if (Permission.haveAccess(1076, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"])))
                {
                    m.spr_tblLetterFileMojazDelete(id, Convert.ToInt32(Session["UserId"]));
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                }
                else
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = "شما مجاز به دسترسی نمی باشید.",
                        Er = 1
                    });
                }
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
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
            Models.AutomationEntities m = new Models.AutomationEntities();
            var q = m.spr_tblLetterFileMojazSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldFormatFileId = q.fldFormatFileId.ToString(),
                fldType = q.fldType.ToString(),
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.AutomationEntities m = new Models.AutomationEntities();

            List<Models.spr_tblLetterFileMojazSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblLetterFileMojazSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeName";
                            break;
                        case "fldPassvand":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPassvand";
                            break;
                        case "fldSize":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSize";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = m.spr_tblLetterFileMojazSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                    else
                        data = m.spr_tblLetterFileMojazSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblLetterFileMojazSelect("", "", Convert.ToInt32(Session["OrganId"]), 100).ToList();
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

            List<Models.spr_tblLetterFileMojazSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        
    }
}
