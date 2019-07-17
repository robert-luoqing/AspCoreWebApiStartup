using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Model.Organization.User
{
    public class LoginUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public int? FailTimes { get; set; }
    }
}
