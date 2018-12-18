using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMS.Core;
using RMS.Core.Interfaces;
using Npgsql;
using HelperManager;
using System.Data;
using HRMS.Infrastructure;
using System.Globalization;

namespace RMS.Infrastructure
{
    public class EmployeeRepository : IEmployeeRepository
    {
        #region UserDecleration

        NpgsqlConnection con = null;
        NpgsqlTransaction trans;
        NpgsqlDataReader npgdr = null;
        string GNextID = string.Empty;
        string EmpRecordId = string.Empty;
        string NextEMPId = string.Empty;
        string UpdateEmpID = string.Empty;

        private string ManageQuote(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str != "" && str != string.Empty && str.Length > 0)
                {
                    str = str.Replace("'", "''");
                }
            }
            return str;
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
                Date = dat[2] + "-" + dat[1] + "-" + dat[0];
            }
            return Date;
        }

        #endregion

        /// <summary>
        /// Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Employee Personal Information

        public bool SaveBranchAllowance(PersonalInformation User, NpgsqlTransaction TRANS)
        {
            bool isSaved = false;
            string Gender = string.Empty;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                if (User.Gender == "Male")
                {
                    Gender = "M";
                }
                else
                {
                    Gender = "F";
                }
                if (String.IsNullOrEmpty(User.RFID))
                {
                    User.RFID = "0";
                }
                string marieddate = Convert.ToDateTime(User.DateOfMarage).ToString("dd/MM/yyyy");

                NextEMPId = BindEMPId();

                //  string strInsert = "INSERT INTO tabhrmsemployeepersonaldetails(vchemployeeid, vchname, vchsurname, datdob, numage,vchplaceofbirth, vchgender, vchmaritalstatus, datdateofmarriage, vchbloodgroup, vchmobilenumber, vchresidentialnumber, vchemail, vchbiometricid, vchrfid, vchimageurl, vchstatus, statusid, createdby, createddate)VALUES ( '" + NextEMPId + "', '" + ManageQuote(User.Name.ToUpper()) + "', '" + ManageQuote(User.Sname.ToUpper()) + "', '" + FormatDate(User.DOB) + "', " + Convert.ToInt64(User.Age) + ", '" + ManageQuote(User.PlaceOfBirth) + "', '" + Gender + "', '" + ManageQuote(User.MaritalStatus) + "','" + FormatDate(marieddate) + "', '" + ManageQuote(User.BloodGroup) + "', " + Convert.ToInt64(User.MobileNumber) + ", " + Convert.ToInt64(User.Res) + ", '" + ManageQuote(User.Email) + "', '" + ManageQuote(User.BioMetricId) + "', '" + ManageQuote(User.RFID) + "', '" + User.UploadUrl + "', '" + 'Y' + "', " + 1 + ", '" + 1 + "', Current_timestamp)returning recordid;";
                string strInsert = "INSERT INTO tabhrmsemployeepersonaldetails(vchemployeeid, vchname, vchsurname, datdob, numage,vchplaceofbirth, vchgender, vchmaritalstatus, datdateofmarriage, vchbloodgroup, vchmobilenumber, vchresidentialnumber, vchemail, vchbiometricid, vchrfid, vchimageurl, vchstatus, statusid, createdby, createddate)VALUES ( '" + NextEMPId + "', '" + ManageQuote(User.Name.ToUpper()) + "', '" + ManageQuote(User.Sname.ToUpper()) + "', '" + FormatDate(User.DOB) + "', " + Convert.ToInt64(User.Age) + ", '" + ManageQuote(User.PlaceOfBirth) + "', '" + Gender + "', '" + ManageQuote(User.MaritalStatus) + "','" + FormatDate(marieddate) + "', '" + ManageQuote(User.BloodGroup) + "', " + Convert.ToInt64(User.MobileNumber) + ", " + Convert.ToInt64(User.Res) + ", '" + ManageQuote(User.Email) + "', '" + ManageQuote(User.BioMetricId) + "', '" + ManageQuote(User.RFID) + "', '" + User.UploadUrl + "', '" + ManageQuote(User.PresentStatusOfEMP) + "', " + 1 + ", '" + 1 + "', Current_timestamp)returning recordid;";
                EmpRecordId = NPGSqlHelper.ExecuteScalar(TRANS, CommandType.Text, strInsert).ToString();
                isSaved = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Personal_Information");
                throw;

            }
            // finally
            // {
            //if (con.State == ConnectionState.Open)
            //{
            //    con.Close();
            //    con.Dispose();
            //    con.ClearPool();
            //}
            // }
            return isSaved;
        }

        public string BindEMPId()
        {
            string NextEMPId = string.Empty;
            try
            {
                string BranchCode = NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select vchbranchid from tabbranch;").ToString();
                string EMPMaxId = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select coalesce(vchemployeeid,'0') from(select substring(vchemployeeid FROM '[0-9]+')::int val,vchemployeeid from tabhrmsemployeepersonaldetails group by vchemployeeid order by val desc limit 1)x"));

                if (!string.IsNullOrEmpty(EMPMaxId))
                {

                    string[] words = EMPMaxId.Split(new[] { BranchCode }, StringSplitOptions.None);
                    long id = Convert.ToInt64(words[1]) + 1;
                    NextEMPId = BranchCode + id;
                }
                else
                {
                    NextEMPId = BranchCode + "1";
                }
                return NextEMPId;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Personal_Information");
                throw;
            }
        }

        public List<EmergencyContact> BindMaritalStatus()
        {
            List<EmergencyContact> lsMaritalStatus = new List<EmergencyContact>();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter("select recordid,vchmaritalstatus from tabhrmsmaritalstatusmst where statusid=1 order by vchmaritalstatus;", NPGSqlHelper.SQLConnString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    EmergencyContact objMaritalStatus = new EmergencyContact();

                    objMaritalStatus.Maritalstatus = dr["vchmaritalstatus"].ToString();
                    objMaritalStatus.MaritalstatusId = dr["recordid"].ToString();
                    lsMaritalStatus.Add(objMaritalStatus);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Personal_Information");
            }
            return lsMaritalStatus;
        }

        public List<EmergencyContact> BindBloodGroup()
        {
            List<EmergencyContact> lstBloodGroup = new List<EmergencyContact>();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter("select recordid,vchbloodgroup from tabhrmsbloodgroupmst where statusid=1 order by vchbloodgroup;", NPGSqlHelper.SQLConnString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    EmergencyContact objBloodGroup = new EmergencyContact();

                    objBloodGroup.BloodGroup = dr["vchbloodgroup"].ToString();
                    objBloodGroup.BloodGroupId = dr["recordid"].ToString();
                    lstBloodGroup.Add(objBloodGroup);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Personal_Information");
            }
            return lstBloodGroup;
        }
        #endregion


        /// <summary>
        /// Shubha 
        /// </summary>
        /// <param name="lstEmpAllowCheck"></param>
        /// <returns></returns>
        #region EmployeeAllowanceCheck

        public bool Save(EmpAllowanceCheck objEmpAllowCheck, List<EmpAllowanceCheck> lstEmpAllowCheck)
        {
            Boolean IsValid = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                trans = con.BeginTransaction();

                string strInsert = "INSERT INTO temptabhrmsemployeeallowancescheckdetails(employeeid, vchemployeeid, vchallowancename, numamount, statusid, createdby, createddate) select employeeid, vchemployeeid, vchallowancename, numamount, statusid, createdby, createddate from tabhrmsemployeeallowancescheckdetails where vchemployeeid =trim(split_part('" + ManageQuote(objEmpAllowCheck.vchemployeeid) + "','-',1))";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);

                strInsert = "DELETE FROM  tabhrmsemployeeallowancescheckdetails where vchemployeeid =trim(split_part('" + ManageQuote(objEmpAllowCheck.vchemployeeid) + "','-',1))";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);

                for (int i = 0; i < lstEmpAllowCheck.Count; i++)
                {
                    if (lstEmpAllowCheck[i].state)
                    {
                        strInsert = "INSERT INTO tabhrmsemployeeallowancescheckdetails(employeeid, vchemployeeid,vchallowancename, numamount, STATUSID, CREATEDBY, CREATEDDATE,modifiedby,modifieddate) VALUES (cast(trim(split_part('" + ManageQuote(objEmpAllowCheck.vchemployeeid) + "','-',2)) as bigint),trim(split_part('" + ManageQuote(objEmpAllowCheck.vchemployeeid) + "','-',1)),'" + ManageQuote(lstEmpAllowCheck[i].vchallowancename) + "','" + lstEmpAllowCheck[i].numamount + "'," + objEmpAllowCheck.statusid + "," + objEmpAllowCheck.createdby + ",current_timestamp,1,current_timestamp)";
                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                    }
                }
                trans.Commit();
                //cmd = new NpgsqlCommand(strInsert, con);
                IsValid = true;


            }
            catch (Exception ex)
            {
                trans.Rollback();
                EventLogger.WriteToErrorLog(ex, "EmployeeAlloweanceCheck");
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
            return IsValid;

        }

        public List<EmpAllowanceCheck> ShowEmployeeName()
        {
            List<EmpAllowanceCheck> lstemployeename = new List<EmpAllowanceCheck>();
            try
            {
                string str = "SELECT VCHEMPLOYEEID ||'-'||RECORDID as VCHEMPLOYEEID, UPPER(VCHNAME ||' ' || VCHSURNAME) AS VCHFULLNAME FROM TABHRMSEMPLOYEEPERSONALDETAILS where statusid=1  ORDER BY VCHFULLNAME";
                // string str = "SELECT tp.VCHEMPLOYEEID ||'-'||tp.RECORDID ||'-'||numbasic as VCHEMPLOYEEID, UPPER(VCHNAME ||' ' || VCHSURNAME) AS VCHFULLNAME FROM TABHRMSEMPLOYEEPERSONALDETAILS tp join tabhrmsemployeeemployment te on tp.VCHEMPLOYEEID=te.VCHEMPLOYEEID  where tp.statusid=1  ORDER BY VCHFULLNAME";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                while (npgdr.Read())
                {
                    EmpAllowanceCheck objEmpAllowanceCheckDTO = new EmpAllowanceCheck();
                    // objEmpAllowanceCheckDTO.employeeid = Convert.ToString(npgdr["employeeid"]);
                    objEmpAllowanceCheckDTO.vchemployeeid = npgdr["vchemployeeid"].ToString();
                    objEmpAllowanceCheckDTO.vchfullname = npgdr["VCHFULLNAME"].ToString();
                    lstemployeename.Add(objEmpAllowanceCheckDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmployeeAlloweanceCheck");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstemployeename;
        }

        public List<EmpAllowanceCheck> ShowDeductionEmployeeName()
        {
            List<EmpAllowanceCheck> lstemployeename = new List<EmpAllowanceCheck>();
            try
            {
                //string str = "SELECT VCHEMPLOYEEID ||'-'||RECORDID as VCHEMPLOYEEID, UPPER(VCHNAME ||' ' || VCHSURNAME) AS VCHFULLNAME FROM TABHRMSEMPLOYEEPERSONALDETAILS where statusid=1  ORDER BY VCHFULLNAME";
                string str = "SELECT tp.VCHEMPLOYEEID ||'-'||tp.RECORDID ||'-'||numbasic as VCHEMPLOYEEID, UPPER(VCHNAME ||' ' || VCHSURNAME) AS VCHFULLNAME FROM TABHRMSEMPLOYEEPERSONALDETAILS tp join tabhrmsemployeeemployment te on tp.VCHEMPLOYEEID=te.VCHEMPLOYEEID  where tp.statusid=1  ORDER BY VCHFULLNAME";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                while (npgdr.Read())
                {
                    EmpAllowanceCheck objEmpAllowanceCheckDTO = new EmpAllowanceCheck();
                    // objEmpAllowanceCheckDTO.employeeid = Convert.ToString(npgdr["employeeid"]);
                    objEmpAllowanceCheckDTO.vchemployeeid = npgdr["vchemployeeid"].ToString();
                    objEmpAllowanceCheckDTO.vchfullname = npgdr["VCHFULLNAME"].ToString();
                    lstemployeename.Add(objEmpAllowanceCheckDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmployeeAlloweanceCheck");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstemployeename;
        }

        public List<EmpAllowanceCheck> ShowEmployeeAllowenceDetails(string vchemployeeid)
        {
            List<EmpAllowanceCheck> lstemployeename = new List<EmpAllowanceCheck>();
            try
            {
                string str = "";
                if (vchemployeeid != string.Empty && vchemployeeid != null)
                {
                    str = "select state1 as state,vchallowancename1 as vchallowancename, numamount1 as numamount from fngetEmployeeallowancedetails(trim(split_part('" + ManageQuote(vchemployeeid) + "','-',1))) order by vchallowancename";
                }
                else
                {
                    str = "select false as state,cast ('VEHICLE ALLOWANCE' as varchar) as vchallowancename,0 as numamount union all select false as state,cast ('EDUCATION ALLOWANCE' as varchar) as vchallowancename,0 as numamount union all select false as state,cast ('LOYALTY ALLOWANCE' as varchar) as vchallowancename,0 as numamount union all select false as state,cast ('BRANCH ALLOWANCE' as varchar) as vchallowancename,0 as numamount union all select false as state ,vchallowancename,0 as numamount from tabhrmsotherallowancemst where  statusid=1 order by vchallowancename";
                }
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                while (npgdr.Read())
                {
                    EmpAllowanceCheck objEmpAllowanceCheckDTO = new EmpAllowanceCheck();
                    objEmpAllowanceCheckDTO.vchallowancename = Convert.ToString(npgdr["vchallowancename"]);
                    objEmpAllowanceCheckDTO.numamount = Convert.ToDecimal(npgdr["numamount"]);
                    objEmpAllowanceCheckDTO.state = Convert.ToBoolean(npgdr["state"]);
                    lstemployeename.Add(objEmpAllowanceCheckDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmployeeAlloweanceCheck");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstemployeename;
        }
        #endregion

        /// <summary>
        /// Venkatesh
        /// </summary>
        /// <returns></returns>
        #region Emergency Contact details
        public bool SaveEmergencyContactDetails(EmergencyContactDetails User, NpgsqlTransaction TRANS)
        {
            bool isSaved = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                string strInsert = "insert into tabhrmsemployeeemergencycontactdetails(employeeid,vchemployeeid,vchcontactpersonname,vchrelationship,vchcontactnumber,statusid,createdby,createddate)values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(User.ContactPersonName) + "','" + ManageQuote(User.RelationShip) + "'," + Convert.ToInt64(User.ContactNumber) + ",1,1,current_timestamp);";
                NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, strInsert);
                isSaved = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Contact_details");
                throw;
            }
            return isSaved;
        }

        public List<EmergencyContact> BindRelationShip()
        {
            List<EmergencyContact> lsRelationShip = new List<EmergencyContact>();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter("select recordid,vchrelationship from tabhrmsrelationshipmst where statusid=1 order by vchrelationship;", NPGSqlHelper.SQLConnString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    EmergencyContact objRelationShip = new EmergencyContact();

                    objRelationShip.RelationShip = dr["vchrelationship"].ToString();
                    objRelationShip.RelationShipId = dr["recordid"].ToString();
                    lsRelationShip.Add(objRelationShip);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Contact_details");
            }
            return lsRelationShip;
        }
        #endregion


        /// <summary>
        /// Naveen
        /// </summary>
        /// <returns></returns>
        #region Employee Family Details
        public List<EmployeeFamilydetails> getEmployeeid_family()
        {
            List<EmployeeFamilydetails> lstls = new List<EmployeeFamilydetails>();
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);



                NpgsqlDataAdapter da = new NpgsqlDataAdapter("select vchname,vchemployeeid||'-'||recordid as employeeid  from tabhrmsemployeepersonaldetails", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    EmployeeFamilydetails ld = new EmployeeFamilydetails();
                    ld.vchname = dr["vchname"].ToString();
                    ld.employeeid = dr["employeeid"].ToString();
                    lstls.Add(ld);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();
                con.ClearPool();
            }
            return lstls;
        }
        public List<occupationdetails> GetOccupation()
        {
            List<occupationdetails> lstoc = new List<occupationdetails>();
            try
            {
                string Query = "SELECT RECORDID,VCHOCCUPATION FROM TABHRMSOCCUPATIONMST WHERE STATUSID =1";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query);
                while (npgdr.Read())
                {
                    occupationdetails objoc = new occupationdetails();
                    objoc.recordid = Convert.ToInt32(npgdr["RECORDID"]);
                    objoc.vchoccupation = Convert.ToString(npgdr["VCHOCCUPATION"]);


                    lstoc.Add(objoc);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                throw;
            }

            return lstoc;
        }
        public List<RelationDetails> GetRelation()
        {
            List<RelationDetails> lstrl = new List<RelationDetails>();
            try
            {
                string Query = "SELECT RECORDID,VCHRELATIONSHIP FROM TABHRMSRELATIONSHIPMST WHERE STATUSID=1";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query);
                while (npgdr.Read())
                {
                    RelationDetails objrl = new RelationDetails();
                    objrl.recordid = Convert.ToInt32(npgdr["RECORDID"]);
                    objrl.vchrelationship = Convert.ToString(npgdr["VCHRELATIONSHIP"]);
                    lstrl.Add(objrl);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                throw;
            }

            return lstrl;
        }
        public bool CreateEmpFamilyDetails(List<EmployeeFamilydetails> lstEmployeeFamilydetails, NpgsqlTransaction TRANS)
        {
            bool isvalid = false;
            try
            {
                if (string.IsNullOrEmpty(EmpRecordId) && string.IsNullOrEmpty(EmpRecordId))
                {
                    EmpRecordId = lstEmployeeFamilydetails[0].employeeid;
                    NextEMPId = lstEmployeeFamilydetails[0].vchemployeeid;
                }
                for (int i = 0; i < lstEmployeeFamilydetails.Count; i++)
                {
                    if (string.IsNullOrEmpty(lstEmployeeFamilydetails[i].datdob))
                    {
                        lstEmployeeFamilydetails[i].datdob = null;
                    }

                    string Query = "INSERT INTO TABHRMSEMPLOYEEFAMILYDETAILS(EMPLOYEEID,VCHEMPLOYEEID,VCHPERSONNAME,VCHRELATIONSHIP,VCHISNOMINEE,VCHOCCUPATION,VCHMOBILENUMBER,DATDOB,STATUSID,CREATEDBY,CREATEDDATE) VALUES(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(lstEmployeeFamilydetails[i].vchpersonname) + "','" + ManageQuote(lstEmployeeFamilydetails[i].vchrelationship) + "',CASE WHEN UPPER('False')=UPPER('" + lstEmployeeFamilydetails[i].vchisnominee + "') THEN 0 ELSE 1 END,'" + ManageQuote(lstEmployeeFamilydetails[i].vchoccupation) + "','" + ManageQuote(lstEmployeeFamilydetails[i].vchmobilenumber) + "','" + FormatDate(lstEmployeeFamilydetails[i].datdob) + "',1,1,CURRENT_TIMESTAMP) returning recordid";
                    int reccordid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(TRANS, CommandType.Text, Query));
                    if (lstEmployeeFamilydetails[i].vcheducationlevel != null)
                    {
                        Query = " insert into tabhrmsemployeefamilyeducationdetails (employeeid,employeefamilyid,vchemployeeid,vchpersonname,vcheducationlevel,statusid,createdby,createddate) values(" + EmpRecordId + "," + reccordid + ",'" + NextEMPId + "','" + ManageQuote(lstEmployeeFamilydetails[i].vchpersonname) + "','" + ManageQuote(lstEmployeeFamilydetails[i].vcheducationlevel) + "',1,1,CURRENT_TIMESTAMP)";
                        NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, Query);
                    }
                }
                isvalid = true;
                //trans.Commit();
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                // trans.Rollback();
                isvalid = false;

            }
            //finally
            //{
            //    if (con.State == ConnectionState.Open)
            //        con.Close();
            //}
            return isvalid;
        }
        public List<EmployeeFamilydetails> GetFamilyDetails()
        {
            List<EmployeeFamilydetails> lstempf = new List<EmployeeFamilydetails>();
            try
            {
                string Query = " SELECT tf.RECORDID,(case when upper(vchrelationship)='SON' or upper(vchrelationship)='DAUGHTER' then (select recordid from tabhrmsemployeefamilyeducationdetails where employeefamilyid::int=tf.recordid) else '0'  end) as drecordid,(case when upper(vchrelationship)='SON' or upper(vchrelationship)='DAUGHTER' then (select vcheducationlevel from tabhrmsemployeefamilyeducationdetails where employeefamilyid::int=tf.recordid) else ' '  end) as vcheducationlevel,tf.VCHEMPLOYEEID||'-'||tf.EMPLOYEEID as EMPLOYEEID,tf.VCHEMPLOYEEID,tf.VCHPERSONNAME,VCHRELATIONSHIP,CASE WHEN  VCHISNOMINEE='1' THEN 'TRUE' ELSE 'FALSE' END AS VCHISNOMINEE,VCHOCCUPATION,VCHMOBILENUMBER,DATDOB::text,tf.statusid FROM TABHRMSEMPLOYEEFAMILYDETAILS tf  where tf.statusid=1";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query);
                while (npgdr.Read())
                {
                    EmployeeFamilydetails objempf = new EmployeeFamilydetails();
                    objempf.recordid = Convert.ToString(npgdr["RECORDID"]);
                    objempf.drecordid = Convert.ToString(npgdr["drecordid"]);
                    objempf.employeeid = Convert.ToString(npgdr["EMPLOYEEID"]);
                    objempf.vchemployeeid = Convert.ToString(npgdr["VCHEMPLOYEEID"]);
                    objempf.vchpersonname = Convert.ToString(npgdr["VCHPERSONNAME"]);
                    objempf.vchrelationship = Convert.ToString(npgdr["VCHRELATIONSHIP"]);
                    objempf.vchisnominee = Convert.ToString(npgdr["VCHISNOMINEE"]);
                    objempf.vchoccupation = Convert.ToString(npgdr["VCHOCCUPATION"]);
                    objempf.vchmobilenumber = Convert.ToString(npgdr["VCHMOBILENUMBER"]);
                    objempf.datdob = Convert.ToString(npgdr["DATDOB"]);
                    objempf.vcheducationlevel = Convert.ToString(npgdr["vcheducationlevel"]);
                    lstempf.Add(objempf);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }

            return lstempf;
        }
        public List<EmployeeFamilydetails> GetChildrenDetails()
        {
            List<EmployeeFamilydetails> lstempc = new List<EmployeeFamilydetails>();
            try
            {
                string Query = "SELECT VCHEMPLOYEEID,VCHPERSONNAME FROM TABHRMSEMPLOYEEFAMILYDETAILS WHERE VCHEMPLOYEEID ='EKIT147' AND TRIM(VCHRELATIONSHIP) IN('SON','DAUGHTER') AND STATUSID=1";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query);
                while (npgdr.Read())
                {
                    EmployeeFamilydetails objempc = new EmployeeFamilydetails();
                    objempc.vchemployeeid = Convert.ToString(npgdr["VCHEMPLOYEEID"]);
                    objempc.vchpersonname = Convert.ToString(npgdr["VCHPERSONNAME"]);
                    lstempc.Add(objempc);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstempc;
        }
        public bool UpdateFamilyDetails(EmployeeFamilydetails objFamilyDetails)
        {
            bool isvalid = true;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                if (objFamilyDetails.vchisnominee.ToUpper() == "FALSE")
                    objFamilyDetails.vchisnominee = "0";
                else
                    objFamilyDetails.vchisnominee = "1";

                string Query = "UPDATE TABHRMSEMPLOYEEFAMILYDETAILS SET VCHPERSONNAME='" + ManageQuote(objFamilyDetails.vchpersonname) + "' ,VCHRELATIONSHIP='" + ManageQuote(objFamilyDetails.vchrelationship) + "', VCHISNOMINEE='" + objFamilyDetails.vchisnominee + "' , VCHOCCUPATION='" + ManageQuote(objFamilyDetails.vchoccupation) + "', VCHMOBILENUMBER='" + ManageQuote(objFamilyDetails.vchmobilenumber) + "', DATDOB='" + FormatDate(objFamilyDetails.datdob) + "', MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID=" + objFamilyDetails.recordid + ";";
                //string Query = UPDATE TABHRMSEMPLOYEEFAMILYDETAILS SET VCHPERSONNAME='Krishna' ,VCHRELATIONSHIP='SON IN LAW', VCHISNOMINEE=CASE WHEN UPPER('False')=UPPER('TRUE') THEN 0 ELSE 1 END ,VCHOCCUPATION='Krishna' ,VCHMOBILENUMBER='Krishna', MODIFIEDBY=1 ,MODIFIEDDATE=CURRENT_TIMESTAMP WHERE  VCHEMPLOYEEID='EKIT147' AND RECORDID=
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                if (objFamilyDetails.vcheducationlevel != null)
                {
                    Query = "UPDATE tabhrmsemployeefamilyeducationdetails SET VCHPERSONNAME='" + ManageQuote(objFamilyDetails.vchpersonname) + "' ,vcheducationlevel='" + ManageQuote(objFamilyDetails.vcheducationlevel) + "', MODIFIEDBY=1, MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID=" + objFamilyDetails.drecordid + ";";

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                }

                trans.Commit();
                isvalid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                trans.Rollback();
                isvalid = false;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return isvalid;
        }
        public bool DeleteFamilyDetails(string recordid, string drecordid)
        {
            bool isvalid = true;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string Query = "update   TABHRMSEMPLOYEEFAMILYDETAILS  set statusid=2 where recordid=" + recordid + "; ";
                //string Query = UPDATE TABHRMSEMPLOYEEFAMILYDETAILS SET VCHPERSONNAME='Krishna' ,VCHRELATIONSHIP='SON IN LAW', VCHISNOMINEE=CASE WHEN UPPER('False')=UPPER('TRUE') THEN 0 ELSE 1 END ,VCHOCCUPATION='Krishna' ,VCHMOBILENUMBER='Krishna', MODIFIEDBY=1 ,MODIFIEDDATE=CURRENT_TIMESTAMP WHERE  VCHEMPLOYEEID='EKIT147' AND RECORDID=
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);

                Query = "update   tabhrmsemployeefamilyeducationdetails  set statusid=2 where recordid=" + drecordid + "; ";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);


                trans.Commit();
                isvalid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                trans.Rollback();
                isvalid = false;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return isvalid;
        }
        public int DeleteEducationDetailsResult(EmployeeFamilydetails objFamilyDetails)
        {
            int status = 0;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}
                //string Query="
                //NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, str);

                status = 1;
                //}

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                status = 0;
                // throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                //if (con.State == ConnectionState.Open)
                //{
                //    con.Close();
                //}
                //con.Dispose();
                //con.ClearPool();
                //trans.Dispose();
            }
            return status;
        }


        public List<FamilyEducationDetails> GetEducationDetails()
        {
            List<FamilyEducationDetails> lstoc = new List<FamilyEducationDetails>();
            try
            {
                string Query = "select vcheducationlevel,vchdescription from tabhrmseducationallowancemst where statusid=1   ; ";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query);
                while (npgdr.Read())
                {
                    FamilyEducationDetails objoc = new FamilyEducationDetails();
                    objoc.vcheducationlevel = Convert.ToString(npgdr["vcheducationlevel"]);
                    objoc.vchdescription = Convert.ToString(npgdr["vchdescription"]);


                    lstoc.Add(objoc);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");

                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstoc;
        }
        #endregion



        /// <summary>
        /// Vijay kandimalla
        /// </summary>
        #region Addresses

        string StrInsertOne = string.Empty;
        string StrInsertTwo = string.Empty;
        string strSelect = string.Empty;
        List<Addresses> objlstloademployees = new List<Addresses>();
        DataSet ds = null;
        DataTable dt = null;


        public List<City> ShowCities(string strSateid)
        {
            List<City> lstStates = new List<City>();
            try
            {

                string strcity = "select cityid,cityname from tabcity where stateid=" + strSateid + " and statusid=1  order by cityname;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcity);
                while (npgdr.Read())
                {
                    City objstate = new City();

                    objstate.CityId = npgdr["cityid"].ToString();
                    objstate.CityName = npgdr["cityname"].ToString();

                    lstStates.Add(objstate);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Addresses");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstStates;
        }

        //public bool SaveEmployee(Addresses objAddresses, NpgsqlTransaction TRANS)
        //{
        //    bool IsSaved = false;
        //    try
        //    {
        //        //string[] dates = objAddresses.vchemployeeid.Split('-');
        //        //string recordid = dates[0];
        //        //string vchemployeeid = dates[1];
        //        if (objAddresses.DoorNo != string.Empty && objAddresses.Area != null && objAddresses.Country != null && objAddresses.State != null && objAddresses.CityName != null && objAddresses.PinCode != null || objAddresses.StreetName != null)
        //        {
        //            StrInsertOne = "insert into tabhrmsemployeeaddressdetails(employeeid,vchemployeeid,vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,vchaddresstype,statusid,createdby,createddate,countryid,stateid) values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(objAddresses.DoorNo) + "','" + ManageQuote(objAddresses.StreetName) + "','" + ManageQuote(objAddresses.Area) + "','" + ManageQuote(objAddresses.Country) + "','" + ManageQuote(objAddresses.State) + "','" + objAddresses.CityName + "','" + ManageQuote(objAddresses.PinCode) + "','CON',1,1,current_date," + objAddresses.CountryId + "," + objAddresses.StateId + ");";
        //            int i = NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, StrInsertOne);
        //            if (i > 0)
        //            {
        //                if (objAddresses.dDoorNo != null && objAddresses.dArea != null && objAddresses.CountryId != null && objAddresses.StateId != null && objAddresses.CityId != null && objAddresses.dPinCode != null || objAddresses.dStreetName != null)
        //                {
        //                    StrInsertTwo = "insert into tabhrmsemployeeaddressdetails(employeeid,vchemployeeid,vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,vchaddresstype,statusid,createdby,createddate,countryid,stateid) values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(objAddresses.dDoorNo) + "','" + ManageQuote(objAddresses.dStreetName) + "','" + ManageQuote(objAddresses.dArea) + "','" + ManageQuote(objAddresses.Country) + "','" + ManageQuote(objAddresses.State) + "','" + ManageQuote(objAddresses.CityName) + "','" + ManageQuote(objAddresses.dPinCode) + "','PER',1,1,current_date," + objAddresses.dCountryId + "," + objAddresses.dStateId + ");";
        //                    int j = NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, StrInsertTwo);
        //                    if (j > 0)
        //                    {
        //                    }
        //                    else
        //                    {
        //                        IsSaved = false;
        //                    }
        //                }
        //                IsSaved = true;
        //            }
        //            else
        //            {
        //                IsSaved = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "Addresses");
        //    }
        //    return IsSaved;
        //}
        public bool SaveEmployee(Addresses objAddresses, NpgsqlTransaction TRANS)
        {
            bool IsSaved = false;
            try
            {
                int i = 0;
                int j = 0;
                if ((!string.IsNullOrEmpty(objAddresses.DoorNo)) || (!string.IsNullOrEmpty(objAddresses.Area)) || (!string.IsNullOrEmpty(objAddresses.Country)) || (!string.IsNullOrEmpty(objAddresses.State)) || (!string.IsNullOrEmpty(objAddresses.CityName)) || (!string.IsNullOrEmpty(objAddresses.PinCode)) || (!string.IsNullOrEmpty(objAddresses.StreetName)))
                {
                    if (objAddresses.SameOrNot)
                    {
                        StrInsertOne = "insert into tabhrmsemployeeaddressdetails(employeeid,vchemployeeid,vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,vchaddresstype,statusid,createdby,createddate,countryid,stateid) values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(objAddresses.DoorNo) + "','" + ManageQuote(objAddresses.StreetName) + "','" + ManageQuote(objAddresses.Area) + "','" + ManageQuote(objAddresses.Country) + "','" + ManageQuote(objAddresses.State) + "','" + objAddresses.CityName + "','" + ManageQuote(objAddresses.PinCode) + "','CON',1,1,current_date," + objAddresses.CountryId + "," + objAddresses.StateId + ");";
                        i = NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, StrInsertOne);
                        StrInsertTwo = "insert into tabhrmsemployeeaddressdetails(employeeid,vchemployeeid,vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,vchaddresstype,statusid,createdby,createddate,countryid,stateid) values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(objAddresses.DoorNo) + "','" + ManageQuote(objAddresses.StreetName) + "','" + ManageQuote(objAddresses.Area) + "','" + ManageQuote(objAddresses.Country) + "','" + ManageQuote(objAddresses.State) + "','" + objAddresses.CityName + "','" + ManageQuote(objAddresses.PinCode) + "','PER',1,1,current_date," + objAddresses.CountryId + "," + objAddresses.StateId + ");";
                        j = NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, StrInsertTwo);

                    }
                    else
                    {
                        if ((!string.IsNullOrEmpty(objAddresses.DoorNo)) || (!string.IsNullOrEmpty(objAddresses.Area)) || (!string.IsNullOrEmpty(objAddresses.Country)) || (!string.IsNullOrEmpty(objAddresses.State)) || (!string.IsNullOrEmpty(objAddresses.CityName)) || (!string.IsNullOrEmpty(objAddresses.PinCode)) || (!string.IsNullOrEmpty(objAddresses.StreetName)))
                        {
                            StrInsertOne = "insert into tabhrmsemployeeaddressdetails(employeeid,vchemployeeid,vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,vchaddresstype,statusid,createdby,createddate,countryid,stateid) values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(objAddresses.DoorNo) + "','" + ManageQuote(objAddresses.StreetName) + "','" + ManageQuote(objAddresses.Area) + "','" + ManageQuote(objAddresses.Country) + "','" + ManageQuote(objAddresses.State) + "','" + objAddresses.CityName + "','" + ManageQuote(objAddresses.PinCode) + "','CON',1,1,current_date," + objAddresses.CountryId + "," + objAddresses.StateId + ");";
                            i = NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, StrInsertOne);
                        }
                        if (objAddresses.dDoorNo != null && objAddresses.dArea != null && objAddresses.CountryId != null && objAddresses.StateId != null && objAddresses.CityId != null && objAddresses.dPinCode != null || objAddresses.dStreetName != null)
                        {
                            StrInsertTwo = "insert into tabhrmsemployeeaddressdetails(employeeid,vchemployeeid,vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,vchaddresstype,statusid,createdby,createddate,countryid,stateid) values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(objAddresses.dDoorNo) + "','" + ManageQuote(objAddresses.dStreetName) + "','" + ManageQuote(objAddresses.dArea) + "','" + ManageQuote(objAddresses.Country) + "','" + ManageQuote(objAddresses.State) + "','" + ManageQuote(objAddresses.CityName) + "','" + ManageQuote(objAddresses.dPinCode) + "','PER',1,1,current_date," + objAddresses.dCountryId + "," + objAddresses.dStateId + ");";
                            j = NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, StrInsertTwo);
                        }

                    }
                    if (i > 0 && j > 0)
                    {
                        IsSaved = true;
                    }

                }
                else
                {
                    IsSaved = true;
                }



            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Addresses");

            }
            return IsSaved;
        }

        public List<Addresses> LoadEmployeeNames()
        {
            try
            {
                strSelect = "select (recordid||'-'||vchemployeeid) as recordidempid,vchname from tabhrmsemployeepersonaldetails order by  vchname;";
                ds = new DataSet();
                dt = new DataTable();


                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strSelect);
                dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Addresses objAddresses = new Addresses();
                    //objAddresses.recordid = dt.Rows[i]["recordid"].ToString();
                    objAddresses.vchemployeeid = dt.Rows[i]["recordidempid"].ToString();
                    objAddresses.vchname = dt.Rows[i]["vchname"].ToString();
                    objlstloademployees.Add(objAddresses);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Addresses");
            }
            return objlstloademployees;
        }


        #endregion

        /// <summary>
        /// Srinath
        /// </summary>
        /// <returns></returns>
        #region EducationDetails Srinath

        /// <summary>
        /// Binding EmployeeId tO Employeeid Dropdownlist
        /// </summary>
        /// <returns></returns>

        public List<EducationDetails> GetEmployeeiddetails()
        {

            List<EducationDetails> lstPersonal = new List<EducationDetails>();
            try
            {


                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);



                NpgsqlDataAdapter da = new NpgsqlDataAdapter("select vchname,vchemployeeid||'-'||recordid as employeeid  from tabhrmsemployeepersonaldetails", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    EducationDetails objPersonalDetails = new EducationDetails();


                    objPersonalDetails.EmployeeName = dr["vchname"].ToString();
                    objPersonalDetails.Employeeid = dr["employeeid"].ToString();


                    lstPersonal.Add(objPersonalDetails);

                }









            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EducationDetails");

            }
            return lstPersonal;

            //List<EducationDetails> lstPersonal = new List<EducationDetails>();
            //try
            //{

            //    string Strpersonalmst = "select recordid,vchemployeeid from tabhrmsemployeepersonaldetails";


            //    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Strpersonalmst);
            //    while (npgdr.Read())
            //    {
            //        EducationDetails objPersonalDetails = new EducationDetails();

            //        objPersonalDetails.EmployeeRecordid = npgdr["recordid"].ToString();

            //        objPersonalDetails.Employeeid = npgdr["vchemployeeid"].ToString();



            //        lstPersonal.Add(objPersonalDetails);
            //    }
            //}
            //catch (Exception ex)
            //{


            //}
            //return lstPersonal;
        }

        /// <summary>
        /// Getting Groups Based On Course
        /// </summary>
        /// <param name="CourseName"></param>
        /// <returns></returns>


        public List<Group> GetGroups(string CourseName)
        {
            List<Group> lst = new List<Group>();
            try
            {

                string Strcoursemst = "select recordid,vchcourse,vchgroup  from tabhrmsgroupmst where vchcourse='" + CourseName + "' where statusid=1";


                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Strcoursemst);
                while (npgdr.Read())
                {
                    Group objGroupDetails = new Group();

                    objGroupDetails.Recordid = npgdr["recordid"].ToString();

                    objGroupDetails.CourseName = npgdr["vchcourse"].ToString();

                    objGroupDetails.GroupName = npgdr["vchgroup"].ToString();



                    lst.Add(objGroupDetails);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "CourseName");

            }
            finally
            {
                npgdr.Dispose();
            }
            return lst;
        }

        /// <summary>
        /// Create Education Details
        /// </summary>
        /// <param name="EducationDetails"></param>
        /// <returns></returns>

        public bool CreateEducationDetails(List<EducationDetails> lstEducationDetails, NpgsqlTransaction TRANS)
        {

            bool isSaved = false;
            try
            {
                string str = string.Empty;
                //string date = System.DateTime.Now.ToString("dd-MM-yyyy");


                for (int i = 0; i < lstEducationDetails.Count; i++)
                {
                    str = "insert into tabhrmsemployeeeducationdetails(employeeid,vchemployeeid,vchcourse,vchgroup,vchschoolorcollege,vchplace,numyear,numpercentageofmarks,statusid,createdby,createddate ) values (" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(lstEducationDetails[i].Course) + "' ,'" + ManageQuote(lstEducationDetails[i].Group) + "','" + ManageQuote(lstEducationDetails[i].SchoolorCollege) + "','" + ManageQuote(lstEducationDetails[i].Place) + "','" + Convert.ToDouble(lstEducationDetails[i].Year) + "','" + Convert.ToDouble(lstEducationDetails[i].MarksPercentage) + "',1,1,current_timestamp);";

                    //str = "insert into tabhrmsemployeeeducationdetails(vchemployeeid,vchcourse,vchgroup,vchschoolorcollege,vchplace,numyear,numpercentageofmarks,statusid,createdby,createddate ) values ('" + ManageQuote(EducationDetails.Employeeid) + "','" + ManageQuote(EducationDetails.Course) + "' ,'" + ManageQuote(EducationDetails.Group) + "','" + ManageQuote(EducationDetails.SchoolorCollege) + "','" + ManageQuote(EducationDetails.Place) + "','" + Convert.ToDouble(EducationDetails.Year) + "','" + Convert.ToDouble(EducationDetails.MarksPercentage) + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, str);
                    isSaved = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EducationDetails");
                // status = 0;

                // throw ex;
                //ErrorMessage(ex);
            }
            //finally
            //{
            //    if (con.State == ConnectionState.Open)
            //    {
            //        con.Close();
            //        con.Dispose();
            //        con.ClearPool();
            //    }
            //}
            return isSaved;
        }

        /// <summary>
        /// Binding EducationDetails To Grid
        /// </summary>
        /// <returns></returns>

        public List<EducationDetails> ShowEducationDetails()
        {
            List<EducationDetails> lst = new List<EducationDetails>();
            try
            {

                string strgeteducationdetails = "select recordid, vchemployeeid||'-'||employeeid as vchemployeeid,vchcourse,vchgroup,vchschoolorcollege,vchplace,numyear,numpercentageofmarks  from tabhrmsemployeeeducationdetails";


                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strgeteducationdetails);
                while (npgdr.Read())
                {
                    EducationDetails objEducationDetails = new EducationDetails();

                    objEducationDetails.recordid = npgdr["recordid"].ToString();

                    objEducationDetails.Employeeid = npgdr["vchemployeeid"].ToString();

                    objEducationDetails.Course = npgdr["vchcourse"].ToString();

                    objEducationDetails.Group = npgdr["vchgroup"].ToString();

                    objEducationDetails.SchoolorCollege = npgdr["vchschoolorcollege"].ToString();
                    objEducationDetails.Place = npgdr["vchplace"].ToString();

                    objEducationDetails.Year = Convert.ToDouble(npgdr["numyear"]);

                    objEducationDetails.MarksPercentage = Convert.ToDouble(npgdr["numpercentageofmarks"]);

                    lst.Add(objEducationDetails);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EducationDetails");

            }
            finally
            {
                npgdr.Dispose();
            }
            return lst;
        }

        /// <summary>
        /// Getting All courses from Database
        /// </summary>
        /// <returns></returns>

        public List<Course> GetcourseDetails()
        {
            List<Course> lst = new List<Course>();
            try
            {

                string Strcoursemst = "select recordid,vchcourse  from tabhrmscoursemst where statusid=1";


                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Strcoursemst);
                while (npgdr.Read())
                {
                    Course objCourseDetails = new Course();

                    objCourseDetails.recordid = npgdr["recordid"].ToString();

                    objCourseDetails.GetCousrse = npgdr["vchcourse"].ToString();



                    lst.Add(objCourseDetails);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "EducationDetails");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lst;
        }

        /// <summary>
        /// Getting All Groups
        /// </summary>
        /// <returns></returns>

        public List<Group> GetGroupDetails()
        {
            List<Group> lst = new List<Group>();
            try
            {

                string Strcoursemst = "select recordid,vchcourse,vchgroup  from tabhrmsgroupmst where statusid=1";


                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Strcoursemst);
                while (npgdr.Read())
                {
                    Group objGroupDetails = new Group();

                    objGroupDetails.Recordid = npgdr["recordid"].ToString();

                    objGroupDetails.CourseName = npgdr["vchcourse"].ToString();

                    objGroupDetails.GroupName = npgdr["vchgroup"].ToString();



                    lst.Add(objGroupDetails);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EducationDetails");

            }
            finally
            {
                npgdr.Dispose();
            }
            return lst;
        }





        /// <summary>
        /// Deleting Education Details Based On EmployeeId
        /// </summary>
        /// <param name="Employeeid"></param>
        /// <returns></returns>



        public int DeleteEducationDetailsResult(string Employeeid)
        {
            int status = 0;
            try
            {
                string str = string.Empty;


                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }





                str = "delete from tabhrmsemployeeeducationdetails where recordid=" + Employeeid + "";
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, str);




                status = 1;
                //}


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EducationDetails");
                status = 0;
                // throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                }
            }
            return status;
        }

        /// <summary>
        /// Updating Education Details Based On EmployeeId
        /// </summary>
        /// <param name="EducationDetails"></param>
        /// <returns></returns>

        public int UpdateEducationDetails(EducationDetails EducationDetails)
        {
            int status = 0;
            try
            {
                string str = string.Empty;


                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }



                // str = "update tabhrmsemployeeeducationdetails set vchcourse='" + ManageQuote(EducationDetails.Course) + "',vchgroup='" + ManageQuote(EducationDetails.Group) + "',vchplace='" + ManageQuote(EducationDetails.Place) + "',numyear='" + EducationDetails.Year + "',numpercentageofmarks='" + EducationDetails.MarksPercentage + "',vchschoolorcollege='" + ManageQuote(EducationDetails.SchoolorCollege) + "' where vchemployeeid='" + EducationDetails.Employeeid + "'";

                str = "update tabhrmsemployeeeducationdetails set vchcourse='" + ManageQuote(EducationDetails.Course) + "',vchgroup='" + ManageQuote(EducationDetails.Group) + "',vchplace='" + ManageQuote(EducationDetails.Place) + "',numyear='" + EducationDetails.Year + "',numpercentageofmarks='" + EducationDetails.MarksPercentage + "',vchschoolorcollege='" + ManageQuote(EducationDetails.SchoolorCollege) + "' where recordid=" + EducationDetails.recordid + "";
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, str);




                status = 1;
                //}


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EducationDetails");
                status = 0;
                // throw ex;
                //ErrorMessage(ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                }
            }
            return status;
        }


        #endregion


        /// <summary> 
        /// Santosh
        /// </summary>
        /// <returns></returns>
        #region Training and Certification

        public List<employeeCourses> ShowCertificatesCourses(string ID)
        {
            List<employeeCourses> lstpf = new List<employeeCourses>();
            try
            {
                string Query = "";
                if (ID != null && ID != string.Empty)
                {
                    Query = " select  recordid,vchemployeeid||'-'||employeeid as vchemployeeid,(select vchname||' '||vchsurname from tabhrmsemployeepersonaldetails  where vchemployeeid=t.vchemployeeid  ) as vchname,vchcertificateorcourse ,vchcertificateorcourse as vchcertificateorcourse1,to_char(datfromdate,'dd-Mon-yyyy') as datfromdate,to_char(dattodate,'dd-Mon-yyyy') as dattodate ,createdby from tabhrmsemployeecertificationdetails t where statusid=1 and vchemployeeid||'-'||employeeid='" + ID + "';";
                }
                else
                {
                    Query = "  select  recordid,vchemployeeid||'-'||employeeid as vchemployeeid,(select vchname||' '||vchsurname from tabhrmsemployeepersonaldetails  where vchemployeeid=t.vchemployeeid  ) as vchname,vchcertificateorcourse ,vchcertificateorcourse as vchcertificateorcourse1,to_char(datfromdate,'dd-Mon-yyyy') as datfromdate,to_char(dattodate,'dd-Mon-yyyy') as dattodate ,createdby from tabhrmsemployeecertificationdetails t  where statusid=1 ;";
                }
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, Query);
                while (npgdr.Read())
                {
                    employeeCourses objpf = new employeeCourses();
                    objpf.recordid = Convert.ToString(npgdr["recordid"]);
                    objpf.vchname = Convert.ToString(npgdr["vchname"]);
                    objpf.EmployeeID = Convert.ToString(npgdr["vchemployeeid"]);
                    objpf.vchcertificateorcourse = Convert.ToString(npgdr["vchcertificateorcourse"]);
                    objpf.vchcertificateorcourse1 = Convert.ToString(npgdr["vchcertificateorcourse1"]);
                    objpf.datfromdate = Convert.ToString(npgdr["datfromdate"]);
                    objpf.dattodate = Convert.ToString(npgdr["dattodate"]);
                    objpf.CreatedBy = npgdr["createdby"].ToString();

                    lstpf.Add(objpf);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Training_Course");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstpf;
        }

        public bool DeleteCertificateCourse(string employeeid, string coursename, string recordid)
        {
            bool isvalid = false;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);


                string Query = "update    tabhrmsemployeecertificationdetails set statusid=2 where recordid=" + recordid + ";";//vchemployeeid='"+employeeid +"' and vchcertificateorcourse='"+coursename +"';";
                NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, Query);
                isvalid = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Training_Course");
                //trans.Rollback();
                isvalid = false;
                throw;
            }
            return isvalid;
        }

        public int SaveCertificateCourse(employeeCourses ecs)
        {
            int isvalid = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                trans = con.BeginTransaction();
                string Query = "select count(*) from tabhrmsemployeecertificationdetails where vchemployeeid ='" + ecs.vchemployeeid + "' and upper(vchcertificateorcourse)='" + ManageQuote(ecs.vchcertificateorcourse).ToUpper() + "' and statusid=1 ;";
                int tcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, Query));
                Query = "select count(*) from tabhrmsemployeecertificationdetails where vchemployeeid ='" + ecs.vchemployeeid + "' and upper(vchcertificateorcourse)='" + ManageQuote(ecs.vchcertificateorcourse).ToUpper() + "' and statusid=2 ;";
                int tcount1 = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, Query));

                if (tcount <= 0 && tcount1 <= 0)
                {
                    Query = "insert into tabhrmsemployeecertificationdetails(employeeid,vchemployeeid,vchcertificateorcourse,datfromdate,dattodate,statusid,createdby,createddate)";
                    Query += "values(" + ecs.EmployeeID.Split('-')[01].Replace(" ", "") + ",'" + ecs.EmployeeID.Split('-')[0].Replace(" ", "") + "','" + ManageQuote(ecs.vchcertificateorcourse.ToUpper()) + "','" + FormatDate(ecs.datfromdate) + "','" + FormatDate(ecs.dattodate) + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                    isvalid = 1;
                }
                else if (tcount1 > 0)
                {

                    Query = "update   tabhrmsemployeecertificationdetails set statusid=1,datfromdate='" + FormatDate(ecs.datfromdate) + "',dattodate='" + FormatDate(ecs.dattodate) + "',modifieddate=current_timestamp where upper( vchcertificateorcourse)='" + ecs.vchcertificateorcourse.ToUpper() + "'  and vchemployeeid='" + ecs.vchemployeeid + "'; ";

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                    isvalid = 2;

                }
                else if (tcount > 0)
                {
                    isvalid = 3;
                }
                trans.Commit();


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Training_Course");
                trans.Rollback();
                isvalid = 0;
                throw;
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
            return isvalid;
        }

        public int UpdateCertificateCourse(employeeCourses ecs)
        {
            int isvalid = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);

                string Query = "select count(*) from tabhrmsemployeecertificationdetails where vchemployeeid ='" + ecs.vchemployeeid + "' and upper(vchcertificateorcourse)='" + ManageQuote(ecs.vchcertificateorcourse).ToUpper() + "' and statusid=1 ;";
                int tcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(con, CommandType.Text, Query));


                if (tcount == 1 && ecs.vchcertificateorcourse.ToUpper() == ecs.vchcertificateorcourse1.ToUpper())
                {
                    Query = "update   tabhrmsemployeecertificationdetails set vchcertificateorcourse='" + ManageQuote(ecs.vchcertificateorcourse.ToUpper()) + "',datfromdate='" + FormatDate(ecs.datfromdate) + "',dattodate='" + FormatDate(ecs.dattodate) + "',modifieddate=current_timestamp where recordid=" + ecs.recordid + " ";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, Query);
                    isvalid = 1;
                }
                else if (tcount == 1 && ecs.vchcertificateorcourse.ToUpper() != ecs.vchcertificateorcourse1.ToUpper())
                {

                    isvalid = 3;
                }
                else if (tcount == 0)
                {
                    Query = "update   tabhrmsemployeecertificationdetails set vchcertificateorcourse='" + ManageQuote(ecs.vchcertificateorcourse.ToUpper()) + "',datfromdate='" + FormatDate(ecs.datfromdate) + "',dattodate='" + FormatDate(ecs.dattodate) + "',modifieddate=current_timestamp where recordid=" + ecs.recordid + " ";
                    NPGSqlHelper.ExecuteNonQuery(con, CommandType.Text, Query);
                    isvalid = 1;
                }







            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Training_Course");
                //trans.Rollback();
                isvalid = 0;
                throw;
            }
            return isvalid;
        }



        public bool SaveCertificateCourse(List<employeeCourses> ecs, NpgsqlTransaction TRANS)
        {
            bool isvalid = false;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                //trans = con.BeginTransaction();
                for (int i = 0; i < ecs.Count; i++)
                {
                    string Query = "select count(*) from tabhrmsemployeecertificationdetails where vchemployeeid ='" + NextEMPId + "' and upper(vchcertificateorcourse)='" + ManageQuote(ecs[i].vchcertificateorcourse).ToUpper() + "' ;";
                    int tcount = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(TRANS, CommandType.Text, Query));
                    if (tcount <= 0)
                    {
                        Query = "insert into tabhrmsemployeecertificationdetails(employeeid,vchemployeeid,vchcertificateorcourse,datfromdate,dattodate,statusid,createdby,createddate)";
                        Query += "values(" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(ecs[i].vchcertificateorcourse) + "','" + FormatDate(ecs[i].datfromdate) + "','" + FormatDate(ecs[i].dattodate) + "',1,1,current_timestamp);";
                        NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, Query);
                    }
                    else
                    {
                        Query = "update   tabhrmsemployeecertificationdetails set statusid=1,datfromdate='" + FormatDate(ecs[i].datfromdate) + "',dattodate='" + FormatDate(ecs[i].dattodate) + "',modifieddate=current_timestamp  where vchemployeeid ='" + NextEMPId + "' and upper(vchcertificateorcourse)='" + ManageQuote(ecs[i].vchcertificateorcourse).ToUpper() + "' ;";
                        NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, Query);
                    }
                }
                //trans.Commit();
                isvalid = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Training_Certification");
                trans.Rollback();
                isvalid = false;
                throw;
            }
            //finally
            //{
            //    if (con.State == ConnectionState.Open)
            //    {
            //        con.Close();
            //        con.Dispose();
            //        con.ClearPool();
            //        trans.Dispose();
            //    }
            //}
            return isvalid;
        }


        #endregion

        /// <summary>
        /// saikishore
        /// </summary>
        /// <returns></returns>
        #region Previous Experience

        public List<PreviousExperience> ShowPreviousExpDetails(PreviousExperience PreviousExperience)
        {
            List<PreviousExperience> lstPreviousexp = new List<PreviousExperience>();
            string strPreviousexp = string.Empty;
            try
            {
                //string strPreviousexp = "select recordid,employeeid,vchemployeeid,vchorganization,vchdesignation,datfromdate::text,dattodate::text,vchplace,numlastpay,vchreasonforleaving from tabhrmsemployeepreviousexperience";
                if (PreviousExperience.vchemployeeid != null && PreviousExperience.vchemployeeid != "")
                {
                    strPreviousexp = "select tp.vchemployeeid ||'-'||tp.recordid as vchemployee, upper(vchname ||' ' || vchsurname) as vchfullname,tpr.recordid,employeeid,tp.vchemployeeid,vchorganization,vchdesignation,to_char( datfromdate,'dd-Mon-yyyy') AS datfromdate,to_char( dattodate,'dd-Mon-yyyy')AS dattodate,vchplace,numlastpay,vchreasonforleaving from tabhrmsemployeepreviousexperience tpr join tabhrmsemployeepersonaldetails tp on tpr.employeeid=tp.recordid where tp.statusid=1 and tp.vchemployeeid ||'-'||tp.recordid='" + PreviousExperience.vchemployeeid + "';";
                }
                else
                {
                    strPreviousexp = "select tp.vchemployeeid ||'-'||tp.recordid as vchemployee, upper(vchname ||' ' || vchsurname) as vchfullname,tpr.recordid,employeeid,tp.vchemployeeid,vchorganization,vchdesignation,to_char( datfromdate,'dd-Mon-yyyy') AS datfromdate,to_char( dattodate,'dd-Mon-yyyy')AS dattodate,vchplace,numlastpay,vchreasonforleaving from tabhrmsemployeepreviousexperience tpr join tabhrmsemployeepersonaldetails tp on tpr.employeeid=tp.recordid where tp.statusid=1;";
                }
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strPreviousexp);
                while (npgdr.Read())
                {
                    PreviousExperience objPreviousExperience = new PreviousExperience();
                    objPreviousExperience.recordid = Convert.ToString(npgdr["recordid"].ToString());
                    objPreviousExperience.employeeid = npgdr["employeeid"].ToString();
                    objPreviousExperience.vchfullname = npgdr["vchfullname"].ToString();
                    objPreviousExperience.vchemployeeid = npgdr["vchemployee"].ToString();
                    objPreviousExperience.organisation = npgdr["vchorganization"].ToString();
                    objPreviousExperience.designation = npgdr["vchdesignation"].ToString();
                    objPreviousExperience.frmdate = npgdr["datfromdate"].ToString();
                    objPreviousExperience.todate = npgdr["dattodate"].ToString();
                    objPreviousExperience.place = npgdr["vchplace"].ToString();
                    objPreviousExperience.lastpay = npgdr["numlastpay"].ToString();
                    objPreviousExperience.reasonsforLeaving = npgdr["vchreasonforleaving"].ToString();
                    lstPreviousexp.Add(objPreviousExperience);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Previous_Experience");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPreviousexp;

        }

        public List<PreviousExperience> ShowReasonforLeaving()
        {
            List<PreviousExperience> lstReasonforLeaving = new List<PreviousExperience>();
            try
            {

                string strcountry = "select recordid,vchreasonsfoeleaving from tabhrmsreasonsfoeleavingmst where statusid=1 order by vchreasonsfoeleaving;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    PreviousExperience objReasonforLeaving = new PreviousExperience();

                    objReasonforLeaving.reasonsforLeavingid = npgdr["recordid"].ToString();
                    objReasonforLeaving.reasonsforLeaving = npgdr["vchreasonsfoeleaving"].ToString();

                    lstReasonforLeaving.Add(objReasonforLeaving);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstReasonforLeaving;
        }

        public List<PreviousExperience> showPrevExpDropdown()
        {

            List<PreviousExperience> lstPreviousexp = new List<PreviousExperience>();
            try
            {
                string preExpdropdown = "select vchemployeeid,recordid as employeeid from tabhrmsemployeepersonaldetails";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, preExpdropdown);
                while (npgdr.Read())
                {
                    PreviousExperience objPreviousExperience = new PreviousExperience();

                    objPreviousExperience.vchemployeeid = npgdr["vchemployeeid"].ToString();
                    objPreviousExperience.employeeid = Convert.ToString(npgdr["employeeid"].ToString());
                    lstPreviousexp.Add(objPreviousExperience);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Previous_Experience");
                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPreviousexp;

        }

        public int CreatePreviousExperience(PreviousExperience objPreviousExperience)
        {
            int status = 0;
            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                //if (con.State == ConnectionState.Closed)
                //    con.Open();
                string strCount = "select count(*) from tabhrmsemployeepreviousexperience  where vchemployeeid =trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',1)) and ( datfromdate between '" + FormatDate(objPreviousExperience.frmdate) + "' and '" + FormatDate(objPreviousExperience.todate) + "' or  dattodate between '" + FormatDate(objPreviousExperience.frmdate) + "' and '" + FormatDate(objPreviousExperience.todate) + "' or '" + FormatDate(objPreviousExperience.frmdate) + "' between datfromdate and dattodate or '" + FormatDate(objPreviousExperience.todate) + "' between datfromdate and dattodate ) and statusid=1";
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCount));
                if (count == 0)
                {
                    string strInsert = @"INSERT INTO tabhrmsemployeepreviousexperience(employeeid,vchemployeeid,vchorganization,vchdesignation,datfromdate,dattodate,vchplace,numlastpay,vchreasonforleaving,statusid,createdby,createddate) values(cast(trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',2)) as bigint),trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',1)),'" + ManageQuote(objPreviousExperience.organisation.ToUpper()) + "','" + ManageQuote(objPreviousExperience.designation.ToUpper()) + "','" + FormatDate(objPreviousExperience.frmdate) + "','" + FormatDate(objPreviousExperience.todate) + "','" + ManageQuote(objPreviousExperience.place.ToUpper()) + "'," + objPreviousExperience.lastpay + ",'" + ManageQuote(objPreviousExperience.reasonsforLeaving) + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    status = 1;
                }
                else
                {
                    status = 2;
                }


            }


            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Previous_Experience");
                status = 0;
                throw ex;
            }


            return status;

        }
        public int UpdatePreviousExperience(PreviousExperience objPreviousExperience)
        {
            int status = 0;
            try
            {
                string strCount = "select count(*) from tabhrmsemployeepreviousexperience  where vchemployeeid =trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',1)) and ( datfromdate between '" + FormatDate(objPreviousExperience.frmdate) + "' and '" + FormatDate(objPreviousExperience.todate) + "' or  dattodate between '" + FormatDate(objPreviousExperience.frmdate) + "' and '" + FormatDate(objPreviousExperience.todate) + "' or '" + FormatDate(objPreviousExperience.frmdate) + "' between datfromdate and dattodate or '" + FormatDate(objPreviousExperience.todate) + "' between datfromdate and dattodate ) and statusid=1";
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCount));
                if (count <= 1)
                {
                    string strupdateExp = @"update tabhrmsemployeepreviousexperience set employeeid=cast(trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',2)) as bigint),vchemployeeid=trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',1)),vchorganization='" + ManageQuote(objPreviousExperience.organisation.ToUpper()) + "',vchdesignation='" + ManageQuote(objPreviousExperience.designation.ToUpper()) + "',datfromdate='" + FormatDate(objPreviousExperience.frmdate) + "',dattodate='" + FormatDate(objPreviousExperience.todate)
                        + "',vchplace='" + ManageQuote(objPreviousExperience.place.ToUpper()) + "',numlastpay=" + objPreviousExperience.lastpay + ",vchreasonforleaving='" + ManageQuote(objPreviousExperience.reasonsforLeaving) + "',modifiedby=1,modifieddate=current_timestamp where recordid=" + objPreviousExperience.recordid + ";";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdateExp);
                    status = 1;
                }
                else
                {
                    status = 2;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Previous_Experience");
                throw ex;
            }
            return status;

        }
        //public bool UpdatePreviousExperience(PreviousExperience objPreviousExperience)
        //{
        //    bool status = false;
        //    try
        //    {
        //        status=UpdatePreviousExperience(objPreviousExperience);
        //        if (status)
        //        {
        //            string strupdateExp = @"update tabhrmsemployeepreviousexperience set employeeid=cast(trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',2)) as bigint),vchemployeeid=trim(split_part('" + ManageQuote(objPreviousExperience.vchemployeeid) + "','-',1)),vchorganization='" + ManageQuote(objPreviousExperience.organisation.ToUpper()) + "',vchdesignation='" + ManageQuote(objPreviousExperience.designation.ToUpper()) + "',datfromdate='" + FormatDate(objPreviousExperience.frmdate) + "',dattodate='" + FormatDate(objPreviousExperience.todate)
        //                + "',vchplace='" + ManageQuote(objPreviousExperience.place.ToUpper()) + "',numlastpay=" + objPreviousExperience.lastpay + ",vchreasonforleaving='" + ManageQuote(objPreviousExperience.reasonsforLeaving) + "',modifiedby=1,modifieddate=current_timestamp where recordid=" + objPreviousExperience.recordid + ";";
        //            NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdateExp);
        //            status = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "Previous_Experience");
        //        throw ex;
        //    }
        //    return status;

        //}

        public bool DeletePreviousExperience(PreviousExperience objPreviousExperience)
        {
            bool status = false;
            try
            {
                string strdeleteexp = "delete from  tabhrmsemployeepreviousexperience where recordid=" + objPreviousExperience.recordid + ";";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strdeleteexp);
                status = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Previous_Experience");
                throw ex;
            }
            return status;

        }


        #endregion

        /// <summary>
        /// Raji Reddy
        /// </summary>
        /// <returns></returns>
        #region Employeement Deatails

        public List<EmploymentDetails> ShowEmployee()
        {
            List<EmploymentDetails> lstEmployee = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select vchemployeeid,vchname from tabhrmsemployeepersonaldetails order by vchname;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objEmployee = new EmploymentDetails();

                    objEmployee.Employeeid = npgdr["vchemployeeid"].ToString();
                    objEmployee.Employee = npgdr["vchname"].ToString();

                    lstEmployee.Add(objEmployee);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstEmployee;
        }

        public List<EmploymentDetails> ShowTypeOfEmployment()
        {
            List<EmploymentDetails> lstTypeofEmployment = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchtypeofemployment from tabhrmstypeofemploymentmst where statusid=1 order by vchtypeofemployment ;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objEmployment = new EmploymentDetails();

                    objEmployment.TypeofEmploymentId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objEmployment.TypeOfEmployment = npgdr["vchtypeofemployment"].ToString();

                    lstTypeofEmployment.Add(objEmployment);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstTypeofEmployment;
        }

        public List<EmploymentDetails> ShowDesignations()
        {
            List<EmploymentDetails> lstDesignations = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchdesignation from tabhrmsdesignationmst where statusid=1 order by vchdesignation;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objDesignations = new EmploymentDetails();
                    objDesignations.DesignationId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objDesignations.Designation = npgdr["vchdesignation"].ToString();
                    lstDesignations.Add(objDesignations);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstDesignations;
        }

        public List<EmploymentDetails> ShowDepartments()
        {
            List<EmploymentDetails> lstDepartments = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchdepartment from tabhrmsdepartmentmst where statusid=1 order by vchdepartment;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objDepartments = new EmploymentDetails();
                    objDepartments.DepartmentId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objDepartments.Department = npgdr["vchdepartment"].ToString();
                    lstDepartments.Add(objDepartments);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstDepartments;
        }

        public List<EmploymentDetails> ShowReportingManager(string strDepartment)
        {
            List<EmploymentDetails> lstReportingManager = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select vchname||' '||vchsurname as vchname from tabhrmsemployeeemployment tee join tabhrmsemployeepersonaldetails tep on tee.vchemployeeid=tep.vchemployeeid where vchdepartment='" + ManageQuote(strDepartment) + "' and tee.statusid=1 order by vchname;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objManager = new EmploymentDetails();
                    objManager.ReportingManager = npgdr["vchname"].ToString();
                    lstReportingManager.Add(objManager);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstReportingManager;
        }

        public List<EmploymentDetails> ShowSalaryType()
        {
            List<EmploymentDetails> lstSalaryTypes = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchsalarytype from tabhrmssalarytypemst where statusid=1 order by vchsalarytype;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objSalaryTypes = new EmploymentDetails();
                    objSalaryTypes.SalaryTypeId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objSalaryTypes.SalaryType = npgdr["vchsalarytype"].ToString();
                    lstSalaryTypes.Add(objSalaryTypes);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstSalaryTypes;
        }

        public List<EmploymentDetails> ShowPayScale()
        {
            List<EmploymentDetails> lstPayScales = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchpayscalename from tabhrmspayscalesmst where statusid=1 order by vchpayscalename;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objPayScales = new EmploymentDetails();
                    objPayScales.PayScaleId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objPayScales.PayScale = npgdr["vchpayscalename"].ToString();
                    lstPayScales.Add(objPayScales);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstPayScales;
        }

        public List<EmploymentDetails> ShowPayScaleDetails(string strPayScale)
        {
            List<EmploymentDetails> lstScaleDetails = new List<EmploymentDetails>();
            try
            {

                //   string strScale = "select numbasic from tabhrmspayscalesmst where recordid=" + ManageQuote(strPayScaleid) + " and statusid=1;";
                string strScale = "select numbasic from tabhrmspayscalesmst where vchpayscalename='" + ManageQuote(strPayScale) + "' and statusid=1;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strScale);
                while (npgdr.Read())
                {
                    EmploymentDetails objScale = new EmploymentDetails();

                    objScale.BasicAmount = npgdr["numbasic"].ToString();

                    lstScaleDetails.Add(objScale);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");

                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstScaleDetails;
        }

        public List<EmploymentDetails> ShowSalaryStructure()
        {
            List<EmploymentDetails> lstSalaryStructure = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchname from tabhrmssalarystructuremst where statusid=1 order by vchname;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                lstSalaryStructure.Add(new EmploymentDetails() { SalaryStructure = "None", SalaryStructureId = 0 });
                while (npgdr.Read())
                {
                    EmploymentDetails objSalaryStructure = new EmploymentDetails();
                    objSalaryStructure.SalaryStructureId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objSalaryStructure.SalaryStructure = npgdr["vchname"].ToString();
                    lstSalaryStructure.Add(objSalaryStructure);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstSalaryStructure;
        }

        public List<EmploymentDetails> ShowLeaveStructure()
        {
            List<EmploymentDetails> lstLeaveStructure = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select recordid,vchname from tabhrmsleavestructuremst where statusid=1 order by vchname;";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objLeaveStructure = new EmploymentDetails();
                    objLeaveStructure.LeaveStructureId = Convert.ToInt32(npgdr["recordid"].ToString());
                    objLeaveStructure.LeaveStructure = npgdr["vchname"].ToString();
                    lstLeaveStructure.Add(objLeaveStructure);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstLeaveStructure;
        }

        public bool CreateEmployment(EmploymentDetails lstemp, NpgsqlTransaction TRANS)
        {
            bool IsValid = false;
            try
            {
                string stremp = string.Empty;
                //int empid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select recordid from tabhrmsemployeepersonaldetails where vchemployeeid='" + ManageQuote(lstemp.Employeeid) + "';"));
                if (lstemp.PayScale != null && lstemp.PayScale != "")
                {
                    stremp = "INSERT INTO tabhrmsemployeeemployment(employeeid, vchemployeeid, vchtypeofemployment, vchdesignation,vchdepartment, vchreportingmanager, vchsalarytype, vchpayscalename,numbasic, numvda, vchsalarystructurename, vchleavestructurename,datreportingdate, vchjoinedas, datjoiningdate, statusid, createdby,createddate) VALUES (" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(lstemp.TypeOfEmployment) + "','" + ManageQuote(lstemp.Designation) + "','" + ManageQuote(lstemp.Department) + "','" + ManageQuote(lstemp.ReportingManager) + "','" + ManageQuote(lstemp.SalaryType) + "','" + ManageQuote(lstemp.PayScale) + "','" + Convert.ToDecimal(lstemp.BasicAmount) + "','" + Convert.ToDecimal(lstemp.VDA) + "','" + ManageQuote(lstemp.SalaryStructure) + "','" + ManageQuote(lstemp.LeaveStructure) + "','" + FormatDate(lstemp.DateOfReporting) + "','" + ManageQuote(lstemp.JoinedAs) + "','" + FormatDate(lstemp.KapilGroupJoinDate) + "', 1, 1,current_timestamp);";
                }
                else
                {
                    stremp = "INSERT INTO tabhrmsemployeeemployment(employeeid, vchemployeeid, vchtypeofemployment, vchdesignation,vchdepartment, vchreportingmanager, vchsalarytype, numbasic, numvda, vchsalarystructurename, vchleavestructurename,datreportingdate, vchjoinedas, datjoiningdate, statusid, createdby,createddate) VALUES (" + EmpRecordId + ",'" + NextEMPId + "','" + ManageQuote(lstemp.TypeOfEmployment) + "','" + ManageQuote(lstemp.Designation) + "','" + ManageQuote(lstemp.Department) + "','" + ManageQuote(lstemp.ReportingManager) + "','" + ManageQuote(lstemp.SalaryType) + "','" + Convert.ToDecimal(lstemp.BasicAmount) + "','" + Convert.ToDecimal(lstemp.VDA) + "','" + ManageQuote(lstemp.SalaryStructure) + "','" + ManageQuote(lstemp.LeaveStructure) + "','" + FormatDate(lstemp.DateOfReporting) + "','" + ManageQuote(lstemp.JoinedAs) + "','" + FormatDate(lstemp.KapilGroupJoinDate) + "', 1, 1,current_timestamp);";
                }
                NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, stremp);
                IsValid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw ex;

            }

            return IsValid;
        }

        public List<EmploymentDetails> ShowSalaryStructureDetails(string strSStructure)
        {
            List<EmploymentDetails> lstSStructureDetails = new List<EmploymentDetails>();
            try
            {

                string strcountry = "select vchpayitem,vchpayitemtype,vchmode from tabhrmssalarystructuremstdetails tsd join tabhrmssalarystructuremst ts on tsd.detailsid=ts.recordid where vchname='" + ManageQuote(strSStructure) + "'; ";
                strcountry = "select vchpayitem,vchpayitemtype,tsd.vchmode,case when tsd.vchmode='Flat' then numamount::text else concat(numamount::text,'%') end as numamount   from tabhrmssalarystructuremstdetails tsd join tabhrmssalarystructuremst ts on tsd.detailsid=ts.recordid join tabhrmspayitems tp on tp.vchpayitemname=tsd.vchpayitem where vchname='" + ManageQuote(strSStructure) + "'; ";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objSStructureDetails = new EmploymentDetails();
                    objSStructureDetails.Payitem = npgdr["vchpayitem"].ToString();
                    objSStructureDetails.PayitemType = npgdr["vchpayitemtype"].ToString();
                    objSStructureDetails.Mode = npgdr["vchmode"].ToString();
                    objSStructureDetails.BasicAmount = npgdr["numamount"].ToString();
                    lstSStructureDetails.Add(objSStructureDetails);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }

            return lstSStructureDetails;
        }

        public List<EmploymentDetails> ShowLeaveStructureDetails(string strLStructure)
        {
            List<EmploymentDetails> lstLStructureDetails = new List<EmploymentDetails>();
            try
            {

                //string strcountry = "select vchleavetype,numdayperyear,nummaxaccumulatedays,nummaxenchashmentdays,numnoofdayspermonth from tabhrmsleavestructuremst tl join tabhrmsleavestructuremstdetails tld on tl.recordid=tld.detailsid  where tl.vchname='" + ManageQuote(strLStructure) + "';";
                string strcountry = "select tld.vchleavetype,tld.numdayperyear,tld.nummaxaccumulatedays,tld.nummaxenchashmentdays,tld.numnoofdayspermonth from tabhrmsleavestructuremst tl join tabhrmsleavestructuremstdetails tld on tl.recordid=tld.detailsid  where tl.vchname='" + ManageQuote(strLStructure) + "';";

                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strcountry);
                while (npgdr.Read())
                {
                    EmploymentDetails objLStructureDetails = new EmploymentDetails();
                    objLStructureDetails.LeaveType = npgdr["vchleavetype"].ToString();
                    objLStructureDetails.DaysPerYear = npgdr["numdayperyear"].ToString();
                    objLStructureDetails.AccumulateDays = npgdr["nummaxaccumulatedays"].ToString();
                    objLStructureDetails.EnchashmentDays = npgdr["nummaxenchashmentdays"].ToString();
                    objLStructureDetails.DaysPerMonth = npgdr["numnoofdayspermonth"].ToString();
                    lstLStructureDetails.Add(objLStructureDetails);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employeement_Deatails");
                throw;
            }

            return lstLStructureDetails;
        }


        #endregion


        /// <summary>
        /// Santhosh
        /// </summary>


        #region Kapil Career


        public int createKapilCareer(List<KapilCareerModel> kpm, NpgsqlTransaction TRANS)
        {
            int return_count = 0;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                for (int i = 0; i < kpm.Count; i++)
                {
                    string strInsert = "select count(*) from tabhrmsemployeekapilcarrerdetails where upper(vchcompanyname)='" + ManageQuote(kpm[i].CompanyName.ToUpper()) + "' and upper(vchemployeeid)='" + ManageQuote(kpm[i].EmployeeID.ToUpper()) + "';";
                    int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(TRANS, CommandType.Text, strInsert));
                    if (count <= 0)
                    {
                        strInsert = "INSERT INTO tabhrmsemployeekapilcarrerdetails(vchcompanyname,employeeid, vchemployeeid, vchdesignation, datfromdate, dattodate, vchsscminutesno, vchreasonfortransfer,   vchlocation, statusid, createdby, createddate,vchearnedleavesclaimed,datearnedleavesdate,vchkhcno, datvalidfromdate,datvalidtodate )  VALUES ( ";
                        strInsert += " '" + ManageQuote(kpm[i].CompanyName) + "',  '" + EmpRecordId + "', '" + NextEMPId + "',  '" + ManageQuote(kpm[i].Designation) + "',  '" + FormatDate(kpm[i].kc_fromdate) + "',  '" + FormatDate(kpm[i].kc_todate) + "',";
                        strInsert += " '" + ManageQuote(kpm[i].sscminutesno) + "',  '" + ManageQuote(kpm[i].ReasonTransfer) + "',   '" + ManageQuote(kpm[i].location) + "', ";
                        strInsert += "1,1, current_timestamp, '" + ManageQuote(kpm[i].ELClaimed) + "', '" + FormatDate(kpm[i].EL_date) + "','" + ManageQuote(kpm[i].KHCNo) + "', '" + FormatDate(kpm[i].KHC_fromdate) + "', '" + FormatDate(kpm[i].KHC_todate) + "');";
                        NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, strInsert);

                        //strInsert = "INSERT INTO tabhrmsemployeeeldetails(vchemployeeid, vchearnedleavesclaimed,datearnedleavesdate, statusid, createdby, createddate)  ";
                        //strInsert += "VALUES ('" + ManageQuote(kpm[i].vchemployeeid) + "', '" + ManageQuote(kpm[i].ELClaimed) + "', '" + FormatDate(kpm[i].EL_date) + "', " + ManageQuote(kpm[i].statusid) + ",";
                        //strInsert += " " + ManageQuote(kpm[i].createdby) + ", current_timestamp);";
                        //NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);

                        //strInsert = " INSERT INTO tabhrmsemployeekhcdetails(vchemployeeid, vchkhcno, datvalidfromdate,datvalidtodate, statusid, createdby, createddate)";
                        //strInsert += "VALUES ('" + ManageQuote(kpm[i].vchemployeeid) + "', '" + ManageQuote(kpm[i].KHCNo) + "', '" + FormatDate(kpm[i].KHC_fromdate) + "', '" + FormatDate(kpm[i].KHC_todate) + "',";
                        //strInsert += "" + ManageQuote(kpm[i].statusid) + ", " + ManageQuote(kpm[i].createdby) + ", current_timestamp);";
                        //NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                        //trans.Commit();
                        return_count = 1;
                    }
                    else
                    {
                        strInsert = "select count(*) from tabhrmsemployeekapilcarrerdetails where upper(vchcompanyname)='" + ManageQuote(kpm[i].CompanyName.ToUpper()) + "' and upper(vchemployeeid)='" + ManageQuote(kpm[i].EmployeeID.ToUpper()) + "' and statusid=1;";
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(TRANS, CommandType.Text, strInsert));
                        if (count >= 1)
                            return_count = 3;
                        else
                            return_count = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");

                return_count = 0;

            }
            return return_count;
        }


        public List<KapilCareerModel> ShowKapilCareerGrid(string ID)
        {
            List<KapilCareerModel> lstls = new List<KapilCareerModel>();
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                string query = string.Empty;
                if (ID != null && ID != "")
                {
                    var x = ID.Split('-');
                    query = "select recordid,vchemployeeid ,vchdesignation,(select vchname ||' '||vchsurname from tabhrmsemployeepersonaldetails where vchemployeeid =t.vchemployeeid  ) as vchname  ,vchcompanyname,to_char( datfromdate,'dd-Mon-yyyy') as datfromdate,to_char( dattodate,'dd-Mon-yyyy') as dattodate  from tabhrmsemployeekapilcarrerdetails t  WHERE statusid=1 and vchemployeeid='" + x[0].ToString() + "';";
                }
                else
                {
                    query = "select recordid,vchemployeeid ,vchdesignation,(select vchname ||' '||vchsurname from tabhrmsemployeepersonaldetails where vchemployeeid =t.vchemployeeid  ) as vchname  ,vchcompanyname,to_char( datfromdate,'dd-Mon-yyyy') as datfromdate,to_char( dattodate,'dd-Mon-yyyy') as dattodate  from tabhrmsemployeekapilcarrerdetails t  WHERE statusid=1;";
                }

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    KapilCareerModel ld = new KapilCareerModel();
                    ld.RecordId = dr["recordid"].ToString();
                    ld.Designation = dr["vchdesignation"].ToString();
                    ld.vchname = dr["vchname"].ToString();
                    ld.EmployeeID = dr["vchemployeeid"].ToString();
                    ld.CompanyName = dr["vchcompanyname"].ToString();
                    ld.kc_fromdate = dr["datfromdate"].ToString();
                    ld.kc_todate = dr["dattodate"].ToString();
                    lstls.Add(ld);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();

                    con.Dispose();
                    con.ClearPool();
                }
            }
            return lstls;
        }

        public List<KapilCareerModel> GetDesignations()
        {
            List<KapilCareerModel> lstls = new List<KapilCareerModel>();
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(" select vchdesignation,recordid from tabhrmsdesignationmst   WHERE statusid=1;", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    KapilCareerModel ld = new KapilCareerModel();
                    ld.RecordId = dr["recordid"].ToString();
                    ld.Designation = dr["vchdesignation"].ToString();
                    lstls.Add(ld);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();

                    con.Dispose();
                    con.ClearPool();
                }
            }
            return lstls;
        }
        public List<KapilCareerModel> GetReasons()
        {
            List<KapilCareerModel> lstls = new List<KapilCareerModel>();
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(" select vchreasonsfoetransfer,recordid from tabhrmsreasonsfoetransfermst  WHERE statusid=1 ;", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    KapilCareerModel ld = new KapilCareerModel();
                    ld.RecordId = dr["recordid"].ToString();
                    ld.ReasonTransfer = dr["vchreasonsfoetransfer"].ToString();
                    lstls.Add(ld);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();

                    con.Dispose();
                    con.ClearPool();
                }
            }
            return lstls;
        }


        public List<KapilCareerModel> getSelectedRowDetails(int recordid)
        {
            List<KapilCareerModel> lstls = new List<KapilCareerModel>();
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);



                NpgsqlDataAdapter da = new NpgsqlDataAdapter("  SELECT recordid,vchcompanyname,  VCHEMPLOYEEID ||'-'|| employeeid as vchemployeeid,vchdesignation,to_char( datfromdate,'dd-Mon-yyyy') as datfromdate,to_char( dattodate,'dd-Mon-yyyy') as dattodate ,                vchsscminutesno, vchreasonfortransfer,   vchlocation,vchearnedleavesclaimed,to_char( datearnedleavesdate,'dd-Mon-yyyy') as datearnedleavesdate,vchkhcno, to_char( datvalidfromdate,'dd-Mon-yyyy') as datvalidfromdate,to_char( datvalidtodate,'dd-Mon-yyyy') as datvalidtodate   FROM tabhrmsemployeekapilcarrerdetails where recordid=" + recordid + ";", con);




                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    KapilCareerModel ld = new KapilCareerModel();
                    ld.RecordId = dr["recordid"].ToString();
                    ld.CompanyName = dr["vchcompanyname"].ToString();
                    ld.EmployeeID = dr["vchemployeeid"].ToString();
                    ld.Designation = dr["vchdesignation"].ToString();
                    ld.kc_fromdate = dr["datfromdate"].ToString();
                    ld.kc_todate = dr["dattodate"].ToString();
                    ld.sscminutesno = dr["vchsscminutesno"].ToString();
                    ld.ReasonTransfer = dr["vchreasonfortransfer"].ToString();
                    ld.location = dr["vchlocation"].ToString();

                    ld.ELClaimed = dr["vchearnedleavesclaimed"].ToString();
                    ld.EL_date = dr["datearnedleavesdate"].ToString();

                    ld.KHCNo = dr["vchkhcno"].ToString();
                    ld.KHC_fromdate = dr["datvalidfromdate"].ToString();
                    ld.KHC_todate = dr["datvalidtodate"].ToString();

                    lstls.Add(ld);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();

                    con.Dispose();

                }
            }
            return lstls;
        }
        public int createKapilCareer(KapilCareerModel kpm)
        {
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strCount = "select count(*) from tabhrmsemployeepreviousexperience  where vchemployeeid =trim(split_part('" + ManageQuote(kpm.vchemployeeid.Split('-')[0]).Replace(" ", "") + "','-',1)) and ( datfromdate between '" + FormatDate(kpm.kc_fromdate) + "' and '" + FormatDate(kpm.kc_todate) + "' or  dattodate between '" + FormatDate(kpm.kc_fromdate) + "' and '" + FormatDate(kpm.kc_todate) + "' or '" + FormatDate(kpm.kc_fromdate) + "' between datfromdate and dattodate or '" + FormatDate(kpm.kc_todate) + "' between datfromdate and dattodate ) and statusid=1";
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strCount));


                if (count <= 0)
                {
                    strCount = "select count(*) from tabhrmsemployeekapilcarrerdetails  where vchemployeeid = '" + ManageQuote(kpm.EmployeeID.ToUpper()).Split('-')[0].Replace(" ", "") + "' and ( datfromdate between '" + FormatDate(kpm.kc_fromdate) + "' and '" + FormatDate(kpm.kc_todate) + "' or  dattodate between '" + FormatDate(kpm.kc_fromdate) + "' and '" + FormatDate(kpm.kc_todate) + "' or '" + FormatDate(kpm.kc_fromdate) + "' between datfromdate and dattodate or '" + FormatDate(kpm.kc_todate) + "' between datfromdate and dattodate ) and statusid=1";
                    count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strCount));
                    if (count <= 0)
                    {
                        string strInsert = "select count(*) from tabhrmsemployeekapilcarrerdetails where upper(vchcompanyname)='" + ManageQuote(kpm.CompanyName.ToUpper()) + "' and upper(vchemployeeid)='" + ManageQuote(kpm.EmployeeID.ToUpper()).Split('-')[0].Replace(" ", "") + "';";
                        count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert));
                        if (count <= 0)
                        {
                            if (kpm.EL_date != null)
                            {
                                strInsert = "INSERT INTO tabhrmsemployeekapilcarrerdetails(vchcompanyname,employeeid, vchemployeeid, vchdesignation, datfromdate, dattodate, vchsscminutesno, vchreasonfortransfer,   vchlocation, statusid, createdby, createddate,vchearnedleavesclaimed,datearnedleavesdate,vchkhcno, datvalidfromdate,datvalidtodate )  VALUES ( ";
                                strInsert += " '" + ManageQuote(kpm.CompanyName.ToUpper()) + "', " + ManageQuote(kpm.vchemployeeid.Split('-')[1]).Replace(" ", "") + ", '" + ManageQuote(kpm.vchemployeeid.Split('-')[0]).Replace(" ", "") + "',  '" + ManageQuote(kpm.Designation) + "',  '" + FormatDate(kpm.kc_fromdate) + "',  '" + FormatDate(kpm.kc_todate) + "',";
                                strInsert += " '" + ManageQuote(kpm.sscminutesno.ToUpper()) + "',  '" + ManageQuote(kpm.ReasonTransfer) + "',   '" + ManageQuote(kpm.location.ToUpper()) + "', ";
                                strInsert += "" + ManageQuote(kpm.statusid) + ",  " + ManageQuote(kpm.createdby) + ", current_timestamp, '" + ManageQuote(kpm.ELClaimed.ToUpper()) + "', '" + FormatDate(kpm.EL_date) + "','" + ManageQuote(kpm.KHCNo.ToUpper()) + "', '" + FormatDate(kpm.KHC_fromdate) + "', '" + FormatDate(kpm.KHC_todate) + "');";
                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                            }
                            else
                            {
                                strInsert = "INSERT INTO tabhrmsemployeekapilcarrerdetails(vchcompanyname,employeeid, vchemployeeid, vchdesignation, datfromdate, dattodate, vchsscminutesno, vchreasonfortransfer,   vchlocation, statusid, createdby, createddate,vchearnedleavesclaimed,datearnedleavesdate,vchkhcno, datvalidfromdate,datvalidtodate )  VALUES ( ";
                                strInsert += " '" + ManageQuote(kpm.CompanyName.ToUpper()) + "', " + ManageQuote(kpm.vchemployeeid.Split('-')[1]).Replace(" ", "") + ", '" + ManageQuote(kpm.vchemployeeid.Split('-')[0]).Replace(" ", "") + "',  '" + ManageQuote(kpm.Designation) + "',  '" + FormatDate(kpm.kc_fromdate) + "',  '" + FormatDate(kpm.kc_todate) + "',";
                                strInsert += " '" + ManageQuote(kpm.sscminutesno.ToUpper()) + "',  '" + ManageQuote(kpm.ReasonTransfer) + "',   '" + ManageQuote(kpm.location.ToUpper()) + "', ";
                                strInsert += "" + ManageQuote(kpm.statusid) + ",  " + ManageQuote(kpm.createdby) + ", current_timestamp, '" + ManageQuote(kpm.ELClaimed.ToUpper()) + "', null,'" + ManageQuote(kpm.KHCNo.ToUpper()) + "', '" + FormatDate(kpm.KHC_fromdate) + "', '" + FormatDate(kpm.KHC_todate) + "');";
                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                            }

                            trans.Commit();
                            return 1;
                        }
                        else
                        {
                            strInsert = "select count(*) from tabhrmsemployeekapilcarrerdetails where upper(vchcompanyname)='" + ManageQuote(kpm.CompanyName.ToUpper()) + "' and upper(vchemployeeid)='" + ManageQuote(kpm.EmployeeID.ToUpper()).Split('-')[0].Replace(" ", "") + "' and statusid=1;";
                            count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert));
                            if (count >= 1)
                                return 3;
                            else
                                return 2;


                        }
                    }
                    else
                    {
                        return 5;
                    }
                }
                else
                {
                    return 4;
                }



            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
                //throw ex;
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();

                    con.Dispose();
                    con.ClearPool();
                }

            }
        }

        public int updateKapilCareer(KapilCareerModel kpm)
        {
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = "select count(*) from tabhrmsemployeekapilcarrerdetails where upper(vchcompanyname)='" + ManageQuote(kpm.CompanyName.ToUpper()) + "' and upper(vchemployeeid)='" + ManageQuote(kpm.EmployeeID.ToUpper()).Split('-')[0].Replace(" ", "") + "';";
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strInsert));
                if (count <= 1)
                {
                    if (kpm.EL_date != null)
                    {
                        strInsert = "UPDATE tabhrmsemployeekapilcarrerdetails ";
                        strInsert += "SET   vchcompanyname='" + ManageQuote(kpm.CompanyName.ToUpper()) + "', ";
                        strInsert += "    vchdesignation= '" + ManageQuote(kpm.Designation) + "', datfromdate= '" + FormatDate(kpm.kc_fromdate) + "', dattodate= '" + FormatDate(kpm.kc_todate) + "', ";
                        strInsert += "vchsscminutesno= '" + ManageQuote(kpm.sscminutesno.ToUpper()) + "', vchreasonfortransfer='" + ManageQuote(kpm.ReasonTransfer) + "', vchlocation='" + ManageQuote(kpm.location.ToUpper()) + "', vchearnedleavesclaimed= '" + ManageQuote(kpm.ELClaimed.ToUpper()) + "', ";
                        strInsert += "datearnedleavesdate='" + FormatDate(kpm.EL_date) + "', vchkhcno='" + ManageQuote(kpm.KHCNo.ToUpper()) + "', datvalidfromdate= '" + FormatDate(kpm.KHC_fromdate) + "', datvalidtodate='" + FormatDate(kpm.KHC_todate) + "', ";
                        strInsert += " modifiedby=" + ManageQuote(kpm.modifiedby) + ", modifieddate=current_timestamp";
                        strInsert += " WHERE recordid=" + kpm.RecordId + ";";
                    }
                    else
                    {
                        strInsert = "UPDATE tabhrmsemployeekapilcarrerdetails ";
                        strInsert += "SET   vchcompanyname='" + ManageQuote(kpm.CompanyName.ToUpper()) + "', ";
                        strInsert += "    vchdesignation= '" + ManageQuote(kpm.Designation) + "', datfromdate= '" + FormatDate(kpm.kc_fromdate) + "', dattodate= '" + FormatDate(kpm.kc_todate) + "', ";
                        strInsert += "vchsscminutesno= '" + ManageQuote(kpm.sscminutesno.ToUpper()) + "', vchreasonfortransfer='" + ManageQuote(kpm.ReasonTransfer) + "', vchlocation='" + ManageQuote(kpm.location.ToUpper()) + "', vchearnedleavesclaimed= '" + ManageQuote(kpm.ELClaimed.ToUpper()) + "', ";
                        strInsert += "datearnedleavesdate=null, vchkhcno='" + ManageQuote(kpm.KHCNo.ToUpper()) + "', datvalidfromdate= '" + FormatDate(kpm.KHC_fromdate) + "', datvalidtodate='" + FormatDate(kpm.KHC_todate) + "', ";
                        strInsert += " modifiedby=" + ManageQuote(kpm.modifiedby) + ", modifieddate=current_timestamp";
                        strInsert += " WHERE recordid=" + kpm.RecordId + ";";

                    }
                }
                else
                {

                    return 2;


                }


                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);


                trans.Commit();
                return 1;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();

                    con.Dispose();

                    trans.Dispose();
                }
            }
        }

        public bool DeleteKapilCareer(int recordid)
        {
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = "update   tabhrmsemployeekapilcarrerdetails  set statusid=2 WHERE recordid=" + recordid + " ";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);


                trans.Commit();
                return true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();

            }
        }

        public int updateExistedKapilCareer(KapilCareerModel kpm)
        {
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                string strInsert = "UPDATE tabhrmsemployeekapilcarrerdetails  ";
                strInsert += "SET    statusid=1 ";
                strInsert += " WHERE   upper(vchcompanyname)='" + ManageQuote(kpm.CompanyName.ToUpper()) + "' and upper(vchemployeeid)='" + ManageQuote(kpm.EmployeeID.ToUpper()) + "';";



                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);


                trans.Commit();
                return 1;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Kapil_Career");
                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Dispose();

            }
        }



        #endregion

        /// <summary>
        /// Developed By: SRIKANTH.K
        /// </summary>
        /// <returns></returns>
        #region Deductions..

        public List<DeductionDTO> ShowDeductions(DeductionDTO C)
        {
            List<DeductionDTO> lstDeduction = new List<DeductionDTO>();
            string strDeductions = string.Empty;
            try
            {

                //npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT RECORDID, EMPLOYEEID, VCHEMPLOYEEID, NUMBASIC, VCHDEDUCTIONTYPE,VCHRECOVERYTYPE, NUMAMOUNT, NUMTENUREMONTHS, NUMDEDUCTIONAMOUNT,DATDEDUCTIONSTARTDATE, DATDEDUCTIONENDDATE, STATUSID  FROM TABHRMSEMPLOYEEDEDUCTIONS");
                //npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "SELECT td.VCHEMPLOYEEID ||' - '||td.RECORDID as VCHEMPLOYEE, UPPER(VCHNAME ||' ' || VCHSURNAME||' - '||td.VCHEMPLOYEEID) AS VCHFULLNAME,TE.VCHNAME,TD.RECORDID,TD.EMPLOYEEID,TD.VCHEMPLOYEEID,TD.NUMBASIC,TD.VCHDEDUCTIONTYPE,TD.VCHRECOVERYTYPE,TD.NUMAMOUNT,TD.NUMTENUREMONTHS,TD.NUMDEDUCTIONAMOUNT,TD.DATDEDUCTIONSTARTDATE,TD.DATDEDUCTIONENDDATE,TD.STATUSID FROM TABHRMSEMPLOYEEPERSONALDETAILS TE  JOIN TABHRMSEMPLOYEEDEDUCTIONS TD ON TE.RECORDID=TD.EMPLOYEEID WHERE TD.STATUSID=1");
                if (C.vchemployeeid != null && C.vchemployeeid != "")
                {
                    // strDeductions = "SELECT td.VCHEMPLOYEEID ||'-'||td.RECORDID as VCHEMPLOYEE, UPPER(VCHNAME ||' ' || VCHSURNAME) AS VCHFULLNAME,TE.VCHNAME||' '||TE.vchsurname as VCHNAME,TD.RECORDID,TD.EMPLOYEEID,TD.VCHEMPLOYEEID ||'-'||TD.EMPLOYEEID as VCHEMPLOYEEID,TD.NUMBASIC,TD.VCHDEDUCTIONTYPE,TD.VCHRECOVERYTYPE,TD.NUMAMOUNT,TD.NUMTENUREMONTHS,TD.NUMDEDUCTIONAMOUNT,to_char( TD.DATDEDUCTIONSTARTDATE,'dd-Mon-yyyy') AS DATDEDUCTIONSTARTDATE,to_char( TD.DATDEDUCTIONENDDATE,'dd-Mon-yyyy') AS DATDEDUCTIONENDDATE,TD.STATUSID FROM TABHRMSEMPLOYEEPERSONALDETAILS TE  JOIN TABHRMSEMPLOYEEDEDUCTIONS TD ON TE.RECORDID=TD.EMPLOYEEID WHERE TE.STATUSID=1 and (td.vchemployeeid||'-'||td.employeeid)='" + C.vchemployeeid + "'";
                    strDeductions = "select td.vchemployeeid as  vchemployee, upper(vchname ||' ' || vchsurname) as vchfullname,te.vchname||' '||te.vchsurname as vchname,td.recordid,td.employeeid,td.vchemployeeid ||'-'||td.employeeid||'-'||ts.numbasic as vchemployeeid,td.numbasic,td.vchdeductiontype,td.vchrecoverytype,td.numamount,td.numtenuremonths,td.numdeductionamount,to_char( td.datdeductionstartdate,'dd-mon-yyyy') as datdeductionstartdate,to_char( td.datdeductionenddate,'dd-mon-yyyy') as datdeductionenddate,td.statusid from tabhrmsemployeepersonaldetails te  join tabhrmsemployeedeductions td on te.recordid=td.employeeid join tabhrmsemployeeemployment ts on te.vchemployeeid=ts.vchemployeeid where td.statusid=1 and (td.vchemployeeid)='" + C.vchemployeeid.Split('-').First() + "'";
                }
                else
                {
                    // strDeductions = "SELECT td.VCHEMPLOYEEID ||'-'||td.RECORDID as VCHEMPLOYEE, UPPER(VCHNAME ||' ' || VCHSURNAME) AS VCHFULLNAME,TE.VCHNAME||' '||TE.vchsurname as VCHNAME,TD.RECORDID,TD.EMPLOYEEID,TD.VCHEMPLOYEEID ||'-'||TD.EMPLOYEEID as VCHEMPLOYEEID,TD.NUMBASIC,TD.VCHDEDUCTIONTYPE,TD.VCHRECOVERYTYPE,TD.NUMAMOUNT,TD.NUMTENUREMONTHS,TD.NUMDEDUCTIONAMOUNT,to_char( TD.DATDEDUCTIONSTARTDATE,'dd-Mon-yyyy') AS DATDEDUCTIONSTARTDATE,to_char( TD.DATDEDUCTIONENDDATE,'dd-Mon-yyyy') AS DATDEDUCTIONENDDATE,TD.STATUSID FROM TABHRMSEMPLOYEEPERSONALDETAILS TE  JOIN TABHRMSEMPLOYEEDEDUCTIONS TD ON TE.RECORDID=TD.EMPLOYEEID WHERE TE.STATUSID=1";
                    strDeductions = "select td.vchemployeeid as  vchemployee, upper(vchname ||' ' || vchsurname) as vchfullname,te.vchname||' '||te.vchsurname as vchname,td.recordid,td.employeeid,td.vchemployeeid ||'-'||td.employeeid||'-'||ts.numbasic as vchemployeeid,td.numbasic,td.vchdeductiontype,td.vchrecoverytype,td.numamount,td.numtenuremonths,td.numdeductionamount,to_char( td.datdeductionstartdate,'dd-mon-yyyy') as datdeductionstartdate,to_char( td.datdeductionenddate,'dd-mon-yyyy') as datdeductionenddate,td.statusid from tabhrmsemployeepersonaldetails te  join tabhrmsemployeedeductions td on te.recordid=td.employeeid join tabhrmsemployeeemployment ts on te.vchemployeeid=ts.vchemployeeid where td.statusid=1 and te.statusid=1;";
                }
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strDeductions);
                while (npgdr.Read())
                {
                    DeductionDTO objDeductionDTO = new DeductionDTO();
                    objDeductionDTO.RecordId = Convert.ToInt32(npgdr["RECORDID"]);
                    objDeductionDTO.Employeeid = Convert.ToString(npgdr["VCHEMPLOYEE"]);
                    objDeductionDTO.EmployeeName = Convert.ToString(npgdr["vchname"]);
                    objDeductionDTO.vchemployeeid = Convert.ToString(npgdr["VCHEMPLOYEEID"]);
                    objDeductionDTO.vchfullname = Convert.ToString(npgdr["vchfullname"]);
                    objDeductionDTO.BasicAmount = Convert.ToDecimal(npgdr["NUMBASIC"]);
                    objDeductionDTO.DeductionType = Convert.ToString(npgdr["VCHDEDUCTIONTYPE"]);
                    objDeductionDTO.RecoveryType = Convert.ToString(npgdr["VCHRECOVERYTYPE"]);
                    objDeductionDTO.Amount = Convert.ToDecimal(npgdr["NUMAMOUNT"]);
                    objDeductionDTO.TenureMonths = Convert.ToDecimal(npgdr["NUMTENUREMONTHS"]);
                    objDeductionDTO.DeductionAmount = Convert.ToDecimal(npgdr["NUMDEDUCTIONAMOUNT"]);
                    objDeductionDTO.StartDate = Convert.ToString(npgdr["DATDEDUCTIONSTARTDATE"]).Remove(0, 3);
                    objDeductionDTO.EndDate = Convert.ToString(npgdr["DATDEDUCTIONENDDATE"]).Remove(0, 3);
                    objDeductionDTO.StatusID = Convert.ToString(npgdr["STATUSID"]);
                    lstDeduction.Add(objDeductionDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Deductions");
                throw;
            }
            finally
            {
                npgdr.Dispose();
            }

            return lstDeduction;
        }
        //public bool CreateDeduction(DeductionDTO C)
        //{
        //    Boolean IsValid = false;
        //    try
        //    {
        //        //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
        //        //if (con.State != ConnectionState.Open)
        //        //{
        //        //    con.Open();
        //        //}
        //        //int res = GetCount(C.CourseName);
        //        //if (res == 0)
        //        //{

        //        string strInsert = "INSERT INTO TABHRMSEMPLOYEEDEDUCTIONS( EMPLOYEEID, VCHEMPLOYEEID, NUMBASIC,VCHDEDUCTIONTYPE,VCHRECOVERYTYPE, NUMAMOUNT, NUMTENUREMONTHS, NUMDEDUCTIONAMOUNT,DATDEDUCTIONSTARTDATE, DATDEDUCTIONENDDATE, STATUSID, CREATEDBY, CREATEDDATE) VALUES (" + EmpRecordId + ",'" + NextEMPId + "','" + C.BasicAmount + "','" + ManageQuote(C.DeductionType) + "','" + ManageQuote(C.RecoveryType) + "','" + C.Amount + "','" + C.TenureMonths + "','" + C.DeductionAmount + "','" + FormatDate(C.StartDate) + "','" + FormatDate(C.EndDate) + "',1,1,current_timestamp)";
        //        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
        //        //cmd = new NpgsqlCommand(strInsert, con);
        //        IsValid = true;
        //        // }

        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "Deductions");
        //        throw ex;
        //    }
        //    //finally
        //    //{
        //    //    if (con.State == ConnectionState.Open)
        //    //    {
        //    //        con.Close();
        //    //        con.Dispose();
        //    //        con.ClearPool();
        //    //    }
        //    //}
        //    return IsValid;

        //}


        public bool CreateDeduction(DeductionDTO C)
        {
            Boolean IsValid = false;
            string StartDate = "01-" + C.StartDate;
            string EndDate = "01-" + C.EndDate;
            int Count = 0;
            try
            {
                // int empid = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select recordid from tabhrmsemployeepersonaldetails where vchemployeeid='" + ManageQuote(C.Employeeid.ToString()) + "';"));
                if (C.DeductionType.ToUpper() == "ADVANCE")
                {
                    string strCount = "select count(*) from TABHRMSEMPLOYEEDEDUCTIONS where vchdeductiontype='Advance' and employeeid=" + C.vchemployeeid.Split('-')[1] + "  and (datdeductionstartdate + (12 || 'month')::INTERVAL) > '" + FormatDate(StartDate) + "'";
                    Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strCount));
                }
                if (Count > 0 && C.DeductionType.ToUpper() == "ADVANCE")
                {
                    IsValid = false;
                }
                else
                {
                    string strInsert = "INSERT INTO TABHRMSEMPLOYEEDEDUCTIONS( EMPLOYEEID, VCHEMPLOYEEID, NUMBASIC,VCHDEDUCTIONTYPE,VCHRECOVERYTYPE, NUMAMOUNT, NUMTENUREMONTHS, NUMDEDUCTIONAMOUNT,DATDEDUCTIONSTARTDATE, DATDEDUCTIONENDDATE, STATUSID, CREATEDBY, CREATEDDATE) VALUES (cast(trim(split_part('" + ManageQuote(C.vchemployeeid) + "','-',2)) as bigint),trim(split_part('" + ManageQuote(C.vchemployeeid) + "','-',1)),'" + C.BasicAmount + "','" + ManageQuote(C.DeductionType) + "','" + ManageQuote(C.RecoveryType) + "','" + C.Amount + "','" + C.TenureMonths + "','" + C.DeductionAmount + "','" + FormatDate(StartDate) + "','" + FormatDate(EndDate) + "',1,1,current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Deductions");
                throw ex;
            }

            return IsValid;

        }
        public bool DeleteDeduction(int id)
        {
            bool IsValid = false;
            try
            {

                string strDelete = "update TABHRMSEMPLOYEEDEDUCTIONS set statusid='2'  where recordid=" + id + "";

                int res = NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                if (res == 1)
                {
                    IsValid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Deductions");
                throw;
            }
            finally
            {

            }
            return IsValid;
        }
        public bool UpdateDeduction(DeductionDTO C)
        {
            Boolean IsValid = false;
            string StartDate = "01-" + C.StartDate;
            string EndDate = "01-" + C.EndDate;
            try
            {


                string strUpdate = "update TABHRMSEMPLOYEEDEDUCTIONS SET  NUMBASIC=" + C.BasicAmount + ", VCHDEDUCTIONTYPE='" + ManageQuote(C.DeductionType) + "',VCHRECOVERYTYPE='" + ManageQuote(C.RecoveryType) + "', NUMAMOUNT='" + C.Amount + "', NUMTENUREMONTHS='" + (C.TenureMonths) + "', NUMDEDUCTIONAMOUNT='" + (C.DeductionAmount) + "', DATDEDUCTIONSTARTDATE='" + ManageQuote(StartDate) + "', DATDEDUCTIONENDDATE='" + ManageQuote(EndDate) + "', MODIFIEDBY=1, MODIFIEDDATE=current_timestamp where recordid=" + C.RecordId + " ";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strUpdate);

                IsValid = true;


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Deductions");
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
        /// <returns></returns>
        #region Employee Identification Details

        public bool SaveEmployeeIdentifications(EmployeeIdentification EmployeeIdentification, List<BankDetails> lstBankDetails, NpgsqlTransaction TRANS)
        {
            bool isSaved = true;

            try
            {
                //con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                //trans = con.BeginTransaction();

                string strInsert = "INSERT INTO TABHRMSEMPLOYEEIDENTIFICATIONDETAILS(EMPLOYEEID, VCHEMPLOYEEID, VCHESISTATUS, VCHESINO,VCHPFSTATUS, VCHPFNO, DATPFEFFETIVEDATE, VCHUANNO, VCHPASSPORTNO, VCHAADHARNO, VCHPANNO, VCHDRIVINGLICENCENO, VCHVEHICLEREGISTRATIONNO,VCHVEHICLEINSURANCENO, DATINSURANCEEXPDATE, STATUSID, CREATEDBY,CREATEDDATE) VALUES ( " + EmpRecordId + ", '" + NextEMPId + "', '" + EmployeeIdentification.EsiStatus + "', '" + ManageQuote(EmployeeIdentification.EsiNo.Trim().ToUpper()) + "','" + EmployeeIdentification.PfStatus + "', '" + ManageQuote(EmployeeIdentification.Pfno.Trim().ToUpper()) + "', '" + EmployeeIdentification.PfEffectiveDate + "', '" + EmployeeIdentification.UanNo + "', '" + ManageQuote(EmployeeIdentification.PassportNo.Trim().ToUpper()) + "', '" + EmployeeIdentification.AadharNo + "','" + ManageQuote(EmployeeIdentification.PancardNo.Trim().ToUpper()) + "', '" + EmployeeIdentification.DrivingLicenceNo + "', '" + ManageQuote(EmployeeIdentification.VehicleRegistrationNo.Trim().ToUpper()) + "', '" + ManageQuote(EmployeeIdentification.VehicleInsuranceNo.Trim().ToUpper()) + "', '" + EmployeeIdentification.InsuranceDate + "', 1,1,current_timestamp)";
                NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, strInsert);


                foreach (var item in lstBankDetails)
                {
                    string strBankDetails = "INSERT INTO TABHRMSEMPLOYEEBANKACCOUNTDETAILS( EMPLOYEEID, VCHEMPLOYEEID, VCHBANKNAME, VCHBRANCH,VCHACCOUNTNO, STATUSID, CREATEDBY, CREATEDDATE) VALUES ( " + EmpRecordId + ", '" + NextEMPId + "', '" + ManageQuote(item.Bankname) + "', '" + ManageQuote(item.Branchname) + "', '" + ManageQuote(item.BankAccountNo) + "', 1,1,current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(TRANS, CommandType.Text, strBankDetails);

                }


                // trans.Commit();

                isSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;

            }
            //finally
            //{
            //    if (con.State == ConnectionState.Open)
            //    {
            //        con.Close();

            //        con.Dispose();
            //        con.ClearPool();
            //    }
            //}
            return isSaved;
        }

        public string SaveEmployeeIdentifications(EmployeeIdentification employeeIdentification, List<BankDetails> lstBankDetails)
        {
            bool isSaved = true;
            int Employeeno;



            string Idproofexist = string.Empty;
            string txtproofexist = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();
                int count = 0;

                Idproofexist = "SELECT  (SELECT COUNT(VCHPFNO) PFNO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHPFNO='" + employeeIdentification.Pfno + "' AND '" + employeeIdentification.Pfno + "'!=''  and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))||'@@'||(SELECT COUNT(VCHESINO) ESINO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHESINO='" + employeeIdentification.EsiNo + "' AND '" + employeeIdentification.EsiNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))||'@@'||(SELECT COUNT(VCHPASSPORTNO) PASSPORTNO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHPASSPORTNO='" + employeeIdentification.PassportNo + "' AND '" + employeeIdentification.PassportNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))";
                Idproofexist = Idproofexist + " ||'@@'||(SELECT COUNT(VCHDRIVINGLICENCENO) DRIVINGLICENCENO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHDRIVINGLICENCENO='" + employeeIdentification.DrivingLicenceNo + "' AND '" + employeeIdentification.DrivingLicenceNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))||'@@'||(SELECT COUNT(VCHUANNO) UANNO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHUANNO='" + employeeIdentification.UanNo + "' AND '" + employeeIdentification.UanNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))||'@@'||(SELECT COUNT(VCHAADHARNO) VCHAADHARNO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHAADHARNO='" + employeeIdentification.AadharNo + "' AND '" + employeeIdentification.AadharNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))||'@@'||(SELECT COUNT(VCHPANNO) VCHPANNO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHPANNO='" + employeeIdentification.PancardNo + "' AND '" + employeeIdentification.PancardNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "'))||'@@'||(SELECT COUNT(VCHVEHICLEREGISTRATIONNO) VCHVEHICLEREGISTRATIONNO FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHVEHICLEREGISTRATIONNO='" + employeeIdentification.VehicleRegistrationNo + "' AND '" + employeeIdentification.VehicleRegistrationNo + "'!='' and vchemployeeid not in('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "')) ";
                string ExistIdProof = Convert.ToString(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, Idproofexist));
                count = CheckCount(employeeIdentification.Employeeid.Split('-')[0].Trim(), "TABHRMSEMPLOYEEIDENTIFICATIONDETAILS", "VCHEMPLOYEEID");
                string strInsertOrUpdate = string.Empty;
                string[] parts1 = ExistIdProof.Split(new string[] { "@@" }, StringSplitOptions.None);
                int Idproofcount = 0;
                Idproofcount = Convert.ToInt32(parts1[0]) + Convert.ToInt32(parts1[1]) + Convert.ToInt32(parts1[2]) + Convert.ToInt32(parts1[3]) + Convert.ToInt32(parts1[4]) + Convert.ToInt32(parts1[5]) + Convert.ToInt32(parts1[6]) + Convert.ToInt32(parts1[7]);

                if (Idproofcount <= 0)
                {
                    if (count > 0)
                    {
                        strInsertOrUpdate = "UPDATE TABHRMSEMPLOYEEIDENTIFICATIONDETAILS SET VCHESISTATUS= '" + employeeIdentification.EsiStatus + "', VCHESINO='" + ManageQuote(employeeIdentification.EsiNo) + "',VCHPFSTATUS='" + employeeIdentification.PfStatus + "', VCHPFNO='" + ManageQuote(employeeIdentification.Pfno) + "', DATPFEFFETIVEDATE='" + Convert.ToDateTime(employeeIdentification.PfEffectiveDate) + "', VCHUANNO='" + ManageQuote(employeeIdentification.UanNo) + "', VCHPASSPORTNO='" + ManageQuote(employeeIdentification.PassportNo) + "', VCHAADHARNO='" + ManageQuote(employeeIdentification.AadharNo) + "', VCHPANNO='" + ManageQuote(employeeIdentification.PancardNo) + "', VCHDRIVINGLICENCENO='" + ManageQuote(employeeIdentification.DrivingLicenceNo) + "', VCHVEHICLEREGISTRATIONNO='" + ManageQuote(employeeIdentification.VehicleRegistrationNo) + "',VCHVEHICLEINSURANCENO='" + ManageQuote(employeeIdentification.VehicleInsuranceNo) + "', DATINSURANCEEXPDATE='" + Convert.ToDateTime(employeeIdentification.InsuranceDate) + "', MODIFIEDBY=1,MODIFIEDDATE=CURRENT_TIMESTAMP WHERE VCHEMPLOYEEID='" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "' ";
                    }
                    else
                    {
                        strInsertOrUpdate = "INSERT INTO TABHRMSEMPLOYEEIDENTIFICATIONDETAILS(  employeeid,VCHEMPLOYEEID, VCHESISTATUS, VCHESINO,VCHPFSTATUS, VCHPFNO, DATPFEFFETIVEDATE, VCHUANNO, VCHPASSPORTNO, VCHAADHARNO, VCHPANNO, VCHDRIVINGLICENCENO, VCHVEHICLEREGISTRATIONNO,VCHVEHICLEINSURANCENO, DATINSURANCEEXPDATE, STATUSID, CREATEDBY,CREATEDDATE) VALUES ( " + employeeIdentification.Employeeid.Split('-')[1].Trim() + ", '" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "', '" + employeeIdentification.EsiStatus + "', '" + ManageQuote(employeeIdentification.EsiNo) + "','" + employeeIdentification.PfStatus + "', '" + ManageQuote(employeeIdentification.Pfno) + "', '" + Convert.ToDateTime(employeeIdentification.PfEffectiveDate) + "', '" + ManageQuote(employeeIdentification.UanNo) + "', '" + ManageQuote(employeeIdentification.PassportNo) + "', '" + employeeIdentification.AadharNo + "','" + ManageQuote(employeeIdentification.PancardNo) + "', '" + ManageQuote(employeeIdentification.DrivingLicenceNo) + "', '" + ManageQuote(employeeIdentification.VehicleRegistrationNo) + "', '" + ManageQuote(employeeIdentification.VehicleInsuranceNo) + "', '" + Convert.ToDateTime(employeeIdentification.InsuranceDate) + "', 1,1,current_timestamp) ";

                    }
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsertOrUpdate);
                    foreach (var item in lstBankDetails)
                    {
                        int res = 0;
                        string strBankDetails = string.Empty;
                        string CheckExist = "select COUNT(*) from TABHRMSEMPLOYEEBANKACCOUNTDETAILS WHERE RECORDID=" + item.Bankrecordid + ";";
                        res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, CheckExist));
                        if (res > 0)
                        {
                            strBankDetails = "UPDATE TABHRMSEMPLOYEEBANKACCOUNTDETAILS SET VCHBANKNAME='" + ManageQuote(item.Bankname) + "',VCHBRANCH='" + ManageQuote(item.Branchname) + "',VCHACCOUNTNO='" + ManageQuote(item.BankAccountNo) + "',MODIFIEDBY=1,MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID=" + item.Bankrecordid + ";";
                        }
                        else
                        {
                            strBankDetails = "INSERT INTO TABHRMSEMPLOYEEBANKACCOUNTDETAILS( EMPLOYEEID, VCHEMPLOYEEID, VCHBANKNAME, VCHBRANCH,VCHACCOUNTNO, STATUSID, CREATEDBY, CREATEDDATE) VALUES ( " + employeeIdentification.Employeeid.Split('-')[1].Trim() + ", '" + ManageQuote(employeeIdentification.Employeeid.Split('-')[0].Trim()) + "', '" + ManageQuote(item.Bankname) + "', '" + ManageQuote(item.Branchname) + "', '" + ManageQuote(item.BankAccountNo) + "', 1,1,current_timestamp)";

                        }

                        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strBankDetails);
                    }

                    trans.Commit();
                }
                else
                {
                    if (Convert.ToInt32(parts1[0]) > 0)
                    {
                        txtproofexist = "PF No.,";
                    }
                    if (Convert.ToInt32(parts1[1]) > 0)
                    {
                        txtproofexist = txtproofexist + " ESI No. ,";
                    }
                    if (Convert.ToInt32(parts1[2]) > 0)
                    {
                        txtproofexist = txtproofexist + " Passport No. ,";
                    }
                    if (Convert.ToInt32(parts1[3]) > 0)
                    {
                        txtproofexist = txtproofexist + " Drivinglicence No. ,";
                    }
                    if (Convert.ToInt32(parts1[4]) > 0)
                    {
                        txtproofexist = txtproofexist + " UAN No., ";
                    }
                    if (Convert.ToInt32(parts1[5]) > 0)
                    {
                        txtproofexist = txtproofexist + " Aadhar No., ";
                    }
                    if (Convert.ToInt32(parts1[6]) > 0)
                    {
                        txtproofexist = txtproofexist + " PAN No., ";
                    }
                    if (Convert.ToInt32(parts1[7]) > 0)
                    {
                        txtproofexist = txtproofexist + " Vehicle Registration No.";
                    }
                    txtproofexist = txtproofexist.TrimEnd(',');
                }


                isSaved = true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                }
            }
            return txtproofexist;

        }

        public EmployeeIdentification ShowEmployeeIdentificationDetails(string Employeeid)
        {
            EmployeeIdentification objEmployeeIdentification = new EmployeeIdentification();
            try
            {
                string str_Data = string.Empty;
                str_Data += "SELECT VCHESINO, VCHPFSTATUS,vchesistatus, VCHPFNO, DATPFEFFETIVEDATE, VCHUANNO, VCHPASSPORTNO, VCHAADHARNO, VCHPANNO, VCHDRIVINGLICENCENO, VCHVEHICLEREGISTRATIONNO, VCHVEHICLEINSURANCENO, DATINSURANCEEXPDATE FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHEMPLOYEEID='" + Employeeid.Split('-')[0].Trim() + "' ;";
                str_Data += "SELECT recordid,vchemployeeid,VCHBANKNAME, VCHBRANCH, VCHACCOUNTNO FROM TABHRMSEMPLOYEEBANKACCOUNTDETAILS  where vchemployeeid='" + Employeeid.Split('-')[0].Trim() + "' and statusid=1 ;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str_Data);
                objEmployeeIdentification.Employeeid = Employeeid;
                while (npgdr.Read())
                {

                    objEmployeeIdentification.Pfno = npgdr["VCHPFNO"].ToString();
                    if (!string.IsNullOrEmpty(npgdr["DATPFEFFETIVEDATE"].ToString()))
                    {
                        objEmployeeIdentification.PfEffectiveDate = Convert.ToDateTime(npgdr["DATPFEFFETIVEDATE"]).ToString("dd/MM/yyyy");
                    }

                    objEmployeeIdentification.EsiNo = npgdr["VCHESINO"].ToString();
                    objEmployeeIdentification.PfStatus = Convert.ToString(npgdr["vchpfstatus"]);
                    objEmployeeIdentification.EsiStatus = Convert.ToString(npgdr["vchesistatus"]);
                    objEmployeeIdentification.UanNo = npgdr["VCHUANNO"].ToString();
                    objEmployeeIdentification.PassportNo = npgdr["VCHPASSPORTNO"].ToString();
                    objEmployeeIdentification.AadharNo = npgdr["VCHAADHARNO"].ToString();
                    objEmployeeIdentification.PancardNo = npgdr["VCHPANNO"].ToString();
                    objEmployeeIdentification.DrivingLicenceNo = npgdr["VCHDRIVINGLICENCENO"].ToString();
                    objEmployeeIdentification.VehicleRegistrationNo = npgdr["VCHVEHICLEREGISTRATIONNO"].ToString();
                    objEmployeeIdentification.VehicleInsuranceNo = npgdr["VCHVEHICLEINSURANCENO"].ToString();
                    if (!string.IsNullOrEmpty(npgdr["DATINSURANCEEXPDATE"].ToString()))
                    {
                        objEmployeeIdentification.InsuranceDate = Convert.ToDateTime(npgdr["DATINSURANCEEXPDATE"]).ToString("dd/MM/yyyy");
                    }

                }
                if (npgdr.NextResult())
                {
                    List<BankDetails> lstBankDetails = new List<BankDetails>();
                    while (npgdr.Read())
                    {
                        BankDetails objBankDetails = new BankDetails();
                        objBankDetails.BankAccountNo = npgdr["VCHACCOUNTNO"].ToString();
                        objBankDetails.Bankname = npgdr["VCHBANKNAME"].ToString();
                        objBankDetails.Branchname = npgdr["VCHBRANCH"].ToString();
                        objBankDetails.Bankrecordid = Convert.ToInt32(npgdr["recordid"]);
                        objBankDetails.BankStatus = "Y";
                        lstBankDetails.Add(objBankDetails);
                    }
                    objEmployeeIdentification.BankDetails = lstBankDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objEmployeeIdentification;
        }

        public bool DeleteBankDetails(int Bankrecordid)
        {
            bool isDeleted = false;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                string Delete = "UPDATE  tabhrmsemployeebankaccountdetails SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE recordid=" + Bankrecordid + ";";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Delete);
                isDeleted = true;
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    con.ClearPool();
                }
            }
            return isDeleted;
        }

        public bool CheckAccountNoExist(EmployeeIdentification employeeIdentification, BankDetails lstBankDetails)
        {
            bool isSaved = true;
            int Count = 0;
            string strAccountNo = string.Empty;
            try
            {
                strAccountNo = "SELECT count(*) FROM TABHRMSEMPLOYEEBANKACCOUNTDETAILS WHERE VCHACCOUNTNO='" + lstBankDetails.BankAccountNo + "' AND VCHEMPLOYEEID NOT IN('" + employeeIdentification.Employeeid.Split('-')[0].Trim() + "');";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strAccountNo));
                if (Count == 0)
                {
                    isSaved = true;
                }
                else
                {
                    isSaved = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return isSaved;

        }

        #endregion

        /// <summary>
        /// Sateesh Dasari 09-12-2015
        /// </summary>
        /// <param name="objAll"></param>
        /// <returns></returns>
        /// 
        #region SaveOrUpdateEmployeeDetails...

        public bool SaveEmployeedetails(empAll objAll)
        {
            bool isSaved = false;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                #region All..
                //PERSONALINFORMATION   1
                if (objAll.personalinformation != null)
                {
                    isSaved = SaveBranchAllowance(objAll.personalinformation, trans);
                }
                //Emergency_Contact_Details   2
                if (isSaved && objAll.emergencycontactdetails != null)
                {
                    isSaved = SaveEmergencyContactDetails(objAll.emergencycontactdetails, trans);
                }
                //Employee_Family_Details   3
                if (isSaved && objAll.lstemployeefamilydetails != null)
                {
                    isSaved = CreateEmpFamilyDetails(objAll.lstemployeefamilydetails, trans);
                }
                //Address   4
                if (isSaved && objAll.addresses != null)
                {
                    isSaved = SaveEmployee(objAll.addresses, trans);
                }
                //Education_Details   5
                if (isSaved && objAll.lsteducationdetails != null)
                {
                    isSaved = CreateEducationDetails(objAll.lsteducationdetails, trans);
                    // isSaved = true;
                }
                //Certification_Training   6
                if (isSaved && objAll.lstemployeecourses != null)
                {
                    isSaved = SaveCertificateCourse(objAll.lstemployeecourses, trans);

                }
                //Previous_Experience   7
                //if (isSaved && objAll.previousexperience != null)
                //{
                //    isSaved = CreatePreviousExperience(objAll.previousexperience, trans);

                //}
                //Employment_Details   8
                if (isSaved && objAll.employmentdetails != null)
                {
                    isSaved = CreateEmployment(objAll.employmentdetails, trans);

                }
                //EmployeeIdentifications   9
                if (isSaved && objAll.EmployeeIdentification != null)
                {
                    isSaved = SaveEmployeeIdentifications(objAll.EmployeeIdentification, objAll.BankDetails, trans);
                }
                //Kapil_Career_Model   10
                if (isSaved && objAll.lstkapilcareermodel != null)
                {
                    createKapilCareer(objAll.lstkapilcareermodel, trans);
                }
                #endregion
                trans.Commit();
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employee_Details");
                trans.Rollback();
                throw;
            }
            return isSaved;
        }

        public bool DeleteEmployee(string EmpID)
        {
            bool isDeleted = false;
            try
            {
                string Delete = "UPDATE  TABHRMSEMPLOYEEPERSONALDETAILS SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE VCHEMPLOYEEID='" + EmpID + "';";
                Delete += "update tabhrmsemployeefamilydetails SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE VCHEMPLOYEEID='" + EmpID + "';";
                Delete += "update tabhrmsemployeeemergencycontactdetails SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE VCHEMPLOYEEID='" + EmpID + "';";
                Delete += "update tabhrmsemployeeeducationdetails SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE VCHEMPLOYEEID='" + EmpID + "';";
                Delete += "update tabhrmsemployeeaddressdetails SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE VCHEMPLOYEEID='" + EmpID + "';";
                Delete += "update tabhrmsemployeeemployment SET STATUSID=2,modifiedby=1,modifieddate=current_timestamp WHERE VCHEMPLOYEEID='" + EmpID + "';";
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, Delete);
                isDeleted = true;
            }
            catch (Exception)
            {

                throw;
            }
            return isDeleted;
        }

        public List<EmployeeDetails> GetEmpGridDetails()
        {
            List<EmployeeDetails> lst = new List<EmployeeDetails>();
            try
            {
                string strlocation = "SELECT P.VCHEMPLOYEEID,P.VCHNAME||' '||P.VCHSURNAME AS EMPLOYEENAME,P.VCHMOBILENUMBER,E.VCHDESIGNATION,E.VCHDEPARTMENT FROM TABHRMSEMPLOYEEPERSONALDETAILS P left JOIN TABHRMSEMPLOYEEEMPLOYMENT E ON P.VCHEMPLOYEEID=E.VCHEMPLOYEEID WHERE P.STATUSID=1 order by P.createddate desc;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    EmployeeDetails objEmp = new EmployeeDetails();
                    objEmp.EmployeeID = npgdr["VCHEMPLOYEEID"].ToString();
                    objEmp.EmployeeName = npgdr["EMPLOYEENAME"].ToString().ToUpper();
                    objEmp.Mobilenumber = npgdr["VCHMOBILENUMBER"].ToString();
                    objEmp.Designation = npgdr["VCHDESIGNATION"].ToString().ToUpper();
                    objEmp.Department = npgdr["VCHDEPARTMENT"].ToString().ToUpper();
                    lst.Add(objEmp);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employee_Details");

                throw ex;
            }
            finally
            {
                npgdr.Dispose();
            }
            return lst;
        }

        //public empAll GetEmployeeDetails(string EmpID)
        //{
        //    empAll objEmp = new empAll();
        //    string str_Data = string.Empty;
        //    try
        //    {
        //        str_Data = "SELECT  recordid,vchemployeeid, vchname, vchsurname, datdob, numage, vchplaceofbirth, vchgender, vchmaritalstatus, datdateofmarriage,vchbloodgroup, vchmobilenumber, vchresidentialnumber, vchemail, vchbiometricid, vchrfid, vchimageurl, vchstatus FROM tabhrmsemployeepersonaldetails where vchemployeeid='" + EmpID + "';";
        //        str_Data += "SELECT vchemployeeid, vchcontactpersonname, vchrelationship,vchcontactnumber FROM tabhrmsemployeeemergencycontactdetails where vchemployeeid='" + EmpID + "';";
        //        // str_Data += "SELECT recordid,vchpersonname, vchrelationship, vchisnominee, vchoccupation, vchmobilenumber, datdob FROM tabhrmsemployeefamilydetails where vchemployeeid ='" + EmpID + "';";
        //        str_Data += "SELECT tfd.recordid,tfd.employeeid,tfd.vchemployeeid,tfd.vchpersonname,vchrelationship,vchisnominee,vchoccupation,vchmobilenumber,datdob,vcheducationlevel FROM tabhrmsemployeefamilydetails tfd left join   tabhrmsemployeefamilyeducationdetails tfed on tfd.employeeid=tfed.employeeid and tfd.recordid=tfed.employeefamilyid where tfd.vchemployeeid ='" + EmpID + "';";
        //        str_Data += "SELECT vchdoorno, vchstreet, vcharea, vchcity, vchstate, vchcountry, vchpincode,vchaddresstype,countryid,stateid FROM tabhrmsemployeeaddressdetails where vchemployeeid='" + EmpID + "';";
        //        str_Data += "SELECT recordid,vchcourse, vchgroup, vchschoolorcollege, vchplace, numyear, numpercentageofmarks FROM tabhrmsemployeeeducationdetails where vchemployeeid ='" + EmpID + "';";
        //        str_Data += "SELECT vchcertificateorcourse, datfromdate, dattodate FROM tabhrmsemployeecertificationdetails where vchemployeeid='" + EmpID + "';";
        //        str_Data += "SELECT vchorganization, vchdesignation, datfromdate, dattodate, vchplace, numlastpay, vchreasonforleaving FROM tabhrmsemployeepreviousexperience where vchemployeeid='" + EmpID + "';";
        //        str_Data += "SELECT vchtypeofemployment, vchdesignation, vchdepartment, vchreportingmanager, vchsalarytype, vchpayscalename, numbasic, numvda, vchsalarystructurename, vchleavestructurename, datreportingdate, vchjoinedas, datjoiningdate FROM tabhrmsemployeeemployment where vchemployeeid='" + EmpID + "';";
        //        str_Data += "SELECT VCHESINO, VCHPFSTATUS, VCHPFNO, DATPFEFFETIVEDATE, VCHUANNO, VCHPASSPORTNO, VCHAADHARNO, VCHPANNO, VCHDRIVINGLICENCENO, VCHVEHICLEREGISTRATIONNO, VCHVEHICLEINSURANCENO, DATINSURANCEEXPDATE FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHEMPLOYEEID='" + EmpID + "';";
        //        str_Data += "SELECT recordid,VCHBANKNAME, VCHBRANCH, VCHACCOUNTNO FROM TABHRMSEMPLOYEEBANKACCOUNTDETAILS  where vchemployeeid='" + EmpID + "';";
        //        str_Data += "SELECT vchcompanyname, vchprevemployeeid, vchdesignation, datfromdate, dattodate, vchsscminutesno, vchreasonfortransfer,vchlocation, vchearnedleavesclaimed, datearnedleavesdate, vchkhcno, datvalidfromdate, datvalidtodate  FROM tabhrmsemployeekapilcarrerdetails where vchemployeeid='" + EmpID + "';";
        //        using (NpgsqlConnection connection = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
        //        {
        //            using (NpgsqlCommand command = new NpgsqlCommand(str_Data, connection))
        //            {
        //                connection.Open();
        //                using (NpgsqlDataReader npgdr = command.ExecuteReader())
        //                {
        //                    #region Personal...
        //                    while (npgdr.Read())
        //                    {
        //                        PersonalInformation objPersonal = new PersonalInformation();
        //                        objPersonal.RecordId = npgdr["recordid"].ToString();
        //                        objPersonal.EmployeeId = npgdr["vchemployeeid"].ToString();
        //                        objPersonal.Name = npgdr["vchname"].ToString();
        //                        objPersonal.Sname = npgdr["vchsurname"].ToString();
        //                        if (!string.IsNullOrEmpty(npgdr["datdob"].ToString()))
        //                        {
        //                            objPersonal.DOB = Convert.ToDateTime(npgdr["datdob"]).ToString("dd/MM/yyyy");
        //                        }

        //                        objPersonal.Age = npgdr["numage"].ToString();
        //                        objPersonal.PlaceOfBirth = npgdr["vchplaceofbirth"].ToString();
        //                        if (npgdr["vchgender"].ToString() == "F")
        //                        {
        //                            objPersonal.Gender = "Female";
        //                        }
        //                        else if (npgdr["vchgender"].ToString() == "M")
        //                        {
        //                            objPersonal.Gender = "Male";
        //                        }
        //                        objPersonal.MaritalStatus = npgdr["vchmaritalstatus"].ToString();

        //                        objPersonal.DateOfMarage = npgdr["datdateofmarriage"].ToString();

        //                        objPersonal.BloodGroup = npgdr["vchbloodgroup"].ToString();
        //                        objPersonal.MobileNumber = npgdr["vchmobilenumber"].ToString();
        //                        objPersonal.Res = npgdr["vchresidentialnumber"].ToString();
        //                        objPersonal.Email = npgdr["vchemail"].ToString();
        //                        objPersonal.BioMetricId = npgdr["vchbiometricid"].ToString();
        //                        objPersonal.RFID = npgdr["vchrfid"].ToString();
        //                        objPersonal.UploadUrl = npgdr["vchimageurl"].ToString();
        //                        objPersonal.PresentStatusOfEMP = npgdr["vchstatus"].ToString();
        //                        objEmp.personalinformation = objPersonal;

        //                    }
        //                    #endregion

        //                    #region Contact_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        while (npgdr.Read())
        //                        {
        //                            EmergencyContactDetails objEmergencyContactDetails = new EmergencyContactDetails();
        //                            objEmergencyContactDetails.ContactPersonName = npgdr["vchcontactpersonname"].ToString();
        //                            objEmergencyContactDetails.RelationShip = npgdr["vchrelationship"].ToString();
        //                            objEmergencyContactDetails.ContactNumber = npgdr["vchcontactnumber"].ToString();
        //                            objEmp.emergencycontactdetails = objEmergencyContactDetails;
        //                        }
        //                    }
        //                    #endregion

        //                    #region Family_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        List<EmployeeFamilydetails> lstFamily_Data = new List<EmployeeFamilydetails>();
        //                        while (npgdr.Read())
        //                        {
        //                            EmployeeFamilydetails objFamily = new EmployeeFamilydetails();
        //                            objFamily.recordid = npgdr["recordid"].ToString();
        //                            objFamily.employeeid = npgdr["employeeid"].ToString();
        //                            objFamily.vchemployeeid = npgdr["vchemployeeid"].ToString();
        //                            objFamily.vchpersonname = npgdr["vchpersonname"].ToString();
        //                            objFamily.vchrelationship = npgdr["vchrelationship"].ToString();
        //                            objFamily.vchisnominee = npgdr["vchisnominee"].ToString();
        //                            objFamily.vchoccupation = npgdr["vchoccupation"].ToString();
        //                            objFamily.vchmobilenumber = npgdr["vchmobilenumber"].ToString();
        //                            objFamily.vcheducationlevel = npgdr["vcheducationlevel"].ToString();
        //                            objFamily.RecordStatus = "N";
        //                            if (!string.IsNullOrEmpty(npgdr["datdob"].ToString()))
        //                            {
        //                                objFamily.datdob = Convert.ToDateTime(npgdr["datdob"]).ToString("dd/MM/yyyy");
        //                            }
        //                            // objFamily.datdob = npgdr["datdob"].ToString();
        //                            lstFamily_Data.Add(objFamily);
        //                        }
        //                        objEmp.lstemployeefamilydetails = lstFamily_Data;
        //                    }
        //                    #endregion

        //                    #region Address_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        Addresses objAddresses = new Addresses();
        //                        while (npgdr.Read())
        //                        {
        //                            if (npgdr["vchaddresstype"].ToString() == "CON")
        //                            {
        //                                objAddresses.AddressType = npgdr["vchaddresstype"].ToString();
        //                                objAddresses.DoorNo = npgdr["vchdoorno"].ToString();
        //                                objAddresses.StreetName = npgdr["vchstreet"].ToString();
        //                                objAddresses.Area = npgdr["vcharea"].ToString();
        //                                objAddresses.CityName = npgdr["vchcity"].ToString();
        //                                objAddresses.State = npgdr["vchstate"].ToString();
        //                                objAddresses.Country = npgdr["vchcountry"].ToString();
        //                                objAddresses.PinCode = npgdr["vchpincode"].ToString();
        //                                objAddresses.CountryId = npgdr["countryid"].ToString();
        //                                objAddresses.StateId = npgdr["stateid"].ToString();
        //                            }
        //                            else if (npgdr["vchaddresstype"].ToString() == "PER")
        //                            {
        //                                objAddresses.dAddressType = npgdr["vchaddresstype"].ToString();
        //                                objAddresses.dDoorNo = npgdr["vchdoorno"].ToString();
        //                                objAddresses.dStreetName = npgdr["vchstreet"].ToString();
        //                                objAddresses.dArea = npgdr["vcharea"].ToString();
        //                                objAddresses.dCityName = npgdr["vchcity"].ToString();
        //                                objAddresses.State = npgdr["vchstate"].ToString();
        //                                objAddresses.Country = npgdr["vchcountry"].ToString();
        //                                objAddresses.dPinCode = npgdr["vchpincode"].ToString();
        //                                objAddresses.dCountryId = npgdr["countryid"].ToString();
        //                                objAddresses.dStateId = npgdr["stateid"].ToString();
        //                            }
        //                            objEmp.addresses = objAddresses;
        //                        }
        //                    }
        //                    #endregion

        //                    #region Educational_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        List<EducationDetails> lstFamilyEducation_Data = new List<EducationDetails>();
        //                        while (npgdr.Read())
        //                        {
        //                            EducationDetails objFamilyEducation = new EducationDetails();
        //                            objFamilyEducation.recordid = npgdr["recordid"].ToString();
        //                            objFamilyEducation.Course = npgdr["vchcourse"].ToString();
        //                            objFamilyEducation.Group = npgdr["vchgroup"].ToString();
        //                            objFamilyEducation.SchoolorCollege = npgdr["vchschoolorcollege"].ToString();
        //                            objFamilyEducation.Place = npgdr["vchplace"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["numyear"].ToString()))
        //                            {
        //                                objFamilyEducation.Year = Convert.ToDouble(npgdr["numyear"].ToString());
        //                            }
        //                            if (!string.IsNullOrEmpty(npgdr["numpercentageofmarks"].ToString()))
        //                            {
        //                                objFamilyEducation.MarksPercentage = Convert.ToDouble(npgdr["numpercentageofmarks"].ToString());
        //                            }
        //                            lstFamilyEducation_Data.Add(objFamilyEducation);
        //                        }
        //                        objEmp.lsteducationdetails = lstFamilyEducation_Data;
        //                    }
        //                    #endregion

        //                    #region Certification_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        List<employeeCourses> lstemployeeCourses = new List<employeeCourses>();
        //                        while (npgdr.Read())
        //                        {
        //                            employeeCourses objemployeeCourses = new employeeCourses();
        //                            objemployeeCourses.vchcertificateorcourse = npgdr["vchcertificateorcourse"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["datfromdate"].ToString()))
        //                            {
        //                                objemployeeCourses.dattodate = Convert.ToDateTime(npgdr["datfromdate"]).ToString("dd/MM/yyyy");
        //                                //objemployeeCourses.dattodate = npgdr["datfromdate"].ToString();
        //                            }
        //                            if (!string.IsNullOrEmpty(npgdr["dattodate"].ToString()))
        //                            {
        //                                objemployeeCourses.dattodate = Convert.ToDateTime(npgdr["dattodate"]).ToString("dd/MM/yyyy");
        //                                //objemployeeCourses.dattodate = npgdr["dattodate"].ToString();
        //                            }
        //                            lstemployeeCourses.Add(objemployeeCourses);
        //                        }
        //                        objEmp.lstemployeecourses = lstemployeeCourses;
        //                    }
        //                    #endregion

        //                    #region PrevoiusWork_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        while (npgdr.Read())
        //                        {
        //                            PreviousExperience objPreviousExperience = new PreviousExperience();
        //                            objPreviousExperience.organisation = npgdr["vchorganization"].ToString();
        //                            objPreviousExperience.designation = npgdr["vchdesignation"].ToString();
        //                            objPreviousExperience.frmdate = npgdr["datfromdate"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["dattodate"].ToString()))
        //                            {
        //                                objPreviousExperience.todate = Convert.ToDateTime(npgdr["dattodate"]).ToString("dd/MM/yyyy");
        //                            }
        //                            objPreviousExperience.place = npgdr["vchplace"].ToString();
        //                            objPreviousExperience.lastpay = npgdr["numlastpay"].ToString();
        //                            objPreviousExperience.reasonsforLeaving = npgdr["vchreasonforleaving"].ToString();
        //                            objEmp.previousexperience = objPreviousExperience;
        //                        }
        //                    }
        //                    #endregion

        //                    #region Employment_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        while (npgdr.Read())
        //                        {
        //                            EmploymentDetails objEmploymentDetails = new EmploymentDetails();
        //                            objEmploymentDetails.TypeOfEmployment = npgdr["vchtypeofemployment"].ToString();
        //                            objEmploymentDetails.Designation = npgdr["vchdesignation"].ToString();
        //                            objEmploymentDetails.Department = npgdr["vchdepartment"].ToString();
        //                            objEmploymentDetails.ReportingManager = npgdr["vchreportingmanager"].ToString();
        //                            objEmploymentDetails.SalaryType = npgdr["vchsalarytype"].ToString();
        //                            objEmploymentDetails.PayScale = npgdr["vchpayscalename"].ToString();
        //                            objEmploymentDetails.BasicAmount = npgdr["numbasic"].ToString();
        //                            objEmploymentDetails.VDA = npgdr["numvda"].ToString();
        //                            objEmploymentDetails.SalaryStructure = npgdr["vchsalarystructurename"].ToString();
        //                            objEmploymentDetails.LeaveStructure = npgdr["vchleavestructurename"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["datreportingdate"].ToString()))
        //                            {
        //                                objEmploymentDetails.DateOfReporting = Convert.ToDateTime(npgdr["datreportingdate"]).ToString("dd/MM/yyyy");
        //                            }
        //                            // objEmploymentDetails.DateOfReporting = npgdr["datreportingdate"].ToString();
        //                            objEmploymentDetails.JoinedAs = npgdr["vchjoinedas"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["datjoiningdate"].ToString()))
        //                            {
        //                                objEmploymentDetails.KapilGroupJoinDate = Convert.ToDateTime(npgdr["datjoiningdate"]).ToString("dd/MM/yyyy");
        //                            }
        //                            //  objEmploymentDetails.KapilGroupJoinDate = npgdr["datjoiningdate"].ToString();
        //                            objEmp.employmentdetails = objEmploymentDetails;
        //                        }
        //                    }
        //                    #endregion

        //                    #region Identification_Data..

        //                    if (npgdr.NextResult())
        //                    {
        //                        while (npgdr.Read())
        //                        {
        //                            EmployeeIdentification employeeidentification = new EmployeeIdentification();
        //                            employeeidentification.Pfno = npgdr["VCHPFNO"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["DATPFEFFETIVEDATE"].ToString()))
        //                            {
        //                                employeeidentification.PfEffectiveDate = Convert.ToDateTime(npgdr["DATPFEFFETIVEDATE"]).ToString("dd/MM/yyyy");
        //                            }
        //                            employeeidentification.EsiNo = npgdr["VCHESINO"].ToString();
        //                            employeeidentification.UanNo = npgdr["VCHUANNO"].ToString();
        //                            employeeidentification.PassportNo = npgdr["VCHPASSPORTNO"].ToString();
        //                            employeeidentification.AadharNo = npgdr["VCHAADHARNO"].ToString();
        //                            employeeidentification.PancardNo = npgdr["VCHPANNO"].ToString();
        //                            employeeidentification.DrivingLicenceNo = npgdr["VCHDRIVINGLICENCENO"].ToString();
        //                            employeeidentification.VehicleRegistrationNo = npgdr["VCHVEHICLEREGISTRATIONNO"].ToString();
        //                            employeeidentification.VehicleInsuranceNo = npgdr["VCHVEHICLEINSURANCENO"].ToString();
        //                            if (!string.IsNullOrEmpty(npgdr["DATINSURANCEEXPDATE"].ToString()))
        //                            {
        //                                employeeidentification.InsuranceDate = Convert.ToDateTime(npgdr["DATINSURANCEEXPDATE"]).ToString();
        //                            }
        //                            objEmp.EmployeeIdentification = employeeidentification;
        //                        }
        //                    }
        //                    if (npgdr.NextResult())
        //                    {
        //                        List<BankDetails> lstBankDetails = new List<BankDetails>();
        //                        while (npgdr.Read())
        //                        {
        //                            BankDetails objBankDetails = new BankDetails();
        //                            objBankDetails.BankAccountNo = npgdr["VCHACCOUNTNO"].ToString();
        //                            objBankDetails.Bankname = npgdr["VCHBANKNAME"].ToString();
        //                            objBankDetails.Branchname = npgdr["VCHBRANCH"].ToString();
        //                            objBankDetails.Bankrecordid = Convert.ToInt32(npgdr["recordid"]);
        //                            lstBankDetails.Add(objBankDetails);
        //                        }
        //                        objEmp.BankDetails = lstBankDetails;
        //                    }
        //                    #endregion

        //                    #region KapilCareer_Data..
        //                    if (npgdr.NextResult())
        //                    {
        //                        List<KapilCareerModel> lstKapilCareerModel = new List<KapilCareerModel>();
        //                        while (npgdr.Read())
        //                        {
        //                            KapilCareerModel objKapilCareerModel = new KapilCareerModel();
        //                            objKapilCareerModel.CompanyName = npgdr["vchcompanyname"].ToString();
        //                            objKapilCareerModel.vchEmlpoyeeID = npgdr["vchprevemployeeid"].ToString();
        //                            objKapilCareerModel.Designation = npgdr["vchdesignation"].ToString();
        //                            objKapilCareerModel.kc_fromdate = npgdr["datfromdate"].ToString();
        //                            objKapilCareerModel.kc_todate = npgdr["dattodate"].ToString();
        //                            objKapilCareerModel.sscminutesno = npgdr["vchsscminutesno"].ToString();
        //                            objKapilCareerModel.ReasonTransfer = npgdr["vchreasonfortransfer"].ToString();
        //                            objKapilCareerModel.location = npgdr["vchlocation"].ToString();
        //                            objKapilCareerModel.ELClaimed = npgdr["vchearnedleavesclaimed"].ToString();
        //                            objKapilCareerModel.EL_date = npgdr["datearnedleavesdate"].ToString();
        //                            objKapilCareerModel.KHCNo = npgdr["vchkhcno"].ToString();
        //                            objKapilCareerModel.kc_fromdate = npgdr["datvalidfromdate"].ToString();
        //                            objKapilCareerModel.kc_todate = npgdr["datvalidtodate"].ToString();
        //                            lstKapilCareerModel.Add(objKapilCareerModel);

        //                        }
        //                        objEmp.lstkapilcareermodel = lstKapilCareerModel;
        //                    }
        //                    #endregion
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "Employee_Details");
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //npgdr.Dispose();
        //    }
        //    return objEmp;
        //}

        public empAll GetEmployeeDetails(string EmpID)
        {
            empAll objEmp = new empAll();
            string str_Data = string.Empty;

            try
            {
                str_Data = "SELECT  recordid,vchemployeeid, vchname, vchsurname, datdob, numage, vchplaceofbirth, vchgender, vchmaritalstatus, datdateofmarriage,vchbloodgroup, vchmobilenumber, vchresidentialnumber, vchemail, vchbiometricid, vchrfid, vchimageurl, vchstatus FROM tabhrmsemployeepersonaldetails where vchemployeeid='" + EmpID + "';";
                str_Data += "SELECT vchemployeeid, vchcontactpersonname, vchrelationship,vchcontactnumber FROM tabhrmsemployeeemergencycontactdetails where vchemployeeid='" + EmpID + "';";
                str_Data += "SELECT tfd.recordid,tfd.employeeid,tfd.vchemployeeid,tfd.vchpersonname,vchrelationship,vchisnominee,vchoccupation,vchmobilenumber,datdob,vcheducationlevel FROM tabhrmsemployeefamilydetails tfd left join   tabhrmsemployeefamilyeducationdetails tfed on tfd.employeeid=tfed.employeeid and tfd.recordid=tfed.employeefamilyid where tfd.vchemployeeid ='" + EmpID + "';";
                str_Data += "SELECT vchdoorno, vchstreet, vcharea, vchcity, vchstate, vchcountry, vchpincode,vchaddresstype,countryid,stateid FROM tabhrmsemployeeaddressdetails where vchemployeeid='" + EmpID + "';";
                str_Data += "SELECT recordid,vchcourse, vchgroup, vchschoolorcollege, vchplace, numyear, numpercentageofmarks FROM tabhrmsemployeeeducationdetails where vchemployeeid ='" + EmpID + "';";
                //  str_Data += "SELECT vchcertificateorcourse, datfromdate, dattodate FROM tabhrmsemployeecertificationdetails where vchemployeeid='" + EmpID + "';";
                // str_Data += "SELECT vchorganization, vchdesignation, datfromdate, dattodate, vchplace, numlastpay, vchreasonforleaving FROM tabhrmsemployeepreviousexperience where vchemployeeid='" + EmpID + "';";
                str_Data += "SELECT vchtypeofemployment, vchdesignation, vchdepartment, vchreportingmanager, vchsalarytype, vchpayscalename, numbasic, numvda, vchsalarystructurename, vchleavestructurename, datreportingdate, vchjoinedas, datjoiningdate FROM tabhrmsemployeeemployment where vchemployeeid='" + EmpID + "';";
                // str_Data += "SELECT VCHESINO, VCHPFSTATUS, VCHPFNO, DATPFEFFETIVEDATE, VCHUANNO, VCHPASSPORTNO, VCHAADHARNO, VCHPANNO, VCHDRIVINGLICENCENO, VCHVEHICLEREGISTRATIONNO, VCHVEHICLEINSURANCENO, DATINSURANCEEXPDATE FROM TABHRMSEMPLOYEEIDENTIFICATIONDETAILS WHERE VCHEMPLOYEEID='" + EmpID + "';";
                //str_Data += "SELECT recordid,VCHBANKNAME, VCHBRANCH, VCHACCOUNTNO FROM TABHRMSEMPLOYEEBANKACCOUNTDETAILS  where vchemployeeid='" + EmpID + "';";
                //str_Data += "SELECT vchcompanyname, vchprevemployeeid, vchdesignation, datfromdate, dattodate, vchsscminutesno, vchreasonfortransfer,vchlocation, vchearnedleavesclaimed, datearnedleavesdate, vchkhcno, datvalidfromdate, datvalidtodate  FROM tabhrmsemployeekapilcarrerdetails where vchemployeeid='" + EmpID + "';";
                using (NpgsqlConnection connection = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(str_Data, connection))
                    {
                        connection.Open();
                        using (NpgsqlDataReader npgdr = command.ExecuteReader())
                        {
                            #region Personal...
                            while (npgdr.Read())
                            {
                                PersonalInformation objPersonal = new PersonalInformation();
                                objPersonal.RecordId = npgdr["recordid"].ToString();
                                objPersonal.EmployeeId = npgdr["vchemployeeid"].ToString();
                                objPersonal.Name = npgdr["vchname"].ToString();
                                objPersonal.Sname = npgdr["vchsurname"].ToString();
                                if (!string.IsNullOrEmpty(npgdr["datdob"].ToString()))
                                {
                                    objPersonal.DOB = Convert.ToDateTime(npgdr["datdob"]).ToString("dd/MM/yyyy");
                                }

                                objPersonal.Age = npgdr["numage"].ToString();
                                objPersonal.PlaceOfBirth = npgdr["vchplaceofbirth"].ToString();
                                if (npgdr["vchgender"].ToString() == "F")
                                {
                                    objPersonal.Gender = "Female";
                                }
                                else if (npgdr["vchgender"].ToString() == "M")
                                {
                                    objPersonal.Gender = "Male";
                                }
                                objPersonal.MaritalStatus = npgdr["vchmaritalstatus"].ToString();

                                objPersonal.DateOfMarage = npgdr["datdateofmarriage"].ToString();

                                objPersonal.BloodGroup = npgdr["vchbloodgroup"].ToString();
                                objPersonal.MobileNumber = npgdr["vchmobilenumber"].ToString();
                                objPersonal.Res = npgdr["vchresidentialnumber"].ToString();
                                objPersonal.Email = npgdr["vchemail"].ToString();
                                objPersonal.BioMetricId = npgdr["vchbiometricid"].ToString();
                                objPersonal.RFID = npgdr["vchrfid"].ToString();
                                objPersonal.UploadUrl = npgdr["vchimageurl"].ToString();
                                objPersonal.PresentStatusOfEMP = npgdr["vchstatus"].ToString();
                                objEmp.personalinformation = objPersonal;

                            }
                            #endregion

                            #region Contact_Data..
                            if (npgdr.NextResult())
                            {
                                while (npgdr.Read())
                                {
                                    EmergencyContactDetails objEmergencyContactDetails = new EmergencyContactDetails();
                                    objEmergencyContactDetails.ContactPersonName = npgdr["vchcontactpersonname"].ToString();
                                    objEmergencyContactDetails.RelationShip = npgdr["vchrelationship"].ToString();
                                    objEmergencyContactDetails.ContactNumber = npgdr["vchcontactnumber"].ToString();
                                    objEmp.emergencycontactdetails = objEmergencyContactDetails;
                                }
                            }
                            #endregion

                            #region Family_Data..
                            if (npgdr.NextResult())
                            {
                                List<EmployeeFamilydetails> lstFamily_Data = new List<EmployeeFamilydetails>();
                                while (npgdr.Read())
                                {
                                    EmployeeFamilydetails objFamily = new EmployeeFamilydetails();
                                    objFamily.recordid = npgdr["recordid"].ToString();
                                    objFamily.employeeid = npgdr["employeeid"].ToString();
                                    objFamily.vchemployeeid = npgdr["vchemployeeid"].ToString();
                                    objFamily.vchpersonname = npgdr["vchpersonname"].ToString();
                                    objFamily.vchrelationship = npgdr["vchrelationship"].ToString();
                                    objFamily.vchisnominee = npgdr["vchisnominee"].ToString();
                                    objFamily.vchoccupation = npgdr["vchoccupation"].ToString();
                                    objFamily.vchmobilenumber = npgdr["vchmobilenumber"].ToString();
                                    objFamily.vcheducationlevel = npgdr["vcheducationlevel"].ToString();
                                    objFamily.RecordStatus = "N";
                                    if (!string.IsNullOrEmpty(npgdr["datdob"].ToString()))
                                    {
                                        objFamily.datdob = Convert.ToDateTime(npgdr["datdob"]).ToString("dd/MM/yyyy");
                                    }
                                    // objFamily.datdob = npgdr["datdob"].ToString();
                                    lstFamily_Data.Add(objFamily);
                                }
                                objEmp.lstemployeefamilydetails = lstFamily_Data;
                            }
                            #endregion

                            #region Address_Data..
                            if (npgdr.NextResult())
                            {
                                Addresses objAddresses = new Addresses();
                                while (npgdr.Read())
                                {
                                    if (npgdr["vchaddresstype"].ToString() == "CON")
                                    {
                                        objAddresses.AddressType = npgdr["vchaddresstype"].ToString();
                                        objAddresses.DoorNo = npgdr["vchdoorno"].ToString();
                                        objAddresses.StreetName = npgdr["vchstreet"].ToString();
                                        objAddresses.Area = npgdr["vcharea"].ToString();
                                        objAddresses.CityName = npgdr["vchcity"].ToString();
                                        objAddresses.State = npgdr["vchstate"].ToString();
                                        objAddresses.Country = npgdr["vchcountry"].ToString();
                                        objAddresses.PinCode = npgdr["vchpincode"].ToString();
                                        objAddresses.CountryId = npgdr["countryid"].ToString();
                                        objAddresses.StateId = npgdr["stateid"].ToString();
                                    }
                                    else if (npgdr["vchaddresstype"].ToString() == "PER")
                                    {
                                        objAddresses.dAddressType = npgdr["vchaddresstype"].ToString();
                                        objAddresses.dDoorNo = npgdr["vchdoorno"].ToString();
                                        objAddresses.dStreetName = npgdr["vchstreet"].ToString();
                                        objAddresses.dArea = npgdr["vcharea"].ToString();
                                        objAddresses.dCityName = npgdr["vchcity"].ToString();
                                        objAddresses.State = npgdr["vchstate"].ToString();
                                        objAddresses.Country = npgdr["vchcountry"].ToString();
                                        objAddresses.dPinCode = npgdr["vchpincode"].ToString();
                                        objAddresses.dCountryId = npgdr["countryid"].ToString();
                                        objAddresses.dStateId = npgdr["stateid"].ToString();
                                    }
                                    objEmp.addresses = objAddresses;
                                }
                            }
                            #endregion

                            #region Educational_Data..
                            if (npgdr.NextResult())
                            {
                                List<EducationDetails> lstFamilyEducation_Data = new List<EducationDetails>();
                                while (npgdr.Read())
                                {
                                    EducationDetails objFamilyEducation = new EducationDetails();
                                    objFamilyEducation.recordid = npgdr["recordid"].ToString();
                                    objFamilyEducation.Course = npgdr["vchcourse"].ToString();
                                    objFamilyEducation.Group = npgdr["vchgroup"].ToString();
                                    objFamilyEducation.SchoolorCollege = npgdr["vchschoolorcollege"].ToString();
                                    objFamilyEducation.Place = npgdr["vchplace"].ToString();
                                    if (!string.IsNullOrEmpty(npgdr["numyear"].ToString()))
                                    {
                                        objFamilyEducation.Year = Convert.ToDouble(npgdr["numyear"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(npgdr["numpercentageofmarks"].ToString()))
                                    {
                                        objFamilyEducation.MarksPercentage = Convert.ToDouble(npgdr["numpercentageofmarks"].ToString());
                                    }
                                    lstFamilyEducation_Data.Add(objFamilyEducation);
                                }
                                objEmp.lsteducationdetails = lstFamilyEducation_Data;
                            }
                            #endregion

                            #region Employment_Data..
                            if (npgdr.NextResult())
                            {
                                while (npgdr.Read())
                                {
                                    EmploymentDetails objEmploymentDetails = new EmploymentDetails();
                                    objEmploymentDetails.TypeOfEmployment = npgdr["vchtypeofemployment"].ToString();
                                    objEmploymentDetails.Designation = npgdr["vchdesignation"].ToString();
                                    objEmploymentDetails.Department = npgdr["vchdepartment"].ToString();
                                    objEmploymentDetails.ReportingManager = npgdr["vchreportingmanager"].ToString();
                                    objEmploymentDetails.SalaryType = npgdr["vchsalarytype"].ToString();
                                    objEmploymentDetails.PayScale = npgdr["vchpayscalename"].ToString();
                                    objEmploymentDetails.BasicAmount = npgdr["numbasic"].ToString();
                                    objEmploymentDetails.VDA = npgdr["numvda"].ToString();
                                    objEmploymentDetails.SalaryStructure = npgdr["vchsalarystructurename"].ToString();
                                    objEmploymentDetails.LeaveStructure = npgdr["vchleavestructurename"].ToString();
                                    if (!string.IsNullOrEmpty(npgdr["datreportingdate"].ToString()))
                                    {
                                        objEmploymentDetails.DateOfReporting = Convert.ToDateTime(npgdr["datreportingdate"]).ToString("dd/MM/yyyy");
                                    }
                                    // objEmploymentDetails.DateOfReporting = npgdr["datreportingdate"].ToString();
                                    objEmploymentDetails.JoinedAs = npgdr["vchjoinedas"].ToString();
                                    if (!string.IsNullOrEmpty(npgdr["datjoiningdate"].ToString()))
                                    {
                                        objEmploymentDetails.KapilGroupJoinDate = Convert.ToDateTime(npgdr["datjoiningdate"]).ToString("dd/MM/yyyy");
                                    }
                                    //  objEmploymentDetails.KapilGroupJoinDate = npgdr["datjoiningdate"].ToString();
                                    objEmp.employmentdetails = objEmploymentDetails;
                                }
                            }
                            #endregion


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employee_Details");
                throw ex;
            }
            finally
            {
                //npgdr.Dispose();
            }
            return objEmp;
        }

        public bool UpdateEmployee(empAll objAll)
        {
            bool isUpdated = false;
            con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            trans = con.BeginTransaction();
            try
            {
                UpdateEmpID = objAll.personalinformation.EmployeeId;
                #region All..
                //PERSONALINFORMATION   1
                if (objAll.personalinformation != null)
                {
                    isUpdated = UpdateEmolyeePersonalDetails(objAll.personalinformation, trans);
                }
                //Emergency_Contact_Details   
                if (isUpdated && objAll.emergencycontactdetails != null)
                {
                    isUpdated = UpdateEmolyeeEmergencyContactDetails(objAll.emergencycontactdetails, trans);
                }
                //Employee_Family_Details   2
                if (isUpdated && objAll.lstemployeefamilydetails.Count > 0)
                {
                    isUpdated = UpdateEmolyeeFamilyDetails(objAll.lstemployeefamilydetails, trans);
                }
                //Address   3
                if (isUpdated && objAll.addresses != null)
                {
                    isUpdated = UpdateEmolyeeAddress(objAll.addresses, trans);
                }
                //Education_Details   4
                if (isUpdated && objAll.lsteducationdetails.Count > 0)
                {
                    isUpdated = UpdateEmolyeeEducationDetails(objAll.lsteducationdetails, trans);
                }

                //Employment_Details   5
                if (isUpdated && objAll.employmentdetails != null)
                {
                    isUpdated = UpdateEmolyeeEmployment(objAll.employmentdetails, trans);
                }

                #endregion
                if (isUpdated)
                {
                    trans.Commit();

                }
                else
                {
                    trans.Rollback();
                }


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employee_Details");
                trans.Rollback();
                throw;
            }
            return isUpdated;
        }

        private void UpdateEmolyeeKapilCareer(List<KapilCareerModel> lstkapilCareerModel, NpgsqlTransaction trans)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool UpdateEmolyeeEmployment(EmploymentDetails employmentDetails, NpgsqlTransaction trans)
        {
            bool isUPdated = false;
            string strEmployment = string.Empty;
            try
            {
                //string strCount = "select count(*) from tabhrmsemployeeemployment where vchemployeeid='" + UpdateEmpID + "';";
                //int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strCount));

                //if (count > 0)
                //{

                if (string.IsNullOrEmpty(employmentDetails.VDA))
                {
                    employmentDetails.VDA = "0";
                }

                strEmployment = "update tabhrmsemployeeemployment set vchtypeofemployment='" + ManageQuote(employmentDetails.TypeOfEmployment) + "',vchdesignation='" + ManageQuote(employmentDetails.Designation) + "',vchdepartment='" + ManageQuote(employmentDetails.Department) + "',vchreportingmanager='" + ManageQuote(employmentDetails.ReportingManager) + "',vchsalarytype='" + ManageQuote(employmentDetails.SalaryType) + "',vchpayscalename='" + ManageQuote(employmentDetails.PayScale) + "',numbasic=" + employmentDetails.BasicAmount + ",numvda='" + employmentDetails.VDA + "',vchsalarystructurename='" + ManageQuote(employmentDetails.SalaryStructure) + "',vchleavestructurename='" + ManageQuote(employmentDetails.LeaveStructure) + "',datreportingdate='" + FormatDate(employmentDetails.DateOfReporting) + "',vchjoinedas='" + ManageQuote(employmentDetails.JoinedAs) + "',datjoiningdate='" + FormatDate(employmentDetails.KapilGroupJoinDate) + "',modifiedby=1,modifieddate=current_timestamp where vchemployeeid='" + UpdateEmpID + "';";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strEmployment);
                isUPdated = true;
                //}
                //else
                //{

                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isUPdated;
        }

        private void UpdateEmolyeePreviousExperience(PreviousExperience previousExperience, NpgsqlTransaction trans)
        {
            try
            {
                //UpdateEmpID
                string countemployeepreviousexp = "select count(*) from tabhrmsemployeepreviousexperience where vchemployeeid='" + UpdateEmpID + "'";

                int count = (int)NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, countemployeepreviousexp);

                if (count >= 1)
                {
                    string updateemployeePrevousexp = "update tabhrmsemployeepreviousexperience set vchorganization='" + ManageQuote(previousExperience.organisation) + "',vchdesignation='" + ManageQuote(previousExperience.designation) + "',datfromdate='" + FormatDate(previousExperience.frmdate) + "',dattodate='" + FormatDate(previousExperience.todate) + "',vchplace='" + ManageQuote(previousExperience.place) + "',numlastpay=" + Convert.ToDouble(previousExperience.lastpay) + ",vchreasonforleaving='" + previousExperience.reasonsforLeaving + "' where vchemployeeid='" + UpdateEmpID + "'";

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateemployeePrevousexp);

                }

                else
                {
                    string InsertEmployeePreviousExp = @"INSERT INTO tabhrmsemployeepreviousexperience(employeeid,vchemployeeid,vchorganization,vchdesignation,datfromdate,dattodate,vchplace,numlastpay,vchreasonforleaving,statusid,createdby,createddate) values(" + EmpRecordId + ",'" +
                    UpdateEmpID + "','" + previousExperience.organisation + "','" + previousExperience.designation + "','" + FormatDate(previousExperience.frmdate) + "','" + FormatDate(previousExperience.todate) + "','" + previousExperience.place + "'," + previousExperience.lastpay + ",'" + previousExperience.reasonsforLeaving + "',1,1,current_timestamp);";

                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, InsertEmployeePreviousExp);
                }

            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "Previous_Experience");
            }
        }

        private void UpdateEmolyeeCertificateCourse(List<employeeCourses> lstemployeeCourses, NpgsqlTransaction trans)
        {
            throw new NotImplementedException();
        }

        private bool UpdateEmolyeeEducationDetails(List<EducationDetails> lsteducationDetails, NpgsqlTransaction trans)
        {
            bool isUpdated = false;
            try
            {

                string strDelete = "delete from tabhrmsemployeeeducationdetails  where VCHEMPLOYEEID='" + UpdateEmpID + "' ";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDelete);

                isUpdated = CreateEducationDetails(lsteducationDetails, trans);

                //UpdateEmpID
                #region Old...


                //string countemployee = "select count(*) from tabhrmsemployeeeducationdetails where vchemployeeid='" + UpdateEmpID + "'";

                //int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, countemployee));
                //for (int i = 0; i < lsteducationDetails.Count; i++)
                //{
                //    //if (count >= 1)
                //    //{
                //    string updateemployeeeducationdetails = "update tabhrmsemployeeeducationdetails set vchcourse='" + ManageQuote(lsteducationDetails[i].Course) + "',vchgroup='" + ManageQuote(lsteducationDetails[i].Group) + "',vchschoolorcollege='" + ManageQuote(lsteducationDetails[i].SchoolorCollege) + "',vchplace='" + ManageQuote(lsteducationDetails[i].Place) + "',numyear=" + Convert.ToInt32(lsteducationDetails[i].Year) + ",numpercentageofmarks=" + Convert.ToDouble(lsteducationDetails[i].MarksPercentage) + " where vchemployeeid='" + UpdateEmpID + "'";
                //    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, updateemployeeeducationdetails);
                //    isUPdated = true;
                //    //}
                //    //else
                //    //{
                //    //    string InsertEmployeeDetails = "insert into tabhrmsemployeeeducationdetails(employeeid,vchemployeeid,vchcourse,vchgroup,vchschoolorcollege,vchplace,numyear,numpercentageofmarks,statusid,createdby,createddate ) values (" + EmpRecordId + ",'" + UpdateEmpID + "','" + ManageQuote(lsteducationDetails[i].Course) + "' ,'" + ManageQuote(lsteducationDetails[i].Group) + "','" + ManageQuote(lsteducationDetails[i].SchoolorCollege) + "','" + ManageQuote(lsteducationDetails[i].Place) + "','" + Convert.ToDouble(lsteducationDetails[i].Year) + "','" + Convert.ToDouble(lsteducationDetails[i].MarksPercentage) + "',1,1,current_timestamp);";
                //    //    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, InsertEmployeeDetails);
                //    //    isUPdated = true;
                //    //}
                //}
                #endregion
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "Education_Details");
            }
            return isUpdated;
        }

        //private bool UpdateEmolyeeAddress(Addresses addresses, NpgsqlTransaction trans)
        //{
        //    bool isUpdated = false;
        //    try
        //    {
        //        string Query = string.Empty;
        //        if (addresses.DoorNo != string.Empty && addresses.Area != null && addresses.Country != null && addresses.State != null && addresses.CityName != null && addresses.PinCode != null || addresses.StreetName != null)
        //        {
        //            if (addresses.AddressType != null)
        //            {
        //                if (addresses.AddressType == "CON")
        //                {
        //                    Query = "update tabhrmsemployeeaddressdetails set (vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,modifiedby,modifieddate,countryid,stateid) VALUES('" + ManageQuote(addresses.DoorNo) + "','" + ManageQuote(addresses.StreetName) + "','" + ManageQuote(addresses.Area) + "','" + ManageQuote(addresses.Country) + "','" + ManageQuote(addresses.State) + "','" + ManageQuote(addresses.CityName) + "','" + addresses.PinCode + "',2,CURRENT_TIMESTAMP," + addresses.CountryId + "," + addresses.StateId + ") where VCHEMPLOYEEID='" + UpdateEmpID + "' ";
        //                    int i = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
        //                    if (i > 0)
        //                    {
        //                        if (addresses.dDoorNo != null && addresses.dArea != null && addresses.CountryId != null && addresses.StateId != null && addresses.CityId != null && addresses.dPinCode != null || addresses.dStreetName != null)
        //                        {
        //                            if (addresses.AddressType == "PER")
        //                            {
        //                                Query = "update tabhrmsemployeeaddressdetails set (vchdoorno,vchstreet,vcharea,vchcountry,vchstate,vchcity,vchpincode,modifiedby,modifieddate,countryid,stateid) VALUES('" + ManageQuote(addresses.dDoorNo) + "','" + ManageQuote(addresses.dStreetName) + "','" + ManageQuote(addresses.dArea) + "','" + ManageQuote(addresses.dCountry) + "','" + ManageQuote(addresses.dState) + "','" + ManageQuote(addresses.dCityName) + "','" + addresses.dPinCode + "',2,CURRENT_TIMESTAMP," + addresses.dCountryId + "," + addresses.dStateId + ") where VCHEMPLOYEEID='" + UpdateEmpID + "' ";
        //                                int j = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
        //                                if (j > 0)
        //                                {

        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EventLogger.WriteToErrorLog(ex, "Family_Details");
        //    }
        //    return isUpdated;
        //}
        private bool UpdateEmolyeeAddress(Addresses addresses, NpgsqlTransaction trans)
        {
            bool isUpdated = false;
            try
            {
                string Query = string.Empty;

                if (addresses.SameOrNot)
                {
                    Query = "update tabhrmsemployeeaddressdetails set  vchdoorno='" + ManageQuote(addresses.DoorNo) + "',vchstreet='" + ManageQuote(addresses.StreetName) + "',vcharea='" + ManageQuote(addresses.Area) + "',vchcountry='" + ManageQuote(addresses.Country) + "',vchstate='" + ManageQuote(addresses.State) + "',vchcity='" + ManageQuote(addresses.CityName) + "',vchpincode='" + addresses.PinCode + "',countryid=" + addresses.CountryId + ",stateid=" + addresses.StateId + ",modifiedby=2,modifieddate=CURRENT_TIMESTAMP where VCHEMPLOYEEID='" + UpdateEmpID + "' ";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                    isUpdated = true;
                }
                else
                {
                    if (addresses.AddressType == "CON")
                    {
                        Query = "update tabhrmsemployeeaddressdetails set  vchdoorno='" + ManageQuote(addresses.DoorNo) + "',vchstreet='" + ManageQuote(addresses.StreetName) + "',vcharea='" + ManageQuote(addresses.Area) + "',vchcountry='" + ManageQuote(addresses.Country) + "',vchstate='" + ManageQuote(addresses.State) + "',vchcity='" + ManageQuote(addresses.CityName) + "',vchpincode='" + addresses.PinCode + "',countryid=" + addresses.CountryId + ",stateid=" + addresses.StateId + ",modifiedby=2,modifieddate=CURRENT_TIMESTAMP where VCHEMPLOYEEID='" + UpdateEmpID + "' and vchaddresstype='" + ManageQuote(addresses.AddressType) + "' ";
                        int i = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                        isUpdated = true;
                    }
                    if (addresses.dAddressType == "PER")
                    {
                        Query = "update tabhrmsemployeeaddressdetails set  vchdoorno='" + ManageQuote(addresses.dDoorNo) + "',vchstreet='" + ManageQuote(addresses.dStreetName) + "',vcharea='" + ManageQuote(addresses.dArea) + "',vchcountry='" + ManageQuote(addresses.dCountry) + "',vchstate='" + ManageQuote(addresses.dState) + "',vchcity='" + ManageQuote(addresses.dCityName) + "',vchpincode='" + addresses.dPinCode + "',countryid=" + addresses.dCountryId + ",stateid=" + addresses.dStateId + ",modifiedby=2,modifieddate=CURRENT_TIMESTAMP where VCHEMPLOYEEID='" + UpdateEmpID + "' and vchaddresstype='" + ManageQuote(addresses.dAddressType) + "'  ";
                        int j = NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                        isUpdated = true;
                    }
                }



            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Addresses");
            }
            return isUpdated;
        }

        private bool UpdateEmolyeeFamilyDetails(List<EmployeeFamilydetails> lstemployeeFamilydetails, NpgsqlTransaction trans)
        {
            bool isUpdated = false;
            try
            {
                string strDelete = "delete from tabhrmsemployeefamilyeducationdetails  where VCHEMPLOYEEID='" + UpdateEmpID + "' ;delete from TABHRMSEMPLOYEEFAMILYDETAILS  where VCHEMPLOYEEID='" + UpdateEmpID + "' ";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDelete);

                isUpdated = CreateEmpFamilyDetails(lstemployeeFamilydetails, trans);

                #region StatusWise


                //for (int i = 0; i < lstemployeeFamilydetails.Count; i++)
                //{
                //    if (lstemployeeFamilydetails[i].RecordStatus == "N")
                //    {

                //    }
                //    else if (lstemployeeFamilydetails[i].RecordStatus == "U")
                //    {
                //        string Query = "update TABHRMSEMPLOYEEFAMILYDETAILS set (VCHPERSONNAME,VCHRELATIONSHIP,VCHISNOMINEE,VCHOCCUPATION,VCHMOBILENUMBER,DATDOB,STATUSID,CREATEDBY,CREATEDDATE) =('" + ManageQuote(lstemployeeFamilydetails[i].vchpersonname) + "','" + ManageQuote(lstemployeeFamilydetails[i].vchrelationship) + "',CASE WHEN UPPER('False')=UPPER('" + lstemployeeFamilydetails[i].vchisnominee + "') THEN 0 ELSE 1 END,'" + ManageQuote(lstemployeeFamilydetails[i].vchoccupation) + "','" + ManageQuote(lstemployeeFamilydetails[i].vchmobilenumber) + "','" + FormatDate(lstemployeeFamilydetails[i].datdob) + "',1,1,CURRENT_TIMESTAMP) where VCHEMPLOYEEID='" + UpdateEmpID + "' ";
                //        if (lstemployeeFamilydetails[i].vcheducationlevel != null)
                //        {
                //            string Query2 = " update tabhrmsemployeefamilyeducationdetails set (vchpersonname,vcheducationlevel,statusid,createdby,createddate) =('" + ManageQuote(lstemployeeFamilydetails[i].vchpersonname) + "','" + ManageQuote(lstemployeeFamilydetails[i].vcheducationlevel) + "',1,1,CURRENT_TIMESTAMP) where vchemployeeid='" + UpdateEmpID + "'";
                //            NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query2);
                //        }
                //        NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, Query);
                //        isUpdated = true;
                //    }
                //    else if (lstemployeeFamilydetails[i].RecordStatus == "D")
                //    {


                //    }
                //}
                #endregion

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Family_Details");
            }
            return isUpdated;
        }

        private bool UpdateEmolyeeEmergencyContactDetails(EmergencyContactDetails emergencyContactDetails, NpgsqlTransaction trans)
        {
            bool isUpdated = false;
            try
            {
                long Count = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select Count(*) from tabhrmsemployeeemergencycontactdetails where vchemployeeid='" + UpdateEmpID + "';"));

                //if (Count > 0)
                //{
                string strInsert = "update tabhrmsemployeeemergencycontactdetails set (vchcontactpersonname,vchrelationship,vchcontactnumber,statusid,modifiedby,modifieddate)=('" + ManageQuote(emergencyContactDetails.ContactPersonName) + "','" + ManageQuote(emergencyContactDetails.RelationShip) + "'," + Convert.ToInt64(emergencyContactDetails.ContactNumber) + ",1,1,current_timestamp) where vchemployeeid='" + UpdateEmpID + "';";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                //}
                //else
                //{
                //    //string strInsert = "insert into tabhrmsemployeeemergencycontactdetails(employeeid,vchemployeeid,vchcontactpersonname,vchrelationship,vchcontactnumber,statusid,createdby,createddate)values(" + EmpRecordId + ",'" + UpdateEmpID + "','" + ManageQuote(emergencyContactDetails.ContactPersonName) + "','" + ManageQuote(emergencyContactDetails.RelationShip) + "'," + Convert.ToInt64(emergencyContactDetails.ContactNumber) + ",1,1,current_timestamp);";
                //   // NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                //}
                isUpdated = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Contact_details");
                throw;
            }
            return isUpdated;
        }

        private bool UpdateEmolyeePersonalDetails(PersonalInformation User, NpgsqlTransaction trans)
        {
            bool isUpdated = false;
            string Gender = string.Empty;
            try
            {
                if (User.Gender == "Male")
                {
                    Gender = "M";
                }
                else
                {
                    Gender = "F";
                }
                if (String.IsNullOrEmpty(User.RFID))
                {
                    User.RFID = "0";
                }
                string marieddate = Convert.ToDateTime(User.DateOfMarage).ToString("dd/MM/yyyy");

                //  string strupdate = "update  tabhrmsemployeepersonaldetails set (datdob, numage,vchplaceofbirth, vchgender, vchmaritalstatus, datdateofmarriage, vchbloodgroup, vchmobilenumber, vchresidentialnumber, vchemail, vchbiometricid, vchrfid, vchimageurl, vchstatus, statusid, modifiedby,modifieddate)= (  '" + FormatDate(User.DOB) + "', " + Convert.ToInt64(User.Age) + ", '" + ManageQuote(User.PlaceOfBirth) + "', '" + Gender + "', '" + ManageQuote(User.MaritalStatus) + "','" + FormatDate(marieddate) + "', '" + ManageQuote(User.BloodGroup) + "', " + Convert.ToInt64(User.MobileNumber) + ", " + Convert.ToInt64(User.Res) + ", '" + ManageQuote(User.Email) + "', '" + ManageQuote(User.BioMetricId) + "', '" + ManageQuote(User.RFID) + "', '" + User.UploadUrl + "', '" + 'Y' + "', " + 1 + ", '" + 1 + "', Current_timestamp) where vchemployeeid='" + UpdateEmpID + "';";
                string strupdate = "update  tabhrmsemployeepersonaldetails set (datdob, numage,vchplaceofbirth, vchgender, vchmaritalstatus, datdateofmarriage, vchbloodgroup, vchmobilenumber, vchresidentialnumber, vchemail, vchbiometricid, vchrfid, vchimageurl, vchstatus, statusid, modifiedby,modifieddate)= (  '" + FormatDate(User.DOB) + "', " + Convert.ToInt64(User.Age) + ", '" + ManageQuote(User.PlaceOfBirth) + "', '" + Gender + "', '" + ManageQuote(User.MaritalStatus) + "','" + FormatDate(marieddate) + "', '" + ManageQuote(User.BloodGroup) + "', " + Convert.ToInt64(User.MobileNumber) + ", " + Convert.ToInt64(User.Res) + ", '" + ManageQuote(User.Email) + "', '" + ManageQuote(User.BioMetricId) + "', '" + ManageQuote(User.RFID) + "', '" + User.UploadUrl + "', '" + ManageQuote(User.PresentStatusOfEMP) + "', " + 1 + ", '" + 1 + "', Current_timestamp) where vchemployeeid='" + UpdateEmpID + "';";
                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strupdate);
                isUpdated = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Personal_Information");

                throw;


            }
            return isUpdated;
        }

        public int CheckCount(string strCheckValue, string strTableName, string strColumnName)
        {

            string strcount = "SELECT COUNT(*)  FROM " + strTableName + " WHERE upper(" + strColumnName + ")='" + ManageQuote(strCheckValue.Trim().ToUpper()) + "'and statusid=1;";

            int res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strcount));
            return res;
        }

        private void UpdateEmployeeIdentification(EmployeeIdentification employeeIdentification, List<BankDetails> BankDetails, NpgsqlTransaction trans)
        {
            try
            {

                int count = 0;

                count = CheckCount(employeeIdentification.Employeeid, "TABHRMSEMPLOYEEIDENTIFICATIONDETAILS", "VCHEMPLOYEEID");

                if (count > 0)
                {
                    string UpdateEmployeeDetails = "UPDATE TABHRMSEMPLOYEEIDENTIFICATIONDETAILS SET VCHESISTATUS= '" + employeeIdentification.EsiStatus + "', VCHESINO='" + ManageQuote(employeeIdentification.EsiNo) + "',VCHPFSTATUS='" + employeeIdentification.PfStatus + "', VCHPFNO='" + ManageQuote(employeeIdentification.Pfno) + "', DATPFEFFETIVEDATE='" + employeeIdentification.PfEffectiveDate + "', VCHUANNO='" + ManageQuote(employeeIdentification.UanNo) + "', VCHPASSPORTNO='" + ManageQuote(employeeIdentification.PassportNo) + "', VCHAADHARNO='" + ManageQuote(employeeIdentification.AadharNo) + "', VCHPANNO='" + ManageQuote(employeeIdentification.PancardNo) + "', VCHDRIVINGLICENCENO='" + ManageQuote(employeeIdentification.DrivingLicenceNo) + "', VCHVEHICLEREGISTRATIONNO='" + ManageQuote(employeeIdentification.VehicleRegistrationNo) + "',VCHVEHICLEINSURANCENO='" + ManageQuote(employeeIdentification.VehicleInsuranceNo) + "', DATINSURANCEEXPDATE='" + employeeIdentification.InsuranceDate + "', MODIFIEDBY=" + employeeIdentification.Createdby + ",MODIFIEDDATE=CURRENT_TIMESTAMP WHERE VCHEMPLOYEEID='" + employeeIdentification.Employeeid + "'";
                }
                else
                {
                    string strInsert = "INSERT INTO TABHRMSEMPLOYEEIDENTIFICATIONDETAILS(EMPLOYEEID, VCHEMPLOYEEID, VCHESISTATUS, VCHESINO,VCHPFSTATUS, VCHPFNO, DATPFEFFETIVEDATE, VCHUANNO, VCHPASSPORTNO, VCHAADHARNO, VCHPANNO, VCHDRIVINGLICENCENO, VCHVEHICLEREGISTRATIONNO,VCHVEHICLEINSURANCENO, DATINSURANCEEXPDATE, STATUSID, CREATEDBY,CREATEDDATE) VALUES ( " + employeeIdentification.EmployeeNo + ", '" + employeeIdentification.Employeeid + "', '" + employeeIdentification.EsiStatus + "', '" + employeeIdentification.EsiNo + "','" + employeeIdentification.PfStatus + "', '" + employeeIdentification.Pfno + "', '" + employeeIdentification.PfEffectiveDate + "', '" + employeeIdentification.UanNo + "', '" + employeeIdentification.PassportNo + "', '" + employeeIdentification.AadharNo + "','" + employeeIdentification.PancardNo + "', '" + employeeIdentification.DrivingLicenceNo + "', '" + employeeIdentification.VehicleRegistrationNo + "', '" + employeeIdentification.VehicleInsuranceNo + "', '" + employeeIdentification.InsuranceDate + "', 1," + employeeIdentification.Createdby + ",current_timestamp)";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strInsert);
                }

                foreach (var item in BankDetails)
                {
                    int res = 0;
                    string strBankDetails = string.Empty;
                    string CheckExist = "select COUNT(*) from TABHRMSEMPLOYEEBANKACCOUNTDETAILS WHERE RECORDID=" + item.Bankrecordid + ";";
                    res = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, CheckExist));
                    if (res > 0)
                    {
                        strBankDetails = "UPDATE TABHRMSEMPLOYEEBANKACCOUNTDETAILS SET VCHBANKNAME='" + ManageQuote(item.Bankname) + "',VCHBRANCH='" + ManageQuote(item.Branchname) + "',VCHACCOUNTNO='" + ManageQuote(item.BankAccountNo) + "',MODIFIEDBY=" + employeeIdentification.Createdby + ",MODIFIEDDATE=CURRENT_TIMESTAMP WHERE RECORDID=" + item.Bankrecordid + ";";
                    }
                    else
                    {
                        strBankDetails = "INSERT INTO TABHRMSEMPLOYEEBANKACCOUNTDETAILS( EMPLOYEEID, VCHEMPLOYEEID, VCHBANKNAME, VCHBRANCH,VCHACCOUNTNO, STATUSID, CREATEDBY, CREATEDDATE) VALUES ( 9, 'FK1', '" + ManageQuote(item.Bankname) + "', '" + ManageQuote(item.Branchname) + "', '" + ManageQuote(item.BankAccountNo) + "', 1,1,current_timestamp)";

                    }
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strBankDetails);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public List<GroupDTO> getGroups(string strCourse)
        {
            List<GroupDTO> lstState = new List<GroupDTO>();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter("select recordid,upper(vchgroup) as vchgroup from tabhrmsgroupmst where vchcourse='" + ManageQuote(strCourse) + "' and statusid=1 order by vchgroup;", NPGSqlHelper.SQLConnString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    GroupDTO objGroup = new GroupDTO();

                    objGroup.RecordId = Convert.ToInt32(dr["recordid"].ToString());
                    objGroup.GroupName = dr["vchgroup"].ToString();

                    lstState.Add(objGroup);

                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "Employee Details");
            }
            return lstState;
        }

        #region EmployeeDetails

        public bool SaveEmpDetails(EmpDetails objEmpDetails)
        {
            Boolean isvalid = false;
            string strPersonal = string.Empty;
            string strAddress = string.Empty;
            long empid = 0;
            string Gender = string.Empty;

            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                //if (objEmpDetails.Gender == "Male")
                //{
                //    Gender = "M";
                //}
                //else
                //{
                //    Gender = "F";
                //}

                strPersonal = "INSERT INTO tabhrmsemployeepersonaldetails( vchemployeeid, vchname, vchsurname, vchgender, vchmobilenumber,vchemail,vchstatus, statusid, createdby,createddate) VALUES ('" + objEmpDetails.EmployeeID + "','" + ManageQuote(objEmpDetails.Name.ToUpper()) + "','" + ManageQuote(objEmpDetails.Sname.ToUpper()) + "','" + ManageQuote(objEmpDetails.Gender) + "','" + ManageQuote(objEmpDetails.MobileNumber) + "','" + ManageQuote(objEmpDetails.Email) + "','" + ManageQuote(objEmpDetails.EmployeeStatus) + "',1,1,current_timestamp)returning recordid ;";
                empid = Convert.ToInt64(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strPersonal));

                strAddress = "INSERT INTO tabhrmsemployeeaddressdetails(employeeid, vchemployeeid, vchdoorno, vchstreet, vcharea,vchcity, vchstate, vchcountry, vchpincode,statusid,createdby, createddate,countryid,stateid) VALUES (" + empid + ",'" + objEmpDetails.EmployeeID + "','" + ManageQuote(objEmpDetails.DoorNo) + "','" + ManageQuote(objEmpDetails.StreetName.ToUpper()) + "','" + ManageQuote(objEmpDetails.Area.ToUpper()) + "','" + ManageQuote(objEmpDetails.CityName) + "','" + ManageQuote(objEmpDetails.State) + "','" + ManageQuote(objEmpDetails.Country) + "','" + ManageQuote(objEmpDetails.PinCode) + "',1,1,current_timestamp,'" + objEmpDetails.CountryId + "','" + objEmpDetails.StateId + "');";
                strAddress = strAddress + "INSERT INTO tabhrmsemployeeemployment(employeeid, vchemployeeid, vchdesignation,vchtypeofemployment,statusid, createdby,createddate,vchworktype,vchpreferredtimeofwork,vchworkstations) VALUES (" + empid + ",'" + ManageQuote(objEmpDetails.EmployeeID) + "','" + ManageQuote(objEmpDetails.Designation) + "','" + ManageQuote(objEmpDetails.EmploymentType) + "',1,1,current_timestamp,'" + ManageQuote(objEmpDetails.WorkType) + "','" + ManageQuote(objEmpDetails.PreferredTimeofWork) + "','" + ManageQuote(objEmpDetails.WorkStation) + "');";
                NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, strAddress);


                trans.Commit();
                isvalid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
            }
            return isvalid;
        }

        public List<EmpDetails> GetEmpDetails()
        {
            List<EmpDetails> lstEmpdetails = new List<EmpDetails>();
            try
            {
                string strquery = "select tpd.employeeid,tpd.vchemployeeid,tpd.vchname,tpd.vchsurname,tpd.vchgender,tpd.vchmobilenumber,tpd.vchemail,tpd.vchstatus,tpd.vchdoorno,tpd.vchstreet,tpd.vcharea,tpd.vchcity,tpd.vchstate,tpd.vchcountry,tpd.countryid,tpd.stateid,vchdesignation,vchtypeofemployment,vchworktype,vchpreferredtimeofwork,vchworkstations,tpd.vchpincode from (select employeeid,tp.vchemployeeid,vchname,vchsurname,vchgender,vchmobilenumber,vchemail,vchstatus,vchdoorno,vchstreet,vcharea,vchcity,vchstate,vchcountry,countryid,stateid,vchpincode from tabhrmsemployeepersonaldetails tp join tabhrmsemployeeaddressdetails td on tp.recordid=td.employeeid and tp.statusid=1) tpd join tabhrmsemployeeemployment te on tpd.employeeid=te.employeeid where statusid=1 order by employeeid;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strquery);
                while (npgdr.Read())
                {
                    EmpDetails objEmp = new EmpDetails();
                    objEmp.EmployeeID = (npgdr["vchemployeeid"].ToString());
                    objEmp.Name = npgdr["vchname"].ToString();
                    objEmp.Sname = npgdr["vchsurname"].ToString();
                    objEmp.Gender = npgdr["vchgender"].ToString();
                    objEmp.MobileNumber = npgdr["vchmobilenumber"].ToString();
                    objEmp.Email = npgdr["vchemail"].ToString();
                    objEmp.EmployeeStatus = npgdr["vchstatus"].ToString();
                    objEmp.DoorNo = npgdr["vchdoorno"].ToString();
                    objEmp.StreetName = npgdr["vchstreet"].ToString();
                    objEmp.Area = npgdr["vcharea"].ToString();
                    objEmp.CityName = npgdr["vchcity"].ToString();
                    objEmp.State = npgdr["vchstate"].ToString();
                    objEmp.Country = npgdr["vchcountry"].ToString();
                    objEmp.CountryId = npgdr["countryid"].ToString();
                    objEmp.StateId = npgdr["stateid"].ToString();
                    objEmp.Designation = npgdr["vchdesignation"].ToString();
                    objEmp.EmploymentType = npgdr["vchtypeofemployment"].ToString();
                    objEmp.WorkType = npgdr["vchworktype"].ToString();
                    objEmp.PreferredTimeofWork = npgdr["vchpreferredtimeofwork"].ToString();
                    objEmp.WorkStation = npgdr["vchworkstations"].ToString();
                    objEmp.PinCode = npgdr["vchpincode"].ToString();

                    lstEmpdetails.Add(objEmp);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "EmpDetails");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstEmpdetails;
        }

        public bool UpdateEmpDetails(EmpDetails objEmp)
        {
            bool IsValid = false;
            string strupdate = string.Empty;
            try
            {
                //int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabcity where countryid=" + objcity.CountryId + " and stateid=" + objcity.StateId + " and upper(cityname)='" + ManageQuote(objcity.CityName.ToUpper()) + "' and statusid=1 and cityid<>" + objcity.CityId + ";"));
                //if (count == 0)
                //{
                strupdate = "UPDATE tabhrmsemployeepersonaldetails SET vchname='" + ManageQuote(objEmp.Name.ToUpper()) + "',vchsurname='" + ManageQuote(objEmp.Sname.ToUpper()) + "',vchgender='" + ManageQuote(objEmp.Gender) + "',vchmobilenumber='" + ManageQuote(objEmp.MobileNumber) + "',vchemail='" + ManageQuote(objEmp.Email) + "',vchstatus='" + ManageQuote(objEmp.EmployeeStatus) + "',modifiedby=1,modifieddate=current_timestamp WHERE vchemployeeid='" + ManageQuote(objEmp.EmployeeID) + "';";
                strupdate = strupdate + "UPDATE tabhrmsemployeeemployment SET vchdesignation='" + ManageQuote(objEmp.Designation) + "',vchtypeofemployment='" + ManageQuote(objEmp.EmploymentType) + "',vchworktype='" + ManageQuote(objEmp.WorkType) + "',vchpreferredtimeofwork='" + ManageQuote(objEmp.PreferredTimeofWork) + "',vchworkstations='" + ManageQuote(objEmp.WorkStation) + "', modifiedby=1, modifieddate=current_timestamp WHERE vchemployeeid='" + ManageQuote(objEmp.EmployeeID) + "';";
                strupdate = strupdate + "UPDATE tabhrmsemployeeaddressdetails SET vchdoorno='" + ManageQuote(objEmp.DoorNo) + "',vchstreet='" + ManageQuote(objEmp.StreetName.ToUpper()) + "',vcharea='" + ManageQuote(objEmp.Area.ToUpper()) + "',vchcity='" + ManageQuote(objEmp.CityName) + "',vchstate='" + ManageQuote(objEmp.State) + "',vchcountry='" + ManageQuote(objEmp.Country) + "',vchpincode='" + ManageQuote(objEmp.PinCode) + "',modifiedby=1,modifieddate=current_timestamp, countryid='" + ManageQuote(objEmp.CountryId) + "', stateid='" + ManageQuote(objEmp.StateId) + "' WHERE vchemployeeid='" + ManageQuote(objEmp.EmployeeID) + "';";

                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strupdate);
                IsValid = true;
                //}


            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }
            finally
            {

            }
            return IsValid;

        }

        public bool DeleteEmpDetails(EmpDetails objEmp)
        {
            bool IsValid = false;
            string strDelete = string.Empty;
            try
            {
                strDelete = "update tabhrmsemployeepersonaldetails set statusid=2,modifiedby=1,modifieddate=current_timestamp where vchemployeeid='" + ManageQuote(objEmp.EmployeeID) + "';";
                strDelete = strDelete + "update tabhrmsemployeeaddressdetails set statusid=2,modifiedby=1,modifieddate=current_timestamp where vchemployeeid='" + ManageQuote(objEmp.EmployeeID) + "';";
                strDelete = strDelete + "update tabhrmsemployeeemployment set statusid=2,modifiedby=1,modifieddate=current_timestamp where vchemployeeid='" + ManageQuote(objEmp.EmployeeID) + "';";

                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strDelete);

                IsValid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "EmpDetails");
                throw;
            }

            return IsValid;
        }

        public List<EmpDetails> GetWorkStations()
        {
            List<EmpDetails> lstWorkstations = new List<EmpDetails>();
            try
            {
                string strWorkstation = "select recordid,vchworkstation from tabhrmsworkstationsmst where statusid=1 order by vchworkstation;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strWorkstation);
                while (npgdr.Read())
                {
                    EmpDetails objWorkstations = new EmpDetails();
                    objWorkstations.WorkStationId = npgdr["recordid"].ToString();
                    objWorkstations.WorkStation = npgdr["vchworkstation"].ToString();

                    lstWorkstations.Add(objWorkstations);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "EmpDetails");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstWorkstations;
        }

        public bool SaveWorkStation(EmpDetails objEmpDetails)
        {
            Boolean isvalid = false;
            string strWorkstation = string.Empty;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabhrmsworkstationsmst where upper(vchworkstation)='" + ManageQuote(objEmpDetails.WorkStation.ToUpper()) + "' and statusid=1;"));
                if (count == 0)
                {
                    strWorkstation = "INSERT INTO tabhrmsworkstationsmst(vchworkstation,statusid,createdby,createddate) VALUES ('" + ManageQuote(objEmpDetails.WorkStation.ToUpper()) + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strWorkstation);
                    isvalid = true;
                }

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "WorkStation");
            }
            return isvalid;
        }

        #endregion


        #region Assign Shift

        public List<AssignShift> GetShift()
        {
            List<AssignShift> lstShift = new List<AssignShift>();
            try
            {
                string strShift = "select shiftid,shiftname from tabposshiftmst where statusid=1 order by shiftname;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strShift);
                while (npgdr.Read())
                {
                    AssignShift objShift = new AssignShift();
                    objShift.ShiftId = npgdr["shiftid"].ToString();
                    objShift.Shiftname = npgdr["shiftname"].ToString();

                    lstShift.Add(objShift);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "AssignShift");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstShift;
        }

        public List<AssignShift> GetShiftTimings(string strShiftId)
        {
            List<AssignShift> lstShiftTimings = new List<AssignShift>();
            try
            {
                string strWorkstation = "select (fromtime||'-'||totime) as time from tabposshiftmst where shiftid=" + strShiftId + " and statusid=1 order by shiftname;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strWorkstation);
                while (npgdr.Read())
                {
                    AssignShift objShiftTimings = new AssignShift();

                    objShiftTimings.ShiftTime = npgdr["time"].ToString();

                    lstShiftTimings.Add(objShiftTimings);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "AssignShift");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstShiftTimings;
        }

        public List<AssignShift> GetEmployees(string strShiftId, string strFromDate, string strToDate)
        {
            List<AssignShift> lstEmpdetails = new List<AssignShift>();
            string strquery = string.Empty;

            try
            {
                //string strquery = "select  tpd.employeeid,tpd.vchemployeeid,tpd.vchname,tpd.vchmobilenumber,tpd.vchdesignation,tpd.vchtypeofemployment,tpd.vchworktype,tpd.vchpreferredtimeofwork,tpd.vchworkstations,shifttime,breaktime,vchstatus from (select employeeid,tp.vchemployeeid,vchname,vchmobilenumber,vchdesignation,vchtypeofemployment,vchworktype,vchpreferredtimeofwork,vchworkstations from tabhrmsemployeepersonaldetails tp join tabhrmsemployeeemployment te on tp.recordid=te.employeeid and tp.statusid=1) tpd left join tabposassignshift tas on tpd.employeeid=tas.employeeid order by employeeid;";
                //strquery = "select employeeid,tp.vchemployeeid,vchname,vchmobilenumber,vchdesignation,vchtypeofemployment,vchworktype,vchpreferredtimeofwork,vchworkstations from tabhrmsemployeepersonaldetails tp join tabhrmsemployeeemployment te on tp.recordid=te.employeeid and tp.statusid=1  order by employeeid;";
                //strquery = "select employeeid,tp.vchemployeeid,vchname,vchmobilenumber,vchdesignation,vchtypeofemployment,vchworktype,vchpreferredtimeofwork,vchworkstations from tabhrmsemployeepersonaldetails tp join tabhrmsemployeeemployment te on tp.recordid=te.employeeid and tp.statusid=1 where employeeid not in(select employeeid from tabposassignshift) order by employeeid;";
                strquery = "select employeeid,tp.vchemployeeid,vchname,vchmobilenumber,vchdesignation,vchtypeofemployment,vchworktype,vchpreferredtimeofwork,vchworkstations from tabhrmsemployeepersonaldetails tp join tabhrmsemployeeemployment te on tp.recordid=te.employeeid and tp.statusid=1 where employeeid not in(select distinct employeeid from tabposassignshift where shiftid='" + strShiftId + "' and datfromdate between '" + FormatDate(strFromDate) + "' and '" + FormatDate(strToDate) + "') order by employeeid;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strquery);
                while (npgdr.Read())
                {
                    AssignShift objEmp = new AssignShift();
                    objEmp.Employeeid = (npgdr["employeeid"].ToString());
                    objEmp.vchEmployeeid = (npgdr["vchemployeeid"].ToString());
                    objEmp.Name = npgdr["vchname"].ToString();
                    objEmp.MobileNumber = npgdr["vchmobilenumber"].ToString();
                    objEmp.Designation = npgdr["vchdesignation"].ToString();
                    objEmp.EmploymentType = npgdr["vchtypeofemployment"].ToString();
                    objEmp.WorkType = npgdr["vchworktype"].ToString();
                    objEmp.PreferredTimeofWork = npgdr["vchpreferredtimeofwork"].ToString();
                    objEmp.WorkStation = npgdr["vchworkstations"].ToString();
                    //objEmp.ShiftTime = npgdr["shifttime"].ToString();
                    //objEmp.BreakTime = npgdr["breaktime"].ToString();
                    //objEmp.CheckSelect = npgdr["vchstatus"].ToString();
                    lstEmpdetails.Add(objEmp);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstEmpdetails;
        }

        public bool SaveAssignShift(AssignShift AssignShiftDTO, List<AssignShift> lstAssignShift)
        {
            Boolean isvalid = false;
            string strAssign = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();

                IFormatProvider culture = new CultureInfo("en-US", true);
                DateTime from = DateTime.ParseExact(AssignShiftDTO.FromDate, "dd/MM/yyyy", culture);
                AssignShiftDTO.FromDate = from.ToString();

                DateTime to = DateTime.ParseExact(AssignShiftDTO.ToDate, "dd/MM/yyyy", culture);
                AssignShiftDTO.ToDate = to.ToString();

                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(trans, CommandType.Text, "select count(*) from tabposassignshift where shiftid='" + ManageQuote(AssignShiftDTO.ShiftId) + "' and datfromdate between '" + FormatDate(Convert.ToDateTime(AssignShiftDTO.FromDate).ToString("dd/MM/yyyy")) + "' and '" + FormatDate(Convert.ToDateTime(AssignShiftDTO.ToDate).ToString("dd/MM/yyyy")) + "' and vchemployeeid not in(select vchemployeeid from tabhrmsemployeepersonaldetails where statusid=1);"));
                if (count == 0)
                {
                    for (int i = 0; i < lstAssignShift.Count; i++)
                    {
                        if (lstAssignShift[i].CheckSelect == "Y")
                        {
                            for (var dt = Convert.ToDateTime(AssignShiftDTO.FromDate); dt <= Convert.ToDateTime(AssignShiftDTO.ToDate); dt = dt.AddDays(1))
                            {
                                strAssign = "INSERT INTO tabposassignshift(shiftid,shiftname,shifttime,breakfromtime,breaktotime,datfromdate,dattodate,employeeid,vchemployeeid,vchempname,vchdegination,vchemploymenttype,vchworktype,vchpreferredtimeofwork,vchworkstation,vchmobileno,vchstatus,statusid,createdby,createddate)VALUES ('" + ManageQuote(AssignShiftDTO.ShiftId) + "','" + ManageQuote(AssignShiftDTO.Shiftname) + "','" + ManageQuote(AssignShiftDTO.ShiftTime) + "','" + ManageQuote(lstAssignShift[i].BreakFromTime) + "','" + ManageQuote(lstAssignShift[i].BreakToTime) + "','" + FormatDate(dt.ToString("dd/MM/yyyy")) + "','" + FormatDate(dt.ToString("dd/MM/yyyy")) + "','" + lstAssignShift[i].Employeeid + "','" + ManageQuote(lstAssignShift[i].vchEmployeeid) + "','" + ManageQuote(lstAssignShift[i].Name) + "','" + ManageQuote(lstAssignShift[i].Designation) + "','" + ManageQuote(lstAssignShift[i].EmploymentType) + "','" + ManageQuote(lstAssignShift[i].WorkType) + "','" + ManageQuote(lstAssignShift[i].PreferredTimeofWork) + "','" + ManageQuote(lstAssignShift[i].WorkStation) + "','" + ManageQuote(lstAssignShift[i].MobileNumber) + "','" + ManageQuote(lstAssignShift[i].CheckSelect) + "',1,1,current_timestamp);";
                                NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strAssign);
                            }
                        }
                    }
                    isvalid = true;
                }
                else
                {
                    isvalid = false;
                }

                trans.Commit();

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
            }

            return isvalid;

        }

        #endregion


        #region Edit Shift

        public List<AssignShift> GetEmpInformation(string datDate, string strShiftId)
        {
            List<AssignShift> lstEmpdetails = new List<AssignShift>();
            string strquery = string.Empty;

            try
            {
                strquery = "select vchempname,vchmobileno,vchdegination,vchemploymenttype,vchworktype,vchpreferredtimeofwork,vchworkstation,shifttime,breakfromtime,breaktotime from tabposassignshift where datfromdate='" + FormatDate(datDate) + "' and shiftid='" + strShiftId + "' order by employeeid; ";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strquery);
                while (npgdr.Read())
                {
                    AssignShift objEmp = new AssignShift();
                    //objEmp.Employeeid = (npgdr["employeeid"].ToString());
                    //objEmp.vchEmployeeid = (npgdr["vchemployeeid"].ToString());
                    objEmp.Name = npgdr["vchempname"].ToString();
                    objEmp.MobileNumber = npgdr["vchmobileno"].ToString();
                    objEmp.Designation = npgdr["vchdegination"].ToString();
                    objEmp.EmploymentType = npgdr["vchemploymenttype"].ToString();
                    objEmp.WorkType = npgdr["vchworktype"].ToString();
                    objEmp.PreferredTimeofWork = npgdr["vchpreferredtimeofwork"].ToString();
                    objEmp.WorkStation = npgdr["vchworkstation"].ToString();
                    objEmp.ShiftTime = npgdr["shifttime"].ToString();
                    objEmp.BreakFromTime = npgdr["breakfromtime"].ToString();
                    objEmp.BreakToTime = npgdr["breaktotime"].ToString();
                    //objEmp.FromDate = npgdr["datfromdate"].ToString();
                    //objEmp.ToDate = npgdr["dattodate"].ToString();
                    lstEmpdetails.Add(objEmp);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "EditShift");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstEmpdetails;

        }

        public List<AssignShift> ShowEmployees(string datDate, string strShiftId)
        {
            List<AssignShift> lstEmployees = new List<AssignShift>();
            string stremployee = string.Empty;
            try
            {
                stremployee = "select (recordid||'-'||vchemployeeid)empid,vchname from tabhrmsemployeepersonaldetails where vchstatus='Y' and statusid=1 order by vchname;";
                //stremployee = "select (recordid||'-'||vchemployeeid)empid,vchname from tabhrmsemployeepersonaldetails where vchemployeeid not in(select vchemployeeid from tabposassignshift where shiftid='" + strShiftId + "' and datfromdate='" + FormatDate(datDate) + "') and vchstatus='Y' and statusid=1 order by vchname;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, stremployee);
                while (npgdr.Read())
                {
                    AssignShift objEmployees = new AssignShift();
                    objEmployees.Employeeid = (npgdr["empid"].ToString());
                    objEmployees.Name = npgdr["vchname"].ToString();
                    lstEmployees.Add(objEmployees);
                }
            }
            catch (Exception ex)
            {

                EventLogger.WriteToErrorLog(ex, "EditShift");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstEmployees;


        }

        public bool SaveEditShift(AssignShift EditShiftDTO, List<AssignShift> lstEditShift)
        {
            Boolean isvalid = false;
            string strEdit = string.Empty;
            string strDelete = string.Empty;
            try
            {
                con = new NpgsqlConnection(NPGSqlHelper.SQLConnString);
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                trans = con.BeginTransaction();



                for (int i = 0; i < lstEditShift.Count; i++)
                {
                    //if (lstEditShift[i].CheckSelect == "Y")
                    //{
                    strDelete = "delete from tabposassignshift where datfromdate='" + FormatDate(EditShiftDTO.FromDate) + "' and shiftid='" + EditShiftDTO.ShiftId + "';";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strDelete);

                    strEdit = "INSERT INTO tabposassignshift(shiftid,shiftname,shifttime,breakfromtime,breaktotime,datfromdate,dattodate,employeeid,vchemployeeid,vchempname,vchdegination,vchemploymenttype,vchworktype,vchpreferredtimeofwork,vchworkstation,vchmobileno,vchstatus,statusid,createdby,createddate)VALUES ('" + ManageQuote(EditShiftDTO.ShiftId) + "','" + ManageQuote(EditShiftDTO.Shiftname) + "','" + ManageQuote(EditShiftDTO.ShiftTime) + "','" + ManageQuote(lstEditShift[i].BreakFromTime) + "','" + ManageQuote(lstEditShift[i].BreakToTime) + "','" + FormatDate(EditShiftDTO.FromDate) + "','" + FormatDate(EditShiftDTO.FromDate) + "','" + ManageQuote(lstEditShift[i].Employeeid.Split('-')[0]).Replace(" ", "") + "','" + ManageQuote(lstEditShift[i].Employeeid.Split('-')[1]).Replace(" ", "") + "','" + ManageQuote(lstEditShift[i].Name) + "','" + ManageQuote(lstEditShift[i].Designation) + "','" + ManageQuote(lstEditShift[i].EmploymentType) + "','" + ManageQuote(lstEditShift[i].WorkType) + "','" + ManageQuote(lstEditShift[i].PreferredTimeofWork) + "','" + ManageQuote(lstEditShift[i].WorkStation) + "','" + ManageQuote(lstEditShift[i].MobileNumber) + "','" + ManageQuote(lstEditShift[i].CheckSelect) + "',1,1,current_timestamp);";
                    NPGSqlHelper.ExecuteNonQuery(trans, CommandType.Text, strEdit);
                    //}
                }

                trans.Commit();
                isvalid = true;

            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "AssignShift");
            }

            return isvalid;

        }



        #endregion
    }
}
