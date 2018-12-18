using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMS.Core
{
    public class Employee
    {

    }

    /// <summary>
    /// Venkatesh
    /// </summary>
    /// <returns></returns>
    #region Personal Information

    public class PersonalInformation
    {
        public string RecordId { get; set; }
        public string EmployeeId { get; set; }
        public string BioMetricId { get; set; }
        public string RFID { get; set; }
        public string Name { get; set; }
        public string Sname { get; set; }
        public string DOB { get; set; }
        public string Age { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string DateOfMarage { get; set; }
        public string BloodGroup { get; set; }
        public string MobileNumber { get; set; }
        public string Res { get; set; }
        public string PresentStatusOfEMP { get; set; }
        public string Email { get; set; }
        public string UploadUrl { get; set; }

    }

    #endregion

    /// <summary>
    /// Madhu
    /// </summary>
    /// <returns></returns>
    #region Employee Identification and Other Details

    public class EmployeeIdentification
    {
        public string PfStatus { get; set; }
        public string Pfno { get; set; }
        public string PfEffectiveDate { get; set; }
        public string UanNo { get; set; }
        public string EsiStatus { get; set; }
        public string EsiNo { get; set; }
        public string AadharNo { get; set; }
        public string Employeeid { get; set; }
        public long EmployeeNo { get; set; }
        public string PancardNo { get; set; }
        public string DrivingLicenceNo { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public string VehicleInsuranceNo { get; set; }
        public string InsuranceDate { get; set; }
        public string PassportNo { get; set; }
        public List<BankDetails> BankDetails { get; set; }
        public int Createdby { get; set; }

    }

    public class BankDetails
    {
        public string Bankname { get; set; }
        public string Branchname { get; set; }
        public string BankAccountNo { get; set; }
        public int Bankrecordid { get; set; }
        public string BankStatus { get; set; }
    }


    #endregion

    /// <summary>
    /// Shubha
    /// </summary>
    /// <returns></returns>
    #region Employee Allowances Check

    public class EmpAllowanceCheck
    {
        public long recordid { get; set; }
        public string employeeid { get; set; }
        public string vchemployeeid { get; set; }
        public string vchallowancename { get; set; }
        public decimal numamount { get; set; }
        public bool state { get; set; }
        public int statusid { get; set; }
        public int createdby { get; set; }
        public string createddate { get; set; }
        public int modifiedby { get; set; }
        public string modifieddate { get; set; }

        public string vchfullname { get; set; }
    }

    #endregion


    /// <summary>
    /// Venkatesh
    /// </summary>
    #region Contact Details

    public class EmergencyContactDetails
    {
        public string ContactPersonName { get; set; }
        public string RelationShip { get; set; }
        public string ContactNumber { get; set; }
        public string EMPId { get; set; }

    }

    #endregion

    /// <summary>
    /// Naveen Krishna.G
    /// </summary>

    #region Employee FamilyDetails
    public class EmployeeFamilydetails
    {
        public string recordid { get; set; }
        public string drecordid { get; set; }
        public string employeeid { get; set; }
        public string vchemployeeid { get; set; }
        public string vchpersonname { get; set; }
        public string vchrelationship { get; set; }
        public string vchisnominee { get; set; }
        public string vchoccupation { get; set; }
        public string vchmobilenumber { get; set; }
        public string datdob { get; set; }
        public int statusid { get; set; }
        public int createdby { get; set; }
        public int modifiedby { get; set; }
        public string vcheducationlevel { get; set; }

        public string vchname { get; set; }

        public string RecordStatus { get; set; }
    }
    public class occupationdetails
    {
        public int recordid { get; set; }
        public string vchoccupation { get; set; }
        public string vchdescription { get; set; }

    }
    public class RelationDetails
    {
        public int recordid { get; set; }
        public string vchrelationship { get; set; }
        public int vchdescription { get; set; }

    }
    public class FamilyEducationDetails
    {
        public int recordid { get; set; }
        public int employeeid { get; set; }
        public int employeefamilyid { get; set; }
        public string vchemployeeid { get; set; }
        public string vchpersonname { get; set; }
        public string vcheducationlevel { get; set; }
        public int statusid { get; set; }
        public int createdby { get; set; }
        public int modifiedby { get; set; }


        public string vchdescription { get; set; }
    }
    #endregion

    /// <summary>
    /// Vijay
    /// </summary>
    #region Address

    public class Addresses
    {
        public string DoorNo { get; set; }
        public string StreetName { get; set; }
        public string Area { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string CityName { get; set; }
        public string PinCode { get; set; }

        public string dDoorNo { get; set; }
        public string dStreetName { get; set; }
        public string dArea { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
        public string CityId { get; set; }
        public string dPinCode { get; set; }

        public string dCountry { get; set; }
        public string dState { get; set; }
        public string dCityName { get; set; }

        public string vchemployeeid { get; set; }
        public string vchname { get; set; }

        public string AddressType { get; set; }
        public string dAddressType { get; set; }

        public string dCountryId { get; set; }

        public string dStateId { get; set; }
        public bool SameOrNot { get; set; }
    }


    #endregion

    /// <summary>
    /// Srinath
    /// </summary>
    /// <returns></returns>
    #region EducationDetails Srinath
    /// <summary>
    /// Course 
    /// </summary>
    public class Course
    {
        public string recordid { set; get; }


        public string GetCousrse { set; get; }
    }
    /// <summary>
    /// Group
    /// </summary>

    public class Group
    {
        public string Recordid { set; get; }

        public string CourseName { set; get; }

        public string GroupName { set; get; }

    }

    /// <summary>
    /// Education Details
    /// </summary>
    public class EducationDetails
    {
        public string recordid { set; get; }

        public string EmployeeRecordid { set; get; }

        public string GetCousrse { set; get; }

        public string Group { set; get; }

        public string Place { set; get; }

        public double Year { set; get; }
        public string EmployeeName { set; get; }

        public double MarksPercentage { set; get; }

        public string Course { set; get; }

        public string Employeeid { set; get; }

        public string SchoolorCollege { set; get; }
    }




    #endregion

    /// <summary>
    /// Santosh
    /// </summary> 
    /// <returns></returns>
    #region Training and Certification

    public class employeeCourses
    {
        public string vchemployeeid { get; set; }
        public string vchcertificateorcourse { get; set; }
        public string datfromdate { get; set; }
        public string dattodate { get; set; }



        public string CreatedBy { get; set; }

        public string recordid { get; set; }

        public string EmployeeID { get; set; }

        public string vchname { get; set; }

        public string vchcertificateorcourse1 { get; set; }
    }

    #endregion


    /// <summary>
    /// saikishore
    /// </summary>
    /// <returns></returns>
    /// 
    #region Previous Experience

    public class PreviousExperience
    {

        public string recordid { get; set; }
        public int id { get; set; }
        public string employeeid { get; set; }
        public string name { get; set; }
        public string organisation { get; set; }
        public string designation { get; set; }
        public string frmdate { get; set; }
        public string todate { get; set; }
        public string place { get; set; }
        public string lastpay { get; set; }
        public string reasonsforLeaving { get; set; }
        public string reasonsforLeavingid { get; set; }
        public string vchemployeeid { get; set; }
        public string vchfullname { get; set; }

    }

    #endregion

    /// <summary>
    /// Raji Reddy
    /// </summary>
    /// <returns></returns>
    #region Employeement Deatails
    public class EmploymentDetails
    {
        public string TypeOfEmployment { get; set; }
        public int TypeofEmploymentId { get; set; }
        public int DesignationId { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int SalaryTypeId { get; set; }
        public string SalaryType { get; set; }
        public int PayScaleId { get; set; }
        public string PayScale { get; set; }
        public int SalaryStructureId { get; set; }
        public string SalaryStructure { get; set; }
        public int LeaveStructureId { get; set; }
        public string LeaveStructure { get; set; }
        public string BasicAmount { get; set; }
        public string VDA { get; set; }
        public string DateOfReporting { get; set; }
        public string JoinedAs { get; set; }
        public string KapilGroupJoinDate { get; set; }
        public string ReportingManager { get; set; }
        public string Employeeid { get; set; }
        public string Employee { get; set; }
        public string Payitem { get; set; }
        public string PayitemType { get; set; }
        public string Mode { get; set; }
        public string LeaveType { get; set; }
        public string DaysPerYear { get; set; }
        public string AccumulateDays { get; set; }
        public string EnchashmentDays { get; set; }
        public string DaysPerMonth { get; set; }


    }


    #endregion

    /// <summary>
    /// Satish D
    /// </summary>
    /// <returns></returns>
    public class EmployeeDetails
    {
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string Mobilenumber { get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }

    }


    /// <summary>
    /// Kapil Career
    /// </summary>
    /// <returns></returns>
    #region Kapil Career

    public class KapilCareerModel
    {

        public string CompanyName { get; set; }

        public string EmployeeID { get; set; }
        public string vchEmlpoyeeID { get; set; }
        public string Designation { get; set; }
        public string kc_fromdate { get; set; }
        public string kc_todate { get; set; }
        public string sscminutesno { get; set; }
        public string location { get; set; }
        public string ReasonTransfer { get; set; }

        public string ELClaimed { get; set; }
        public string EL_date { get; set; }


        public string KHCNo { get; set; }
        public string KHC_fromdate { get; set; }
        public string KHC_todate { get; set; }

        public string statusid { get; set; }
        public string createdby { get; set; }
        public string createddate { get; set; }
        public string modifiedby { get; set; }
        public string modifieddate { get; set; }


        public string vchemployeeid { get; set; }
        public string vchname { get; set; }

        public string RecordId { get; set; }
    }

    #endregion


    /// <summary>
    /// Developed By: SRIKANTH.K
    /// </summary>
    /// <returns></returns>
    public class DeductionDTO
    {
        public int RecordId { get; set; }
        public string Employeeid { get; set; }
        public string EmployeeName { get; set; }
        public decimal BasicAmount { get; set; }
        public string DeductionType { get; set; }
        public string RecoveryType { get; set; }
        public decimal Amount { get; set; }
        public decimal TenureMonths { get; set; }
        public decimal DeductionAmount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StatusID { get; set; }
        public string vchfullname { get; set; }
        public string vchemployeeid { get; set; }

    }

    /// <summary>
    /// Sateesh D (Create/Update Employee Masters)  09-12-2015.
    /// </summary>
    #region EmpDetails

    public class empAll
    {
        public PersonalInformation personalinformation { get; set; }

        public EmergencyContactDetails emergencycontactdetails { get; set; }

        public List<EmployeeFamilydetails> lstemployeefamilydetails { get; set; }

        public Addresses addresses { get; set; }

        public List<EducationDetails> lsteducationdetails { get; set; }

        public List<employeeCourses> lstemployeecourses { get; set; }

        public PreviousExperience previousexperience { get; set; }

        public EmploymentDetails employmentdetails { get; set; }

        public List<KapilCareerModel> lstkapilcareermodel { get; set; }

        public EmployeeIdentification EmployeeIdentification { get; set; }
        public List<BankDetails> BankDetails { get; set; }
    }

    #endregion

    #region Employee Details

    public class EmpDetails
    {
        public string EmployeeID { get; set; }

        public string Name { get; set; }

        public string Sname { get; set; }

        public string Email { get; set; }

        public string EmployeeStatus { get; set; }

        public string Gender { get; set; }

        public string Designation { get; set; }

        public string MobileNumber { get; set; }

        public string WorkType { get; set; }

        public string EmploymentType { get; set; }

        public string PreferredTimeofWork { get; set; }

        public string WorkStationId { get; set; }

        public string WorkStation { get; set; }

        public string DoorNo { get; set; }

        public string StreetName { get; set; }

        public string Area { get; set; }

        public string Country { get; set; }

        public string CountryId { get; set; }

        public string State { get; set; }

        public string StateId { get; set; }

        public string CityName { get; set; }

        public string PinCode { get; set; }


    }

    #endregion

    #region Assign Shift

    public class AssignShift
    {

        public string ShiftId { get; set; }

        public string Shiftname { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string ShiftTime { get; set; }

        public string Name { get; set; }

        public string Designation { get; set; }

        public string MobileNumber { get; set; }

        public string WorkType { get; set; }

        public string EmploymentType { get; set; }

        public string PreferredTimeofWork { get; set; }

        public string WorkStationId { get; set; }

        public string WorkStation { get; set; }

        public string BreakFromTime { get; set; }

        public string BreakToTime { get; set; }

        public string CheckSelect { get; set; }

        public string Employeeid { get; set; }

        public string vchEmployeeid { get; set; }

    }

    #endregion
}
