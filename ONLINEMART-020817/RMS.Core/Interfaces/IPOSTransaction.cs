using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace RMS.Core.Interfaces
{
    public interface IPOSTransaction
    {


        #region KotChange

        List<KOTTakingDTO> ShowKotChangeDetails(string TableId);
        bool SaveKOTChangeDetails(List<KOTTakingDTO> kc, string Delete, int C, int S);

        #endregion


        #region Dashboardchange

        List<DashboardDTO> ShowAlltablesformodel();
        bool SaveTableDetails(string TableName, int Createdby);

        bool updateTableDetails(int Createdby);
        #endregion

        #region Kot Transfer

        List<KOTTakingDTO> ShowKotTransferDetails(string TableId);

        bool KOTTransfereDetails(string KotTransferDto, string TableId, string date, string time, string hostname, int ttablesid);


        //bool KOTTransfereDetails(string KotTransferDto, string TableId, string date, string time, string hostname);

        List<KOTTakingDTO> ShowKotDetailsBytableid(string TableId);
        //DataTable GetCashierComboData(int Userid);

        DataTable GetCashierComboDataBind(string tableid);
        DataTable GetCashierComboData();

        #endregion


        #region Bill Cancel

        DataTable GetBillComboData(string TableId);

        DataTable getBillDetals(string TableId);

        bool SaveBillSDeleted(string JsonString, string createby);

        #endregion


        #region BillSettlement
        List<BillSettlementDTO> ShowBillNos();
        bool SaveBillSettlement(BillSettlementDTO BS, CashDetails lstCashDetails, List<CardDetails> lstCardDetails);

        string getStusForBillsPending(string TableId);
        #endregion

        #region KOT Taking

        string ShowSessionForKOTtaking();
        List<ItemsDTO> ShowItemsForSession();
        List<KOTTakingDTO> ShowSessionDetailsForSession(string tablename, int Userid);
        List<MergingTables> ShowAvailabletables(string SectionID, string TableName);
        int CreateKOT(KOTTakingDTO Dept, List<ItemsDTO> ItemDetails, List<Mergetables> Mergetables);
        List<KotAvailabletables> ShowMergetables(string tablename);
        List<ItemsDTO> ShowPendingKOT(string tablename);
        System.Data.DataSet Kotslip(string kotno);
        #endregion

        #region Bill-Generation


        List<BillGeneration> ShowkotDetails(Core.BillGeneration objBillGeneration);
        List<BillGeneration> ShowBillVouchers();
        //   BillEditDTO ShowVoucherDetails(string voucherid);
        bool SaveBillGeneration(BillGeneration objBillGeneration, List<BillGeneration> lstKotDetails);

        List<BillGeneration> ShowServicetax();
        List<BillGeneration> ShowServiceCharge();
        #endregion

        #region BILL EDIT

        List<BillEditDTO> ShowReasons();
        List<BillEditDTO> ShowVouchers(string Billid);
        BillEditDTO ShowVoucherDetails(string voucherid);
        List<BillEditDTO> ShowBillNosBillEdit();
        List<BillEditDTO> ShowBillEditServicetax();
        List<BillEditDTO> ShowBillEditServiceCharge();

        List<BillEditDTO> ShowBillDetails(string Billid);
        bool SaveBillEdit(BillEditDTO objBillEdit, List<BillEditDTO> lstKotDetails);


        #endregion


        #region Dash Board

        List<DashboardDTO> ShowAlltables(int userid);
        DashboardDTO ShowKOtsCount(int userid);
        string CheckBillStatus(string tableid);




        #region Charts
        List<DashboardDTO> ChartListForCategorywiseToday(int userid);



        List<DashboardDTO> ChartListForCategorywiseWeek(int userid);

        List<DashboardDTO> ChartListForCategorywiseMonth(int userid);

        #endregion
        #endregion


        #region KOT Cancel
        List<KOTCancel> ShowKot(string TableNO);
        List<KOTCancel> ShowReason();
        List<KOTCancel> ShowKotCancelGridData(string TableNO, string KotId);
        bool SaveKotCancel(List<KOTCancel> KTC);
        #endregion

        #region Issue Voucher
        List<CutomerMaster> ShowCutomerNames();
        List<UserMaster> ShowUserNames();
        List<Vouchertype> ShowVouchers(string VouchName, string VouchID);
        List<Vouchertype> ShowVoucherTypes();
        bool SaveIssueVoucher(IssueVoucher ISV);
        #endregion

        #region  KOT TABLE CHANGE
        //table change 
        KOTTablesDTO ShowFromTableNoDetails(string tableId);
        List<KOTTablesDTO> ShowToTableNoDetails(string Table);
        List<KOTTablesDTO> ShowToTableNoDetailsRunning(string tab);
        int SaveFromToTableDetails(KOTTablesDTO tab, long Createdby, string tableclick);

        List<KOTTablesDTO> ShowToMergeTableNoDetails(string Table);
        #endregion

        #region Merge Table

        List<MergingTables> GetTables(string TableNo, int Userid);
        DataTable GetkotNo(string JsonData);
        bool SaveMerge(string tableId, List<Mergetables> myDeserializedObjList);
        #endregion

        #region KOT REPRINT

        List<KOTTakingDTO> ShowReprintKot(string tablid);

        bool SaveKotReprint(KOTTakingDTO objKotReprint);

        #endregion

        List<KOTTablesDTO> ShowToMergeTableNoDetailsKotTransfer(string tab);
        List<KOTTablesDTO> ShowToTableNoDetailsRunningKotTransfer(string tab);


        #region Srinath
        List<DashboardDTO> ChartListForCategorywisePeriod(int Userid, String Period);
        List<DashboardDTO> ChartListForSessionwisePeriod(int Userid, string Period);

        List<DashboardDTO> ChartListForSectionwisePeriod(int Userid, string Period);

        #endregion

        


        #region Material Indent By Vikram Chathilla

        //List<MaterialIndentDTO> ShowIndentProducts();

        //List<MaterialIndentDTO> ShowStorage();

        //List<MaterialIndentDTO> GetSelfdetails(string showroomselected);

        //List<MaterialIndentDTO> showrequestedby();

        //List<MaterialIndentDTO> GetExistingIndentNo(string IndentType);

        //List<MaterialIndentDTO> GetExistingIndents(string Indents);

        //List<MaterialIndentDTO> GetProductAvailability(string productid, string productname);

        //bool SaveIndent(List<MaterialIndentDTO> griddata, MaterialIndentDTO BE);

        //List<MaterialIndentDTO> ShowDepartment();


        //  bool SaveIndent(MaterialIndentDTO BS, List<MaterialIndentDTO> griddata, MaterialIndentDTO BE);

        // List<MaterialIndentDTO> ShowUOM1();

        #endregion








        bool SaveCompanyDetails(BillSettlementDTO C);

        DataTable getComapnynamesOfBilltocomapny();
    }
}
