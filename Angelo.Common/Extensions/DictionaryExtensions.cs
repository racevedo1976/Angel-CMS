using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Angelo.Common.Extensions
{
    public static class DictionaryExtensions
    {
        
        public static IDictionary<string, object> ToDictionary(this object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var fields = new Dictionary<string, object>();
            var type = instance.GetType();
            //type.GetFields(bindingAttr: BindingFlags.Public)
            foreach (var field in type.GetProperties())
            {
                fields.Add(field.Name, field.GetValue(instance));
            }

            return fields;
        }
        
        public static TResult ConvertTo<TResult>(this IDictionary dictionary) where TResult : class
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            var instance = Activator.CreateInstance<TResult>();
            //var fields = typeof(TResult).GetFields();
            var fields = typeof(TResult).GetProperties();

            var itor = dictionary.GetEnumerator();
            while (itor.MoveNext())
            {
                var field = fields.FirstOrDefault(x => x.Name == (string)itor.Key);
                if (field != null)
                {
                    try
                    {
                        var value = Convert.ChangeType(itor.Value, field.PropertyType);
                        field.SetValue(instance, value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Type conversion failed for {field.Name} for value {itor.Value}", ex);
                    }

                }
            }

            return instance;
        }

        public static  IEnumerable<TResult> ConvertTo<TResult>(this IDictionary<string, object> dictionary, Func<KeyValuePair<string, object>, TResult> mapper) where TResult : class
        {
            var results = new List<TResult>();

            foreach(var item in dictionary)
            {
                var instance = mapper.Invoke(item);
                results.Add(instance);
            }

            return results;
        }

        public static void ForEach(this IDictionary<string, object> dictionary, Action<KeyValuePair<string, object>> action)
        {
            foreach (var item in dictionary)
            {
                action.Invoke(item);   
            }
        }
        
    }
}
