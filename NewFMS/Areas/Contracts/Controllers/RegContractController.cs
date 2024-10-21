using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using NewFMS.Areas.Contracts.Models;

namespace NewFMS.Areas.Contracts.Controllers
{
    public class RegContractController : Controller
    {
        //
        // GET: /Contracts/RegContract/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();

        WCF_Budget.BudgetService servicB = new WCF_Budget.BudgetService();
        WCF_Budget.ClsError ErrB = new WCF_Budget.ClsError();
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

        public ActionResult GetTypeGharardad()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblContractTypeSelect("", "",Convert.ToInt32(Session["OrganId"]), 0).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetProject(string StartDate)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicB.GetCodingBudje_DetailWithFilter("Project_Sarmayeii", StartDate.Substring(0,4), Session["OrganId"].ToString(), "", 0, IP, out ErrB).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult Save(Models.spr_tblContractsSelect reg)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Areas.Contracts.Models.ContractEntities m = new Areas.Contracts.Models.ContractEntities();
                if (reg.fldDesc == null)
                    reg.fldDesc = "";
                if (reg.fldId == 0)
                {
                    if (NewFMS.Models.Permission.haveAccess(1193, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        var q = m.spr_tblContractsSelect("fldShomare", reg.fldShomare.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "این شماره قرارداد قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblContractsInsert(reg.fldContractTypeId, reg.fldNaghshOrgan, reg.fldTarikh, reg.fldShomare, reg.fldSubject
                           , reg.fldTarikhEblagh, reg.fldShomareEblagh, reg.fldAshkhasId, reg.fldMablagh, reg.fldSuplyMaterialsType, reg.fldStartDate, reg.fldEndDate, reg.fldMandePardakhtNashode,
                           Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, reg.fldDesc,reg.fldBudjeCodingId_Detail);
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
                    if (NewFMS.Models.Permission.haveAccess(1194, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        var q = m.spr_tblContractsSelect("fldShomare", reg.fldShomare.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
                        if (q != null && q.fldId != reg.fldId)
                        {
                            return Json(new
                            {
                                Msg = "این شماره قرارداد قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblContractsUpdate(reg.fldId, reg.fldContractTypeId, reg.fldNaghshOrgan, reg.fldTarikh, reg.fldShomare, reg.fldSubject
                            , reg.fldTarikhEblagh, reg.fldShomareEblagh, reg.fldAshkhasId, reg.fldMablagh, reg.fldSuplyMaterialsType, reg.fldStartDate, reg.fldEndDate, reg.fldMandePardakhtNashode,
                            Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]), IP, reg.fldDesc, reg.fldBudjeCodingId_Detail);
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
                if (NewFMS.Models.Permission.haveAccess(1195, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    //حذف
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    m.spr_tblContractsDelete(id, Convert.ToInt32(Session["UserId"]));
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
            var q = m.spr_tblContractsSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]),1).FirstOrDefault();
            var NaghshOrgan="0";
            if(q.fldNaghshOrgan==true)
                NaghshOrgan="1";

            return Json(new
            {
                fldId = q.fldId,
                fldAshkhasId = q.fldAshkhasId,
                fldContractTypeId=q.fldContractTypeId.ToString(),
                fldEndDate=q.fldEndDate,
                fldMablagh=q.fldMablagh,
                fldMandePardakhtNashode=q.fldMandePardakhtNashode,
                fldNaghshName=q.fldNaghshName,
                fldNaghshOrgan = NaghshOrgan,
                fldNameTarfDovom=q.fldNameTarfDovom,
                fldOrganId=q.fldOrganId,
                fldShomare=q.fldShomare,
                fldShomareEblagh=q.fldShomareEblagh,
                fldStartDate=q.fldStartDate,
                fldSubject=q.fldSubject,
                fldSuplyMaterialsType=q.fldSuplyMaterialsType.ToString(),
                fldSuplyMaterialsType_Name=q.fldSuplyMaterialsType_Name,
                fldTarikh =q.fldTarikh,
                fldTarikhEblagh=q.fldTarikhEblagh,
                fldTypeTitle = q.fldTypeTitle,
                fldDesc = q.fldDesc,
                fldBudjeCodingId_Detail=q.fldBudjeCodingId_Detail.ToString()
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
                List<Models.spr_tblContractsSelect> data = null;
                if (filterHeaders.Conditions.Count > 0)
                {
                    string field = "";
                    string searchtext = "";
                    List<Models.spr_tblContractsSelect> data1 = null;
                    foreach (var item in filterHeaders.Conditions)
                    {
                        var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                        switch (item.FilterProperty.Name)
                        {
                            case "fldId":
                                searchtext = ConditionValue.Value.ToString();
                                field = "fldId";
                                break;
                            case "fldSubject":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldSubject";
                                break;
                            case "fldMablagh":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldMablagh";
                                break;
                            case "fldNameTarfDovom":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldNameTarfDovom";
                                break;
                            case "fldShomare":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldShomare";
                                break;
                            case "fldStartDate":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldStartDate";
                                break;
                            case "fldEndDate":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldEndDate";
                                break;
                            case "fldDesc":
                                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                field = "fldDesc";
                                break;
                        }
                        if (data != null)
                            data1 = m.spr_tblContractsSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                        else
                            data = m.spr_tblContractsSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                    }
                    if (data != null && data1 != null)
                        data.Intersect(data1);
                }
                else
                {
                    data = m.spr_tblContractsSelect("", "", Convert.ToInt32(Session["OrganId"]), 100).ToList();
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

                List<Models.spr_tblContractsSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
                //-- end paging ------------------------------------------------------------

                return this.Store(rangeData, data.Count);
            }
            else return null;
        }
        public ActionResult TamdidWin(string ContractId)
        {//باز شدن پنجره جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.ContractId = ContractId;
           
            return result;
        }
        public ActionResult GetTypeTmadid()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblTamdidTypesSelect("", "", 0).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult ReadTamdid(StoreRequestParameters parameters, int ContractId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            ContractEntities m = new ContractEntities();

            List<Models.spr_tblTamdidSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblTamdidSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTypeTamdidName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeTamdidName";
                            break;
                        case "fldTarikhPayan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhPayan";
                            break;
                        case "fldMablaghAfzayeshi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghAfzayeshi";
                            break;

                    }
                    if (data != null)
                        data1 = m.spr_tblTamdidSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0).Where(l => l.fldContractId == ContractId).ToList();
                    else
                        data = m.spr_tblTamdidSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0).Where(l => l.fldContractId == ContractId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblTamdidSelect("fldContractId", ContractId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).ToList();
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

            List<Models.spr_tblTamdidSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveTamdid(Models.spr_tblTamdidSelect reg)
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
                    if (NewFMS.Models.Permission.haveAccess(1197, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        var q = m.spr_tblTamdidSelect("fldTarikhPayan", reg.fldTarikhPayan.ToString(), Convert.ToInt32(Session["OrganId"]), 1).Where(l => l.fldContractId == reg.fldContractId).FirstOrDefault();
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "تمدید قرارداد به این تاریخ قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblTamdidInsert(reg.fldContractId, reg.fldTamdidTypeId, reg.fldTarikhPayan, reg.fldMablaghAfzayeshi, 
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
                    if (NewFMS.Models.Permission.haveAccess(1198, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        var q = m.spr_tblTamdidSelect("fldTarikhPayan", reg.fldTarikhPayan.ToString(), Convert.ToInt32(Session["OrganId"]), 1).Where(l => l.fldContractId == reg.fldContractId).FirstOrDefault();
                        if (q != null && q.fldId != reg.fldId)
                        {
                            return Json(new
                            {
                                Msg = "تمدید قرارداد به این تاریخ قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblTamdidUpdate(reg.fldId, reg.fldContractId, reg.fldTamdidTypeId, reg.fldTarikhPayan, reg.fldMablaghAfzayeshi,
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
        public ActionResult DeleteTamdid(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                ContractEntities m = new ContractEntities();
                if (NewFMS.Models.Permission.haveAccess(1199, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    //حذف
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    m.spr_tblTamdidDelete(id, Convert.ToInt32(Session["UserId"]));
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
        public ActionResult DetailsTamdid(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblTamdidSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
          

            return Json(new
            {
                fldId = q.fldId,
                fldContractId = q.fldContractId,
                fldTamdidTypeId = q.fldTamdidTypeId.ToString(),
                fldMablaghAfzayeshi = q.fldMablaghAfzayeshi,
                fldTarikhPayan = q.fldTarikhPayan,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TazaminWin(string ContractId)
        {//باز شدن پنجره جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.ContractId = ContractId;

            return result;
        }
        public ActionResult GetTypeTazmin()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblWarrantyTypeSelect("", "", 0).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldName });
            return this.Store(q);
        }
        public ActionResult ReadTazamin(StoreRequestParameters parameters, int ContractId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            ContractEntities m = new ContractEntities();

            List<Models.spr_tblTazaminSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblTazaminSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTypeTamdidName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeTamdidName";
                            break;
                        case "fldTarikhPayan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhPayan";
                            break;
                        case "fldMablaghAfzayeshi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghAfzayeshi";
                            break;

                    }
                    if (data != null)
                        data1 = m.spr_tblTazaminSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0).Where(l => l.fldContractId == ContractId).ToList();
                    else
                        data = m.spr_tblTazaminSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0).Where(l => l.fldContractId == ContractId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblTazaminSelect("fldContractId", ContractId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).ToList();
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

            List<Models.spr_tblTazaminSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveTazamin(Models.spr_tblTazaminSelect reg)
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
                    if (NewFMS.Models.Permission.haveAccess(1201, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        var q = m.spr_tblTazaminSelect("fldSepamNum", reg.fldSepamNum.ToString(), Convert.ToInt32(Session["OrganId"]), 1).Where(l => l.fldContractId == reg.fldContractId).FirstOrDefault();
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "ضامن با این شماره قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblTazaminInsert(reg.fldContractId, reg.fldWarrantyTypeId, reg.fldTypeTamdid, reg.fldSepamNum,reg.fldTarikh,reg.fldMablagh,
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
                    if (NewFMS.Models.Permission.haveAccess(1202, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        var q = m.spr_tblTazaminSelect("fldSepamNum", reg.fldSepamNum.ToString(), Convert.ToInt32(Session["OrganId"]), 1).Where(l => l.fldContractId == reg.fldContractId).FirstOrDefault();
                        if (q != null && q.fldId != reg.fldId)
                        {
                            return Json(new
                            {
                                Msg = "ضامن با این شماره قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblTazaminUpdate(reg.fldId, reg.fldContractId, reg.fldWarrantyTypeId, reg.fldTypeTamdid, reg.fldSepamNum, reg.fldTarikh, reg.fldMablagh,
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
        public ActionResult DeleteTazamin(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                ContractEntities m = new ContractEntities();
                if (NewFMS.Models.Permission.haveAccess(1203, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    //حذف
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    m.spr_tblTazaminDelete(id, Convert.ToInt32(Session["UserId"]));
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
        public ActionResult DetailsTazamin(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblTazaminSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();

            var tamdid = "1";
            if (q.fldTypeTamdid==false)
                tamdid = "0";

            return Json(new
            {
                fldId = q.fldId,
                fldContractId = q.fldContractId,
                fldTypeTamdid = tamdid,
                fldMablagh = q.fldMablagh,
                fldSepamNum = q.fldSepamNum,
                fldTarikh=q.fldTarikh,
                fldWarrantyTypeId=q.fldWarrantyTypeId.ToString(),
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmzaWin(string ContractId)
        {//باز شدن پنجره جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.ContractId = ContractId;

            return result;
        }
        public ActionResult ReadEmza(StoreRequestParameters parameters, int ContractId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            ContractEntities m = new ContractEntities();

            List<Models.spr_tblContractSignersSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblContractSignersSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTypeTamdidName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeTamdidName";
                            break;
                        case "fldTarikhPayan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhPayan";
                            break;
                        case "fldMablaghAfzayeshi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghAfzayeshi";
                            break;

                    }
                    if (data != null)
                        data1 = m.spr_tblContractSignersSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0).Where(l => l.fldContractId == ContractId).ToList();
                    else
                        data = m.spr_tblContractSignersSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0).Where(l => l.fldContractId == ContractId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblContractSignersSelect("fldContractId", ContractId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).ToList();
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

            List<Models.spr_tblContractSignersSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveSigners(Models.spr_tblContractSignersSelect reg)
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
                    if (NewFMS.Models.Permission.haveAccess(1213, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        var q = m.spr_tblContractSignersSelect("fldEmpolyId", reg.fldEmpolyId.ToString(), Convert.ToInt32(Session["OrganId"]), 1).Where(l => l.fldContractId == reg.fldContractId).FirstOrDefault();
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "این شخص برای این قرارداد قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblContractSignersInsert(reg.fldContractId, reg.fldCompanyPostId, reg.fldEmpolyId, reg.fldPostEjraeeId,
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
                    if (NewFMS.Models.Permission.haveAccess(1214, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        var q = m.spr_tblContractSignersSelect("fldEmpolyId", reg.fldEmpolyId.ToString(), Convert.ToInt32(Session["OrganId"]), 1).Where(l => l.fldContractId == reg.fldContractId).FirstOrDefault();
                        if (q != null && q.fldId != reg.fldId)
                        {
                            return Json(new
                            {
                                Msg = "این شخص برای این قرارداد قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblContractSignersUpdate(reg.fldId, reg.fldContractId, reg.fldCompanyPostId, reg.fldEmpolyId, reg.fldPostEjraeeId,
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
        public ActionResult DeleteSigners(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                ContractEntities m = new ContractEntities();
                if (NewFMS.Models.Permission.haveAccess(1215, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    //حذف
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    m.spr_tblContractSignersDelete(id, Convert.ToInt32(Session["UserId"]));
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
        public ActionResult DetailsSigners(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblContractSignersSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();

            var typeShakhs = "1";
            if (q.fldCompanyPostId == null)
                typeShakhs = "0";

            return Json(new
            {
                fldId = q.fldId,
                fldContractId = q.fldContractId,
                typeShakhs = typeShakhs,
                fldName_family = q.fldName_family,
                fldEmpolyId = q.fldEmpolyId,
                fldCompanyPostId = q.fldCompanyPostId,
                fldPostEjraeeId = q.fldPostEjraeeId,
                fldPost=q.fldPost
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
