using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RMS.Core.Interfaces
{
    public interface IPOSMasters
    {





        bool DeleteItemcategoryCheck(ItemCategoryDTO objItemCategoryDTO);

        #region Origin

        DataTable ShowOrigin();
        int SaveOrigin(string Json, string createby);
        int UpdateOrigin(string Json, string createby);
        bool DeleteOrigin(string ID, string createby);


        #endregion


        #region  session

        bool SaveSession(List<SessionDTO> myDeserializedObjList, string Json, string Createdby);
        bool UpdateSession(List<SessionDTO> myDeserializedObjList, string Json, string Id, string Createdby);
        bool DeleteSessionData(string Id, string Createdby);

        DataTable getTreeViewData();
        DataTable getTreeBtnUpdateViewData(string SessionId);
        DataTable GetSessionData();

        int GetSessionCount(int From, int To, string SessionId, string Text);

        #endregion




        #region Item Master
        DataTable GetPrintersData();
        DataTable GetOriginDropDownData();
        DataTable GetDepartmentDropDownData();
        DataTable GetCategoryDropDownData();
        DataTable GetSubCategoryDropDownData(string Id);
        DataTable GetTaxDropdownData();
        DataTable GetRecommendedDrinkData();

        bool SaveItemMaster(string ItemData, string createby);
        DataTable ShowItemmaster();
        bool UpdateItemMaster(string ItemData, string createby);

        int CountUpdateItemMaster(string ItemName);

        bool DeleteItemMaster(string id, string createby,string Name);
        int GetItemExistCount(string ItemName);


        #endregion


        #region Voucher...

        List<VoucherDTO> ShowVoucher();
        int SaveVoucher(VoucherDTO Dept);
        int UpdateVoucher(VoucherDTO Dept);
        bool DeleteVoucher(int voucherid);

        #endregion

        #region Department...

        List<DepartmentDTO> ShowDepartment();
        int SaveDepartment(DepartmentDTO Dept);
        int UpdateDepartment(DepartmentDTO Dept);
        bool DeleteDepartment(int RecordId);

        #endregion

        #region Printer...

        List<PrinterDTO> ShowPrinter();
        int SavePrinter(PrinterDTO Dept);
        int UpdatePrinter(PrinterDTO Dept);
        bool DeletePrinter(int PrinterId);

        #endregion


        #region Tax...
        List<TaxDTO> ShowTaxDetails();
        int SaveTax(TaxDTO Tx);
        int UpdateTax(TaxDTO Tx);
        int CountUpdateTax(string name);
        bool DeleteTax(int id, string name);
        #endregion

        #region Section...
        List<SectionDTO> ShowSection();
        int SaveSection(string Sec, string cre);
        bool DeleteSection(int SectionId);
        int UpdateSection(string Sec, string cre);
        #endregion

        #region TableandCovers...
        List<TablesDTO> ShowTableandCovers();
        int SaveTableandCovers(TablesDTO sec);
        int UpdateTableandCovers(TablesDTO sec);
        bool DeleteTableandCovers(int Tablesid);
        string checkStatustoDelete(int Tablesid);

        #endregion

        #region Reasons


        DataTable ShowReasons();

        int SaveReasons(string Json, string createby);
        int UpdateReasons(string Json, string createby);
        bool DeleteReasons(string ID, string createby);

        #endregion

        #region PaymentType
        List<PaymentsDTO> showPayments();
        int SavePayments(PaymentsDTO payment);
        int UpdatePayments(PaymentsDTO payment);
        bool DeletePayments(PaymentsDTO payment);
        #endregion

        #region ItemCategory...

        List<ItemCategoryDTO> ShowItemCategory();
        int SaveItemCategory(ItemCategoryDTO ItemCategory);
        int UpdateItemCategory(ItemCategoryDTO ItemCategory);
        bool DeleteItemCategory(int CategoryId);
        #endregion

        #region ItemSubCategory...

        List<ItemSubCategoryDTO> ShowItemSubCategory();
        int SaveItemSubCategory(ItemSubCategoryDTO ItemSubCategory);
        int UpdateItemSubCategory(ItemSubCategoryDTO ItemSubCategory);
        bool DeleteItemSubCategory(int SubCategoryId);
        #endregion

        #region VoucherType
        List<VoucherTypeDTO> ShowVoucherType();
        int SaveVoucherType(VoucherTypeDTO VchType);
        int UpdateVoucherType(VoucherTypeDTO VchType);
        bool DeleteVoucherType(int RecordId);
        List<VoucherTypeDTO> DDlVoucherType();
        #endregion

        #region CODEMASTER
        bool SaveCodeMaster(List<CodeMastreDTO> code);
        #endregion

        #region DeliveryBoyDetails



        bool SaveDeliveryBoyDetails(DeliveryBoyDTO DeliveryBoyDTO, int createdby, int statusid, string Employeetype);

        List<DeliveryBoyDTO> ShowDeliverboydetailsbyid();

        int UpdateDeliverboydetails(DeliveryBoyDTO DeliveryBoyDTO);

        bool DeleteDeliverboydetails(string ID);

        #endregion

        #region Table Selection

        List<DashboardDTO> ShowAlltablesformodel();
        List<DashboardDTO> GetrunningTablenames(string TableNames);
        List<DashboardDTO> RunningStatusTableNames(string TableNames);
        DataTable GetHostNames();

        List<DashboardDTO> ShowAssignedtables(int Userid);

        bool SaveTableDetails(string TableName, int Createdby, string Fromdate, string Todate);

        bool UpdateAssigenedTables(string TableNames, int hostid);
     
        #endregion



        bool SaveCreateDayspecial(List<SessionDTO> myDeserializedObjList, string JsonDatatab1, string createdby);

        DataTable getTreeViewDataDayspecial(string SessionId);

        bool UpdateDaySpecial(List<SessionDTO> myDeserializedObjList, string JsonDatatab1, string SId, string createdby);
    }
}
