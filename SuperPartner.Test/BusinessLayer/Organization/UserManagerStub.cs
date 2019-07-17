using SuperPartner.DataLayer.DataContext;
using SuperPartner.DataLayer.Organization;
using SuperPartner.Model.Organization.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SuperPartner.Utils.Helper;

namespace SuperPartner.Test.BusinessLayer.Organization
{
    public class UserManagerStub : UserDao
    {
        private List<User> list;

        public UserManagerStub(List<User> list) : base(null)
        {
            this.list = list;
        }

        public override List<LoginUser> GetLoginUserByLoginName(string loginName)
        {
            return this.list.Where(o => o.LoginName == loginName).Select(o =>
            {
                var result = new LoginUser();
                ObjectHelper.CopyPropertyValue(o, result);
                return result;
            }).ToList();
        }

        public override void UpdateFailTimes(int userId, int failTime)
        {
            this.list.Where(o => o.UserId == userId).Single().FailTimes = failTime;
        }
    }
}
