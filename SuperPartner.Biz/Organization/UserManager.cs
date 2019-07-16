using SuperPartner.Biz.Common;
using SuperPartner.DataLayer.Organization;
using SuperPartner.Model.Common;
using SuperPartner.Model.Organization.User;
using SuperPartner.Utils.Cryptography;
using SuperPartner.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Biz.Organization
{
    public class UserManager : ManagerBase
    {
        private UserDao userDao;
        public UserManager(BizContext bizContext, UserDao userDao) : base(bizContext)
        {
            this.userDao = userDao;
        }

        public string Login(string userName, string password)
        {
            // Verify user name and password
            // .....
            var token = this.BizContext.TokenHandler.GeneToken(user);
            return token;
        }

        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="keyword">The keyword which use to search user name or login name</param>
        /// <param name="pager">pager object</param>
        /// <returns>Return matched user infomation</returns>
        public List<WsUserInfo> GetUserList(string keyword, WsPager pager)
        {
            var result = this.userDao.GetUserList(keyword, pager);
            return result;
        }

        /// <summary>
        /// Get user count which matched search condition
        /// </summary>
        /// <param name="keyword">The keyword which use to search user name or login name</param>
        /// <returns>return matched user count</returns>
        public int GetUserCount(string keyword)
        {
            var result = this.userDao.GetUserCount(keyword);
            return result;
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="user">User object</param>
        public void AddUser(WsUserDetail user)
        {
            user.PwdExpredDate = DateTime.Now.AddDays(this.BizContext.Configuration.PwdExpiredInterval);
            // Encode password to md5
            user.Password = MD5Helper.ToMD5(user.Password);
            this.userDao.AddUser(user);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">User object</param>
        public void UpdateUser(WsUserDetail user)
        {
            this.userDao.UpdateUser(user);
        }
    }
}
