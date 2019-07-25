using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.Authorization
{
    public class PermFunc
    {
        public string FuncCode { get; set; }
        public string FuncName { get; set; }
        /// <summary>
        /// Multiple url, the url is Regex, each url sperate by "\n"
        /// </summary>
        public string AssociateUrls { get; set; }
        public string FuncDesc { get; set; }

        /// <summary>
        /// The property use to for UI
        /// For example, If you want show in UI as a sort, you can use the field
        /// </summary>
        public Dictionary<string, string> ExtendProperties { get; set; }
    }
}
