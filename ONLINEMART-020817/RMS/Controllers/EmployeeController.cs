using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMS.Core;
using RMS.Core.Interfaces;
using HelperManager;
using System.Data;
using RMS.Infrastructure;
using System.Text;
using HRMS.Infrastructure;


namespace RMS.Controllers
{
    //[SessionExpire]
    public class EmployeeController : Controller
    {
        #region UserDeclearations

        private IEmployeeRepository objEmployeeRepository = new EmployeeRepository();

        #endregion


        /// <summary>
        /// SRIKANTH.K
        /// </summary>
        /// <returns></returns>
        #region Deductions
        
        public ActionResult CreateDeductions()
        {
            return View();
        }



        #endregion

        /// <summary>
        /// Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Personal Information

        public ActionResult PersonalInformation()
        {
            return View();
        }

        public JsonResult GenerateEMPId()
        {
            EmployeeRepository objEmployeeMastersRepository = new EmployeeRepository();
            string NextId = objEmployeeMastersRepository.BindEMPId();
            return this.Json(NextId, JsonRequestBehavior.AllowGet);

        }
        //public JsonResult SaveEmployeeDetails(HRMS.Core.PersonalInformation User)
        //{
        //    EmployeeRepository objEmployeeMastersRepository = new EmployeeRepository();
        //    string NextId = objEmployeeMastersRepository.SaveBranchAllowance(User);

