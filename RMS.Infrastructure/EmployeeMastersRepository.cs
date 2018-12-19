using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMS.Core;
using RMS.Core.Interfaces;
using Npgsql;
using HelperManager;
using System.Data;
using System.Web.Script.Serialization;
using HRMS.Infrastructure;

namespace RMS.Infrastructure
{
    public class EmployeeMastersRepository : IEmployeeMastersRepository
    {

        /// <summary>
        /// rajesh
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        #region Module...

        //public List<ModuleNames> GetParentModuleNames(int userid)
        //{
        //    List<ModuleNames> objModuleValues = new List<ModuleNames>();
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        //npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select modulename,moduleid,moduledescription from tabmodules where moduletype='HRMS' order by moduleid;");
        //        //npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, " select modulename,moduleid,moduledescription from tabmodules where moduletype='HRMS' and moduleid in(select moduleid from tabrolefunctions where userid=" + userid + ") order by moduleid;");
        //        ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select distinct modulename,tm.moduleid,moduledescription from tabmodules tm join tabrolefunctions trf on tm.moduleid=trf.moduleid where moduletype='HRMS'  and userid=" + userid + " order by moduleid");
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                ModuleNames objmodule = new ModuleNames();
        //                objmodule.ModuleId = Convert.ToInt32(ds.Tables[0].Rows[i]["moduleid"]);
        //                objmodule.ModuleName = Convert.ToString(ds.Tables[0].Rows[i]["modulename"]);
        //                objmodule.moduledescription = Convert.ToString(ds.Tables[0].Rows[i]["moduledescription"]);

        //                objModuleValues.Add(objmodule);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        ds.Dispose();

        //    }

        //    return objModuleValues;
        //}
        //public List<ChildModules> GetChildFunctionNames(int userid)
        //{
        //    List<ChildModules> objChildModuleValues = new List<ChildModules>();
        //    DataSet ds = new DataSet();
        //    try
        //    {


        //        ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select distinct functionname,functionurl,tf.moduleid,functionsortorder from tabfunctions tf join tabrolefunctions trf on tf.functionid=trf.functionid where type='HRMS'  and userid=" + userid + " order by functionsortorder;");
        //        // npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select functionname,functionurl,moduleid from tabfunctions where type='HRMS'  order by moduleid;");
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                ChildModules objChildmodule = new ChildModules();
        //                objChildmodule.ModuleId = Convert.ToInt32(ds.Tables[0].Rows[i]["moduleid"]);
        //                objChildmodule.FunctionName = Convert.ToString(ds.Tables[0].Rows[i]["functionname"]);
        //                objChildmodule.FunctionUrl = Convert.ToString(ds.Tables[0].Rows[i]["functionurl"]);

        //                objChildModuleValues.Add(objChildmodule);
        //            }

        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        ds.Dispose();

        //    }

        //    return objChildModuleValues;
        //}

        #endregion

        #region UserDeclerations

        NpgsqlConnection con = null;
        NpgsqlTransaction trans;
        NpgsqlDataReader npgdr = null;
        string GNextID = string.Empty;

        public int CheckCount(string strCheckValue, string strTableName, string strColumnName)
        {

            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE upper(" + strColumnName + ")='" + ManageQuote(strCheckValue.Trim().ToUpper()) + "'and statusid=1;";

            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }

        public int UpdateCheckCount(string strCheckValue, string strTableName, string strColumnName, string strColumnname1)
        {

            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE upper(" + strColumnName + ")='" + ManageQuote(strCheckValue.Trim().ToUpper()) + "'and statusid=1 and recordid<>" + strColumnname1 + ";";

            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }

        private string ManageQuote(string strMessage)
        {
            try
            {
                if (strMessage != null && strMessage != "")
                {
                    strMessage = strMessage.Replace("'", "''");
                }
            }
            catch (Exception)
            {
                return strMessage;
            }
            return strMessage;
        }

        public static string FormatDate(string strDate)
        {
            string Date = null;
            string[] dat = null;
            if (strDate != null)
            {
                if (strDate.Contains("/"))
                {
                    dat = strDate.Split('/');
                }
                else if (strDate.Contains("-"))
                {
                    dat = strDate.Split('-');
                }
                if (dat[2].Length > 4)
                {
                    dat[2] = dat[2].Substring(0, 4);
                }
                Date = dat[2] + "-" + dat[1] + "-" + dat[0];
            }
            return Date;
        }

        #endregion

        /// <summary>
        /// Vikram
        /// </summary>
        #region Marital Status...

        public bool SaveTypeOfMaritaStatus(MaritalStatus objStatus)
        {
            Boolean IsValid = false;
            try
            {

                int count = check_Existed(objStatus);
                int countstatus = check_status(objStatus);
                if (countstatus == 1)
                {
                    string strInsert = "UPDATE TABHRMSMARITALSTATUSMST SET STATUSID=1,VCHDESCRIPTION='" + ManageQuote(objStatus.vchdescription) + "' WHERE STATUSID=2 AND VCHMARITALSTATUS='" + ManageQuote(objStatus.vchmaritalstatus.ToUpper().Trim()) + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    IsValid = true;
                }
                else if (count == 0)
                {
                    string strInsert1 = "insert into tabhrmsmaritalstatusmst (vchmaritalstatus,vchdescription,STATUSID,createdby,createddate)values('" + ManageQuote(objStatus.vchmaritalstatus.Trim().ToUpper()) + "','" + ManageQuote(objStatus.vchdescription) + "',1,1,CURRENT_TIMESTAMP);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert1);
                    IsValid = true;
                }
                else
                {
                    IsValid = false;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Marital_Status");
                throw ex;
            }
            finally
            {
            }
            return IsValid;
        }
        private int check_status(MaritalStatus objStatus)
        {
            try
            {
                string check_status = "select count(*) from tabhrmsmaritalstatusmst  where vchmaritalstatus='" + ManageQuote(objStatus.vchmaritalstatus.Trim().ToUpper()) + "' and statusid=2;";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_status));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int check_Existed(MaritalStatus objStatus)
        {
            try
            {
                string check_exist = "select count(*) from tabhrmsmaritalstatusmst  where vchmaritalstatus='" + ManageQuote(objStatus.vchmaritalstatus.Trim().ToUpper()) + "' and statusid=1;;";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Marital_Status");
                throw ex;
            }

        }

        public List<MaritalStatus> GetTypeOfMaritalStatus()
        {
            List<MaritalStatus> lstMAritalStatus = new List<MaritalStatus>();
            DataSet ds = new DataSet();
            try
            {
                //   ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select * from tabhrmsmaritalstatusmst WHERE STATUSID=1  ORDER BY RECORDID  DESC;");
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select * from tabhrmsmaritalstatusmst WHERE STATUSID=1  ;");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        MaritalStatus objStatus = new MaritalStatus();
                        objStatus.recordid = Convert.ToInt16(ds.Tables[0].Rows[i]["recordid"]);
                        objStatus.vchdescription = ds.Tables[0].Rows[i]["vchdescription"].ToString();
                        objStatus.vchmaritalstatus = ds.Tables[0].Rows[i]["vchmaritalstatus"].ToString();
                        lstMAritalStatus.Add(objStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Marital_Status");
                throw ex;
            }
            finally
            {
                ds.Dispose();
            }
            return lstMAritalStatus;
        }

        public bool DeleteTypeOfMaritalStatus(int recordid)
        {
            bool is_Updated = false;
            try
            {
                string delete = "UPDATE TABHRMSMARITALSTATUSMST SET STATUSID=2,MODIFIEDBY=1,MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID='" + recordid + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, delete);
                is_Updated = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Marital_Status");
                throw;
            }

            return is_Updated;
        }

        public bool UpdateMaritalStatus(MaritalStatus objStatus)
        {
            bool is_Updated = false;
            try
            {
                int count = UpdateCheckCount(objStatus.vchmaritalstatus, "TABHRMSMARITALSTATUSMST", "VCHMARITALSTATUS", objStatus.recordid.ToString());
                if (count == 0)
                {
                    string update = "UPDATE TABHRMSMARITALSTATUSMST SET VCHMARITALSTATUS='" + ManageQuote(objStatus.vchmaritalstatus.ToUpper()) + "', VCHDESCRIPTION='" + ManageQuote(objStatus.vchdescription) + "',MODIFIEDBY=1 ,MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID='" + objStatus.recordid + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, update);
                    is_Updated = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Marital_Status");
                throw;
            }
            return is_Updated;
        }

        public bool CheckMaritalStatus(MaritalStatus objStatus)
        {
            bool is_Updated = false;
            int numcount = 0;
            try
            {
                string update = "select count(*) from tabhrmsemployeepersonaldetails where vchmaritalstatus='" + ManageQuote(objStatus.vchmaritalstatus) + "' and statusid=1;";
                numcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, update));
                if (numcount == 0)
                {
                    is_Updated = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Marital_Status");
                throw;
            }
            return is_Updated;
        }

