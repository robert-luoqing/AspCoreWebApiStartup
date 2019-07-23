using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// The permission role 
    /// </summary>
    public class PermRole
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }

        /// <summary>
        /// The property use to for UI
        /// For example, If you want show in UI as a sort, you can use the field
        /// </summary>
        public Dictionary<string, string> ExtendProperties { get; set; }
    }
}
