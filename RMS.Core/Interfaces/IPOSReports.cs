using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core.Interfaces
{
    public interface IPOSReports
    {
        DataSet BillGenerationReport(string BillNumber);

        DataSet companyname();

        DataSet CategorywiseReport(DateTime fromdate, DateTime todate, string Category);

        DataSet subCategorywiseReport(DateTime fromdate, DateTime todate, string CategoryName, string SubCategory);

        DataSet ItemwiseSaleReport(DateTime fromdate, DateTime todate, string Category);

        DataSet KotChangeslip(string kotno);

        DataSet Kotslip(string kotno);

        #region SaleWiseReport
        DataTable GetCashierComboData();
        DataSet SalesGenerateReports(DateTime fromdate, DateTime todate, int userid);

     //   DataSet companyname();

        DataSet usernmaere(int userid);
        #endregion
        #region TableWiseReport
        DataSet TableGenerateReports(DateTime fromdate, DateTime todate, string tablename);
        #endregion
        #region WaiterwiseReport
        DataSet WaiterwiseGenerateReports(DateTime fromdate, DateTime todate);
        #endregion

        #region WeeklyWiseReport

        DataSet WeeklyWiseReport(string fromdate, string todate);
       // DataSet WeeklyWiseReport(DateTime fromdate, DateTime todate);
        #endregion

        #region SectionWiseSaleReport

        DataSet SectionWiseSaleReport(DateTime fromdate, DateTime todate, string sectionname);

        #endregion

        #region ItemCancelGenerateReports
        DataSet ItemCancelGenerateReports(DateTime fromdate, DateTime todate);
        #endregion


        DataSet BillwiseReport(DateTime fromdate, DateTime todate, string BillNo);

        DataSet ShiftwiseItemSaleReport(DateTime fromdate, DateTime todate, string Shift);

        DataSet ItemConsumptionSaleReport(DateTime fromdate, DateTime todate, string Shift);

        DataTable GetCategorynames();

        DataTable Getsubcategorynames(string categoryid);

        DataTable GetItemnames();

        DataTable GetTablenames();

        DataTable GetSectionnames();

        DataTable GetBillNos();

        DataSet ComplementarySaleReport(DateTime fromdate, DateTime todate);

        DataSet BillDetailsReport(string BillNo);

        DataTable GetSessionnames();

        DataTable GetDepartmentnames();



        DataSet BillReprintReport(string BillNo, string Billtype);

        DataTable GetBillNumbers(string Billtype);

        bool Billprintdetails(string BillNo, string Billtype);

        DataSet DaywisesaleReport(DateTime fromdate, DateTime todate);


        #region MMSSaleReports
        DataSet SaleBillGenerationReport(string SaleNo);       
        string GetUsername(int m);
        #endregion

       // string Quantity(string SaleNo);
       // DataSet Quantity(string SaleNo);
    }
}
