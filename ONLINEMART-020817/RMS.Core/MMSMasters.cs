using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core
{
    public class MMSMasters
    {
    }

    #region ProductCategoryMaster...

    public class ProductCategoryDTO
    {
        public int CategoryId { set; get; }
        public string CategoryName { set; get; }
        public string CategoryCode { set; get; }
        public long createdby { get; set; }
        public int statusid { get; set; }
    }

    #endregion

    #region ProductSubcategoryMst...

    public class ProductSubCategoryDTO
    {

        public int SubCategoryId { set; get; }
        public string CategoryId { set; get; }
        public string CategoryCode { set; get; }
        public string Category { set; get; }
        public string Subcategory { set; get; }
        public string SubcategoryCode { set; get; }
        public long createdby { get; set; }
        public int statusid { get; set; }

    }

    #endregion

    #region Tax...

    public class ProductTaxDTO
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public string Percentage { get; set; }
        public int statusid { get; set; }
        public long createdby { get; set; }

    }

    #endregion

    #region Country....
    public class CountryDTO
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string Description { get; set; }

    }

    #endregion

    #region State...

    public class StateDTO
    {

        public int StateId { get; set; }
        public string state { get; set; }
        public string CountryId { get; set; }
        public string Country { get; set; }



    }

    #endregion

    #region City...

    public class City
    {
        public string CityName { get; set; }
        public string CityId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Description { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
        public string StatusId { get; set; }
        public long Createdby { get; set; }

    }

    #endregion

    #region ProductMaster
    public class ProductMasterDTO
    {
        public int productid { get; set; }
        public string productname { get; set; }
        public string productcode { get; set; }

        public decimal numsalerate { get; set; }

        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string Subcategory { get; set; }

        public int producttypeid { get; set; }
        public string producttypename { get; set; }
        public string MeasuredUOMName { get; set; }
        public string PurchaseuomName { get; set; }

        public string OrderNo { get; set; }

        public float Qty { get; set; }
        public decimal Rate { get; set; }
        public int recordid { get; set; }
        public string uomid { get; set; }
        public string uom { get; set; }
        public string uomabbreviation { get; set; }

        public decimal orderqty { get; set; }
        public string storagelocationid { get; set; }
        public string storagelocationname { get; set; }

        public string shelfid { get; set; }
        public string shelfname { get; set; }
        public string shelfcode { get; set; }

        public string vendorname { get; set; }
        public int vendorid { get; set; }
        public string vendorcode { get; set; }

        public string Salesuom { get; set; }
        public string Purchaseuom { get; set; }
        public string Purchaseuomname { get; set; }
        public string Measureduom { get; set; }
        public string SalesuomName { get; set; }

        public int PurchaseUOMfrom { get; set; }
        public int PurchaseUOMTo { get; set; }
        public int SalesuomFrom { get; set; }
        public int SalesuomTo { get; set; }

        public int statusid { get; set; }

        public decimal? Minqty { get; set; }
        public decimal? Maxqty { get; set; }

        public decimal productcost { get; set; }

        public long createdby { get; set; }



        public string productuptodate { get; set; }
    }


    public class CheckGRNDetails
    {
        public string  ProductName { get; set; }
        public long productId { get; set; }
    }
    #endregion

    #region UOM
    public class UOMDTO
    {
        public int UOMid { set; get; }
        public string Name { set; get; }
        public string NameAbbreviation { set; get; }
        public decimal Quantity { set; get; }
        public string UOM { set; get; }
        public string UOMAbbreviation { set; get; }
        public int statusid { get; set; }
        public long createdby { get; set; }
    }
    #endregion

    #region ProductType
    public class ProductTypeDTO
    {
        public int RecordId { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductTypeCode { get; set; }
        public int statusid { get; set; }
        public long createdby { get; set; }
    }
    #endregion

    #region RestaurantLocation
    public class StorageLocationDTO
    {
        public int RecordId { get; set; }
        public string StorageLocationName { get; set; }
        public string StorageLocationCode { get; set; }
        public int statusid { get; set; }
        public long createdby { get; set; }
    }
    #endregion

    #region RestaurantLocation
    public class RestaurantLocationDTO
    {
        public int RecordId { get; set; }
        public string RestaurantName { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Area { get; set; }
        public int Stateid { get; set; }
        public int Cityid { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public int Pincode { get; set; }
        public long createdby { get; set; }
        public int statusid { get; set; }

    }
    #endregion

    #region ShelfMaster
    public class ShelfMasterDTO
    {
        public int Shelfid { get; set; }
        public string ShelfName { get; set; }
        public string ShelfCode { get; set; }
        public string Storageid { get; set; }
        public string StorageLocationName { get; set; }
        public string StorageLocationCode { get; set; }
        public int statusid { get; set; }
        public long createdby { get; set; }
    }
    #endregion

    #region VendorDetails

    public class CountryVendorDTO
    {
        public int countryid { get; set; }
        public string countryname { get; set; }
    }
    public class StateVendorDTO
    {
        public int stateid { get; set; }
        public string statename { get; set; }
    }
    public class CityDTO
    {
        public int cityid { get; set; }
        public string cityname { get; set; }
    }
    public class VendorDetailsDTO
    {
        //<MAIN VENDOR>
        public string vendorname { get; set; }
        public int vendorid { get; set; }
        public string vendorcode { get; set; }
        //<VENDOR ADDRESS> 
        public string line1 { get; set; }
        public string line2 { get; set; }
        public int countryid { get; set; }
        public int stateid { get; set; }
        public int cityid { get; set; }
        public string cityname { get; set; }
        public string statename { get; set; }
        public string countryname { get; set; }
        public string pincode { get; set; }
        public int addressid { get; set; }

        //<VENDOR Contact> 
        public int contactid { get; set; }
        public string contactperson { get; set; }
        public string mobileno { get; set; }
        public string landlineno { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        //<Vendor Bank> 
        public string bankname { get; set; }
        public string bankacno { get; set; }
        public string bankifsc { get; set; }
        public string bankaddress { get; set; }
        public string panno { get; set; }
        public string tinno { get; set; }
        public string taxtype { get; set; }
        public string vatorcst { get; set; }
        public string servicetax { get; set; }
        public string excise { get; set; }
        public string remarks { get; set; }

        public int recordid { get; set; }
        public decimal productcost { get; set; }
        public string productuptodate { get; set; }
        public string productuom { get; set; }

        
    }
    public class ProductlistDTO
    {
        public int productid { get; set; }
        public string productname { get; set; }
    }
    public class VendorProductDTO
    {
        //<MAIN VENDOR>
        public string vendorname { get; set; }
        public int vendorid { get; set; }
        public string vendorcode { get; set; }
        //<Vendor Product>
        public int productid { get; set; }
        public string productname { get; set; }
        public string productuom { get; set; }
        public string productcost { get; set; }
        public string productuptodate { get; set; }
        public string productcode { get; set; }
        public string productcategory { get; set; }
        public string productsubcategory { get; set; }
        public int recordid { get; set; }
    }
    #endregion

    #region Purchaseorder

    public class PurchaseOrderTAXDTO
    {
        public string taxtype { get; set; }
        //--EXCISE
        public string TaxExcisePercentage { get; set; }
        public string TaxExciseAmount { get; set; }
        public string TaxCESSPercentage { get; set; }
        public string TaxCESSAmount { get; set; }
        public string TaxSHCESSPercentage { get; set; }
        public string TaxSHCESSAmount { get; set; }

        //--VAT/CST
        public string vatorcst { get; set; }
        public string taxvatcst { get; set; }
        public string TaxvatorcstAmount { get; set; }

        //--TRANSPORT
        public string TransportCharges { get; set; }

        //--DISCOUNT
        public string DiscountType { get; set; }
        public string DiscountValue { get; set; }
        public string DiscountFlatPercentage { get; set; }

        public string TotalAmount { get; set; }
        public string BasicAmount { get; set; }

        //--total amounts
        public string totalvatorcst { get; set; }
        public string totalexcise { get; set; }
        public string totalcess { get; set; }
        public string totalshcess { get; set; }
        public string totalproductdiscount { get; set; }



        public string PlaceofDelivery { get; set; }
        public string TermsandConditions { get; set; }
    }

    public class PurchaseOrderDTO
    {
        public int recordid { set; get; }

        public string PRNO { get; set; }

        public string PQNO { get; set; }

        public string PlaceofDelivery { get; set; }

        public string PoType { set; get; }

        public string PoThrough { set; get; }

        public string VendorID { get; set; }

        public string VendorName { set; get; }

        public string PurchaseOrderId { set; get; }

        public string PurchseOrderDate { set; get; }

        public string ContactPerson { set; get; }

        public string ProductID { get; set; }

        public string Productcode { get; set; }

        public string ProductName { set; get; }

        public string ProductCategoryID { get; set; }

        public string ProductCategoryName { set; get; }

        public string ProductSubcategoryID { get; set; }

        public string ProductSubCategoryName { set; get; }

        public string Uom { set; get; }

        public string PurchaseUom { set; get; }

        public decimal Quantity { set; get; }

        public decimal EstimateRate { set; get; }

        public string DeliveredBefore { set; get; }

        public string Terms { set; get; }

        public string createdby { set; get; }

        public string RequestedBy { set; get; }

        public string ApprovalBy { set; get; }

        public string Contactno { set; get; }

        public string OrderId { get; set; }

        public string TransportCharges { get; set; }

        public string poid { get; set; }

        //public int CreatedBy { get; set; }

        //--EXCISE
        public string TaxExcisePercentage { get; set; }
        public string TaxExciseAmount { get; set; }
        public string TaxCESSPercentage { get; set; }
        public string TaxCESSAmount { get; set; }
        public string TaxSHCESSPercentage { get; set; }
        public string TaxSHCESSAmount { get; set; }

        // vat or cst
        public string vatorcst { get; set; }
        public string taxvatcst { get; set; }
        public string TaxvatorcstAmount { get; set; }

        public string taxtype { get; set; }
        public string Amount { get; set; }

        //ProductDiscount
        public string ProductDiscountType { get; set; }
        public string ProductDiscountValue { get; set; }
        public string ProductDiscountFlatPercentage { get; set; }


    }

    #endregion


    # region ComboCreation

    public class Productdetails
    {
        public long productid { get; set; }
        public string productname { get; set; }
        public string productcode { get; set; }
        public string uomname { get; set; }
        public string numsalerate { get; set; }
        public string vchuomid { get; set; }
    }

    public class ComboDetails
    {
        public string comboname { get; set; }
        public string combocode { get; set; }
        public string actualcost { get; set; }
        public string combocost { get; set; }
        public string noofproducts { get; set; }
        public string datcombodate { get; set; }
        public string productid { get; set; }
        public string productname { get; set; }
        public string productcode { get; set; }
        public string uom { get; set; }
        public string qafc { get; set; }
        public string numsalerate { get; set; }
        public string rate { get; set; }
        public string amount { get; set; }
    }






    #endregion
}
