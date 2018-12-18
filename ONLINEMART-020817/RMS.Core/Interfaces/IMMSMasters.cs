using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core.Interfaces
{
    public interface IMMSMasters
    {
        #region ProductCategory...

        List<ProductCategoryDTO> ShowProductCategory();
        int SaveProductCategory(ProductCategoryDTO ItemCategory);
        int UpdateProductCategory(ProductCategoryDTO ItemCategory);
        bool DeleteProductCategory(int CategoryId);

        int CheckProductCategory(ProductCategoryDTO Ic);
        #endregion

        #region Product Subcategory....

        List<ProductSubCategoryDTO> ShowProductSubCategory();
        int SaveProductSubCategory(ProductSubCategoryDTO SubCategory);
        int UpdateProductSubCategory(ProductSubCategoryDTO SubCategory);
        bool DeleteSubCategory(int SubCategoryId, string Subcategory);

        #endregion

        #region Tax...
        List<ProductTaxDTO> ShowTaxDetails();
        int SaveTax(ProductTaxDTO Tx);
        int UpdateTax(ProductTaxDTO Tx);
        int CountUpdateTax(string name);
        bool DeleteTax(int id, string name);
        #endregion

        #region Country..

        bool SaveCountry(CountryDTO objCountry);
        List<CountryDTO> ShowCountryDetails();
        bool UpdateCountry(CountryDTO objCountry);
        bool DeleteCountry(int countryid);
        int CheckCountry(CountryDTO objCountry);

        #endregion

        #region State...

        bool SaveState(StateDTO objState);
        bool UpdateState(StateDTO objState);
        bool DeleteState(int stateid, string countryid);
        int CheckState(StateDTO objState);

        #endregion


        #region City...

        bool CreateCity(City strCity);
        List<City> ShowCountry();
        List<City> ShowStates(string strcountryid);
        List<City> BindCityDetails();
        bool UpdateCity(City objcity);
        bool DeleteCity(City citydto);
        int CheckCity(City objcity);


        #endregion

        #region ProductType...
        List<ProductTypeDTO> ShowProductType();
        int SaveProductType(ProductTypeDTO Dept);
        int UpdateProductType(ProductTypeDTO Dept);
        bool DeleteProductType(int RecordId);
        #endregion

        #region Product Master...

        List<ProductMasterDTO> ShowProductdetails();
        List<ProductMasterDTO> ShowProductType1();
        List<ProductMasterDTO> ShowUOM1();
        List<ProductMasterDTO> ShowStorageLocation1();
        List<ProductMasterDTO> ShowShelf1(string Storageid);
        List<ProductMasterDTO> ShowShelf1();
        int SaveProductMaster(ProductMasterDTO PM, out string productcode);
        //, out string productid, 

        int UpdateProductdetails(ProductMasterDTO PM);

        int DeleteProductMaster(string productcode);
        List<ProductMasterDTO> ShowOrderdetails(string productcode);
        // bool UpdatevendorProductsinformation(VendorDetailsDTO VP);
        #endregion

        #region StorageLocation...
        List<StorageLocationDTO> ShowStorageLocation();
        int SaveStorageLocation(StorageLocationDTO Dept);
        int UpdateStorageLocation(StorageLocationDTO Dept);
        bool DeleteStorageLocation(int RecordId);
        int CheckStorageLocation(StorageLocationDTO SL);
        #endregion


        #region VendorDetails
        DataTable ShowVendordetails();
        List<VendorDetailsDTO> GetvendornamesComboData();
        List<CountryVendorDTO> ShowVendorCountry();
        List<StateVendorDTO> ShowState(string Countryid);
        List<CityDTO> Showcities(string Countryid, string stateid);
        bool SaveVendordetails(VendorDetailsDTO VD, out string vendorid, out string vendorcode);
        List<VendorDetailsDTO> getVendorData(string ID);
        bool DeleteVendorData(string ID);
        bool UpdateVendordetails(VendorDetailsDTO VD);

        List<VendorDetailsDTO> getVendorAddressData(string ID);
        bool SaveAddressdetails(VendorDetailsDTO VD);
        bool UpdateVendorAddress(VendorDetailsDTO VD);
        bool DeleteVendorAddress(string ID);

        List<ProductlistDTO> productlist();
        VendorProductDTO ShowProductDetails(string Productid);
        List<VendorProductDTO> vendorProducts(string ID);
        bool SavevendorProducts(VendorProductDTO VP);
        bool UpdatevendorProducts(VendorProductDTO VP);
        bool Deletevendorproducts(VendorProductDTO VP);
        #endregion

        #region StorageLocation...

        List<RestaurantLocationDTO> ShowRestaurantLocation();
        bool DeleteRestaurantLocation(int Recordid);
        int UpdateRestaurantLocation(RestaurantLocationDTO RestaurantLoc);
        int SaveRestaurantLocation(RestaurantLocationDTO RestaurantLoc);
        List<RestaurantLocationDTO> ShowState();
        List<RestaurantLocationDTO> ShowCity();

        #endregion



        #region

        int SaveUOM(UOMDTO Ic);

        bool DeleteUOM(int p);

        List<UOMDTO> ShowUOM();

        int UpdateUOM(UOMDTO Ic);

        #endregion

        #region Shelf

        List<ShelfMasterDTO> ShowShelfMaster();

        int SaveShelfMaster(ShelfMasterDTO Ic);

        int UpdateShelfMaster(ShelfMasterDTO Ic);

        bool DeleteShelfMaster(int p);
        #endregion




        #region purchaseorder

        DataTable GetVendorNames();
        DataTable GetVendorContactPerson(int Name);
        DataTable GetPlaceofDeliveryData();
        DataTable getProductStoragelocations(string Prdid);
        DataTable GetProductuombyid(int prdid, int vendorid);
        DataTable Getvendorsbyproduct(int Prdid);
        DataTable BindUOMNames();
        DataTable GetVendorProducts(int VID);
        bool SavePurchaseOrder(PurchaseOrderDTO PurchaseOrderDTO, List<PurchaseOrderDTO> listPurchaseOrderDTO, PurchaseOrderTAXDTO TAX, out string pono);

        #endregion







        List<ProductMasterDTO> vendorProductsMaster(string ID);

        //bool UpdatevendorProductsinform(VendorDetailsDTO VP);

        //bool UpdatevendorProductsinform(VendorDetailsDTO VP);

        bool UpdatevendorProductsinform(VendorDetailsDTO VP);



        List<PurchaseOrderDTO> vendorDetails(string ID);



        List<ProductSubCategoryDTO> ShowProductSubCategory(string CategoryId);

        bool SavevendorProductsMaster(VendorProductDTO VP);



        int CheckProducttype(ProductTypeDTO PT);

        //bool CheckGRNCount(CheckGRNDetails v);
        int CheckGRNCount(string Product);


        #region Combo Pack Creation


        DataTable GetProductnames();

        DataTable GetComboProductDetails(string ProductId);

        string SaveComboDetails(List<ComboDetails> objSaveDetails, string NoOfProducts, string ActualAmount, string ComboCost, string ComboName, string ComboDate);

        List<ComboDetails> GetComboDetails();

        List<ComboDetails> GetComboDetailsforUpdate(string ComboId);

        string UpdateComboDetails(List<ComboDetails> objSaveDetails, string NoOfProducts, string ActualAmount, string ComboCost, string ComboName, string ComboDate, long Modifiedby, string ComboCode);

        bool DeleteComboPackDetails(string Code);


        #endregion
    }
}
