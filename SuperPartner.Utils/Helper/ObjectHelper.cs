using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace SuperPartner.Utils.Helper
{
    public static class ObjectHelper
    {
        /// <summary>
        /// Copy same name's property
        /// </summary>
        /// <param name="sourceObj">Source object</param>
        /// <param name="targetObj">Target object</param>
        public static void CopyPropertyValue(object sourceObj, object targetObj)
        {
            var sourceProperties = sourceObj.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            var targetProperties = targetObj.GetType().GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
            foreach (var sourceProperty in sourceProperties)
            {
                var propertyName = sourceProperty.Name;
                var targetProperty = targetProperties.Where(o => o.Name == propertyName).FirstOrDefault();
                if (targetProperty != null)
                {
                    targetProperty.SetValue(targetObj, sourceProperty.GetValue(sourceObj));
                }
            }
        }
    }
}
