using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using System.Xml;

namespace NewFMS.Areas.Chek.Controllers
{
    public class SodorCheckController : Controller
    {
        //
        // GET: /Chek/SodorCheck/
        WCF_Chek.ChekService servic = new WCF_Chek.ChekService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Chek.ClsError Err = new WCF_Chek.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();

        public ActionResult Index(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.SodorCheck();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult StatusVosuli(string Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic.GetSodorCheckDetail(Convert.ToInt32(Id),Convert.ToInt32(Session["OrganId"]), IP, out Err);
            PartialView.ViewBag.fldVaziat = q.fldvaziat.ToString();
            PartialView.ViewBag.fldTarikhVaziat = q.fldTarikhVaziat;

            PartialView.ViewBag.Tarikh = Cservic.GetDate(IP, out CErr).fldTarikh;
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }
        public ActionResult GetSerialChek(string DasteChekId,string Id)
        {
            List<Models.SerialChek> ch = new List<Models.SerialChek>();
            var q = servic.GetDasteCheckDetail(Convert.ToInt32(DasteChekId),Convert.ToInt32(Session["OrganId"]), IP, out Err);
            for (int i = Convert.ToInt32(q.fldShoroeSeriyal); i < Convert.ToInt32(q.fldShoroeSeriyal) + q.fldTedadBarg; i++)
            {
                Models.SerialChek CboSerial = new Models.SerialChek();
                var m1 = servic.GetSodorCheckWithFilter("fldCodeSerialCheck", i.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                //if (Id == "0")
                //{
                //    if (m1 == null)
                //    {
                //        CboSerial.fldSerial = i;
                //        ch.Add(CboSerial);
                //    }
                //}
                //else
                //{
                    if (m1 == null || (m1 != null && Id == m1.fldId.ToString()))
                    {
                        CboSerial.fldSerial = i;
                        ch.Add(CboSerial);
                    }
                //}                
            }
            return Json(ch, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFactor(byte Type)
        {
            Contracts.Models.ContractEntities m = new Contracts.Models.ContractEntities();
            switch (Type)
            {
                case 1:
                    var q = m.spr_tblContractsSelect("AllFactorTrue", "", Convert.ToInt32(Session["OrganId"]), 0).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldSubject });
                    return this.Store(q);
                case 2:
                    var q1= m.spr_tblFactorSelect("Invoice", "", Convert.ToInt32(Session["OrganId"]), 0).Where(l=>l.fldStatus).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldSharhSanad });
                    return this.Store(q1);
                case 3:
                    var q2 = m.spr_tblTankhah_GroupSelect("AllFactorTrue", "", Convert.ToInt32(Session["OrganId"]), 0).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
                    return this.Store(q2);
            }
            return null;
        }
        public ActionResult New(string id, string DasteChekId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.DasteChekId = DasteChekId;
            return PartialView;
        }
        public ActionResult Print(string SodorchekId, string DasteChekId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.SodorchekId = SodorchekId;
            PartialView.ViewBag.DasteChekId = DasteChekId;
            return PartialView;
        }

