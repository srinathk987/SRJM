using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMS.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Employee Personal Information , Emergency Contact Details ..

        //string SaveBranchAllowance(PersonalInformation User);

        string BindEMPId();
        #endregion

        /// <summary>
        /// Madhu
        /// </summary>
        /// <returns></returns>
        #region Employee Identification and Other Details

        string SaveEmployeeIdentifications(EmployeeIdentification EmployeeIdentification, List<BankDetails> lstBankDetails);

        EmployeeIdentification ShowEmployeeIdentificationDetails(string Employeeid);

        bool DeleteBankDetails(int Bankrecordid);

        bool CheckAccountNoExist(EmployeeIdentification EmployeeIdentification, BankDetails BankDetails);

        #endregion


        /// <summary>
        /// Shubha
        /// </summary>
        /// <param name="lstEmpAllowCheck"></param>
        /// <returns></returns>
        #region EmployeeAllowanceCheck

        bool Save(EmpAllowanceCheck objEmpAllowCheck, List<EmpAllowanceCheck> lstEmpAllowCheck);
        List<EmpAllowanceCheck> ShowEmployeeName();
        List<EmpAllowanceCheck> ShowDeductionEmployeeName();
        List<EmpAllowanceCheck> ShowEmployeeAllowenceDetails(string vchemployeeid);
        #endregion

        /// <summary>
        /// Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Emergency Contact details
        //void SaveEmergencyContactDetails(EmergencyContactDetails User);
        #endregion

        /// <summary>
        /// Naveen
        /// </summary>
        /// <returns></returns>
        #region Employee Family Details
        List<occupationdetails> GetOccupation();
        List<EmployeeFamilydetails> getEmployeeid_family();
        List<RelationDetails> GetRelation();
        List<EmployeeFamilydetails> GetFamilyDetails();
        //bool CreateEmpFamilyDetails(EmployeeFamilydetails objEmployeeFamilydetails);
        List<EmployeeFamilydetails> GetChildrenDetails();
        bool UpdateFamilyDetails(EmployeeFamilydetails objFamilyDetails);
        bool DeleteFamilyDetails(string recordid, string drecordid);
        List<FamilyEducationDetails> GetEducationDetails();
        #endregion

        /// <summary>
        /// Vijay
        /// </summary>
        /// <returns></returns>
        #region Addresses

        List<City> ShowCities(string strSateid);
        //bool SaveEmployee(Addresses objAddresses);
        List<Addresses> LoadEmployeeNames();
        #endregion

        /// <summary>
        /// Srinath
        /// </summary>
        /// <returns></returns>
        #region EducationDetails Srinath

        /// <summary>
        /// Create Education Details
        /// </summary>
        /// <param name="EducationDetails"></param>
        /// <returns></returns>
        //int CreateEducationDetails(EducationDetails EducationDetails);

        /// <summary>
        /// Binding EducationDetails To Grid
        /// </summary>
        /// <returns></returns>

        List<EducationDetails> ShowEducationDetails();

        /// <summary>
        /// Getting Groups Based On Course
        /// </summary>
        /// <param name="CourseName"></param>
        /// <returns></returns>

        List<Group> GetGroups(string CourseName);

        /// <summary>
        /// Getting All courses from Database
        /// </summary>
        /// <returns></returns>
        List<Course> GetcourseDetails();

        /// <summary>
        /// Binding EmployeeId tO Employeeid Dropdownlist
        /// </summary>
        /// <returns></returns>
        List<EducationDetails> GetEmployeeiddetails();
        /// <summary>
        /// Getting All Groups
        /// </summary>
        /// <returns></returns>
        List<Group> GetGroupDetails();

        /// <summary>
        /// Deleting Education Details Based On EmployeeId
        /// </summary>
        /// <param name="Employeeid"></param>
        /// <returns></returns>

        int DeleteEducationDetailsResult(string Employeeid);

        /// <summary>
        /// Updating Education Details Based On EmployeeId
        /// </summary>
        /// <param name="EducationDetails"></param>
        /// <returns></returns>

        int UpdateEducationDetails(EducationDetails EducationDetails);





        #endregion

        /// <summary>
        /// Santosh
        /// </summary>
        /// <returns></returns>
        #region Training and Certification

        List<employeeCourses> ShowCertificatesCourses(string ID);
        bool DeleteCertificateCourse(string employeeid, string coursename, string recordid);
        int SaveCertificateCourse(employeeCourses Dept);
        int UpdateCertificateCourse(employeeCourses Dept);

        #endregion

        /// <summary>
        /// saikishore
        /// </summary>
        /// <returns></returns>
        #region Previous Experience
        List<PreviousExperience> ShowPreviousExpDetails(PreviousExperience objPreviousExperience);
        List<PreviousExperience> showPrevExpDropdown();
        List<PreviousExperience> ShowReasonforLeaving();
        int CreatePreviousExperience(PreviousExperience objPreviousExperience);
        int UpdatePreviousExperience(PreviousExperience objPreviousExperience);
        bool DeletePreviousExperience(PreviousExperience objPreviousExperience);


        #endregion

        /// <summary>
        /// Raji Reddy
        /// </summary>
        /// <returns></returns>
        #region Employeement Deatails

        List<EmploymentDetails> ShowEmployee();
        List<EmploymentDetails> ShowTypeOfEmployment();
        List<EmploymentDetails> ShowDesignations();
        List<EmploymentDetails> ShowDepartments();
        List<EmploymentDetails> ShowReportingManager(string strDepartment);
        List<EmploymentDetails> ShowSalaryType();
        List<EmploymentDetails> ShowPayScale();
        List<EmploymentDetails> ShowPayScaleDetails(string strPayScaleid);
        List<EmploymentDetails> ShowSalaryStructure();
        List<EmploymentDetails> ShowLeaveStructure();
        //bool CreateEmployment(EmploymentDetails lstemp);
        List<EmploymentDetails> ShowSalaryStructureDetails(string strSStructure);
        List<EmploymentDetails> ShowLeaveStructureDetails(string strLStructure);


        #endregion

        /// <summary>
        /// Santosh
        /// </summary>
        /// <returns></returns>
        #region Kapil Career

        // bool createKapilCareer();
        int updateExistedKapilCareer(KapilCareerModel kpm);
        bool DeleteKapilCareer(int recordid);
        int updateKapilCareer(KapilCareerModel kpm);
        List<KapilCareerModel> ShowKapilCareerGrid(string id);
        List<KapilCareerModel> getSelectedRowDetails(int recordid);
        int createKapilCareer(KapilCareerModel kpm);
        List<KapilCareerModel> GetDesignations();
        List<KapilCareerModel> GetReasons();

        #endregion


        /// <summary>
        /// Developed By: SRIKANTH.K
        /// </summary>
        /// <returns></returns>
        #region Deductions...

        List<DeductionDTO> ShowDeductions(DeductionDTO C);
        bool CreateDeduction(DeductionDTO C);
        bool DeleteDeduction(int id);
        bool UpdateDeduction(DeductionDTO C);


        #endregion
        /// <summary>
        /// Satish.D
        /// </summary>
        /// <returns></returns>
        #region SaveOrUpdateEmployeeDetails...
        bool SaveEmployeedetails(empAll objAll);

        List<EmployeeDetails> GetEmpGridDetails();

        empAll GetEmployeeDetails(string EmpID);

        bool UpdateEmployee(empAll objAll);

        bool DeleteEmployee(string EmpID);
        #endregion

        List<GroupDTO> getGroups(string strCourse);

        #region Employee Details

        bool SaveEmpDetails(EmpDetails objEmpDetails);

        List<EmpDetails> GetEmpDetails();

        bool UpdateEmpDetails(EmpDetails objEmp);

        bool DeleteEmpDetails(EmpDetails objEmp);

        List<EmpDetails> GetWorkStations();

        bool SaveWorkStation(EmpDetails objEmpDetails);


        #endregion


        #region Assign Shift

        List<AssignShift> GetShift();

        List<AssignShift> GetShiftTimings(string strShiftId);

        List<AssignShift> GetEmployees(string strShiftId, string strFromDate, string strToDate);


        bool SaveAssignShift(AssignShift AssignShiftDTO, List<AssignShift> lstAssignShift);

        #endregion


        #region Edit Shift

        List<AssignShift> GetEmpInformation(string datDate, string strShiftId);

        List<AssignShift> ShowEmployees(string datDate, string strShiftId);

        bool SaveEditShift(AssignShift EditShiftDTO, List<AssignShift> lstEditShift);

        #endregion





    }
}

