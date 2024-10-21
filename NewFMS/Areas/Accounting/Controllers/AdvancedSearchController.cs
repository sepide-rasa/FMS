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
    public class AdvancedSearchController : Controller
    {
        // GET: Accounting/AdvancedSearch
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
        public ActionResult GetFieldName()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Fields = new string[10, 3] { { "fldDocumentNum", "شماره سند", "1" }, { "fldAtfNum", "شماره عطف", "1" }, { "fldTarikhDocument", "تاریخ", "3" }, 
            { "fldArchiveNum", "شماره بایگانی", "1" },{"fldDescriptionDocu","شرح سند","2" }, {"fldShomareFaree","شماره فرعی","1" }, { "fldType","وضعیت","2"}, 
            {"fldDescriptionCoding","عنوان آرتیکل","1" }, { "fldDescription","شرح","1"},{"fldNameParvande","شماره پرونده","1" } };
            List<Models.DocFields> LF=new List<Models.DocFields>();
            for (int i = 0; i < 10; i++)
            {
                Models.DocFields F = new Models.DocFields();
                F.Name = Fields[i, 1];
                F.EnName = Fields[i, 0];
                F.Type =Convert.ToByte(Fields[i, 2]);
                LF.Add(F);
            }
            return this.Store(LF);
        }
        public ActionResult GetOperator(byte state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<Models.DocFields> LO = new List<Models.DocFields>();
            var Op = new string[8]{"=","!=",">=","<=",">","<","خالی","خالی نباشد"};
            switch (state)
            {
                case 2:
                    Op = new string[7] { "شامل", "شامل نباشد", "عین عبارت", "شروع با", "خاتمه با", "خالی", "خالی نباشد" };
                    break;
                case 3:
                    Op = new string[7] { "=","!=", "بین", "قبل از", "بعد از", "خالی", "خالی نباشد" };
                    break;
            }
            for (int i = 0; i < Op.Length; i++)
            {
                Models.DocFields F = new Models.DocFields();
                F.Name = Op[i];
                LO.Add(F);
            }
            return this.Store(LO);
        }
    }
}