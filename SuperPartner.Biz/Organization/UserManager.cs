using SuperPartner.Biz.Common;
using SuperPartner.DataLayer.Organization;
using SuperPartner.Model.Common;
using SuperPartner.Model.Exception;
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

        /// <summary>
        /// The handler have below step
        /// 1. Get user by login name
        /// 2. Compaire the trytimes of failed, if failed try times above a special number which defined in setting. the direct return(throw) error
        /// 3. if password incorrect, increase trytimes
        /// </summary>
        /// <param name="loginName">login name</param>
        /// <param name="password">password</param>
        /// <returns>return token</returns>
        public string Login(string loginName, string password)
        {
            if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(password))
                throw new SpException("The login name and password do not allow to be null");

            var users = this.userDao.GetLoginUserByLoginName(loginName.Trim());
            if (users.Count != 1)
                throw new SpException("The user name and password incorrect");
            var user = users[0];

            var remainTimes = (this.BizContext.Configuration.MaxLoginTryTimes ?? int.MaxValue) - (user.FailTimes ?? 0);
            if (remainTimes <= 0)
            {
                this.userDao.UpdateFailTimes(user.UserId, (user.FailTimes ?? 0) + 1);
                throw new SpException("The account locked, because login times reach maxinum");
            }

            if (user.Password != MD5Helper.ToMD5(password))
            {
                this.userDao.UpdateFailTimes(user.UserId, (user.FailTimes ?? 0) + 1);
                if (remainTimes < 4)
                    throw new SpException("The user name and password incorrect. You have " + (remainTimes - 1).ToString() + " time(s) to try");
                else
                    throw new SpException("The user name and password incorrect");
            }

            this.userDao.UpdateFailTimes(user.UserId, 0);
            user.Password = null;
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
