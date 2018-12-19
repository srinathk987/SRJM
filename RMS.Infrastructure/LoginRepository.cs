using System.Data;
using HelperManager;
using RMS.Core.Interfaces;
using Npgsql;
using System;
using RMS.Core;
using System.Collections.Generic;
using HRMS.Infrastructure;
using System.Globalization;

namespace RMS.Infrastructure
{
    public class LoginRepository : ILogin
    {
        #region Login
        NpgsqlDataReader npgdr = null;
        public bool checkUser(Core.Login objLogin)
        {
            //bool flag = false;
            try
            {

                using (NpgsqlConnection con = new NpgsqlConnection(NPGSqlHelper.SQLConnString))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select userid from tabuserinfo where username='" + ManageQuote(objLogin.username) + "' and userpassword='" + ManageQuote(objLogin.password) + "'", con))
                    {
                        objLogin.userid = Convert.ToInt64(cmd.ExecuteScalar());

                    }
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                        con.ClearPool();
                    }
                }
                if (objLogin.userid != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                // EventLogger.WriteToErrorLog(ex, "Login");
                // npgdr.Dispose();
                throw;
            }
            finally
            {


            }
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


        public DataSet GetUserid(string username)
        {
            try
            {
                string StrUsers = "select userid from tabuserinfo where username='" + username + "'";


                return NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, StrUsers);

            }
            catch (System.Exception ex)
            {
                // EventLogger.WriteToErrorLog(ex, "Login");
                throw;
            }



        }


        #endregion

        #region Module...

        public List<Login> GetParentModuleNames(int userid)
        {
            List<Login> objModuleValues = new List<Login>();
            DataSet ds = new DataSet();
            try
            {

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, "select distinct modulename,tm.moduleid,moduledescription,modulesortorder,parentmodulename from tabmodules tm join tabrolefunctions trf on tm.moduleid=trf.moduleid where moduletype in('RMS','HRMS') and modulename not in ('RMSDASHBOARD') and userid=" + userid + " order by modulesortorder");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Login objmodule = new Login();
                        objmodule.ModuleId = Convert.ToInt32(ds.Tables[0].Rows[i]["moduleid"]);
                        objmodule.ModuleName = Convert.ToString(ds.Tables[0].Rows[i]["modulename"]);
                        objmodule.ParentModuleName = Convert.ToString(ds.Tables[0].Rows[i]["parentmodulename"]);
                        if (objmodule.ModuleName.ToLower().Contains("master"))
                            objmodule.moduleclass = "menu-icon fa fa-cog";
                        if (objmodule.ModuleName.ToLower().Contains("tran"))
                            objmodule.moduleclass = "menu-icon fa fa-pie-chart";
                        if (objmodule.ModuleName.ToLower().Contains("report"))
                            objmodule.moduleclass = "menu-icon fa fa-random";
                        if (objmodule.ModuleName.ToLower().Contains("home"))
                            objmodule.moduleclass = "menu-icon fa fa-home";
                        objmodule.moduledescription = Convert.ToString(ds.Tables[0].Rows[i]["moduledescription"]);

                        objModuleValues.Add(objmodule);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ds.Dispose();

            }

