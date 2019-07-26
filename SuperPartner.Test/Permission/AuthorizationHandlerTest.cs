using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperPartner.Permission.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Test.Permission
{
    [TestClass]
    public class AuthorizationHandlerTest
    {
        [TestMethod]
        public void TestCheckFunctionHasPermission()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckFunction("1", "UserPermission");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCheckFunctionHasPermissionUseCache()
        {
            var stub = CreateStub();
            var authorizationHandler = new AuthorizationHandler(stub);
            authorizationHandler.UseCache = true;
            authorizationHandler.CheckFunction("1", "UserPermission");
            var accessStorageTimes = stub.accessTimes;
            authorizationHandler.CheckFunction("1", "UserPermission");

            Assert.IsTrue(accessStorageTimes == stub.accessTimes);
        }

        [TestMethod]
        public void TestCheckFunctionHasNoPermission()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckFunction("1", "CustomerPermission");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCheckUrlHasPermission()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckUrl("1", "/api/TEST", null, null);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCheckUrlHasPermission2()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckUrl("1", "/api/Tns/abc?a=23", null, null);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCheckUrlHasNoPermission1()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckUrl("1", "a/api/TEST", null, null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCheckUrlHasNoPermission2()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckUrl("1", "api/TEST", null, null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCheckUrlHasNoPermission3()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckUrl("1", "/api/TEST/abc?c=2", null, null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCheckUrlHasNoPermission4()
        {
            var stub = CreateStub();

            var authorizationHandler = new AuthorizationHandler(stub);
            var result = authorizationHandler.CheckUrl("1", "api/Tns", null, null);
            Assert.IsFalse(result);
        }

        private static AuthorizationHandlerStub CreateStub()
        {
            var stub = new AuthorizationHandlerStub();
            stub.funcsInMemory = new List<PermFunc>()
            {
                new PermFunc() {FuncCode="UserPermission", AssociateUrls="^/api/test$\n/api/Tns.*"}
            };
            stub.user2FuncsInMemory.Add("1", new List<FuncAssignation>() {
                new FuncAssignation() { FuncCode = "UserPermission" }
            });
            return stub;
        }
    }
}
