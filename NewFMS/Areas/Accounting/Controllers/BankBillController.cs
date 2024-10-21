using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using System.IO;
using NewFMS.Models;
using Aspose.Cells;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Aspose.Cells.Utility;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class BankBillController : Controller
    {
        // GET: Accounting/BankBill
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId)
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
            result.ViewBag.FiscalYearId = FiscalYearId.ToString();
            return result;
        }
        public ActionResult TransactionList(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId;
            return result;
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic_Com.GetBankWithFilter("", "", 0, IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldBankName });
            return this.Store(data);
        }
        public ActionResult GetBankTemplate(int BankId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic.GetBankTemplate_HeaderWithFilter("fldBankId", BankId.ToString(), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNamePattern });
            return this.Store(data);
        }
        public ActionResult GetAccountNum(int BankId,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic_Com.GetShomareHesabeOmoomiWithFilter("OrganId", Session["OrganId"].ToString(), FiscalYearId.ToString(), BankId.ToString(), 0, IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldShomareHesab });
            return this.Store(data);
        }
        //[HttpPost]
        //public FileContentResult Download(int fldId)
        //{
        //    if (Request.IsAjaxRequest() == false)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        Workbook book = new Workbook();
        //        Worksheet worksheet = book.Worksheets[0];
        //        MemoryStream stream = new MemoryStream();
        //        // set JsonLayoutOptions to treat Arrays as Table
        //        JsonLayoutOptions jsonLayoutOptions = new JsonLayoutOptions();
        //        jsonLayoutOptions.ArrayAsTable = true;
        //        jsonLayoutOptions.IgnoreNull = true;
        //        var q = servic.GetBankBill_HeaderDetail(fldId, IP, out Err);
        //        JsonUtility.ImportData(q.fldJsonFile, worksheet.Cells, 0, 0, jsonLayoutOptions);
        //        stream = book.SaveToStream();
        //        return File(stream.ToArray(), MimeType.Get(".xlsx"), "DownloadFile.xlsx");
        //        //book.Save(Server.MapPath(@"~\Uploaded\DownloadFile.xlsx"));
        //        //return File(Server.MapPath(@"~\Uploaded\DownloadFile.xlsx"), MimeType.Get(".xlsx"), "DownloadFile.xlsx");
        //    }
        //}
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePathBill"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathBill"].ToString());
                    Session.Remove("savePathBill");
                    System.IO.File.Delete(physicalPath);
                }

                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var FileNamee = Path.GetFileNameWithoutExtension(Request.Files[0].FileName) + Session["UserId"] + extension;
                HttpPostedFileBase file = Request.Files[0];
                string savePath = Server.MapPath(@"~\Uploaded\" + FileNamee);

                var IsExcel = FileInfoExtensions.IsExcel(file);
                if (IsExcel == true)
                {
                    //if (Request.Files[0].ContentLength <= 1048576)
                    //{
                    file.SaveAs(savePath);
                    Session["savePathBill"] = savePath;
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل با موفقیت آپلود شد.",
                        IsValid = true
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                    //else
                    //{
                    //    object r = new
                    //    {
                    //        success = true,
                    //        name = Request.Files[0].FileName,
                    //        Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد.",
                    //        IsValid = false
                    //    };
                    //    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده معتبر نمی باشد.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                }
            }
            catch (Exception x)
            {
                if (Session["savePathBill"] != null)
                {
                    System.IO.File.Delete(Session["savePathBill"].ToString());
                    Session.Remove("savePathBill");
                }
                return null;
            }
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_Accounting.OBJ_BankBillHeader> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_BankBillHeader> data1 = null;
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
                        case "fldNamePattern":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNamePattern";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetBankBill_HeaderWithFilter(field, searchtext, 0, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetBankBill_HeaderWithFilter(field, searchtext, 0, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetBankBill_HeaderWithFilter("", "", 0, IP, out Err).ToList();
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
            List<WCF_Accounting.OBJ_BankBillHeader> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadTransactions(StoreRequestParameters parameters, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_BankBillDetails> data = null;
            data = servic.GetBankBill_DetailsWithFilter("fldHedearId", HeaderId.ToString(),"", 0, IP, out Err).OrderByDescending(l=>l.fldTarikh).ThenByDescending(l=>l.fldTime).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }
            List<WCF_Accounting.OBJ_BankBillDetails> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteBankBill(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Save(WCF_Accounting.OBJ_BankBillHeader Header, int BankId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int HeaderId = 0;
            try
            {
                //ذخیره
                if (Session["savePathBill"] != null)
                {
                    var workbook = new Workbook(Session["savePathBill"].ToString());
                    MemoryStream stream = new MemoryStream();
                    workbook.Save(stream,SaveFormat.Json);
                    var jsonstr = Encoding.UTF8.GetString(stream.ToArray());

                    HeaderId = servic.InsertBankBill(Header.fldName, Header.fldPatternId, Header.fldShomareHesabId, Header.fldFiscalYearId, jsonstr, IP, Header.fldDesc,
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err);
                    if (Err.ErrorType)
                    {
                        Msg = Err.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                    else
                    {
                        var Temp = servic.GetBankTemplate_HeaderDetail(Header.fldPatternId, IP, out Err);
                        var dt = ProcessXlsBill(Convert.ToInt32(Temp.fldFileId), Temp.fldStartRow, workbook);
                        if (dt.Rows.Count > 0)
                        {
                            System.Data.DataColumn newColumn = new System.Data.DataColumn("fldHedearId", typeof(System.Int32));
                            newColumn.DefaultValue = HeaderId;
                            dt.Columns.Add(newColumn);

                            System.Data.DataColumn newColumn1 = new System.Data.DataColumn("fldBankId", typeof(System.Int32));
                            newColumn1.DefaultValue = BankId;
                            dt.Columns.Add(newColumn1);                            

                            var FeedBack = ProcessBulkCopyBill(dt);
                            if (FeedBack == 1)
                            {
                                if (servic.CheckCountData(HeaderId,IP,out Err).Value)
                                {
                                    Msg = "بارگزاری فایل اکسل با موفقیت انجام شد.";
                                    MsgTitle = "ذخیره موفق";
                                }
                                else
                                {
                                    Msg = "فایل انتخاب شده، قبلا ذخیره شده است.";
                                    MsgTitle = "خطا";
                                    Er = 1;
                                }                                
                            }
                            else
                            {
                                servic.DeleteBankBill(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                                Msg = "خطا در هنگام ذخیره سازی اطلاعات";
                                MsgTitle = "خطا";
                                Er = 1;
                            }
                        }
                        else
                        {
                            servic.DeleteBankBill(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            Er = 1;
                            Msg = "خطا در خواندن فایل Excel";
                            MsgTitle = "خطا";
                        }
                    }
                    if (Session["savePathBill"] != null)
                    {
                        System.IO.File.Delete(Session["savePathBill"].ToString());
                        Session.Remove("savePathBill");
                    }
                }
                else
                {
                    Msg = "لطفا فایل صورتحساب را بارگزاری نمایید.";
                    MsgTitle = "خطا";
                    Er = 1;
                }                
            }
            catch (Exception x)
            {
                servic.DeleteBankBill(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
                if (Session["savePathBill"] != null)
                {
                    System.IO.File.Delete(Session["savePathBill"].ToString());
                    Session.Remove("savePathBill");
                }
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        private DataTable ProcessXlsBill(int PatternFileId, int FirstRow, Workbook wbbill)
        {
            try
            {
                //گرفتن فایل الگو
                var f = servic_Com.GetFileWithFilter("fldId", PatternFileId.ToString(), 1, IP, out Err_Com).FirstOrDefault();

                //باز کردن فایل الگو
                Stream stream = new MemoryStream(f.fldImage);
                Aspose.Cells.Workbook wbtemp = new Aspose.Cells.Workbook(stream);
                var Tempsheet = wbtemp.Worksheets[0];

                //باز کردن فایل صورتحساب
                //Aspose.Cells.Workbook wbbill = new Aspose.Cells.Workbook(Session["savePathBill"].ToString());
                var Billsheet = wbbill.Worksheets[0];

                //گرفتن رنج دیتا در فایل صورتحساب
                Range Billrange = Billsheet.Cells.MaxDisplayRange;

                //گرفتن همان رنج در فایل الگو
                Range Temprange = Tempsheet.Cells.CreateRange(Billrange.Address);

                //کپی فرمول در رنج دیتا
                Aspose.Cells.Range source = Tempsheet.Cells.CreateRange("AE1:AJ1");
                Aspose.Cells.Range target = Tempsheet.Cells.CreateRange(String.Format("AE{0}:AJ{1}", 2, Billsheet.Cells.MaxDataRow + 1));
                source.AutoFill(target);

                //کپی کردن دیتا در فایل الگو
                Temprange.Copy(Billrange);

                //بدست آوردن ستونهای خودساخته و محاسبه فرمول 
                var rangestr = String.Format("AE{0}:AJ{1}", FirstRow, Billsheet.Cells.MaxDataRow + 1);
                Range MyRange = Tempsheet.Cells.CreateRange(rangestr);
                CalculationOptions op = new CalculationOptions();
                op.IgnoreError = true;
                foreach (Cell cell in MyRange)
                {
                    cell.Calculate(op);
                }

                //اکسپورت فایل اکسل به دیتا تیبل
                DataTable dt = new DataTable();
                dt = Tempsheet.Cells.ExportDataTable(FirstRow - 1, 30, (Billsheet.Cells.MaxDataRow - FirstRow) + 2, 6, false);
                wbtemp.Dispose();
                wbbill.Dispose();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            dt.Columns[i].ColumnName = "fldTarikh";
                            break;
                        case 1:
                            dt.Columns[i].ColumnName = "fldTime";
                            break;
                        case 2:
                            dt.Columns[i].ColumnName = "fldCodePeygiri";
                            break;
                        case 3:
                            dt.Columns[i].ColumnName = "fldMandeh";
                            break;
                        case 4:
                            dt.Columns[i].ColumnName = "fldBedehkar";
                            break;
                        case 5:
                            dt.Columns[i].ColumnName = "fldBestankar";
                            break;
                    }
                }
                return dt;
            }
            catch (Exception x)
            {
                return new DataTable();
            }
        }
        private Byte ProcessBulkCopyBill(DataTable dt)
        {
            byte Feedback = 1;
            string connString = ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (var copy = new SqlBulkCopy(conn))
                {
                    conn.Open();
                    copy.DestinationTableName = "Acc.tblBankBill_Details";
                    copy.ColumnMappings.Add("fldTarikh", "fldTarikh");
                    copy.ColumnMappings.Add("fldTime", "fldTime");
                    copy.ColumnMappings.Add("fldCodePeygiri", "fldCodePeygiri");
                    copy.ColumnMappings.Add("fldMandeh", "fldMandeh");
                    copy.ColumnMappings.Add("fldBedehkar", "fldBedehkar");
                    copy.ColumnMappings.Add("fldBestankar", "fldBestankar");
                    copy.ColumnMappings.Add("fldHedearId", "fldHedearId");
                    copy.ColumnMappings.Add("fldBankId", "fldBankId");
                    copy.BatchSize = dt.Rows.Count;
                    var c = dt.Rows.Count;
                    try
                    {
                        copy.WriteToServer(dt);
                    }
                    catch (Exception x)
                    {
                        Feedback = 0;
                    }
                }
            }
            return Feedback;
        }
    }
}