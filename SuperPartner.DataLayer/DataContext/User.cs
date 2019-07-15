using System;
using System.Collections.Generic;

namespace SuperPartner.DataLayer.DataContext
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public DateTime? PwdExpredDate { get; set; }
        public int? Status { get; set; }
        public string Desc { get; set; }
        public int? FailTimes { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