        //    return this.Json(NextId, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult FileUpload(HttpPostedFileBase file)
        {
            string filepathtosave = string.Empty;
            try
            {
                /*Geting the file name*/
                string filename = System.IO.Path.GetFileName(file.FileName);
                /*Saving the file in server folder*/
                file.SaveAs(Server.MapPath("~/Images/" + filename));
                filepathtosave = "Images/" + filename;
                /*Storing image path to show preview*/
                ViewBag.ImageURL = filepathtosave;
                /*
                 * HERE WILL BE YOUR CODE TO SAVE THE FILE DETAIL IN DATA BASE
                 *
                 */

                ViewBag.Message = "File Uploaded successfully.";
            }
            catch
            {
                ViewBag.Message = "Error while uploading the files.";
            }
            return this.Json(filepathtosave, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindMaritalStatus()
        {
            EmployeeRepository objEmployeeMastersRepository = new EmployeeRepository();
            List<EmergencyContact> lstMaritalStatus = new List<EmergencyContact>();
            lstMaritalStatus = objEmployeeMastersRepository.BindMaritalStatus();
            return this.Json(lstMaritalStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindBloodGroup()
        {
            EmployeeRepository objEmployeeMastersRepository = new EmployeeRepository();
            List<EmergencyContact> lstMaritalStatus = new List<EmergencyContact>();
            lstMaritalStatus = objEmployeeMastersRepository.BindBloodGroup();
            return this.Json(lstMaritalStatus, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// Madhu
        /// </summary>
        /// <returns></returns>
        #region Employee Identification and Other Details

        public ActionResult EmployeeIdentificationAndOtherDetails()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveEmployeeIdentifications(EmployeeIdentification EmployeeIdentification, List<BankDetails> BankDetails)
        {
            string isSaved = string.Empty;
            isSaved = objEmployeeRepository.SaveEmployeeIdentifications(EmployeeIdentification, BankDetails);

            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ShowEmployeeIdentificationDetails(string Employeeid)
        {
            EmployeeIdentification objEmployeeIdentification = new EmployeeIdentification();
            objEmployeeIdentification = objEmployeeRepository.ShowEmployeeIdentificationDetails(Employeeid);
            return new JsonResult { Data = objEmployeeIdentification, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult DeleteBankDetails(BankDetails BankDetails)
        {
            bool IsDeleted = objEmployeeRepository.DeleteBankDetails(BankDetails.Bankrecordid);
            return new JsonResult { Data = IsDeleted, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult CheckAccountNoExist(EmployeeIdentification EmployeeIdentification, BankDetails BankDetails)
        {
            bool isSaved = false;
            isSaved = objEmployeeRepository.CheckAccountNoExist(EmployeeIdentification, BankDetails);

            return new JsonResult { Data = isSaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region Employee Allowance Check

        public ActionResult EmpAllowCheck()
        {
            return View();
        }

        public JsonResult SaveEmpAllowCheckDetails(EmpAllowanceCheck objEmpAllowCheck, List<EmpAllowanceCheck> lstEmpAllowCheck)
        {

            try
            {
                objEmpAllowCheck.statusid = 1;
                objEmpAllowCheck.createdby = Convert.ToInt32(Session["UserId"]);

                bool res = objEmployeeRepository.Save(objEmpAllowCheck, lstEmpAllowCheck);
                return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            catch (Exception ex)
            {
                throw ex;
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }

        public JsonResult ShowEmployeeName()
        {
            List<EmpAllowanceCheck> lstemployeename = new List<EmpAllowanceCheck>();


            try
            {
                lstemployeename = objEmployeeRepository.ShowEmployeeName();
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "CalenderPeriod");
            }

            var data = new JsonResult { Data = lstemployeename, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }
        public JsonResult ShowDeductionEmployeeName()
        {
            List<EmpAllowanceCheck> lstemployeename = new List<EmpAllowanceCheck>();


            try
            {
                lstemployeename = objEmployeeRepository.ShowDeductionEmployeeName();
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "CalenderPeriod");
            }

            var data = new JsonResult { Data = lstemployeename, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }




        [HttpPost]
        public JsonResult ShowEmployeeAllowenceDetails(string vchemployeeid)
        {
            List<EmpAllowanceCheck> lstEmployeeAllowenceDetails = new List<EmpAllowanceCheck>();

            try
            {

                lstEmployeeAllowenceDetails = objEmployeeRepository.ShowEmployeeAllowenceDetails(vchemployeeid);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmployeeAlloweanceCheck");
            }

            var gridData = new JsonResult { Data = lstEmployeeAllowenceDetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return gridData;
        }

        #endregion

        /// <summary>
        /// Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Emergency Contact details

        public ActionResult EmergencyContactDetails()
        {
            return View();
        }

        public JsonResult BindRelationShip()
        {
            EmployeeRepository objEmployeeMastersRepository = new EmployeeRepository();
            List<EmergencyContact> lstRelationShip = new List<EmergencyContact>();
            lstRelationShip = objEmployeeMastersRepository.BindRelationShip();
            return this.Json(lstRelationShip, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveEmergencyContactDetails(HRMS.Core.EmergencyContactDetails User)
        //{
        //    EmployeeRepository objEmployeeMastersRepository = new EmployeeRepository();
        //    objEmployeeMastersRepository.SaveEmergencyContactDetails(User);

        //    return this.Json(JsonRequestBehavior.AllowGet);
        //}


        #endregion

        /// <summary>
        /// naveen krishna
        /// </summary>
        /// <returns></returns>
        #region Family Details

        public JsonResult GetEmployee_family()
        {

            List<EmployeeFamilydetails> lstls = objEmployeeRepository.getEmployeeid_family();


            return new JsonResult { Data = lstls, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult DeleteFamilyMember(string recordid, string drecordid)
        {
            try
            {
                bool res = objEmployeeRepository.DeleteFamilyDetails(recordid, drecordid);
                List<occupationdetails> lstoccupation = new List<Core.occupationdetails>();
                List<RelationDetails> lstrelation = new List<Core.RelationDetails>();
                List<EmployeeFamilydetails> lstempfamily = new List<Core.EmployeeFamilydetails>();

                lstoccupation = objEmployeeRepository.GetOccupation();
                lstrelation = objEmployeeRepository.GetRelation();
                lstempfamily = objEmployeeRepository.GetFamilyDetails();
                var getdata = new { GetOccupationDetails = lstoccupation, GetRelationDetails = lstrelation, GetFamilyDetails = lstempfamily };
                ViewBag.EmployeeFamilyDetails = getdata;

                return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; ;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public JsonResult getFamilyDetails()
        {
            List<occupationdetails> lstoccupation = new List<Core.occupationdetails>();
            List<RelationDetails> lstrelation = new List<Core.RelationDetails>();
            List<EmployeeFamilydetails> lstempfamily = new List<Core.EmployeeFamilydetails>();
            List<FamilyEducationDetails> lsteducations = new List<Core.FamilyEducationDetails>();

            lstoccupation = objEmployeeRepository.GetOccupation();
            lstrelation = objEmployeeRepository.GetRelation();
            lstempfamily = objEmployeeRepository.GetFamilyDetails();
            lsteducations = objEmployeeRepository.GetEducationDetails();

            var getdata = new { GetOccupationDetails = lstoccupation, GetRelationDetails = lstrelation, GetFamilyDetails = lstempfamily, GetEducations = lsteducations };
            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; ;

        }

        [HttpGet]
        public ActionResult CreateFamilyDetails()
        {
            List<occupationdetails> lstoccupation = new List<Core.occupationdetails>();
            List<RelationDetails> lstrelation = new List<Core.RelationDetails>();
            List<EmployeeFamilydetails> lstempfamily = new List<Core.EmployeeFamilydetails>();
            List<FamilyEducationDetails> lsteducations = new List<Core.FamilyEducationDetails>();

            lstoccupation = objEmployeeRepository.GetOccupation();
            lstrelation = objEmployeeRepository.GetRelation();
            lstempfamily = objEmployeeRepository.GetFamilyDetails();
            lsteducations = objEmployeeRepository.GetEducationDetails();

            var getdata = new { GetOccupationDetails = lstoccupation, GetRelationDetails = lstrelation, GetFamilyDetails = lstempfamily, GetEducations = lsteducations };
            ViewBag.EmployeeFamilyDetails = getdata;
            return View();
        }

        //[HttpPost]
        //public JsonResult CreateFamilyDetails(EmployeeFamilydetails FamilyDetails)
        //{

        //    try
        //    {
        //        int status = 0;
        //        EmployeeFamilydetails objEducationMaster = new EmployeeFamilydetails();

        //        if (objEmployeeRepository.CreateEmpFamilyDetails(FamilyDetails))
        //        {
        //            status = 1;
        //        }


        //        List<EmployeeFamilydetails> listfamily = new List<EmployeeFamilydetails>();

        //        listfamily = objEmployeeRepository.GetFamilyDetails();
        //        var getdata = new { griddetails = listfamily, msgstatus = status };
        //        var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        return data;

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, errorMessage = ex.Message });
        //    }
        //}

        public JsonResult UpdateFamilyDetails(EmployeeFamilydetails objFamilyDetails)
        {
            try
            {
                objEmployeeRepository.UpdateFamilyDetails(objFamilyDetails);
                List<EmployeeFamilydetails> Listfamilydetais = new List<EmployeeFamilydetails>();
                Listfamilydetais = objEmployeeRepository.GetFamilyDetails();
                bool status = true;
                var getdata = new { griddetails = Listfamilydetais, msgstatus = status };
                var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        public JsonResult GetChildrenDetails()
        {
            List<EmployeeFamilydetails> lstfc = new List<Core.EmployeeFamilydetails>();
            lstfc = objEmployeeRepository.GetChildrenDetails();

            return new JsonResult { Data = lstfc, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        /// <summary>
        /// Vijay
        /// </summary>
        /// <returns></returns>
        #region Addresses

        public ActionResult Addresses()
        {

            return View();
        }

        //[HttpPost]
        //public ActionResult Addresses(Addresses objAddresses)
        //{
        //    bool issaved;

        //    issaved = objEmployeeRepository.SaveEmployee(objAddresses);
        //    return View();
        //    //return View(objAddresses);
        //    //   return new JsonResult { Data = issaved, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}


        public JsonResult BindCities(string strSateid)
        {
            List<City> lststate = objEmployeeRepository.ShowCities(strSateid);
            return new JsonResult { Data = lststate, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult ShowEmployees()
        {
            List<Addresses> lstaddress = new List<Addresses>();
            if (ModelState.IsValid)
            {
                lstaddress = objEmployeeRepository.LoadEmployeeNames();

            }
            var data = new JsonResult { Data = lstaddress, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return data;
        }

        #endregion

        /// <summary>
        /// Srinath
        /// </summary>
        /// <returns></returns>
        #region EducationDetails

        /// <summary>
        /// Binding Groups Based On CourseName
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public JsonResult ShowGroups(string strName)
        {
            List<Group> lstgroup = new List<Group>();
            if (ModelState.IsValid)
            {
                lstgroup = objEmployeeRepository.GetGroups(strName);

                // return RedirectToAction("Index");
            }
            //return View(strcity);

            var data = new JsonResult { Data = lstgroup, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }



        public JsonResult GetEmployeeIds()
        {
            List<EducationDetails> listPersonaldetails = new List<EducationDetails>();

            listPersonaldetails = objEmployeeRepository.GetEmployeeiddetails();


            return new JsonResult { Data = listPersonaldetails, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult Getlistcourse()
        {
            List<Course> listcourse = new List<Course>();

            listcourse = objEmployeeRepository.GetcourseDetails();

            return new JsonResult { Data = listcourse, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Getlistgroups()
        {
            List<Group> listgroup = new List<Group>();

            listgroup = objEmployeeRepository.GetGroupDetails();

            return new JsonResult { Data = listgroup, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult showeducationdetails()
        {
            List<EducationDetails> listeducationDetailsgrid = new List<EducationDetails>();
            listeducationDetailsgrid = objEmployeeRepository.ShowEducationDetails();

            return new JsonResult { Data = listeducationDetailsgrid, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        /// <summary>
        /// Showing  Education Details View
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult CreateEducationDetails()
        {
            //List<EducationDetails> listeducationDetailsgrid = new List<EducationDetails>();

            //List<EducationDetails> listPersonaldetails = new List<EducationDetails>();

            //List<Course> listcourse = new List<Course>();

            //List<Group> listgroup = new List<Group>();

            //listgroup = objEmployeeRepository.GetGroupDetails();

            //listcourse = objEmployeeRepository.GetcourseDetails();

            //listeducationDetailsgrid = objEmployeeRepository.ShowEducationDetails();

            //listPersonaldetails = objEmployeeRepository.GetEmployeeiddetails();


            //var getdata = new { Getpersonaldetails = listPersonaldetails, Getcouse = listcourse, GroupDeatis = listgroup, griddetails = listeducationDetailsgrid };

            //ViewBag.EducationDetails = getdata;
            return View();

        }


        /// <summary>
        /// Saving Education Details
        /// </summary>
        /// <param name="EducationDetails"></param>
        /// <returns></returns>

        //[HttpPost]
        //public JsonResult CreateEducationDetails(EducationDetails EducationDetails)
        //{

        //    try
        //    {

        //        EducationDetails objEducationMaster = new EducationDetails();

        //        int status = objEmployeeRepository.CreateEducationDetails(EducationDetails);

        //        List<EducationDetails> listeducation = new List<EducationDetails>();

        //        listeducation = objEmployeeRepository.ShowEducationDetails();
        //        var getdata = new { griddetails = listeducation, msgstatus = status };
        //        var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        return data;

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, errorMessage = ex.Message });
        //    }
        //}


        /// <summary>
        /// Updating Education Details
        /// </summary>
        /// <param name="EducationDetails"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult UpdateEducationDetails(EducationDetails EducationDetails)
        {

            try
            {
                int status = objEmployeeRepository.UpdateEducationDetails(EducationDetails);


                List<EducationDetails> ListEducationMaster = new List<EducationDetails>();

                ListEducationMaster = objEmployeeRepository.ShowEducationDetails();
                var getdata = new { griddetails = ListEducationMaster, msgstatus = status };
                var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        /// <summary>
        /// Deleting Education Details
        /// </summary>
        /// <param name="Employeeid"></param>
        /// <returns></returns>

        public JsonResult DeleteEducationDetails(string Employeeid)
        {
            int status = objEmployeeRepository.DeleteEducationDetailsResult(Employeeid);


            List<EducationDetails> ListEducationDetails = new List<EducationDetails>();

            ListEducationDetails = objEmployeeRepository.ShowEducationDetails();
            var getdata = new { griddetails = ListEducationDetails, msgstatus = status };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;


        }


        #endregion

        /// <summary>
        /// Santosh
        /// </summary>
        /// <returns></returns>
        #region Training/Course
        public ActionResult createCertificationTraining()
        {

            return View();
        }

        public JsonResult ShowEmployeeCourses()
        {
            List<employeeCourses> lstEmployeeCourses = new List<employeeCourses>();
            lstEmployeeCourses = objEmployeeRepository.ShowCertificatesCourses("");
            var data = new JsonResult { Data = lstEmployeeCourses, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }
        public JsonResult ShowEmployeecourse(employeeCourses EC)
        {
            List<employeeCourses> lstEmployeeCourses = new List<employeeCourses>();
            lstEmployeeCourses = objEmployeeRepository.ShowCertificatesCourses(EC.recordid);
            var data = new JsonResult { Data = lstEmployeeCourses, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }
        public int createEmployeeCourse(employeeCourses courses)
        {
            int saveResult = objEmployeeRepository.SaveCertificateCourse(courses);
            return saveResult;
            //return false;
        }
        public int updateEmployeeCourse(employeeCourses courses)
        {
            int saveResult = objEmployeeRepository.UpdateCertificateCourse(courses);
            return saveResult;
        }
        public bool DeleteEmployeeCourse(string employeeid, string coursename, string recordid)
        {
            bool saveResult = objEmployeeRepository.DeleteCertificateCourse(employeeid, coursename, recordid);
            return saveResult;
        }


        #endregion

        /// <summary>
        /// saikishore
        /// </summary>
        /// <returns></returns>
        #region Previous Experience

        public ActionResult CreatePreviousExperience()
        {
            return View();
        }
        public JsonResult CreatePreviousExperience1(PreviousExperience PreviousExperience)
        {
            List<PreviousExperience> lstdropdown = new List<PreviousExperience>();


            lstdropdown = objEmployeeRepository.showPrevExpDropdown();
            List<PreviousExperience> lstPreviousExperience = new List<PreviousExperience>();

            lstPreviousExperience = objEmployeeRepository.ShowPreviousExpDetails(PreviousExperience);



            var getdata = new { griddetails = lstPreviousExperience, dropdown = lstdropdown };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;


        }
        public ActionResult DeletePreviousExperience(string recordid)
        {
            PreviousExperience objpreviousexp = new Core.PreviousExperience();
            objpreviousexp.recordid = Convert.ToString(recordid);
            bool status = objEmployeeRepository.DeletePreviousExperience(objpreviousexp);
            List<PreviousExperience> lstPreviousExperience = new List<PreviousExperience>();

            lstPreviousExperience = objEmployeeRepository.ShowPreviousExpDetails(objpreviousexp);

            var getdata = new { griddetails = lstPreviousExperience, msgstatus = status };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;

        }
        public JsonResult GetEmployeeddl()
        {
            List<PreviousExperience> lstdropdown = new List<PreviousExperience>();


            lstdropdown = objEmployeeRepository.showPrevExpDropdown();




            var getdata = new { griddetails = lstdropdown };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        public JsonResult ShowReasonforLeaving()
        {
            List<PreviousExperience> lstReasonforLeaving = new List<PreviousExperience>();
            lstReasonforLeaving = objEmployeeRepository.ShowReasonforLeaving();
            var getdata = new { griddetails = lstReasonforLeaving };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        public ActionResult CreatePreviousExperienceDetails(PreviousExperience PreviousExperience)
        {
            int status = objEmployeeRepository.CreatePreviousExperience(PreviousExperience);
            List<PreviousExperience> lstPreviousExperience = new List<PreviousExperience>();

            lstPreviousExperience = objEmployeeRepository.ShowPreviousExpDetails(PreviousExperience);

            var getdata = new { griddetails = lstPreviousExperience, msgstatus = status };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;


        }
        public ActionResult UpdatePreviousExperience(PreviousExperience PreviousExperience)
        {

            int status = objEmployeeRepository.UpdatePreviousExperience(PreviousExperience);
            List<PreviousExperience> lstPreviousExperience = new List<PreviousExperience>();

            lstPreviousExperience = objEmployeeRepository.ShowPreviousExpDetails(PreviousExperience);

            var getdata = new { griddetails = lstPreviousExperience, msgstatus = status };
            var data = new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;

        }

        #endregion

        /// <summary>
        /// Raji Reddy
        /// </summary>
        /// <returns></returns>
        #region Employeement Deatails

        public ActionResult CreateEmployementDetails()
        {
            return View();
        }

        public JsonResult ShowEmployee()
        {
            List<EmploymentDetails> lstEmploymee = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstEmploymee = objEmployeeRepository.ShowEmployee();

                }

                var data = new JsonResult { Data = lstEmploymee, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowTypeofEmployment()
        {
            List<EmploymentDetails> lstTypeofEmployment = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstTypeofEmployment = objEmployeeRepository.ShowTypeOfEmployment();

                }

                var data = new JsonResult { Data = lstTypeofEmployment, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowDesignations()
        {
            List<EmploymentDetails> lstDesignations = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstDesignations = objEmployeeRepository.ShowDesignations();
                }

                var data = new JsonResult { Data = lstDesignations, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowReportingManager(string strDepartment)
        {
            List<EmploymentDetails> lstReportingManager = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstReportingManager = objEmployeeRepository.ShowReportingManager(strDepartment);
                }

                var data = new JsonResult { Data = lstReportingManager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowDepartments()
        {
            List<EmploymentDetails> lstDepartments = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstDepartments = objEmployeeRepository.ShowDepartments();
                }

                var data = new JsonResult { Data = lstDepartments, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowSalaryType()
        {
            List<EmploymentDetails> lstSalaryTypes = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstSalaryTypes = objEmployeeRepository.ShowSalaryType();
                }

                var data = new JsonResult { Data = lstSalaryTypes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowPayScale()
        {
            List<EmploymentDetails> lstPayScale = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstPayScale = objEmployeeRepository.ShowPayScale();
                }

                var data = new JsonResult { Data = lstPayScale, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowPayScaleDetails(string strScaleid)
        {
            List<EmploymentDetails> lstScale = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstScale = objEmployeeRepository.ShowPayScaleDetails(strScaleid);
                }

                var data = new JsonResult { Data = lstScale, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowSalaryStructure()
        {
            List<EmploymentDetails> lstSalaryStructure = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstSalaryStructure = objEmployeeRepository.ShowSalaryStructure();
                }

                var data = new JsonResult { Data = lstSalaryStructure, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ShowLeaveStructure()
        {
            List<EmploymentDetails> lstLeaveStructure = new List<EmploymentDetails>();
            try
            {
                if (ModelState.IsValid)
                {
                    lstLeaveStructure = objEmployeeRepository.ShowLeaveStructure();
                }

                var data = new JsonResult { Data = lstLeaveStructure, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public JsonResult CreateEmployment(EmploymentDetails emp)
        //{
        //    List<EmploymentDetails> lstemp = new List<EmploymentDetails>();
        //    bool isValid = false;
        //    try
        //    {
        //        isValid = objEmployeeRepository.CreateEmployment(emp);
        //        var result = new { Success = isValid, data = lstemp };
        //        var data = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public JsonResult ShowSalaryStructureDetails(string strSStructure)
        {
            List<EmploymentDetails> lstSStructure = new List<EmploymentDetails>();
            if (ModelState.IsValid)
            {
                lstSStructure = objEmployeeRepository.ShowSalaryStructureDetails(strSStructure);
            }

            var data = new JsonResult { Data = lstSStructure, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        public JsonResult ShowLeaveStructureDetails(string strLStructure)
        {
            List<EmploymentDetails> lstLStructure = new List<EmploymentDetails>();
            if (ModelState.IsValid)
            {
                lstLStructure = objEmployeeRepository.ShowLeaveStructureDetails(strLStructure);
            }

            var data = new JsonResult { Data = lstLStructure, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        #endregion

        /// <summary>
        /// Santosh
        /// </summary>
        /// <returns></returns>
        #region Kapil Career
        public ActionResult createKapilCareer()
        {
            return View();
        }

        [HttpPost]
        public int createKapilCareer(KapilCareerModel kpm)
        {


            return objEmployeeRepository.createKapilCareer(kpm);
        }


        public int updateKapilCareer(KapilCareerModel kpm)
        {


            return objEmployeeRepository.updateKapilCareer(kpm);
        }
        public bool DeleteKapilCareer(int recordid)
        {


            return objEmployeeRepository.DeleteKapilCareer(recordid);

        }

        public int updateExistedKapilCareer(KapilCareerModel kpm)
        {


            return objEmployeeRepository.updateExistedKapilCareer(kpm);
        }
        public JsonResult ShowKapilCareerGrid(string ID)
        {
            List<KapilCareerModel> lstls = objEmployeeRepository.ShowKapilCareerGrid(ID);
            return new JsonResult { Data = lstls, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult getSelectedRowDetails(int recordid)
        {

            List<KapilCareerModel> lstls = objEmployeeRepository.getSelectedRowDetails(recordid);
            return new JsonResult { Data = lstls, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ShowDesignationReason()
        {

            List<KapilCareerModel> lstDesignations = new List<KapilCareerModel>();
            List<KapilCareerModel> lstReasons = new List<KapilCareerModel>();


            lstDesignations = objEmployeeRepository.GetDesignations();
            lstReasons = objEmployeeRepository.GetReasons();

            var getdata = new { GetDesignations = lstDesignations, GetReasons = lstReasons };

            return new JsonResult { Data = getdata, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; ;


        }

        #endregion

        /// <summary>
        /// Developed By: SRIKANTH.K
        /// </summary>
        /// <returns></returns>
        /// #region Deduction...
        #region deductions
        public ActionResult CreateDeduction()
        {
            return View();
        }


        public JsonResult ShowDeductions(DeductionDTO grp)
        {
            List<DeductionDTO> all = objEmployeeRepository.ShowDeductions(grp);
            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //public JsonResult ShowDeductions12(string vchemployeeid)
        //{
        //    List<DeductionDTO> all = objEmployeeRepository.ShowDeductions(vchemployeeid);
        //    return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        [HttpPost]
        public JsonResult CreateDeduction(DeductionDTO grp, string Status)
        {

            try
            {
                //if (ModelState.IsValid)
                //{
                // objEmployeeRepository objemployeeMasters = new objEmployeeRepository();
                bool res = objEmployeeRepository.CreateDeduction(grp);
                // return Json(new { success = true });

                //}
                //else
                //{
                //    return Json(new { success = false });
                //}


                List<DeductionDTO> all = objEmployeeRepository.ShowDeductions(grp);
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        public JsonResult UpdateDeduction(DeductionDTO grp)
        {

            try
            {

                bool res = objEmployeeRepository.UpdateDeduction(grp);

                List<DeductionDTO> all = objEmployeeRepository.ShowDeductions(grp);
                var result = new { Success = res, data = all };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult DeleteDeduction(GroupDTO grp)
        {
            var c = objEmployeeRepository.DeleteDeduction(grp.RecordId);
            return new JsonResult { Data = c, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        /// <summary>
        /// Sateesh Dasari 09-12-2015
        /// </summary>
        /// <returns></returns>
        #region CreateOrUpdateEmployee...

        public ActionResult GetAllEmployeeBindDetails()
        {
            EmployeeMastersController obj = new EmployeeMastersController();
            var EmpID = GenerateEMPId();
            var GridData = GetEmpGridDetails();
            var varMaritalStatus = BindMaritalStatus();
            var BloodGroup = BindBloodGroup();
            var RelationShip = BindRelationShip();
            var FamilyDetails = getFamilyDetails();
            var ShowCountry = obj.ShowCountry();
            var listcourse = Getlistcourse();
            var listgroups = Getlistgroups();
            var EmployeeCourses = ShowEmployeeCourses();
            var TypeofEmployment = ShowTypeofEmployment();
            var Designations = ShowDesignations();
            var Departments = ShowDepartments();
            var PayScale = ShowPayScale();
            var SalaryType = ShowSalaryType();
            var SalaryStructure = ShowSalaryStructure();
            var LeaveStructure = ShowLeaveStructure();
            return new JsonResult
            {
                Data = new
                {
                    EmpID,
                    GridData,
                    varMaritalStatus,
                    BloodGroup,
                    RelationShip,
                    FamilyDetails,
                    ShowCountry,
                    listcourse,
                    listgroups,
                    EmployeeCourses,
                    TypeofEmployment,
                    Designations,
                    Departments,
                    PayScale,
                    SalaryType,
                    SalaryStructure,
                    LeaveStructure
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult EditEmployee()
        {
            return View();
        }

        public bool SaveEmpDetails(empAll objAll)                //Save OR Update Single Employee Details
        {
            return objEmployeeRepository.SaveEmployeedetails(objAll);

        }

        public ActionResult GetEmpGridDetails()                  //work with get grid details
        {
            List<EmployeeDetails> lstAllDetails = new List<EmployeeDetails>();
            lstAllDetails = objEmployeeRepository.GetEmpGridDetails();
            return Json(lstAllDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeDetails(string EmpID)     // to fetch the single employee data
        {
            empAll objempDetails = new empAll();
            objempDetails = objEmployeeRepository.GetEmployeeDetails(EmpID);
            return Json(objempDetails, JsonRequestBehavior.AllowGet);
        }

        public bool UpdateEmployee(empAll objAll)
        {
            return objEmployeeRepository.UpdateEmployee(objAll);
        }

        public bool DeleteEmployee(string EmpID)
        {
            return objEmployeeRepository.DeleteEmployee(EmpID);
        }

        #endregion

        public JsonResult getGroups(string strCourse)
        {
            try
            {
                List<GroupDTO> lstGroup = new List<GroupDTO>();
                if (ModelState.IsValid)
                {
                    lstGroup = objEmployeeRepository.getGroups(strCourse);
                }

                var data = new JsonResult { Data = lstGroup, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return data;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "States");
                throw;
            }
        }

        #region Employee Details

        public ActionResult EmployeeDetails()
        {
            return View();
        }

        public bool CreateEmpDetails(EmpDetails objEmpDetails)
        {
            try
            {
                return objEmployeeRepository.SaveEmpDetails(objEmpDetails);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }
        }

        public JsonResult GetEmpDetails()
        {
            try
            {
                List<EmpDetails> lstEmp = new List<EmpDetails>();
                lstEmp = objEmployeeRepository.GetEmpDetails();
                return new JsonResult { Data = lstEmp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }
        }


        public bool UpdateEmpDetails(EmpDetails objEmp)
        {
            try
            {
                return objEmployeeRepository.UpdateEmpDetails(objEmp);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }
        }

        public JsonResult ShowWorkStations()
        {
            try
            {
                List<EmpDetails> lstWorkstation = new List<EmpDetails>();
                lstWorkstation = objEmployeeRepository.GetWorkStations();
                return new JsonResult { Data = lstWorkstation, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }


        }

        public bool DeleteEmpDetails(EmpDetails objEmp)
        {
            try
            {
                return objEmployeeRepository.DeleteEmpDetails(objEmp);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
        }


        public bool CreateWorkStation(EmpDetails objEmpDetails)
        {
            try
            {
                return objEmployeeRepository.SaveWorkStation(objEmpDetails);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }
        }

        #endregion


        #region Assign Shift

        public ActionResult AssignShift()
        {
            return View();
        }

        public JsonResult ShowShift()
        {
            try
            {
                List<AssignShift> lstShift = new List<AssignShift>();
                lstShift = objEmployeeRepository.GetShift();
                return new JsonResult { Data = lstShift, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
                throw;
            }


        }

        public JsonResult ShowShiftTimings(string strShiftId)
        {
            try
            {
                List<AssignShift> lstShiftTimings = new List<AssignShift>();
                lstShiftTimings = objEmployeeRepository.GetShiftTimings(strShiftId);
                return new JsonResult { Data = lstShiftTimings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
                throw;
            }
        }

        public JsonResult GetEmployees(string strShiftId, string strFromDate, string strToDate)
        {
            try
            {
                List<AssignShift> lstEmp = new List<AssignShift>();
                if (strShiftId != null && strShiftId != "" && strFromDate != null && strFromDate != "" && strToDate != null && strToDate != "")
                {
                    lstEmp = objEmployeeRepository.GetEmployees(strShiftId, strFromDate, strToDate);
                }
                return new JsonResult { Data = lstEmp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
                throw;
            }
        }

        public bool CreateAssignShift(AssignShift AssignShiftDTO, List<AssignShift> lstAssignShift)
        {
            try
            {
                return objEmployeeRepository.SaveAssignShift(AssignShiftDTO, lstAssignShift);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
                throw;
            }
        }

        #endregion

        #region Edit Shift

        public ActionResult EditShift()
        {
            return View();
        }


        public JsonResult GetEmpInformation(string datDate, string strShiftId)
        {
            try
            {
                List<AssignShift> lstEmp = new List<AssignShift>();
                if (datDate != null && datDate != "" && strShiftId != null && strShiftId != "")
                {
                    lstEmp = objEmployeeRepository.GetEmpInformation(datDate, strShiftId);
                }
                return new JsonResult { Data = lstEmp, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EditShift");
                throw;
            }
        }

        public JsonResult ShowEmps(string datDate, string strShiftId)
        {
            try
            {
                List<AssignShift> lstEmployees = new List<AssignShift>();
                if (datDate != null && datDate != "" && strShiftId != null && strShiftId != "")
                {
                    lstEmployees = objEmployeeRepository.ShowEmployees(datDate, strShiftId);
                }
                return new JsonResult { Data = lstEmployees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EditShift");
                throw;
            }

        }


        public bool CreateEditShift(AssignShift EditShiftDTO, List<AssignShift> lstEditShift)
        {
            try
            {
                return objEmployeeRepository.SaveEditShift(EditShiftDTO, lstEditShift);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EditShift");
                throw;
            }
        }

        #endregion

    }
}
