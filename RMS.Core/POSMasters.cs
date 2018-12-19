using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core
{
    public class POSMasters
    {
    }


    public class SessionDTO
    {
        public string text { set; get; }
        public string type { set; get; }

        public string categoryid { set; get; }
        public string subcategoryid { set; get; }

        public string categoryname { set; get; }
        public string subcategoryname { set; get; }

        public string itemid { set; get; }
        public string itemname { set; get; }


        public string Selected { set; get; }


    }


    public class DepartmentDTO
    {
        public int RecordId { get; set; }
        public string DeparmentName { get; set; }
        public string DeparmentCode { get; set; }

        public int statusid { get; set; }

        public long createdby { get; set; }
    }

    public class DeliveryBoyDTO
    {



        public string BoyName { get; set; }

        public string MobileNo { get; set; }

        public string VehicleNo { get; set; }


        public string EmployeeType { set; get; }

        public string DeliveryBoyId { get; set; }





    }

    public class VoucherDTO
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherType { get; set; }
        public string Percentage { get; set; }
        public string Amount { get; set; }
        public string ValidPeriod { get; set; }
        public string ValidUpto { get; set; }
        public string Description { get; set; }
        public string TermsandConditions { get; set; }
        // public string Statusid { get; set; }
        public string Total { get; set; }
        public string Type { get; set; }

        public int statusid { get; set; }

        public long createdby { get; set; }
    }
    public class DTO1
    {
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherType { get; set; }
        public string Percentage { get; set; }
        public string Amount { get; set; }
        public string ValidPeriod { get; set; }
        public string ValidUpto { get; set; }
        public string Description { get; set; }
        public string TermsandConditions { get; set; }

        public string Total { get; set; }
        public string Type { get; set; }

        public int statusid { get; set; }

        public long createdby { get; set; }
    }


    public class PrinterDTO
    {
        public int PrinterId { get; set; }
        public string PrinterName { get; set; }
        public string PrinterCode { get; set; }
        public string IpAddress { get; set; }
        public string ProtNo { get; set; }
        public string DeparmentName { get; set; }
        public string DeparmentID { get; set; }
        public int statusid { get; set; }

        public long createdby { get; set; }
    }

    public class TaxDTO
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public string Percentage { get; set; }
        //public string StatusId { get; set; }

        public int statusid { get; set; }

        public long createdby { get; set; }
    }

    public class SectionDTO
    {
        public int SectionId { get; set; }
        public string SectionName123 { get; set; }
        public string SectionCode { get; set; }
        //public string StatusId { get; set; }


        public int statusid { get; set; }

        public long createdby { get; set; }
    }


    public class TablesDTO
    {
        public int TableId { get; set; }
        public string TablesName { get; set; }
        public string TableCode { get; set; }
        public string Convers { get; set; }
        public string SectionId { get; set; }
        public string SectionName { get; set; }
        public string SectionCode { get; set; }

        //table change

        public string fromtableid { get; set; }
        public int numfromcovers { get; set; }
        public int numfromadults { get; set; }
        public int numfromkids { get; set; }
        public string kotid { get; set; }

        public string totableid { get; set; }
        public int numtocovers { get; set; }
        public int numtoadults { get; set; }
        public int numtokids { get; set; }
        public int statusid { get; set; }

        public long createdby { get; set; }
    }
    public class PaymentsDTO
    {
        public int paymenttypeid { get; set; }
        public string paymentmodename { get; set; }
        public string paymentmodecode { get; set; }
        public string chargesAppl { get; set; }
        public string taxtype { get; set; }
        public int percentage { get; set; }
        public double amount { get; set; }


        public int statusid { get; set; }

        public long createdby { get; set; }
    }
    public class VoucherTypeDTO
    {
        public int RecordId { get; set; }
        public string VoucherTypeName { get; set; }
        public string VoucherTypeCode { get; set; }
        public string SectionName { get; set; }

        public int statusid { get; set; }

        public long createdby { get; set; }
    }

    #region ItemCategoryMaster

    public class ItemCategoryDTO
    {
        public int CategoryId { set; get; }
        public string CategoryName { set; get; }
        public string CategoryCode { set; get; }

        public string EditCheck { set; get; }
        public long createdby { get; set; }

        public int statusid { get; set; }
    }
    #endregion


    #region ItemSubcategory



    public class ItemSubCategoryDTO
    {

        public int SubCategoryId { set; get; }
        public string CategoryId { set; get; }
        public string CategoryCode { set; get; }
        public string Category { set; get; }
        public string ItemSubcategory { set; get; }
        public string ItemSubcategoryCode { set; get; }



        public long createdby { get; set; }

        public int statusid { get; set; }
    }

    #endregion

    #region CodeMaster

    public class CodeMastreDTO
    {
        public string MasterName { set; get; }
        public string Prefix { set; get; }
    }

    #endregion
}
