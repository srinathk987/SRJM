using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core
{
    public class POSTransaction
    {

    }
    public class KOTTakingDTO
    {
        public int RecordId { get; set; }
        public string SessionName { get; set; }
        public string SessionId { get; set; }
        public string SectionName { get; set; }
        public string SectionCode { get; set; }
        public int Sectionid { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Host { get; set; }
        public int TableNo { get; set; }



        public string TableName { get; set; }
        public string TableCode { get; set; }
        public int Covers { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }


        public int Rate { get; set; }
        public int Price { get; set; }

        public string Remarks { get; set; }
        public string value { get; set; }
        public string text { get; set; }
        public int Adults { get; set; }
        public int Kids { get; set; }
        public int ItemId { get; set; }

        public string KotName { get; set; }
        public int TableId { get; set; }
        public string Reason { get; set; }
        public int KotId { get; set; }
        public string vchKotId { get; set; }

        public decimal RateFixed { get; set; }

        public int createdby { get; set; }

        public int statusid { get; set; }

        public string KotDate { get; set; }
        public string QuantityHidden { get; set; }
        public string Status { get; set; }




    }
    public class KotChangeDTO
    {
        public int RecordId { get; set; }
        public int TableId { get; set; }
        public string TableCode { get; set; }
        public int Covers { get; set; }
        public int Kids { get; set; }
        public int Adults { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
        public int KotId { get; set; }
        public string KotName { get; set; }
        public string Reason { get; set; }
    }

    public class BillSettlementDTO
    {
        public string SessionId { get; set; }
        public string KotDate { get; set; }
        public string KotTime { get; set; }
        public decimal Discount { get; set; }
        public decimal VoucherDiscount { get; set; }
        public string Authorisedby { get; set; }
        public string Status { get; set; }

        public int ReasonId { get; set; }
        public string ReasonName { get; set; }
        public string TableId { get; set; }

        public string ReasonForOrdercancel { get; set; }
        public string TableName { get; set; }
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public string VDiscountPercentage { get; set; }
        public string VDiscountAmount { get; set; }
        public int RecordId { get; set; }
        public string BillNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalVoucherDiscount { get; set; }
        public decimal Gross { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal ServiceTaxValue { get; set; }
        public decimal ServiceTaxPercentage { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal ServiceChargesValue { get; set; }
        public decimal ServiceChargesPercentage { get; set; }

        public decimal NetAmount { get; set; }
        public string KotId { get; set; }
        public string VchKotId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal ItemQty { get; set; }
        public decimal ItemRate { get; set; }
        public string ItemChangableornot { get; set; }
        public string Discounttype { get; set; }
        public decimal Dicount { get; set; }
        public string Select { get; set; }
        public decimal Amount { get; set; }

        public Decimal CardAmount { get; set; }
        public Decimal CashAmount { get; set; }

        public Decimal PaidAmount { get; set; }
        public Decimal BalanceAmount { get; set; }

        CardDetails CardDetails { get; set; }
        CashDetails CashDetails { get; set; }


        public int statusid { get; set; }

        public long createdby { get; set; }

        public string HDStatus { get; set; }
        public string HDNO { get; set; }

        public string Deliverycharges { get; set; }

        public string VatTax { get; set; }

        public string VatTaxvalue { get; set; }
        public string ServiceTaxOnServiceCharge { get; set; }
        public string CompanyName { get; set; }
        public string BilltoCompany { get; set; }

        public string Address { get; set; }
    
        public string ContactPersonName { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }



    }

    public class CardDetails
    {
        public string RecordId { get; set; }
        public string BillNo { get; set; }
        public string CardNo { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string ExpireDate { get; set; }
        public Decimal CardAmount { get; set; }
        public string TransactionNo { get; set; }
    }
    public class VoucherDetails
    {
        public int RecordId { get; set; }
        public string BillNo { get; set; }
        public int CardNo { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string ExpireDate { get; set; }
        public Decimal CardAmount { get; set; }
    }
    public class CashDetails
    {
        public int RecordId { get; set; }
        public string BillNo { get; set; }
        public int Thousand { get; set; }
        public int fiveHundred { get; set; }
        public int Hundred { get; set; }
        public int Fifty { get; set; }
        public int Twenty { get; set; }
        public int Ten { get; set; }
        public int Five { get; set; }
        public int Two { get; set; }
        public int One { get; set; }
        public int Half { get; set; }
        public Decimal CashAmount { get; set; }
    }


    //public class KOTTakingDTO
    //{
    //    public int RecordId { get; set; }
    //    public string SessionName { get; set; }
    //    public string SessionId { get; set; }
    //    public string SectionName { get; set; }
    //    public string SectionCode { get; set; }
    //    public int Sectionid { get; set; }
    //    public string Date { get; set; }
    //    public string Time { get; set; }
    //    public string Host { get; set; }
    //    public int TableNo { get; set; }

    //    public string TableName { get; set; }
    //    public string TableCode { get; set; }
    //    public int Covers { get; set; }
    //    public string ItemName { get; set; }
    //    public string ItemCode { get; set; }
    //    public int Quantity { get; set; }
    //    public string Remarks { get; set; }
    //    public string value { get; set; }
    //    public string text { get; set; }
    //    public int Adults { get; set; }
    //    public int Kids { get; set; }
    //    //public int Itemid { get; set; }



    //}
    public class KotAvailabletables
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class MergingTables
    {
        public string id { get; set; }
        public string label { get; set; }
    }
    public class Mergetables
    {


        public string value { get; set; }
        public string text { get; set; }

        public string id { get; set; }
        public string Kotid { get; set; }
        public string tablesname { get; set; }
        public long createdby { get; set; }

    }

    public class Thirpartysettlement
    {

        public string nooforders { get; set; }

        public string numdueamount { get; set; }


        public string numorderamount { get; set; }

        public string margin { get; set; }
        public string netpayable { get; set; }
        public string vchmargintype { get; set; }
        public string vchpartyid { get; set; }
        public string datorderdate { get; set; }
        public string nummargin { get; set; }




        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public string Chequedate { get; set; }
        public string Date { get; set; }
        public string DueAmount { get; set; }
        public string ModeOfPayment { get; set; }
        public string ThirdPartyNames { get; set; }
        public string ThirpartyName { get; set; }
        public string TotalAmount { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionNo { get; set; }
        public string PaidAmount { get; set; }
        public string PaidAmountforUpdate { get; set; }
        public string statusid { get; set; }
        public string createdby { get; set; }
        public string vchorderno { get; set; }
        public string PaidAfterDiscountAdd { get; set; }
        public string TotalBillAmount { get; set; }
        public string MarginAmount { get; set; }


        public string numnet { get; set; }
        public string datdate { get; set; }
        public string total { get; set; }
        public string vchbillno { get; set; }
        public string CompanyName { get; set; }
        public string fromdate { get; set; }
        public string checkedstatus { get; set; }
        public string paidatstarting { get; set; }


















    }

    public class CustomerDetails
    {

        public string CustomerAddress { get; set; }
        public string CustomerLandline { get; set; }
        public string CustomerName { get; set; }
        public string Customermobile { get; set; }
        public string Email { get; set; }
        public string LandMark { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string Discount { get; set; }
        public string DeliverCharges { get; set; }
        public string OrderTotal { get; set; }
        public string OrderNetTotal { get; set; }
        public int createdby { get; set; }
        public int statusid { get; set; }
        public static string CustomerIDGen { get; set; }
        public string discounttype { get; set; }
        public string ModeOfPayment { get; set; }
        public char vchdeliverychargesstatus { get; set; }
        public char NoDeliverycharges { get; set; }
        public string PinCode { get; set; }
        public string OrderUpdate { get; set; }
        public string orderno { get; set; }
        public string OrderPreviousBillNo { get; set; }
        public string ThirpartyName { get; set; }
        public string ThirdpartyId { get; set; }
        public string ThirdpartyRecorId { get; set; }
        public string servicetax { get; set; }
        public string servicetaxvalue { get; set; }
        public string VatTax { get; set; }
        public string VatTaxAmount { get; set; }
        public string OtherCharges { get; set; }








    }

    public class ItemsDTO
    {
        public string Item { get; set; }
        public string ActualRate { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
        public string Extras { get; set; }
        public int ItemId { get; set; }
        public decimal Rate { get; set; }
        public decimal RateFixed { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; }
        public int TableNo { get; set; }
        public int rowindex { get; set; }
        public string TableName { get; set; }
        public string KotName { get; set; }
        public string Department { get; set; }
        public string QuantityHidden { get; set; }









        public string subcategoryname { get; set; }
    }
    public class BillGeneration
    {


        public long sessionid { get; set; }
        public long sectionid { get; set; }
        public string billdate { get; set; }
        public string billtime { get; set; }
        public string vchhostname { get; set; }
        public long tableid { get; set; }
        public string tablesname { get; set; }
        public decimal numamount { get; set; }
        public string noncharge { get; set; }

        public long ReasonId { get; set; }

        public string ReasonName { get; set; }


        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public decimal VDiscountPercentage { get; set; }
        public decimal VDiscountAmount { get; set; }
        public int recordid { get; set; }
        public string billno { get; set; }
        public decimal totalamount { get; set; }

        public decimal discount { get; set; }
        public decimal VDiscountvalue { get; set; }


        public decimal totaldiscount { get; set; }
        public decimal totalvoucherdiscount { get; set; }
        public decimal gross { get; set; }
        public decimal servicetax { get; set; }
        public decimal servicecharges { get; set; }
        public decimal servicetaxvalue { get; set; }
        public decimal servicechargesvalue { get; set; }
        public decimal vamount { get; set; }
        public decimal netamount { get; set; }
        public long kotid { get; set; }
        public string vchkotid { get; set; }
        public long itemid { get; set; }
        public string itemname { get; set; }
        public string itemcode { get; set; }
        public decimal itemqty { get; set; }
        public decimal itemrate { get; set; }
        public string itemchangableornot { get; set; }
        public string discounttype { get; set; }
        public decimal discountamount { get; set; }

        public string IndDiscountType { get; set; }
        public decimal IndiviItemQty { get; set; }

        public string select { get; set; }

        public string chkselect { get; set; }

        public string chknoncharge { get; set; }

        public int statusid { get; set; }
        public long createdby { get; set; }

        public string vchauthorisedby { get; set; }

        public string VATtax { get; set; }
        public string VAtaxvalue { get; set; }
        public string ServiceTaxOnServiceCharge { get; set; }




    }
    #region BILL EDIT

    public class BillEditDTO
    {
        public int ReasonId { get; set; }
        public string ReasonName { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public int VoucherId { get; set; }
        public string VoucherName { get; set; }
        public decimal VDiscountPercentage { get; set; }
        public decimal VDiscountAmount { get; set; }
        public int RecordId { get; set; }
        public string BillNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalVoucherDiscount { get; set; }
        public decimal Gross { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal ServiceTaxValue { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal ServiceChargesValue { get; set; }
        public decimal NetAmount { get; set; }
        public string KotId { get; set; }
        public string VchKotId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal ItemQty { get; set; }
        public decimal ItemRate { get; set; }
        public string ItemChangableornot { get; set; }
        public string Discounttype { get; set; }
        public decimal Dicount { get; set; }
        public string Select { get; set; }
        public decimal Amount { get; set; }
        public string AuthorisedBy { get; set; }
        public string Session { get; set; }
        public string NonchargableReason { get; set; }

        public string IndDiscountType { get; set; }
        public decimal IndiviItemQty { get; set; }




        public int statusid { get; set; }

        public long createdby { get; set; }



        public string VatTax { get; set; }

        public string VAtaxvalue { get; set; }
        public string ServiceTaxOnServiceCharge { get; set; }
    }


    #endregion

    #region Dash Board
    public class DashboardDTO
    {



        public int Tableid { get; set; }

        public string Tablesid { set; get; }


        public string[] Mytableid
        {
            get;
            set;
        }

        public string[] MytableName
        {
            get;
            set;
        }

        public string SectioName { get; set; }
        public string TableName { get; set; }
        public string TableStatus { get; set; }
        public int Totalkots { get; set; }
        public int CompletedKots { get; set; }
        public int RunningKots { get; set; }
        public int CancelKots { get; set; }
        public string PlanName { get; set; }
        public decimal PaymentAmount { get; set; }







        //public int Tableid { get; set; }
        //public string TableName { get; set; }
        //public string TableStatus { get; set; }
        //public int Totalkots { get; set; }
        //public int CompletedKots { get; set; }
        //public int RunningKots { get; set; }
        //public int CancelKots { get; set; }
        //public string PlanName { get; set; }
        //public decimal PaymentAmount { get; set; }

        public string Categoryname { get; set; }

        public string Uomname { get; set; }

        public string Pluname { get; set; }

        public string Unitprice { get; set; }

        public string Availableqty { get; set; }
    }

    #endregion

    #region KOT Cancel

    public class KOTCancel
    {
        public int RecordId { get; set; }
        public string SessionName { get; set; }
        public int SessionId { get; set; }
        public string SectionName { get; set; }
        public string SectionCode { get; set; }
        public int Sectionid { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Host { get; set; }
        public int TableNo { get; set; }

        public string Status { set; get; }

        public string TableName { get; set; }
        public string TableCode { get; set; }
        public int Covers { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }

        public double Rate { get; set; }
        public int Price { get; set; }

        public string Remarks { get; set; }
        public string value { get; set; }
        public string text { get; set; }
        public int Adults { get; set; }
        public int Kids { get; set; }
        public int ItemId { get; set; }

        public string KotName { get; set; }
        public int TableId { get; set; }
        public string Reason { get; set; }
        public int KotId { get; set; }
        public string vchKotId { get; set; }

        public decimal RateFixed { get; set; }

        //public int createdby { get; set; }

        public int statusid { get; set; }

        public string KotDate { get; set; }
        public string Kot { get; set; }

        public long createdby { get; set; }














        //public string TableNo { get; set; }
        //public string KotId { get; set; }
        public string ReasonID { get; set; }
        //public string Reason { get; set; }
        // public string Kot { get; set; }
        public string Item { get; set; }
        public string Itemid { get; set; }
        //public string KotName { get; set; }
        //public string ItemCode { get; set; }
        public string ItemQty { get; set; }
        //public string Extras { get; set; }

        //public int statusid { get; set; }

        //public long createdby { get; set; }
    }

    #endregion

    #region Issue Voucher
    public class IssueVoucher
    {
        //CustomerName,ContactNO,Date,Time==Customer
        //VoucherType,IssuedBy,VoucherNO,AuthorizePerson,Remarks=====Issue Voucher Details
        //VoucherValue,Vat,ServiceTax,ServiceCharge,NetPayable=====Tax Details
        //PaymentSplit,PaymentMode==Payment Main
        //CardNo,CardType,NameOnCard,CardExpDate,CardAmount ==Card Payment
        //CouponNo,TypeofCoupon,CouponnName,CouponExpDate,CouponAmount===Coupon Payment
        //Thousand,FiveHundred,Hundred,Fifty,Twenty,Ten,Five,Two,One,FiftyPaisa,CashTotal==Cash Payment


        //CustomerName,ContactNO,Date,Time==Customer
        public string CustomerName { get; set; }
        public string ContactNO { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        //CustomerName,ContactNO,Date,Time==Customer


        //VoucherType,IssuedBy,VoucherNO,AuthorizePerson,Remarks=====Issue Voucher Details
        public string VoucherType { get; set; }
        public string IssuedBy { get; set; }
        public string VoucherNO { get; set; }
        public string AuthorizePerson { get; set; }
        public string Remarks { get; set; }
        //VoucherType,IssuedBy,VoucherNO,AuthorizePerson,Remarks=====Issue Voucher Details


        //VoucherValue,Vat,ServiceTax,ServiceCharge,NetPayable=====Tax Details
        public string VoucherValue { get; set; }
        public string Vat { get; set; }
        public string ServiceTax { get; set; }
        public string ServiceCharge { get; set; }
        public string NetPayable { get; set; }
        public string Charge { get; set; }
        //VoucherValue,Vat,ServiceTax,ServiceCharge,NetPayable=====Tax Details


        //PaymentSplit,PaymentMode==Payment Main
        public string PaymentSplit { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentType { get; set; }
        //PaymentSplit,PaymentMode==Payment Main


        //CardNo,CardType,NameOnCard,CardExpDate,CardAmount ==Card Payment
        public string CardNo { get; set; }
        public string CardType { get; set; }
        public string NameOnCard { get; set; }
        public string CardExpDate { get; set; }
        public string CardAmount { get; set; }
        //CardNo,CardType,NameOnCard,CardExpDate,CardAmount ==Card Payment


        //CouponNo,TypeofCoupon,CouponnName,CouponExpDate,CouponAmount===Coupon Payment
        public string CouponNo { get; set; }
        public string TypeofCoupon { get; set; }
        public string CouponName { get; set; }
        public string CouponExpDate { get; set; }
        public string CouponAmount { get; set; }
        //CouponNo,TypeofCoupon,CouponName,CouponExpDate,CouponAmount===Coupon Payment


        //Thousand,FiveHundred,Hundred,Fifty,Twenty,Ten,Five,Two,One,FiftyPaisa,CashTotal==Cash Payment
        public string Thousand { get; set; }
        public string FiveHundred { get; set; }
        public string Hundred { get; set; }
        public string Fifty { get; set; }
        public string Twenty { get; set; }
        public string Ten { get; set; }
        public string Five { get; set; }
        public string Two { get; set; }
        public string One { get; set; }
        public string FiftyPaisa { get; set; }
        public string CashTotal { get; set; }

        public Decimal PaidAmount { get; set; }
        public Decimal BalanceAmount { get; set; }

        //Thousand,FiveHundred,Hundred,Fifty,Twenty,Ten,Five,Two,One,FiftyPaisa,CashTotal==Cash Payment

        public int statusid { get; set; }
        public long createdby { get; set; }
    }

    public class CutomerMaster
    {
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerTime { get; set; }
        public string CustomerDate { get; set; }
    }

    public class UserMaster
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }

    public class Vouchertype
    {
        public string VoucherTypeName { get; set; }
        public string ChargeApplicable { get; set; }
        public string VoucherTypeId { get; set; }
        public string VoucherDescription { get; set; }
        public string VoucherValue { get; set; }
    }

    #endregion


    public class KOTTablesDTO
    {
        public string TOID { get; set; }
        //table change
        public string fromTableName { get; set; }
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

        public int SectionId { get; set; }


        public string TableName { get; set; }

        public long createdby { get; set; }
    }




    #region Kot Transfer


    public class KotTransferDto
    {
        public string KotId { set; get; }
    }



    #endregion

    public class IndentDTO
    {
        public string vchindentno { get; set; }
        public string datindentdate { get; set; }
        public string vchindenttype { get; set; }
        public string vchrequestedby { get; set; }
        public string vchapprovalby { get; set; }
        public string productid { get; set; }
        public string productname { get; set; }
        public string productcategoryname { get; set; }
        public string productsubcategoryname { get; set; }
        public string producttypename { get; set; }
        public string productcode { get; set; }
        public string categoryname { get; set; }
        public string subcategoryname { get; set; }
        public string producttype { get; set; }
        public string vchuom { get; set; }
        public string uom { get; set; }
        public string vchuomid { get; set; }
        public string minqty { get; set; }
        public string maxqty { get; set; }
        public string issuedqy { get; set; }
        public string storagelocation { get; set; }
        public string numindentqty { get; set; }
        public string numavailabilityqty { get; set; }
        public string issueduom { get; set; }
        public string vchissueno { get; set; }

        public string IssuedBy { get; set; }
        public string productcategoryid { get; set; }
        public string productsubcategoryid { get; set; }
        public string numindentconvertionqty { get; set; }
        public string shelfid { get; set; }
        public string shelfname { get; set; }
        public string vchsaleuomid { get; set; }
        public string numsaleuomconvertion { get; set; }
        public string userid { get; set; }
        public string ApprovalBy { get; set; }
        public string IssueType { get; set; }

        public string storagelocationid { get; set; }
        public string storagelocationname { get; set; }

        public IndentDTO person { get; set; }
        public string DeparmentName { get; set; }
        public string previssueqty { get; set; }
        public string ConversionValue { get; set; }
        public string numissueconvertionqty { get; set; }

        public string DirOrIndent { get; set; }
        public string fromstorage { get; set; }
        public string tostorage { get; set; }
        public string vchdate { get; set; }
    }

    //public class IndentDTO
    //{
    //    public string vchindentno { get; set; }
    //    public string datindentdate { get; set; }
    //    public string vchindenttype { get; set; }
    //    public string vchrequestedby { get; set; }
    //    public string vchapprovalby { get; set; }
    //    public string productid { get; set; }
    //    public string productname { get; set; }
    //    public string productcategoryname { get; set; }
    //    public string productsubcategoryname { get; set; }
    //    public string producttypename { get; set; }
    //    public string productcode { get; set; }
    //    public string categoryname { get; set; }
    //    public string subcategoryname { get; set; }
    //    public string producttype { get; set; }
    //    public string vchuom { get; set; }
    //    public string uom { get; set; }
    //    public string minqty { get; set; }
    //    public string maxqty { get; set; }
    //    public string issuedqy { get; set; }
    //    public string storagelocation { get; set; }
    //    public string numindentqty { get; set; }
    //    public string numavailabilityqty { get; set; }
    //    public string issueduom { get; set; }
    //    public string vchissueno { get; set; }

    //    public string IssuedBy { get; set; }
    //    public string productcategoryid { get; set; }
    //    public string productsubcategoryid { get; set; }
    //    public string numindentconvertionqty { get; set; }
    //    public string shelfid { get; set; }
    //    public string shelfname { get; set; }
    //    public string vchsaleuomid { get; set; }
    //    public string numsaleuomconvertion { get; set; }
    //    public string userid { get; set; }
    //    public string ApprovalBy { get; set; }
    //    public string IssueType { get; set; }

    //    public string storagelocationid { get; set; }
    //    public string storagelocationname { get; set; }
    //    public IndentDTO person { get; set; }
    //    public string DeparmentName { get; set; }
    //    public string previssueqty { get; set; }
    //    public string ConversionValue { get; set; }
    //    public string numissueconvertionqty { get; set; }

    //    public string DirOrIndent { get; set; }
    //}



    #region Home Delivery Charges
    public class DeliveryChargesDTO
    {
        public int recordid { get; set; }
        public string vchpincode { get; set; }
        public decimal vchcharges { get; set; }
        public int vchstatus { get; set; }
        public int createdby { get; set; }
        public DateTime createddate { get; set; }
        public int modifiedby { get; set; }
        public DateTime modifieddate { get; set; }


    }
    #endregion



    #region  Indent Details

    //public class MaterialIndentDTO
    //{
    //    //BE 
    //    public string IndentNo { get; set; }
    //    public string Indenttypenew { get; set; }
    //    public string Indenttypeexisting { get; set; }
    //    public DateTime vchdate { get; set; }
    //    public string RequestedBy { get; set; }
    //    public string vchindentno { get; set; }
    //    public string ApprovedBy { get; set; }
    //    public decimal? indentqty { get; set; }

    //    // Grid Fields
    //    public string productname { get; set; }
    //    public string categoryname { get; set; }
    //    public string subcategoryname { get; set; }
    //    public string producttype { get; set; }
    //    public string uomname { get; set; }
    //    public decimal? minqty { get; set; }
    //    public decimal? maxqty { get; set; }
    //    public string storagelocationname { set; get; }
    //    public string storagelocationcode { get; set; }
    //    public string shelfname { get; set; }
    //    public decimal? AvailableQty { get; set; }
    //    public int createdby { set; get; }
    //    public int productid { set; get; }
    //    public int productcategoryid { get; set; }
    //    public int productsubcategoryid { get; set; }
    //    public string productcode { get; set; }
    //    public string vchuomid { get; set; }
    //    public string uomid { get; set; }
    //    public string uom { get; set; }
    //    public int recordid { get; set; }

    //    public string storagetext { get; set; }

    //    public int deptid { get; set; }
    //    public string DeparmentName { get; set; }
    //    public string DeparmentCode { get; set; }

    //}


    #endregion

    public class ThirdParty
    {

        public string VendorImage { get; set; }
        public string Address { get; set; }
        public string MarginAmount { get; set; }
        public string FlatOrPercentage { get; set; }
        public string fromdate { get; set; }
        public string Duration { get; set; }
        public string ModeOfPayment { get; set; }
        public string ContactNo { get; set; }
        public string ThirdPartyName { get; set; }
        public string Margintype { get; set; }

        public int statusid { get; set; }

        public int createdby { get; set; }
    }
}
