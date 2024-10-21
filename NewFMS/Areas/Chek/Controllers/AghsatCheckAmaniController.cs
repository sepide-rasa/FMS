using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Chek.Controllers
{
    public class AghsatCheckAmaniController : Controller
    {
        //
        // GET: /Chek/AghsatCheckAmani/
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
        public ActionResult DetailAghsat(string ChekVaredeId, string Mablagh, byte status)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var tarikh = servic.GetDate(IP, out Err).fldTarikh;
            PartialView.ViewBag.Mablagh = Mablagh;
            PartialView.ViewBag.ChekVaredeId = ChekVaredeId;
            PartialView.ViewBag.tarikh = tarikh;
            PartialView.ViewBag.status = status; 
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult EditTarikh_Mablagh(string Id, string Mablagh, string Tarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Mablagh = Mablagh;
            PartialView.ViewBag.Mablagh_H = MyLib.NumberTool.Num2Str(Convert.ToUInt64(Mablagh), 1);
            PartialView.ViewBag.Tarikh = Tarikh;
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }
        public ActionResult Save(List<WCF_Chek.OBJ_AghsatCheckAmani> rowArray, int IdCheckHayeVarede)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servicChek.DeleteAghsatChckAmaniForChckVaredeId(IdCheckHayeVarede, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrChek);
                if (ErrChek.ErrorType)
                {
                    return Json(new
                    {
                        Msg = ErrChek.ErrorMsg,
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in rowArray)
                {
                    MsgTitle = "ذخیره موفق";
                    Msg = servicChek.InsertAghsatCheckAmani(item.fldMablagh, item.fldTarikh, item.fldNobat, item.fldIdCheckHayeVarede, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrChek);
                }

                if (ErrChek.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrChek.ErrorMsg;
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
            var q = servic.GetUserWithFilter("", "", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameEmployee });
            return this.Store(q);
        }
        public ActionResult FullSearch(string NoeeChek, string AzTarikh, string TaTarikh, string AzTarikh_V, string TaTarikh_V, string AzMablagh, string TaMablagh, string BankName, string AzSerial, string TaSerial, string ShobeName, string DarVajh, string Babat, string user)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int OrganId = Convert.ToInt32(Session["OrganId"]);
            if (AzMablagh == "0")
                AzMablagh = "";
            if (TaMablagh == "0")
                TaMablagh = "";
            string commandText = "select * from(SELECT  tblCheck.fldId, tblCheck.fldshobeid fldIdShobe, tblCheck.fldMablaghSanad   fldMablagh, tblCheck.fldAshkhasId, tblCheck.fldTarikhSarResid fldTarikhVosolCheck,  " +
                       " tblCheck.fldOrganId,tblCheck.fldTarikhAkhz fldTarikhDaryaftCheck, tblCheck.fldShomareSanad fldShenaseKamelCheck, tblCheck.fldBabat, tblCheck.fldTypeSanad fldNoeeCheck,  "+
                       "   CASE WHEN tblCheck.fldTypeSanad = 1 THEN N'امانی' WHEN tblCheck.fldTypeSanad = 0 THEN N'وصولی' END AS NoeeCheckName, tblCheck.fldUserId,  "+
                       "   tblCheck.fldDesc, tblCheck.fldDate, dbo.Fn_AssembelyMiladiToShamsi(tblCheck.fldDate) AS TarikhSabt, Com.tblSHobe.fldName AS fldShobeName, Com.tblBank.fldBankName,  "+
                       "   Com.tblBank.fldId AS fldBankId, Com.tblEmployee.fldName + ' ' + Com.tblEmployee.fldFamily AS NameFamily, CASE tblCheck.fldTypeSanad WHEN 0 THEN tblcheck.fldStatus ELSE N'' END AS fldvaziat, "+
						"  CASE tblCheck.fldTypeSanad WHEN 0   then  CASE WHEN [tblCheck].fldStatus = 1 THEN N'در انتظار وصول'  "+
						" 			WHEN [tblCheck].fldStatus = 2 THEN N'وصول شده'  "+
						" 			WHEN [tblCheck].fldStatus = 3 THEN N'برگشت خورده' "+
						" 			 WHEN [tblCheck].fldStatus = 4 THEN N'حقوقی شده'  "+
							" 		 WHEN [tblCheck].fldStatus= 5 THEN N'عودت داده شده' end  ELSE N'' END AS NameVaziat, "+
							" 		  CASE fldTypeSanad WHEN 1  then ISNULL( fldDateStatus,N'')  "+
							" 		   ELSE N'' END AS fldTarikhVaziat,case when tblcheck.fldStatus is not null THEN 1 ELSE 0 END AS fldStatus, "+
							"     tblEmployee_1.fldName+' '+ tblEmployee_1.fldFamily as fldSaderKonandeh, tblEmployee_1.fldCodemeli "+
						" 		,N'حقیقی' as fldTypeShakhs "+
       "    FROM            drd.tblCheck INNER JOIN "+
             "             Com.tblSHobe ON  drd.tblCheck.fldShobeId = Com.tblSHobe.fldId INNER JOIN "+
               "           Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN "+
                "          Com.tblUser ON  drd.tblCheck.fldUserId = Com.tblUser.fldId INNER JOIN "+
               "           Com.tblEmployee ON Com.tblUser.fldEmployId = Com.tblEmployee.fldId INNER JOIN "+
                "          Com.tblAshkhas ON  drd.tblCheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN "+
                "          Com.tblEmployee AS tblEmployee_1 ON Com.tblAshkhas.fldHaghighiId = tblEmployee_1.fldId "+
				" 		  WHERE fldOrganId=  "+  OrganId +  
				" 		   union all    "+
                "         SELECT    tblCheck.fldId, tblCheck.fldshobeid fldIdShobe, tblCheck.fldMablaghSanad   fldMablagh, tblCheck.fldAshkhasId, tblCheck.fldTarikhSarResid fldTarikhVosolCheck,  "+
                 "        tblCheck.fldOrganId,tblCheck.fldTarikhAkhz fldTarikhDaryaftCheck, tblCheck.fldShomareSanad fldShenaseKamelCheck, tblCheck.fldBabat, tblCheck.fldTypeSanad fldNoeeCheck,  "+
                 "         CASE WHEN tblCheck.fldTypeSanad = 1 THEN N'امانی' WHEN tblCheck.fldTypeSanad = 0 THEN N'وصولی' END AS NoeeCheckName, tblCheck.fldUserId,  "+
                 "         tblCheck.fldDesc, tblCheck.fldDate, dbo.Fn_AssembelyMiladiToShamsi(tblCheck.fldDate) AS TarikhSabt, Com.tblSHobe.fldName AS fldShobeName, Com.tblBank.fldBankName,  "+
                 "         Com.tblBank.fldId AS fldBankId, Com.tblEmployee.fldName + ' ' + Com.tblEmployee.fldFamily AS NameFamily, CASE tblCheck.fldTypeSanad WHEN 0 THEN tblcheck.fldStatus ELSE N'' END AS fldvaziat, "+
					" 	 CASE tblCheck.fldTypeSanad WHEN 0   then  CASE WHEN [tblCheck].fldStatus = 1 THEN N'در انتظار وصول'  "+
							" 		WHEN [tblCheck].fldStatus = 2 THEN N'وصول شده'  "+
							" 		WHEN [tblCheck].fldStatus = 3 THEN N'برگشت خورده' "+
							" 		 WHEN [tblCheck].fldStatus = 4 THEN N'حقوقی شده' "+ 
							" 		 WHEN [tblCheck].fldStatus= 5 THEN N'عودت داده شده' end  ELSE N'' END AS NameVaziat, "+
							" 		  CASE fldTypeSanad WHEN 1  then ISNULL( fldDateStatus,N'')  "+
							" 		   ELSE N'' END AS fldTarikhVaziat,case when tblcheck.fldStatus is not null THEN 1 ELSE 0 END AS fldStatus, "+
						" 		 Com.tblAshkhaseHoghoghi.fldName, Com.tblAshkhaseHoghoghi.fldShenaseMelli "+
					" 		   ,N'حقوقی' as fldTypeShakhs "+
" FROM            drd.tblcheck INNER JOIN "+
                 "         Com.tblSHobe ON tblcheck.fldshobeid = Com.tblSHobe.fldId INNER JOIN "+
                   "       Com.tblBank ON Com.tblSHobe.fldBankId = Com.tblBank.fldId INNER JOIN "+
                   "       Com.tblUser ON tblcheck.fldUserId = Com.tblUser.fldId INNER JOIN "+
                  "        Com.tblEmployee ON Com.tblUser.fldEmployId = Com.tblEmployee.fldId INNER JOIN "+
                     "     Com.tblAshkhas ON tblcheck.fldAshkhasId = Com.tblAshkhas.fldId INNER JOIN "+
                 "         Com.tblAshkhaseHoghoghi ON Com.tblAshkhas.fldHoghoghiId = Com.tblAshkhaseHoghoghi.fldId)p " 
                    + where(NoeeChek, AzTarikh, TaTarikh, AzTarikh_V, TaTarikh_V, AzMablagh, TaMablagh, BankName, AzSerial, TaSerial, ShobeName, DarVajh, Babat, user,OrganId)+
                    " order by fldId desc ";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var avarez = service.GetData(commandText).Tables[0];

            List<WCF_Chek.OBJ_CheckHayeVarede> p = new List<WCF_Chek.OBJ_CheckHayeVarede>();
            for (int i = 0; i < avarez.Rows.Count; i++)
            {
                WCF_Chek.OBJ_CheckHayeVarede l = new WCF_Chek.OBJ_CheckHayeVarede();
                l.fldId = (int)(avarez.Rows[i]["fldId"]);
                l.fldBabat = (string)(avarez.Rows[i]["fldBabat"]);
                l.fldBankName = (string)(avarez.Rows[i]["fldBankName"]);
                l.fldSaderKonandeh = (string)(avarez.Rows[i]["fldSaderKonandeh"]);
                l.fldShenaseKamelCheck = (string)(avarez.Rows[i]["fldShenaseKamelCheck"]);
                l.fldMablagh = (int)(avarez.Rows[i]["fldMablagh"]);
                l.fldShobeName = (string)(avarez.Rows[i]["fldShobeName"]);
                l.fldTarikhDaryaftCheck = (string)(avarez.Rows[i]["fldTarikhDaryaftCheck"]);
                l.fldTarikhVosolCheck = (string)(avarez.Rows[i]["fldTarikhVosolCheck"]);
                l.NoeeCheckName = (string)(avarez.Rows[i]["NoeeCheckName"]);
                l.fldTarikhVaziat = (string)(avarez.Rows[i]["fldTarikhVaziat"]);
                l.fldStatus = (int)(avarez.Rows[i]["fldStatus"]);
                if (l.fldStatus == 1)
                    l.NameVaziat = "اقساط تعیین وضعیت شده";
                p.Add(l);
            }

            return Json(p.ToList(), JsonRequestBehavior.AllowGet);
        }
        private string where(string NoeeChek, string AzTarikh, string TaTarikh, string AzTarikh_V, string TaTarikh_V, string AzMablagh, string TaMablagh, string BankName, string AzSerial, string TaSerial, string ShobeName, string DarVajh, string Babat, string user,int OrganId)
        {
            try
            {
                string s = "";
                if (NoeeChek != "" && NoeeChek != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldNoeeCheck=" + 0;
                }
                if (AzTarikh != "" && AzTarikh != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldTarikhDaryaftCheck>='" + AzTarikh + "'";
                }
                if (TaTarikh != "" && TaTarikh != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldTarikhDaryaftCheck<='" + TaTarikh + "'";
                }
                if (AzTarikh_V != "" && AzTarikh_V != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldTarikhVosolCheck>='" + AzTarikh_V + "'";
                }
                if (TaTarikh_V != "" && TaTarikh_V != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldTarikhVosolCheck<='" + TaTarikh_V + "'";
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
                    s = s + " fldShenaseKamelCheck>='" + AzSerial + "'";
                }
                if (TaSerial != "" && TaSerial != null)
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldShenaseKamelCheck<='" + TaSerial + "'";
                }
                if (ShobeName != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldShobeName=N'" + ShobeName + "'";
                }
                if (DarVajh != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldSaderKonandeh=N'" + DarVajh + "'";
                }
                if (Babat != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " fldBabat=N'" + Babat + "'";
                }
                if (user != "")
                {
                    if (s != "")
                        s = s + " and ";
                    s = s + " NameFamily=N'" + user + "'";
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
        public ActionResult Read(StoreRequestParameters parameters, string ChekVaredeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Chek.OBJ_AghsatCheckAmani> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Chek.OBJ_AghsatCheckAmani> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldIdCheckHayeVarede":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldIdCheckHayeVarede";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablagh";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldTarikhVaziat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhVaziat";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                        case "fldNobat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNobat";
                            break;
                    }
                    if (data != null)
                        data1 = servicChek.GetAghsatCheckAmaniWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).Where(l => l.fldIdCheckHayeVarede == Convert.ToInt32(ChekVaredeId)).ToList();
                    else
                        data = servicChek.GetAghsatCheckAmaniWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).Where(l => l.fldIdCheckHayeVarede == Convert.ToInt32(ChekVaredeId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicChek.GetAghsatCheckAmaniWithFilter("fldIdCheckHayeVarede", ChekVaredeId,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
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

            List<WCF_Chek.OBJ_AghsatCheckAmani> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
