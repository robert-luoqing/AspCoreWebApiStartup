using SuperPartner.DataLayer.Common;
using SuperPartner.Model.Organization.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SuperPartner.Model.Common;
using SuperPartner.DataLayer.DataContext;
using SuperPartner.Model.Enums;
using Microsoft.EntityFrameworkCore;
using SuperPartner.Model.Exception;

namespace SuperPartner.DataLayer.Organization
{
    public class UserDao : DaoBase
    {
        public UserDao(DaoContext daoContext) : base(daoContext)
        {
        }

        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="keyword">The keyword which use to search user name or login name</param>
        /// <param name="pager">pager object</param>
        /// <returns>Return matched user infomation</returns>
        public virtual List<WsUserInfo> GetUserList(string keyword, WsPager pager)
        {
            var context = this.DaoContext.GetDbContext();
            var query = from o in context.User
                        select new WsUserInfo()
                        {
                            UserId = o.UserId,
                            UserName = o.UserName,
                            LoginName = o.LoginName,
                            Desc = o.Desc
                        };
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(o => o.UserName.Contains(keyword) || o.LoginName.Contains(keyword));
            }

            return this.ApplyPager(query, pager).ToList();
        }

        /// <summary>
        /// Get user count which matched search condition
        /// </summary>
        /// <param name="keyword">The keyword which use to search user name or login name</param>
        /// <returns>return matched user count</returns>
        public virtual int GetUserCount(string keyword)
        {
            var context = this.DaoContext.GetDbContext();
            var query = from o in context.User
                        select o;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(o => o.UserName.Contains(keyword) || o.LoginName.Contains(keyword));
            }

            return query.Count();
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>The new user id</returns>
        public virtual int AddUser(WsUserDetail user)
        {
            var context = this.DaoContext.GetDbContext();
            var dbEntity = new User();
            dbEntity.UserName = user.UserName;
            dbEntity.LoginName = user.LoginName;
            dbEntity.Password = user.Password;
            dbEntity.PwdExpredDate = user.PwdExpredDate;
            dbEntity.Status = (int)UserStatus.Normal;
            dbEntity.Desc = user.Desc;
            dbEntity.FailTimes = 0;
            dbEntity.ModifiedBy = user.ModifiedBy;
            dbEntity.ModifiedTime = DateTime.Now;
            dbEntity.CreatedBy = user.CreatedBy;
            dbEntity.CreatedTime = DateTime.Now;

            context.User.Add(dbEntity);
            context.SaveChanges();

            return dbEntity.UserId;
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">User object</param>
        public virtual void UpdateUser(WsUserDetail user)
        {
            var context = this.DaoContext.GetDbContext();
            var dbEntity = new User();
            dbEntity.UserId = user.UserId;
            context.User.Attach(dbEntity);
            if (context.Entry(dbEntity).State == EntityState.Added)
                throw new SpException("No record exist");

            dbEntity.UserName = user.UserName;
            dbEntity.LoginName = user.LoginName;
            dbEntity.Desc = user.Desc;
            dbEntity.ModifiedBy = user.ModifiedBy;
            dbEntity.ModifiedTime = DateTime.Now;
            context.SaveChanges();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId">The user id which need deleted</param>
        public virtual void DeleteUser(int userId)
        {
            var context = this.DaoContext.GetDbContext();
            var dbEntity = new User();
            dbEntity.UserId = userId;
            context.User.Attach(dbEntity);
            context.User.Remove(dbEntity);
            context.SaveChanges();
        }

        /// <summary>
        /// Get login user by login name
        /// </summary>
        /// <param name="loginName">Login name</param>
        /// <returns>The data of login</returns>
        public virtual List<LoginUser> GetLoginUserByLoginName(string loginName)
        {
            var context = this.DaoContext.GetDbContext();
            var query = from o in context.User
                        where o.LoginName == loginName
                        select new LoginUser()
                        {
                            UserId = o.UserId,
                            UserName = o.UserName,
                            LoginName = o.LoginName,
                            Password = o.Password,
                            FailTimes = o.FailTimes
                        };

            return query.ToList();
        }

        /// <summary>
        /// Increase fail times by user id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="failTime">Fail time which need to set</param>
        public virtual void UpdateFailTimes(int userId, int failTime)
        {
            var context = this.DaoContext.GetDbContext();
            var dbEntity = new User();
            dbEntity.UserId = userId;
            context.User.Attach(dbEntity);
            dbEntity.FailTimes = failTime;
            context.SaveChanges();
        }
    }
}
