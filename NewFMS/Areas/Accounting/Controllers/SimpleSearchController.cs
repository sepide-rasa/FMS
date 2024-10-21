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
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class SimpleSearchController : Controller
    {
        // GET: Accounting/SimpleSearch
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        public ActionResult Index(string containerId, string Year, string FiscalYearId)
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
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult IndexWin(string Year, string FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = Year;
            return result;
        }
        public string whereSearch(string ShomareSanad, string Bedehkar, string Bestankar, string ShomareFish, string Codee, string AzTarikh, string TaTarikh, string Artikl, string Payani)
        {
            string wheretext = "";

            if (ShomareSanad != "")
            {
                wheretext = " fldDocumentNum= " + ShomareSanad;
            }
            if (Bedehkar != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldBedehkar= " + Bedehkar;
            }
            if (Bestankar != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldBestankar= " + Bestankar;
            }
            if (ShomareFish != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldfish_check= " + ShomareFish;
            }
            if (Codee != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldCode='" + Codee + "' ";
            }
            if (AzTarikh != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldTarikhDocument>= '" + AzTarikh + "' ";
            }
            if (TaTarikh != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldTarikhDocument< '" + TaTarikh + "' ";
            }
            if (Artikl != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldSh_Artikl like N'%" + Artikl+"%' " ;
            }
            if (Payani != "")
            {
                if (wheretext != "") wheretext = wheretext + " and ";
                wheretext = wheretext + " fldDescriptionDocu like N'%" + Payani + "%' ";
            }
            //where fldTarikhDocument between '1401/01/01' and '1401/01/01' and fldCode=11 and fldfish_check=1236
            //and fldDocumentNum=1 and fldBedehkar=1000 and fldBestankar=1000 and fldSh_Artikl='' and fldDescriptionDocu=''	";

            if (wheretext != "")
                wheretext = "where " + wheretext;
            return wheretext;
        }
        public ActionResult Search(string ShomareSanad, string Bedehkar, string Bestankar, string ShomareFish, string Codee, string AzTarikh, string TaTarikh, string Artikl, string Payani)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
          
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                SqlConnection con = new SqlConnection();
                    try
                    {

                        string commandText = "select * from (select fldDocumentNum ,fldDescriptionDocu ,d.fldDescription as fldSh_Artikl ,fldTarikhDocument,c.fldCode, "+
                                "  d.fldBedehkar,d.fldBestankar,case when fldCaseTypeId=3 then fldShenaseKamelCheck  "+
			                      " 	 when fldCaseTypeId=4 then fldShomareSanad  "+
			                      " 	 when fldCaseTypeId=6 then ShomareFish end fldfish_check  "+
                                 "  ,case when fldCaseTypeId=3 then N'چک وارده '  "+
                                "   when fldCaseTypeId=4 then N'چک صادره'  "+
                                "   when fldCaseTypeId=6 then N'فیش' end fldfish_checkType  "+
								"   ,fldType ,isnull(fldExistsFile,cast(0 as bit)) fldIsArchive ,  h1.fldId  ,cast(atf as int ) fldAtfNum, cast(ShomareRoozane as int) ShomareRoozane,  fldArchiveNum, "+
             "    fldShomareFaree ,m.fldTitle as  fldNameModule  "+
			" 	,case when fldPId is null then fldDocumentNum  else (select h.fldDocumentNum from acc.tblDocumentRecord_Header1 as h where h1.fldPId=h.fldid) end fldMainDocNum  "+
			" 	,dt.fldName    as  fldTypeSanadName "+
			" 	,case when fldType=1 and fldaccept=1 then N'قطعی' when fldType=1 and fldaccept=0 then N'موقت' when fldType=2 then N'یادداشت' end as fldTypeName "+
			  "    from acc.tblDocumentRecord_Header h   "+
                   "                inner join acc.tblDocumentRecord_Header1 h1 on h1.fldDocument_HedearId=h.fldid  "+
                    "               inner join acc.tblDocumentRecord_Details d on d.fldDocument_HedearId=h.fldId  "+
                     "              inner join acc.tblCoding_Details c on c.fldid=d.fldCodingId  "+
						" 		  inner join[ACC].[tblDocumentType] dt on dt.fldid=h1.fldTypeSanadId "+
                       "            left join acc.tblCase ca on ca.fldid=fldCaseId 						 "+		  
						" 		  left join com.tblModule m on m.fldid=h1.fldModuleErsalId "+
                        "           outer apply (select fldShenaseKamelCheck  from chk.tblCheckHayeVarede c where c.fldid=fldSourceId )CheckVarede  "+
                         "          outer apply (select fldShomareSanad  from drd.tblCheck c where c.fldid=fldSourceId and fldReplyTaghsitId is null )CheckSadere  "+
                          "         outer apply (select c.fldid ShomareFish   from drd.tblSodoorFish c    "+
		                   "                				where c.fldid=fldSourceId AND  not exists (select * from drd.tblEbtal where fldFishId=c.fldid)  "+
			                "                    )Fish "+
								" 	outer apply(select top(1) cast(1 as bit)fldExistsFile from acc.tblDocumentRecorde_File where fldDocumentHeaderId=h.fldid)Doc_File "+
							" 		outer Apply( "+
								" 				select atf from (select row_number()over (order by h2.fldId) atf,h2.fldid    "+
								" 				from acc.tblDocumentRecord_Header hh "+
								" 				inner join acc.tblFiscalYear f1 on hh.fldFiscalYearId=f1.fldid "+
								" 				inner join acc.tblDocumentRecord_Header1 as h2 on h2.fldDocument_HedearId=hh.fldid "+
								" 				where h.fldOrganId=hh.fldOrganId and h.fldYear=hh.fldYear  "+
								" 					and h2.fldModuleSaveId=h1.fldModuleSaveId and fldDocumentNum <>0 "+
									" 				)t where t.fldid=h1.fldid)atfNum	 "+
								" 	outer apply (select ShomareRoozane from (select row_number()over (order by h2.fldDate) ShomareRoozane,h2.fldid   "+
									" 			from acc.tblDocumentRecord_Header hh "+
										" 		inner join acc.tblFiscalYear f1 on hh.fldFiscalYearId=f1.fldid "+
									" 			inner join acc.tblDocumentRecord_Header1 as h2 on h2.fldDocument_HedearId=hh.fldid "+
									" 			where h.fldOrganId=hh.fldOrganId and h.fldYear=hh.fldYear  "+
									" 				and h2.fldModuleSaveId=h1.fldModuleSaveId and cast(h2.fldDate as date)=cast(h1.fldDate as date) 	 "+
                                    " 				and fldDocumentNum <>0)t where t.fldid=h1.fldid)Roozane " +
                              "     where fldModuleSaveId=4 )t  " + whereSearch(ShomareSanad, Bedehkar, Bestankar, ShomareFish, Codee, AzTarikh, TaTarikh, Artikl, Payani);
//where fldTarikhDocument between '1401/01/01' and '1401/01/01' and fldCode=11 and fldfish_check=1236
                        //and fldDocumentNum=1 and fldBedehkar=1000 and fldBestankar=1000 and fldSh_Artikl='' and fldDescriptionDocu=''	";

                        List<Models.SimpleSearch> Inf = new List<Models.SimpleSearch>();
                        string ConnectionString = ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString;
                        con = new SqlConnection(ConnectionString);
                        con.Open();
                        SqlCommand com = new SqlCommand(commandText, con);
                        com.CommandTimeout = 200000;
                        var dr = com.ExecuteReader();
                        while (dr.Read())
                        {
                            Inf.Add(new Models.SimpleSearch
                            {
                                fldDocumentNum = Convert.ToInt32(dr["fldDocumentNum"]),
                                fldDescriptionDocu = (dr["fldDescriptionDocu"]).ToString(),
                                fldSh_Artikl = (dr["fldSh_Artikl"]).ToString(),
                                fldTarikhDocument = (dr["fldTarikhDocument"]).ToString(),
                                fldCode = (dr["fldCode"]).ToString(),
                                fldBedehkar =Convert.ToInt64( dr["fldBedehkar"]),
                                fldBestankar = Convert.ToInt64(dr["fldBestankar"]),
                                fldfish_check = dr["fldfish_check"].ToString(),
                                fldfish_checkType = (dr["fldfish_checkType"]).ToString(),

                                
                            });
                        }
                        con.Close();
                        return new JsonResult()
                        {
                            Data = new
                            {
                                Er = 0,
                                ListSanad = Inf
                            },
                            MaxJsonLength = Int32.MaxValue,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    catch (Exception x)
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                        var ErMsg = "";
                        if (x.InnerException != null)
                            ErMsg = x.InnerException.Message;
                        else
                            ErMsg = x.Message;
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = ErMsg,
                            Er = 1
                        });
                    }
               
            }
        }
        public ActionResult SearchWin(string ShomareSanad, string Bedehkar, string Bestankar, string ShomareFish, string Codee, string AzTarikh, string TaTarikh, string Artikl, string Payani)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
          
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                SqlConnection con = new SqlConnection();
                    try
                    {

                        string commandText = "select * from (select fldDocumentNum ,fldDescriptionDocu ,d.fldDescription as fldSh_Artikl ,fldTarikhDocument,c.fldCode, "+
                                "  d.fldBedehkar,d.fldBestankar,case when fldCaseTypeId=3 then fldShenaseKamelCheck  "+
			                      " 	 when fldCaseTypeId=4 then fldShomareSanad  "+
			                      " 	 when fldCaseTypeId=6 then ShomareFish end fldfish_check  "+
                                 "  ,case when fldCaseTypeId=3 then N'چک وارده '  "+
                                "   when fldCaseTypeId=4 then N'چک صادره'  "+
                                "   when fldCaseTypeId=6 then N'فیش' end fldfish_checkType  "+
								"   ,fldType ,isnull(fldExistsFile,cast(0 as bit)) fldIsArchive ,  h1.fldId  ,cast(atf as int ) fldAtfNum, cast(ShomareRoozane as int) ShomareRoozane,  fldArchiveNum, "+
             "   isnull(fldShomareFaree,cast(0 as int))  fldShomareFaree ,m.fldTitle as  fldNameModule  " +
			" 	,case when fldPId is null then fldDocumentNum  else (select h.fldDocumentNum from acc.tblDocumentRecord_Header1 as h where h1.fldPId=h.fldid) end fldMainDocNum  "+
			" 	,dt.fldName    as  fldTypeSanadName "+
			" 	,case when fldType=1 and fldaccept=1 then N'قطعی' when fldType=1 and fldaccept=0 then N'موقت' when fldType=2 then N'یادداشت' end as fldTypeName "+
			  "    from acc.tblDocumentRecord_Header h   "+
                   "                inner join acc.tblDocumentRecord_Header1 h1 on h1.fldDocument_HedearId=h.fldid  "+
                    "               inner join acc.tblDocumentRecord_Details d on d.fldDocument_HedearId=h.fldId  "+
                     "              inner join acc.tblCoding_Details c on c.fldid=d.fldCodingId  "+
						" 		  inner join[ACC].[tblDocumentType] dt on dt.fldid=h1.fldTypeSanadId "+
                       "            left join acc.tblCase ca on ca.fldid=fldCaseId 						 "+		  
						" 		  left join com.tblModule m on m.fldid=h1.fldModuleErsalId "+
                        "           outer apply (select fldShenaseKamelCheck  from chk.tblCheckHayeVarede c where c.fldid=fldSourceId )CheckVarede  "+
                         "          outer apply (select fldShomareSanad  from drd.tblCheck c where c.fldid=fldSourceId and fldReplyTaghsitId is null )CheckSadere  "+
                          "         outer apply (select c.fldid ShomareFish   from drd.tblSodoorFish c    "+
		                   "                				where c.fldid=fldSourceId AND  not exists (select * from drd.tblEbtal where fldFishId=c.fldid)  "+
			                "                    )Fish "+
								" 	outer apply(select top(1) cast(1 as bit)fldExistsFile from acc.tblDocumentRecorde_File where fldDocumentHeaderId=h.fldid)Doc_File "+
							" 		outer Apply( "+
								" 				select atf from (select row_number()over (order by h2.fldId) atf,h2.fldid    "+
								" 				from acc.tblDocumentRecord_Header hh "+
								" 				inner join acc.tblFiscalYear f1 on hh.fldFiscalYearId=f1.fldid "+
								" 				inner join acc.tblDocumentRecord_Header1 as h2 on h2.fldDocument_HedearId=hh.fldid "+
								" 				where h.fldOrganId=hh.fldOrganId and h.fldYear=hh.fldYear  "+
								" 					and h2.fldModuleSaveId=h1.fldModuleSaveId and fldDocumentNum <>0 "+
									" 				)t where t.fldid=h1.fldid)atfNum	 "+
								" 	outer apply (select ShomareRoozane from (select row_number()over (order by h2.fldDate) ShomareRoozane,h2.fldid   "+
									" 			from acc.tblDocumentRecord_Header hh "+
										" 		inner join acc.tblFiscalYear f1 on hh.fldFiscalYearId=f1.fldid "+
									" 			inner join acc.tblDocumentRecord_Header1 as h2 on h2.fldDocument_HedearId=hh.fldid "+
									" 			where h.fldOrganId=hh.fldOrganId and h.fldYear=hh.fldYear  "+
									" 				and h2.fldModuleSaveId=h1.fldModuleSaveId and cast(h2.fldDate as date)=cast(h1.fldDate as date) 	 "+
                                    " 				and fldDocumentNum <>0)t where t.fldid=h1.fldid)Roozane " +
                              "     where fldModuleSaveId=4 )t  " + whereSearch(ShomareSanad, Bedehkar, Bestankar, ShomareFish, Codee, AzTarikh, TaTarikh, Artikl, Payani);
//where fldTarikhDocument between '1401/01/01' and '1401/01/01' and fldCode=11 and fldfish_check=1236
                        //and fldDocumentNum=1 and fldBedehkar=1000 and fldBestankar=1000 and fldSh_Artikl='' and fldDescriptionDocu=''	";

                        List<Models.SimpleSearch> Inf = new List<Models.SimpleSearch>();
                        string ConnectionString = ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString;
                        con = new SqlConnection(ConnectionString);
                        con.Open();
                        SqlCommand com = new SqlCommand(commandText, con);
                        com.CommandTimeout = 200000;
                        var dr = com.ExecuteReader();
                        string SanadNumbers = "";
                        while (dr.Read())
                        {
                            Inf.Add(new Models.SimpleSearch
                            {
                                fldDocumentNum = Convert.ToInt32(dr["fldDocumentNum"]),
                                fldDescriptionDocu = (dr["fldDescriptionDocu"]).ToString(),
                                fldSh_Artikl = (dr["fldSh_Artikl"]).ToString(),
                                fldTarikhDocument = (dr["fldTarikhDocument"]).ToString(),
                                fldCode = (dr["fldCode"]).ToString(),
                                fldBedehkar =Convert.ToInt64( dr["fldBedehkar"]),
                                fldBestankar = Convert.ToInt64(dr["fldBestankar"]),
                                fldfish_check = dr["fldfish_check"].ToString(),
                                fldfish_checkType = (dr["fldfish_checkType"]).ToString(),
                                fldId = Convert.ToInt32(dr["fldId"]),
                                fldType = Convert.ToByte(dr["fldType"]),
                                fldIsArchive = Convert.ToBoolean(dr["fldIsArchive"]),
                                fldAtfNum = Convert.ToInt32(dr["fldAtfNum"]),
                                ShomareRoozane = Convert.ToInt32(dr["ShomareRoozane"]),
                                fldArchiveNum = (dr["fldArchiveNum"]).ToString(),
                                fldShomareFaree = Convert.ToInt32(dr["fldShomareFaree"]),
                                fldNameModule = (dr["fldNameModule"]).ToString(),
                                fldMainDocNum = Convert.ToInt32(dr["fldMainDocNum"]),
                                fldTypeSanadName = (dr["fldTypeSanadName"]).ToString(),
                                fldTypeName = (dr["fldTypeName"]).ToString()
     
                            
                                
                            });
                            SanadNumbers = SanadNumbers + Convert.ToInt32(dr["fldId"]) + ";";    
                        }
                        con.Close();

                        var ListSanadHeader = servic.GetDocumentRecord_HeaderWithFilter("SanadNumbers", SanadNumbers, "", "", Convert.ToInt32(Session["OrganId"]), 0, 4, 500, IP, out Err).ToList();
                        return new JsonResult()
                        {
                            Data = new
                            {
                                Er = 0,
                                ListSanad = Inf,
                                ListSanadHeader = ListSanadHeader
                            },
                            MaxJsonLength = Int32.MaxValue,
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    catch (Exception x)
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                        var ErMsg = "";
                        if (x.InnerException != null)
                            ErMsg = x.InnerException.Message;
                        else
                            ErMsg = x.Message;
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = ErMsg,
                            Er = 1
                        });
                    }
               
            }
        }
        public ActionResult GetTitle(string Code,string Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var title = "";
            var h = servic.GetCoding_HeaderWithFilter("fldYear", Year,Convert.ToInt32(Session["OrganId"]),0, IP, out Err).FirstOrDefault();
            if (h != null)
            {
                var q = servic.GetCoding_DetailsWithFilter("fldCode", Code, h.fldId.ToString(), "", 0, IP, out Err).FirstOrDefault();
                if (q != null)
                    title = q.fldTitle;
            }
            return Json(new
            {
                fldTitle =title
            }, JsonRequestBehavior.AllowGet);
        }
    }
}