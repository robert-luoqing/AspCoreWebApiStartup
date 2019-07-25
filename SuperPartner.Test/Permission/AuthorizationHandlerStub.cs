using SuperPartner.Permission.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperPartner.Test.Permission
{
    public class AuthorizationHandlerStub : IAuthorizationStorageProvider
    {
        public Dictionary<string, List<FuncAssignation>> user2FuncsInMemory = new Dictionary<string, List<FuncAssignation>>();
        public List<PermFunc> funcsInMemory = new List<PermFunc>();
        public int accessTimes = 0;

        public void AddFuncToUser(string userId, string funcCode, AccessLevel level)
        {
            if (!this.user2FuncsInMemory.ContainsKey(userId))
                this.user2FuncsInMemory.Add(userId, new List<FuncAssignation>());
            var user2Funcs = this.user2FuncsInMemory[userId];
            var removedFuncs = user2Funcs.Where(o => o.FuncCode == funcCode).ToList();
            foreach (var item in removedFuncs)
                user2Funcs.Remove(item);
            user2Funcs.Add(new FuncAssignation()
            {
                FuncCode = funcCode,
                AccessLevel = level
            });
        }

        public void AssignWholeFuncsToUser(string userId, List<FuncAssignation> funcCodes)
        {
            this.user2FuncsInMemory[userId] = funcCodes;
        }

        public void CreateFunc(PermFunc func)
        {
            var removedFuncs = funcsInMemory.Where(o => o.FuncCode == func.FuncCode).ToList();
            foreach (var item in removedFuncs)
                funcsInMemory.Remove(item);
            funcsInMemory.Add(func);
        }

        public List<PermFunc> GetFuncs(bool includeDesc, bool includeExtendProperties)
        {
            accessTimes++;
            return this.funcsInMemory;
        }

        public List<FuncAssignation> GetFuncsByUser(string userId)
        {
            accessTimes++;
            if (this.user2FuncsInMemory.ContainsKey(userId))
                return this.user2FuncsInMemory[userId];
            return new List<FuncAssignation>();
        }

        public void RemoveFunc(string funcCode)
        {
            var removedFuncs = funcsInMemory.Where(o => o.FuncCode == funcCode).ToList();
            foreach (var item in removedFuncs)
                funcsInMemory.Remove(item);
        }

        public void RemoveFuncFromUser(string userId, string funcCode)
        {
            if (this.user2FuncsInMemory.ContainsKey(userId))
            {
                var user2Funcs = this.user2FuncsInMemory[userId];
                var removedFuncs = user2Funcs.Where(o => o.FuncCode == funcCode).ToList();
                foreach (var item in removedFuncs)
                    user2Funcs.Remove(item);
            }
        }
    }
}
