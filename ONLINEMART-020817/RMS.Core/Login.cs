using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Core
{

    public class Login
    {
        public int ModuleId { get; set; }
        public string ParentModuleName { get; set; }
        public string ModuleName { get; set; }
        public string moduledescription { get; set; }
        public string moduleclass { get; set; }
        public string Functionclass { get; set; }
        public string FunctionName { get; set; }
        public string active { get; set; }
        public string FunctionUrl { get; set; }
        public string actionname { get; set; }
        public long userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public string ConfirmPassword { get; set; }

        public string NewPassword { get; set; }

        public string OldPassword { get; set; }
    }



    /// <summary>
    /// MURALI 
    /// </summary>

    #region Function
    public class FunctionsDTO
    {
        public string FunctionName { get; set; }
        public int FunctionId { get; set; }
        public bool FunctionStatus { get; set; }
        public string FunctionUrl { get; set; }
        public int Moduleid { get; set; }
        public int FunctionOrder { get; set; }
    }
    #endregion

    #region New User
    public class UserInfo
    {
        public string UserID { get; set; }

        public string Empname { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string NewPassword { get; set; }

        public string OldPassword { get; set; }

        public string EmployeeID { get; set; }
    }
    #endregion

    #region Module
    public class ModuleDTO1
    {
        public int ModuleId { get; set; }
        public string ParentModulename { get; set; }
        public string parentModuleId { get; set; }
        public string Modulename { get; set; }
        public bool chkStatus { get; set; }
        public string ModuleType { get; set; }
    }
    public class ModuleDTO
    {

        public int ModuleId { get; set; }
        public string ParentModulename { get; set; }
        public string Modulename { get; set; }
        public bool chkStatus { get; set; }

        public string parentModuleId { get; set; }

        public int functionid { get; set; }
        public string functionname { get; set; }
        public string functionurl { get; set; }
        public int Moduleid { get; set; }
        public string modulename { get; set; }
    }
    #endregion

    /// <summary>
    /// MURALI 
    /// </summary>

}
