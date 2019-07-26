using Microsoft.EntityFrameworkCore;
using SuperPartner.Permission.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SuperPartner.Permission.Authorization
{
    /// <summary>
    /// Notice, The provide mix the logic and access togather
    /// So in these implement also include a little logic
    /// </summary>
    public class AuthorizationStorageProvider : IAuthorizationStorageProvider
    {
        private PermissionDataContext dataContext;
        public AuthorizationStorageProvider(PermissionDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void AssignFuncToUser(string userId, string funcCode, AccessLevel level)
        {
            var dbEntity = dataContext.P_User2Functions.Where(o => o.UserId == userId && o.FuncCode == funcCode).SingleOrDefault();
            if (dbEntity == null)
            {
                dbEntity = new P_User2Function();
                dbEntity.UserId = userId;
                dbEntity.FuncCode = funcCode;
                dataContext.P_User2Functions.Add(dbEntity);
            }

            dbEntity.AccessLevel = (int)level;
            dataContext.SaveChanges();
        }

        public void AssignWholeFuncsToUser(string userId, List<FuncAssignation> funcCodes)
        {
            var query = from o in dataContext.P_User2Functions
                        where o.UserId == userId
                        select o;
            dataContext.P_User2Functions.RemoveRange(query);
            dataContext.SaveChanges();
            foreach (var funcCodeObj in funcCodes)
            {
                var dbEntity = new P_User2Function();
                dbEntity.UserId = userId;
                dbEntity.FuncCode = funcCodeObj.FuncCode;
                if (funcCodeObj.AccessLevel != null)
                    dbEntity.AccessLevel = (int)funcCodeObj.AccessLevel.Value;
                dataContext.P_User2Functions.Add(dbEntity);
            }

            dataContext.SaveChanges();
        }

        public void CreateOrUpdateFunc(PermFunc func)
        {
            var dbEntity = dataContext.P_Functions.Where(o => o.FuncCode == func.FuncCode).SingleOrDefault();
            if (dbEntity == null)
            {
                dbEntity = new P_Function();
                dbEntity.FuncCode = func.FuncCode;
                dataContext.P_Functions.Add(dbEntity);
            }

            dbEntity.FuncName = func.FuncName;
            dbEntity.AssociateUrls = func.AssociateUrls;
            dbEntity.FuncDesc = func.FuncDesc;
            if (func.ExtendProperties != null)
                dbEntity.ExtendProperties = this.DictionaryToXml(func.ExtendProperties);
            else
                dbEntity.ExtendProperties = null;
            dataContext.SaveChanges();
        }

        public List<PermFunc> GetFuncs(bool includeDesc, bool includeExtendProperties)
        {
            // The Linq very difficult to write concat sql string, So here will use four if to implement it;
            // In here, in order to save performance, we are not get all data from databse. We just got the properties which we wanted.
            List<P_Function> list = null;
            if (includeDesc == true && includeExtendProperties == false)
            {
                var query = from o in dataContext.P_Functions
                            select new P_Function()
                            {
                                FuncCode = o.FuncCode,
                                FuncName = o.FuncName,
                                AssociateUrls = o.AssociateUrls,
                                FuncDesc = o.FuncDesc
                            };
                list = query.ToList();
            }
            else if (includeDesc == false && includeExtendProperties == true)
            {
                var query = from o in dataContext.P_Functions
                            select new P_Function()
                            {
                                FuncCode = o.FuncCode,
                                FuncName = o.FuncName,
                                AssociateUrls = o.AssociateUrls,
                                ExtendProperties = o.ExtendProperties
                            };
                list = query.ToList();
            }
            else if (includeDesc == false && includeExtendProperties == false)
            {
                var query = from o in dataContext.P_Functions
                            select new P_Function()
                            {
                                FuncCode = o.FuncCode,
                                FuncName = o.FuncName,
                                AssociateUrls = o.AssociateUrls
                            };
                list = query.ToList();
            }
            else
            {
                var query = from o in dataContext.P_Functions
                            select new P_Function()
                            {
                                FuncCode = o.FuncCode,
                                FuncName = o.FuncName,
                                AssociateUrls = o.AssociateUrls,
                                FuncDesc = o.FuncDesc,
                                ExtendProperties = o.ExtendProperties
                            };
                list = query.ToList();
            }

            var result = new List<PermFunc>();
            foreach (var dbItem in list)
            {
                var convertedObj = new PermFunc();
                convertedObj.FuncCode = dbItem.FuncCode;
                convertedObj.FuncName = dbItem.FuncName;
                convertedObj.AssociateUrls = dbItem.AssociateUrls;
                convertedObj.FuncDesc = dbItem.FuncDesc;
                if (!string.IsNullOrWhiteSpace(dbItem.ExtendProperties))
                    convertedObj.ExtendProperties = this.XmlToDictionary(dbItem.ExtendProperties);
                result.Add(convertedObj);
            }

            return result;
        }

        public List<FuncAssignation> GetFuncsByUser(string userId)
        {
            var query = from o in dataContext.P_User2Functions
                        where o.UserId == userId
                        select new FuncAssignation()
                        {
                            FuncCode = o.FuncCode,
                            AccessLevel = (AccessLevel?)o.AccessLevel
                        };
            return query.ToList();
        }

        public void RemoveFunc(string funcCode)
        {
            var query = from o in dataContext.P_Functions
                        where o.FuncCode == funcCode
                        select o;
            dataContext.P_Functions.RemoveRange(query);
            dataContext.SaveChanges();
        }

        public void RemoveFuncFromUser(string userId, string funcCode)
        {
            var query = from o in dataContext.P_User2Functions
                        where o.FuncCode == funcCode && o.UserId == userId
                        select o;
            dataContext.P_User2Functions.RemoveRange(query);
            dataContext.SaveChanges();
        }

        private string DictionaryToXml(Dictionary<string, string> dict)
        {
            XElement el = new XElement("root",
                dict.Select(kv => new XElement(kv.Key, kv.Value)));
            return el.ToString();
        }

        private Dictionary<string, string> XmlToDictionary(string xmlStr)
        {
            XElement rootElement = XElement.Parse(xmlStr);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var el in rootElement.Elements())
            {
                dict.Add(el.Name.LocalName, el.Value);
            }

            return dict;
        }
    }
}
