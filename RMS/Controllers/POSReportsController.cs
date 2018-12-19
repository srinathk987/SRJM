using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace RMS.Controllers
{
    public class POSReportsController : Controller
    {
        //
        // GET: /POSReports/
        private IPOSReports posTransactions = new POSReportsRepository();

        #region UserDeclaration

        private DataTable dt = null;

        private string JsonString = string.Empty;

        #endregion UserDeclaration

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BillGenerationReport(string billno)
        {
            System.Data.DataSet ds = posTransactions.BillGenerationReport(billno);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string Hostname = "";
            string session = "";
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptBillGeneration.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("TableId", ds.Tables[0].Rows[0]["tableid"]);
            rptH.SetParameterValue("BillNo", ds.Tables[0].Rows[0]["vchbillno"]);
            rptH.SetParameterValue("HostName", Hostname);
            rptH.SetParameterValue("SessionNo", session);
            rptH.SetParameterValue("Printtype", "Original");
            //  rptH.SetParameterValue("Billtype", "billtype");

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #region categorywise report

        public ActionResult categorywiseReport()
        {
            return View();
        }

        public JsonResult GetCategorynames()
        {
            dt = new DataTable();
            dt = posTransactions.GetCategorynames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewcategorywiseReport(DateTime fromdate, DateTime todate, string Category)
        {
            System.Data.DataSet ds = posTransactions.CategorywiseReport(fromdate, todate, Category);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/Categorywisereport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion categorywise report

        #region subCategorywise

        public ActionResult CreateSubcategorywiseReport()
        {
            return View();
        }
        public JsonResult Getsubcategorynames(string CategoryID)
        {
            dt = new DataTable();
            dt = posTransactions.Getsubcategorynames(CategoryID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult subCategorywiseReport(DateTime fromdate, DateTime todate,string CategoryName, string subcategoryName)
        {
            System.Data.DataSet ds = posTransactions.subCategorywiseReport(fromdate, todate,CategoryName, subcategoryName);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/SubCategorywisereport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion subCategorywise

        #region Itemwise

        public ActionResult CreateItemwiseReport()
        {
            return View();
        }

        public JsonResult GetItemnames()
        {
            dt = new DataTable();
            dt = posTransactions.GetItemnames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemwiseSaleReport(DateTime fromdate, DateTime todate, string itemName)
        {
            System.Data.DataSet ds = posTransactions.ItemwiseSaleReport(fromdate, todate, itemName);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/ItemwiseSalereport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion Itemwise

        public ActionResult KotChangeslip(string kotno)
        {
            DataSet ds = posTransactions.KotChangeslip(kotno);
            DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/KotiChangeSlipReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            // rptH.SetParameterValue("CompanyName", "KAPIL ENGINEERS AND INFRASTRUCTURES PVT LTD");
            // rptH.SetParameterValue("Address", "SY NO 115/1,Kapil Towers,15th Floor,Nanakramguda");
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Month", "kotno");
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult Kotslip(string kotno)
        {
            DataSet ds = posTransactions.Kotslip(kotno);
            DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/KotiSlipReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            //rptH.SetParameterValue("CompanyName", "KAPIL ENGINEERS AND INFRASTRUCTURES PVT LTD");
            //rptH.SetParameterValue("Address", "SY NO 115/1,Kapil Towers,15th Floor,Nanakramguda");
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Month", "kotno");
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #region SalesReport

        public ActionResult SalesReport()
        {
            return View();
        }

        public JsonResult GetCashierComboData()
        {
            dt = new DataTable();
            dt = posTransactions.GetCashierComboData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalesGenerateReports(DateTime fromdate, DateTime todate, int userid)
        {
            System.Data.DataSet ds = posTransactions.SalesGenerateReports(fromdate, todate, userid);
            System.Data.DataSet ds1 = posTransactions.companyname();
            System.Data.DataSet ds2 = posTransactions.usernmaere(userid);
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            DateTime rptfromdate = fromdate;
            DateTime rpttodate = todate;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/Salesrpt.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);

            rptH.SetParameterValue("FromDate", rptfromdate);
            rptH.SetParameterValue("ToDate", rpttodate);
            rptH.SetParameterValue("UserName", ds2.Tables[0].Rows[0]["username"]);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion SalesReport

        #region TableWilse Sale

        public ActionResult TableWiseSale()
        {
            return View();
        }

        public JsonResult GetTablename()
        {

            dt = new DataTable();
            dt = posTransactions.GetTablenames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TableGenerateReports(DateTime fromdate, DateTime todate, string tablename)
        {
            System.Data.DataSet ds = posTransactions.TableGenerateReports(fromdate, todate, tablename);
            System.Data.DataSet ds1 = posTransactions.companyname();
            //System.Data.DataSet ds2 = posreports.usernmaere(userid);
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            DateTime rptfromdate = fromdate;
            DateTime rpttodate = todate;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptTableWise.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);

            rptH.SetParameterValue("FromDate", rptfromdate);
            rptH.SetParameterValue("ToDate", rpttodate);
            //rptH.SetParameterValue("UserName", ds2.Tables[0].Rows[0]["username"]);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion TableWilse Sale

        #region Waiterwisesale

        public ActionResult WaiterwiseSale()
        {
            return View();
        }

        public ActionResult WaiterwiseGenerateReports(DateTime fromdate, DateTime todate)
        {
            System.Data.DataSet ds = posTransactions.WaiterwiseGenerateReports(fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            //System.Data.DataSet ds2 = posreports.usernmaere(userid);
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            DateTime rptfromdate = fromdate;
            DateTime rpttodate = todate;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptWaiterwise.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);

            rptH.SetParameterValue("FromDate", rptfromdate);
            rptH.SetParameterValue("ToDate", rpttodate);
            //rptH.SetParameterValue("UserName", ds2.Tables[0].Rows[0]["username"]);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion Waiterwisesale

        #region WeeklyWiseReport

        public ActionResult CreateWeeklyWiseReport()
        {
            return View();
        }
        public ActionResult WeeklyWiseReport(string fromdate, string todate)
        {
            System.Data.DataSet ds = posTransactions.WeeklyWiseReport(fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/WeeklywiseReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }



        #endregion

        #region SectionWiseSale

        public ActionResult SectionWiseSale()
        {
            return View();
        }


        public JsonResult GetSectionnames()
        {
            dt = new DataTable();
            dt = posTransactions.GetSectionnames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SectionWiseSaleReport(DateTime fromdate, DateTime todate, string sectionname)
        {
            System.Data.DataSet ds = posTransactions.SectionWiseSaleReport(fromdate, todate, sectionname);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/Sectionwisereport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion


        #region ItemCancel
        public ActionResult ItemCancel()
        {
            return View();
        }

        public ActionResult ItemCancelGenerateReports(DateTime fromdate, DateTime todate)
        {
            System.Data.DataSet ds = posTransactions.ItemCancelGenerateReports(fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            //System.Data.DataSet ds2 = posreports.usernmaere(userid);
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            DateTime rptfromdate = fromdate;
            DateTime rpttodate = todate;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptItemCancle.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);

            rptH.SetParameterValue("FromDate", rptfromdate);
            rptH.SetParameterValue("ToDate", rpttodate);
            //rptH.SetParameterValue("UserName", ds2.Tables[0].Rows[0]["username"]);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion

        #region   Item cosumption Sale Report

        public ActionResult ItemConsumptionSaleReport()
        {
            return View();
        }

        public JsonResult GetDepartmentnames()
        {
            dt = new DataTable();
            dt = posTransactions.GetDepartmentnames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ItemConsumptionReport(DateTime fromdate, DateTime todate, string department)
        {
            System.Data.DataSet ds = posTransactions.ItemConsumptionSaleReport(fromdate, todate, department);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/ItemCosumptionreport .rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            //if (true)
            //{
            //    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
            //    return File(stream, "application/excel");
            //}
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //  stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");
        }

        #endregion


        #region  BillWise Sale Report

        public ActionResult BillWiseSaleReport()
        {

            return View();
        }

        public JsonResult GetBillNos()
        {
            dt = new DataTable();
            dt = posTransactions.GetBillNos();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BillWiseReport(DateTime fromdate, DateTime todate, string BillNo)
        {
            System.Data.DataSet ds = posTransactions.BillwiseReport(fromdate, todate, BillNo);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/BillWiseSaleReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion


        //#region  ShiftWise Item Sale Report

        //public ActionResult ShiftWiseSaleReport()
        //{

        //    return View();
        //}

        //public ActionResult ShiftWiseReport(DateTime fromdate, DateTime todate, string Shift)
        //{
        //    System.Data.DataSet ds = posTransactions.ShiftwiseItemSaleReport(fromdate, todate, Shift);
        //    System.Data.DataSet ds1 = posTransactions.companyname();
        //    string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
        //    string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
        //    ReportClass rptH = new ReportClass();
        //    rptH.FileName = Server.MapPath("/Reports/ShiftWiseSaleReport 2.rpt");
        //    rptH.Load();
        //    rptH.SetDataSource(ds.Tables[0]);
        //    rptH.SetParameterValue("CompanyName", strcompanyname);
        //    rptH.SetParameterValue("Address", strcompanyAddress);
        //    rptH.SetParameterValue("Date", fromdate);
        //    rptH.SetParameterValue("Todate", todate);
        //    if (true)
        //    {

        //    }
        //    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
        //    return File(stream, "application/excel");
        //}

        //#endregion

        #region  ShiftWise Item Sale Report

        public ActionResult ShiftWiseSaleReport()
        {

            return View();
        }

        public JsonResult GetSessionnames()
        {
            dt = new DataTable();
            dt = posTransactions.GetSessionnames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShiftWiseReport(DateTime fromdate, DateTime todate, string Shift)
        {
            System.Data.DataSet ds = posTransactions.ShiftwiseItemSaleReport(fromdate, todate, Shift);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/ShiftWiseSaleReport 2.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            //if (true)
            //{
            //    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
            //    return File(stream, "application/excel");
            //}
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //  stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");
        }

        #endregion

        #region  BillNo Details Report

        public ActionResult BillNoDetailsReport()
        {
            return View();
        }


        public ActionResult BillDetailsReport(string BillNo)
        {
            System.Data.DataSet ds = posTransactions.BillDetailsReport(BillNo);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/BillNoDetailsReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion

        #region  Complementary Report

        public ActionResult ComplementaryReport()
        {

            return View();
        }



        public ActionResult ComplementarySaleReport(DateTime fromdate, DateTime todate)
        {
            System.Data.DataSet ds = posTransactions.ComplementarySaleReport(fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/ComplementaryReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion

        public ActionResult PurchaseOrderReport(string billno)
        {
            System.Data.DataSet ds = posTransactions.BillGenerationReport(billno);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string Hostname = "";
            string session = "";
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptBillGeneration.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("TableId", ds.Tables[0].Rows[0]["tableid"]);
            rptH.SetParameterValue("BillNo", ds.Tables[0].Rows[0]["vchbillno"]);
            rptH.SetParameterValue("HostName", Hostname);
            rptH.SetParameterValue("SessionNo", session);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }



        #region Bill Reprint

        public ActionResult BillReprint()
        {
            return View();

        }

        public JsonResult GetBillNumbers(string Billtype)
        {
            dt = new DataTable();
            dt = posTransactions.GetBillNumbers(Billtype);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BillReprintReport(string BillNo, string Billtype, string Takeawayno, string Printtype)
        {

            System.Data.DataSet ds = posTransactions.BillReprintReport(BillNo, Billtype);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string Hostname = "";
            string session = "";
            ReportClass rptH = new ReportClass();
            if (Takeawayno == null)
            {

                Takeawayno = "";
            }
            if (Billtype == "Dinning")
            {
                rptH.FileName = Server.MapPath("/Reports/rptBillGeneration.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);

            }
            else
            {
                rptH.FileName = Server.MapPath("/Reports/rptBillReprint.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("TakeAwayNo", Takeawayno);
                rptH.SetParameterValue("Billtype", Billtype);
            }


            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("TableId", ds.Tables[0].Rows[0]["tableid"]);
            rptH.SetParameterValue("BillNo", ds.Tables[0].Rows[0]["vchbillno"]);
            rptH.SetParameterValue("HostName", Hostname);
            rptH.SetParameterValue("Printtype", Printtype);
            rptH.SetParameterValue("SessionNo", session);

            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public bool Billprintdetails(string BillNo, string Billtype)
        {
            return posTransactions.Billprintdetails(BillNo, Billtype);
        }



        #endregion

        #region DayWiseSale

        public ActionResult DayWiseSale()
        {

            return View();
        }


        public ActionResult daywisesaleReport(DateTime fromdate, DateTime todate)
        {
            System.Data.DataSet ds = posTransactions.DaywisesaleReport(fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/DayWiseSaleReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("Date", fromdate);
            rptH.SetParameterValue("Todate", todate);
            //if (true)
            //{
            //    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
            //    return File(stream, "application/excel");
            //}
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //  stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");
        }

        #endregion




        #region MMSSaleReports

        public ActionResult SaleBillGenerationReport(string SaleNo, string Items, string qnty)
        {
            int n = Convert.ToInt32(Session["UserId"]);
            string Uname = posTransactions.GetUsername(n);      
            
           // System.Data.DataSet squantity = posTransactions.Quantity(SaleNo);
            System.Data.DataSet ds = posTransactions.SaleBillGenerationReport(SaleNo);
            //System.Data.DataSet ds1 = posTransactions.companyname();
            //string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            //string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string strcompanyname = "DECCAN FRUIT GROWERS ASSOCIATION";
            string strcompanyAddress = "NANAKRAMGUDA,FINANCIAL DISTRICT,GACHIBOWLI,HYDERABAD";
            var culture = CultureInfo.CreateSpecificCulture("hi-IN");
            string currencySymbol = culture.NumberFormat.CurrencySymbol;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptMMSSaleDetails.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);           
            rptH.SetParameterValue("BillNo", ds.Tables[0].Rows[0]["detailsid"]);  
            DataTable dt = ds.Tables[0];
            int nn = Convert.ToInt32(dt.Compute("sum(items)", ""));
            rptH.SetParameterValue("Items", nn);          
            rptH.SetParameterValue("username", Uname.ToUpper());
            rptH.SetParameterValue("Rs", currencySymbol);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
           rptH.PrintOptions.PrinterName = "EPSON TM-U220 Receipt"; 
            //rptH.PrintOptions.PrinterName = "Microsoft XPS Document Writer";
           rptH.PrintToPrinter(1, false, 0, 0);         
            return File(stream, "application/pdf");
        }

        public ActionResult DuplicateSaleBillGenerationReport(string SaleNo, string Items, string qnty)
        {
            int n = Convert.ToInt32(Session["UserId"]);
            string Uname = posTransactions.GetUsername(n);

            // System.Data.DataSet squantity = posTransactions.Quantity(SaleNo);
            System.Data.DataSet ds = posTransactions.SaleBillGenerationReport(SaleNo);
            //System.Data.DataSet ds1 = posTransactions.companyname();
            //string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            //string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string strcompanyname = "DECCAN FRUIT GROWERS ASSOCIATION";
            string strcompanyAddress = "NANAKRAMGUDA,FINANCIAL DISTRICT,GACHIBOWLI,HYDERABAD";
            var culture = CultureInfo.CreateSpecificCulture("hi-IN");
            string currencySymbol = culture.NumberFormat.CurrencySymbol;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptMMSSaleDetails.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("BillNo", ds.Tables[0].Rows[0]["detailsid"]);
            DataTable dt = ds.Tables[0];
            int nn = Convert.ToInt32(dt.Compute("sum(items)", ""));
            rptH.SetParameterValue("Items", nn);
            rptH.SetParameterValue("username", Uname.ToUpper());
            rptH.SetParameterValue("Rs", currencySymbol);           
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            rptH.PrintOptions.PrinterName = "EPSON TM-U220 Receipt";
            //rptH.PrintOptions.PrinterName = "Microsoft XPS Document Writer";
            rptH.PrintToPrinter(1, false, 0, 0);
            return File(stream, "application/pdf");
        }

        #endregion

    }
}