            return objModuleValues;
        }
        public List<Login> GetChildFunctionNames(int userid, string ModuleId, string type)
        {
            List<Login> objChildModuleValues = new List<Login>();
            DataSet ds = new DataSet();
            string str = "";
            try
            {
                if (type == "RMSDASHBOARD")
                {
                    str = "select distinct functionname,functionurl,tf.moduleid,functionsortorder,vchfunctionclass from tabfunctions tf join tabrolefunctions trf on tf.functionid=trf.functionid join  tabposfunctionclasses fc on tf.functionid=fc.functionid and fc.functionid=trf.functionid where type='" + type + "'  and userid=" + userid + " order by functionsortorder;";
                }
                else
                {
                    str = "select distinct functionname,functionurl,tf.moduleid,functionsortorder,vchfunctionclass from tabfunctions tf join tabrolefunctions trf on tf.functionid=trf.functionid left join  tabposfunctionclasses fc on tf.functionid=fc.functionid and fc.functionid=trf.functionid where type='" + type + "'  and userid=" + userid + "  and tf.moduleid=" + ModuleId + "  order by functionsortorder;";
                }

                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                // npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, "select functionname,functionurl,moduleid from tabfunctions where type='HRMS'  order by moduleid;");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Login objChildmodule = new Login();
                        objChildmodule.ModuleId = Convert.ToInt32(ds.Tables[0].Rows[i]["moduleid"]);
                        objChildmodule.FunctionName = Convert.ToString(ds.Tables[0].Rows[i]["functionname"]);
                        objChildmodule.Functionclass = Convert.ToString(ds.Tables[0].Rows[i]["vchfunctionclass"]);
                        objChildmodule.FunctionUrl = Convert.ToString(ds.Tables[0].Rows[i]["functionurl"]);

                        objChildModuleValues.Add(objChildmodule);
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ds.Dispose();

            }

            return objChildModuleValues;
        }

        #endregion


        /// <summary>
        /// MURALI
        /// </summary>
        #region User Rights

        public bool SaveNewUser(UserInfo E)
        {
            Boolean IsValid = false;
            try
            {
                int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select * from tabuserinfo  where username='" + E.UserName + "' and userpassword='" + E.Password + "' and statusid=1;"));
                if (count == 0)
                {
                    string strSave = string.Empty;
                    if (Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabuserinfo where staffid=" + E.EmployeeID + " and statusid=1;")) == 0)
                    {
                        strSave = @"insert into tabuserinfo(username,userpassword,statusid,createdby,createddate,applicationid,usertype,staffid,vchempname) ";
                        strSave += " values('" + ManageQuote(E.UserName.ToString()) + "','" + E.Password + "',1,1,current_timestamp,1,1," + E.EmployeeID + ",'"+E.Empname+"') returning userid;";
                        string StrID = Convert.ToString(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, strSave));
                        //NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, "update TABHRMSEMPLOYEEPERSONALDETAILS set userid=" + StrID + " where recordid=" + E.EmployeeID + ";");
                        IsValid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "SaveNewUser");
            }
            return IsValid;
        }
        public bool SaveChangeUser(UserInfo E)
        {
            Boolean IsValid = false;
            try
            {
                int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabuserinfo  where userid='" + E.UserID + "' and userpassword='" + E.OldPassword + "' and statusid=1;"));
                if (count == 1)
                {
                    string strSave = string.Empty;
                    strSave = @"update tabuserinfo set userpassword='" + E.NewPassword + "' where userid='" + E.UserID + "' and statusid=1;";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            return IsValid;
        }
        public List<UserInfo> ShowUserName()
        {
            List<UserInfo> lstemployeename = new List<UserInfo>();
            try
            {
                string str = "SELECT  userid, upper(username) as username FROM tabuserinfo where statusid=1 ORDER BY username";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                while (npgdr.Read())
                {
                    UserInfo objUserInfoDTO = new UserInfo();
                    objUserInfoDTO.UserID = npgdr["userid"].ToString();
                    objUserInfoDTO.UserName = npgdr["username"].ToString();
                    lstemployeename.Add(objUserInfoDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstemployeename;
        }
        public DataTable ShowEmployeeName()
        {
            DataSet ds = new DataSet();
            //List<UserInfo> lstemployeename = new List<UserInfo>();
            try
            {

                string str = "select recordid as employeeid,vchname as employeename  from TABHRMSEMPLOYEEPERSONALDETAILS where statusid =1 order by 2; ";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                //while (npgdr.Read())
                //{
                //    UserInfo objUserInfoDTO = new UserInfo();
                //    objUserInfoDTO.UserID = npgdr["recordid"].ToString();
                //    objUserInfoDTO.UserName = npgdr["vchemployeename"].ToString();
                //    lstemployeename.Add(objUserInfoDTO);
                //}
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowEmployeeName");
            }
            finally
            {
                //npgdr.Dispose();
            }
            return ds.Tables[0];
        }
        public List<ModuleDTO> ShowModuleName()
        {
            List<ModuleDTO> lstModule = new List<ModuleDTO>();
            try
            {
                string str = "select modulename,moduleid from tabmodules where statusid=1 and moduletype in ('RMS','HRMS');";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                while (npgdr.Read())
                {
                    ModuleDTO objModuleDTO = new ModuleDTO();
                    objModuleDTO.Moduleid = Convert.ToInt32(npgdr["moduleid"]);
                    objModuleDTO.Modulename = npgdr["modulename"].ToString();
                    lstModule.Add(objModuleDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstModule;
        }
        public List<FunctionsDTO> getFunctions(string Moduleid)
        {
            List<FunctionsDTO> lstFunction = new List<FunctionsDTO>();
            try
            {
                string str = "select functionname,functionid,statusid from tabfunctions where moduleid=1 and statusid=1;";
                str = "select functionname,functionid,statusid,moduleid from tabfunctions where moduleid=" + Moduleid + " and statusid=1;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                while (npgdr.Read())
                {
                    FunctionsDTO objFunctionsDTO = new FunctionsDTO();
                    objFunctionsDTO.FunctionId = Convert.ToInt32(npgdr["functionid"]);
                    objFunctionsDTO.FunctionName = npgdr["functionname"].ToString();
                    objFunctionsDTO.Moduleid = Convert.ToInt16(npgdr["moduleid"]);
                    objFunctionsDTO.FunctionStatus = false;

                    lstFunction.Add(objFunctionsDTO);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstFunction;
        }
        public bool SaveFunction(FunctionsDTO F)
        {
            Boolean IsValid = false;
            try
            {
                //int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select * from tabrmsuserinfo  where username='" + E.UserName + "' and userpassword='" + E.Password + "' and statusid=1 and type='HRMS';"));
                int count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabfunctions  where functionname='" + ManageQuote(F.FunctionName.Trim().ToUpper().ToString()) + "' and statusid=1 and moduleid=" + F.Moduleid + ";"));
                if (count == 0)
                {
                    string strSave = string.Empty;
                    //strSave = "insert into tabrmsuserinfo(username,applicationid,userpassword,usertype,issystemuser,staffid,statusid,createdby,createddate,type) ";
                    //strSave += " values('" + E.UserName.ToUpper().ToString() + "',1,'" + E.Password + "',1,0,0,1,1,current_timestamp,'HRMS');";
                    strSave = @"insert into tabfunctions(functionname,functionurl,statusid,moduleid,createdby,createddate,type) ";
                    strSave += " values('" + ManageQuote(F.FunctionName.ToUpper().ToString()) + "','" + F.FunctionUrl + "',1," + F.Moduleid + ",1,current_timestamp,'RMS');";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            return IsValid;
        }
        public bool SaveFunctionOrder(List<FunctionsDTO> FunctionDTO)
        {
            Boolean IsValid = false;
            int Count = 0;
            try
            {
                //select functionname,functionsortorder,functionurl,statusid,moduleid,createdby,createddate,modifiedby,modifieddate from tabrmsfunctions;
                string strInser = "insert into tabfunctions(functionname,functionsortorder,functionurl,statusid,moduleid,createdby,createddate) values";
                strInser = string.Empty;
                foreach (var F in FunctionDTO)
                {
                    Count = 0;
                    Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabfunctions where functionname='" + F.FunctionName + "' and statusid=1 and functionid=" + F.FunctionId + " and moduleid=" + F.Moduleid + ";"));
                    if (Count > 0)
                    {
                        strInser += @"update  tabfunctions set functionsortorder=" + F.FunctionOrder + " where functionname='" + F.FunctionName + "' and statusid=1 and functionid=" + F.FunctionId + " and moduleid=" + F.Moduleid + ";";
                    }
                }
                NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInser);
                IsValid = true;
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            return IsValid;
        }
        public List<ModuleDTO> ShowRoleFunctions(string ID)
        {

            NpgsqlDataReader npgdr = null;
            List<ModuleDTO> lstJquery = new List<ModuleDTO>();
            try
            {
                string strlocation = "select f.functionid,f.functionname,f.functionurl,f.moduleid,m.modulename from tabfunctions f left join tabmodules m on m.moduleid=f.moduleid order by modulename;";
                strlocation = "select x.functionid,functionname,functionurl,x.moduleid,modulename,coalesce(status,false) as status from (select f.functionid,f.functionname,f.functionurl,f.moduleid,m.modulename  from tabfunctions f left join tabmodules m on m.moduleid=f.moduleid  where moduletype in ('RMS') order by modulename)x left join (SELECT functionid,moduleid,true as status FROM tabrolefunctions where userid=" + ID + ")y on x.moduleid=y.moduleid and x.functionid=y.functionid;";
                strlocation = "select x.functionid,functionname,functionurl,x.moduleid, modulename,coalesce(status,false) as status from (select f.functionid,f.functionname,f.functionurl,f.moduleid,m.parentmodulename||'_'||m.modulename as  modulename from tabfunctions f left join tabmodules m on m.moduleid=f.moduleid  where moduletype in ('RMS') order by modulename)x left join (SELECT functionid,moduleid,true as status FROM tabrolefunctions where userid=" + ID + ")y on x.moduleid=y.moduleid and x.functionid=y.functionid;";
                npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, strlocation);
                while (npgdr.Read())
                {
                    ModuleDTO obj = new ModuleDTO();
                    obj.functionid = Convert.ToInt32(npgdr["functionid"].ToString());
                    obj.functionname = npgdr["functionname"].ToString();
                    obj.functionurl = npgdr["functionurl"].ToString();
                    obj.Moduleid = Convert.ToInt32(npgdr["moduleid"].ToString());
                    obj.modulename = npgdr["modulename"].ToString();
                    obj.chkStatus = Convert.ToBoolean(npgdr["status"]);
                    lstJquery.Add(obj);
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            finally
            {
                npgdr.Dispose();
            }
            return lstJquery;
        }
        public bool SaveUserAuthentication(List<ModuleDTO> RoleFunctions, string UserID)
        {
            Boolean IsValid = false;
            int Count = 0;
            try
            {
                //select functionname,functionsortorder,functionurl,statusid,moduleid,createdby,createddate,modifiedby,modifieddate from tabrmsfunctions;
                string strInsert = string.Empty;
                Count = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*) from tabrolefunctions where  userid=" + UserID + " and moduleid in (select moduleid from tabmodules where moduletype in ('RMS'));"));
                if (Count > 0)
                {
                    string Delete = "delete from tabrolefunctions where userid=" + UserID + " and moduleid in (select moduleid from tabmodules where moduletype in ('RMS'));";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, Delete);
                }
                if (RoleFunctions.Count > 0)
                {
                    strInsert = "insert into tabrolefunctions(userid,functionid,moduleid,permissions) values ";
                    foreach (var Role in RoleFunctions)
                    {
                        strInsert += "(" + UserID + "," + Role.functionid + "," + Role.Moduleid + ",'A'),";
                    }
                    strInsert = strInsert.Substring(0, strInsert.Length - 1);
                    if (strInsert.Length > 77)
                    {
                        NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strInsert);
                        IsValid = true;
                    }
                }
                else
                {
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            return IsValid;
        }
        public List<ModuleDTO> ShowMainModules(string ID, string Form, string ModuleType)
        {
            List<ModuleDTO> lstModule = new List<ModuleDTO>();
            DataSet ds = new DataSet();
            try
            {
                if (Form == "MODULE")
                {
                    string strGetCountry = "select distinct  moduleid,modulename from tabmodules where parentmodulename is not null and moduletype in ('" + ModuleType + "') and parentmoduleid=" + ID + " and moduleid not in (" + ID + ") order by modulename;";
                    ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetCountry);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ModuleDTO objC = new ModuleDTO();
                            objC.ModuleId = Convert.ToInt32(ds.Tables[0].Rows[i]["moduleid"]);
                            objC.ParentModulename = ds.Tables[0].Rows[i]["modulename"].ToString();
                            lstModule.Add(objC);
                        }
                    }
                }
                else
                {
                    string str = "select distinct modulename,moduleid from tabmodules where statusid=1 and moduletype in ('" + ModuleType + "') and  parentmoduleid=" + ID + " and moduleid not in (" + ID + ");";
                    npgdr = NPGSqlHelper.ExecuteReader(NPGSqlHelper.SQLConnString, CommandType.Text, str);
                    while (npgdr.Read())
                    {
                        ModuleDTO objModuleDTO = new ModuleDTO();
                        objModuleDTO.Moduleid = Convert.ToInt32(npgdr["moduleid"]);
                        objModuleDTO.Modulename = npgdr["modulename"].ToString();
                        lstModule.Add(objModuleDTO);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowMainModules");
            }
            finally
            {
                ds.Dispose();
            }
            return lstModule;
        }
        public bool SaveModule(ModuleDTO1 objModule)
        {
            Boolean isvalid = false;
            try
            {
                int count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*)  from tabmodules where statusid=1 and parentmoduleid=" + objModule.parentModuleId + " and modulename='" + ManageQuote(objModule.Modulename.Trim().ToUpper()) + "';"));
                if (count == 0)
                {
                    string strSave = string.Empty;
                    int moduleorder = 0;
                    //strSave = "INSERT INTO tabmodules(modulename, moduledescription, statusid, createdby, createddate, parentmodulename,moduletype) VALUES ('" + objModule.Modulename.Trim().ToUpper() + "',null,1,1,current_timestamp,'" + objModule.Modulename.Trim().ToUpper() + "','" + objModule.ModuleType + "');";
                    moduleorder = Convert.ToInt16(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, "select count(*)  from tabmodules where statusid=1 and parentmoduleid=" + objModule.parentModuleId + ";"));
                    if (moduleorder == 0)
                    {
                        moduleorder = 1;
                    }
                    else
                    {
                        moduleorder = moduleorder + 1;
                    }
                    strSave = "INSERT INTO tabmodules(modulename,modulelevelno,modulesortorder, moduledescription, statusid, createdby, createddate, parentmoduleid,parentmodulename,moduletype) VALUES ('" + ManageQuote(objModule.Modulename.Trim().ToUpper()) + "'," + moduleorder + "," + moduleorder + ",'" + objModule.Modulename + "',1,1,current_timestamp," + objModule.parentModuleId + ",'" + objModule.ParentModulename.Trim().ToUpper() + "','" + objModule.ModuleType + "');";
                    NPGSqlHelper.ExecuteNonQuery(NPGSqlHelper.SQLConnString, CommandType.Text, strSave);
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            return isvalid;
        }
        private int check_Existed(ModuleDTO1 objModulename)
        {
            int Count = 0;
            try
            {
                string check_exist = "select count(modulename) from tabmodules where statusid=1 and modulename= '" + objModulename.Modulename.Trim().ToUpper() + "'";
                Count = Convert.ToInt32(NPGSqlHelper.ExecuteScalar(NPGSqlHelper.SQLConnString, CommandType.Text, check_exist));
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            return Count;
        }
        public List<ModuleDTO> ModueTypeChange(string Type, string Form)
        {
            List<ModuleDTO> lstmodule = new List<ModuleDTO>();
            DataSet ds = new DataSet();
            try
            {
                string strGetCountry = "select distinct  parentmoduleid,parentmodulename from tabmodules where parentmodulename is not null  and moduletype in ('" + Type + "') and parentmoduleid is not null order by parentmodulename;";
                ds = NPGSqlHelper.ExecuteDataset(NPGSqlHelper.SQLConnString, CommandType.Text, strGetCountry);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ModuleDTO objC = new ModuleDTO();
                        objC.ModuleId = Convert.ToInt32(ds.Tables[0].Rows[i]["parentmoduleid"]);
                        objC.ParentModulename = ds.Tables[0].Rows[i]["parentmodulename"].ToString();
                        lstmodule.Add(objC);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            finally
            {
                ds.Dispose();
            }
            return lstmodule;
        }
        #endregion
    }
}
