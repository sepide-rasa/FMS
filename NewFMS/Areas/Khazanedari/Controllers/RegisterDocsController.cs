using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using FastMember;
using System.IO;
using NewFMS.Models;

namespace NewFMS.Areas.Khazanedari.Controllers
{
    public class RegisterDocsController : Controller
    {
        // GET: Khazanedari/RegisterDocs
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servic_Drd = new WCF_Daramad.DaramadService();

        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Daramad.ClsError Err_drd = new WCF_Daramad.ClsError();
        public ActionResult Index(string containerId, short Year, int FiscalYearId)
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
            if (servic_com.GetDate(IP, out Err_com).fldTarikh.Substring(0, 4) == Year.ToString())
            {
                result.ViewBag.Flag = 1;
            }
            else
            {
                result.ViewBag.Flag = 0;
            }
            result.ViewBag.Year = Year;
            result.ViewBag.FiscalYearId = FiscalYearId;
            result.ViewBag.OrganId = Session["OrganId"].ToString();
            return result;
        }        
        public ActionResult New(int HeaderId, int FiscalYearId, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //چک کردن اختتامیه
            var q = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "4", "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 1, IP, out Err).FirstOrDefault();
            if (q != null)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "پس از ثبت سند اختتامیه، امکان ثبت سند دیگری وجود ندارد."
                });
                DirectResult dr = new DirectResult();
                return dr;
            }
            byte ErsalShode = 0;
            if (HeaderId != 0)
            {
                var Header1 = servic.GetDocumentRecord_HeaderWithFilter("fldPId", HeaderId.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 1, IP, out Err).FirstOrDefault();
                if (Header1 != null)
                {
                    ErsalShode = 1;
                }
            }
            var Setting = servic_com.GetGeneralSettingWithFilter("fldModuleId", "4", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_com).ToList();
            var result = new Ext.Net.MVC.PartialViewResult();
            var CurrentDate = servic_com.GetDate(IP, out Err_com).fldTarikh;
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.CurrentDate = CurrentDate;
            result.ViewBag.FiscalYearId = FiscalYearId;
            result.ViewBag.Year = Year;
            result.ViewBag.ErsalShode = ErsalShode;
            result.ViewBag.CheckRemain = Setting.Where(l => l.fldId == 4).FirstOrDefault().fldValue;
            result.ViewBag.Required = Setting.Where(l => l.fldId == 5).FirstOrDefault().fldValue;
            return result;
        }
        public ActionResult CheckStatus(int DocId, byte ModuleId,short Year,int fldPid)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int Id = 0;
            if (ModuleId == 10)
            {
                var q = servic.GetDocumentRecord_HeaderWithFilter("fldPId", fldPid.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), Year, 10,0, IP, out Err).ToList();
                if (q.Where(l => l.fldTypeSanad == 3).Count()>0)
                {
                    if (q.Where(l => l.fldTypeSanadName_Pid.Contains("مرجوع شده") && l.fldId == DocId).Count()>0)
                    {
                        return Json(new
                        {
                            Id = Id,
                            Status = false
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var Status = servic.CheckDocStatus(DocId, IP,out Id, out Err);
            return Json(new
            {
                Id=Id,
                Status = Status
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckDelAllowed(int DocId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDocumentRecord_HeaderWithFilter("fldPId", DocId.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), 0, 0, 1, IP, out Err).FirstOrDefault();
            return Json(new
            {
                Status = q!=null?false:true
            }, JsonRequestBehavior.AllowGet);
        }        
        public ActionResult Read(StoreRequestParameters parameters, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Accounting.OBJ_DocumentRecord_Header> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_DocumentRecord_Header> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDocumentNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDocumentNum";
                            break;
                        case "fldAtfNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAtfNum";
                            break;
                        case "ShomareRoozane":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "ShomareRoozane";
                            break;
                        case "fldArchiveNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldArchiveNum";
                            break;
                        case "fldShomareFaree":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareFaree";
                            break;
                        case "fldTarikhDocument":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhDocument";
                            break;
                        case "fldDescriptionDocu":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDescriptionDocu";
                            break;
                        case "fldTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeName";
                            break;
                        case "fldNameModule":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameModule";
                            break;
                        case "fldTypeSanadName_Pid":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeSanadName_Pid";
                            break;
                        case "fldMainDocNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMainDocNum";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetDocumentRecord_HeaderWithFilter(field, searchtext, "", "", Convert.ToInt32(Session["OrganId"]), Year, 10, 100, IP, out Err).ToList();
                    else
                        data = servic.GetDocumentRecord_HeaderWithFilter(field, searchtext, "", "", Convert.ToInt32(Session["OrganId"]), Year, 10, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetDocumentRecord_HeaderWithFilter("", "", "", "", Convert.ToInt32(Session["OrganId"]), Year, 10, 100, IP, out Err).ToList();
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

            List<WCF_Accounting.OBJ_DocumentRecord_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Save(WCF_Accounting.OBJ_DocumentRecord_Header Header, List<NewFMS.Models.DocDetails> DocumentRecord_DetailsArray, int? BankBillId, byte? Type, int? CheckId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int Msg2 = 0; var IdHeader = 0; var DocumentNumber = 0;
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "acc.tblDocumentRecord_Details" };
                using (var reader = FastMember.ObjectReader.Create(DocumentRecord_DetailsArray))
                {
                    dt.Load(reader);
                }
                if ((Header.fldTypeSanad == 1 || Header.fldTypeSanad == 4) && Header.fldType == 2)
                {
                    Header.fldTypeSanad = 2;
                }
                if (Header.fldId == 0)
                {
                    //ذخیره
                    dt.Columns.Remove("fldId");
                    dt.Columns.Remove("fldDocument_HedearId");
                    dt.Columns.Remove("fldCaseId");
                    var NumDoc = servic.InsertDocument(Header.fldArchiveNum, Header.fldDescriptionDocu, Header.fldTarikhDocument, 0, Header.fldType,
                        dt, 10, 10, Header.fldShomareFaree, Convert.ToInt32(Header.fldFiscalYearId), Convert.ToByte(Header.fldTypeSanad), null,Header.fldEdit, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                    Msg = NumDoc == 0 ? "یادداشت با موفقیت ثبت شد." : "سند شماره " + NumDoc + "  با موفقیت ثبت شد.";
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (NumDoc != 0)
                    {
                        var Sanad = servic.GetDocumentRecord_HeaderWithFilter("fldDocumentNum", NumDoc.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), Header.fldYear, 10, 1, IP, out Err).FirstOrDefault();
                        IdHeader = Sanad.fldId;
                        DocumentNumber = NumDoc;
                        if (BankBillId != null)
                        {
                            MsgTitle = "عملیات موفق";
                            DocumentRecord_DetailsArray.Reverse();
                            var SourceIds = String.Join(",", DocumentRecord_DetailsArray.Skip(1).Select(x => x.fldSourceId));
                            Msg = servic.BankBillMap(Convert.ToInt32(BankBillId), IdHeader,Convert.ToByte(Type), SourceIds, IP, "", Session["Username"].ToString(), Session["Password"].ToString(), out Err);
                            if (Err.ErrorType)
                            {
                                Msg = Err.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;
                            }
                        }
                        else if (CheckId != null)
                        {
                            servic_Drd.UpdateReceive_Check(Convert.ToInt32(CheckId),IdHeader, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err_drd);
                        }
                    }
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = "ویرایش با موفقیت انجام شد.";
                    var q = servic.GetDocumentRecord_HeaderDetail(Header.fldId, Convert.ToInt32(Session["OrganId"]), Header.fldYear, 10, IP, out Err);
                    if (q.fldAccept == 1)
                    {
                        return Json(new
                        {
                            Msg = "سند انتخاب شده به حسابداری ارسال شده و امکان ویرایش آن وجود ندارد.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    servic.UpdateDocument(Header.fldId, Header.fldDocumentNum, Header.fldArchiveNum, Header.fldDescriptionDocu, Header.fldTarikhDocument, 0, Header.fldType,
                        dt, 10, 10, Header.fldShomareFaree, Convert.ToByte(Header.fldTypeSanad), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    IdHeader = Header.fldId;
                    DocumentNumber = Header.fldDocumentNum;
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
                IdHeader = IdHeader,
                DocumentNumber = DocumentNumber
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendToModule(string DocId, byte fldModuleErsalId, byte fldModuleSaveId, string DocDate)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {
                if (DocDate == "")
                    DocDate = servic_com.GetDate(IP, out Err_com).fldTarikh;
                MsgTitle = "عملیات موفق";
                var DocIdList= DocId.Split(',').ToList();
                foreach (var item in DocIdList)
                {
                    Msg = servic.Document_CopyInsert(Convert.ToInt32(item), Convert.ToInt32(Session["OrganId"]), fldModuleErsalId, fldModuleSaveId, 1, DocDate, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
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
                Er = Er,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRelatedDocs(int Document_HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var RelatedDocIds = "";
            var q = servic.SelectMortabet(Document_HeaderId, 10, IP, out Err).Where(l => l.fldId != Document_HeaderId).ToList();
            //var q = servic.GetDocumentRecord_HeaderWithFilter("Mortabet", Document_HeaderId.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), 0, 10, 0, IP, out Err)
            //    .Where(l => l.fldId != Document_HeaderId);
            if(q.Count()>0){
                RelatedDocIds=String.Join(",", q.Select(l => l.fldId));
            }
            return Json(new
            {
                RelatedDocIds = RelatedDocIds
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCodingDetails_Tooltip(int fldCodingId, short fldYear)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetRptByCoding(fldCodingId, Convert.ToInt32(Session["OrganId"]), fldYear, 10, IP, out Err);
            var Tashkhis = "";
            if (q.fldType == 1)
            {
                Tashkhis = "بدهکار";
            }
            else if (q.fldType == 2)
            {
                Tashkhis = "بستانکار";
            }
            return Json(new
            {
                fldBedehkar = q.fldBedehkar,
                fldBestankar = q.fldBestankar,
                fldTitle = q.fldTitle,
                Tashkhis = Tashkhis,
                fldType = q.fldType,
                MandehHeasb = q.MandehHeasb
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRemain(int fldCodingId, short fldYear, long Bes_Bed, string Type, int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            long Bedehkar = 0, Bestankar = 0;
            if (Type == "ColBed_Khazane")
            {
                Bedehkar = Bes_Bed;
            }
            else
            {
                Bestankar = Bes_Bed;
            }
            var q = servic.CheckRemain(fldCodingId, Id, Convert.ToInt64(Bedehkar), Convert.ToInt64(Bestankar), Convert.ToInt32(Session["OrganId"]), fldYear, 10, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldCheck = q.fldCheck
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFirst_LastOfYear(short Year, int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var LastDate = "";
            var LastDateShamsi = "";

            var q = servic.GetDocumentRecord_HeaderWithFilter("lastDateDoc", Id.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), Year, 10, 0, IP, out Err).FirstOrDefault();
            if (q != null)
            {
                LastDate = MyLib.Shamsi.Shamsi2miladiDateTime(q.fldTarikhDocument).ToString();
                LastDateShamsi = q.fldTarikhDocument;
            }
            var LastDay = MyLib.Shamsi.LastDayOfShamsiYear(Year);
            var MinDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/01/01").ToString();
            var MaxDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/12/" + LastDay.ToString().PadLeft(2, '0')).ToString();
            return Json(new
            {
                MinDate = MinDate,
                MaxDate = MaxDate,
                LastDate = LastDate,
                LastDateShamsi = LastDateShamsi,
                MinDateShamsi = Year.ToString() + "/01/01",
                MaxDateShamsi = Year.ToString() + "/12/" + LastDay.ToString().PadLeft(2, '0')
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDocType(short Year, int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var q1 = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "1", "", "", Convert.ToInt32(Session["OrganId"]), Year, 10, 1, IP, out Err).FirstOrDefault();
            //if (q1 != null)
            //{
            //    var q = servic.GetDocumentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Where(l => l.fldId != 1).Select(c => new { fldId = c.fldId, fldName = c.fldName });
            //    return this.Store(q);
            //}
            //else
            //{
            var q = servic.GetDocumentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldId != 1 && l.fldId != 4 && l.fldId != 5).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName }).ToList();
                return this.Store(q);
            //}
        }
        public ActionResult ReadDocRegister_Details(StoreRequestParameters parameters, int HeaderId)
        {
            List<WCF_Accounting.OBJ_DocumentRecord_Details> data = null;
            data = servic.GetDocumentRecord_DetailsWithFilter("fldDocument_HedearId", HeaderId.ToString(), 0, IP, out Err).ToList();
            return this.Store(data);
        }
        public ActionResult GetDefaultNum_Atf(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetLastNum_AtfDocumentRecord(Year, Convert.ToInt32(Session["OrganId"]), 10, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldDocumentNum = q.fldDocumentNum,
                fldAtfNum = q.fldAtfNum,
                ShomareRoozane = q.ShomareRoozane
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetHeadLines(short Year, int? ProjectId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (ProjectId != 0 && ProjectId != null)
            {
                var q = servic.SelectCoding_Project("fldId", ProjectId.ToString(), 0, IP, out Err).ToList().Select(c => new
                {
                    fldId = c.fldCodingDetailId,
                    fldName = "(" + c.fldCodeAcc + ")" + c.fldCodingTitle,
                    fldMahiyatId = c.fldMahiyatId,
                    fldItemId = 0
                });
                return this.Store(q);
            }
            else
            {
                var q = servic.GetCoding_DetailsWithFilter("Child", Year.ToString(), Session["OrganId"].ToString(), "", 0, IP, out Err).OrderBy(l => l.fldTitle).ToList().Select(c => new { fldId = c.fldId, fldName = "(" + c.fldCode + ")" + c.fldTitle, fldMahiyatId = c.fldMahiyatId, fldItemId = c.fldItemId });
                return this.Store(q);
            }
        }
        public ActionResult GetCostCenter()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCenterCostWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameCenter });
            return this.Store(q);
        }
    }
}