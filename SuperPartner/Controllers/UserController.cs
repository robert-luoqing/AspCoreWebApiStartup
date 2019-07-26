using Microsoft.AspNetCore.Mvc;
using SuperPartner.Biz.Organization;
using SuperPartner.Filters;
using SuperPartner.Model.Common;
using SuperPartner.Model.Organization.User;
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

        /// <summary>
        /// Search user by keyword
        /// </summary>
        /// <param name="req">The conditio property is keyword which use to search User Name or Login Name</param>
        /// <returns>Matched user information</returns>
        [HttpPost("list")]
        [SpFunction("UserOperation")]
        public ActionResult<List<WsUserInfo>> GetUserList([FromBody] WsListRequest<string> req)
        {
            return this.userManager.GetUserList(req.Condition, req.Pager);
        }

        /// <summary>
        /// Get user count by keyword
        /// </summary>
        /// <param name="req">The conditio property is keyword which use to search User Name or Login Name</param>
        /// <returns>matched user count</returns>
        [HttpPost("count")]
        [SpFunction("UserOperation")]
        public ActionResult<WsResponse<int>> GetUserCount([FromBody] WsListRequest<string> req)
        {
            WsResponse<int> result = this.userManager.GetUserCount(req.Condition);
            return result;
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="user">user information</param>
        /// <returns>Return success status if success</returns>
        [HttpPost("add")]
        [SpFunction("UserOperation")]
        public ActionResult<WsResponse> AddUser([FromBody] WsUserDetail user)
        {
            this.userManager.AddUser(user);
            return new WsResponse();
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">user information</param>
        /// <returns>Return success status if success</returns>
        [HttpPost("update")]
        [SpFunction("UserOperation")]
        public ActionResult<WsResponse> UpdateUser([FromBody] WsUserDetail user)
        {
            this.userManager.UpdateUser(user);
            return new WsResponse();
        }

        /// <summary>
        /// Login by user name and password
        /// </summary>
        /// <remarks>
        /// <![CDATA[If login failed, the error message of trans object will return to invoke.]]>
        /// </remarks>
        /// <param name="loginName">login name</param>
        /// <param name="password">password</param>
        /// <returns>Return token if success, the exception will return if failed</returns>
        [HttpPost("login")]
        public ActionResult<WsResponse<string>> Login(string loginName, string password)
        {
            var token = this.userManager.Login(loginName, password);
            return new WsResponse<string>()
            {
                Data = token
            };
        }
    }
}
