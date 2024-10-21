using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace NewFMS.Areas.Chek.Controllers
{
    public class RptSodorChekController : Controller
    {
        //
        // GET: /Chek/RptSodorChek/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_Chek.ChekService servicChek = new WCF_Chek.ChekService();
        WCF_Chek.ClsError ErrChek = new WCF_Chek.ClsError();
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
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetBankWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetShobe()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSHobeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetUser()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetUserWithFilter("", "","", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameEmployee });
            return this.Store(q);
        }
        public ActionResult ReadSodorChek(StoreRequestParameters parameters)
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
                        case "fldTarikhVosol":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhVosol";
                            break;
                        case "fldCodeSerialCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeSerialCheck";
                            break;
                        case "fldDarVajh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarVajh";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablagh";
                            break;
                        case "fldMoshakhaseDasteCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMoshakhaseDasteCheck";
                            break;
                        case "fldBabat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBabat";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "Name_Family":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Name_Family";
                            break;
                        case "TarikhSabt":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "TarikhSabt";
                            break;
                    }
                    if (data != null)
                        data1 = servicChek.GetSodorCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
                    else
                        data = servicChek.GetSodorCheckWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicChek.GetSodorCheckWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
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
        public ActionResult Print(string containerId, string AzTarikh, string TaTarikh, string AzTarikh_V, string TaTarikh_V, string AzMablagh, string TaMablagh, string BankName, string AzSerial, string TaSerial, string ShobeName, string DarVajh, string Babat, string user)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.AzTarikh = AzTarikh;
            result.ViewBag.TaTarikh = TaTarikh;
            result.ViewBag.AzTarikh_V = AzTarikh_V;
            result.ViewBag.TaTarikh_V = TaTarikh_V; result.ViewBag.AzMablagh = AzMablagh;
            result.ViewBag.TaMablagh = TaMablagh; result.ViewBag.BankName = BankName;
            result.ViewBag.AzSerial = AzSerial; result.ViewBag.TaSerial = TaSerial;
            result.ViewBag.ShobeName = ShobeName; result.ViewBag.DarVajh = DarVajh;
            result.ViewBag.Babat = Babat; result.ViewBag.user = user;
            return result;
        }
        public ActionResult GeneratePDFSodorChek(string AzTarikh, string TaTarikh, string AzTarikh_V, string TaTarikh_V, string AzMablagh, string TaMablagh, string BankName, string AzSerial, string TaSerial, string ShobeName, string DarVajh, string Babat, string user)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (AzMablagh == "0")
                    AzMablagh = "";
                if (TaMablagh == "0")
                    TaMablagh = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1.RptSodorChekDataTable sodorchek = new DataSet.DataSet1.RptSodorChekDataTable();
                int OrganId = Convert.ToInt32(Session["OrganId"]);
                string commandText = "select * from (SELECT     chk.tblSodorCheck.fldId, chk.tblSodorCheck.fldIdDasteCheck, chk.tblSodorCheck.fldTarikhVosol," +
                " chk.tblSodorCheck.fldCodeSerialCheck, chk.tblSodorCheck.fldBabat, chk.tblSodorCheck.fldBabatFlag,  chk.tblSodorCheck.fldOrganId,"+
                " chk.tblSodorCheck.fldMablagh, chk.tblSodorCheck.fldDesc, dbo.Fn_AssembelyMiladiToShamsi(chk.tblSodorCheck.fldDate) AS TarikhSabt,"+
                " chk.tblSodorCheck.fldDate, chk.tblSodorCheck.fldUserId, chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe,"+ 
                " Com.tblBank.fldBankName, tblEmployee_1.fldName + ' ' + tblEmployee_1.fldFamily AS fldDarVajh, tblEmployee_1.fldCodemeli AS ShomareMeli,"+
                " Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId, Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family,"+
                " CASE fldBabatFlag WHEN 1 THEN chk.tblSodorCheck.fldBabat ELSE '' END AS BabatText, Com.date_to_words(chk.tblSodorCheck.fldTarikhVosol) AS TarikhShamsi,"+
                " chk.Num_ToWords(chk.tblSodorCheck.fldMablagh) AS MablaghBeHorof , ISNULL( ( SELECT TOP(1) fldvaziat FROM chk.tblCheckStatus"+
                " WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),4)as fldvaziat,"+
                " ISNULL( ( SELECT TOP(1) CASE chk.tblCheckStatus.fldvaziat WHEN 1 THEN N'در انتظار وصول'  WHEN 2 THEN N'وصول شده' WHEN 3 THEN N'برگشت خورده'"+
                " WHEN 4 THEN N'حقوقی شده'  when 5  THEN N'عودت داده شده' END FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),"+
                " N'در انتظار وصول') AS NameVaziat,ISNULL( ( SELECT TOP(1) tblCheckStatus.fldTarikh FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId"+
                " ORDER BY fldId desc),N'') fldTarikhVaziat FROM chk.tblSodorCheck INNER JOIN"+
                " chk.tblDasteCheck ON chk.tblSodorCheck.fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN"+
                " Com.tblShomareHesabeOmoomi ON chk.tblDasteCheck.fldIdShomareHesab = Com.tblShomareHesabeOmoomi.fldId INNER JOIN"+
                " Com.tblSHobe ON Com.tblSHobe.fldId = Com.tblShomareHesabeOmoomi.fldShobeId INNER JOIN"+
                " Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN"+
                " Com.tblUser ON Com.tblUser.fldId = chk.tblSodorCheck.fldUserId INNER JOIN"+
                " Com.tblEmployee ON Com.tblUser.fldEmployId = Com.tblEmployee.fldId INNER JOIN"+
                " Com.tblAshkhas ON chk.tblSodorCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN"+
                " Com.tblEmployee AS tblEmployee_1 ON Com.tblAshkhas.fldHaghighiId = tblEmployee_1.fldId"+				 
				" UNION SELECT chk.tblSodorCheck.fldId, chk.tblSodorCheck.fldIdDasteCheck, chk.tblSodorCheck.fldTarikhVosol, chk.tblSodorCheck.fldCodeSerialCheck,"+
                " chk.tblSodorCheck.fldBabat, chk.tblSodorCheck.fldBabatFlag,  chk.tblSodorCheck.fldOrganId, chk.tblSodorCheck.fldMablagh, chk.tblSodorCheck.fldDesc,"+
                " dbo.Fn_AssembelyMiladiToShamsi(chk.tblSodorCheck.fldDate) AS TarikhSabt, chk.tblSodorCheck.fldDate, chk.tblSodorCheck.fldUserId,"+
                " chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe, Com.tblBank.fldBankName, Com.tblAshkhaseHoghoghi.fldName AS fldDarVajh,"+
                " Com.tblAshkhaseHoghoghi.fldShenaseMelli AS ShomareMeli, Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId,"+
                " Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family, CASE fldBabatFlag WHEN 1 THEN chk.tblSodorCheck.fldBabat ELSE '' END AS BabatText,"+ 
                " Com.date_to_words(chk.tblSodorCheck.fldTarikhVosol) AS TarikhShamsi, chk.Num_ToWords(chk.tblSodorCheck.fldMablagh) AS MablaghBeHorof,"+ 
                " ISNULL( ( SELECT TOP(1) fldvaziat FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),4)as fldvaziat,"+
				" ISNULL( ( SELECT TOP(1) CASE chk.tblCheckStatus.fldvaziat WHEN 1 THEN N'در انتظار وصول'  WHEN 2 THEN N'وصول شده' WHEN 3 THEN N'برگشت خورده'"+
                " WHEN 4 THEN N'حقوقی شده'  when 5  THEN N'عودت داده شده' END FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),"+
                " N'در انتظار وصول') AS NameVaziat, ISNULL( ( SELECT TOP(1) tblCheckStatus.fldTarikh FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId"+
                " ORDER BY fldId desc),N'') fldTarikhVaziat FROM chk.tblSodorCheck INNER JOIN"+
                " chk.tblDasteCheck ON chk.tblSodorCheck.fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN"+
                " Com.tblShomareHesabeOmoomi ON Com.tblShomareHesabeOmoomi.fldId = chk.tblDasteCheck.fldIdShomareHesab INNER JOIN"+
                " Com.tblSHobe ON Com.tblShomareHesabeOmoomi.fldShobeId = Com.tblSHobe.fldId INNER JOIN"+
                " Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN"+
                " Com.tblUser ON Com.tblUser.fldId = chk.tblSodorCheck.fldUserId INNER JOIN"+
                " Com.tblAshkhas ON chk.tblSodorCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN"+
                " Com.tblAshkhaseHoghoghi ON Com.tblAshkhas.fldHoghoghiId = Com.tblAshkhaseHoghoghi.fldId INNER JOIN"+
                " Com.tblEmployee ON Com.tblEmployee.fldId = Com.tblUser.fldEmployId)p"
                + where(AzTarikh, TaTarikh, AzTarikh_V, TaTarikh_V, AzMablagh, TaMablagh, BankName, AzSerial, TaSerial, ShobeName, DarVajh, Babat, user,OrganId);
