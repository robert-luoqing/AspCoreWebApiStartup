using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperPartner.Permission.Authorization;
using SuperPartner.Permission.DataContext;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SuperPartner.Test.Permission
{
    [TestClass]
    public class AuthorizationStorageProviderTest
    {
        private string connectionString = "Server=localhost;Database=SpFramework;Trusted_Connection=True;";


        [TestInitialize]
        public void Initial()
        {
            var dataContext = GetDbContext();
            dataContext.Database.ExecuteSqlCommand("DELETE FROM P_Functions");
            dataContext.Database.ExecuteSqlCommand("DELETE FROM P_User2Functions");
        }

        [TestMethod]
        public void TestCreateOrUpdateFunc()
        {
            var dataContext = GetDbContext();

            //Test
            var provider = new AuthorizationStorageProvider(dataContext);
            var extendProperties = new Dictionary<string, string>();
            extendProperties.Add("Index", "0001");
            var funcCode = Guid.NewGuid().ToString();
            provider.CreateOrUpdateFunc(new PermFunc()
            {
                FuncCode = funcCode,
                FuncName = "User Operation",
                FuncDesc = "Operating user module, like query user list, add/update/delete user",
                AssociateUrls = "^/api/user/",
                ExtendProperties = extendProperties
            });

            //Verify
            var dbData = dataContext.P_Functions
                .FromSql($"SELECT * FROM P_Functions WHERE FuncCode=@FuncCode", new SqlParameter("FuncCode", funcCode))
                .ToList();

            Assert.AreEqual(dbData.Count, 1);
            Assert.AreEqual(dbData[0].FuncCode, funcCode);
            Assert.AreEqual(dbData[0].FuncName, "User Operation");
        }

        [TestMethod]
        public void TestRemoveFunc()
        {
            var dataContext = GetDbContext();

            // Prepare data
            var provider = new AuthorizationStorageProvider(dataContext);
            var funcCode = Guid.NewGuid().ToString();
            provider.CreateOrUpdateFunc(new PermFunc()
            {
                FuncCode = funcCode,
                FuncName = "User Operation",
            });

            // Test it
            provider.RemoveFunc(funcCode);

            //Verify
            var dbData = dataContext.P_Functions
                .FromSql($"SELECT * FROM P_Functions WHERE FuncCode=@FuncCode", new SqlParameter("FuncCode", funcCode))
                .ToList();

            Assert.AreEqual(dbData.Count, 0);
        }

        [TestMethod]
        public void TestGetFuncs()
        {
            var dataContext = GetDbContext();
            // Prepare data
            var provider = new AuthorizationStorageProvider(dataContext);
            var funcCode = CreatePrepareData(provider);

            // Test it
            var results = provider.GetFuncs(true, true);
            var matched = results.Where(o => o.FuncCode == funcCode).ToList();

            Assert.AreEqual(matched.Count, 1);
            Assert.AreEqual(matched[0].FuncDesc, "Operating user module, like query user list, add/update/delete user");
            Assert.IsTrue(matched[0].ExtendProperties["Index"]=="0001");
        }

        private string CreatePrepareData(AuthorizationStorageProvider provider)
        {
            var funcCode = Guid.NewGuid().ToString();
            var extendProperties = new Dictionary<string, string>();
            extendProperties.Add("Index", "0001");
            provider.CreateOrUpdateFunc(new PermFunc()
            {
                FuncCode = funcCode,
                FuncName = "User Operation",
                FuncDesc = "Operating user module, like query user list, add/update/delete user",
                AssociateUrls = "^/api/user/",
                ExtendProperties = extendProperties
            });

            return funcCode;
        }

        [TestMethod]
        public void TestAssignWholeFuncsToUser()
        {
            var dataContext = GetDbContext();
            // Prepare data
            var provider = new AuthorizationStorageProvider(dataContext);
            var funcCode1 = Guid.NewGuid().ToString();
            var funcCode2 = Guid.NewGuid().ToString();

            // Test
            provider.AssignWholeFuncsToUser("Robert456",
                new List<FuncAssignation>()
                {
                    new FuncAssignation() { FuncCode=funcCode1, AccessLevel=AccessLevel.AccessSelfData},
                    new FuncAssignation() { FuncCode=funcCode2, AccessLevel=AccessLevel.AccessSelfData},
                });

            // verify
            var dbData = dataContext.P_User2Functions
                .FromSql($"SELECT * FROM P_User2Functions WHERE UserId='Robert456'")
                .ToList();

            Assert.AreEqual(dbData.Count, 2);
            Assert.AreEqual(dbData[0].AccessLevel, (int)AccessLevel.AccessSelfData);
        }

        [TestMethod]
        public void TestAssignFuncToUser()
        {
            var dataContext = GetDbContext();
            // Prepare data
            var provider = new AuthorizationStorageProvider(dataContext);
            var funcCode1 = Guid.NewGuid().ToString();
            var funcCode2 = Guid.NewGuid().ToString();

            // Test
            provider.AssignFuncToUser("Robert123", funcCode2, AccessLevel.AccessSelfData);
            provider.AssignFuncToUser("Robert123", funcCode1, AccessLevel.AccessCompanyData);

            // verify
            var dbData = dataContext.P_User2Functions
                .FromSql($"SELECT * FROM P_User2Functions WHERE FuncCode=@FuncCode and UserId='Robert123'", new SqlParameter("FuncCode", funcCode1))
                .ToList();

            Assert.AreEqual(dbData.Count, 1);
            Assert.AreEqual(dbData[0].AccessLevel, (int)AccessLevel.AccessCompanyData);
        }

        [TestMethod]
        public void TestRemoveFuncFromUser()
        {
            var dataContext = GetDbContext();
            // Prepare data
            var provider = new AuthorizationStorageProvider(dataContext);
            var funcCode1 = Guid.NewGuid().ToString();
            var funcCode2 = Guid.NewGuid().ToString();
            provider.AssignFuncToUser("Robert111", funcCode2, AccessLevel.AccessSelfData);
            provider.AssignFuncToUser("Robert111", funcCode1, AccessLevel.AccessCompanyData);

            // Test
            provider.RemoveFuncFromUser("Robert111", funcCode2);

            // verify
            var dbData = dataContext.P_User2Functions
                .FromSql($"SELECT * FROM P_User2Functions WHERE FuncCode=@FuncCode and UserId='Robert111'", new SqlParameter("FuncCode", funcCode2))
                .ToList();

            Assert.AreEqual(dbData.Count, 0);
        }

        [TestMethod]
        public void TestGetFuncsByUser()
        {
            var dataContext = GetDbContext();
            // Prepare data
            var provider = new AuthorizationStorageProvider(dataContext);
            var funcCode1 = Guid.NewGuid().ToString();
            var funcCode2 = Guid.NewGuid().ToString();
            provider.AssignFuncToUser("Robert342", funcCode2, AccessLevel.AccessSelfData);
            provider.AssignFuncToUser("Robert342", funcCode1, AccessLevel.AccessCompanyData);

            // Test
            var results = provider.GetFuncsByUser("Robert342");

            // verify
            var isMatched = results.Where(o => o.FuncCode == funcCode1 || o.FuncCode == funcCode2).ToList();
            Assert.AreEqual(isMatched.Count, 2);
        }


        private PermissionDataContext GetDbContext()
        {
            var myLoggerFactory = new LoggerFactory(new[] {
                new ConsoleLoggerProvider((_, __) =>
                {
                    return true;
                }, true) });
            var optionsBuilder = new DbContextOptionsBuilder<PermissionDataContext>();
            optionsBuilder.UseLoggerFactory(myLoggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer(connectionString);
            var dataContext = new PermissionDataContext(optionsBuilder.Options);
            return dataContext;
        }
    }
}
