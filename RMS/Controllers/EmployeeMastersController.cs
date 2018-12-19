using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using RMS.Core;
using RMS.Core.Interfaces;
using RMS.Infrastructure;
using HRMS.Infrastructure;

namespace RMS.Controllers
{
    //[SessionExpire]
    public class EmployeeMastersController : Controller
    {
        //
        // GET: /EmployeeMasters/

        #region User Declearations

        private IEmployeeMastersRepository employeeMasters = new EmployeeMastersRepository();
        //private IPayRollMastersRepository PayrollMasters = new PayRollMastersRepository();

        #endregion


        /// <summary>
        /// Vikram
        /// </summary>
        /// <returns></returns>
        #region Marital status

        public ActionResult CreateMaritalStatus()
        {
            return View();
        }

        public bool SaveMaritalStatus(MaritalStatus marital)
        {
            return employeeMasters.SaveTypeOfMaritaStatus(marital);
        }

        public bool UpdateMaritalStatus(MaritalStatus objStatus)
        {
            return employeeMasters.UpdateMaritalStatus(objStatus);
        }

        public ActionResult GetTypeOfMaritalStatus()
        {
            List<MaritalStatus> lstMAritalStatus = new List<MaritalStatus>();
            lstMAritalStatus = employeeMasters.GetTypeOfMaritalStatus();
            return Json(lstMAritalStatus, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteTypeOfMaritalStatus(MaritalStatus objStatus)
        {
            return employeeMasters.DeleteTypeOfMaritalStatus(objStatus.recordid);
        }

        public JsonResult CheckMaritalStatus(MaritalStatus objStatus)
        {
            try
            {
                var c = employeeMasters.CheckMaritalStatus(objStatus);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RelationShip");
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }




        #endregion

        /// <summary>
        /// Not Assigned
        /// </summary>
        /// <returns></returns>
        #region Blood Group


        public ActionResult BloodGroup()
        {

            return View();
        }

        public JsonResult BindBloodGroup()
        {


            List<BloodGroupDTO> all = employeeMasters.BindBloodGroup();
            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult CreateBloodGroup(BloodGroupDTO Blood, string Status)
        {
            try
            {
                bool blod = employeeMasters.CreateBloodGroup(Blood);
                List<BloodGroupDTO> all = employeeMasters.BindBloodGroup();
                var result = new { Success = blod, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Bllod_Group");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        public JsonResult UpdateBloodGroup(BloodGroupDTO Blood)
        {

            try
            {

                bool blod = employeeMasters.UpdateBloodGroup(Blood);

                List<BloodGroupDTO> all = employeeMasters.BindBloodGroup();
                var result = new { Success = blod, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Bllod_Group");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        public JsonResult DeleteBloodGroup(BloodGroupDTO Blood)
        {
            var c = employeeMasters.DeleteBloodGroup(Blood.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckBloodGroup(BloodGroupDTO Blood)
        {
            var c = employeeMasters.CheckBloodGroup(Blood);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //DeleteBloodGroup


        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        #region RelationShip ...

        public ActionResult RelationShip()
        {

            return View();
        }

        public JsonResult BindRelation()
        {
            List<RelationDTO> all = employeeMasters.BindRelations();
            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveRelations(RelationDTO Rel, string Status)
        {

            try
            {
                //if (ModelState.IsValid)
                //{
                // employeeMasters objemployeeMasters = new employeeMasters();
                bool res = employeeMasters.Save(Rel);
                // return Json(new { success = res });

                //}
                //else
                //{
                //    return Json(new { success = false });
                //}


                List<RelationDTO> all = employeeMasters.BindRelations();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RelationShip");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        public JsonResult UpdateRelations(RelationDTO Rel)
        {

            try
            {

                bool res = employeeMasters.UpdateRelations(Rel);

                List<RelationDTO> all = employeeMasters.BindRelations();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RelationShip");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult DeleteRelation(RelationDTO Rel)
        {
            var c = employeeMasters.DeleteRecord(Rel.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckRelations(RelationDTO Rel)
        {
            try
            {
                var c = employeeMasters.CheckRelations(Rel);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "RelationShip");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        #endregion

        /// <summary>
        /// Rajesh
        /// </summary>
        /// <returns></returns>
        /// 
        #region Country...

        public ActionResult CreateCountry()
        {
            return View();
        }
        public bool SaveCountry(CountryDTO objCountry)
        {
            return employeeMasters.SaveCountry(objCountry);

        }
        [HttpGet]
        public JsonResult ShowCountryDetails()
        {
            List<CountryDTO> lstEADetails = new List<CountryDTO>();
            lstEADetails = employeeMasters.ShowCountryDetails();
            return new JsonResult { Data = lstEADetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public bool UpdateCountry(CountryDTO objCountry)
        {

            return employeeMasters.UpdateCountry(objCountry);
        }
        public bool DeleteCountry(CountryDTO objCountry)
        {
            return employeeMasters.DeleteCountry(objCountry.CountryId);
        }

        public bool CheckCountry(CountryDTO objCountry)
        {
            return employeeMasters.CheckCountry(objCountry);
        }



        #endregion

        /// <summary>
        /// Rajesh
        /// </summary>
        /// <returns></returns>
        #region State

        public ActionResult CreateState()
        {
            return View();
        }
        public bool SaveState(StateDTO objState)
        {
            return employeeMasters.SaveState(objState);

        }
        public bool UpdateState(StateDTO objState)
        {
            return employeeMasters.UpdateState(objState);

        }
        public bool DeleteState(StateDTO objState)
        {
            return employeeMasters.DeleteState(objState.StateId, objState.CountryId);

        }

        public JsonResult BindStateDetails()
        {
            EmployeeMastersRepository objEmployeeMastersRepository = new EmployeeMastersRepository();
            List<StateDTO> lstState = new List<StateDTO>();
            lstState = objEmployeeMastersRepository.BindState();
            return this.Json(lstState, JsonRequestBehavior.AllowGet);
        }

        public bool CheckState(StateDTO objState)
        {
            return employeeMasters.CheckState(objState);

        }

        //[HttpGet]
        //public JsonResult ShowCountryDetails()
        //{
        //    List<CountryDTO> lstEADetails = new List<CountryDTO>();
        //    lstEADetails = employeeMasters.ShowCountryDetails();
        //    return new JsonResult { Data = lstEADetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
        //public bool UpdateCountry(CountryDTO objCountry)
        //{

        //    return employeeMasters.UpdateCountry(objCountry);
        //}
        //public bool DeleteCountry(CountryDTO objCountry)
        //{
        //    return employeeMasters.DeleteCountry(objCountry.CountryId);
        //}

        #endregion

        /// <summary>
        /// RajiReddy
        /// </summary>
        /// <returns></returns>
        #region CityDetails

        [HttpGet]
        public ActionResult CreateCity()
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
                    isValid = employeeMasters.CreateCity(m);
                    lstCitydetails = employeeMasters.BindCityDetails();
                }

                var result = new { Success = isValid, data = lstCitydetails };
                var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "CityDetails");
                throw ex;
            }
        }

        public JsonResult ShowCountry()
        {
            List<City> lstcountry = new List<City>();
            if (ModelState.IsValid)
            {
                lstcountry = employeeMasters.ShowCountry();
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
                    lststate = employeeMasters.ShowStates(strcountryid);
                }

                var data = new JsonResult { Data = lststate, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "States");
                throw;
            }
        }

        public JsonResult BindCityDetails()
        {
            try
            {
                List<City> lstcity = employeeMasters.BindCityDetails();
                return new JsonResult { Data = lstcity, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }

        public bool UpdateCity(City objcity)
        {
            try
            {
                return employeeMasters.UpdateCity(objcity);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }

        public bool DeleteCity(City cdto)
        {
            try
            {
                return employeeMasters.DeleteCity(cdto);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }

        public bool CheckCity(City objcity)
        {
            try
            {
                return employeeMasters.CheckCity(objcity);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Madhu
        /// </summary>
        /// <returns></returns>
        #region Department

        [HttpGet]
        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BindCompanyNames()
        {
            List<DepartmentMaster> lstDepartment = employeeMasters.BindCompanyNames();
            List<DepartmentMaster> ShowDepartmentDetails = employeeMasters.ShowDepartmentDetails();

            var getdata = new { company = lstDepartment, griddetails = ShowDepartmentDetails };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public JsonResult ShowDepartmentDetails()
        {
            List<DepartmentMaster> lstDepartment = employeeMasters.ShowDepartmentDetails();
            return new JsonResult { Data = lstDepartment, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveDepartment(DepartmentMaster Dept)
        {
            bool isSaved = employeeMasters.SaveDepartment(Dept);
            List<DepartmentMaster> lstDepartment = employeeMasters.ShowDepartmentDetails();
            var getdata = new { griddetails = lstDepartment, Saved = isSaved };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //string daata = "";
            //return new JsonResult { Data = lstDepartment, daata = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult DeleteDepartment(DepartmentMaster Dept)
        {

            var c = employeeMasters.DeleteDepartment(Dept.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public JsonResult UpdateDepartment(DepartmentMaster Dept)
        {

            bool isUpdate = employeeMasters.UpdateDepartment(Dept);
            List<DepartmentMaster> lstDepartment = employeeMasters.ShowDepartmentDetails();
            var getdata = new { griddetails = lstDepartment, isUpdate = isUpdate };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult CheckDepartment(DepartmentMaster Dept)
        {
            try
            {
                var c = employeeMasters.CheckDepartment(Dept);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        #endregion

        /// <summary>
        /// K.Nagendar
        /// </summary>
        /// <returns></returns>
        #region Designation


        public ActionResult Designation()
        {
            return View();
        }

        private string ConvertDatatableToJsonString(DataTable dt)
        {
            string JsonString = "";
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            JsonString = serializer.Serialize(rows);

            return JsonString;
        }

        public JsonResult CreateDesignation(string JsonData)
        {

            bool result = employeeMasters.SaveDesignation(JsonData);
            string JsonString = "";
            if (result == true)
            {
                DataTable dt = new DataTable();
                dt = employeeMasters.ShowDesignation();
                JsonString = ConvertDatatableToJsonString(dt);
            }

            var Totalresult = new { Data = JsonString, TorF = result };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowDesignation()
        {
            string JsonString = "";
            DataTable dt = new DataTable();
            dt = employeeMasters.ShowDesignation();
            JsonString = ConvertDatatableToJsonString(dt); ;
            var Totalresult = new { Data = JsonString };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDesignation(int id)
        {
            string JsonString = "";
            bool Status = employeeMasters.DeleteDesignation(id);
            DataTable dt = new DataTable();
            dt = employeeMasters.ShowDesignation();
            JsonString = ConvertDatatableToJsonString(dt);
            var Totalresult = new { Data = JsonString, TorF = Status };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDesignation(int id, string JsonData)
        {
            string JsonString = "";
            bool Status = employeeMasters.UpdateDesignation(id, JsonData);
            if (Status == true)
            {
                DataTable dt = new DataTable();
                dt = employeeMasters.ShowDesignation();
                JsonString = ConvertDatatableToJsonString(dt);
            }
            var Totalresult = new { Data = JsonString, TorF = Status };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDesignation(string JsonData)
        {
            try
            {
                var c = employeeMasters.CheckDesignation(JsonData);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Designation");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }


        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        /// <returns></returns>
        #region COURSE...



        public ActionResult CreateCourse()
        {
            return View();
        }

        public JsonResult ShowCourse()
        {
            List<CourseDTO> all = employeeMasters.ShowCourse();
            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult CreateCourse(CourseDTO Rel, string Status)
        {

            try
            {
                //if (ModelState.IsValid)
                //{
                // employeeMasters objemployeeMasters = new employeeMasters();
                bool res = employeeMasters.CreateCourse(Rel);
                // return Json(new { success = true });

                //}
                //else
                //{
                //    return Json(new { success = false });
                //}


                List<CourseDTO> all = employeeMasters.ShowCourse();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Course");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        public JsonResult UpdateCourse(CourseDTO Rel)
        {

            try
            {

                bool res = employeeMasters.UpdateCourse(Rel);

                List<CourseDTO> all = employeeMasters.ShowCourse();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Course");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult DeleteCourse(CourseDTO Rel)
        {
            var c = employeeMasters.DeleteCourse(Rel.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckCourse(CourseDTO Rel)
        {
            var c = employeeMasters.CheckCourse(Rel);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        /// <returns></returns>
        #region GROUP...

        public ActionResult CreateGroup()
        {
            return View();
        }

        public JsonResult ShowGroup()
        {
            List<GroupDTO> all = employeeMasters.ShowGroup();
            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult CreateGroup(GroupDTO grp, string Status)
        {

            try
            {
                //if (ModelState.IsValid)
                //{
                // employeeMasters objemployeeMasters = new employeeMasters();
                bool res = employeeMasters.CreateGroup(grp);
                // return Json(new { success = true });

                //}
                //else
                //{
                //    return Json(new { success = false });
                //}


                List<GroupDTO> all = employeeMasters.ShowGroup();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Course");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        public JsonResult UpdateGroup(GroupDTO grp)
        {

            try
            {

                bool res = employeeMasters.UpdateGroup(grp);

                List<GroupDTO> all = employeeMasters.ShowGroup();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Course");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult DeleteGroup(GroupDTO grp)
        {
            var c = employeeMasters.DeleteGroup(grp.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckGroup(GroupDTO grp)
        {
            var c = employeeMasters.CheckGroup(grp);
            return new JsonResult
            {
                Data = c,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        /// <summary>
        /// Srinivas
        /// </summary>
        /// <returns></returns>
        #region Occupation

        public ActionResult CreateOccupation()
        {
            return View();
        }
        [HttpGet]
        public JsonResult ShowOccupationDetails()
        {
            List<OccupationMaster> lstOccupation = employeeMasters.ShowOccupationDetails();
            var getdata = new { griddetails = lstOccupation };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveOccupation(OccupationMaster Occupation)
        {
            bool isSaved = employeeMasters.SaveOccupation(Occupation);
            List<OccupationMaster> lstOccupation = employeeMasters.ShowOccupationDetails();
            var getdata = new { griddetails = lstOccupation, Saved = isSaved };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //string daata = "";
            //return new JsonResult { Data = lstDepartment, daata = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult DeleteOccupation(OccupationMaster Occupation)
        {

            var c = employeeMasters.DeleteOccupation(Occupation.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public JsonResult UpdateOccupation(OccupationMaster Occupation)
        {

            bool isUpdate = employeeMasters.UpdateOccupation(Occupation);
            List<OccupationMaster> lstOccupation = employeeMasters.ShowOccupationDetails();
            var getdata = new { griddetails = lstOccupation, isUpdate = isUpdate };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult CheckOccupation(OccupationMaster Occupation)
        {
            try
            {
                var c = employeeMasters.CheckOccupation(Occupation);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Occupation");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region Reason for Leaving

        public ActionResult CreateReasonforLeaving()
        {
            return View();
        }

        public JsonResult BindReasonForLeaving()
        {
            List<ReasonForLeavingDTO> all = employeeMasters.BindReasonForLeaving();
            var data = new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }
        public JsonResult SaveReasonForLeavingDetails(ReasonForLeavingDTO ResForLeaning)
        {

            try
            {
                ResForLeaning.StatusID = 1;
                ResForLeaning.createdby = Convert.ToInt32(Session["UserId"]);
                bool res = employeeMasters.Save(ResForLeaning);

                List<ReasonForLeavingDTO> all = employeeMasters.BindReasonForLeaving();
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_for_Leaving");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult DeleteReasonForLeaving(ReasonForLeavingDTO ResForLeaning)
        {

            bool c = employeeMasters.DeleteReasonForLeavingRecord(ResForLeaning.RecordId);


            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult UpdateReasonForLeavingRecord(ReasonForLeavingDTO ResForLeaning)
        {

            bool c = employeeMasters.UpdateReasonForLeavingRecord(ResForLeaning);
            List<ReasonForLeavingDTO> all = employeeMasters.BindReasonForLeaving();
            var result = new { Success = c, data = all };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckReasonForLeaving(ReasonForLeavingDTO ResForLeaning)
        {
            try
            {
                var c = employeeMasters.CheckReasonForLeaving(ResForLeaning);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ResForLeaning");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region ReasonForTransfer

        public ActionResult ReasonForTransfering()
        {
            return View();
        }
        public JsonResult BindReasonForTransfering()
        {
            List<ReasonForTransferingDTO> all = employeeMasters.BindReasonForTransfering();
            var data = new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }
        public JsonResult SaveReasonForTransferDetails(ReasonForTransferingDTO ResForTrans)
        {

            try
            {
                ResForTrans.StatusID = 1;
                ResForTrans.createdby = Convert.ToInt32(Session["UserId"]);
                bool res = employeeMasters.Save(ResForTrans);

                List<ReasonForTransferingDTO> all = employeeMasters.BindReasonForTransfering();
                var result = new { Success = res, griddata = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_for_Transfer");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult DeleteReasonForTransfer(ReasonForTransferingDTO ResForTrans)
        {

            bool c = employeeMasters.DeleteReasonForTransferingRecord(ResForTrans.RecordId);


            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult UpdateReasonForTransferRecord(ReasonForTransferingDTO ResForTrans)
        {

            bool c = employeeMasters.UpdateReasonForTransferingRecord(ResForTrans);
            List<ReasonForTransferingDTO> all = employeeMasters.BindReasonForTransfering();
            var result = new { Success = c, griddata = all };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CheckReasonForTransfering(ReasonForTransferingDTO ResForTrans)
        {
            try
            {
                var c = employeeMasters.CheckReasonForTransfering(ResForTrans);
                return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ReasonForTransfer");
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        #endregion

        /// <summary>
        /// Satish.D
        /// </summary>
        /// <returns></returns>
        #region TypeOfEmployment

        public ActionResult CreateTypeOfEmployee()
        {
            return View();
        }

        public bool SaveEmployement(EmployeeDTO objEmp)
        {
            return employeeMasters.SaveTypeOfEmployee(objEmp);
        }

        public bool UpdateEmployement(EmployeeDTO objEmp)
        {
            return employeeMasters.UpdateEmployement(objEmp);
        }

        public ActionResult GetTypeOfEmployement()
        {
            List<EmployeeDTO> lstEmp = new List<EmployeeDTO>();
            lstEmp = employeeMasters.GetTypeOfEmployement();
            return Json(lstEmp, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteTypeOfEmployement(EmployeeDTO objEmp)
        {
            return employeeMasters.DeleteTypeOfEmployement(objEmp.recordid);
        }

        public bool CheckTypeOfEmployement(EmployeeDTO objEmp)
        {
            return employeeMasters.CheckTypeOfEmployement(objEmp);
        }


        #endregion

        /// <summary>
        /// Not Assigned
        /// </summary>
        /// <returns></returns>
        #region Type of Receipt

        #endregion

        /// <summary>
        /// Not Assigned
        /// </summary>
        /// <returns></returns>
        #region Branch

        #endregion

        /// <summary>
        /// Not Assigned
        /// </summary>
        /// <returns></returns>
        #region Study of Child

        #endregion

        //#region SalaryType
        //public ActionResult SalaryType()
        //{
        //    List<SalaryTypeDTO> all = PayrollMasters.BindSalaryType();
        //    var getdata = new { Data = all };
        //    ViewBag.Data = getdata;
        //    return View();
        //}


        // #endregion

        #region Shift Master

        public ActionResult ShiftMaster()
        {
            return View();
        }

        public JsonResult CreateShift(Shift objShift)
        {
            List<Shift> lstShift = new List<Shift>();
            List<Shift> lstShiftdetails = new List<Shift>();
            bool isValid = false;
            try
            {

                if (ModelState.IsValid)
                {
                    isValid = employeeMasters.CreateShift(objShift);
                    lstShiftdetails = employeeMasters.ShowShiftDetails();
                }

                var result = new { Success = isValid, data = lstShiftdetails };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw ex;
            }
        }

        public JsonResult ShowShiftDetails()
        {
            try
            {
                List<Shift> lstShiftdetails = employeeMasters.ShowShiftDetails();
                return new JsonResult { Data = lstShiftdetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }
        }

        public bool UpdateShift(Shift objShift)
        {
            try
            {
                return employeeMasters.UpdateShift(objShift);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }
        }

        public bool DeleteShift(Shift objShift)
        {
            try
            {
                return employeeMasters.DeleteShift(objShift);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }
        }

        public ActionResult GetShiftExist(int From, int To, string ShiftName)
        {
            int count = employeeMasters.GetShiftExist(From, To, ShiftName);
            var Totalresult = new { Data = count };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
        }


        public bool CheckShift(Shift objShift)
        {
            try
            {
                return employeeMasters.CheckShift(objShift);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }
        }


        #endregion





















    }
}
