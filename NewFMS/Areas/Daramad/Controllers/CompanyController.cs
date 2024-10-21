using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.Xml;
using System.Text;
using System.IO;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Daramad/Company/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Daramad.OBJ_Company Company)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Company.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCompany(Company.fldTitle, Company.fldShenaseMeli, Company.fldKarbarId, Company.fldURL, Company.fldUserNameService, Company.fldPassService, Company.fldOrganId, Company.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCompany(Company.fldId, Company.fldTitle, Company.fldShenaseMeli, Company.fldKarbarId, Company.fldURL, Company.fldUserNameService, Company.fldPassService, Company.fldOrganId, Company.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCompany(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetCompanyDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldKarbarId = q.fldKarbarId,
                fldShenaseMeli = q.fldShenaseMeli,
                fldURL = q.fldURL,
                fldUserName = q.fldUserName,
                fldUserNameService = q.fldUserNameService,
                fldPassService = q.fldPassService,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول استان و امکان سرچ کردن بر اساس   
        /// </summary>

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_Company> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_Company> data1 = null;
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
                        case "fldShenaseMeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseMeli";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;
                        case "fldURL":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldURL";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetCompanyWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetCompanyWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetCompanyWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_Company> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public FileContentResult testXML(string UserName, string Pass, int FiscalYearId)
        {
            var Path = Server.MapPath(@"~\Uploaded\textXmlIn.xml");
            XmlTextReader reader = new XmlTextReader(Path);

            StringBuilder sb = new StringBuilder();
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sb);

            writer.WriteStartDocument();
            writer.WriteStartElement("AllData");

            Pass=Cservic.HashPass(Pass);

            long Amount = 0;
            var code = "";
            var Tax = "";
            var Complications = "";
            var AccountNum="";
            var BankID = "";
            var BankName = "";
            var OrganName = "";
            var OrganNationalID = "";
            var Type = "0";
            var Name = "";
            var Family = "";
            var NationalCode = "";
            var CompanyType = "0";
            var NationalID = "";
            var RegistrationNum = "";

            var CodeDaramad = "";
            var CodeDescription = "";
            long CodeAmount = 0;
            int CodeTax = 0;
            int CodeComplications = 0;

            List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> ArrCode = new List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez>();

            //InsertElamAvarez:fldAshkhasId  OrganId
            //InsertSodoorFish:fldShomareHesabId  MablaghKoliAsliValue  fldShorooshenaseGhabz  jamkol
            //InsertCodhayeDaramadiElamAvarez_External:fldSharheCodeDaramad  fldShomareHesabCodeDaramadId  fldAsliValue maliyat avarez
            //InsertSodoorFish_Detail:IdCodeDaramadElamAvarez

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        switch (reader.Name)
                        {
                            case "Fish":
                                if (code != "")
                                {
                                    /*save fish ghabli*/
                                    var org = Cservic.GetOrganizationWithFilter("CheckName", OrganName, 0, UserName, Pass, IP, out CErr).Where(l => l.fldShenaseMelli == OrganNationalID).FirstOrDefault();
                                    var Tanzimat = servic.GetTanzimateDaramadWithFilter("fldOrganId", org.fldId.ToString(),FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                    var AshkhaseHesab = Cservic.GetAshkhasWithFilter("fldHoghoghiId", org.fldAshkhaseHoghoghiId.ToString(), "", 0, IP, out CErr).FirstOrDefault();

                                    if (Convert.ToByte(Type) == 2)
                                        NationalCode = NationalID;
                                    if (CErr.ErrorType)
                                    {
                                        Cservic.InsertError(CErr.ErrorMsg, Cservic.GetDate(IP, out CErr).fldDateTime, IP, "", UserName, Pass, out CErr);
                                        return null;
                                    }
                                    if (Err.ErrorType)
                                    {
                                        Cservic.InsertError(Err.ErrorMsg, Cservic.GetDate(IP, out CErr).fldDateTime, IP, "", UserName, Pass, out CErr);
                                        return null;
                                    }

                                    int ashkhasId = servic.AshkhasIdForXmlInput(Convert.ToByte(Type), NationalCode, Name, Family, RegistrationNum, Convert.ToByte(CompanyType), UserName, Pass, IP, out Err);
                                    if (!Err.ErrorType)
                                    {
                                        int ShomareHEsabId = servic.ShomareHesabIdForXml(AccountNum, BankID, AshkhaseHesab.fldId, UserName, Pass, IP, out Err);
                                        if (!Err.ErrorType)
                                        {
                                            int ElamAvarezId = servic.InsertElamAvarez(ashkhasId, true, org.fldId, null, null, "", "کد سیستم مبدا: " + code, UserName, Pass, org.fldId, IP, out Err);
                                            int FishId = servic.InsertSodoorFish(ElamAvarezId, ShomareHEsabId, Amount, Tanzimat.fldShorooshenaseGhabz, Amount, "", UserName, Pass, org.fldId, IP, out Err);
                                            if (Err.ErrorType)
                                            {
                                                servic.DeleteKoliElamAvarez(ElamAvarezId, UserName, Pass, org.fldId, IP, out Err);
                                                ElamAvarezId = 0;
                                                FishId = 0;
                                            }
                                            foreach (var item in ArrCode)
                                            {
                                                var s = servic.GetShomareHesabCodeDaramadWithFilter("fldDaramadCode", item.fldDaramadCode, org.fldId.ToString(),FiscalYearId, 0, IP, out Err).FirstOrDefault();
                                                int IdCodeDaramadElamAvarez = servic.InsertCodhayeDaramadiElamAvarez_External(ElamAvarezId, item.fldSharheCodeDaramad,s.fldId, item.fldAsliValue, 1, item.fldMaliyatValue, item.fldAvarezValue, "", UserName, Pass, org.fldId, IP, out Err);
                                                if (Err.ErrorType)
                                                {
                                                    servic.DeleteKoliElamAvarez(ElamAvarezId, UserName, Pass, org.fldId, IP, out Err);
                                                    ElamAvarezId = 0;
                                                    FishId = 0;
                                                }

                                                servic.InsertSodoorFish_Detail(FishId, IdCodeDaramadElamAvarez, "", UserName, Pass, org.fldId, IP, out Err);
                                                if (Err.ErrorType)
                                                {
                                                    servic.DeleteKoliElamAvarez(ElamAvarezId, UserName, Pass, org.fldId, IP, out Err);
                                                    ElamAvarezId = 0;
                                                    FishId = 0;
                                                }
                                            }

                                            if (ElamAvarezId != 0)
                                            {
                                                writer.WriteStartElement("FishOut");//Start FishOut
                                                writer.WriteAttributeString("Code", code);
                                                writer.WriteStartElement("Serial");
                                                writer.WriteRaw(FishId.ToString());
                                                writer.WriteEndElement();

                                                writer.WriteEndElement();//End FishOut
                                            }
                                        }
                                    }
                                }
                                Amount = 0;
                                code = "";
                                Tax = "";
                                Complications = "";
                                BankID = "";
                                BankName = "";
                                OrganName = "";
                                OrganNationalID = "";
                                Type = "0";
                                Name = "";
                                Family = "";
                                NationalCode = "";
                                CompanyType = "0";
                                NationalID = "";
                                RegistrationNum = "";

                                ArrCode = new List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez>();

                                //reader.Read();
                                code = reader.GetAttribute("Code");
                                break;
                            case "Amount":
                                reader.Read();
                                Amount = Convert.ToInt64(reader.Value);
                                break;
                            case "Tax":
                                reader.Read();
                                Tax = reader.Value;
                                Tax=Tax.Replace("\r\n", "");
                                break;
                            case "Complications":
                                reader.Read();
                                Complications = reader.Value;
                                Complications = Complications.Replace("\r\n", "");
                                break;
                            case "AccountNum":
                                reader.Read();
                                AccountNum = reader.Value;
                                AccountNum = AccountNum.Replace("\r\n", "");
                                break;
                            case "BankID":
                                reader.Read();
                                BankID = reader.Value;
                                BankID = BankID.Replace("\r\n", "");
                                break;
                            case "BankName":
                                reader.Read();
                                BankName = reader.Value;
                                BankName = BankName.Replace("\r\n", "");
                                break;
                            case "OrganName":
                                reader.Read();
                                OrganName = reader.Value;
                                OrganName = OrganName.Replace("\r\n", "");
                                break;
                            case "OrganNationalID":
                                reader.Read();
                                OrganNationalID = reader.Value;
                                OrganNationalID = OrganNationalID.Replace("\r\n", "");
                                break;
                            case "Type":
                                reader.Read();
                                Type = reader.Value;
                                Type = Type.Replace("\r\n", "");
                                if (Type == "")
                                    Type = "0";
                                break;
                            case "Name":
                                reader.Read();
                                Name = reader.Value;
                                Name = Name.Replace("\r\n", "");
                                break;
                            case "Family":
                                reader.Read();
                                Family = reader.Value;
                                Family = Family.Replace("\r\n", "");
                                break;
                            case "NationalCode":
                                reader.Read();
                                NationalCode = reader.Value;
                                NationalCode = NationalCode.Replace("\r\n", "");
                                break;
                            case "CompanyType":
                                reader.Read();
                                CompanyType = reader.Value;
                                CompanyType = CompanyType.Replace("\r\n", "");
                                if (CompanyType == "")
                                    CompanyType = "0";
                                break;
                            case "NationalID":
                                reader.Read();
                                NationalID = reader.Value;
                                NationalID = NationalID.Replace("\r\n", "");
                                break;
                            case "RegistrationNum":
                                reader.Read();
                                RegistrationNum = reader.Value;
                                RegistrationNum = RegistrationNum.Replace("\r\n", "");
                                break;
                            case "CodeDaramad":
                                if (CodeDaramad != "")
                                {
                                    WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez V = new WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez();
                                    V.fldDaramadCode = CodeDaramad;
                                    V.fldSharheCodeDaramad = CodeDescription;
                                    V.fldAsliValue = CodeAmount;
                                    V.fldMaliyatValue = CodeTax;
                                    V.fldAvarezValue = CodeComplications;
                                    ArrCode.Add(V);

                                    CodeDaramad = "";
                                    CodeDescription = "";
                                    CodeAmount = 0;
                                    CodeTax = 0;
                                    CodeComplications = 0;
                                }

                                //reader.Read();
                                CodeDaramad = reader.GetAttribute("Code");
                                CodeDaramad = CodeDaramad.Replace("\r\n", "");
                                break;
                            case "CodeDescription":
                                reader.Read();
                                CodeDescription = reader.Value;
                                CodeDescription = CodeDescription.Replace("\r\n", "");
                                break;
                            case "CodeAmount":
                                reader.Read();
                                CodeAmount = Convert.ToInt64(reader.Value);
                                break;
                            case "CodeTax":
                                reader.Read();
                                CodeTax =  Convert.ToInt32(reader.Value);
                                break;
                            case "CodeComplications":
                                reader.Read();
                                CodeComplications =  Convert.ToInt32(reader.Value);
                                break;
                        }
                        break;

                    case XmlNodeType.Text: //Display the text in each element.
                        //array.Add(reader.Value);
                        break;
                }
            }
            if (code != "")
            {
                if (CodeDaramad != "")
                {
                    WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez V = new WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez();
                    V.fldDaramadCode = CodeDaramad;
                    V.fldSharheCodeDaramad = CodeDescription;
                    V.fldAsliValue = CodeAmount;
                    V.fldMaliyatValue = CodeTax;
                    V.fldAvarezValue = CodeComplications;
                    ArrCode.Add(V);
                }
                /*save fish ghabli*/
                var org = Cservic.GetOrganizationWithFilter("CheckName", OrganName, 0, UserName, Pass, IP, out CErr).Where(l => l.fldShenaseMelli == OrganNationalID).FirstOrDefault();
                var Tanzimat = servic.GetTanzimateDaramadWithFilter("fldOrganId", org.fldId.ToString(),FiscalYearId, 1, IP, out Err).FirstOrDefault();
                var AshkhaseHesab = Cservic.GetAshkhasWithFilter("fldHoghoghiId", org.fldAshkhaseHoghoghiId.ToString(), "", 0, IP, out CErr).FirstOrDefault();
                if (Convert.ToByte(Type) == 2)
                    NationalCode = NationalID;
                if (CErr.ErrorType)
                {
                    Cservic.InsertError(CErr.ErrorMsg, Cservic.GetDate(IP, out CErr).fldDateTime, IP, "", UserName, Pass, out CErr);
                    return null;
                }
                if (Err.ErrorType)
                {
                    Cservic.InsertError(Err.ErrorMsg, Cservic.GetDate(IP, out CErr).fldDateTime, IP, "", UserName, Pass, out CErr);
                    return null;
                }
                int ashkhasId = servic.AshkhasIdForXmlInput(Convert.ToByte(Type), NationalCode, Name, Family, RegistrationNum, Convert.ToByte(CompanyType), UserName, Pass, IP, out Err);
                if (!Err.ErrorType)
                {
                    int ShomareHEsabId = servic.ShomareHesabIdForXml(AccountNum, BankID, AshkhaseHesab.fldId, UserName, Pass, IP, out Err);
                    if (!Err.ErrorType)
                    {
                        int ElamAvarezId = servic.InsertElamAvarez(ashkhasId, true, org.fldId, null, null, "", "کد سیستم مبدا: " + code, UserName, Pass, org.fldId, IP, out Err);
                        int FishId = servic.InsertSodoorFish(ElamAvarezId, ShomareHEsabId, Amount, Tanzimat.fldShorooshenaseGhabz, Amount, "", UserName, Pass, org.fldId, IP, out Err);
                        if (Err.ErrorType)
                        {
                            servic.DeleteKoliElamAvarez(ElamAvarezId, UserName, Pass, org.fldId, IP, out Err);
                            ElamAvarezId = 0;
                            FishId = 0;
                        }

                        foreach (var item in ArrCode)
                        {
                            var s = servic.GetShomareHesabCodeDaramadWithFilter("fldDaramadCode", item.fldDaramadCode, org.fldId.ToString(),FiscalYearId, 0, IP, out Err).FirstOrDefault();
                            int IdCodeDaramadElamAvarez = servic.InsertCodhayeDaramadiElamAvarez_External(ElamAvarezId, item.fldSharheCodeDaramad, s.fldId, item.fldAsliValue, 1, item.fldMaliyatValue, item.fldAvarezValue, "", UserName, Pass, org.fldId, IP, out Err);
                            if (Err.ErrorType)
                            {
                                servic.DeleteKoliElamAvarez(ElamAvarezId, UserName, Pass, org.fldId, IP, out Err);
                                ElamAvarezId = 0;
                                FishId = 0;
                            }

                            servic.InsertSodoorFish_Detail(FishId, IdCodeDaramadElamAvarez, "", UserName, Pass, org.fldId, IP, out Err);
                            if (Err.ErrorType)
                            {
                                servic.DeleteKoliElamAvarez(ElamAvarezId, UserName, Pass, org.fldId, IP, out Err);
                                ElamAvarezId = 0;
                                FishId = 0;
                            }
                        }

                        if (ElamAvarezId != 0)
                        {
                            writer.WriteStartElement("FishOut");//Start FishOut
                            writer.WriteAttributeString("Code", code);
                            writer.WriteStartElement("Serial");
                            writer.WriteRaw(FishId.ToString());
                            writer.WriteEndElement();

                            writer.WriteEndElement();//End FishOut
                        }
                    }
                }
            }

            //foreach (var item in all)
            //{
                
            //}
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();



            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());

            byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(xmlDocument.OuterXml);
            return File(buffer, MimeType.Get(".xml"), "File.xml");
        }
    }
}
