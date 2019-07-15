using SuperPartner.DataLayer.Common;
using SuperPartner.Model.Organization.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SuperPartner.Model.Common;
using SuperPartner.DataLayer.DataContext;
using SuperPartner.Model.Enums;

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
        public List<WsUserInfo> GetUserList(string keyword, WsPager pager)
        {
            using (var context = this.DaoContext.GetDbContext())
            {
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
        }

        /// <summary>
        /// Get user count which matched search condition
        /// </summary>
        /// <param name="keyword">The keyword which use to search user name or login name</param>
        /// <returns>return matched user count</returns>
        public int GetUserCount(string keyword)
        {
            using (var context = this.DaoContext.GetDbContext())
            {
                var query = from o in context.User
                            select o;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim();
                    query = query.Where(o => o.UserName.Contains(keyword) || o.LoginName.Contains(keyword));
                }

                return query.Count();
            }
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>The new user id</returns>
        public int AddUser(WsUserDetail user)
        {
            using (var context = this.DaoContext.GetDbContext())
            {
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
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user">User object</param>
        public void UpdateUser(WsUserDetail user)
        {
            using (var context = this.DaoContext.GetDbContext())
            {
                var dbEntity = new User();
                dbEntity.UserId = user.UserId;
                context.User.Attach(dbEntity);

                dbEntity.UserName = user.UserName;
                dbEntity.LoginName = user.LoginName;
                dbEntity.Desc = user.Desc;
                dbEntity.ModifiedBy = user.ModifiedBy;
                dbEntity.ModifiedTime = DateTime.Now;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId">The user id which need deleted</param>
        public void DeleteUser(int userId)
        {
            using (var context = this.DaoContext.GetDbContext())
            {
                var dbEntity = new User();
                dbEntity.UserId = userId;
                context.User.Attach(dbEntity);
                context.User.Remove(dbEntity);
                context.SaveChanges();
            }
        }
    }
}
