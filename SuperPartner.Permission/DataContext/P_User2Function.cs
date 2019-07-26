using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SuperPartner.Permission.DataContext
{
    public class P_User2Function
    {
        public string UserId { get; set; }
        public string FuncCode { get; set; }
        public int? AccessLevel { get; set; }
    }
}