//                string commandText = "select * from( SELECT        chk.tblSodorCheck.fldId, chk.tblSodorCheck.fldIdDasteCheck, chk.tblSodorCheck.fldTarikhVosol, chk.tblSodorCheck.fldCodeSerialCheck, chk.tblSodorCheck.fldBabat, chk.tblSodorCheck.fldBabatFlag,  chk.tblSodorCheck.fldOrganId, "+
//                        " chk.tblSodorCheck.fldMablagh, chk.tblSodorCheck.fldDesc, dbo.Fn_AssembelyMiladiToShamsi(chk.tblSodorCheck.fldDate) AS TarikhSabt, chk.tblSodorCheck.fldDate, chk.tblSodorCheck.fldUserId,  " +
//                       "   chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe, Com.tblBank.fldBankName, tblEmployee_1.fldName + ' ' + tblEmployee_1.fldFamily AS fldDarVajh,  "+
//                        "  tblEmployee_1.fldCodemeli AS ShomareMeli, Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId,  "+
//                        "  Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family, CASE fldBabatFlag WHEN 1 THEN chk.tblSodorCheck.fldBabat ELSE '' END AS BabatText,  "+
//                       "   Com.date_to_words(chk.tblSodorCheck.fldTarikhVosol) AS TarikhShamsi, chk.Num_ToWords(chk.tblSodorCheck.fldMablagh) AS MablaghBeHorof , "+
//                            " 	ISNULL( ( SELECT TOP(1) fldvaziat FROM chk.tblCheckStatus "+
//                      "  WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),4)as fldvaziat, "+
//                             "           ISNULL( ( SELECT TOP(1) CASE chk.tblCheckStatus.fldvaziat WHEN 1 THEN N'در انتظار وصول'  WHEN 2 THEN N'وصول شده' WHEN 3 THEN N'برگشت خورده' WHEN 4 THEN N'حقوقی شده'  when 5  THEN N'عودت داده شده' END FROM chk.tblCheckStatus "+
//                      "  WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),N'در انتظار وصول') AS NameVaziat,  "+
//                          "                       ISNULL( ( SELECT TOP(1) tblCheckStatus.fldTarikh FROM chk.tblCheckStatus "+
//                      "  WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),N'') fldTarikhVaziat "+
//" d  chk.tblSodorCheck INNER JOIN "+
//                     "     chk.tblDasteCheck ON chk.tblSodorCheck.fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN "+
//                      "    Com.tblShomareHesabeOmoomi ON chk.tblDasteCheck.fldIdShomareHesab = Com.tblShomareHesabeOmoomi.fldId INNER JOIN "+
//                      "    Com.tblSHobe ON Com.tblSHobe.fldId = Com.tblShomareHesabeOmoomi.fldShobeId INNER JOIN "+
//                      "    Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN "+
//                      "    Com.tblUser ON Com.tblUser.fldId = chk.tblSodorCheck.fldUserId INNER JOIN "+
//                      "    Com.tblEmployee ON Com.tblUser.fldEmployId = Com.tblEmployee.fldId INNER JOIN "+
//                      "    Com.tblAshkhas ON chk.tblSodorCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN "+
//                      "    Com.tblEmployee AS tblEmployee_1 ON Com.tblAshkhas.fldHaghighiId = tblEmployee_1.fldId  "+
//                      "       UNION all      "+
//                      "     SELECT        chk.tblSodorCheck.fldId, chk.tblSodorCheck.fldIdDasteCheck, chk.tblSodorCheck.fldTarikhVosol, chk.tblSodorCheck.fldCodeSerialCheck, chk.tblSodorCheck.fldBabat, chk.tblSodorCheck.fldBabatFlag,  chk.tblSodorCheck.fldOrganId,  "+
//                      "    chk.tblSodorCheck.fldMablagh, chk.tblSodorCheck.fldDesc, dbo.Fn_AssembelyMiladiToShamsi(chk.tblSodorCheck.fldDate) AS TarikhSabt, chk.tblSodorCheck.fldDate, chk.tblSodorCheck.fldUserId,  " +
//                     "     chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe, Com.tblBank.fldBankName, Com.tblAshkhaseHoghoghi.fldName AS fldDarVajh,  "+
//                     "     Com.tblAshkhaseHoghoghi.fldShenaseMelli AS ShomareMeli, Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId,  "+
//                     "     Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family, CASE fldBabatFlag WHEN 1 THEN chk.tblSodorCheck.fldBabat ELSE '' END AS BabatText,  "+
//                     "     Com.date_to_words(chk.tblSodorCheck.fldTarikhVosol) AS TarikhShamsi, chk.Num_ToWords(chk.tblSodorCheck.fldMablagh) AS MablaghBeHorof,  "+
//                         "                         		ISNULL( ( SELECT TOP(1) fldvaziat FROM chk.tblCheckStatus "+
//                    "    WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),4)as fldvaziat, "+
//                     "                   ISNULL( ( SELECT TOP(1) CASE chk.tblCheckStatus.fldvaziat WHEN 1 THEN N'در انتظار وصول'  WHEN 2 THEN N'وصول شده' WHEN 3 THEN N'برگشت خورده' WHEN 4 THEN N'حقوقی شده'  when 5  THEN N'عودت داده شده' END FROM chk.tblCheckStatus "+
//                    "    WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),N'در انتظار وصول') AS NameVaziat,  "+
//                            "                     ISNULL( ( SELECT TOP(1) tblCheckStatus.fldTarikh FROM chk.tblCheckStatus "+
//                    "    WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),N'') fldTarikhVaziat "+
//" FROM            chk.tblSodorCheck INNER JOIN "+
//                  "        chk.tblDasteCheck ON chk.tblSodorCheck.fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN "+
//                    "      Com.tblShomareHesabeOmoomi ON Com.tblShomareHesabeOmoomi.fldId = chk.tblDasteCheck.fldIdShomareHesab INNER JOIN "+
//                    "      Com.tblSHobe ON Com.tblShomareHesabeOmoomi.fldShobeId = Com.tblSHobe.fldId INNER JOIN "+
//                    "      Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN "+
//                     "     Com.tblUser ON Com.tblUser.fldId = chk.tblSodorCheck.fldUserId INNER JOIN "+
//                     "     Com.tblAshkhas ON chk.tblSodorCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN "+
//                    "      Com.tblAshkhaseHoghoghi ON Com.tblAshkhas.fldHoghoghiId = Com.tblAshkhaseHoghoghi.fldId INNER JOIN "+
//                    "      Com.tblEmployee ON Com.tblEmployee.fldId = Com.tblUser.fldEmployId)p  " 
//                    + where(AzTarikh, TaTarikh, AzTarikh_V, TaTarikh_V, AzMablagh, TaMablagh, BankName, AzSerial, TaSerial, ShobeName, DarVajh, Babat, user,OrganId);

                string ConnectionString = ConfigurationManager.ConnectionStrings["RasaNewFMSConnectionString"].ConnectionString;
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand com = new SqlCommand(commandText, con);
                com.CommandTimeout = 200000;
                var dr = com.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    dt.RptSodorChek.Rows.Add();
                    dt.RptSodorChek[i].fldId =Convert.ToInt32( dr["fldId"]);
                    dt.RptSodorChek[i].fldBabat = dr["fldBabat"].ToString();
                    dt.RptSodorChek[i].fldBankName = dr["fldBankName"].ToString();
                    dt.RptSodorChek[i].fldCodeSerialCheck = dr["fldCodeSerialCheck"].ToString();
                    dt.RptSodorChek[i].fldDarVajh = dr["fldDarVajh"].ToString();
                    dt.RptSodorChek[i].fldMablagh =Convert.ToInt64(dr["fldMablagh"]);
                    dt.RptSodorChek[i].fldMoshakhaseDasteCheck = dr["fldMoshakhaseDasteCheck"].ToString();
                    dt.RptSodorChek[i].fldTarikhVosol = dr["fldTarikhVosol"].ToString();
                    dt.RptSodorChek[i].Name_Family = dr["Name_Family"].ToString();
                    dt.RptSodorChek[i].NameShobe = dr["NameShobe"].ToString();
                    dt.RptSodorChek[i].TarikhSabt = dr["TarikhSabt"].ToString();
                    i = i + 1;
                }
                con.Close();

                string Parametr = "";
                if (AzTarikh != "")
                    Parametr = " از تاریخ " + AzTarikh + "تا تاریخ " + TaTarikh+", ";
                if (AzTarikh_V != "")
                    Parametr = Parametr + " در تاریخ وصول " + AzTarikh_V + "تا تاریخ " + TaTarikh_V + ", ";
                if (AzMablagh != "")
                    Parametr = Parametr + " در مبلغ " + AzMablagh + "تا مبلغ " + TaMablagh + ", ";
                /*if (BankName != "")
                    Parametr = Parametr + " بانک " + BankName + ", ";
                if(ShobeName!="")
                    Parametr = Parametr + " شعبه " + ShobeName + ", ";*/
                if (AzSerial != "")
                    Parametr = Parametr + " از سریال " + AzSerial + "تا سریال " + TaSerial + ", ";
                /*if (DarVajh != "")
                    Parametr = Parametr + " در وجه " + DarVajh + ", ";
                if (Babat != "")
                    Parametr = Parametr + " بابت " + Babat + ", ";*/
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Chek\RptSodorChek.frx");
                Report.RegisterData(dt, "rasaFMSDaramad");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Parametr", Parametr);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult FullSearch(string AzTarikh, string TaTarikh, string AzTarikh_V, string TaTarikh_V, string AzMablagh, string TaMablagh, string BankName, string AzSerial, string TaSerial, string ShobeName, string DarVajh, string Babat, string user)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int OrganId = Convert.ToInt32(Session["OrganId"]);
            if (AzMablagh == "0")
                AzMablagh = "";
            if (TaMablagh == "0")
                TaMablagh = "";

            string commandText = "select * from (SELECT     chk.tblSodorCheck.fldId, chk.tblSodorCheck.fldIdDasteCheck, chk.tblSodorCheck.fldTarikhVosol,"+
                " chk.tblSodorCheck.fldCodeSerialCheck, chk.tblSodorCheck.fldBabat, chk.tblSodorCheck.fldBabatFlag,  chk.tblSodorCheck.fldOrganId,"+
                " chk.tblSodorCheck.fldMablagh, chk.tblSodorCheck.fldDesc, dbo.Fn_AssembelyMiladiToShamsi(chk.tblSodorCheck.fldDate) AS TarikhSabt,"+
                " chk.tblSodorCheck.fldDate, chk.tblSodorCheck.fldUserId, chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe,"+ 
                " Com.tblBank.fldBankName, tblEmployee_1.fldName + ' ' + tblEmployee_1.fldFamily AS fldDarVajh, tblEmployee_1.fldCodemeli AS ShomareMeli,"+
                " Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId, Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family,"+
                " CASE fldBabatFlag WHEN 1 THEN chk.tblSodorCheck.fldBabat ELSE '' END AS BabatText, Com.date_to_words(chk.tblSodorCheck.fldTarikhVosol) AS TarikhShamsi,"+
                " chk.Num_ToWords(chk.tblSodorCheck.fldMablagh) AS MablaghBeHorof , ISNULL( ( SELECT TOP(1) fldvaziat FROM chk.tblCheckStatus"+
                " WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),4)as fldvaziat,"+
                " ISNULL( ( SELECT TOP(1) CASE chk.tblCheckStatus.fldvaziat WHEN 1 THEN N'در انتظار وصول'  WHEN 2 THEN N'وصول شده' WHEN 3 THEN N'برگشت خورده'"+
                " WHEN 4 THEN N'حقوقی شده'  when 5  THEN N'عودت داده شده' END FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),"+
                " N'در انتظار وصول') AS NameVaziat,ISNULL( ( SELECT TOP(1) tblCheckStatus.fldTarikh FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId"+
                " ORDER BY fldId desc),N'') fldTarikhVaziat FROM chk.tblSodorCheck INNER JOIN"+
                " chk.tblDasteCheck ON chk.tblSodorCheck.fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN"+
                " Com.tblShomareHesabeOmoomi ON chk.tblDasteCheck.fldIdShomareHesab = Com.tblShomareHesabeOmoomi.fldId INNER JOIN"+
                " Com.tblSHobe ON Com.tblSHobe.fldId = Com.tblShomareHesabeOmoomi.fldShobeId INNER JOIN"+
                " Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN"+
                " Com.tblUser ON Com.tblUser.fldId = chk.tblSodorCheck.fldUserId INNER JOIN"+
                " Com.tblEmployee ON Com.tblUser.fldEmployId = Com.tblEmployee.fldId INNER JOIN"+
                " Com.tblAshkhas ON chk.tblSodorCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN"+
                " Com.tblEmployee AS tblEmployee_1 ON Com.tblAshkhas.fldHaghighiId = tblEmployee_1.fldId"+				 
				" UNION SELECT chk.tblSodorCheck.fldId, chk.tblSodorCheck.fldIdDasteCheck, chk.tblSodorCheck.fldTarikhVosol, chk.tblSodorCheck.fldCodeSerialCheck,"+
                " chk.tblSodorCheck.fldBabat, chk.tblSodorCheck.fldBabatFlag,  chk.tblSodorCheck.fldOrganId, chk.tblSodorCheck.fldMablagh, chk.tblSodorCheck.fldDesc,"+
                " dbo.Fn_AssembelyMiladiToShamsi(chk.tblSodorCheck.fldDate) AS TarikhSabt, chk.tblSodorCheck.fldDate, chk.tblSodorCheck.fldUserId,"+
                " chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe, Com.tblBank.fldBankName, Com.tblAshkhaseHoghoghi.fldName AS fldDarVajh,"+
                " Com.tblAshkhaseHoghoghi.fldShenaseMelli AS ShomareMeli, Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId,"+
                " Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family, CASE fldBabatFlag WHEN 1 THEN chk.tblSodorCheck.fldBabat ELSE '' END AS BabatText,"+ 
                " Com.date_to_words(chk.tblSodorCheck.fldTarikhVosol) AS TarikhShamsi, chk.Num_ToWords(chk.tblSodorCheck.fldMablagh) AS MablaghBeHorof,"+ 
                " ISNULL( ( SELECT TOP(1) fldvaziat FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),4)as fldvaziat,"+
				" ISNULL( ( SELECT TOP(1) CASE chk.tblCheckStatus.fldvaziat WHEN 1 THEN N'در انتظار وصول'  WHEN 2 THEN N'وصول شده' WHEN 3 THEN N'برگشت خورده'"+
                " WHEN 4 THEN N'حقوقی شده'  when 5  THEN N'عودت داده شده' END FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId ORDER BY fldId desc),"+
                " N'در انتظار وصول') AS NameVaziat, ISNULL( ( SELECT TOP(1) tblCheckStatus.fldTarikh FROM chk.tblCheckStatus WHERE fldSodorCheckId=chk.tblSodorCheck.fldId"+
                " ORDER BY fldId desc),N'') fldTarikhVaziat FROM chk.tblSodorCheck INNER JOIN"+
                " chk.tblDasteCheck ON chk.tblSodorCheck.fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN"+
                " Com.tblShomareHesabeOmoomi ON Com.tblShomareHesabeOmoomi.fldId = chk.tblDasteCheck.fldIdShomareHesab INNER JOIN"+
                " Com.tblSHobe ON Com.tblShomareHesabeOmoomi.fldShobeId = Com.tblSHobe.fldId INNER JOIN"+
                " Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN"+
                " Com.tblUser ON Com.tblUser.fldId = chk.tblSodorCheck.fldUserId INNER JOIN"+
                " Com.tblAshkhas ON chk.tblSodorCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN"+
                " Com.tblAshkhaseHoghoghi ON Com.tblAshkhas.fldHoghoghiId = Com.tblAshkhaseHoghoghi.fldId INNER JOIN"+
                " Com.tblEmployee ON Com.tblEmployee.fldId = Com.tblUser.fldEmployId)p"
                + where(AzTarikh, TaTarikh, AzTarikh_V, TaTarikh_V, AzMablagh, TaMablagh, BankName, AzSerial, TaSerial, ShobeName, DarVajh, Babat, user,OrganId);

            //string commandText = "select * from( SELECT   [Drd].[tblCheck].fldId,  [Drd].[tblCheck].fldIdDasteCheck,  [Drd].[tblCheck].fldTarikhSarResid fldTarikhVosol, " +
            //            " [Drd].[tblCheck].fldShomareSanad fldCodeSerialCheck,  [Drd].[tblCheck].fldBabat,  [Drd].[tblCheck].fldBabatFlag, " +
            //            " [Drd].[tblCheck].fldOrganId,[Drd].[tblCheck].fldMablaghSanad fldMablagh,  [Drd].[tblCheck].fldDesc, " +
            //            "  fldTarikhAkhz AS TarikhSabt,  [Drd].[tblCheck].fldDate,  [Drd].[tblCheck].fldUserId,  " +
            //            "  chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe, Com.tblBank.fldBankName, tblEmployee_1.fldName + ' ' + tblEmployee_1.fldFamily AS fldDarVajh,  " +
            //             " tblEmployee_1.fldCodemeli AS ShomareMeli, Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId,  " +
            //             " Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family, CASE fldBabatFlag WHEN 1 THEN  [Drd].[tblCheck].fldBabat ELSE '' END AS BabatText,  " +
            //            "  Com.date_to_words( [Drd].[tblCheck].fldTarikhSarResid) AS TarikhShamsi, chk.Num_ToWords( [Drd].[tblCheck].fldMablaghSanad) AS MablaghBeHorof , " +
            //            "  [tblCheck].fldStatus as fldvaziat,CASE WHEN [tblCheck].fldStatus = 1 THEN N'در انتظار وصول' WHEN [tblCheck].fldStatus = 2 THEN N'وصول شده'  " +
            //            "  WHEN [tblCheck].fldStatus = 3 THEN N'برگشت خورده' WHEN [tblCheck].fldStatus = 4 THEN N'حقوقی شده' WHEN [tblCheck].fldStatus= 5 THEN N'عودت داده شده' END  AS NameVaziat,  " +
            //            "   ISNULL( fldDateStatus,N'') fldTarikhVaziat " +
            //           "    FROM             [Drd].[tblCheck] INNER JOIN " +
            //            "  chk.tblDasteCheck ON  [Drd].[tblCheck].fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN " +
            //            "  Com.tblShomareHesabeOmoomi ON chk.tblDasteCheck.fldIdShomareHesab = Com.tblShomareHesabeOmoomi.fldId INNER JOIN " +
            //           "   Com.tblSHobe ON Com.tblSHobe.fldId = Com.tblShomareHesabeOmoomi.fldShobeId INNER JOIN " +
            //             " Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN " +
            //            "  Com.tblUser ON Com.tblUser.fldId =  [Drd].[tblCheck].fldUserId INNER JOIN " +
            //             " Com.tblEmployee ON Com.tblUser.fldEmployId = Com.tblEmployee.fldId INNER JOIN " +
            //             " Com.tblAshkhas ON  [Drd].[tblCheck].fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN " +
            //             " Com.tblEmployee AS tblEmployee_1 ON Com.tblAshkhas.fldHaghighiId = tblEmployee_1.fldId " +
            //             "  UNION    " +
            //             " SELECT     [Drd].[tblCheck].fldId,  [Drd].[tblCheck].fldIdDasteCheck,  [Drd].[tblCheck].fldTarikhSarResid fldTarikhVosol,  " +
            //            " [Drd].[tblCheck].fldShomareSanad fldCodeSerialCheck,  [Drd].[tblCheck].fldBabat,  [Drd].[tblCheck].fldBabatFlag,    " +
            //            " [Drd].[tblCheck].fldOrganId,[Drd].[tblCheck].fldMablaghSanad fldMablagh,  [Drd].[tblCheck].fldDesc, " +
            //            " fldTarikhAkhz AS TarikhSabt,  [Drd].[tblCheck].fldDate,  [Drd].[tblCheck].fldUserId,  " +
            //            "  chk.tblDasteCheck.fldMoshakhaseDasteCheck, Com.tblSHobe.fldName AS NameShobe, Com.tblBank.fldBankName,tblAshkhaseHoghoghi.fldName AS fldDarVajh,  " +
            //            "  tblAshkhaseHoghoghi.fldShenaseMelli AS ShomareMeli, Com.tblShomareHesabeOmoomi.fldShomareHesab, Com.tblAshkhas.fldId AS AshkhasId,  " +
            //            "  Com.tblEmployee.fldFamily + ' ' + Com.tblEmployee.fldName AS Name_Family, CASE fldBabatFlag WHEN 1 THEN  [Drd].[tblCheck].fldBabat ELSE '' END AS BabatText,  " +
            //            "  Com.date_to_words( [Drd].[tblCheck].fldTarikhSarResid) AS TarikhShamsi, chk.Num_ToWords( [Drd].[tblCheck].fldMablaghSanad) AS MablaghBeHorof , " +
            //           "  [tblCheck].fldStatus as fldvaziat,CASE WHEN [tblCheck].fldStatus = 1 THEN N'در انتظار وصول' WHEN [tblCheck].fldStatus = 2 THEN N'وصول شده'  " +
            //            " WHEN [tblCheck].fldStatus = 3 THEN N'برگشت خورده' WHEN [tblCheck].fldStatus = 4 THEN N'حقوقی شده'  WHEN [tblCheck].fldStatus= 5 THEN N'عودت داده شده' END  AS NameVaziat,  " +
            //            " ISNULL( fldDateStatus,N'') fldTarikhVaziat " +
            //            "   FROM             [Drd].[tblCheck] INNER JOIN " +
            //            "  chk.tblDasteCheck ON  [Drd].[tblCheck].fldIdDasteCheck = chk.tblDasteCheck.fldId INNER JOIN " +
            //            "  Com.tblShomareHesabeOmoomi ON Com.tblShomareHesabeOmoomi.fldId = chk.tblDasteCheck.fldIdShomareHesab INNER JOIN " +
            //            "  Com.tblSHobe ON Com.tblShomareHesabeOmoomi.fldShobeId = Com.tblSHobe.fldId INNER JOIN " +
            //            "  Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN " +
            //             " Com.tblUser ON Com.tblUser.fldId =  [Drd].[tblCheck].fldUserId INNER JOIN " +
            //            "  Com.tblAshkhas ON  [Drd].[tblCheck].fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN " +
            //            "  Com.tblAshkhaseHoghoghi ON Com.tblAshkhas.fldHoghoghiId = Com.tblAshkhaseHoghoghi.fldId INNER JOIN " +
            //            "  Com.tblEmployee ON Com.tblEmployee.fldId = Com.tblUser.fldEmployId)p" 
                    //+ where(AzTarikh, TaTarikh, AzTarikh_V, TaTarikh_V, AzMablagh, TaMablagh, BankName, AzSerial, TaSerial, ShobeName, DarVajh, Babat, user,OrganId);

            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var avarez = service.GetData(commandText).Tables[0];

            List<WCF_Chek.OBJ_SodorCheck> p = new List<WCF_Chek.OBJ_SodorCheck>();
            for (int i = 0; i < avarez.Rows.Count; i++)
            {
                WCF_Chek.OBJ_SodorCheck l = new WCF_Chek.OBJ_SodorCheck();
                l.fldId = (int)(avarez.Rows[i]["fldId"]);
                l.fldBabat = (string)(avarez.Rows[i]["fldBabat"]);
                l.fldBankName = (string)(avarez.Rows[i]["fldBankName"]);
                l.fldCodeSerialCheck = (string)(avarez.Rows[i]["fldCodeSerialCheck"]);
                l.fldDarVajh = (string)(avarez.Rows[i]["fldDarVajh"]);
                l.fldMablagh = (long)(avarez.Rows[i]["fldMablagh"]);
                l.fldMoshakhaseDasteCheck = (string)(avarez.Rows[i]["fldMoshakhaseDasteCheck"]);
                l.fldTarikhVosol = (string)(avarez.Rows[i]["fldTarikhVosol"]);
                l.Name_Family = (string)(avarez.Rows[i]["Name_Family"]);
                l.NameShobe = (string)(avarez.Rows[i]["NameShobe"]);
                l.TarikhSabt = (string)(avarez.Rows[i]["TarikhSabt"]);
                l.fldIdDasteCheck = (int)(avarez.Rows[i]["fldIdDasteCheck"]);

                p.Add(l);
            }

            return Json(p.ToList(), JsonRequestBehavior.AllowGet);
        }
        private string where(string AzTarikh, string TaTarikh, string AzTarikh_V, string TaTarikh_V, string AzMablagh, string TaMablagh, string BankName, string AzSerial, string TaSerial, string ShobeName, string DarVajh, string Babat, string user,int OrganId)
        {
            try
            {
                string s = "";
                if (AzTarikh != "" && AzTarikh != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " TarikhSabt>='" +AzTarikh + "'";
                }
                if (TaTarikh != "" && TaTarikh != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " TarikhSabt<='" + TaTarikh + "'";
                }
                if (AzTarikh_V != "" && AzTarikh_V != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldTarikhVosol>='" + AzTarikh_V + "'";
                }
                if (TaTarikh_V != "" && TaTarikh_V != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldTarikhVosol<='" + TaTarikh_V + "'";
                }
                if (AzMablagh != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldMablagh>=" + AzMablagh;
                }
                if (TaMablagh != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldMablagh<=" + TaMablagh;
                }
                if (BankName != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldBankName=N'" + BankName + "'";
                }
                if (AzSerial != "" && AzSerial != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldCodeSerialCheck>='" + AzSerial + "'";
                }
                if (TaSerial != "" && TaSerial != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldCodeSerialCheck<='" + TaSerial + "'";
                }
                if (ShobeName != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " NameShobe=N'" + ShobeName + "'";
                }
                if (DarVajh != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldDarVajh=N'" + DarVajh + "'";
                }
                if (Babat != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " chk.tblSodorCheck.fldBabat=N'" + Babat + "'";
                }
                if (user != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " Name_Family=N'" + user + "'";
                }
                if (OrganId != 0)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldOrganId=" + OrganId;
                }
                if (s != "")
                    s = " WHERE " + s;

                return s;
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    return x.InnerException.Message;
                return "";
            }
        }
    }
}
