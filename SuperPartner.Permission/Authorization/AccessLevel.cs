using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// The access level which use to assign user
    /// </summary>
    public enum AccessLevel
    {
        AccessSelfData =1,
        AccessDepartmentData = 2,
        AccessOrganizationData = 3,
        AccessCompanyData = 4,

        DenyAccessData = 9
    }
}
