using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperPartner.Biz.Common;
using SuperPartner.Biz.Organization;
using SuperPartner.DataLayer.DataContext;
using SuperPartner.Model.Common;
using SuperPartner.Model.Exception;
using SuperPartner.Model.Organization.User;
using SuperPartner.Permission.TokenHandler;
using SuperPartner.Utils.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Test.BusinessLayer.Organization
{
    [TestClass]
    public class UserManagerTest
    {
        /// <summary>
        /// User name/password correct
        /// Check1: a token given
        /// Check2: Trytimes clear to zore
        /// </summary>
        [TestMethod]
        public void TestLoginWithNormalCorrectLogin()
        {
            // Prepare env.
            var users = this.GetPrepareUsers();
            var stub = new UserManagerStub(users);

            var bizContext = GetBizContext();
            var userManager = new UserManager(bizContext, stub);
            var token = userManager.Login("admin", "123456");

            // Check
            var loginUser = bizContext.TokenHandler.GetAssociateObjectByToken<LoginUser>(token);
            Assert.AreEqual(loginUser.UserId, 1);
            Assert.AreEqual(users[0].FailTimes, 0);
        }

        /// <summary>
        /// User name/password correct(trytimes out of control)
        /// Check1: an correct exception will given
        /// Check2: Trytime have inceased
        /// </summary>
        [TestMethod]
        public void TestLoginWithCorrectUserAndPwdButTrytimesExtend()
        {
            var users = this.GetPrepareUsers();
            var stub = new UserManagerStub(users);
            var bizContext = GetBizContext();

            users[0].FailTimes = 10;
            bizContext.Configuration.MaxLoginTryTimes = 10;

            var userManager = new UserManager(bizContext, stub);

            try
            {
                var token = userManager.Login("admin", "123456");
            }
            catch (SpException ex)
            {
                Assert.AreEqual(ex.Message, "The account locked, because login times reach maxinum");
                Assert.AreEqual(users[0].FailTimes, 11);
            }
        }

        /// <summary>
        /// User name/password incorrect(trytimes in control)
        /// Check1: an correct exception will given
        /// Check2: Trytime have inceased
        /// </summary>
        [TestMethod]
        public void TestLoginWithNormalIncorrectLogin()
        {
            var users = this.GetPrepareUsers();
            var stub = new UserManagerStub(users);
            var bizContext = GetBizContext();

            users[0].FailTimes = 0;

            var userManager = new UserManager(bizContext, stub);

            try
            {
                var token = userManager.Login("admin", "1234567");
            }
            catch (SpException ex)
            {
                Assert.AreEqual(ex.Message, "The user name and password incorrect");
                Assert.AreEqual(users[0].FailTimes, 1);
            }
        }

        /// <summary>
        /// User name/password incorrect(trytimes in control)
        /// Check1: an correct exception will given
        /// Check2: Trytime have inceased
        /// </summary>
        [TestMethod]
        public void TestLoginWithNormalIncorrectLoginAndTryMultipleTimes()
        {
            var users = this.GetPrepareUsers();
            var stub = new UserManagerStub(users);
            var bizContext = GetBizContext();

            users[0].FailTimes = 8;
            bizContext.Configuration.MaxLoginTryTimes = 10;

            var userManager = new UserManager(bizContext, stub);

            try
            {
                var token = userManager.Login("admin", "1234567");
            }
            catch (SpException ex)
            {
                Assert.AreEqual(ex.Message, "The user name and password incorrect. You have 1 time(s) to try");
                Assert.AreEqual(users[0].FailTimes, 9);
            }
        }

        /// <summary>
        /// User name/password incorrect(trytimes in control)
        /// Check1: an correct exception will given
        /// Check2: Trytime have inceased
        /// </summary>
        [TestMethod]
        public void TestLoginWithTryMultipleTimesIncorrectLogin()
        {
            var users = this.GetPrepareUsers();
            var stub = new UserManagerStub(users);
            var bizContext = GetBizContext();

            users[0].FailTimes = 10;
            bizContext.Configuration.MaxLoginTryTimes = 10;

            var userManager = new UserManager(bizContext, stub);

            try
            {
                var token = userManager.Login("admin", "123456");
            }
            catch (SpException ex)
            {
                Assert.AreEqual(ex.Message, "The account locked, because login times reach maxinum");
                Assert.AreEqual(users[0].FailTimes, 11);
            }
        }

        private List<User> GetPrepareUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    UserId=1,
                    LoginName="admin",
                    Password = MD5Helper.ToMD5("123456"),
                    FailTimes = 2
                }
            };
        }

        private BizContext GetBizContext()
        {
            var tokenHandler = new MemoryTokenHandler();
            var bizContext = new BizContext(new SpConfiguration()
            {
                MaxLoginTryTimes = 10
            });
            bizContext.TokenHandler = tokenHandler;
            return bizContext;
        }
    }
}
