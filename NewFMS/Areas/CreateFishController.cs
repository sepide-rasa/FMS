using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace NewFMS.Areas
{
    public class CreateFishController : Controller
    {
        // GET: CreateFish 
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        WCF_Weigh.WeighService servic_Weigh = new WCF_Weigh.WeighService();
        WCF_Weigh.ClsError Err_Weigh = new WCF_Weigh.ClsError();

        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        string User = ""; string Pass = ""; int Org = 0; int TableId = 0; int IDElamAvarez = 0; int SodourFishId = 0;
        public void Index(int ShomareHesabCodeDaramadId, int AshkhasId, int OrganId, List<WCF_Daramad.OBJ_ParametreSabet_Value> Values, int TableId1, long Price, int Count, decimal? Ton
            , string User1, string Pass1, int Org1, int FiscalYearId, out WCF_Common.ClsError MyErr)
        {
            MyErr = new WCF_Common.ClsError();
            User = User1;
            Pass = Pass1;
            Org = Org1;
            TableId = TableId1;
            SaveParamsValue(ShomareHesabCodeDaramadId, AshkhasId, OrganId, Values, Price, Count, Ton, User, Pass, Org, FiscalYearId, out Err_Com);
            if (Err_Com.ErrorType)
            {
                MyErr = new WCF_Common.ClsError { ErrorMsg = Err_Com.ErrorMsg, ErrorType = Err_Com.ErrorType };
            }
            MyErr.OutputId = IDElamAvarez;
            MyErr.OutputId2 = SodourFishId;
        }
        public void Fish(int CodeDaramadElamAvarezID, int FiscalYearId, out WCF_Common.ClsError MyErr)
        {
            MyErr = new WCF_Common.ClsError();
            int sal = 0, mah = 0;

            try
            {
                var FishInf = servic.GetEbtal_SodoorFish(IDElamAvarez, IP, out Err).FirstOrDefault();
                SodourFishId = servic.InsertSodoorFish(IDElamAvarez, FishInf.fldShomareHesabId, FishInf.fldJamFish,
                    FishInf.fldShorooShenaseGhabz, FishInf.fldJamKol, "", User, Pass,
                    Org, IP, out Err);
                if (Err.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertSodoorFish: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                    return;
                }
                servic.InsertSodoorFish_Detail(SodourFishId, CodeDaramadElamAvarezID, "", User, Pass,
                    Org, IP, out Err);
                if (Err.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertSodoorFish_Detail: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                    return;
                }

                var organ = servic_Com.GetOrganizationDetail(FishInf.fldOrganId, User, Pass, IP, out Err_Com);
                if (organ.fldCodAnformatic != null)
                {
                    var FishNum = FishInf.fldShorooShenaseGhabz + SodourFishId.ToString();
                    sal = FishNum.Length - 2;
                    if (FishNum.Length > 8)
                    {
                        string s = FishNum.Substring(8).PadRight(2, '0');
                        FishNum = FishNum.Substring(0, 8);
                        mah = Convert.ToInt32(s);
                    }
                    ghabz gh = new ghabz(Convert.ToInt32(FishNum), Convert.ToInt32(organ.fldCodAnformatic), Convert.ToInt32(organ.fldCodKhedmat)
                        , Convert.ToInt64(FishInf.fldJamFish), sal, mah);
                    var ShGhabz = gh.ShGhabz;
                    var ShPardakht = gh.ShPardakht;
                    var BarcodeText = gh.BarcodeText;
                    servic.UpdateShenaseGhabz_Pardakht(SodourFishId, ShGhabz, ShPardakht, BarcodeText, User, Pass, IP, out Err);
                    if (Err.ErrorType)
                    {
                        MyErr = new WCF_Common.ClsError { ErrorMsg = "UpdateShenaseGhabz_Pardakht: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                        return;
                    }
                }
                servic_Weigh.InsertElamAvarez_ModuleOrgan(IDElamAvarez, CodeDaramadElamAvarezID, TableId, "", User, Pass, IP, out Err_Weigh);
                if (Err_Weigh.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertElamAvarez_ModuleOrgan: " + Err_Weigh.ErrorMsg, ErrorType = Err_Weigh.ErrorType };
                    return;
                }
                if (SodourFishId != 0)//ارسال به حسابداری
                {
                    var setting = servic.GetTanzimateDaramadWithFilter("fldOrganId_HesabRayan", organ.fldId.ToString(), FiscalYearId, 0, IP, out Err).FirstOrDefault();
                    var fishDetail = servic.GetSodoorFish_DetailWithFilter("fldfishid", SodourFishId.ToString(), 0, IP, out Err).ToList();
                    if (fishDetail.Count > 0)
                    {
                        var codeDaramadi = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId_HesabRayan", fishDetail[0].fldCodeElamAvarezId.ToString(), 1, IP, out Err).FirstOrDefault();
                        var hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", codeDaramadi.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                        if (hesab != null)
                        {
                            var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                            if (company != null)//ارسال به حسابرایان
                            {
                                XmlDocument XDoc1 = new XmlDocument();
                                // Create root node.
                                XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                                XDoc1.AppendChild(XElemRoot1);
                                XmlDocument XDoc2 = new XmlDocument();
                                // Create root node.
                                XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                                XDoc2.AppendChild(XElemRoot2);
                                XmlDocument XDoc = new XmlDocument();
                                // Create root node.
                                XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                                XDoc.AppendChild(XElemRoot);
                                bool discount = false;
                                foreach (var item in fishDetail)
                                {
                                    codeDaramadi = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId_HesabRayan", item.fldCodeElamAvarezId.ToString(), 1, IP, out Err).FirstOrDefault();
                                    hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", codeDaramadi.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                                    if (hesab != null)
                                    {
                                        XmlElement Xsource = XDoc.CreateElement("Node");
                                        Xsource.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                        Xsource.SetAttribute("RevenueTaxCost", "0");
                                        Xsource.SetAttribute("RevenueAvarezCost", "0");
                                        Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                                        Xsource.SetAttribute("RevenueBed", "0");
                                        Xsource.SetAttribute("RevenueBes", "0");
                                        Xsource.SetAttribute("AmountSavingBand", "0");
                                        var fish1 = servic.GetSodoorFishDetail(SodourFishId, IP, out Err);
                                        if (discount == false)
                                        {
                                            Xsource.SetAttribute("Discount", (fish1.fldJamKol - fish1.fldMablaghAvarezGerdShode).ToString());
                                            discount = true;
                                        }
                                        else
                                            Xsource.SetAttribute("Discount", "0");
                                        if (codeDaramadi.fldTakhfifAsliValue != null && codeDaramadi.fldTakhfifAsliValue != 0)
                                        {
                                            Xsource.SetAttribute("RevenueCost", (codeDaramadi.fldTakhfifAsliValue).ToString());
                                        }
                                        else
                                        {
                                            Xsource.SetAttribute("RevenueCost", (codeDaramadi.fldAsliValue).ToString());
                                        }
                                        XElemRoot.AppendChild(Xsource);

                                        if (codeDaramadi.fldAvarezValue != 0)
                                        {
                                            var code = servic.GetShomareHesabCodeDaramadWithFilter("fldId_HesabRayan", setting.fldAvarezId.ToString(), "", FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                            hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", code.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                            if (hesab != null)
                                            {
                                                XmlElement Xsource1 = XDoc.CreateElement("Node");
                                                Xsource1.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                                Xsource1.SetAttribute("RevenueTaxCost", "0");
                                                Xsource1.SetAttribute("RevenueAvarezCost", "0");
                                                Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                                                Xsource1.SetAttribute("RevenueBed", "0");
                                                Xsource1.SetAttribute("RevenueBes", "0");
                                                Xsource1.SetAttribute("AmountSavingBand", "0");
                                                Xsource1.SetAttribute("Discount", "0");
                                                if (codeDaramadi.fldTakhfifAvarezValue != null && codeDaramadi.fldTakhfifAvarezValue != 0)
                                                {
                                                    Xsource1.SetAttribute("RevenueCost", codeDaramadi.fldTakhfifAvarezValue.ToString());
                                                }
                                                else
                                                {
                                                    Xsource1.SetAttribute("RevenueCost", codeDaramadi.fldAvarezValue.ToString());
                                                }
                                                XElemRoot.AppendChild(Xsource1);
                                            }
                                        }
                                        if (codeDaramadi.fldMaliyatValue != 0)
                                        {
                                            var code = servic.GetShomareHesabCodeDaramadWithFilter("fldId_HesabRayan", setting.fldMaliyatId.ToString(), "", FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                            hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", code.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                            if (hesab != null)
                                            {
                                                XmlElement Xsource2 = XDoc.CreateElement("Node");
                                                Xsource2.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                                Xsource2.SetAttribute("RevenueTaxCost", "0");
                                                Xsource2.SetAttribute("RevenueAvarezCost", "0");
                                                Xsource2.SetAttribute("RevenueTaxAvarezCost", "0");
                                                Xsource2.SetAttribute("RevenueBed", "0");
                                                Xsource2.SetAttribute("RevenueBes", "0");
                                                Xsource2.SetAttribute("AmountSavingBand", "0");
                                                Xsource2.SetAttribute("Discount", "0");
                                                if (codeDaramadi.fldTakhfifMaliyatValue != null && codeDaramadi.fldTakhfifMaliyatValue != 0)
                                                {
                                                    Xsource2.SetAttribute("RevenueCost", codeDaramadi.fldTakhfifMaliyatValue.ToString());
                                                    //Xsource2.SetAttribute("Discount", (codeDaramadi.fldMaliyatValue - codeDaramadi.fldTakhfifMaliyatValue).ToString());
                                                }
                                                else
                                                {
                                                    Xsource2.SetAttribute("RevenueCost", codeDaramadi.fldMaliyatValue.ToString());
                                                }
                                                XElemRoot.AppendChild(Xsource2);
                                            }
                                        }
                                    }
                                }
                                var fish = servic.GetSodoorFishDetail(SodourFishId, IP, out Err);
                                var elamavarez = servic.GetElamAvarezDetail(fish.fldElamAvarezId, organ.fldId.ToString(), IP, out Err);
                                var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", FishInf.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                                HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                                XmlNode x = h.AccountListRevenue(Convert.ToInt32(company.fldPassService));
                                string xx = "<hesabs>" + x.InnerXml.Replace("\"", "\'") + "</hesabs>";
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xx));
                                System.Xml.XmlReader xr = System.Xml.XmlReader.Create(ms);
                                string hesabNumber = "0";
                                while (xr.Read())
                                {
                                    if (xr.NodeType == System.Xml.XmlNodeType.Element)
                                    {
                                        if (xr.Name == "Node")
                                        {
                                            if (xr["AccountNum"].ToString() == hesabNum.fldShomareHesab)
                                            {
                                                hesabNumber = xr["ID"].ToString();
                                            }
                                        }
                                    }
                                }
                                var k = h.RegisterNewFicheByAccYearCostAndDiscount(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService), SodourFishId.ToString(), elamavarez.fldNameShakhs,
                                    fish.fldDate, hesabNumber, SodourFishId.ToString(),
                                    "کد ملی: " + elamavarez.fldShenaseMeli + "_" + fish.fldDesc, "", "", "", "", "", 8, 1, "", "", (long)fish.fldMablaghAvarezGerdShode, 0, (fish.fldJamKol - fish.fldMablaghAvarezGerdShode)/*fish.fldMablaghTakhfif*/, XDoc, XDoc1, XDoc2);
                                servic.UpdateSendToMali_Fish("SendToMali", true, SodourFishId, IP, out Err);
                                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + k + "-" + SodourFishId.ToString() + "\n");
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                MyErr = new WCF_Common.ClsError { ErrorMsg = "Fish_catch: " + x.InnerException != null ? x.InnerException.Message : x.Message, ErrorType = true };
            }
        }
        public void Calculation(int Count, int fldFormulId, int ShomareHesabCodeDaramadId, int CodeDaramadElamAvarezID,
            string CodeDescription, int FiscalYearId, out WCF_Common.ClsError MyErr)
        {
            MyErr = new WCF_Common.ClsError();
            try
            {
                var q = servic_ComPay.GetComputationFormulaWithFilter("fldId", fldFormulId.ToString(), "", "", 0, 0, IP, out Err_ComPay).FirstOrDefault();
                string Formul = q.fldFormule;
                string[] ReferenceSplit = q.fldLibrary.Split(';').Reverse().Skip(1).Reverse().ToArray();

                ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();

                CompilerParameters loParameters = new CompilerParameters();
                for (int i = 0; i < ReferenceSplit.Length; i++)
                {
                    if (ReferenceSplit[i] == "System.dll" || ReferenceSplit[i] == "System.Data.dll" || ReferenceSplit[i] == "System.Xml.dll" || ReferenceSplit[i] == "System.Core.dll")
                    {
                        loParameters.ReferencedAssemblies.Add(ReferenceSplit[i]);
                    }
                    else
                    {
                        var Path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Reference\" + ReferenceSplit[i]);
                        loParameters.ReferencedAssemblies.Add(Path);
                    }
                }
                loParameters.GenerateInMemory = true;

                CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Formul);
                Assembly loAssembly = loCompiled.CompiledAssembly;
                object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");
                object[] loCodeParms = new object[1];
                loCodeParms[0] = CodeDaramadElamAvarezID;
                object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                var Price = (long)loResult;

                servic.UpdateCodhayeDaramadiElamAvarez(CodeDaramadElamAvarezID, IDElamAvarez, CodeDescription, Convert.ToInt32(ShomareHesabCodeDaramadId), Price, Count, "",
                    User, Pass, Org, IP, out Err);
                if (Err.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "UpdateCodhayeDaramadiElamAvarez: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                    return;
                }
                Fish(CodeDaramadElamAvarezID, FiscalYearId, out Err_Com);
                if (Err_Com.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "Fish: " + Err_Com.ErrorMsg, ErrorType = Err_Com.ErrorType };
                    return;
                }
            }
            catch (Exception x)
            {
                MyErr = new WCF_Common.ClsError { ErrorMsg = "Calculation_catch: " + x.InnerException != null ? x.InnerException.Message : x.Message, ErrorType = true };
            }
        }
        public void SaveParamsValue(int ShomareHesabCodeDaramadId, int AshkhasId, int OrganId, List<WCF_Daramad.OBJ_ParametreSabet_Value> Values, long Price,
            int Count, decimal? Ton, string User, string Pass, int Org, int FiscalYearId, out WCF_Common.ClsError MyErr)
        {
            MyErr = new WCF_Common.ClsError();
            var CodeDescription = "";
            try
            {
                var ShomareHesabInf = servic.GetShomareHesabCodeDaramadDetail(ShomareHesabCodeDaramadId, FiscalYearId, IP, out Err);
                if (ShomareHesabInf.fldSharhCodDaramd != null)
                {
                    CodeDescription = ShomareHesabInf.fldSharhCodDaramd.Replace("|", "");
                    var SharhCodDaramdArray = ShomareHesabInf.fldSharhCodDaramd.Split('|');
                    for (byte i = 0; i < SharhCodDaramdArray.Length - 1; i++)
                    {
                        var s = servic.GetParametreSabetWithFilter("fldNameParametreEn", SharhCodDaramdArray[i], ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err)
                            .FirstOrDefault();
                        if (s != null)
                        {
                            CodeDescription = CodeDescription.Replace(SharhCodDaramdArray[i], "<" + s.fldId + ">");
                        }
                    }
                }
                else
                {
                    CodeDescription = ShomareHesabInf.fldDaramadTitle;
                }

                IDElamAvarez = servic.InsertElamAvarez(Convert.ToInt32(AshkhasId), false, OrganId, false, null, "", "",
                    User, Pass, Org, IP, out Err);
                if (Err.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertElamAvarez: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                    return;
                }
                var IDCodeDaramadElamAvarez = servic.InsertCodhayeDaramadiElamAvarez(IDElamAvarez, "", ShomareHesabCodeDaramadId, 0, 1, "",
                    User, Pass, Org, IP, out Err);
                if (Err.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertCodhayeDaramadiElamAvarez: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                    return;
                }

                if (Values != null)
                {
                    foreach (var item in Values)
                    {
                        servic.InsertParametreSabet_Value(IDElamAvarez, item.fldValue, item.fldParametreSabetId, IDCodeDaramadElamAvarez, "",
                            User, Pass, IP, out Err);
                        CodeDescription = CodeDescription.Replace("<" + item.fldParametreSabetId + ">", item.fldValue);
                        if (Err.ErrorType)
                        {
                            MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertParametreSabet_Value: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                            return;
                        }
                    }
                }
                //ذخیره مقادیر پارامترهای ثابت
                var StaticParams = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(), "", 0, IP, out Err)
                    .Where(l => l.fldNoe == false && l.fldTypeParametr == false).ToList();
                foreach (var item in StaticParams)
                {
                    var q = servic.GetParametreSabet_NerkhWithFilter("fldTarikh_ParametreSabetId", item.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                    servic.InsertParametreSabet_Value(IDElamAvarez, q.fldValue, item.fldId, IDCodeDaramadElamAvarez, "", User,
                        Pass, IP, out Err);
                    if (Err.ErrorType)
                    {
                        MyErr = new WCF_Common.ClsError { ErrorMsg = "InsertParametreSabet_Value2: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                        return;
                    }
                }
                //چک کردن دارا بودن فرمول                
                if (ShomareHesabInf.fldFormulMohasebatId != null)
                {
                    Calculation(Count, Convert.ToInt32(ShomareHesabInf.fldFormulMohasebatId), ShomareHesabCodeDaramadId, IDCodeDaramadElamAvarez,
                        CodeDescription, FiscalYearId, out Err_Com);
                    if (Err_Com.ErrorType)
                    {
                        MyErr = new WCF_Common.ClsError { ErrorMsg = "Calculation: " + Err_Com.ErrorMsg, ErrorType = Err_Com.ErrorType };
                        return;
                    }
                }
                else if (ShomareHesabInf.fldFormolsaz != null)
                {
                    calcFormulsaz(ShomareHesabInf.fldFormolsaz, ShomareHesabCodeDaramadId, IDCodeDaramadElamAvarez.ToString(), Count, CodeDescription, Ton, FiscalYearId, out Err_Com);
                }
                else
                {
                    servic.UpdateCodhayeDaramadiElamAvarez(IDCodeDaramadElamAvarez, IDElamAvarez, CodeDescription, ShomareHesabCodeDaramadId, Price, Count, "",
                        User, Pass, Org, IP, out Err);
                    if (Err.ErrorType)
                    {
                        MyErr = new WCF_Common.ClsError { ErrorMsg = "UpdateCodhayeDaramadiElamAvarez: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                        return;
                    }
                    Fish(IDCodeDaramadElamAvarez, FiscalYearId, out Err_Com);
                    if (Err_Com.ErrorType)
                    {
                        MyErr = new WCF_Common.ClsError { ErrorMsg = "Fish: " + Err_Com.ErrorMsg, ErrorType = Err_Com.ErrorType };
                        return;
                    }
                }
            }
            catch (Exception x)
            {
                MyErr = new WCF_Common.ClsError { ErrorMsg = "SaveParamsValue_catch: " + x.InnerException != null ? x.InnerException.Message : x.Message, ErrorType = true };
            }
        }
        public void calcFormulsaz(string Formul, int ShomareHesabCodeDaramadId, string CodeDaramadElamAvarezID, int Count, string CodeDescription, decimal? Ton, int FiscalYearId, out WCF_Common.ClsError MyErr)
        {
            MyErr = new WCF_Common.ClsError();
            try
            {
                string result = "";
                string temp = "";

                Formul = Formul.Replace("÷", "/");
                int Count1 = Formul.Count(k => k == ')');
                int Count2 = Formul.Count(k => k == '(');
                int Count3 = Formul.Count(k => k == ']');
                int Count4 = Formul.Count(k => k == '[');
                if (Count1 != Count2 || Count3 != Count4)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "تعداد پرانتزهای باز شده در فرمول ساز با تعداد پرانتزهای بسته شده یکی نیست.", ErrorType = true };
                    return;
                }

                var s = Formul.Split(';');
                for (int i = 0; i < s.Length - 1; i++)
                {
                    if (s[i] == "then" || s[i] == "else")
                    {
                        s[i] = ",";
                    }
                    else
                    {
                        if (s[i] == "ton")
                        {
                            s[i] = (Ton / 1000).ToString();
                        }
                        else if (s[i] == "kilo")
                        {
                            s[i] = Ton.ToString();
                        }
                        else
                        {
                            var f = servic.GetParametreSabetWithFilter("fldNameParametreEn", s[i], ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                            if (f != null)
                            {
                                s[i] = f.fldId.ToString();
                                if (CodeDaramadElamAvarezID != "")
                                {
                                    var f11 = servic.GetParametreSabet_ValueWithFilter("fldCodeDaramadElamAvarez_fldParametreSabetId", CodeDaramadElamAvarezID, f.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                    if (f11 != null)
                                        s[i] = f11.fldValue.ToString();
                                }
                            }
                            else
                            {
                                var f1 = servic.GetParametreOmoomiWithFilter("fldNameParametreEn", s[i], 0, IP, out Err).FirstOrDefault();
                                if (f1 != null)
                                {
                                    s[i] = "0";
                                    var f11 = servic.GetParametreOmoomi_ValueWithFilter("LastDate", f1.fldId.ToString(), "", 0, IP, out Err).FirstOrDefault();
                                    if (f11 != null)
                                        s[i] = f11.fldValue.ToString();
                                }
                            }
                        }
                    }
                    temp = temp + s[i];
                }
                var sss = Summary(temp).Replace('[', '(').Replace(']', ')');
                result = Calculate(sss).ToString();

                long Price = Convert.ToInt64(result.Split('.')[0]);


                servic.UpdateCodhayeDaramadiElamAvarez(Convert.ToInt32(CodeDaramadElamAvarezID), IDElamAvarez, CodeDescription, Convert.ToInt32(ShomareHesabCodeDaramadId), Price, Count, "",
                    User, Pass, Org, IP, out Err);
                if (Err.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "UpdateCodhayeDaramadiElamAvarez: " + Err.ErrorMsg, ErrorType = Err.ErrorType };
                    return;
                }
                Fish(Convert.ToInt32(CodeDaramadElamAvarezID), FiscalYearId, out Err_Com);
                if (Err_Com.ErrorType)
                {
                    MyErr = new WCF_Common.ClsError { ErrorMsg = "Fish: " + Err_Com.ErrorMsg, ErrorType = Err_Com.ErrorType };
                    return;
                }
            }
            catch (Exception x)
            {
                MyErr = new WCF_Common.ClsError { ErrorMsg = "Calculation_catch: " + x.InnerException != null ? x.InnerException.Message : x.Message, ErrorType = true };
            }
        }

        public static double? Calculate(string Formoul)
        {
            Models.FM F = new Models.FM();
            int num = 12;
            double result;
            if (F.Neu(Formoul, out result) || num != 21)
                return new double?(result);
            return new double?();
        }

        private static int CountOf(string STR, char CHAR)
        {
            int num1 = 0;
            foreach (int num2 in STR)
            {
                if (num2 == (int)CHAR)
                    ++num1;
            }
            return num1;
        }

        private static bool Removeable(string temp)
        {
            if (temp.Length < 1 || (int)temp[0] != 40 && (int)temp[0] != 91 || (int)temp[temp.Length - 1] != 41 && (int)temp[temp.Length - 1] != 93)
                return false;
            int num = 0;
            for (int index = 0; index < temp.Length; ++index)
            {
                if ((int)temp[index] == 40 || (int)temp[index] == 91)
                    ++num;
                if ((int)temp[index] == 41 || (int)temp[index] == 93)
                {
                    --num;
                    if (num == 0 && index < temp.Length - 1)
                        return false;
                    if (num == 0 && index == temp.Length - 1)
                        return true;
                }
            }
            return true;
        }
        private static void spliteStatment(string temp, out string _prefix, out string _if, out string _then, out string _else, out string _postfix)
        {
            bool flag = false;
            do
            {
                if (temp.Length > 0 && (flag = Removeable(temp)))
                    temp = temp.Substring(1, temp.Length - 2);
            }
            while (flag);
            _prefix = "";
            if (!temp.StartsWith("if("))
            {
                for (int index = 2; index < temp.Length; ++index)
                {
                    if ((int)temp[index - 2] == 105 && (int)temp[index - 1] == 102 && (int)temp[index] == 40)
                    {
                        _prefix = temp.Substring(0, index - 2);
                        if (((object)temp.Substring(index - 2)).ToString() != "")
                        {
                            temp = temp.Substring(index - 2);
                            break;
                        }
                    }
                }
            }
            do
            {
                if (temp.Length > 0 && (flag = Removeable(temp)))
                    temp = temp.Substring(1, temp.Length - 2);
            }
            while (flag);
            int num = 0;
            int index1;
            for (index1 = 2; index1 < temp.Length; ++index1)
            {
                if ((int)temp[index1] == 40 || (int)temp[index1] == 91)
                    ++num;
                if ((int)temp[index1] == 41 || (int)temp[index1] == 93)
                    --num;
                if ((int)temp[index1] == 44 && num == 0)
                    break;
            }
            _if = temp.Substring(temp.IndexOf("if(") + 3, index1 - 4);
            int index2;
            for (index2 = index1 + 1; index2 < temp.Length; index2++)
            {
                if ((int)temp[index2] == 40 || (int)temp[index2] == 91)
                    num++;
                if ((int)temp[index2] == 41 || (int)temp[index2] == 93)
                    num--;
                if ((int)temp[index2] == 44 && num == 0)
                    break;
            }
            _then = temp.Substring(temp.IndexOf(_if) + _if.Length + 2, index2 - (temp.IndexOf(_if) + _if.Length + 2));
            _postfix = "";
            if (index2 == temp.Length)
            {
                _else = "";
            }
            else
            {
                int index3;
                for (index3 = index2 + 1; index3 < temp.Length; ++index3)
                {
                    if ((int)temp[index3] == 40 || (int)temp[index3] == 91)
                        ++num;
                    else if (((int)temp[index3] == 41 || (int)temp[index3] == 93) && num != 0)
                        --num;
                    else if (((int)temp[index3] == 41 || (int)temp[index3] == 93) && num == 0)
                        break;
                }
                _else = temp.Substring(temp.IndexOf("if(" + _if + "," + _then) + _if.Length + _then.Length + 7, index3 - (temp.IndexOf("if(" + _if + "," + _then) + _if.Length + _then.Length + 7));
                _postfix = temp.Substring(index3);
            }
        }

        private static bool Condition(string _if)
        {
            if (_if.Contains("!="))
            {
                string[] strArray = _if.Split(new string[1]
        {
          "!="
        }, StringSplitOptions.None);
                double? nullable1 = Calculate(strArray[0]);
                double? nullable2 = Calculate(strArray[1]);
                return (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) != 0;
            }
            else if (_if.Contains("="))
            {
                if (CountOf(_if, '=') > 1)
                    throw new Exception("Error in =");
                string[] strArray = _if.Split(new string[1]
        {
          "="
        }, StringSplitOptions.None);
                double? nullable1 = Calculate(strArray[0]);
                double? nullable2 = Calculate(strArray[1]);
                return (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) != 0;
            }
            else if (_if.Contains("<"))
            {
                if (CountOf(_if, '<') > 2)
                    throw new Exception("Error in <");
                string[] strArray = _if.Split(new string[1]
        {
          "<"
        }, StringSplitOptions.None);
                if (CountOf(_if, '<') == 1)
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    return (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0;
                }
                else
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    int num;
                    if ((nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0)
                    {
                        nullable1 = Calculate(strArray[1]);
                        nullable2 = Calculate(strArray[2]);
                        num = (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) == 0 ? 1 : 0;
                    }
                    else
                        num = 1;
                    return num == 0;
                }
            }
            else
            {
                if (!_if.Contains(">"))
                    return true;
                if (CountOf(_if, '>') > 2)
                    throw new Exception("Error in >");
                string[] strArray = _if.Split(new string[1]
        {
          ">"
        }, StringSplitOptions.None);
                if (CountOf(_if, '>') == 1)
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    return (nullable1.GetValueOrDefault() <= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0;
                }
                else
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    int num;
                    if ((nullable1.GetValueOrDefault() <= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0)
                    {
                        nullable1 = Calculate(strArray[1]);
                        nullable2 = Calculate(strArray[2]);
                        num = (nullable1.GetValueOrDefault() <= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) == 0 ? 1 : 0;
                    }
                    else
                        num = 1;
                    return num == 0;
                }
            }
        }
        private static string Summary(string temp)
        {
            string _prefix = "";
            string _postfix = "";
            string str;
            string _else = str = temp;
            string _then = str;
            string _if = str;
            if (!temp.Trim().Contains("if"))
                return temp;
            spliteStatment(temp, out _prefix, out _if, out _then, out _else, out _postfix);
            if (Condition(Summary(_if)))
                return Summary(_prefix + _then + _postfix);
            else
                return Summary(_prefix + _else + _postfix);
        }
    }
}