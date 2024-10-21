using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using NewFMS.Models;

namespace NewFMS.Areas.Automation.Controllers
{
    public class LetterActionTypeController : Controller
    {
        //
        // GET: /Automation/LetterActionType/
        WCF_Common.CommonService Comservic = new WCF_Common.CommonService();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
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
       
        public ActionResult New(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
    

        public ActionResult Save(Models.spr_tblLetterActionTypeSelect LetterActionType)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {
                if (LetterActionType.fldDesc == null)
                    LetterActionType.fldDesc = "";
                if (LetterActionType.fldId == 0)
                {
                    //ذخیره
                    if (Permission.haveAccess(1100, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"])))
                    {
                        m.spr_tblLetterActionTypeInsert(LetterActionType.fldTitleActionType, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), LetterActionType.fldDesc, IP);
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
                    if (Permission.haveAccess(1101, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"])))
                    {
                        m.spr_tblLetterActionTypeUpdate(LetterActionType.fldId, LetterActionType.fldTitleActionType, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), LetterActionType.fldDesc, IP);
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
                if (Permission.haveAccess(1102, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"])))
                {
                    m.spr_tblLetterActionTypeDelete(id, Convert.ToInt32(Session["UserId"]));
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
            var q = m.spr_tblLetterActionTypeSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]),1).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldTitleActionType = q.fldTitleActionType,
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

            List<Models.spr_tblLetterActionTypeSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblLetterActionTypeSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitleActionType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleActionType";
                            break;
                    }
                    if (data != null)
                        data1 = m.spr_tblLetterActionTypeSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                    else
                        data = m.spr_tblLetterActionTypeSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblLetterActionTypeSelect("", "", Convert.ToInt32(Session["OrganId"]), 100).ToList();
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

            List<Models.spr_tblLetterActionTypeSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveLetterActions(Models.spr_tblLetterActionsSelect LetterAction, string LetterIds)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {
                if (LetterAction.fldDesc == null)
                    LetterAction.fldDesc = "";
                //ذخیره
                LetterIds = LetterIds.Remove(LetterIds.Length - 1);
                var Ids = LetterIds.Split(';');
                foreach (var item in Ids)
                {
                    LetterAction.fldLetterId = Convert.ToInt64(item);
                    m.spr_tblLetterActionsInsert(LetterAction.fldLetterId,LetterAction.fldLetterActionTypeId, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), LetterAction.fldDesc, IP);
                   
                    if (LetterAction.fldLetterActionTypeId == 3)//خاتمه
                    {
                        var Assignment = servic.GetAssignmentWithFilter("fldLetterID", LetterAction.fldLetterId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                        var InternalAssignmentReceiver = servic.GetInternalAssignmentReceiverWithFilter("fldAssignmentID", Assignment.fldID.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                        if (InternalAssignmentReceiver != null)
                            if (InternalAssignmentReceiver.fldID != 0 && InternalAssignmentReceiver.fldID != null)
                            {
                                m.spr_UpdateInternalAssignmentReceiver(InternalAssignmentReceiver.fldID, 3);
                            }
                    }
                    if (LetterAction.fldLetterActionTypeId == 2)//ابطال
                    {
                        var cc=m.spr_DeleteErjaatLetter(LetterAction.fldLetterId, Convert.ToInt32(Session["UserId"])).FirstOrDefault();
                        if (cc.Error_Code != 0)
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = "خطایی با شماره: " + cc.Error_Code + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                                Err = 1
                            });
                        }
                        servic.UpdateLetterStatusId(LetterAction.fldLetterId, 5, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                    }
                }
                MsgTitle = "عملیات موفق";
                Msg = "عملیات با موفقیت انجام شد.";
                if(LetterAction.fldLetterActionTypeId==1)
                    Msg = "بایگانی نامه(ها) با موفقیت انجام شد.";
                else if (LetterAction.fldLetterActionTypeId == 2)
                    Msg = "ابطال نامه(ها) با موفقیت انجام شد.";
                else if (LetterAction.fldLetterActionTypeId == 3)
                    Msg = "خاتمه نامه(ها) با موفقیت انجام شد.";


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

    }
}
