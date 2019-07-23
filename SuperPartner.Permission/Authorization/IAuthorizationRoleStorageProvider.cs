using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// User--assign---funcs
    /// User--assign---roles
    /// Role--include---funcs
    /// </summary>
    public interface IAuthorizationRoleStorageProvider: IAuthorizationStorageProvider
    {
        int CreateRole(PermRole role);
        void RemoveRole(string roleId);
        List<PermRole> GetRoles(bool includeDesc, bool includeExtendProperties);


        void AddRoleToUser(string userId, string roleId);
        void RemoveRoleFromUser(string userId, string roleId);
        List<PermRole> GetRolesByUser(string userId, bool includeDesc, bool includeExtendProperties);


        void AssignWholeFuncsToRole(string roleId, List<FuncAssignation> funcCodes);
        void AddFuncToRole(string roleId, string funcCode, int level);
        void RemoveFuncToRole(string roleId, string funcCode);
        List<FuncAssignation> GetFuncsByRole(string roleId, bool includeDesc, bool includeExtendProperties);
    }
}
