using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core.Interfaces
{
    public interface IMMSReports
    {

        DataTable ShowStorageLocation();

        DataTable ShowFromStorageLocation();

        DataTable ShowPONo(string strVendorid);

        DataSet GetPurchaseOrderDetailsByID(string PurchaseOrderNo, string vendorname);

        DataSet GetPurchaseOrderVendorsDetailsByID(string PurchaseOrderNo);

        DataSet Getstockbystore(string fromdate, string todate, string store, string strReportType);

        DataSet GetPurchaseData(string fromdate, string todate, string strReportType, string VendorName);

        DataTable ShowVendors();

        DataTable ShowInvoiceNo(string strVendorid);

        DataSet PurchaseInvoiceReport(string fromdate, string todate, string InvoiceNo, string Vendorname);

        DataSet GetOutwardData(string fromdate, string todate, string Store, string tostore, string Category, string SubCategory);

        DataSet GetProductIndentReport(string indent);

        DataSet GetVendorPurchaseData(string strVendorName);

        DataSet Productdetailsreport();

        DataSet mmsCategoryReport(string Category, DateTime fromdate, DateTime todate);

        DataSet MmsproductReport(string product, DateTime fromdate, DateTime todate);

        DataTable GetmmsCategorynames();

        DataTable Getmmsproductnames();

        DataSet VendorDetailsReport();

        DataSet SubcategorywisepurchaseReport(string Categoryid, string SubCategoryid, string FromDate, string ToDate);

        DataSet SubcategoryPurchaseandSaleReport(string Categoryid, string SubCategoryid, string FromDate, string ToDate);

        DataSet VendorItemList(string VendorID);

        DataTable getSubcategoryNames(string CategoryID);


        #region MMSSAleReports
        DataSet GetDataBetweenDates(string fromdate, string todate);

        DataSet GetMMSSalesData(string fromdate, string todate);

        //DataSet GetProductWisePurchaseSaleIssueQty();
        DataSet GetProductWisePurchaseSaleIssueQty(string fromdate, string todate);
        #endregion

        DataSet GetProductwiseStockLedgerReport(string ffdate, string ttdate, string product);

        DataTable GetSaleNos();

        DataTable Getmmsproductnameswithselect();

        DataSet GetDaywiseWastageReport(string fromdate, string todate);

        DataSet GetBillWiseSalesReport(string fromdate, string todate);     

        DataTable GetGRNvendornames();    

        DataSet GetGrnReport(string fromdate, string todate, string vendorid);

        DataSet SaleBillGenerationReport(string SaleNo);
    }
}
