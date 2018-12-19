using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RMS.Core.Interfaces
{
    public interface ILogin
    {
        #region Login
        bool checkUser(Core.Login objLogin);

        #endregion

        #region Modules...

        List<Login> GetParentModuleNames(int userid);
        List<Login> GetChildFunctionNames(int userid, string ModuleId, string type);

        #endregion


        /// <summary>
        /// MURALI A
        /// </summary>
        #region  User Rights
        DataTable ShowEmployeeName();
        List<ModuleDTO> ShowRoleFunctions(string ID);
        bool SaveNewUser(UserInfo Employee);
        bool SaveChangeUser(UserInfo Employee);
        List<UserInfo> ShowUserName();
        List<ModuleDTO> ShowModuleName();
        List<FunctionsDTO> getFunctions(string Moduleid);
        bool SaveFunction(FunctionsDTO FunctionsDTO);
        bool SaveFunctionOrder(List<FunctionsDTO> FunctionDTO);
        bool SaveUserAuthentication(List<ModuleDTO> UserList, string UserID);
        bool SaveModule(ModuleDTO1 objModule);
        List<ModuleDTO> ShowMainModules(string ID, string Form, string ModuleType);
        List<ModuleDTO> ModueTypeChange(string Type, string Form);
        #endregion



    }
}
