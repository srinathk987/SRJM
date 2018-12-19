using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using RMS.Core;
using System.Globalization;

namespace RMS.Controllers
{
    public class MMSReportsController : Controller
    {
        private IMMSReports mmsReports = new MMSReportsRepository();
        private IPOSReports posTransactions = new POSReportsRepository();

        private string JsonString = string.Empty;

        private DataTable dt = null;


        public static string FormatDate(string strDate)
        {
            string Date = null;
            string[] dat = null;
            if (strDate != null)
            {
                if (strDate.Contains("/"))
                {
                    dat = strDate.Split('/');
                }
                else if (strDate.Contains("-"))
                {
                    dat = strDate.Split('-');
                }
                if (dat[2].Length > 4)
                {
                    dat[2] = dat[2].Substring(0, 4);
                }
                Date = dat[2] + "-" + dat[1] + "-" + dat[0];
            }
            return Date;
        }
        //
        // GET: /MMSReports/
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult ShowStorageLocation()
        {
            DataTable dtstorelocation;
            try
            {
                dtstorelocation = mmsReports.ShowStorageLocation();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtstorelocation);

                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public JsonResult ShowFromStorageLocation()
        {
            DataTable dtstorelocation;
            try
            {
                dtstorelocation = mmsReports.ShowFromStorageLocation();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtstorelocation);

                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        #region Purchase Order

        public ActionResult PurchaseOrder()
        {
            return View();
        }

        public JsonResult ShowPONo(string strVendorid)
        {
            DataTable dtPoNo;
            try
            {
                if (strVendorid != null && strVendorid != "")
                {
                    dtPoNo = mmsReports.ShowPONo(strVendorid);
                    JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtPoNo);
                }
                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public ActionResult PurchaseOrderReport(string pono, string vendorname, string type)
        {
            System.Data.DataSet ds = mmsReports.GetPurchaseOrderDetailsByID(pono, vendorname);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/PurchaseOrder.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);

            if (type == "Duplicate")
            {
                rptH.SetParameterValue("Type", "Duplicate Purchase Order");
            }
            else
            {
                rptH.SetParameterValue("Type", "Purchase Order");
            }

            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion

        public ActionResult StockReport()
        {
            //System.Data.DataSet ds = mmsReports.GetPurchaseOrderDetailsByID(pono, vendorname);
            //System.Data.DataSet ds1 = posTransactions.companyname();
            //string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            //string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

            //ReportClass rptH = new ReportClass();
            //rptH.FileName = Server.MapPath("/Reports/PurchaseOrder.rpt");
            //rptH.Load();
            //rptH.SetDataSource(ds.Tables[0]);
            //rptH.SetParameterValue("CompanyName", strcompanyname);
            //rptH.SetParameterValue("Address", strcompanyAddress);

            //Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            //return File(stream, "application/pdf");
            return View();
        }

        public ActionResult StockLedgerReport(string fromdate, string todate, string store, string Reporttype)
        {
            System.Data.DataSet ds = mmsReports.Getstockbystore(fromdate, todate, store, Reporttype);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

                ReportClass rptH = new ReportClass();

                if (Reporttype == "Closing Stock")
                {
                    rptH.FileName = Server.MapPath("/Reports/StockReport.rpt");                    
                }
                if (Reporttype == "Stock Ledger")
                {
                    rptH.FileName = Server.MapPath("/Reports/StockLedgerReport.rpt");                    
                }
                rptH.Load();
                rptH.SetDataSource(ds);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                if (Reporttype == "Closing Stock")
                {                  
                    rptH.SetParameterValue("FromDate", FormatDate(fromdate));
                    rptH.SetParameterValue("ToDate", todate);
                }
                if (Reporttype == "Stock Ledger")
                {                   
                    rptH.SetParameterValue("FromDate", FormatDate(fromdate));
                    rptH.SetParameterValue("ToDate", FormatDate(todate));
                }
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("StockReport");
            }

        }

        public ActionResult PurchaseReport()
        {
            return View();
        }

        public ActionResult PurchaseReports(string fromdate, string todate, string Reporttype, string VendorName)
        {
            System.Data.DataSet ds = mmsReports.GetPurchaseData(fromdate, todate, Reporttype, VendorName);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

                ReportClass rptH = new ReportClass();

                if (Reporttype == "Purchse Statement")
                {
                    rptH.FileName = Server.MapPath("/Reports/PurchaseStatement.rpt");
                }
                if (Reporttype == "Purchase Bill Statement")
                {
                    rptH.FileName = Server.MapPath("/Reports/PurchaseBillStatement.rpt");
                }
                if (Reporttype == "Purchase Item Statement")
                {
                    rptH.FileName = Server.MapPath("/Reports/PurchaseItemReport.rpt");
                }
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("FromDate", fromdate);
                rptH.SetParameterValue("ToDate", todate);
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("StockReport");
            }
        }

        public JsonResult ShowVendors()
        {
            DataTable dtVendors;
            try
            {
                dtVendors = mmsReports.ShowVendors();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtVendors);
                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult PurchaseInvoiceReport()
        {
            return View();
        }

        public JsonResult ShowInvoiceNo(string strVendorid)
        {
            DataTable dtInvoiceNo;
            try
            {
                if (strVendorid != null && strVendorid != "")
                {
                    dtInvoiceNo = mmsReports.ShowInvoiceNo(strVendorid);
                    JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtInvoiceNo);
                }
                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public ActionResult PurchaseInvoiceReports(string fromdate, string todate, string InvoiceNo, string Vendorname)
        {
            System.Data.DataSet ds = mmsReports.PurchaseInvoiceReport(fromdate, todate, InvoiceNo, Vendorname);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/PurchaseInvoiceBillStatement.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);

                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("FromDate", fromdate);
                rptH.SetParameterValue("ToDate", todate);

                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("StockReport");
            }
        }


        public ActionResult VendorPurchaseReports(string VendorName)
        {
            System.Data.DataSet ds = mmsReports.GetVendorPurchaseData(VendorName);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

                ReportClass rptH = new ReportClass();

                rptH.FileName = Server.MapPath("/Reports/VendorPurchaseStatement.rpt");

                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);

                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("VendorName", VendorName);
                //rptH.SetParameterValue("FromDate", fromdate);
                //rptH.SetParameterValue("ToDate", todate);

                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("StockReport");
            }
        }

