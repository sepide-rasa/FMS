using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using NewFMS.Areas.Contracts.Models;

namespace NewFMS.Areas.Contracts.Controllers
{
    public class ContractTypeController : Controller
    {
        // GET: Contracts/ContractType
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
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
        public ActionResult Save(Models.spr_tblContractTypeSelect reg)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                ContractEntities m = new ContractEntities();
                if (reg.fldDesc == null)
                    reg.fldDesc = "";
                if (reg.fldId == 0)
                {
                    if (NewFMS.Models.Permission.haveAccess(1205, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        var q = m.spr_tblContractTypeSelect("fldTitle", reg.fldTitle.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "این نوع قرارداد قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblContractTypeInsert(reg.fldTitle, reg.fldDarsadBimePeymankar, reg.fldDarsadBimeKarfarma, reg.fldDarsadAnjamKar, reg.fldDarsadZemanatName,
                           Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, reg.fldDesc);
                    }
                    else
                    {
                        Er = 1;
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                    }
                }
                else
                {
                    if (NewFMS.Models.Permission.haveAccess(1206, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        var q = m.spr_tblContractTypeSelect("fldTitle", reg.fldTitle.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
                        if (q != null && q.fldId != reg.fldId)
                        {
                            return Json(new
                            {
                                Msg = "این نوع قرارداد قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblContractTypeUpdate(reg.fldId, reg.fldTitle, reg.fldDarsadBimePeymankar, reg.fldDarsadBimeKarfarma, reg.fldDarsadAnjamKar, reg.fldDarsadZemanatName,
                            Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, reg.fldDesc);
                    }
                    else
                    {
                        Er = 1;
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                    }
                }

            }
            catch (Exception x)
            {
                NewFMS.Models.AutomationEntities m = new NewFMS.Models.AutomationEntities();
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

            try
            {
                ContractEntities m = new ContractEntities();
                if (NewFMS.Models.Permission.haveAccess(1207, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    //حذف
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    m.spr_tblContractTypeDelete(id, Convert.ToInt32(Session["UserId"]));
                }
                else
                {
                    Er = 1;
                    MsgTitle = "خطا";
                    Msg = "شما مجاز به دسترسی نمی باشید.";
                }

            }
            catch (Exception x)
            {
                NewFMS.Models.AutomationEntities m = new NewFMS.Models.AutomationEntities();
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblContractTypeSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
           

            return Json(new
            {
                fldId = q.fldId,
                fldDarsadAnjamKar = q.fldDarsadAnjamKar,
                fldDarsadBimeKarfarma=q.fldDarsadBimeKarfarma,
                fldDarsadBimePeymankar=q.fldDarsadBimePeymankar,
               fldDarsadZemanatName= q.fldDarsadZemanatName,
                fldTitle=q.fldTitle,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (NewFMS.Models.Permission.haveAccess(1192, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
            {

                var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

                ContractEntities m = new ContractEntities();
                List<Models.spr_tblContractTypeSelect> data = null;
                if (filterHeaders.Conditions.Count > 0)
                {
                    string field = "";
                    string searchtext = "";
                    List<Models.spr_tblContractTypeSelect> data1 = null;
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
                            case "fldDarsadAnjamKar":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldDarsadAnjamKar";
                                break;
                            case "fldDarsadBimeKarfarma":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldDarsadBimeKarfarma";
                                break;
                            case "fldDarsadBimePeymankar":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldDarsadBimePeymankar";
                                break;
                            case "fldDarsadZemanatName":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldDarsadZemanatName";
                                break;
                            case "fldDesc":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldDesc";
                                break;
                        }
                        if (data != null)
                            data1 = m.spr_tblContractTypeSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                        else
                            data = m.spr_tblContractTypeSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                    }
                    if (data != null && data1 != null)
                        data.Intersect(data1);
                }
                else
                {
                    data = m.spr_tblContractTypeSelect("", "", Convert.ToInt32(Session["OrganId"]), 100).ToList();
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

                List<Models.spr_tblContractTypeSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
                //-- end paging ------------------------------------------------------------

                return this.Store(rangeData, data.Count);
            }
            else return null;
        }
    }
}