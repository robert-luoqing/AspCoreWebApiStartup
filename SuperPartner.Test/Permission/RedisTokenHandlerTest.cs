using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperPartner.Permission.TokenHandler;
using SuperPartner.Utils.Json;
using SuperPartner.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SuperPartner.Test.Permission
{
    [TestClass]
    public class RedisTokenHandlerTest
    {
        [TestMethod]
        public void TestGeneToken()
        {
            var redisHelper = new PermissionRedisHelper("localhost", "Sp_Test");
            var redisTokenHandler = new RedisTokenHandler(redisHelper, "token");
            var token = redisTokenHandler.GeneToken("TokenObject");

            // Get date direct from token
            var tokenObj = redisHelper.Get("token:" + token);
            var str = JsonHelper.ToObj<string>(tokenObj, true);

            Assert.AreEqual(str, "TokenObject");
        }

        [TestMethod]
        public void TestRefreshToken()
        {
            var redisHelper = new PermissionRedisHelper("localhost", "Sp_Test");
            var redisTokenHandler = new RedisTokenHandler(redisHelper, "token");
            var token = redisTokenHandler.GeneToken("TokenObject");
            var timeout = redisTokenHandler.GetTimeout();
            Thread.Sleep(3000);
            var remainTimeout = redisHelper.GetExpireTime("token:" + token);
            Assert.IsTrue(remainTimeout <= timeout - 3);

            redisTokenHandler.RefreshToken(token);
            remainTimeout = redisHelper.GetExpireTime("token:" + token);
            Assert.IsTrue(remainTimeout > timeout - 3);
        }

        [TestMethod]
        public void TestRemoveToken()
        {
            var redisHelper = new PermissionRedisHelper("localhost", "Sp_Test");
            var redisTokenHandler = new RedisTokenHandler(redisHelper, "token");
            var token = redisTokenHandler.GeneToken("TokenObject");

            redisTokenHandler.RemoveToken(token);

            var exist = redisHelper.ContainsKey("token:" + token);
            Assert.AreEqual(exist, false);
        }

        [TestMethod]
        public void TestUpdateAssociateDataByToken()
        {
            var redisHelper = new PermissionRedisHelper("localhost", "Sp_Test");
            var redisTokenHandler = new RedisTokenHandler(redisHelper, "token");
            var token = redisTokenHandler.GeneToken("TokenObject");

            redisTokenHandler.UpdateAssociateDataByToken(token, "TokenObject2");

            // Get date direct from token
            var tokenObj = redisHelper.Get("token:" + token);
            var str = JsonHelper.ToObj<string>(tokenObj, true);

            Assert.AreEqual(str, "TokenObject2");
        }

        [TestMethod]
        public void GetAssociateObjectByToken()
        {
            var redisHelper = new PermissionRedisHelper("localhost", "Sp_Test");
            var redisTokenHandler = new RedisTokenHandler(redisHelper, "token");
            var token = redisTokenHandler.GeneToken("TokenObject");

            var str = redisTokenHandler.GetAssociateObjectByToken<string>(token);

            Assert.AreEqual(str, "TokenObject");
        }
    }
}