        public ActionResult GeneratePDFSodorChek(string SodorchekId, string DasteChekId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                NewFMS.DataSet.DataSet1 dt = new DataSet.DataSet1();
                dt.EnforceConstraints = false;
                NewFMS.DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                NewFMS.DataSet.DataSet1TableAdapters.spr_RptSodorCheckSelectTableAdapter SodorCheck = new NewFMS.DataSet.DataSet1TableAdapters.spr_RptSodorCheckSelectTableAdapter();
                NewFMS.DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                SodorCheck.Fill(dt.spr_RptSodorCheckSelect, "fldId", SodorchekId, 0);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                var q = servic.GetDasteCheckDetail(Convert.ToInt32(DasteChekId),Convert.ToInt32(Session["OrganId"]),IP, out Err);
                var olgo = servic.GetOlgoCheckDetail(q.fldIdOlgoCheck,Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (olgo != null)
                {
                    var s = Cservic.GetFileWithFilter("fldId", olgo.fldIdFile.ToString(), 1, IP, out CErr).FirstOrDefault();
                    MemoryStream m = new MemoryStream(s.fldImage);
                    Report.Load(m);
                    Report.RegisterData(dt, "rasaFMSDaramad");
                    Report.RegisterData(dt_Com, "rasaFMSCommon");
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    return File(stream.ToArray(), "application/pdf");
                }
                else
                    return null;
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Save(WCF_Chek.OBJ_SodorCheck ch,int? ContractId,int? FactorId, int? TankhahGroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; long SumMablaghFactor = 0; long SumMablaghCheck = 0;
            Contracts.Models.ContractEntities m = new Contracts.Models.ContractEntities();
            try
            {
                if (ch.fldId == 0)
                {
                    if (FactorId != null)
                    {
                        SumMablaghCheck = servic.GetSumMablaghCheck_Factor("fldFactorId", FactorId.ToString(), IP, out Err);//جمع مبلغ چکهای ثبت شده
                        SumMablaghFactor = m.spr_tblFactorDetailSelect("fldHeaderId", FactorId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).Select(l => l.fldMablagh).Sum();//جمع مبلغ فاکتورهای ثبت شده
                        if (SumMablaghCheck + ch.fldMablagh > SumMablaghFactor)
                        {
                            return Json(new
                            {
                                Msg = "مبلغ چک/چک های صادر شده نمی تواند از مبلغ کل فاکتور بیشتر باشد",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (TankhahGroupId != null)
                    {
                        SumMablaghCheck = servic.GetSumMablaghCheck_Factor("fldTankhahGroupId", TankhahGroupId.ToString(), IP, out Err);//جمع مبلغ چکهای ثبت شده
                        SumMablaghFactor = m.spr_tblFactorDetailSelect("fldTankhahGroupId", TankhahGroupId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).Select(l => l.fldMablagh).Sum();
                        if (SumMablaghCheck + ch.fldMablagh > SumMablaghFactor)
                        {
                            return Json(new
                            {
                                Msg = "مبلغ چک/چک های صادر شده نمی تواند از مبلغ کل دسته فاکتورهای کارپردازی بیشتر باشد",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (ContractId != null)
                    {
                        SumMablaghCheck = servic.GetSumMablaghCheck_Factor("fldContractId", ContractId.ToString(), IP, out Err);//جمع مبلغ چکهای ثبت شده
                        SumMablaghFactor = m.spr_tblFactorDetailSelect("fldContractId", ContractId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).Select(l => l.fldMablagh).Sum();
                        if (SumMablaghCheck + ch.fldMablagh > SumMablaghFactor)
                        {
                            return Json(new
                            {
                                Msg = "مبلغ چک/چک های صادر شده نمی تواند از مبلغ صورت وضعیت های قرارداد بیشتر باشد",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }                    
                    
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSodorCheck(ch.fldIdDasteCheck, ch.fldTarikhVosol, ch.AshkhasId, ch.fldCodeSerialCheck, ch.fldBabat,ch.fldBabatFlag,ch.fldMablagh,
                        FactorId, ContractId, TankhahGroupId,ch.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    var M=servic.GetSodorCheckDetail(ch.fldId, Convert.ToInt32(Session["OrganId"]), IP, out Err).fldMablagh;//MablaghAsli
                    if (FactorId != null)
                    {
                        SumMablaghCheck = servic.GetSumMablaghCheck_Factor("fldFactorId", FactorId.ToString(), IP, out Err)-M;//جمع مبلغ چکهای ثبت شده
                        SumMablaghFactor = m.spr_tblFactorDetailSelect("fldHeaderId", FactorId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).Select(l => l.fldMablagh).Sum();//جمع مبلغ فاکتورهای ثبت شده
                        if (SumMablaghCheck + ch.fldMablagh > SumMablaghFactor)
                        {
                            return Json(new
                            {
                                Msg = "مبلغ چک/چک های صادر شده نمی تواند از مبلغ کل فاکتور بیشتر باشد",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (TankhahGroupId != null)
                    {
                        SumMablaghCheck = servic.GetSumMablaghCheck_Factor("fldTankhahGroupId", TankhahGroupId.ToString(), IP, out Err) - M;//جمع مبلغ چکهای ثبت شده
                        SumMablaghFactor = m.spr_tblFactorDetailSelect("fldTankhahGroupId", TankhahGroupId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).Select(l => l.fldMablagh).Sum();
                        if (SumMablaghCheck + ch.fldMablagh > SumMablaghFactor)
                        {
                            return Json(new
                            {
                                Msg = "مبلغ چک/چک های صادر شده نمی تواند از مبلغ کل دسته فاکتورهای کارپردازی بیشتر باشد",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (ContractId != null)
                    {
                        SumMablaghCheck = servic.GetSumMablaghCheck_Factor("fldContractId", ContractId.ToString(), IP, out Err) - M;//جمع مبلغ چکهای ثبت شده
                        SumMablaghFactor = m.spr_tblFactorDetailSelect("fldContractId", ContractId.ToString(), Convert.ToInt32(Session["OrganId"]), 0).Select(l => l.fldMablagh).Sum();
                        if (SumMablaghCheck + ch.fldMablagh > SumMablaghFactor)
                        {
                            return Json(new
                            {
                                Msg = "مبلغ چک/چک های صادر شده نمی تواند از مبلغ صورت وضعیت های قرارداد بیشتر باشد",
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    } 
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSodorCheck(ch.fldId, ch.fldIdDasteCheck, ch.fldTarikhVosol, ch.AshkhasId, ch.fldCodeSerialCheck, ch.fldBabat, ch.fldBabatFlag, ch.fldMablagh,
                        FactorId, ContractId, TankhahGroupId,ch.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSodorCheckDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldBabat = q.fldBabat,
                fldBabatFlag = q.fldBabatFlag,
                fldCodeSerialCheck = q.fldCodeSerialCheck,
                fldDarVajh = q.fldDarVajh,
                AshkhasId = q.AshkhasId,
                fldIdDasteCheck = q.fldIdDasteCheck,
                fldMablagh = q.fldMablagh,
                fldTarikhVosol = q.fldTarikhVosol,
                fldDesc = q.fldDesc,
               fldContractId=q.fldContractId,
               fldFactorId=q.fldFactorId,
               fldTankhahGroupId=q.fldTankhahGroupId
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
                Msg = servic.DeleteSodorCheck(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult ReloadChek(int DasteChekId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Chek.OBJ_SodorCheck> data = null;
            data = servic.GetSodorCheckWithFilter("fldIdDasteCheck", DasteChekId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadDasteChek(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Chek.OBJ_DasteCheck> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Chek.OBJ_DasteCheck> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldMoshakhaseDasteCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMoshakhaseDasteCheck";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            break;
                        case "fldTedadBarg":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTedadBarg";
                            break;
                        case "fldShoroeSeriyal":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShoroeSeriyal";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetDasteCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetDasteCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetDasteCheckWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Chek.OBJ_DasteCheck> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Read(StoreRequestParameters parameters, int DasteChekId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Chek.OBJ_SodorCheck> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Chek.OBJ_SodorCheck> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablagh";
                            break;
                        case "fldTarikhVosol":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhVosol";
                            break;
                        case "fldDarVajh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarVajh";
                            break;
                        case "fldBabat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBabat";
                            break;
                        case "fldCodeSerialCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeSerialCheck";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetSodorCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(k=>k.fldIdDasteCheck==DasteChekId).ToList();
                    else
                        data = servic.GetSodorCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(k => k.fldIdDasteCheck == DasteChekId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSodorCheckWithFilter("fldIdDasteCheck", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Chek.OBJ_SodorCheck> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveStatus(int fldId, string fldTarikhPardakht, byte fldVaziat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //ذخیره
                MsgTitle = "ذخیره موفق";
                Msg = servic.InsertChekStatus(fldId,null,null, fldVaziat, fldTarikhPardakht,"", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
