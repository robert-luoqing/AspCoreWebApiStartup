using Microsoft.AspNetCore.Mvc;
using SuperPartner.Biz.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperPartner.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController
    {
        private UserManager userManager;
        public UserController(UserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("list")]
        public ActionResult<List<string>> GetUserList([FromBody] string value)
        {
            return this.userManager.GetUserList();
        }


        [HttpGet("count")]
        public ActionResult<List<string>> GetUserCount()
        {
            return this.userManager.GetUserList();
        }
    }
}
