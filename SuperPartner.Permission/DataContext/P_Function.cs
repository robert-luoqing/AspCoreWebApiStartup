using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SuperPartner.Permission.DataContext
{
    public class P_Function
    {
        public string FuncCode { get; set; }
        public string FuncName { get; set; }
        public string AssociateUrls { get; set; }
        public string FuncDesc { get; set; }
        public string ExtendProperties { get; set; }
    }
}
