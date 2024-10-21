using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;

namespace NewFMS.Areas.Budget.Controllers
{
    public class CodingController : Controller
    {
        // GET: Budget/Coding
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Budget.BudgetService servic = new WCF_Budget.BudgetService();
        WCF_Accounting.AccountingService servic_acc = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err_acc = new WCF_Accounting.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
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
        public ActionResult Coding(int HeaderId, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult SelectTemplate(int HeaderId, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.Year = Year.ToString();
            return result;
        }
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var q = servic_acc.GetFiscalYearWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_acc).ToList().Select(c => new { fldId = c.fldYear, fldYear = c.fldYear });
            //return this.Store(q);
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = servic_com.GetDate(IP, out Err_com);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4));
            for (int i = 1394; i < fldsal + 2; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();
                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh.OrderByDescending(l => l.fldYear), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTypeEtebar()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetEtebarTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l=>l.fldId!=1).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetTypeMasraf()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMasrafTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_com.GetOrganizationWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult SearchOperator(byte Type, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Type = Type.ToString();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult ReadOperator(StoreRequestParameters parameters, byte Type, short Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> data = null;
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                    }
                    if (data != null)
                        data1 = servic_acc.GetParvandeInfo(field, searchtext, Type,Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err_acc).ToList();
                    else
                        data = servic_acc.GetParvandeInfo(field, searchtext, Type, Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err_acc).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic_acc.GetParvandeInfo("", "", Type, Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err_acc).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetTemplateCoding(int TemplateNameId, string PCode)
        {
            var q = servic.GetTemplatCodingWithFilter("PCod", PCode, TemplateNameId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(l => new { fldTitle = l.fldName, fldId = l.fldId});
            return this.Store(q);
        }
        public ActionResult GetTemplate()
        {
            var q = servic.GetTemplatNameWithFilter("","", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }

        public ActionResult SaveHeader(WCF_Budget.OBJ_CodingBudje_Header Coding_Header)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int HeaderId = 0;
            try
            {
                if (Coding_Header.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد. آیا مایل هستید کدینگ بودجه از سال قبل کپی شود؟";
                    HeaderId = servic.InsertCodingBudje_Header(Coding_Header.fldYear, Coding_Header.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCodingBudje_Header(Coding_Header.fldId,Coding_Header.fldYear, Coding_Header.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    HeaderId = 0;
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
                Er = Er,
                HeaderId = HeaderId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CopyDetails(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "عملیات موفق";
                Msg = servic.CopyCodingDetail(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCodingBudje_Header(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult DeleteNode(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCodingBudje_Detail(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCodingBudje_HeaderDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldYear = q.fldYear.ToString(),
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsNode(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCodingBudje_DetailDetail(Id, IP, out Err);
            return Json(new
            {
                fldCode = q.fldCode,
                fldTitle = q.fldTitle,
                fldBudCode = q.fldBudCode,
                fldEtebarTypeId = q.fldEtebarTypeId.ToString(),
                fldEtebarType = q.fldEtebarType,
                fldHeaderId = q.fldHeaderId,
                fldLevelId=q.fldLevelId,
                fldMasrafTTitle=q.fldMasrafTTitle,
                fldMasrafTypeId = q.fldMasrafTypeId.ToString(),
                fldOldId=q.fldOldId,
                fldStrhid=q.fldStrhid,
                fldTarh_KhedmatTypeId = q.fldTarh_KhedmatTypeId.ToString(),
                fldYear=q.fldYear,
                fldNameLevel=q.fldNameLevel,
                fldPcode=q.fldPcode,
                fldCodeingLevelId=q.fldCodeingLevelId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsKhedmat(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCodingTarh_KhedmatWithFilter("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldCodingBudjeId = q.fldCodingBudjeId,
                fldEtebarTypeId = q.fldEtebarTypeId.ToString(),
                fldTypeMasrafId = q.fldTypeMasrafId.ToString(),
                fldOrganId = q.fldOrganId.ToString(),
                fldAddress = q.fldAddress,
                fldHoghoghiId = q.fldHoghoghiId,
                fldHaghighiId = q.fldHaghighiId,
                fldStartYear=q.fldStartYear,
                fldEndYear = q.fldEndYear,
                fldPriceVahed = q.fldPriceVahed,
                fldValue = q.fldValue,
                fldKolEtebar = q.fldKolEtebar,
                NameOperator=q.NameOperator
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CopyTemplate(int HeaderId, int TemplateNameId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var q = servic.GetCodingBudje_DetailWithFilter("fldHeaderId", HeaderId.ToString(), "","", 0, IP, out Err).ToList();
                if (Err.ErrorType)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
                    Er = 1;
                }
                else
                {
                    if (q.Count == 0)
                    {
                        MsgTitle = "عملیات موفق";
                        Msg = servic.CopyFromTemplateCodingToCodingBudje(HeaderId,  Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err.ErrorMsg;
                            Er = 1;
                        }
                    }
                    else
                    {
                        Msg = "برای سال انتخاب شده قبلا اطلاعات ثبت شده است.";
                        MsgTitle = "خطا";
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
        public ActionResult LoadTreeCoding(string nod, int HeaderId, string FilterValue)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetCodingBudje_DetailWithFilter("fldPID", nod, HeaderId.ToString(),"%" + FilterValue + "%", 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                var q1 = servic.GetCodingBudje_DetailWithFilter("fldPID", item.fldId.ToString(), HeaderId.ToString(),"", 0, IP, out Err).ToList();
                switch (item.fldNameLevel)
                {
                    case "مأموریت":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "برنامه":
                        asyncNode.IconCls = "KolIco";
                        break;
                    case "طرح/خدمت":
                        asyncNode.IconCls = "MoeinIco";
                        break;
                    default:
                        asyncNode.IconCls = "TafsiliIco";
                        break;
                }
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldBudCode + " - " + item.fldTitle;
                asyncNode.Expanded = false;
                asyncNode.Cls = q1.Count.ToString();
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldTitle = item.fldTitle,
                    fldBudCode = item.fldBudCode,
                    fldNameLevel = item.fldNameLevel,
                    fldEtebarType = item.fldEtebarType,
                    fldEtebarTypeId = item.fldEtebarTypeId,
                    fldHeaderId = item.fldHeaderId,
                    fldLevelId=item.fldLevelId,
                    fldMasrafTTitle=item.fldMasrafTTitle,
                    fldMasrafTypeId= item.fldMasrafTypeId,
                    fldOldId=item.fldOldId,
                    fldStrhid=item.fldStrhid,
                    fldTarh_KhedmatTypeId=item.fldTarh_KhedmatTypeId,
                    fldYear=item.fldYear
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SaveCoding_Details(WCF_Budget.OBJ_CodingBudje_Detail Coding_Details, int PID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Coding_Details.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCodingBudje_Detail(PID, Coding_Details.fldHeaderId, Coding_Details.fldTitle,
                        Coding_Details.fldCode, Coding_Details.fldTarh_KhedmatTypeId, Coding_Details.fldEtebarTypeId,
                        Coding_Details.fldMasrafTypeId, Coding_Details.fldCodeingLevelId,Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCodingBudje_Detail(Coding_Details.fldId,  Coding_Details.fldHeaderId, Coding_Details.fldTitle,
                        Coding_Details.fldCode, Coding_Details.fldTarh_KhedmatTypeId, Coding_Details.fldEtebarTypeId,
                        Coding_Details.fldMasrafTypeId, Coding_Details.fldCodeingLevelId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Budget.OBJ_CodingBudje_Header> data = null;
            data = servic.GetCodingBudje_HeaderWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Budget.OBJ_CodingBudje_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------
            return this.Store(rangeData, data.Count);
        }

        public ActionResult CheckExistPCode(string Pcode, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCodingBudje_DetailWithFilter("fldBudCode", Pcode, HeaderId.ToString(),"", 0, IP, out Err).Where(l=>l.fldLevelId>=2 && l.fldLevelId<4).ToList();
            return Json(new
            {
                Valid = q.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckValidateCode(string Code, int HeaderId, string Pcode, int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Valid = servic.CheckValidCode_CodingDetailBudje(Code, Pcode, Id,HeaderId, IP, out Err).FirstOrDefault().fldValid;
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDefaultCode(string Pcode, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var parentIdd = 0;
            var q = servic.GetDefaultCode_CodingBudje(Pcode,HeaderId, IP, out Err).FirstOrDefault();
            var parent = servic.GetCodingBudje_DetailWithFilter("fldBudCode", Pcode, HeaderId.ToString(),"", 0, IP, out Err).FirstOrDefault();
            if (parent != null)
                parentIdd = parent.fldId;
            return Json(new
            {
                ParentId = parentIdd,
                fldTarh_KhedmatTypeId=parent.fldTarh_KhedmatTypeId,
                DefaultCode = q.fldCode,
                LevelId = q.LevelId,
                LevelName = q.fldLevelName,
                NodLevel=q.NodLevel
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Khedmat(int BudgetCodingId, int CodingTarh_KhedmatId, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.BudgetCodingId = BudgetCodingId.ToString();
            result.ViewBag.CodingTarh_KhedmatId = CodingTarh_KhedmatId.ToString();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult CheckValidYear(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Valid = MyLib.Shamsi.IsShamsiDate(Year + "/01/01");
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }
    }
}