﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Comon.Controllers
{
    public class SearchModuleController : Controller
    {
        //
        // GET: /Comon/SearchModule/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        /// <summary>
        /// فرم اصلی مربوط به سرچ ماژول
        /// </summary>
        /// <remarks>این فرم جهت نمایش تمامی اطلاعات مربوط به ماژول میباشد که در فرم های دیگر قابل استفاده است</remarks>
        /// <param name="State"></param>
        public ActionResult Index(int State)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }

        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول ماژول و امکان سرچ کردن بر اساس   
        /// </summary>
         public ActionResult Read(StoreRequestParameters parameters)
          {
              if (Session["Username"] == null)
                  return RedirectToAction("logon", "Account", new { area = "" });
              var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

              List<WCF_Common.OBJ_Module> data = null;
              if (filterHeaders.Conditions.Count > 0)
              {
                  string field = "";
                  string searchtext = "";
                  List<WCF_Common.OBJ_Module> data1 = null;
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
                      }
                      if (data != null)
                          data1 = servic.GetModuleWithFilter(field, searchtext, 100, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
                      else
                          data = servic.GetModuleWithFilter(field, searchtext, 100, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
                  }
                  if (data != null && data1 != null)
                      data.Intersect(data1);
              }
              else
              {
                  data = servic.GetModuleWithFilter("", "", 100, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
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

              List<WCF_Common.OBJ_Module> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
              //-- end paging ------------------------------------------------------------

              return this.Store(rangeData, data.Count);
          }

    }
}