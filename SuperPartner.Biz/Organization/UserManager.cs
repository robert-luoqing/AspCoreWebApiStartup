using SuperPartner.Biz.Common;
using SuperPartner.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Biz.Organization
{
    public class UserManager : ManagerBase
    {
        private CommonRedisHelper commonRedisHelper;
        public UserManager(CommonRedisHelper commonRedisHelper)
        {
            this.commonRedisHelper = commonRedisHelper;
        }

        public List<string> GetUserList()
        {
            this.commonRedisHelper.Set("Test", "12243");
            return new List<string>()
            {
                "test1", "test2"
            };
        }
    }
}
