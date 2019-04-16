using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Angelo.Common.Extensions
{
    public static class TypeExtentions
    {
        public static TObject Clone<TObject>(this TObject item) where TObject : class
        {
            if (item == null)
                throw new ArgumentNullException("Cannot clone a null object");

            var json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return JsonConvert.DeserializeObject<TObject>(json);
        }

        public static ICollection<Type> GetBaseTypes<TType>()
        {
            return GetBaseTypes(typeof(TType));
        }

        public static ICollection<Type> GetBaseTypes(Type type)
        {
            var results = new List<Type>();
            while (type != typeof(object))
            {
                results.Add(type);
                type = type.GetTypeInfo().BaseType;
            }
            return results;
        }

        public static ICollection<Type> GetDerivedInterfaces(Type runtimeType, Type baseInterfaceType)
        {
            return runtimeType.GetInterfaces().Where(
                type => type.GetInterfaces().Contains(baseInterfaceType)
            ).ToList();
        }

        public static bool IsConstructedFrom(this Type constructedType, Type genericType)
        {
            return constructedType.IsConstructedGenericType
                && constructedType.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsDerivedFrom(this Type derivedType, Type baseType)
        {
            var baseTypes = GetBaseTypes(derivedType);

            return baseTypes.Any(x => x == baseType || x.IsConstructedFrom(baseType)) ||
                derivedType.GetInterfaces().Any(y => y == baseType || y.IsConstructedFrom(baseType));
        }
    }
}
