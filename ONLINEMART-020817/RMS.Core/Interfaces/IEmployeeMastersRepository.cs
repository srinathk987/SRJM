using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RMS.Core.Interfaces
{
    public interface IEmployeeMastersRepository
    {

        //#region Modules...

        //List<ModuleNames> GetParentModuleNames(int userid);
        //List<ChildModules> GetChildFunctionNames(int userid);

        //#endregion

        /// <summary>
        /// Vikram
        /// </summary>
        #region Marital Status


        bool SaveTypeOfMaritaStatus(MaritalStatus objStatus);

        bool UpdateMaritalStatus(MaritalStatus objStatus);

        List<MaritalStatus> GetTypeOfMaritalStatus();

        //bool DeleteTypeOfMaritalStatus(string p);
        bool DeleteTypeOfMaritalStatus(int p);
        //List<MaritalStatus> GetTypeOfMaritalStatus();
        bool CheckMaritalStatus(MaritalStatus objStatus);


        #endregion

        /// <summary>
        /// Not Assigned
        /// </summary>
        /// <returns></returns>
        #region Blood Group
        List<BloodGroupDTO> BindBloodGroup();
        bool CreateBloodGroup(BloodGroupDTO Blood);
        bool UpdateBloodGroup(BloodGroupDTO Blood);
        bool DeleteBloodGroup(int p);
        bool CheckBloodGroup(BloodGroupDTO blood);

        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        #region RELATIONSHIP...
        List<RelationDTO> BindRelations();
        bool Save(RelationDTO C);
        bool DeleteRecord(int id);
        bool UpdateRelations(RelationDTO C);
        bool CheckRelations(RelationDTO C);
        #endregion


        /// <summary>
        /// Rajesh
        /// </summary>
        /// <returns></returns>
        #region Country..

        bool SaveCountry(CountryDTO objCountry);
        List<CountryDTO> ShowCountryDetails();
        bool UpdateCountry(CountryDTO objCountry);
        bool DeleteCountry(int countryid);

        bool CheckCountry(CountryDTO objCountry);

        #endregion

        /// <summary>
        /// Rajesh
        /// </summary>
        /// <returns></returns>
        #region State
        bool SaveState(StateDTO objState);
        bool UpdateState(StateDTO objState);
        bool DeleteState(int stateid, string countryid);
        bool CheckState(StateDTO objState);

        #endregion

        /// <summary>
        /// RajiReddy
        /// </summary>
        /// <returns></returns>
        #region City Details

        bool CreateCity(City strCity);
        List<City> ShowCountry();
        List<City> ShowStates(string strcountryid);
        List<City> BindCityDetails();
        bool UpdateCity(City objcity);
        bool DeleteCity(City citydto);

        bool CheckCity(City objcity);

        #endregion

        /// <summary>
        /// Madhu
        /// </summary>
        #region Department

        List<DepartmentMaster> BindCompanyNames();
        List<DepartmentMaster> ShowDepartmentDetails();
        bool DeleteDepartment(int Department);
        bool SaveDepartment(DepartmentMaster Dept);
        bool UpdateDepartment(DepartmentMaster Dept);
        bool CheckDepartment(DepartmentMaster Dept);

        #endregion

        /// <summary>
        /// K.Nagendar
        /// </summary>
        #region Designation

        bool SaveDesignation(string Json);

        DataTable ShowDesignation();

        bool DeleteDesignation(int id);

        bool UpdateDesignation(int id, string jSON);

        bool CheckDesignation(string Json);

        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        /// <returns></returns>
        #region COURSE...
        List<CourseDTO> ShowCourse();
        bool CreateCourse(CourseDTO C);
        bool DeleteCourse(int id);
        bool UpdateCourse(CourseDTO C);
        bool CheckCourse(CourseDTO C);
        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        /// <returns></returns>
        #region GROUP...
        List<GroupDTO> ShowGroup();
        bool CreateGroup(GroupDTO C);
        bool DeleteGroup(int id);
        bool UpdateGroup(GroupDTO C);

        bool CheckGroup(GroupDTO C);

        #endregion

        /// <summary>
        /// Srinivas
        /// </summary>
        /// <returns></returns>
        #region Occupation
        List<OccupationMaster> ShowOccupationDetails();
        bool DeleteOccupation(int Occupation);
        bool SaveOccupation(OccupationMaster Occuptn);
        bool UpdateOccupation(OccupationMaster Occuptn);
        bool CheckOccupation(OccupationMaster Occupation);

        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region Reason For Leaving
        List<ReasonForLeavingDTO> BindReasonForLeaving();
        bool Save(ReasonForLeavingDTO C);
        bool DeleteReasonForLeavingRecord(int id);
        bool UpdateReasonForLeavingRecord(ReasonForLeavingDTO C);
        bool CheckReasonForLeaving(ReasonForLeavingDTO C);

        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region Reason For Transfering
        List<ReasonForTransferingDTO> BindReasonForTransfering();
        bool Save(ReasonForTransferingDTO C);
        bool DeleteReasonForTransferingRecord(int id);
        bool UpdateReasonForTransferingRecord(ReasonForTransferingDTO C);
        bool CheckReasonForTransfering(ReasonForTransferingDTO C);
        #endregion

        /// <summary>
        /// Satish.D
        /// </summary>
        #region Type of Employment...

        bool SaveTypeOfEmployee(EmployeeDTO objEmp);
        List<EmployeeDTO> GetTypeOfEmployement();
        bool DeleteTypeOfEmployement(int recordid);
        bool UpdateEmployement(EmployeeDTO objEmp);
        bool CheckTypeOfEmployement(EmployeeDTO objEmp);
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


        #region Shift Master

        bool CreateShift(Shift objshift);

        List<Shift> ShowShiftDetails();

        bool UpdateShift(Shift objShift);

        bool DeleteShift(Shift objShift);

        int GetShiftExist(int From, int To, string strShiftName);

        bool CheckShift(Shift objShift);


        #endregion




        
    }
}
