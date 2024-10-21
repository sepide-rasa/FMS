using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.EJ2.Navigations;

namespace NewFMS.Areas.Automation.Controllers
{
    public class DocumentEditorController : Controller
    {
        //
        // GET: /Automation/DocumentEditor/

        public ActionResult Index()
        {
            this.DocumentEditorViewResultHelper();
            return View();
        }
        public PartialViewResult DocumentEditorViewResultHelper()
        {
            List<object> exportItems = new List<object>();
            exportItems.Add(new { text = "Microsoft Word (.docx)", id = "word" });
            exportItems.Add(new { text = "Syncfusion Document Text (.sfdt)", id = "sfdt" });
            ViewBag.ExportItems = exportItems;

            return PartialView();
        }

    }
}
