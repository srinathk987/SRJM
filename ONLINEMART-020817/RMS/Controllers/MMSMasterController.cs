using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMS.Core;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace RMS.Controllers
{
    public class MMSMasterController : Controller
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        #region User Declarations...

        private IMMSMasters mmsMasters = new MMSMastersRepository();
        private IMMSReports mmsReports = new MMSReportsRepository();
        DataTable dt = null;
        string JsonString = string.Empty;

        #endregion

        #region Product Category...
        public ActionResult ProductCategory()
        {
            return View();
        }
        public ActionResult ShowProductCategory()
        {
            List<ProductCategoryDTO> lstDetails = mmsMasters.ShowProductCategory();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveProductCategory(ProductCategoryDTO Ic)
        {
            Ic.statusid = 1;
            Ic.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = mmsMasters.SaveProductCategory(Ic);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteProductCategory(ProductCategoryDTO Ic)
        {
            var c = mmsMasters.DeleteProductCategory(Ic.CategoryId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateProductCategory(ProductCategoryDTO Ic)
        {
            int c = mmsMasters.UpdateProductCategory(Ic);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public ActionResult CheckProductCategory(ProductCategoryDTO Ic)
        {
            int c = mmsMasters.CheckProductCategory(Ic);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #endregion


        #region Product SubCategory...

        // [RoutePrefix("mmsCategory")]
        public ActionResult SubCategory()
        {
            return View();
        }

        public ActionResult ShowProductSubCategory()
        {
            List<ProductSubCategoryDTO> lstSubcategory = mmsMasters.ShowProductSubCategory();
            return new JsonResult { Data = lstSubcategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveProductSubCategory(ProductSubCategoryDTO Isc)
        {
            Isc.statusid = 1;
            Isc.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = mmsMasters.SaveProductSubCategory(Isc);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateProductSubCategory(ProductSubCategoryDTO Isc)
        {
            var c = mmsMasters.UpdateProductSubCategory(Isc);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteSubCategory(ProductSubCategoryDTO Isc)
        {
            bool c = mmsMasters.DeleteSubCategory(Isc.SubCategoryId, Isc.Subcategory);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        #endregion

        #region Product Master...
        public ActionResult ProductMaster()
        {
            return View();
        }
        public ActionResult ShowProductdetails()
        {
            List<ProductMasterDTO> lstDetails = mmsMasters.ShowProductdetails();
            List<VendorDetailsDTO> lstvendors = mmsMasters.GetvendornamesComboData();
            List<ProductMasterDTO> lstUOM = mmsMasters.ShowUOM1();

            var result = new { lstDetails = lstDetails, lstvendors = lstvendors, lstUOM = lstUOM };

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }
        public ActionResult ShowProductType1()
        {
            List<ProductMasterDTO> lstDetails = mmsMasters.ShowProductType1();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ShowUOM1()
        {
            List<ProductMasterDTO> lstDetails = mmsMasters.ShowUOM1();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ShowStorageLocation1()
        {
            List<ProductMasterDTO> lstDetails = mmsMasters.ShowStorageLocation1();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult ShowShelf1()
        {
            List<ProductMasterDTO> lstDetails = mmsMasters.ShowShelf1();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult getShelfnames(string Storageid)
        {
            List<ProductMasterDTO> lstDetails = mmsMasters.ShowShelf1(Storageid);
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveProductMaster(string PM)
        {

            string json = PM;

            var result = JsonConvert.DeserializeObject<ProductMasterDTO>(json);

            string productcode = string.Empty;
            result.statusid = 1;
            result.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = mmsMasters.SaveProductMaster(result, out productcode);
            //, out productid, 
            var data = new { Data = isSaved, Data1 = productcode};
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult UpdateProductdetails(string PM)
        {
            string json = PM;

            var result = JsonConvert.DeserializeObject<ProductMasterDTO>(json);
            int Cnt = mmsMasters.UpdateProductdetails(result);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteProductMaster(string productcode)
        {
            int Cnt = mmsMasters.DeleteProductMaster(productcode);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult vendorProductsMaster(string productcode, string productname)
        {
            List<ProductMasterDTO> lstOrderDetails = mmsMasters.ShowOrderdetails(productname);
            // List<ProductMasterDTO> lstvendorproducts = mmsMasters.vendorProductsMaster(productcode);
            List<ProductMasterDTO> lstvendorproducts = mmsMasters.vendorProductsMaster(productname);

            var result = new { lstOrderDetails = lstOrderDetails, lstvendorproducts = lstvendorproducts };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public JsonResult UpdatevendorProductsinfo(string VP)
        {
            string json = VP;
            var result = JsonConvert.DeserializeObject<VendorDetailsDTO>(json);
            bool Isvalid = mmsMasters.UpdatevendorProductsinform(result);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult getProductSubCategory(string CategoryId)
        {
            List<ProductSubCategoryDTO> lstSubcategory = mmsMasters.ShowProductSubCategory(CategoryId);
            return new JsonResult { Data = lstSubcategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SavevendorProductsMaster(string VP)
        {
            string json = VP;

            var result = JsonConvert.DeserializeObject<VendorProductDTO>(json);
            bool Isvalid = mmsMasters.SavevendorProductsMaster(result);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion


        #region Vendor Details...
        public ActionResult VendorDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        public JsonResult ShowVendordetails()
        {
            dt = new DataTable();
            dt = mmsMasters.ShowVendordetails();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult vendornamesComboData()
        {
            List<VendorDetailsDTO> lst = mmsMasters.GetvendornamesComboData();
            return new JsonResult { Data = lst, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult getVendorData(string ID)
        {
            List<VendorDetailsDTO> lstVendorDetailsDTO = mmsMasters.getVendorData(ID);
            List<VendorDetailsDTO> lstVendorAddress = mmsMasters.getVendorAddressData(ID);
            List<VendorProductDTO> lstvendorproducts = mmsMasters.vendorProducts(ID);

            var Data = new { Vendors = lstVendorDetailsDTO, Address = lstVendorAddress, Products = lstvendorproducts };
            return new JsonResult { Data = Data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetVendorOrderDetails(string ID)
        {

            List<PurchaseOrderDTO> lstvendorDetails = mmsMasters.vendorDetails(ID);
            var Data = new { vendorDetails = lstvendorDetails };
            //  var Data = "";
            return new JsonResult { Data = Data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ShowVendorCountry()
        {
            List<CountryVendorDTO> lstCountryDTO = mmsMasters.ShowVendorCountry();
            return new JsonResult { Data = lstCountryDTO, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ShowState(string Countryid)
        {
            List<StateVendorDTO> lstStateDTO = mmsMasters.ShowState(Countryid);
            return new JsonResult { Data = lstStateDTO, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult Showcities(string Countryid, string stateid)
        {
            List<CityDTO> lstCityDTO = mmsMasters.Showcities(Countryid, stateid);
            return new JsonResult { Data = lstCityDTO, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SaveVendordetails(VendorDetailsDTO VD)
        {
            string vendorid = string.Empty;
            string vendorcode = string.Empty;
            bool Isvalid = mmsMasters.SaveVendordetails(VD, out vendorid, out vendorcode);
            var data = new { Data = Isvalid, Data1 = vendorid, Data2 = vendorcode };
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult DeleteVendorData(string ID)
        {
            Boolean ISDeleted = false;
            ISDeleted = mmsMasters.DeleteVendorData(ID);
            var Result = new { TorF = ISDeleted };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateVendordetails(VendorDetailsDTO VD)
        {
            bool Isvalid = mmsMasters.UpdateVendordetails(VD);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Vendor Products
        public JsonResult productlist()
        {
            List<ProductlistDTO> lstProductlistDTO = mmsMasters.productlist();
            return new JsonResult { Data = lstProductlistDTO, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult getProductDetails(string Productid)
        {
            VendorProductDTO ObjVendorProductDTO = mmsMasters.ShowProductDetails(Productid);
            return new JsonResult { Data = ObjVendorProductDTO, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SavevendorProducts(VendorProductDTO VP)
        {
            bool Isvalid = mmsMasters.SavevendorProducts(VP);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult UpdatevendorProducts(VendorProductDTO VP)
        {
            bool Isvalid = mmsMasters.UpdatevendorProducts(VP);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Deletevendorproducts(string VP)
        {
            VendorProductDTO v = serializer.Deserialize<VendorProductDTO>(VP);
            bool Isvalid = mmsMasters.Deletevendorproducts(v);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckGRNCount(string ProductCode)
        {
            
            int Cnt = mmsMasters.CheckGRNCount(ProductCode);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

          
        }


        //Vendor Address
        public JsonResult SaveAddressdetails(VendorDetailsDTO VD)
        {
            bool Isvalid = mmsMasters.SaveAddressdetails(VD);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult DeleteVendorAddress(string id)
        {
            Boolean ISDeleted = false;
            ISDeleted = mmsMasters.DeleteVendorAddress(id);
            var Result = new { TorF = ISDeleted };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateVendorAddress(VendorDetailsDTO VD)
        {
            bool Isvalid = mmsMasters.UpdateVendorAddress(VD);
            return new JsonResult { Data = Isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Restaurant Location...
        public ActionResult RestaurantLocation()
        {
            return View();
        }

        public ActionResult ShowRestaurantLocation()
        {
            List<RestaurantLocationDTO> lstRestaurantLocation = mmsMasters.ShowRestaurantLocation();
            List<RestaurantLocationDTO> lstState = mmsMasters.ShowState();
            List<RestaurantLocationDTO> lstCity = mmsMasters.ShowCity();
            var result = new { lstRestaurantLocation = lstRestaurantLocation, lstState = lstState, lstCity = lstCity };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            // return new JsonResult { Data = lstRestaurantLocation, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SaveRestaurantLocation(RestaurantLocationDTO RestaurantLoc)
        {
            RestaurantLoc.statusid = 1;
            RestaurantLoc.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = mmsMasters.SaveRestaurantLocation(RestaurantLoc);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteRestaurantLocation(RestaurantLocationDTO RestaurantLoc)
        {
            var c = mmsMasters.DeleteRestaurantLocation(RestaurantLoc.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateRestaurantLocation(RestaurantLocationDTO ResLoc)
        {
            int Cnt = mmsMasters.UpdateRestaurantLocation(ResLoc);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region UOM...
        public ActionResult UOM()
        {
            return View();
        }

        public ActionResult ShowUOM()
        {
            List<UOMDTO> lstDetails = mmsMasters.ShowUOM();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveUOM(UOMDTO Ic)
        {
            Ic.statusid = 1;
            Ic.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = mmsMasters.SaveUOM(Ic);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteUOM(UOMDTO Ic)
        {
            var c = mmsMasters.DeleteUOM(Ic.UOMid);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateUOM(UOMDTO Ic)
        {
            int c = mmsMasters.UpdateUOM(Ic);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #endregion

        #region Tax...
        public ActionResult Tax()
        {
            return View();
        }

        public ActionResult ShowTaxDetails()
        {
            List<ProductTaxDTO> lstTax = mmsMasters.ShowTaxDetails();
            return new JsonResult { Data = lstTax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveTax(ProductTaxDTO Tx)
        {
            Tx.statusid = 1;
            Tx.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = mmsMasters.SaveTax(Tx);

            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteTax(int id, string name)
        {
            var c = mmsMasters.DeleteTax(id, name);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateTax(ProductTaxDTO Tx)
        {
            int Count = mmsMasters.UpdateTax(Tx);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public ActionResult CountUpdateTax(string name)
        {
            int Count = mmsMasters.CountUpdateTax(name);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #endregion

        #region Country...
        public ActionResult Country()
        {
            return View();
        }
        public bool SaveCountry(CountryDTO objCountry)
        {
            return mmsMasters.SaveCountry(objCountry);

        }
        [HttpGet]
        public JsonResult ShowCountryDetails()
        {
            List<CountryDTO> lstEADetails = new List<CountryDTO>();
            lstEADetails = mmsMasters.ShowCountryDetails();
            return new JsonResult { Data = lstEADetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public bool UpdateCountry(CountryDTO objCountry)
        {
            return mmsMasters.UpdateCountry(objCountry);
        }
        public bool DeleteCountry(CountryDTO objCountry)
        {
            return mmsMasters.DeleteCountry(objCountry.CountryId);
        }

        public ActionResult CheckCountry(CountryDTO objCountry)
        {

            int c = mmsMasters.CheckCountry(objCountry);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        #endregion

        #region State...
        public ActionResult State()
        {
            return View();
        }
        public bool SaveState(StateDTO objState)
        {
            return mmsMasters.SaveState(objState);

        }
        public bool UpdateState(StateDTO objState)
        {
            return mmsMasters.UpdateState(objState);

        }
        public bool DeleteState(StateDTO objState)
        {
            return mmsMasters.DeleteState(objState.StateId, objState.CountryId);

        }

        public JsonResult BindStateDetails()
        {
            MMSMastersRepository objmmsmaster = new MMSMastersRepository();

            List<StateDTO> lstState = new List<StateDTO>();
            lstState = objmmsmaster.BindState();
            return this.Json(lstState, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckState(StateDTO objState)
        {

            int c = mmsMasters.CheckState(objState);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #endregion

        #region City...

        [HttpGet]
        public ActionResult City()
        {
            return View();
        }

        public JsonResult CreateCity(City m)
        {
            List<City> lstCity = new List<City>();
            List<City> lstCitydetails = new List<City>();
            bool isValid = false;
            try
            {
                if (ModelState.IsValid)
                {
                    isValid = mmsMasters.CreateCity(m);
                    if (isValid)
                    {
                        lstCitydetails = mmsMasters.BindCityDetails();
                    }
                }
                var result = new { Success = isValid, data = lstCitydetails };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "CityDetails");
                throw ex;
            }
        }

        public JsonResult ShowCountry()
        {
            List<City> lstcountry = new List<City>();
            if (ModelState.IsValid)
            {
                lstcountry = mmsMasters.ShowCountry();
            }
            return new JsonResult { Data = lstcountry, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult ShowStates(string strcountryid)
        {
            try
            {
                List<City> lststate = new List<City>();
                if (ModelState.IsValid)
                {
                    lststate = mmsMasters.ShowStates(strcountryid);
                }

                var data = new JsonResult { Data = lststate, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "States");
                throw;
            }
        }

        public JsonResult BindCityDetails()
        {
            try
            {
                List<City> lstcity = mmsMasters.BindCityDetails();
                return new JsonResult { Data = lstcity, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }

        public JsonResult UpdateCity(City objcity)
        {
            bool isvalid = false;
            try
            {
                isvalid = mmsMasters.UpdateCity(objcity);
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            return new JsonResult { Data = isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult DeleteCity(City cdto)
        {
            bool isvalid = false;
            try
            {
                isvalid = mmsMasters.DeleteCity(cdto);
            }
            catch (Exception ex)
            {
                //EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            return new JsonResult { Data = isvalid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckCity(City objcity)
        {
            try
            {

                int c = mmsMasters.CheckCity(objcity);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            catch (Exception ex)
            {
                // EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }


        #endregion


        #region Shelf...
        public ActionResult Shelf()
        {
            return View();
        }

        public ActionResult ShowShelfMaster()
        {
            List<ShelfMasterDTO> lstDetails = mmsMasters.ShowShelfMaster();
            List<StorageLocationDTO> lstStorageLocation = mmsMasters.ShowStorageLocation();
            var result = new { lstDetails = lstDetails, lstStorageLocation = lstStorageLocation };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveShelfMaster(ShelfMasterDTO Ic)
        {
            Ic.statusid = 1;
            Ic.createdby = Convert.ToInt64(Session["UserId"]);
            int isSaved = mmsMasters.SaveShelfMaster(Ic);
            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteShelfMaster(ShelfMasterDTO Ic)
        {
            var c = mmsMasters.DeleteShelfMaster(Ic.Shelfid);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateShelfMaster(ShelfMasterDTO Ic)
        {
            int c = mmsMasters.UpdateShelfMaster(Ic);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        #endregion

        #region POTandC...
        public ActionResult POTandC()
        {
            return View();
        }

        #endregion

        #region PO CostMaster...
        public ActionResult POCostMaster()
        {
            return View();
        }

        #endregion

        #region PRODUCTTYPE...
        public ActionResult ProductType()
        {
            return View();
        }
        public ActionResult ShowProductType()
        {
            List<ProductTypeDTO> lstProductType = mmsMasters.ShowProductType();
            return new JsonResult { Data = lstProductType, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveProductType(ProductTypeDTO PT)
        {
            PT.statusid = 1;
            PT.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = mmsMasters.SaveProductType(PT);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteProductType(string PT)
        {
            int ID = Convert.ToInt16(PT);
            var c = mmsMasters.DeleteProductType(ID);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateProductType(ProductTypeDTO PT)
        {
            int Cnt = mmsMasters.UpdateProductType(PT);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckProducttype(ProductTypeDTO PT)
        {
            int c = mmsMasters.CheckProducttype(PT);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion

        #region StorageLocation...
        public ActionResult StorageLocation()
        {
            return View();
        }
        public ActionResult ShowStorageLocation()
        {
            List<StorageLocationDTO> lstStorageLocation = mmsMasters.ShowStorageLocation();
            return new JsonResult { Data = lstStorageLocation, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult SaveStorageLocation(StorageLocationDTO SL)
        {
            SL.statusid = 1;
            SL.createdby = Convert.ToInt64(Session["UserId"]);
            int Count = mmsMasters.SaveStorageLocation(SL);
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DeleteStorageLocation(string SL)
        {
            Int16 id = Convert.ToInt16(SL);
            var c = mmsMasters.DeleteStorageLocation(id);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult UpdateStorageLocation(StorageLocationDTO SL)
        {
            int Cnt = mmsMasters.UpdateStorageLocation(SL);
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckStorageLocation(StorageLocationDTO SL)
        {
            int c = mmsMasters.CheckStorageLocation(SL);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion

        //
        // GET: /MMS/
        public ActionResult Index()
        {
            return View();
        }


        #region Purchase Order

        public ActionResult CreateProductOrder()
        {

            return View();

        }
        public JsonResult GetVendorNamesData()
        {

            dt = new DataTable();
            dt = mmsMasters.GetVendorNames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BindUOM()
        {

            dt = new DataTable();
            dt = mmsMasters.BindUOMNames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlaceofDeliveryData()
        {

            dt = new DataTable();
            dt = mmsMasters.GetPlaceofDeliveryData();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContactPerson(int VID)
        {
            dt = new DataTable();
            dt = mmsMasters.GetVendorContactPerson(VID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            DataTable dt1 = new DataTable();
            dt1 = mmsMasters.GetVendorProducts(VID);
            string JsonString1 = string.Empty;
            JsonString1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt1);
            var Totalresult = new { Data = JsonString, Data1 = JsonString1 };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductUomBy(int Prdid, int vendorid)
        {
            dt = new DataTable();
            dt = mmsMasters.GetProductuombyid(Prdid, vendorid);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            DataTable dt1 = new DataTable();
            dt1 = mmsMasters.Getvendorsbyproduct(Prdid);
            string strJsonString = string.Empty;
            strJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt1);
            //string strJsonString1 = string.Empty;
            //DataTable dt2 = new DataTable();
            //dt2 = mmsMasters.getProductStoragelocations(Prdid.ToString());
            //strJsonString1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt2);
            //var data = new { productdetails = JsonString, vendor = strJsonString, ProductStorages = strJsonString1 };
            var data = new { productdetails = JsonString, vendor = strJsonString };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProductStoragelocations(string ID)
        {
            dt = new DataTable();
            dt = mmsMasters.getProductStoragelocations(ID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getvendorsbyproduct(int ID)
        {
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePurchaseOrder(string PurchaseOrderDTO, string listPurchaseOrderDTO, string TAX)
        {
            List<PurchaseOrderDTO> ListItems = serializer.Deserialize<List<PurchaseOrderDTO>>(listPurchaseOrderDTO);
            PurchaseOrderDTO XYZ = serializer.Deserialize<PurchaseOrderDTO>(PurchaseOrderDTO);
            PurchaseOrderTAXDTO wewe = serializer.Deserialize<PurchaseOrderTAXDTO>(TAX);
            XYZ.createdby = Session["UserId"].ToString();
            string pono = string.Empty;
            bool isSaved = mmsMasters.SavePurchaseOrder(XYZ, ListItems, wewe, out pono);
            DataSet ds = new DataSet();

            ds = mmsReports.GetPurchaseOrderVendorsDetailsByID(pono);
            string strJsonString = string.Empty;
            strJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);
            var data = new { isSaved = isSaved, podetails = strJsonString };
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion


        #region ComboPack Creation

        public ActionResult CreateCombo()
        {
            return View();
        }

        public JsonResult GetProductnames()
        {
            DataTable dtProducts;
            try
            {
                dtProducts = mmsMasters.GetProductnames();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dtProducts);
                var Totalresult = new { Data = JsonString };
                return Json(Totalresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetComboProductDetails(string ProductId)
        {
            dt = new DataTable();
            dt = mmsMasters.GetComboProductDetails(ProductId);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var data = new { productdetails = JsonString };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveComboDetails(string NoOfProducts, string objLComboDetails, string ActualAmount, string ComboCost, string ComboName, string ComboDate)
        {
            List<ComboDetails> objSaveDetails = serializer.Deserialize<List<ComboDetails>>(objLComboDetails);
            string ret = mmsMasters.SaveComboDetails(objSaveDetails, NoOfProducts, ActualAmount, ComboCost, ComboName, ComboDate);
            return new JsonResult { Data = ret, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult GetComboDetails()
        {
            List<ComboDetails> objLCombos = new List<ComboDetails>();
            objLCombos = mmsMasters.GetComboDetails();
            return new JsonResult { Data = objLCombos, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetComboDetailsforUpdate(string ComboId)
        {
            List<ComboDetails> objLCombos = new List<ComboDetails>();
            objLCombos = mmsMasters.GetComboDetailsforUpdate(ComboId);
            return new JsonResult { Data = objLCombos, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UpdateComboDetails(string NoOfProducts, string objLComboDetails, string ActualAmount, string ComboCost, string ComboName, string ComboDate, string ComboCode)
        {
            long Modifiedby = Convert.ToInt64(Session["UserId"]);
            List<ComboDetails> objSaveDetails = serializer.Deserialize<List<ComboDetails>>(objLComboDetails);
            string ret = mmsMasters.UpdateComboDetails(objSaveDetails, NoOfProducts, ActualAmount, ComboCost, ComboName, ComboDate, Modifiedby, ComboCode);
            return new JsonResult { Data = ret, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult DeleteComboPackDetails(string ComboCode)
        {
            var c = mmsMasters.DeleteComboPackDetails(ComboCode);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        






        #endregion

    }
}