        public ActionResult OutwardReport()
        {
            return View();
        }

        public ActionResult OutWardReports(string fromdate, string todate, string Store, string tostore, string Category, string SubCategory)
        {
            System.Data.DataSet ds = mmsReports.GetOutwardData(fromdate, todate, Store, tostore, Category, SubCategory);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();

                ReportClass rptH = new ReportClass();

                rptH.FileName = Server.MapPath("/Reports/OuwardItemStatement.rpt");

                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("FromDate", fromdate);
                rptH.SetParameterValue("ToDate", todate);
                rptH.SetParameterValue("Store", Store);
                rptH.SetParameterValue("ToStore", tostore);
                rptH.SetParameterValue("Category", Category);
                rptH.SetParameterValue("Subcategory", SubCategory);

                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("StockReport");
            }
        }

        public ActionResult ProductIndentReports(string indent)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            try
            {
                ds = mmsReports.GetProductIndentReport(indent);
                ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/ProductIndentReport.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        #region Productdetailsreport

        public ActionResult Productdetailsreport()
        {
            System.Data.DataSet ds = mmsReports.Productdetailsreport();
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/ProductdetailsReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        #endregion

        #region CategoryReport

        public ActionResult CategoryReport()
        {
            return View();
        }

        public JsonResult GetmmsCategorynames()
        {

            dt = new DataTable();
            dt = mmsReports.GetmmsCategorynames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MmsCategoryReport(DateTime fromdate, DateTime todate, string Category)
        {
            System.Data.DataSet ds = mmsReports.mmsCategoryReport(Category, fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/mmscategoryReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("FromDate",fromdate);
            rptH.SetParameterValue("ToDate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }


        public JsonResult Getmmsproductnames()
        {

            dt = new DataTable();
            dt = mmsReports.Getmmsproductnames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult mmsproductReport(string product, DateTime fromdate, DateTime todate)
        {

            System.Data.DataSet ds = mmsReports.MmsproductReport(product, fromdate, todate);
            System.Data.DataSet ds1 = posTransactions.companyname();
            string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/mmsproductwiseReport.rpt");
            rptH.Load();
            rptH.SetDataSource(ds.Tables[0]);
            rptH.SetParameterValue("CompanyName", strcompanyname);
            rptH.SetParameterValue("Address", strcompanyAddress);
            rptH.SetParameterValue("FromDate", fromdate);
            rptH.SetParameterValue("ToDate", todate);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult SubcategoryPurchaseandSaleReport(string Categoryid, string SubCategoryid, string FromDate, string ToDate)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            try
            {
                string strcompanyname = "";
                string strcompanyAddress = "";
                ds = mmsReports.SubcategoryPurchaseandSaleReport(Categoryid, SubCategoryid, FromDate, ToDate);
                ds1 = posTransactions.companyname();
                strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/rptSubcategorywisePurchaseAndSale.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("FromDate",FormatDate(FromDate));
                rptH.SetParameterValue("ToDate", FormatDate(ToDate));
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


        public ActionResult VendorDetailsReport()
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            try
            {
                ds = mmsReports.VendorDetailsReport();
                ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/rptVendorDetails.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult SubcategorywisepurchaseReport(string Categoryid, string SubCategoryid, string FromDate, string ToDate)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            try
            {
                string strcompanyname = "";
                string strcompanyAddress = "";
                ds = mmsReports.SubcategorywisepurchaseReport(Categoryid, SubCategoryid, FromDate, ToDate);
                ds1 = posTransactions.companyname();
                strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/rptSubcategorywisepurchase.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                //rptH.SetDataSource(ds);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("FromDate",FormatDate(FromDate));
                rptH.SetParameterValue("ToDate", FormatDate(ToDate));
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult VendorItemList(string VendorID)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            try
            {
                string strcompanyname = "";
                string strcompanyAddress = "";
                ds = mmsReports.VendorItemList(VendorID);
                ds1 = posTransactions.companyname();
                strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/rptVendorItemList.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public JsonResult getSubcategoryNames(string CategoryID)
        {
            if (CategoryID != null && CategoryID != "ALL" && CategoryID != "undefined" && CategoryID != string.Empty)
            {
                dt = new DataTable();
                dt = mmsReports.getSubcategoryNames(CategoryID);
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VendorItemListReport()
        {
            return View();
        }

        #region MMSSaleReports


        public ActionResult MMSSaleBillReceipt()
        {
            return View();
        }

        public JsonResult GetSaleNos()
        {

            dt = new DataTable();
            dt = mmsReports.GetSaleNos();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MmsBetweenDatesReport()
        {
            return View();
        }

        //public ActionResult MMsBetweenDates(DateTime fromdate, DateTime todate)
        //{

        //    //string fdatetimee = fromdate.ToString("mm/dd/yyyy");
        //    //string tdatetimee = todate.ToString("mm/dd/yyyy");
        //    string fdatetimee = fromdate.ToShortDateString();
        //    string tdatetimee = todate.ToShortDateString();
        //    System.Data.DataSet ds = mmsReports.GetDataBetweenDates(fdatetimee, tdatetimee);
        //    if (ds != null)
        //    {               
        //        string dt = FormatDate(fdatetimee);
        //        string dtt = FormatDate(tdatetimee);
        //        DateTime dd = Convert.ToDateTime(dt);
        //        DateTime ddd = Convert.ToDateTime(dtt);
        //        System.Data.DataSet ds1 = posTransactions.companyname();
        //        string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
        //        string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
        //        ReportClass rptH = new ReportClass();
        //        //rptH.FileName = Server.MapPath("/Reports/rptStockUpdate.rpt");
        //        rptH.FileName = Server.MapPath("/Reports/rptStockBetweenDates.rpt");
        //       // rptH.FileName = Server.MapPath("/Reports/rptStockReportBetweenDates.rpt");                
        //        rptH.Load();
        //        rptH.SetDataSource(ds.Tables[0]);
        //        rptH.SetParameterValue("CompanyName", strcompanyname);
        //        rptH.SetParameterValue("Address", strcompanyAddress);
        //        rptH.SetParameterValue("fromDate", dd.ToString("dd-MMM-yyyy"));
        //        rptH.SetParameterValue("ToDate", ddd.ToString("dd-MMM-yyyy"));                
        //        //rptH.SetParameterValue("fromDate", dd.ToString("dd-MMM-yyyy"));
        //        //rptH.SetParameterValue("ToDate", ddd.ToString("dd-MMM-yyyy"));                
        //        Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
        //        return File(stream, "application/pdf");
        //    }
        //    else
        //    {
        //        return View("MmsBetweenDatesReport");
        //    }

        //}

        public ActionResult MMsBetweenDates(string fromdate, string todate)
        {            
           
            System.Data.DataSet ds = mmsReports.GetDataBetweenDates(fromdate, todate);
            if (ds != null)
            {
                
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                //rptH.FileName = Server.MapPath("/Reports/rptStockUpdate.rpt");
                rptH.FileName = Server.MapPath("/Reports/rptStockBetweenDates.rpt");
                // rptH.FileName = Server.MapPath("/Reports/rptStockReportBetweenDates.rpt");                
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("ffdate", FormatDate(fromdate));
                rptH.SetParameterValue("ttdate", FormatDate(todate));                                
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("MmsBetweenDatesReport");
            }

        }

        public ActionResult MMSSalesReport()
        {
            return View();
        }

        public ActionResult GetMMSSalesReport(string fromdate, string todate)
        {
            System.Data.DataSet ds = mmsReports.GetMMSSalesData(fromdate, todate);
            if (ds != null)
            {

                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/MMSDaywiseSalesReport.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("Date", FormatDate(fromdate));
                rptH.SetParameterValue("Todate", FormatDate(todate));
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("MMSSalesReport");
            }
        }

        public ActionResult ProductWisePurchaseSaleIssueQty()
        {
            return View();
        }

        public ActionResult GetProductWisePurchaseSaleIssueQty(string fromdate,string todate)
        {
            System.Data.DataSet ds = mmsReports.GetProductWisePurchaseSaleIssueQty(fromdate,todate);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/rptProductwisePurchaseSaleIssueQty.rpt");
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);               
                rptH.SetParameterValue("fromdate", FormatDate(fromdate));
                rptH.SetParameterValue("todate", FormatDate(todate));
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("ProductWisePurchaseSaleIssueQty");
            }
        }


        public ActionResult BillwiseSalesReport()
        {
            return View();
        }

        public ActionResult GetBillWiseSalesReport(string fromdate,string todate)
        {
            System.Data.DataSet ds = mmsReports.GetBillWiseSalesReport(fromdate, todate);
            if (ds != null || ds.Tables[0].Rows.Count>0)
            {

                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass obJRpth = new ReportClass();
                obJRpth.FileName = Server.MapPath("/Reports/saledetailsbillwise.rpt");
                obJRpth.Load();
                obJRpth.SetDataSource(ds.Tables[0]);
                obJRpth.SetParameterValue("CompanyName", strcompanyname);
                obJRpth.SetParameterValue("Address", strcompanyAddress);
                obJRpth.SetParameterValue("fromdate", FormatDate(fromdate));
                obJRpth.SetParameterValue("todate", FormatDate(todate));
                Stream stream = obJRpth.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("BillwiseSalesReport");
            }
        }



        public ActionResult MMSGRNReport()
        {
            return View();
        }

        public JsonResult GetGRNvendornames()
        {
            DataTable dtVendors;
            try
            {
                dtVendors = mmsReports.GetGRNvendornames();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtVendors);
                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetGrnReport(string fromdate, string todate,string vendorid)
        {
            System.Data.DataSet ds = mmsReports.GetGrnReport(fromdate, todate,vendorid);
            if (ds != null || ds.Tables[0].Rows.Count > 0)
            {

                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass obJRpth = new ReportClass();
                obJRpth.FileName = Server.MapPath("/Reports/rptGRNReport.rpt");
                obJRpth.Load();
                obJRpth.SetDataSource(ds.Tables[0]);
                obJRpth.SetParameterValue("CompanyName", strcompanyname);
                obJRpth.SetParameterValue("Address", strcompanyAddress);
                obJRpth.SetParameterValue("fromdate", FormatDate(fromdate));
                obJRpth.SetParameterValue("todate", FormatDate(todate));
                Stream stream = obJRpth.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("MMSGRNReport");
            }
        }


        public ActionResult DuplicateSaleBillGenerationReport(string SaleNo, string Items, string qnty)
        {
            int n = Convert.ToInt32(Session["UserId"]);
            string Uname = posTransactions.GetUsername(n);

            // System.Data.DataSet squantity = posTransactions.Quantity(SaleNo);
            System.Data.DataSet ds = mmsReports.SaleBillGenerationReport(SaleNo);
            //System.Data.DataSet ds1 = posTransactions.companyname();
            //string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
            //string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
            string strcompanyname = "DECCAN FRUIT GROWERS ASSOCIATION";
            string strcompanyAddress = "NANAKRAMGUDA,FINANCIAL DISTRICT,GACHIBOWLI,HYDERABAD";
            var culture = CultureInfo.CreateSpecificCulture("hi-IN");
            string currencySymbol = culture.NumberFormat.CurrencySymbol;
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("/Reports/rptMMSDuplicateSaleBill.rpt");
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
            rptH.SetParameterValue("Printtype", "DUPLICATE");
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);           
            return File(stream, "application/pdf");
        }


        #endregion




        #region ProductwiseStock Ledger

        public ActionResult ProductwiseStockLedger()
        {
            return View();
        }

        public ActionResult GetProductwiseStockLedgerReport(string fromdate, string todate, string product)
        {
            DataSet ds = new DataSet();
            try
            {

                string dt = FormatDate(fromdate).ToString();
                string dtt = FormatDate(todate).ToString();

                DateTime dd = Convert.ToDateTime(dt);
                DateTime ddd = Convert.ToDateTime(dtt);               
                ds = mmsReports.GetProductwiseStockLedgerReport(dt, dtt, product);
                if (ds != null || ds.Tables[0].Rows.Count > 0)
                {
                    string Productname = ds.Tables[0].Rows[0]["productname"].ToString();
                    System.Data.DataSet ds1 = posTransactions.companyname();
                    string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                    string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                    ReportClass rptH = new ReportClass();
                    rptH.FileName = Server.MapPath("/Reports/rptProductwiseStockLedger.rpt");
                    rptH.Load();
                    rptH.SetDataSource(ds.Tables[0]);
                    rptH.SetParameterValue("CompanyName", strcompanyname);
                    rptH.SetParameterValue("Address", strcompanyAddress);
                    rptH.SetParameterValue("FromDate", dd.ToString("dd-MMM-yyyy"));
                    rptH.SetParameterValue("ToDate", ddd.ToString("dd-MMM-yyyy"));
                    rptH.SetParameterValue("Productname", Productname);
                    Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf");

                }

            }
            catch (Exception Ex)
            {

                throw Ex;
            }

            return View();
        }

        public JsonResult Getmmsproductnameswithselect()
        {

            dt = new DataTable();
            dt = mmsReports.Getmmsproductnameswithselect();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult DayWiseWastageReport()
        {
            return View();
        }

        public ActionResult GetDaywiseWastageReport(string fromdate,string todate)
        {

            System.Data.DataSet ds = mmsReports.GetDaywiseWastageReport(fromdate, todate);
            if (ds != null)
            {
                System.Data.DataSet ds1 = posTransactions.companyname();
                string strcompanyname = ds1.Tables[0].Rows[0]["vchcompanyname"].ToString();
                string strcompanyAddress = ds1.Tables[0].Rows[0]["address"].ToString();
                ReportClass rptH = new ReportClass();
                rptH.FileName = Server.MapPath("/Reports/rptDaywisewastage.rpt");                                
                rptH.Load();
                rptH.SetDataSource(ds.Tables[0]);
                rptH.SetParameterValue("CompanyName", strcompanyname);
                rptH.SetParameterValue("Address", strcompanyAddress);
                rptH.SetParameterValue("FromDate", FormatDate(fromdate));
                rptH.SetParameterValue("ToDate", FormatDate(todate));
                Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf");
            }
            else
            {
                return View("DayWiseWastageReport");
            }
        }
    }
}