        #endregion

        /// <summary>
        /// T.Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Blood Group

        public List<BloodGroupDTO> BindBloodGroup()
        {
            List<BloodGroupDTO> lstcontacts = new List<BloodGroupDTO>();
            DataSet ds = new DataSet();
            try
            {

                //   ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select RECORDID,vchbloodgroup,vchdescription from tabhrmsbloodgroupmst where statusid=1 order by recordid;");
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select RECORDID,vchbloodgroup,vchdescription from tabhrmsbloodgroupmst where statusid=1 ;");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        BloodGroupDTO objBloodGroupDTO = new BloodGroupDTO();
                        objBloodGroupDTO.RecordId = Convert.ToInt32(ds.Tables[0].Rows[i]["RECORDID"]);
                        objBloodGroupDTO.Group = Convert.ToString(ds.Tables[0].Rows[i]["vchbloodgroup"]);
                        objBloodGroupDTO.Description = Convert.ToString(ds.Tables[0].Rows[i]["vchdescription"]);
                        //  objRelationDTO.StatusID = Convert.ToString(dr["STATUSID"]);
                        lstcontacts.Add(objBloodGroupDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Blood_Group");
                throw;
            }
            finally
            {

                ds.Dispose();
            }

            return lstcontacts;
        }

        public bool CreateBloodGroup(BloodGroupDTO C)
        {
            Boolean IsValid = false;
            try
            {

                int res = CheckCount(C.Group, "tabhrmsbloodgroupmst", "vchbloodgroup");
                if (res == 0)
                {
                    string strcount = "select count(*) from tabhrmsbloodgroupmst where vchbloodgroup='" + ManageQuote(C.Group.Trim().ToUpper()) + "' and statusid=2;";
                    int str = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                    if (str == 1)
                    {
                        string strInsert = "update tabhrmsbloodgroupmst set statusid=1,vchdescription='" + ManageQuote(C.Description) + "'  where vchbloodgroup='" + ManageQuote(C.Group.Trim().ToUpper()) + "' ";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                        IsValid = true;
                    }
                    else
                    {

                        string strInsert = "INSERT INTO tabhrmsbloodgroupmst( vchbloodgroup, vchdescription, statusid, createdby, createddate)VALUES ( '" + ManageQuote(C.Group.Trim().ToUpper()) + "', '" + ManageQuote(C.Description) + "', 1, 1, current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);

                        IsValid = true;
                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Blood_Group");


            }
            finally
            {

            }
            return IsValid;

        }

        public bool UpdateBloodGroup(BloodGroupDTO C)
        {
            Boolean IsValid = false;
            try
            {
                int res = UpdateCheckCount(C.Group, "tabhrmsbloodgroupmst", "vchbloodgroup", C.RecordId.ToString());
                if (res == 0)
                {
                    string strUpdate = "update tabhrmsbloodgroupmst set vchbloodgroup='" + ManageQuote(C.Group) + "',VCHDESCRIPTION='" + ManageQuote(C.Description) + "',modifiedby='1',modifieddate=current_timestamp where recordid=" + C.RecordId + " ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);

                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Blood_Group");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        public bool DeleteBloodGroup(int id)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "update tabhrmsbloodgroupmst set statusid=2,modifiedby='1',modifieddate=current_timestamp where recordid='" + id + "'";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Blood_Group");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool CheckBloodGroup(BloodGroupDTO blood)
        {
            bool IsValid = false;
            int checkCount = 0;
            try
            {
                string strCheck = "select count(*) from tabhrmsemployeepersonaldetails where vchbloodgroup='" + ManageQuote(blood.Group) + "' and statusid=1;";
                checkCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCheck));
                if (checkCount == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Blood_Group");
            }
            return IsValid;
        }

        #endregion

        /// <summary>
        /// SRIKANTH.K
        /// </summary>
        #region Relation..

        public List<RelationDTO> BindRelations()
        {
            List<RelationDTO> lstcontacts = new List<RelationDTO>();
            DataSet ds = new DataSet();
            try
            {
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT RECORDID, upper(VCHRELATIONSHIP) as VCHRELATIONSHIP,VCHDESCRIPTION, STATUSID,ROW_NUMBER() OVER() AS  SNO  FROM tabhrmsrelationshipmst where statusid=1");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        RelationDTO objRelationDTO = new RelationDTO();
                        objRelationDTO.SNo = Convert.ToInt32(ds.Tables[0].Rows[i]["SNO"]);
                        objRelationDTO.RecordId = Convert.ToInt32(ds.Tables[0].Rows[i]["RECORDID"]);
                        objRelationDTO.Relation = Convert.ToString(ds.Tables[0].Rows[i]["VCHRELATIONSHIP"]);
                        objRelationDTO.Description = Convert.ToString(ds.Tables[0].Rows[i]["VCHDESCRIPTION"]);
                        //  objRelationDTO.StatusID = Convert.ToString(dr["STATUSID"]);
                        lstcontacts.Add(objRelationDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Relation");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();

                //}
                ds.Dispose();
            }

            return lstcontacts;
        }

        public bool Save(RelationDTO C)
        {
            Boolean IsValid = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                // trans = con.BeginTransaction();
                //int res = GetCount(C.Relation);
                int res = CheckCount(C.Relation, "tabhrmsrelationshipmst", "VCHRELATIONSHIP");
                if (res == 0)
                {
                    string strInsert = "INSERT INTO tabhrmsrelationshipmst( VCHRELATIONSHIP, VCHDESCRIPTION, STATUSID, CREATEDBY, CREATEDDATE) VALUES ('" + ManageQuote(C.Relation.ToUpper()) + "','" + ManageQuote(C.Description) + "',1,1,current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    //cmd = new NpgsqlCommand(strInsert, con);
                    //trans.Commit();
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Relation");
                trans.Rollback();
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();

                    // trans.Dispose();
                }
            }
            return IsValid;

        }
        public bool DeleteRecord(int id)
        {
            bool IsValid = false;
            try
            {

                // string strDelete = "delete from tabhrmsrelationshipmst where recordid=" + id + "";

                string strDelete = "update  tabhrmsrelationshipmst set statusid=2 where recordid=" + id + "";
                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Relation");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public int GetCount(string str)
        {

            string strcount = "SELECT COUNT(*)  FROM tabhrmsrelationshipmst WHERE VCHRELATIONSHIP='" + str + "'";

            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }
        public bool UpdateRelations(RelationDTO C)
        {
            Boolean IsValid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                int res = UpdateCheckCount(C.Relation, "tabhrmsrelationshipmst", "VCHRELATIONSHIP", C.RecordId.ToString());
                if (res == 0)
                {
                    string strUpdate = "update tabhrmsrelationshipmst set VCHRELATIONSHIP='" + ManageQuote(C.Relation) + "',VCHDESCRIPTION='" + ManageQuote(C.Description) + "',modifiedby='1',modifieddate=current_timestamp where recordid=" + C.RecordId + " ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Relation");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();
                //}
            }
            return IsValid;

        }
        public bool CheckRelations(RelationDTO C)
        {
            Boolean IsValid = false;
            int numCount1 = 0;
            int numCount2 = 0;

            try
            {
                numCount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeefamilydetails where upper(vchrelationship)='" + ManageQuote(C.Relation) + "' and statusid=1;"));
                numCount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeemergencycontactdetails where upper(vchrelationship)='" + ManageQuote(C.Relation) + "' and statusid=1;"));
                if (numCount1 == 0 && numCount2 == 0)
                {
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Relation");
                throw;
            }
            return IsValid;

        }


        #endregion

        /// <summary>
        /// Rajesh
        /// </summary>
        /// <returns></returns>
        #region Country
        public bool SaveCountry(CountryDTO objCountry)
        {
            Boolean isvalid = false;
            try
            {

                int count = check_Existed(objCountry);
                if (count == 0)
                {
                    string strSave = "INSERT INTO tabcountry(countryname, statusid, createdby, createddate) VALUES ('" + objCountry.CountryName.Trim().ToUpper() + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return isvalid;
        }
        private int check_Existed(CountryDTO objCountry)
        {
            try
            {
                string check_exist = "SELECT COUNT(countryname) FROM tabcountry  WHERE   statusid=1 and countryname= '" + objCountry.CountryName.Trim().ToUpper() + "'";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CountryDTO> ShowCountryDetails()
        {
            List<CountryDTO> lstCountry = new List<CountryDTO>();
            DataSet ds = new DataSet();
            try
            {
                // string strGetCountry = "select countryid,countryname from tabcountry  where statusid=1 order by countryname;";
                string strGetCountry = "select countryid,countryname from tabcountry  where statusid=1 ;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetCountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CountryDTO objC = new CountryDTO();
                        objC.CountryId = Convert.ToInt32(ds.Tables[0].Rows[i]["countryid"]);
                        objC.CountryName = ds.Tables[0].Rows[i]["countryname"].ToString();
                        lstCountry.Add(objC);
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                ds.Dispose();
            }
            return lstCountry;
        }
        public bool UpdateCountry(CountryDTO objCountry)
        {
            bool isUpdate = false;
            try
            {
                //int count = check_Existed(objCountry);
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcountry where countryname='" + ManageQuote(objCountry.CountryName.ToUpper()) + "' and countryid<>" + objCountry.CountryId + " and statusid=1;"));
                if (count == 0)
                {
                    string update = "UPDATE tabcountry SET countryname='" + objCountry.CountryName.Trim().ToUpper() + "', MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE countryid=" + objCountry.CountryId + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, update);
                    isUpdate = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isUpdate;
        }
        public bool DeleteCountry(int countryid)
        {
            bool isDelete = false;
            try
            {
                int count = check_Existedcountry(countryid);
                if (count == 0)
                {
                    string delete = "UPDATE tabcountry SET STATUSID=2, MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE countryid=" + countryid + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, delete);
                    isDelete = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isDelete;
        }
        private int check_Existedcountry(int countryid)
        {
            try
            {
                string check_exist = "select count(statename) from tabstate where countryid=" + countryid + " and statusid=1;";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckCountry(CountryDTO objCountry)
        {
            bool isUpdate = false;
            try
            {
                int numcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeaddressdetails where vchcountry='" + objCountry.CountryName + "' and statusid=1;"));
                int numcount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + objCountry.CountryId + " and statusid=1; "));
                int numcount3 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabstate where countryid=" + objCountry.CountryId + " and statusid=1;"));
                if (numcount1 == 0 && numcount2 == 0 && numcount3 == 0)
                {
                    isUpdate = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isUpdate;
        }

        #endregion

        /// <summary>
        /// Rajesh
        /// </summary>
        /// <returns></returns>
        #region State

        public bool SaveState(StateDTO objState)
        {
            Boolean isvalid = false;
            try
            {

                int count = check_Existed(objState);
                if (count == 0)
                {
                    string strSave = "INSERT INTO tabstate(statename, countryid,statusid, createdby, createddate) VALUES ('" + objState.state.Trim().ToUpper() + "','" + objState.CountryId + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
            }
            return isvalid;
        }
        private int check_Existed(StateDTO objState)
        {
            try
            {
                string check_exist = "SELECT COUNT(statename) FROM tabstate  WHERE statusid=1 and statename= '" + objState.state.Trim().ToUpper() + "' and countryid=" + objState.CountryId + "";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<StateDTO> BindState()
        {
            List<StateDTO> lstState = new List<StateDTO>();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT STATENAME,ST.STATEID,ST.STATUSID,CO.COUNTRYID,CO.COUNTRYNAME FROM TABSTATE ST LEFT JOIN TABCOUNTRY CO  ON ST.COUNTRYID=CO.COUNTRYID WHERE ST.STATUSID=1;", NPGSqlHelper.SQLConnString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    StateDTO objState = new StateDTO();

                    objState.StateId = Convert.ToInt32(dr["STATEID"].ToString());
                    objState.Country = dr["countryname"].ToString();
                    objState.state = dr["statename"].ToString();
                    objState.CountryId = (dr["countryid"].ToString());
                    lstState.Add(objState);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
            }
            return lstState;
        }
        public bool UpdateState(StateDTO objState)
        {
            Boolean IsValid = false;
            try
            {
                //int count = check_Existed(objState);
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabstate where statusid=1 and statename='" + ManageQuote(objState.state.ToUpper()) + "' and stateid<>" + objState.StateId + ""));
                if (count == 0)
                {
                    string strUpdate = "update tabstate set statename='" + ManageQuote(objState.state.ToUpper()) + "',countryid=" + objState.CountryId + ",modifiedby='1',modifieddate=current_timestamp where stateid=" + objState.StateId + " ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }
        public bool DeleteState(int stateid, string countryid)
        {
            bool IsValid = false;
            try
            {
                int count = check_Existedstate(stateid, countryid);
                if (count == 0)
                {
                    string strDelete = "update tabstate  set statusid=2 where stateid=" + stateid + "";
                    int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }

            return IsValid;
        }
        private int check_Existedstate(int stateid, string countryid)
        {
            try
            {
                string check_exist = "select count(cityname) from tabcity where statusid=1 and stateid=" + stateid + " and countryid=" + countryid + ";";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckState(StateDTO objState)
        {
            Boolean IsValid = false;
            try
            {
                int numcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeaddressdetails where vchstate='" + ManageQuote(objState.state) + "' and statusid=1;"));
                int numcount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where stateid=" + objState.StateId + " and statusid=1;"));
                if (numcount1 == 0 && numcount2 == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        #endregion

        /// <summary>
        /// RajiReddy
        /// </summary>
        /// <returns></returns>
        #region City Details

        public bool CreateCity(City City)
        {
            bool IsValid = false;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + City.CountryId + " and stateid=" + City.StateId + " and upper(cityname)='" + ManageQuote(City.CityName.ToUpper()) + "' and statusid=1;"));
                if (count == 0)
                {
                    string strCity = "insert into tabcity(cityname,stateid,countryid,statusid,createdby,createddate) values('" + ManageQuote(City.CityName.ToUpper()) + "','" + City.StateId + "','" + City.CountryId + "',1,1,current_timestamp) ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strCity);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw ex;

            }
            finally
            {

            }
            return IsValid;
        }

        public List<City> ShowCountry()
        {
            List<City> lstcountry = new List<City>();
            DataSet ds = new DataSet();
            try
            {

                string strcountry = "select countryid,countryname from tabcountry where statusid=1 order by countryname ;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        City objcountry = new City();

                        objcountry.CountryId = ds.Tables[0].Rows[i]["countryid"].ToString();
                        objcountry.Country = ds.Tables[0].Rows[i]["countryname"].ToString();

                        lstcountry.Add(objcountry);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");

                throw;
            }
            finally
            {

                ds.Dispose();
            }
            return lstcountry;
        }

        public List<City> ShowStates(string strcountryid)
        {
            List<City> lstStates = new List<City>();
            DataSet ds = new DataSet();
            try
            {

                string strcountry = "select stateid,statename from tabstate where countryid='" + strcountryid + "'  and statusid=1 order by statename;";

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        City objstate = new City();

                        objstate.StateId = ds.Tables[0].Rows[i]["stateid"].ToString();
                        objstate.State = ds.Tables[0].Rows[i]["statename"].ToString();

                        lstStates.Add(objstate);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "State");
                throw;
            }
            finally
            {
                ds.Dispose();
            }
            return lstStates;
        }

        public List<City> BindCityDetails()
        {
            List<City> lstcitydetails = new List<City>();
            DataSet ds = new DataSet();
            try
            {

                // ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tsc.countryid,countryname,tsc.stateid,statename,cityid,cityname from tabcity tc join (SELECT tco.countryid,stateid,countryname,statename from tabstate ts join tabcountry tco on ts.countryid=tco.countryid)tsc on tc.countryid=tsc.countryid and tc.stateid=tsc.stateid where statusid=1 order by cityid desc;");
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select tsc.countryid,countryname,tsc.stateid,statename,cityid,cityname from tabcity tc join (SELECT tco.countryid,stateid,countryname,statename from tabstate ts join tabcountry tco on ts.countryid=tco.countryid)tsc on tc.countryid=tsc.countryid and tc.stateid=tsc.stateid where statusid=1 order by cityid;");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        City objcityDTO = new City();

                        objcityDTO.Country = Convert.ToString(ds.Tables[0].Rows[i]["countryname"]);
                        objcityDTO.CountryId = Convert.ToString(ds.Tables[0].Rows[i]["countryid"]);
                        objcityDTO.State = Convert.ToString(ds.Tables[0].Rows[i]["statename"]);
                        objcityDTO.StateId = Convert.ToString(ds.Tables[0].Rows[i]["stateid"]);
                        objcityDTO.CityName = Convert.ToString(ds.Tables[0].Rows[i]["cityname"]);
                        objcityDTO.CityId = Convert.ToString(ds.Tables[0].Rows[i]["cityid"]);

                        lstcitydetails.Add(objcityDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }

            finally
            {
                ds.Dispose();
            }
            return lstcitydetails;
        }

        public bool UpdateCity(City objcity)
        {
            bool IsValid = false;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + objcity.CountryId + " and stateid=" + objcity.StateId + " and upper(cityname)='" + ManageQuote(objcity.CityName.ToUpper()) + "' and statusid=1 and cityid<>" + objcity.CityId + ";"));
                if (count == 0)
                {
                    string strupdate = "update tabcity set cityname='" + ManageQuote(objcity.CityName.ToUpper()) + "',stateid=" + objcity.StateId + ",countryid=" + objcity.CountryId + " ,statusid=1,modifiedby=1,modifieddate=current_timestamp where cityid=" + objcity.CityId + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdate);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        public bool DeleteCity(City citydto)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "update tabcity set statusid=2,modifiedby=1,modifieddate=current_timestamp where cityid=" + citydto.CityId + ";";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool CheckCity(City objcity)
        {
            bool IsValid = false;
            try
            {
                int numcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeaddressdetails where vchcity='" + ManageQuote(objcity.CityName) + "' and statusid=1;"));
                if (numcount == 0)
                {
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }


        #endregion

        /// <summary>
        /// Madhu
        /// </summary>
        #region Department

        public List<DepartmentMaster> BindCompanyNames()
        {
            List<DepartmentMaster> lstCompanyNames = new List<DepartmentMaster>();
            try
            {
                string strlocation = "SELECT COMPANYID,VCHCOMPANYNAME from tabcompany ORDER BY VCHCOMPANYNAME;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    DepartmentMaster objDepartmentMaster = new DepartmentMaster();
                    objDepartmentMaster.CompanyName = npgdr["VCHCOMPANYNAME"].ToString();
                    objDepartmentMaster.RecordId = Convert.ToInt32(npgdr["COMPANYID"]);
                    lstCompanyNames.Add(objDepartmentMaster);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                //throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstCompanyNames;
        }

        public List<DepartmentMaster> ShowDepartmentDetails()
        {
            List<DepartmentMaster> lstDepartmentdetails = new List<DepartmentMaster>();
            try
            {
                //  string strlocation = "SELECT RECORDID,VCHCOMPANYNAME,VCHDEPARTMENT,VCHDESCRIPTION FROM TABHRMSDEPARTMENTMST WHERE STATUSID=1 ORDER BY VCHDEPARTMENT;";
                string strlocation = "SELECT RECORDID,VCHCOMPANYNAME,VCHDEPARTMENT,VCHDESCRIPTION FROM TABHRMSDEPARTMENTMST WHERE STATUSID=1 ;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    DepartmentMaster objDepartmentMaster = new DepartmentMaster();
                    objDepartmentMaster.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objDepartmentMaster.CompanyName = npgdr["VCHCOMPANYNAME"].ToString();
                    objDepartmentMaster.Department = Convert.ToString(npgdr["VCHDEPARTMENT"]);
                    objDepartmentMaster.Description = npgdr["VCHDESCRIPTION"].ToString();
                    lstDepartmentdetails.Add(objDepartmentMaster);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                //throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstDepartmentdetails;
        }

        public bool SaveDepartment(DepartmentMaster Dept)
        {
            Boolean IsValid = false;
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(VCHDEPARTMENT) FROM TABHRMSDEPARTMENTMST  WHERE  VCHDEPARTMENT='" + Dept.Department.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO TABHRMSDEPARTMENTMST(VCHDEPARTMENT,VCHDESCRIPTION,STATUSID,createdby,createddate)VALUES('" + Dept.Department.Trim().ToUpper() + "','" + Dept.Description + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool UpdateDepartment(DepartmentMaster Dept)
        {
            Boolean IsValid = false;
            int Count = 0;
            try
            {

                // string check_exist = "SELECT COUNT(VCHDEPARTMENT) FROM TABHRMSDEPARTMENTMST  WHERE  VCHDEPARTMENT='" + Dept.Department.Trim().ToUpper() + "' and statusid=1";
                // Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                Count = UpdateCheckCount(Dept.Department, "TABHRMSDEPARTMENTMST", "VCHDEPARTMENT", Dept.RecordId.ToString());
                if (Count == 0)
                {
                    string strInsert = "UPDATE TABHRMSDEPARTMENTMST SET VCHDESCRIPTION='" + Dept.Description + "',VCHDEPARTMENT='" + Dept.Department + "',modifieddate=current_timestamp,modifiedby=1 WHERE recordid='" + Dept.RecordId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool DeleteDepartment(int RecordId)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "UPDATE TABHRMSDEPARTMENTMST SET STATUSID=2 WHERE RECORDID='" + RecordId + "';";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool CheckDepartment(DepartmentMaster Dept)
        {
            bool IsValid = false;
            try
            {

                string strCheck = "select count(*) from tabhrmsemployeeemployment where vchdepartment='" + ManageQuote(Dept.Department) + "' and statusid=1; ";

                int numcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCheck));

                if (numcount == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Department");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        #endregion

        /// <summary>
        /// K.Nagendar
        /// </summary>
        #region Designation

        public bool SaveDesignation(string Json)
        {
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                if (JsonData.ContainsKey("vchdescription") == false)
                {

                    JsonData.Add("vchdescription", "");

                }
                //trans = con.BeginTransaction();
                string deg = JsonData["vchdesignation"].ToString();
                string Designation = deg.ToUpper().Trim();
                int res = CheckCount(Designation, "tabhrmsdesignationmst", "VCHDESIGNATION");
                if (res == 0)
                {
                    string strQuery = "INSERT INTO tabhrmsdesignationmst(vchdesignation, vchdescription,statusid, createdby, createddate) VALUES ('" + Designation + "','" + JsonData["vchdescription"] + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strQuery);
                    //trans.Commit();
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Designation");

                trans.Rollback();
                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    // con.ClearPool();
                    // trans.Dispose();
                }
            }
            return isSaved;
        }

        public bool DeleteDesignation(int id)
        {

            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                string strQuery = "update tabhrmsdesignationmst set statusid=2 , modifiedby=1 , modifieddate=current_timestamp   where recordid=" + id + "";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);

                trans.Commit();
                isSaved = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Designation");

                trans.Rollback();
                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                    trans.Dispose();
                }
            }
            return isSaved;
        }

        public bool UpdateDesignation(int id, string jSON)
        {
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(jSON);
            bool isSaved = false;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                if (JsonData.ContainsKey("vchdescription") == false)
                {

                    JsonData.Add("vchdescription", "");

                }
                trans = con.BeginTransaction();
                string deg = JsonData["vchdesignation"].ToString();
                string Designation = deg.ToUpper().Trim();
                int res = UpdateCheckCount(Designation, "tabhrmsdesignationmst", "VCHDESIGNATION", id.ToString());
                if (res == 0)
                {
                    string strQuery = "update tabhrmsdesignationmst set vchdesignation='" + Designation + "' ,vchdescription ='" + JsonData["vchdescription"] + "'   where recordid=" + id + " ";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strQuery);
                    trans.Commit();
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Designation");

                trans.Rollback();
                isSaved = false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    //con.ClearPool();
                    trans.Dispose();
                }
            }
            return isSaved;
        }


        public DataTable ShowDesignation()
        {
            DataTable dt = new DataTable();
            string strData = string.Empty;
            try
            {

                strData = "select recordid, vchdesignation,vchdescription from tabhrmsdesignationmst where statusid=1 order by recordid";
                dt = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strData).Tables[0];
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Designation");
            }

            return dt;
        }

        public bool CheckDesignation(string Json)
        {
            bool isSaved = false;
            int numcount1 = 0;
            int numcount2 = 0;
            int numcount3 = 0;
            int numcount4 = 0;
            int numcount5 = 0;
            var JsonData = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(Json);
            try
            {
                numcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeemployment where vchdesignation='" + ManageQuote(JsonData["vchdesignation"].ToString()) + "' and statusid=1; "));
                numcount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeepreviousexperience where vchdesignation='" + ManageQuote(JsonData["vchdesignation"].ToString()) + "' and statusid=1;"));
                numcount3 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeekapilcarrerdetails where vchdesignation='" + ManageQuote(JsonData["vchdesignation"].ToString()) + "' and statusid=1;"));
                numcount4 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsbranchallowancemst where vchdesignation='" + ManageQuote(JsonData["vchdesignation"].ToString()) + "' and statusid=1;"));
                numcount5 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsvehicleallowancemst where vchdesignation='" + ManageQuote(JsonData["vchdesignation"].ToString()) + "' and statusid=1;"));

                if (numcount1 == 0 && numcount2 == 0 && numcount3 == 0 && numcount4 == 0 && numcount5 == 0)
                {
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Designation");

                trans.Rollback();
                isSaved = false;
                throw ex;
            }

            return isSaved;
        }

        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        /// <returns></returns>
        #region COURSE..

        public List<CourseDTO> ShowCourse()
        {
            List<CourseDTO> lstCourse = new List<CourseDTO>();

            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT RECORDID, upper(vchcourse) as vchcourse,VCHDESCRIPTION, STATUSID  FROM tabhrmscoursemst where statusid=1");
                while (npgdr.Read())
                {
                    CourseDTO objCourseDTO = new CourseDTO();
                    objCourseDTO.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objCourseDTO.Course = Convert.ToString(npgdr["vchcourse"]);
                    objCourseDTO.Description = Convert.ToString(npgdr["VCHDESCRIPTION"]);
                    //  objRelationDTO.StatusID = Convert.ToString(dr["STATUSID"]);
                    lstCourse.Add(objCourseDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "COURSE");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();
                //}
                npgdr.Dispose();
            }

            return lstCourse;
        }
        public bool CreateCourse(CourseDTO C)
        {
            Boolean IsValid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                int res = CheckCount(C.Course, "tabhrmscoursemst", "vchcourse");
                if (res == 0)
                {
                    string strInsert = "INSERT INTO tabhrmscoursemst( vchcourse, VCHDESCRIPTION, STATUSID, CREATEDBY, CREATEDDATE) VALUES ('" + ManageQuote(C.Course.ToUpper()) + "','" + ManageQuote(C.Description) + "',1,1,current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    //cmd = new NpgsqlCommand(strInsert, con);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "COURSE");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();
                //}
            }
            return IsValid;

        }
        public bool DeleteCourse(int id)
        {
            bool IsValid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}

                //string strDelete = "delete from tabhrmscoursemst where recordid=" + id + "";
                string strDelete = "update  tabhrmscoursemst set statusid=2 where recordid=" + id + "";
                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "COURSE");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();
                //}
            }
            return IsValid;
        }
        public int GetCourseCount(string str)
        {
            int res = 0;
            try
            {


                string strcount = "SELECT COUNT(*)  FROM tabhrmscoursemst WHERE vchcourse='" + str + "'";

                res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "COURSE");
                throw;
            }
            finally
            {

            }
            return res;
        }
        public bool UpdateCourse(CourseDTO C)
        {
            Boolean IsValid = false;
            try
            {
                int res = UpdateCheckCount(C.Course, "tabhrmscoursemst", "vchcourse", C.RecordId.ToString());
                if (res == 0)
                {
                    string strUpdate = "update tabhrmscoursemst set vchcourse='" + ManageQuote(C.Course) + "',VCHDESCRIPTION='" + ManageQuote(C.Description) + "',modifiedby='1',modifieddate=current_timestamp where recordid=" + C.RecordId + " ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "COURSE");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        public bool CheckCourse(CourseDTO C)
        {
            Boolean IsValid = false;
            int numCount1 = 0;
            int numCount2 = 0;
            try
            {
                numCount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeeducationdetails where vchcourse='" + ManageQuote(C.Course) + "' and statusid=1;"));
                numCount2 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsgroupmst where vchcourse='" + ManageQuote(C.Course) + "' and statusid=1;"));
                if (numCount1 == 0 && numCount2 == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "COURSE");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        #endregion

        /// <summary>
        /// SriKanth
        /// </summary>
        /// <returns></returns>
        #region GROUP..

        public List<GroupDTO> ShowGroup()
        {
            List<GroupDTO> lstGroup = new List<GroupDTO>();

            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT RECORDID, upper(vchcourse) as vchcourse,upper(vchgroup) as vchgroup,VCHDESCRIPTION,STATUSID  FROM tabhrmsgroupmst where statusid=1");
                while (npgdr.Read())
                {
                    GroupDTO objGroupDTO = new GroupDTO();
                    objGroupDTO.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objGroupDTO.CourseName = Convert.ToString(npgdr["vchcourse"]);
                    objGroupDTO.GroupName = Convert.ToString(npgdr["vchgroup"]);
                    objGroupDTO.Description = Convert.ToString(npgdr["VCHDESCRIPTION"]);
                    objGroupDTO.StatusID = Convert.ToString(npgdr["STATUSID"]);
                    lstGroup.Add(objGroupDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Group");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();

                //}
                npgdr.Dispose();
            }

            return lstGroup;
        }
        public bool CreateGroup(GroupDTO C)
        {
            Boolean IsValid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsgroupmst where vchcourse='" + ManageQuote(C.CourseName) + "' and vchgroup='" + ManageQuote(C.GroupName) + "';"));
                if (res == 0)
                {
                    string strInsert = "INSERT INTO tabhrmsgroupmst( vchcourse,vchgroup, VCHDESCRIPTION, STATUSID, CREATEDBY, CREATEDDATE) VALUES ('" + ManageQuote(C.CourseName) + "','" + ManageQuote(C.GroupName.ToUpper()) + "','" + ManageQuote(C.Description) + "',1,1,current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    //cmd = new NpgsqlCommand(strInsert, con);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Group");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();

                //}
            }
            return IsValid;

        }
        public bool DeleteGroup(int id)
        {
            bool IsValid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}

                string strDelete = "update tabhrmsgroupmst set statusid='2'  where recordid=" + id + "";
                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Group");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();

                //}
            }
            return IsValid;
        }
        public bool UpdateGroup(GroupDTO C)
        {
            Boolean IsValid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsgroupmst where vchcourse='" + ManageQuote(C.CourseName) + "' and vchgroup='" + ManageQuote(C.GroupName) + "' and recordid<>" + C.RecordId + ";"));
                if (res == 0)
                {
                    string strUpdate = "update tabhrmsgroupmst set vchcourse='" + ManageQuote(C.CourseName) + "',vchgroup='" + ManageQuote(C.GroupName) + "',VCHDESCRIPTION='" + ManageQuote(C.Description) + "',modifiedby='1',modifieddate=current_timestamp where recordid=" + C.RecordId + " ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);

                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Group");
                throw;
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //    con.Dispose();
                //    con.ClearPool();

                //}
            }
            return IsValid;

        }

        public bool CheckGroup(GroupDTO C)
        {
            Boolean IsValid = false;
            int numCount1 = 0;
            try
            {
                numCount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeeeducationdetails where vchgroup='" + ManageQuote(C.GroupName) + "' and statusid=1;"));

                if (numCount1 == 0)
                {
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Group");
                throw;
            }
            return IsValid;

        }

        #endregion

        /// <summary>
        /// Srinivas
        /// </summary>
        /// <returns></returns>
        #region Occupation

        public List<OccupationMaster> ShowOccupationDetails()
        {
            List<OccupationMaster> lstDepartmentdetails = new List<OccupationMaster>();
            try
            {
                //  string strlocation = "select recordid,vchoccupation,vchdescription from tabhrmsoccupationmst  WHERE STATUSID=1 ORDER BY vchoccupation;";
                string strlocation = "select recordid,vchoccupation,vchdescription from tabhrmsoccupationmst  WHERE STATUSID=1 ;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    OccupationMaster objDepartmentMaster = new OccupationMaster();
                    objDepartmentMaster.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objDepartmentMaster.Occupationname = npgdr["vchoccupation"].ToString();
                    objDepartmentMaster.Description = npgdr["vchdescription"].ToString();
                    lstDepartmentdetails.Add(objDepartmentMaster);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Occupation");
                //throw ex;
                // ErrorMessage(ex);
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstDepartmentdetails;
        }

        public bool SaveOccupation(OccupationMaster Occupation)
        {
            Boolean IsValid = false;
            int Count = 0;
            try
            {

                string check_exist = "SELECT COUNT(vchoccupation) FROM tabhrmsoccupationmst  WHERE  vchoccupation='" + Occupation.Occupationname.Trim().ToUpper() + "' and statusid=1";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                if (Count == 0)
                {
                    string strInsert = "INSERT INTO tabhrmsoccupationmst(vchoccupation,vchdescription,STATUSID,createdby,createddate)VALUES('" + Occupation.Occupationname.Trim().ToUpper() + "','" + Occupation.Description + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Occupation");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool UpdateOccupation(OccupationMaster Occupation)
        {
            Boolean IsValid = false;
            int Count = 0;
            try
            {
                //string check_exist = "SELECT COUNT(vchoccupation) FROM tabhrmsoccupationmst  WHERE  vchoccupation='" + Occupation.Occupationname.Trim().ToUpper() + "' and statusid=1";
                //Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
                Count = UpdateCheckCount(Occupation.Occupationname, "tabhrmsoccupationmst", "vchoccupation", Occupation.RecordId.ToString());
                if (Count == 0)
                {
                    string strInsert = "UPDATE tabhrmsoccupationmst SET VCHDESCRIPTION='" + Occupation.Description + "',vchoccupation='" + Occupation.Occupationname + "',modifieddate=current_timestamp,modifiedby=1 WHERE recordid='" + Occupation.RecordId + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Occupation");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool DeleteOccupation(int RecordId)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "UPDATE tabhrmsoccupationmst SET STATUSID=2 WHERE RECORDID='" + RecordId + "';";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Occupation");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool CheckOccupation(OccupationMaster Occupation)
        {
            bool IsValid = false;
            try
            {
                string strCheck = "select count(*) from tabhrmsemployeefamilydetails where vchoccupation='" + ManageQuote(Occupation.Occupationname) + "' and statusid=1;";

                int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCheck));

                if (res == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Occupation");
                throw;
            }

            return IsValid;
        }

        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region Reason For Leaving
        public List<ReasonForLeavingDTO> BindReasonForLeaving()
        {
            List<ReasonForLeavingDTO> objReasForLeaValues = new List<ReasonForLeavingDTO>();
            try
            {

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select RECORDID,vchreasonsfoeleaving ,vchdescription,StatusID,createdby,createddate from tabhrmsreasonsfoeleavingmst WHERE statusid=1 order by recordid");
                while (npgdr.Read())
                {
                    ReasonForLeavingDTO objReasForLeaDTO = new ReasonForLeavingDTO();
                    objReasForLeaDTO.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objReasForLeaDTO.vchreasonsfoeleaving = Convert.ToString(npgdr["vchreasonsfoeleaving"]);
                    objReasForLeaDTO.vchdescription = Convert.ToString(npgdr["vchdescription"]);
                    objReasForLeaDTO.StatusID = Convert.ToInt32(npgdr["STATUSID"]);
                    objReasForLeaDTO.createdby = Convert.ToInt32(npgdr["createdby"]);
                    objReasForLeaDTO.createddate = Convert.ToString(npgdr["createddate"]);
                    ////objReasForLeaDTO.modifiedby = Convert.ToInt32(npgdr["modifiedby"]);
                    //objReasForLeaDTO.modifieddate = Convert.ToString(npgdr["modifieddate"]);
                    objReasForLeaValues.Add(objReasForLeaDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_Leaving");
                throw;
            }
            finally
            {

                npgdr.Dispose();
            }
            return objReasForLeaValues;
        }
        public bool Save(ReasonForLeavingDTO C)
        {
            Boolean IsValid = false;
            try
            {
                int res = CheckCount(C.vchreasonsfoeleaving, "tabhrmsreasonsfoeleavingmst", "vchreasonsfoeleaving");
                if (res == 0)
                {
                    string strInsert = "INSERT INTO tabhrmsreasonsfoeleavingmst( vchreasonsfoeleaving,vchdescription,StatusID,createdby,createddate) VALUES ('" + C.vchreasonsfoeleaving.ToUpper() + "','" + C.vchdescription + "'," + C.StatusID + ",'" + C.createdby + "',current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    //cmd = new NpgsqlCommand(strInsert, con);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_Leaving");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public bool DeleteReasonForLeavingRecord(int id)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "update tabhrmsreasonsfoeleavingmst set statusid=2 where recordid=" + id + "";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_Leaving");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public bool UpdateReasonForLeavingRecord(ReasonForLeavingDTO C)
        {
            bool IsValid = false;
            try
            {
                int res = UpdateCheckCount(C.vchreasonsfoeleaving, "tabhrmsreasonsfoeleavingmst", "vchreasonsfoeleaving", C.RecordId.ToString());
                if (res == 0)
                {
                    // string strDelete = "update tabhrmsreasonsfoeleavingmst set vchreasonsfoeleaving='" + C.vchreasonsfoeleaving + "',vchdescription='" + C.vchdescription + "' where recordid=" + C.RecordId + "";
                    string strDelete = "update tabhrmsreasonsfoeleavingmst set vchreasonsfoeleaving='" + C.vchreasonsfoeleaving + "',vchdescription='" + C.vchdescription + "',modifieddate=current_timestamp,modifiedby=1 where recordid=" + C.RecordId + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_Leaving");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool CheckReasonForLeaving(ReasonForLeavingDTO C)
        {
            bool IsValid = false;
            int numCount = 0;
            try
            {
                numCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeepreviousexperience where vchreasonforleaving='" + ManageQuote(C.vchreasonsfoeleaving) + "' and statusid=1;"));

                if (numCount == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_Leaving");
                throw;
            }

            return IsValid;
        }


        #endregion

        /// <summary>
        /// Shubha
        /// </summary>
        /// <returns></returns>
        #region Reason For Transfering
        public List<ReasonForTransferingDTO> BindReasonForTransfering()
        {
            List<ReasonForTransferingDTO> objReasForLeaValues = new List<ReasonForTransferingDTO>();
            try
            {

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select RECORDID,vchreasonsfoetransfer ,vchdescription,StatusID,createdby,createddate from tabhrmsreasonsfoetransfermst WHERE statusid=1 order by recordid");
                while (npgdr.Read())
                {
                    ReasonForTransferingDTO objReasForLeaDTO = new ReasonForTransferingDTO();
                    objReasForLeaDTO.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objReasForLeaDTO.vchreasonsfoetransfer = Convert.ToString(npgdr["vchreasonsfoetransfer"]);
                    objReasForLeaDTO.vchdescription = Convert.ToString(npgdr["vchdescription"]);
                    objReasForLeaDTO.StatusID = Convert.ToInt32(npgdr["STATUSID"]);
                    objReasForLeaDTO.createdby = Convert.ToInt32(npgdr["createdby"]);
                    objReasForLeaDTO.createddate = Convert.ToString(npgdr["createddate"]);
                    //objReasForLeaDTO.modifiedby = Convert.ToInt32(npgdr["modifiedby"]);
                    //objReasForLeaDTO.modifieddate = Convert.ToString(npgdr["modifieddate"]);
                    objReasForLeaValues.Add(objReasForLeaDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_transfer");
                throw;
            }
            finally
            {

                npgdr.Dispose();
            }
            return objReasForLeaValues;
        }
        public bool Save(ReasonForTransferingDTO C)
        {
            Boolean IsValid = false;
            try
            {

                int res = CheckCount(C.vchreasonsfoetransfer, "tabhrmsreasonsfoetransfermst", "vchreasonsfoetransfer");
                if (res == 0)
                {
                    string strInsert = "INSERT INTO tabhrmsreasonsfoetransfermst( vchreasonsfoetransfer,vchdescription,StatusID,createdby,createddate) VALUES ('" + C.vchreasonsfoetransfer.ToUpper() + "','" + C.vchdescription + "'," + C.StatusID + ",'" + C.createdby + "',current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    //cmd = new NpgsqlCommand(strInsert, con);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_transfer");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public bool DeleteReasonForTransferingRecord(int id)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "update tabhrmsreasonsfoetransfermst set statusid=2 where recordid=" + id + "";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_transfer");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public bool UpdateReasonForTransferingRecord(ReasonForTransferingDTO C)
        {
            bool IsValid = false;
            try
            {
                int res = UpdateCheckCount(C.vchreasonsfoetransfer, "tabhrmsreasonsfoetransfermst", "vchreasonsfoetransfer", C.RecordId.ToString());
                if (res == 0)
                {
                    //string strDelete = "update tabhrmsreasonsfoetransfermst set vchreasonsfoetransfer='" + C.vchreasonsfoetransfer + "',vchdescription='" + C.vchdescription + "'where recordid=" + C.RecordId + "";
                    string strDelete = "update tabhrmsreasonsfoetransfermst set vchreasonsfoetransfer='" + C.vchreasonsfoetransfer + "',vchdescription='" + C.vchdescription + "',modifieddate=current_timestamp,modifiedby=1 where recordid=" + C.RecordId + "";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_transfer");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public bool CheckReasonForTransfering(ReasonForTransferingDTO C)
        {
            bool IsValid = false;
            int numCount = 0;
            try
            {
                numCount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsemployeekapilcarrerdetails where vchreasonfortransfer='" + ManageQuote(C.vchreasonsfoetransfer) + "' and statusid=1;"));

                if (numCount == 0)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Reason_Leaving");
                throw;
            }

            return IsValid;
        }

        #endregion

        /// <summary>
        /// Satish.D
        /// </summary>
        #region Type of Employement...

        public bool SaveTypeOfEmployee(EmployeeDTO objEmp)
        {
            Boolean IsValid = false;
            try
            {

                //int count = check_Existed(objEmp);
                //if (count == 0)
                //{

                int res = CheckCount(objEmp.TypeOFEmployement, "TABHRMSTYPEOFEMPLOYMENTMST", "vchtypeofemployment");
                if (res == 0)
                {
                    string strcount = "select count(*) from TABHRMSTYPEOFEMPLOYMENTMST where vchtypeofemployment='" + ManageQuote(objEmp.TypeOFEmployement.ToUpper()) + "' and statusid=2;";
                    int str = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
                    if (str == 1)
                    {
                        string strInsert = "update TABHRMSTYPEOFEMPLOYMENTMST set statusid=1,vchdescription='" + ManageQuote(objEmp.EmployementDescription) + "' where vchtypeofemployment='" + ManageQuote(objEmp.TypeOFEmployement.ToUpper()) + "' ";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                        IsValid = true;
                    }
                    else
                    {
                        string strInsert = "INSERT INTO tabhrmstypeofemploymentmst( vchtypeofemployment, vchdescription, STATUSID, CREATEDBY, CREATEDDATE) VALUES ('" + ManageQuote(objEmp.TypeOFEmployement.Trim().ToUpper()) + "','" + ManageQuote(objEmp.EmployementDescription) + "',1,1,current_timestamp)";
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                        IsValid = true;
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Type_Employement");
                throw ex;
            }
            finally
            {

            }
            return IsValid;
        }

        private int check_Existed(EmployeeDTO objEmp)
        {
            try
            {
                string check_exist = "SELECT COUNT(VCHTYPEOFEMPLOYMENT) FROM TABHRMSTYPEOFEMPLOYMENTMST  WHERE  VCHTYPEOFEMPLOYMENT='" + objEmp.TypeOFEmployement.Trim().ToUpper() + "' ";
                return Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Type_Employement");
                throw ex;
            }

        }

        public List<EmployeeDTO> GetTypeOfEmployement()
        {
            List<EmployeeDTO> lstEmp = new List<EmployeeDTO>();
            try
            {
                // string strlocation = "SELECT RECORDID,VCHTYPEOFEMPLOYMENT, VCHDESCRIPTION FROM TABHRMSTYPEOFEMPLOYMENTMST WHERE STATUSID=1 ORDER BY RECORDID DESC;";
                string strlocation = "SELECT RECORDID,VCHTYPEOFEMPLOYMENT, VCHDESCRIPTION FROM TABHRMSTYPEOFEMPLOYMENTMST WHERE STATUSID=1 ;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    EmployeeDTO objEmp = new EmployeeDTO();
                    objEmp.recordid = Convert.ToInt32(npgdr["recordid"]);
                    objEmp.TypeOFEmployement = npgdr["vchtypeofemployment"].ToString();
                    objEmp.EmployementDescription = npgdr["vchdescription"].ToString();
                    lstEmp.Add(objEmp);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Type_Employement");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstEmp;
        }

        public bool DeleteTypeOfEmployement(int recordid)
        {
            bool is_Updated = false;
            try
            {
                string delete = "UPDATE TABHRMSTYPEOFEMPLOYMENTMST SET STATUSID=2, MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID=" + recordid + "";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, delete);
                is_Updated = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Type_Employement");

                throw;
            }

            return is_Updated;
        }

        public bool UpdateEmployement(EmployeeDTO objEmp)
        {
            bool is_Updated = false;
            try
            {

                int count = UpdateCheckCount(objEmp.TypeOFEmployement, "TABHRMSTYPEOFEMPLOYMENTMST", "vchtypeofemployment", objEmp.recordid.ToString());
                //int count = check_Existed(objEmp);
                if (count == 0)
                {
                    //string update = "UPDATE TABHRMSTYPEOFEMPLOYMENTMST SET vchdescription='" + objEmp.EmployementDescription.Trim().ToUpper() + "', MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID=" + objEmp.recordid + "";
                    string update = "UPDATE TABHRMSTYPEOFEMPLOYMENTMST SET vchdescription='" + objEmp.EmployementDescription + "', vchtypeofemployment='" + objEmp.TypeOFEmployement.Trim().ToUpper() + "', MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID='" + objEmp.recordid + "'";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, update);
                    is_Updated = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Type_Employement");

                throw;
            }

            return is_Updated;
        }

        public bool CheckTypeOfEmployement(EmployeeDTO objEmp)
        {
            bool is_Updated = false;
            int numcount = 0;

            try
            {
                string strCheck = "select count(*) from tabhrmsemployeeemployment where vchtypeofemployment='" + ManageQuote(objEmp.TypeOFEmployement) + "' and statusid=1; ";
                numcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCheck));
                if (numcount == 0)
                {
                    is_Updated = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Type_Employement");

                throw;
            }

            return is_Updated;
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

        public bool CreateShift(Shift objshift)
        {
            bool IsValid = false;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabposshiftmst where upper(shiftname)='" + ManageQuote(objshift.Shiftname.ToUpper()) + "' and statusid=1;"));
                if (count == 0)
                {
                    string strShift = "INSERT INTO tabposshiftmst(shiftname, fromtime, totime, statusid, createdby, createddate) VALUES ('" + ManageQuote(objshift.Shiftname.ToUpper()) + "','" + ManageQuote(objshift.FromTime) + "','" + ManageQuote(objshift.ToTime) + "',1,1,current_timestamp); ";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strShift);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw ex;

            }
            finally
            {

            }
            return IsValid;
        }

        public List<Shift> ShowShiftDetails()
        {
            List<Shift> lstShiftdetails = new List<Shift>();
            string strShift = string.Empty;
            try
            {
                strShift = "select shiftid,shiftname,fromtime,totime from tabposshiftmst where statusid=1 order by shiftname;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strShift);
                while (npgdr.Read())
                {
                    Shift objShiftDTO = new Shift();
                    objShiftDTO.ShiftId = npgdr["shiftid"].ToString();
                    objShiftDTO.Shiftname = npgdr["shiftname"].ToString();
                    objShiftDTO.FromTime = npgdr["fromtime"].ToString();
                    objShiftDTO.ToTime = npgdr["totime"].ToString();
                    lstShiftdetails.Add(objShiftDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "City");
                throw;
            }

            return lstShiftdetails;
        }

        public bool UpdateShift(Shift objshift)
        {
            bool IsValid = false;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabposshiftmst where upper(shiftname)='" + ManageQuote(objshift.Shiftname.ToUpper()) + "' and statusid=1;"));
                if (count == 0)
                {
                    string strupdate = "UPDATE tabposshiftmst SET shiftname='" + ManageQuote(objshift.Shiftname.ToUpper()) + "',fromtime='" + ManageQuote(objshift.FromTime) + "',totime='" + ManageQuote(objshift.ToTime) + "', modifiedby=1,modifieddate=current_timestamp WHERE shiftid='" + ManageQuote(objshift.ShiftId) + "';";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdate);
                    IsValid = true;
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        public bool DeleteShift(Shift objshift)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "UPDATE tabposshiftmst SET statusid=2, modifiedby=1,modifieddate=current_timestamp WHERE shiftid='" + ManageQuote(objshift.ShiftId) + "';";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }

        public int GetShiftExist(int From, int To, string strShiftName)
        {
            int count = 0;
            string strData = string.Empty;
            try
            {
                strData = "select count(*) from tabposshiftmst where ( (" + From + " between ((EXTRACT( HOUR FROM  fromtime::time) * 60)+EXTRACT( MINUTES FROM  fromtime::time)) and ((EXTRACT( HOUR FROM  totime::time) * 60)+EXTRACT( MINUTES FROM  totime::time))) or (" + To + " between ((EXTRACT( HOUR FROM  fromtime::time) * 60)+EXTRACT( MINUTES FROM  fromtime::time)) and ((EXTRACT( HOUR FROM  totime::time) * 60)+EXTRACT( MINUTES FROM  totime::time)))) and statusid=1 ";
                count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strData));
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");
                throw;
            }

            return count;

        }

        public bool CheckShift(Shift objShift)
        {
            bool isSaved = false;
            int numcount1 = 0;

            try
            {
                numcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabposassignshift where shiftid='" + ManageQuote(objShift.ShiftId) + "' and statusid=1; "));

                if (numcount1 == 0)
                {
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Shift");

                trans.Rollback();
                isSaved = false;
                throw ex;
            }

            return isSaved;
        }

        #endregion

    }
}
