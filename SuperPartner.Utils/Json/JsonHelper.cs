using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SuperPartner.Utils.Json
{
    public static class JsonHelper
    {
        /// <summary>
        /// Author: Robert
        /// Covert object to json string
        /// </summary>
        /// <param name="obj">object which need convert to json string</param>
        /// <param name="camelCase">Does need use camel case when conversion</param>
        /// <returns>Json string</returns>
        public static string ToJson(object obj, bool camelCase = false)
        {
            if (camelCase)
            {
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
                setting.NullValueHandling = NullValueHandling.Ignore;
                setting.Converters = new List<JsonConverter> { new JsonMinDecimalConverter() };
                return JsonConvert.SerializeObject(obj, setting);
            }
            else
            {
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.NullValueHandling = NullValueHandling.Ignore;
                setting.Converters = new List<JsonConverter> { new JsonMinDecimalConverter() };
                return JsonConvert.SerializeObject(obj, setting);
            }
        }

        /// <summary>
        /// Convert json string to Object
        /// </summary>
        /// <typeparam name="T">object Type</typeparam>
        /// <param name="jsonStr">jsonString</param>
        /// <param name="camelCase">Does need use camel case when conversion</param>
        /// <returns>return converted object</returns>
        public static T ToObj<T>(string jsonStr, bool camelCase = false)
        {
            if (camelCase)
            {
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
                setting.NullValueHandling = NullValueHandling.Ignore;

                return JsonConvert.DeserializeObject<T>(jsonStr, setting);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(jsonStr);
            }
        }

        /// <summary>
        /// Clone object use convert json method
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="data">clone object</param>
        /// <returns>return cloned object</returns>
        public static T Clone<T>(T data)
        {
            var jsonStr = ToJson(data);
            return ToObj<T>(jsonStr);
        }
    }
}
