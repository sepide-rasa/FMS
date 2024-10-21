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

namespace NewFMS.Areas.Accounting.Controllers
{
    public class SearchCodingController : Controller
    {
        // GET: Accounting/SearchCoding
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Budget.BudgetService Bud_Service = new WCF_Budget.BudgetService();
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Archive.ArchiveService ArchiveService = new WCF_Archive.ArchiveService();
        WCF_Archive.ClsError ErrArchive = new WCF_Archive.ClsError();
        WCF_Budget.ClsError ErrBudget = new WCF_Budget.ClsError();
        public ActionResult Index(short Year,int state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
           
            result.ViewBag.Year = Year.ToString();
            result.ViewBag.state = state;
            return result;
        }
        public ActionResult LoadTreeCoding(string nod, short Year, string FilterValue)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var CodingHeader = servic.GetCoding_HeaderWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
            var q = servic.GetCoding_DetailsWithFilter("fldPID", nod, CodingHeader.fldId.ToString(), "%" + FilterValue + "%", 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                var LevelName = servic.GetAccountingLevelDetail(item.fldAccountLevelId, Convert.ToInt32(Session["OrganId"]), IP, out Err).fldName;
                switch (LevelName)
                {
                    case "گروه":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "کل":
                        asyncNode.IconCls = "KolIco";
                        break;
                    case "معین":
                        asyncNode.IconCls = "MoeinIco";
                        break;
                    default:
                        asyncNode.IconCls = "TafsiliIco";
                        break;
                }
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldCode + "-" + item.fldTitle;
                asyncNode.Expanded = false;
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldTempCodingId = item.fldTempCodingId.ToString(),
                    fldTitle = item.fldCode + "-" + item.fldTitle,
                    fldMahiyatId = item.fldMahiyatId,
                    fldTempNameId = item.fldTempNameId.ToString(),
                    fldAccountingTypeId = item.fldAccountingTypeId.ToString(),
                    fldAccountLevelId = item.fldAccountLevelId,
                    fldName_AccountingLevel = item.fldName_AccountingLevel,
                    fldPCod = item.fldPCod,
                    fldDesc = item.fldDesc
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }

    }
}