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
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace RMS.Controllers
{
    public class MMSTransactionController : Controller
    {
        #region Global Declarations
        DataTable dt = null;
        string JsonString = string.Empty;
        private IMMSTransaction mmsTransactions = new MMSTransactionRepository();
        private IMMSMasters objMMSMasters = new MMSMastersRepository();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        #endregion

        public ActionResult Index()
        {
            return View();
        }
        #region Goods Receipt Note
        public JsonResult GetDatepickerEnableStatus()
        {
            string strStatus = mmsTransactions.GetDatepickerEnableStatus();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(strStatus);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }




        public JsonResult getgrntaxdetails(string GRNid, string Vendorid, string GrnType)
        {
            if (GRNid != null)
            {
                GoodsReceivedNoteTAXDTO TX = mmsTransactions.getGRNtaxdetails(GRNid, Vendorid, GrnType);
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(TX);
            }
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getGRNdetails(string GRNid, string Vendorid)
        {
            if (GRNid != null)
            {
                dt = new DataTable();
                dt = mmsTransactions.getGRNdetails(GRNid, Vendorid);
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GRN()
        {
            return View();
        }
        public JsonResult getponumbers(string ID)
        {
            dt = new DataTable();
            dt = mmsTransactions.getponumbers(ID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getgrnnonumbers(string ID, string TypeGrn)
        {
            dt = new DataTable();
            dt = mmsTransactions.getgrnumbers(ID, TypeGrn);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVendorProducts(string ID)
        {
            dt = new DataTable();
            dt = mmsTransactions.GetVendorProducts(ID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getgrngriddetails(string VID, string POID)
        {
            dt = new DataTable();
            dt = mmsTransactions.getgrngriddetails(VID, POID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            string strProuductIds = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strProuductIds += dt.Rows[i]["productid"].ToString() + ',';
            }
            if (strProuductIds.Length > 0)
            {
                strProuductIds = strProuductIds.Substring(0, strProuductIds.Length - 1);
            }
            DataTable dt1 = new DataTable();
            string strJsonString1 = string.Empty;
            IMMSMasters mmsMasters = new MMSMastersRepository();
            dt1 = mmsMasters.getProductStoragelocations(strProuductIds.ToString());
            strJsonString1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt1);
            var Totalresult = new { Data = JsonString, Data1 = strJsonString1 };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getstoragelocations()
        {
            dt = new DataTable();
            dt = mmsTransactions.Getstoragelocations();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            DataTable dt1 = new DataTable();
            dt1 = mmsTransactions.getuomlist();
            string JsonString1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt1);
            var Totalresult = new { Storage = JsonString, Uom = JsonString1 };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getshelfs(string ID)
        {
            dt = new DataTable();
            dt = mmsTransactions.getshelfs(ID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveGRN(string GoodsReceivedNoteDTO, string lstGoodsReceivedNoteDTO, string TAX)
        {
            GoodsReceivedNoteDTO ThirdT = serializer.Deserialize<GoodsReceivedNoteDTO>(GoodsReceivedNoteDTO);
            List<GoodsReceivedNoteDTO> ThirdTs = serializer.Deserialize<List<GoodsReceivedNoteDTO>>(lstGoodsReceivedNoteDTO);
            GoodsReceivedNoteTAXDTO tax = serializer.Deserialize<GoodsReceivedNoteTAXDTO>(TAX);
            Boolean IsSaved = false;
            ThirdT.CreatedBy = Convert.ToInt32(Session["UserId"]);
            IsSaved = mmsTransactions.SaveGRN(ThirdT, ThirdTs, tax);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult BinduomConversionValues()
        {
            dt = new DataTable();
            dt = mmsTransactions.BinduomConversionValues();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getRequestPersons()
        {
            dt = new DataTable();
            dt = mmsTransactions.getRequestPersons();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Physical Stock Update
        public ActionResult PhysicalStockUpdate()
        {

            return View();
        }
        public ActionResult ShowStockusers()
        {
            List<PhysicalStockUpdateDTO> lstDetails = mmsTransactions.ShowStockusers();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ShowStockupdate(string locationid)
        {
            List<PhysicalStockUpdateDTO> lstStockupdate = mmsTransactions.ShowStockupdate(locationid);
            return new JsonResult { Data = lstStockupdate, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SaveStock(string PhysicalStockUpdateDTO, string lstStockupdate)
        {
            Boolean IsSaved = false;
            PhysicalStockUpdateDTO DtoPhy = serializer.Deserialize<PhysicalStockUpdateDTO>(PhysicalStockUpdateDTO);
            List<PhysicalStockUpdateDTO> lstphy = JsonConvert.DeserializeObject<List<PhysicalStockUpdateDTO>>(lstStockupdate);
            IsSaved = mmsTransactions.SaveStock(DtoPhy, lstphy);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowStoragelocation()
        {
            List<PhysicalStockUpdateDTO> lstDetails = mmsTransactions.ShowStoragelocation();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion



        /// <summary>
        /// Vikram Chathilla
        /// </summary>
        /// <returns></returns>
        /// 

        #region Product Indent Issue
        public ActionResult IndentRelease()
        {
            return View();
        }

        public ActionResult ShowProducts()
        {
            List<IndentDTO> lstProducts = mmsTransactions.ShowProducts();
            List<IndentDTO> lstindents = mmsTransactions.ShowIndentNumbers();
            List<IndentDTO> lststorages = mmsTransactions.StorageLocationBind();
            List<ProductMasterDTO> lstissueuom = objMMSMasters.ShowUOM1();
            List<ProductMasterDTO> lstshelf = objMMSMasters.ShowShelf1();
            List<MaterialIndentDTO> lstDepartment = mmsTransactions.ShowDepartment();
            List<MaterialIndentDTO> lstvwavailble = mmsTransactions.GetAvailablestock();
            List<IndentDTO> lstconversionvalues = mmsTransactions.GetConversionvalues();
            var data = new { Products = lstProducts, indents = lstindents, storages = lststorages, issueuom = lstissueuom, shelf = lstshelf, Departments = lstDepartment, vwavailbleqty = lstvwavailble, conversiondata = lstconversionvalues };
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowIndentGrid(string indentdto)
        {
            List<IndentDTO> lstProducts = mmsTransactions.ShowIndentGridDetails(indentdto);
            // List<IndentDTO> lstconversionvalues = mmsTransactions.GetConversionvalues();  , conversiondata = lstconversionvalues
            var data = new { data = lstProducts };
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowDirectIndentGridDetails(IndentDTO lstobjproductname)
        {
            List<IndentDTO> lstDirectIndentGrid = mmsTransactions.ShowDirectIndentGridDetails(lstobjproductname);
            return new JsonResult { Data = lstDirectIndentGrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult DirectShelfDetails(IndentDTO objavailablequantity)
        {
            string lstDirectIndentGrid = mmsTransactions.DirectShelfDetails(objavailablequantity);
            return new JsonResult { Data = lstDirectIndentGrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult StorageShelfDetails(string StorageID)
        {
            dt = new DataTable();
            dt = mmsTransactions.StorageShelfDetails(StorageID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindShelfAvailabilityDetails(IndentDTO objavailablequantity)
        {
            string lstDirectIndentGrid = mmsTransactions.BindShelfAvailabilityDetails(objavailablequantity);
            return new JsonResult { Data = lstDirectIndentGrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveIndentIssueDetails(IndentDTO ID, List<IndentDTO> ID2, string conversionamount)
        {
            int userid = Convert.ToInt32(Session["UserId"]);
            ID.userid = userid.ToString();
            ID.numissueconvertionqty = conversionamount;
            int Indentcount = mmsTransactions.SaveIndentIssueDetails(ID, ID2);
            return new JsonResult { Data = Indentcount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult IndentIssueDetailsBind(IndentDTO objindentissue)
        {
            List<IndentDTO> lstindentissue = mmsTransactions.IssueDetailsBind(objindentissue);
            List<IndentDTO> lstproductstores = mmsTransactions.IssueProductStores(objindentissue);
            List<IndentDTO> lstproductshelfstores = mmsTransactions.IssueProductShelfsStores(objindentissue);
            var data = new { IndentDetails = lstindentissue, storagedetails = lstproductstores, productshelfstores = lstproductshelfstores };
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public ActionResult SaveDirectIndentIssueDetails(string vchdate, string vchrequestedby, string fromstorage, string tostorage, string ApprovalBy, string IssuedBy, string IND2, string deptname)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            List<IndentDTO> ID2 = serializer.Deserialize<List<IndentDTO>>(IND2);
            IndentDTO ID = new IndentDTO();
            ID.ApprovalBy = ApprovalBy;
            ID.IssuedBy = IssuedBy;
            ID.vchrequestedby = vchrequestedby;
            int userid = Convert.ToInt32(Session["UserId"]);
            ID.userid = userid.ToString();
            ID.DeparmentName = deptname;
            ID.fromstorage = fromstorage;
            ID.tostorage = tostorage;
            ID.vchdate = vchdate;
            int Indentcount = mmsTransactions.SaveDirectIndentIssueDetails(ID, ID2);
            return new JsonResult { Data = Indentcount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion




        //#region Product Indent Issue
        //public ActionResult IndentRelease()
        //{
        //    return View();
        //}

        //public ActionResult ShowProducts()
        //{
        //    List<IndentDTO> lstProducts = mmsTransactions.ShowProducts();
        //    List<IndentDTO> lstindents = mmsTransactions.ShowIndentNumbers();
        //    List<IndentDTO> lststorages = mmsTransactions.StorageLocationBind();
        //    List<ProductMasterDTO> lstissueuom = objMMSMasters.ShowUOM1();
        //    List<ProductMasterDTO> lstshelf = objMMSMasters.ShowShelf1();
        //    List<MaterialIndentDTO> lstDepartment = mmsTransactions.ShowDepartment();
        //    List<MaterialIndentDTO> lstvwavailble = mmsTransactions.GetAvailablestock();
        //    List<IndentDTO> lstconversionvalues = mmsTransactions.GetConversionvalues();
        //    var data = new { Products = lstProducts, indents = lstindents, storages = lststorages, issueuom = lstissueuom, shelf = lstshelf, Departments = lstDepartment, vwavailbleqty = lstvwavailble, conversiondata = lstconversionvalues };
        //    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        //public ActionResult ShowIndentGrid(string indentdto)
        //{
        //    List<IndentDTO> lstProducts = mmsTransactions.ShowIndentGridDetails(indentdto);
        //    // List<IndentDTO> lstconversionvalues = mmsTransactions.GetConversionvalues();  , conversiondata = lstconversionvalues
        //    var data = new { data = lstProducts };
        //    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        //public ActionResult ShowDirectIndentGridDetails(IndentDTO lstobjproductname)
        //{
        //    List<IndentDTO> lstDirectIndentGrid = mmsTransactions.ShowDirectIndentGridDetails(lstobjproductname);
        //    return new JsonResult { Data = lstDirectIndentGrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}


        //public ActionResult DirectShelfDetails(IndentDTO objavailablequantity)
        //{
        //    string lstDirectIndentGrid = mmsTransactions.DirectShelfDetails(objavailablequantity);
        //    return new JsonResult { Data = lstDirectIndentGrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        //}
        //public JsonResult StorageShelfDetails(string StorageID)
        //{
        //    dt = new DataTable();
        //    dt = mmsTransactions.StorageShelfDetails(StorageID);
        //    JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        //    var Totalresult = new { Data = JsonString };
        //    return Json(Totalresult, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult BindShelfAvailabilityDetails(IndentDTO objavailablequantity)
        //{
        //    string lstDirectIndentGrid = mmsTransactions.BindShelfAvailabilityDetails(objavailablequantity);
        //    return new JsonResult { Data = lstDirectIndentGrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        //public JsonResult SaveIndentIssueDetails(IndentDTO ID, List<IndentDTO> ID2, string conversionamount)
        //{
        //    int userid = Convert.ToInt32(Session["UserId"]);
        //    ID.userid = userid.ToString();
        //    ID.numissueconvertionqty = conversionamount;
        //    int Indentcount = mmsTransactions.SaveIndentIssueDetails(ID, ID2);
        //    return new JsonResult { Data = Indentcount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        //}

        //public JsonResult IndentIssueDetailsBind(IndentDTO objindentissue)
        //{
        //    List<IndentDTO> lstindentissue = mmsTransactions.IssueDetailsBind(objindentissue);
        //    List<IndentDTO> lstproductstores = mmsTransactions.IssueProductStores(objindentissue);
        //    List<IndentDTO> lstproductshelfstores = mmsTransactions.IssueProductShelfsStores(objindentissue);
        //    var data = new { IndentDetails = lstindentissue, storagedetails = lstproductstores, productshelfstores = lstproductshelfstores };
        //    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}



        //public ActionResult SaveDirectIndentIssueDetails(string vchrequestedby, string ApprovalBy, string IssuedBy, string IND2, string deptname)
        //{

        //    JavaScriptSerializer serializer = new JavaScriptSerializer();

        //    List<IndentDTO> ID2 = serializer.Deserialize<List<IndentDTO>>(IND2);
        //    IndentDTO ID = new IndentDTO();
        //    ID.ApprovalBy = ApprovalBy;
        //    ID.IssuedBy = IssuedBy;
        //    ID.vchrequestedby = vchrequestedby;
        //    int userid = Convert.ToInt32(Session["UserId"]);
        //    ID.userid = userid.ToString();
        //    ID.DeparmentName = deptname;
        //    int Indentcount = mmsTransactions.SaveDirectIndentIssueDetails(ID, ID2);
        //    return new JsonResult { Data = Indentcount, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        //#endregion

        #region Purchase Order
        public JsonResult getexistponumbers(string strType, string Vendorid)
        {
            if (strType != null)
            {
                dt = new DataTable();
                dt = mmsTransactions.getexistponumbers(strType, Vendorid);
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getpodetails(string poid, string Vendorid)
        {
            if (poid != null)
            {
                dt = new DataTable();
                dt = mmsTransactions.getpodetails(poid, Vendorid);
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getpotaxdetails(string poid, string Vendorid)
        {
            if (poid != null)
            {
                PurchaseOrderTAXDTO TX = mmsTransactions.getpotaxdetails(poid, Vendorid);
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(TX);
            }
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Indent Details By Vikram

        public ActionResult Indent()
        {
            return View();
        }
        public ActionResult ShowIndentProducts()
        {
            List<MaterialIndentDTO> lstproduct = mmsTransactions.ShowIndentProducts();

            // List<MaterialIndentDTO> lststorage = mmsTransactions.ShowStorage();
            List<MaterialIndentDTO> lstvwavailble = mmsTransactions.GetAvailablestock();
            List<MaterialIndentDTO> lstrequesteby = mmsTransactions.showrequestedby();
            List<MaterialIndentDTO> lstdeportments = mmsTransactions.ShowDepartment();

            //  List<MaterialIndentDTO> lstshowuoms = objPOSTransaction.ShowUOM1();
            var bindingdata = new { products = lstproduct, Requestedby = lstrequesteby, vwavailableqty = lstvwavailble, deportments = lstdeportments };
            return new JsonResult { Data = bindingdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult BindStorageAreas(string storageareavalue)
        {
            List<MaterialIndentDTO> lststorage = mmsTransactions.ShowStorage(storageareavalue);
            var bindingdata = new { storages = lststorage };
            return new JsonResult { Data = bindingdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetSelfNames(String showroomselected, string productid, string productname, string storagelocationname, string uomname)
        {

            List<MaterialIndentDTO> lstselfdetails = new List<MaterialIndentDTO>();
            if (showroomselected != null)
            {
                if (showroomselected.Contains(':'))
                {
                    showroomselected = showroomselected.Split(':')[1].ToString();
                }
            }

            if (showroomselected != string.Empty)
            {
                lstselfdetails = mmsTransactions.GetSelfdetails(showroomselected);
            }

            var selfdata = new { selfnames = lstselfdetails };
            return new JsonResult { Data = selfdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //public ActionResult GetAvailaibilityQty(string productid, string productname, string uomname, string storagearea, string shelfname)
        //{
        //    List<MaterialIndentDTO> lstproductavailble = new List<MaterialIndentDTO>();
        //    decimal availableqty = 0;
        //    if (productid != null && productname != null && storagearea != null && shelfname != null)
        //    {
        //        availableqty = mmsTransactions.GetProductAvailabilityQty(productid, productname, uomname, storagearea, shelfname);
        //    }
        //    else
        //    {
        //        shelfname = "";
        //        availableqty = mmsTransactions.GetProductAvailabilityQty(productid, productname, uomname, storagearea, shelfname);
        //    }
        //    var AvailaibilityQty = new { AvailbleQty = availableqty };
        //    return new JsonResult { Data = AvailaibilityQty, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        //}


        public JsonResult SaveIndentDetails(List<MaterialIndentDTO> griddata, MaterialIndentDTO BE)
        {

            bool issaved = false;

            try
            {
                int userid = Convert.ToInt32(this.Session["UserId"]);
                BE.createdby = userid;
                string indentno = "";
                issaved = mmsTransactions.SaveIndent(griddata, BE, out indentno);
                var result = new { Success = issaved, indentno = indentno };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult UpdateIndentDetails(List<MaterialIndentDTO> griddata, MaterialIndentDTO BE)
        {

            bool isupdated = false;

            try
            {
                int modifiedby = Convert.ToInt32(this.Session["UserId"]);
                BE.createdby = modifiedby;
                isupdated = mmsTransactions.UpdateIndentDetails(griddata, BE);
                var result = new { Success = isupdated };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetExistedIndenNo(string IndentType)
        {
            try
            {
                List<MaterialIndentDTO> lstIndentTypeDetails = mmsTransactions.GetExistingIndentNo(IndentType);
                var Indentdata = new { Indentdetails = lstIndentTypeDetails };
                return new JsonResult { Data = Indentdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetExistedIndents(string Indents)
        {
            try
            {


                List<MaterialIndentDTO> lstIndentts = mmsTransactions.GetExistingIndents(Indents);
                var Indentss = new { Indents = lstIndentts };

                return new JsonResult { Data = Indentss, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public ActionResult GetProductAvailability(string productid, string productname, string showroomselected, string storagelocationname, string uomname)
        //{
        //    try
        //    {
        //        List<MaterialIndentDTO> lstproductavailble = mmsTransactions.GetProductAvailability(productid, productname, showroomselected, storagelocationname, uomname);
        //        var varavailability = new { productavailability = lstproductavailble };
        //        return new JsonResult { Data = varavailability, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public ActionResult Deleteproductdetails(MaterialIndentDTO gridrowdata)
        {
            bool isdeleted = false;
            try
            {
                isdeleted = mmsTransactions.Deleteproductdetails(gridrowdata);
                var result = new { Success = isdeleted };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetReorderIndents(string IndentType)
        {
            List<MaterialIndentDTO> lstreorderindent = new List<MaterialIndentDTO>();
            try
            {
                lstreorderindent = mmsTransactions.GetReorderIndents(IndentType);
                var Reorderindent = new { Indentdetails = lstreorderindent };
                return new JsonResult { Data = Reorderindent, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region PurchaseReturn
        public ActionResult PurchaseReturn()
        {
            return View();
        }
        public JsonResult getdropdownvalues(string strType, string vendorid)
        {
            dt = new DataTable();
            dt = mmsTransactions.getdropdownvalues(strType, vendorid);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getpurchasereturnGridValues(string strtype, string ID)
        {
            dt = new DataTable();
            dt = mmsTransactions.getpurchasereturnGridValues(strtype, ID);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavePurchaseReturnCancel(GoodsReceivedNoteDTO PURCHASE, List<GoodsReceivedNoteDTO> lstPURCHASE)
        {
            Boolean IsSaved = false;
            PURCHASE.CreatedBy = Convert.ToInt32(Session["UserId"]);
            IsSaved = mmsTransactions.SavePurchaseReturnCancel(PURCHASE, lstPURCHASE);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region MaterialReturn
        public ActionResult MaterialReturn()
        {
            return View();
        }
        #endregion

        #region vendorpayment

        public ActionResult GetAccbalance()
        {
            int Cnt = mmsTransactions.Getbalance();
            return new JsonResult { Data = Cnt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetVendorData()
        {

            dt = new DataTable();
            dt = mmsTransactions.GetVendorNames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetBanksData()
        {

            dt = new DataTable();
            dt = mmsTransactions.GetBankNames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetChequesData(int id)
        {

            dt = new DataTable();
            dt = mmsTransactions.GetCheques(id);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }




        public ActionResult VendorPayment()
        {
            return View();
        }
        public ActionResult ShowVendorPayment(string VendorName)
        {
            VendorPaymentDTO VendorPaymentDTO = new VendorPaymentDTO();
            List<VendorPaymentDTO> lstVendorPayment = new List<VendorPaymentDTO>();

            lstVendorPayment = mmsTransactions.ShowVendorPaymentDetails(VendorName);

            var Alldata = new { griddetails = lstVendorPayment };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public ActionResult CheckMainashVoucherAmount(string lstTotalDetails, string lstInvDetails)
        {
            VendorPaymentDTO XYZ = serializer.Deserialize<VendorPaymentDTO>(lstTotalDetails);
            List<VendorPaymentDTO> ListItems = serializer.Deserialize<List<VendorPaymentDTO>>(lstInvDetails);
            Boolean IsTrueOrFalse = mmsTransactions.CheckMainashVoucherAmount(XYZ, ListItems);
            return new JsonResult { Data = IsTrueOrFalse, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public ActionResult SaveVendorPayment(string lstTotalDetails, string lstInvDetails)
        {

            List<VendorPaymentDTO> ListItems = serializer.Deserialize<List<VendorPaymentDTO>>(lstInvDetails);
            VendorPaymentDTO XYZ = serializer.Deserialize<VendorPaymentDTO>(lstTotalDetails);
            Boolean IsSaved = false;
            XYZ.createdby = 1;
            IsSaved = mmsTransactions.SaveVPDetails(XYZ, ListItems);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };




        }


        #endregion

        #region Material Return

        public ActionResult ShowIssuenumbers()
        {
            List<MaterialReturnDTO> lstDetails = mmsTransactions.ShowIssuenumbers();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ShowIndentnumbers()
        {
            List<MaterialReturnDTO> lstDetails = mmsTransactions.ShowIndentNos();
            return new JsonResult { Data = lstDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ShowMaterialReturn(string Number, string Returntype)
        {
            List<MaterialReturnDTO> lstMaterialReturn = mmsTransactions.ShowMaterialReturn(Number, Returntype);
            return new JsonResult { Data = lstMaterialReturn, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveMaterialReturn(MaterialReturnDTO MaterialReturnDTO, List<MaterialReturnDTO> lstMaterialReturn)
        {
            Boolean IsSaved = false;
            IsSaved = mmsTransactions.SaveMaterialReturn(MaterialReturnDTO, lstMaterialReturn);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #endregion


        #region  MMSSale

        public ActionResult MMSSale()
        {
            return View();
        }

        public ActionResult GetAllDetails()
        {
            List<MMSSaleDTO> objLBarcodeScanner = new List<MMSSaleDTO>();
            objLBarcodeScanner = mmsTransactions.GetAllDetails();
            return new JsonResult { Data = objLBarcodeScanner, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult GetAllProducts()
        {
            dt = mmsTransactions.GetAllProducts();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return new JsonResult { Data = JsonString, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetAvailableStock()
        {
            List<MMSAvailableStockdetails> objLstockdetails = new List<MMSAvailableStockdetails>();
            objLstockdetails = mmsTransactions.GetAvailableStock();
            return new JsonResult { Data = objLstockdetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //public bool SaveSaleDetails(string JsonString, string objLmmssalesave, string PaymentDetails, string Gridtotal)
        //{
        //    SavePaymentDetails objSavePaymentDetails1 = serializer.Deserialize<SavePaymentDetails>(PaymentDetails);
        //    MMSSaleSaveDTO ThirdDetails = serializer.Deserialize<MMSSaleSaveDTO>(JsonString);
        //    List<MMSSaleSaveDTO> objSaveDetails = serializer.Deserialize<List<MMSSaleSaveDTO>>(objLmmssalesave);
        //    int n = objSaveDetails.Count();
        //    bool ret = mmsTransactions.SaveSaleDetails(ThirdDetails, objSaveDetails, objSavePaymentDetails1, Gridtotal);
        //   // return new JsonResult { Data = ret, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    return ret;
        //}

        public JsonResult SaveSaleDetails(string JsonString, string objLmmssalesave, string PaymentDetails, string Gridtotal)
        {
            SavePaymentDetails objSavePaymentDetails1 = serializer.Deserialize<SavePaymentDetails>(PaymentDetails);
            MMSSaleSaveDTO ThirdDetails = serializer.Deserialize<MMSSaleSaveDTO>(JsonString);
            List<MMSSaleSaveDTO> objSaveDetails = serializer.Deserialize<List<MMSSaleSaveDTO>>(objLmmssalesave);
            int n = objSaveDetails.Count();
            string ret = mmsTransactions.SaveSaleDetails(ThirdDetails, objSaveDetails, objSavePaymentDetails1, Gridtotal);
            return new JsonResult { Data = ret, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        //public bool SavePaymentDetails(string objSavePaymentDetails, string Gridtotal)
        //{
        //    SavePaymentDetails objSavePaymentDetails1 = serializer.Deserialize<SavePaymentDetails>(objSavePaymentDetails);
        //    objSavePaymentDetails1.statusid = Convert.ToInt16(Session["UserId"]);
        //    return mmsTransactions.SavePaymentDetails(objSavePaymentDetails1, Gridtotal);
        //}


        //public bool DeleteTempDataOnBack(string JsonString)
        //{
        //    bool isDeleted = false;
        //    MMSSaleSaveDTO ThirdDetails = serializer.Deserialize<MMSSaleSaveDTO>(JsonString);
        //    isDeleted = mmsTransactions.DeleteTempDataOnBack(ThirdDetails);
        //    return isDeleted;
        //}   

        #endregion

        #region Sale Cancel

        public ActionResult SaleCancel()
        {


            return View();
        }
        public JsonResult GetSaleNos()
        {
            dt = mmsTransactions.GetSaleNos();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSaleNoItems(string Id)
        {
           // dt = mmsTransactions.GetSaleNoItems(Id);




            List<SaleCanceDto> lstSaleCanceDto = new List<SaleCanceDto>();

            lstSaleCanceDto = mmsTransactions.GetSaleNoItems(Id);

            var Alldata = new { griddetails = lstSaleCanceDto };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

          
        }


        public ActionResult SaveSaleCancel(string SaleNo, string lstInvDetails,string uncheckddata)
        {

            List<SaleCanceDto> ListItems = serializer.Deserialize<List<SaleCanceDto>>(lstInvDetails);
            List<SaleCanceDto> uncheckListItems = serializer.Deserialize<List<SaleCanceDto>>(uncheckddata);
           // VendorPaymentDTO XYZ = serializer.Deserialize<VendorPaymentDTO>(lstTotalDetails);
            Boolean IsSaved = false;
           // XYZ.createdby = 1;
            IsSaved = mmsTransactions.SaveSaleCancel(SaleNo, ListItems, uncheckListItems);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };




        }

        #endregion
    }
}