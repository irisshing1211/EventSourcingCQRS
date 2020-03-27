using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventSourcingCQRS
{
    public class LogParser
    {
        public static string ConvertToHistory(object obj) => JsonConvert.SerializeObject(obj);

        public static T ConvertStringToObject<T>(string history) => JsonConvert.DeserializeObject<T>(history);

        public static string GetUpdatedField<T>(T oldObj, T newObj)
        {
            var props = typeof(T).GetProperties();
            var fieldList = new Dictionary<string, string>();

            foreach (var prop in props)
            {
                var old = typeof(T).GetProperty(prop.Name).GetValue(oldObj);
                var newVal = typeof(T).GetProperty(prop.Name).GetValue(newObj);

                if (!old.Equals(newVal))
                    fieldList.Add(prop.Name, newVal.ToString());
            }

            return JsonConvert.SerializeObject(fieldList);
        }

        public static T UpdateObjectByData<T>(T old, string stored)
        {
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(stored);

            foreach (var field in data)
            {
                var prop = typeof(T).GetProperty(field.Key);
                Type propertyType = prop.PropertyType;

                // Get the type code so we can switch
                TypeCode typeCode = System.Type.GetTypeCode(propertyType);

                switch (typeCode)
                {
                    case TypeCode.Int32:
                        prop.SetValue(old, Convert.ToInt32(field.Value), null);

                        break;
                    case TypeCode.Int64:
                        prop.SetValue(old, Convert.ToInt64(field.Value), null);

                        break;
                    case TypeCode.String:
                        prop.SetValue(old, field.Value, null);

                        break;
                   
                    default:
                        if (Guid.TryParse(field.Value, out var guid))
                            prop.SetValue(old, guid, null);
                        else
                            prop.SetValue(old, field.Value, null);

                        break;
                }
            }

            return old;
        }
    }
}
