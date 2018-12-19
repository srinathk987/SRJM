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
    public class RMSMastersController : Controller
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        #region User Declarations...

        private IRMSMasters RmsMasters = new RMSMastersRepository();
      //  private IMMSReports mmsReports = new MMSReportsRepository();
        DataTable dt = null;
        string JsonString = string.Empty;

        #endregion
        //
        // GET: /RMSMasters/
        public ActionResult Index()
        {
            return View();
        }

        #region RMSMASTER


        public ActionResult ShowRmsManagement()
        {



            return View();


        }



        public JsonResult GetItemNames()
        {

            dt = new DataTable();
            dt = RmsMasters.GetItemNames();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }



        public JsonResult Getcategoryandsub(string name)
        {

            dt = new DataTable();
            dt = RmsMasters.GetCategoryAndSubCategory(name);
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetProductsNames()
        {

            dt = new DataTable();
            dt = RmsMasters.GetProducts();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ShowProducts(string ProductNames)
        {
            List<RmsDTO> lstDetails = RmsMasters.ShowProductCategory(ProductNames);


            var result = new { lstDetails = lstDetails };

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public ActionResult SaveRMSManagement(string lstRMSDTO, string lstRMSDTODetails)
        {
            RmsDTO XYZ = null;

            List<RmsDTO> ListItems = serializer.Deserialize<List<RmsDTO>>(lstRMSDTODetails);

            if(lstRMSDTO!=null)
            { 
            XYZ = serializer.Deserialize<RmsDTO>(lstRMSDTO);
            }
           
            Boolean IsSaved = false;
            // XYZ.createdby = 1;
            IsSaved = RmsMasters.SaveRMSDetails(XYZ, ListItems);
            return new JsonResult { Data = IsSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };




        }


        public JsonResult GetItemsRMS()
        {

            dt = new DataTable();
            dt = RmsMasters.GetItemsFromRms();
            JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }





        public ActionResult ShowItms(string ItmName)
        {
            List<RmsDTO> lstDetails = RmsMasters.ShowItmCategory(ItmName);


            var result = new { lstDetails = lstDetails };

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        #endregion
	}
}