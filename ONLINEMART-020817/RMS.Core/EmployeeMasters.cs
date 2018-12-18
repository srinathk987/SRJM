using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMS.Core
{
    public class EmployeeMasters
    {

    }

    /// <summary>
    /// Vikram
    /// </summary>
    #region State

    public class State
    {
        //recordid,vchmaritalstatus,vchdescription,statusid,createdby,createddate,modifiedby,modifieddate

        public string state { get; set; }
        public string Country { get; set; }
        public string CountryId { get; set; }

    }

    #endregion

    /// <summary>
    /// Vikram
    /// </summary>
    #region MartialStatus

    public class MaritalStatus
    {
        //recordid,vchmaritalstatus,vchdescription,statusid,createdby,createddate,modifiedby,modifieddate
        public int recordid { get; set; }
        public string vchmaritalstatus { get; set; }
        public string vchdescription { get; set; }
        public int statusid { get; set; }
        //public string createdby { get; set; }
        //public string createddate { get; set; }
        //public string modifiedby { get; set; }
        //public string modifieddate { get; set; }
    }

    #endregion



    /// <summary>
    /// T.Venkatesh
    /// </summary>
    /// <returns></returns>
    #region Blood Group

    public class BloodGroupDTO
    {
        public int RecordId { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public string StatusID { get; set; }
    }

    #endregion


    /// <summary>
    /// SriKanth
    /// </summary>
    #region RelationShip

    public class RelationDTO
    {
        public int SNo { get; set; }
        public int RecordId { get; set; }
        public string Relation { get; set; }
        public string Description { get; set; }
        public string StatusID { get; set; }
    }

    #endregion

    ///// <summary>
    ///// Rajesh
    ///// </summary>
    ///// <returns></returns>
    //#region Country

    //public class CountryDTO
    //{
    //    public int CountryId { get; set; }
    //    public string CountryName { get; set; }
    //    public string Description { get; set; }

    //}

    //#endregion

    ///// <summary>
    ///// Rajesh
    ///// </summary>
    ///// <returns></returns>
    //#region State

    //public class StateDTO
    //{

    //    public int StateId { get; set; }
    //    public string state { get; set; }
    //    public string CountryId { get; set; }
    //    public string Country { get; set; }



    //}

    //#endregion

    ///// <summary>
    ///// RajiReddy
    ///// </summary>
    ///// <returns></returns>
    //#region City

    //public class City
    //{
    //    public string CityName { get; set; }
    //    public string CityId { get; set; }
    //    public string Country { get; set; }
    //    public string State { get; set; }
    //    public string Description { get; set; }
    //    public string CountryId { get; set; }
    //    public string StateId { get; set; }
    //    public string StatusId { get; set; }
    //    public long Createdby { get; set; }

    //}

    //#endregion

    /// <summary>
    /// Madhu
    /// </summary>
    #region Department

    public class DepartmentMaster
    {
        public int RecordId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public int Deptid { get; set; }

    }

    #endregion


    /// <summary>
    /// K.Nagendar
    /// </summary>
    #region Designation

    #endregion

    /// <summary>
    /// SriKanth
    /// </summary>
    /// <returns></returns>
    #region Course

    public class CourseDTO
    {
        public int RecordId { get; set; }
        public string Course { get; set; }
        public string Description { get; set; }
        public string StatusID { get; set; }
    }

    #endregion

    /// <summary>
    /// SriKanth
    /// </summary>
    /// <returns></returns>
    #region Group

    public class GroupDTO
    {
        public int RecordId { get; set; }
        public string CourseName { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string StatusID { get; set; }
    }

    #endregion


    /// <summary>
    /// Srinivas
    /// </summary>
    /// <returns></returns>
    #region Occupation

    public class OccupationMaster
    {
        public int RecordId { get; set; }
        public string Occupationname { get; set; }
        public string Description { get; set; }


    }

    #endregion

    /// <summary>
    /// Shubha
    /// </summary>
    /// <returns></returns>
    #region Reason For Leaving
    public class ReasonForLeavingDTO
    {
        public int RecordId { get; set; }
        public string vchreasonsfoeleaving { get; set; }
        public string vchdescription { get; set; }
        public int StatusID { get; set; }
        public int createdby { get; set; }
        public string createddate { get; set; }
        public int modifiedby { get; set; }
        public string modifieddate { get; set; }
    }


    #endregion

    /// <summary>
    /// Shubha
    /// </summary>
    /// <returns></returns>
    #region Reasong For Transferring

    public class ReasonForTransferingDTO
    {
        public int RecordId { get; set; }
        public string vchreasonsfoetransfer { get; set; }
        public string vchdescription { get; set; }
        public int StatusID { get; set; }
        public int createdby { get; set; }
        public string createddate { get; set; }
        public int modifiedby { get; set; }
        public string modifieddate { get; set; }
    }

    #endregion

    /// <summary>
    /// Satish.D
    /// </summary>
    #region Type of Employement

    public class EmployeeDTO
    {
        public string TypeOFEmployement { get; set; }
        public string EmployementDescription { get; set; }

        public int recordid { get; set; }
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


    #region Shift Master

    public class Shift
    {

        public string ShiftId { get; set; }

        public string Shiftname { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }




    }


    #endregion












}
