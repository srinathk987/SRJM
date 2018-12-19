using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RMS.Infrastructure;

using System.Web.Security;
using System.Data;

using RMS.Infrastructure;
using RMS.Core.Interfaces;
using RMS.Core;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using HRMS.Infrastructure;

namespace RMS.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        string childname = null;
        string modulename = null;
        string status = null;

        #region Login
        //
        // GET: /Login/
        private ILogin objlogin = new LoginRepository();
        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                //return   RedirectToAction("DashBoard", "POSTransaction");
                Request.Cookies.Remove("UserName");
                Session.Remove("UserName");
                Session.Remove("UserId");
                Session.Remove("ModuleNames");
                Session.Remove("modulename");
                Session.Remove("ChildNames");
                Session.Remove("ChildName");
                return View();
            }
            catch (Exception ex)
            {
                //  EventLogger.WriteToErrorLog(ex, "LoginGet");
                throw;
            }

        }


        public ActionResult LogOff()
        {
            try
            {
                Request.Cookies.Remove("UserName");
                Session.Remove("UserName");
                Session.Remove("UserId");
                Session.Remove("ModuleNames");
                Session.Remove("modulename");
                Session.Remove("ChildNames");
                Session.Remove("ChildName");
                FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                //   EventLogger.WriteToErrorLog(ex, "Logoff");
                throw;
            }

        }

        public ActionResult ErrorPageCall()
        {
            try
            {
                //Request.Cookies.Remove("UserName");
                //Session.Remove("UserName");
                //Session.Remove("UserId");
                //FormsAuthentication.SignOut();
                //Session.Abandon();
                return View();
            }
            catch (Exception ex)
            {
                //   EventLogger.WriteToErrorLog(ex, "ErrorpageCall");
                throw;
            }

        }


        public ActionResult UserLogin(Core.Login objLogin)
        {
            int Count = 0;

            if (objlogin.checkUser(objLogin)) // Calls the Login class checkUser() for existence of the user in the database.
            {
                // FormsAuthentication.SetAuthCookie(objLogin.username, false);

                Session["UserName"] = objLogin.username;

                Session["UserId"] = Convert.ToInt64(objLogin.userid);

                if (Session["UserName"] != null && Session["UserId"] != null)
                {

                    Count = 1;
                }
            }
            else
            {
                // return Content("<script language='javascript' type='text/javascript'>alert('Thanks for Feedback!');</script>");
                TempData["AlertMessage"] = "Invalid Username or Password";
                //return the same view with message "Invalid Username or Password"
            }
            return new JsonResult { Data = Count, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Menu
        public ActionResult ShowModuleNames()
        {
            List<Login> lstModules = new List<Login>();
            int userid = 0;

            userid = Convert.ToInt32(Session["UserId"]);
            if (Session["ModuleNames"] == null)
            {
                lstModules = objlogin.GetParentModuleNames(userid);
                Session["ModuleNames"] = lstModules;
            }
            else
            {
                lstModules = (List<Login>)Session["ModuleNames"];

            }

            if (Session["modulename"] != null)
            {
                modulename = Session["modulename"].ToString();
            }
            status = CheckSessions();
            if (lstModules.Count == 0)
                status = "NO";
            if (modulename == "")
                status = "NO";
            var Alldata = new { ModuleNames = lstModules, username = Session["UserName"], modulename = modulename, status = status };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public ActionResult BindChildNames(string ModuleId, string modulename)
        {
            List<Login> lstChildModules = new List<Login>();
            string url = string.Empty;
            int userid = Convert.ToInt32(Session["UserId"]);
            if (userid > 0)
            {
                lstChildModules = objlogin.GetChildFunctionNames(userid, ModuleId, "RMS");
                Session["ChildNames"] = lstChildModules;

                childname = lstChildModules[0].FunctionName;

                Session["ChildName"] = childname;
                Session["modulename"] = modulename;

                url = lstChildModules[0].FunctionUrl.Replace(".", "");

            }
            var Alldata = new { url = url, childname = childname, modulename = modulename, userid = userid };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


        }

        public ActionResult ShowChildNames()
        {

            List<Login> lstChildModules = new List<Login>();
            List<Login> lstModules = new List<Login>();
            status = CheckSessions();
            status = CheckChildSessions(status);
            if (Session["ChildNames"] != null)
            {
                lstChildModules = (List<Login>)Session["ChildNames"];
                if (lstChildModules.Count == 0)
                    status = "NO";
            }
            if (Session["ModuleNames"] != null)
            {

                lstModules = (List<Login>)Session["ModuleNames"];
                if (lstModules.Count == 0)
                    status = "NO";
            }
            if (Session["ChildName"] != null)
            {
                childname = Session["ChildName"].ToString();
                if (childname == "")
                    status = "NO";
            }
            if (Session["modulename"] != null)
            {
                modulename = Session["modulename"].ToString();
                if (modulename == "")
                    status = "NO";
            }

            var Alldata = new { ChildNames = lstChildModules, ModuleNames = lstModules, username = Session["UserName"], childname = childname, modulename = modulename, status = status };

            return new JsonResult { Data = Alldata, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public string CheckSessions()
        {
            string status = "YES";
            if (Session["UserId"] == null)
            {
                status = "NO";
            }
            if (Session["UserName"] == null)
            {
                status = "NO";
            }

            if (Session["ModuleNames"] == null)
            {
                status = "NO";
            }


            return status;
        }
        public string CheckChildSessions(string status)
        {
            if (Session["modulename"] == null)
            {
                status = "NO";
            }

            if (Session["ChildNames"] == null)
            {
                status = "NO";
            }
            if (Session["ChildName"] == null)
            {
                status = "NO";
            }

            return status;
        }

        public ActionResult GetChildName(string ChildName)
        {
            Session["ChildName"] = ChildName;
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckUserData()
        {
            string status = "YES";
            if (Session["UserId"] == null)
            {
                status = "NO";
            }
            if (Session["UserName"] == null)
            {
                status = "NO";
            }
            return new JsonResult { Data = status, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        /// <summary>
        /// MURALI
        /// </summary>
        #region UserRights
        public ActionResult UserRights()
        {
            return View();
        }
        //Binding Grid When Module Selection Change in User Access in User Rights
        [HttpPost]
        public JsonResult ShowRoleFunctions(string ID)
        {
            List<ModuleDTO> lstJQGridModel = new List<ModuleDTO>();
            lstJQGridModel = objlogin.ShowRoleFunctions(ID);
            var data = new JsonResult { Data = lstJQGridModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        //Binding Module name in Add Module in User Rights
        [HttpPost]
        public JsonResult ShowMainModules(string ID, string Form, string ModuleType)
        {
            List<ModuleDTO> lstModuleDTO = new List<ModuleDTO>();
            lstModuleDTO = objlogin.ShowMainModules(ID, Form, ModuleType);
            return new JsonResult { Data = lstModuleDTO, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Saving Existing user Password Changing in UserRights
        public bool SaveChangeUserPassword(UserInfo Employee)
        {
            return objlogin.SaveChangeUser(Employee);
        }

        //Saving New User Registration in UserRights
        public bool SaveNewUser(UserInfo Employee)
        {
            return objlogin.SaveNewUser(Employee);
        }


        //Binding New User Names with HRMS Employee in UserRights
        public JsonResult ShowEmployeeName()
        {
            //List<UserInfo> lstemployeename = new List<UserInfo>();
            object Totalresult = new object();
            var JsonString = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                dt = objlogin.ShowEmployeeName();
                JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                Totalresult = new { Data = JsonString };
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "ShowEmployeeName");

            }
            //var data = new JsonResult { Data = dt, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return Json(Totalresult, JsonRequestBehavior.AllowGet);
            //return data;
        }
        //Binding New User Names in UserRights
        public JsonResult ShowUserName()
        {
            List<UserInfo> lstemployeename = new List<UserInfo>();


            try
            {
                lstemployeename = objlogin.ShowUserName();
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");

            }

            var data = new JsonResult { Data = lstemployeename, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        //Binding  Module Names in Add Function From in UserRights
        public JsonResult ShowModuleName()
        {
            List<ModuleDTO> lstModule = new List<ModuleDTO>();
            try
            {
                lstModule = objlogin.ShowModuleName();
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            var data = new JsonResult { Data = lstModule, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        //Binding Grid when Module Salection Change in Add Function Form in UserRights
        [HttpPost]
        public JsonResult ShowModuleName(string ModuleName)
        {
            //select functionid,functionname,false as functionstatus from tabfunctions;
            List<FunctionsDTO> lstfunction = new List<FunctionsDTO>();
            try
            {
                lstfunction = objlogin.getFunctions(ModuleName);
            }
            catch (Exception ex)
            {
                EventLogger.WriteToErrorLog(ex, "UserRights");
            }
            var data = new JsonResult { Data = lstfunction, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }

        //Saving New Function in Add Function in UserRights
        public bool SaveFunction(FunctionsDTO FunctionDTO)
        {
            return objlogin.SaveFunction(FunctionDTO);
        }

        //Saving  Function Order in Add Function in UserRights
        [HttpPost]
        public bool SaveFunctionOrder(List<FunctionsDTO> FunctionDTO)
        {
            return objlogin.SaveFunctionOrder(FunctionDTO);
        }

        //Saving  Module in Add Module in UserRights
        public bool ModuleSave(ModuleDTO1 Module)
        {
            return objlogin.SaveModule(Module);
        }


        //Saving  User permissions in User Access from  in UserRights
        public bool SaveUserAuthentication(string jsonData, string UserID)
        {
            List<ModuleDTO> UserList = JsonConvert.DeserializeObject<List<ModuleDTO>>(jsonData);
            return objlogin.SaveUserAuthentication(UserList, UserID);
        }

        public JsonResult getDirPath()
        {
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/Views/"), "*.cshtml", SearchOption.AllDirectories);
            int Count = filePaths.Count();
            StringBuilder strBuilder = new StringBuilder();
            string str = string.Empty;
            string Controllerviewpath = string.Empty;

            for (int i = 0; i < Count; i++)
            {
                str = filePaths[i].ToString().Replace("C:\\Users\\EDPDOTNET\\Desktop\\HRMS_UserRights_NEW_UserRights\\HRMS\\Views\\", "");
                Controllerviewpath = str.ToString().Replace(".cshtml", "");
                strBuilder.AppendLine(Controllerviewpath + "_");
            }
            string TotalPaths = strBuilder.ToString();
            string[] paths = TotalPaths.Split('_');
            int PathLength = paths.Length;
            string strmodules = string.Empty;
            string checkmodule = string.Empty;
            string CheckStringModule = string.Empty;
            //for (int j = 0; j < PathLength; j++)
            //{

            //    CheckStringModule = paths[j].ToString().Trim();
            //    checkmodule = CheckStringModule.Split('\\')[1].ToString();
            //    strmodules += checkmodule + "_";
            //}
            //string[] Modules = strmodules.Split('_');
            //var getdata = new { Modules = Modules, Functions = paths };
            DataTable dt = new DataTable();
            if (PathLength > 0)
            {
                dt.Columns.Add("Module");
                dt.Columns.Add("Function");
                dt.Columns.Add("FunctionUrl");
                dt.Columns.Add("status");
                for (int j = 0; j < PathLength; j++)
                {
                    if (paths[j].Contains("\\"))
                    {
                        CheckStringModule = paths[j].ToString().Trim();
                        checkmodule = CheckStringModule.Split('\\')[0].ToString();
                        DataRow dr = dt.NewRow();
                        dr["Module"] = checkmodule;
                        dr["Function"] = CheckStringModule.Split('\\')[1].ToString();
                        dr["FunctionUrl"] = paths[j].ToString();
                        dr["status"] = false;
                        dt.Rows.Add(dr);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(dt, Formatting.None);
            var data = new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
            //return dt;
        }



        /// <summary>
        /// MURALI
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ModueTypeChange(string Type, string Form)
        {
            List<ModuleDTO> lstparentmodules = new List<ModuleDTO>();
            lstparentmodules = objlogin.ModueTypeChange(Type, Form);
            var data = new JsonResult { Data = lstparentmodules, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return data;
        }
        #endregion

        #region change password
        public ActionResult changepassword()
        {
            return View();
        }
        //Saving Existing user Password Changing in UserRights
        public bool SaveChangePassword(UserInfo Employee)
        {
            Employee.UserID = Convert.ToString(Session["UserId"]);
            return objlogin.SaveChangeUser(Employee);
        }
        #endregion
    }
}
