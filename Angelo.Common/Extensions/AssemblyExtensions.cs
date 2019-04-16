using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Angelo.Common.Extensions
{
    public static class AssemblyExtensions
    {

        /// <summary>
        /// Gets the first implementation of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of implementation to find</typeparam>
        public static Type GetImplementation<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.GetImplementation<T>(null);
        }

        /// <summary>
        /// Gets the first implementation of the specified type from a collection of assemblies filtered by a predicated
        /// </summary>
        /// <typeparam name="T">The type of implementation to find</typeparam>
        /// <param name="predicate">A predicate to filter the assemblies.</param>
        public static Type GetImplementation<T>(this IEnumerable<Assembly> assemblies, Func<Assembly, bool> predicate)
        {
            return assemblies.GetImplementations<T>(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Gets all implementations of the specified type from a collection of assemblies.
        /// </summary>
        /// <typeparam name="T">The type of implementation to find</typeparam>
        public static IEnumerable<Type> GetImplementations<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.GetImplementations<T>(null);
        }

        /// <summary>
        /// Gets all implementations of the specified type from a collection of assemblies filtered by a predicate
        /// </summary>
        /// <typeparam name="T">The type of implementation to find</typeparam>
        /// <param name="predicate">A predicate to filter the assemblies</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementations<T>(this IEnumerable<Assembly> assemblies, Func<Assembly, bool> predicate)
        {
            var implementations = new List<Type>();

            if (predicate != null)
                assemblies = assemblies.Where(predicate);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(type => 
                    typeof(T).GetTypeInfo().IsAssignableFrom(type) && type.GetTypeInfo().IsClass
                );
                implementations.AddRange(types);
            }
            return implementations;
        }


        /// <summary>
        /// Creates an instance of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        public static T GetInstance<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.GetInstance<T>(new object[] { });
        }

        /// <summary>
        /// Creates an instance of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="args">Constructor initialization arguments</param>
        public static T GetInstance<T>(this IEnumerable<Assembly> assemblies, object[] args)
        {
            var implementation = assemblies.GetImplementation<T>();

            if (!implementation.GetTypeInfo().IsAbstract)
                return (T)Activator.CreateInstance(implementation, args);

            return default(T);
        }

        /// <summary>
        /// Creates an instance of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="predicate">A predicate to filter the assemblies</param>
        public static T GetInstance<T>(this IEnumerable<Assembly> assemblies, Func<Assembly, bool> predicate)
        {
            return assemblies.GetInstance<T>(predicate, new object[] { });
        }

        /// <summary>
        /// Creates an instance of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="predicate">A predicate to filter the assemblies</param>
        /// <param name="args">Constructor initialization arguments</param>
        public static T GetInstance<T>(this IEnumerable<Assembly> assemblies, Func<Assembly, bool> predicate, object[] args)
        {
            var implementation = assemblies.GetImplementation<T>(predicate);

            if(!implementation.GetTypeInfo().IsAbstract)
                return (T)Activator.CreateInstance(implementation, args);

            return default(T);
        }

        /// <summary>
        /// Creates a typed collection of instances of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.GetImplementations<T>()?.ActivateAll<T>();
        }

        /// <summary>
        /// Creates a typed collection of instances of the specified type from a collection of assemblies
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="args">Constructor initialization arguments</param>
        public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies, params object[] args)
        {
            return assemblies.GetImplementations<T>()?.ActivateAll<T>(args);
        }

        /// <summary>
        /// Creates a typed collection of instances of the specified type from a collection of assemblies filtered by a predicate
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="predicate">A predicate to filter the assemblies</param>
        public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies, Func<Assembly, bool> predicate)
        {
            return assemblies.GetImplementations<T>(predicate)?.ActivateAll<T>();
        }

        /// <summary>
        /// Creates a typed collection of instances of the specified type from a collection of assemblies filtered by a predicate
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="args">Constructor initialization arguments</param>
        /// <param name="predicate">A predicate to filter the assemblies</param>
        public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies, Func<Assembly, bool> predicate, params object[] args)
        {
            return assemblies.GetImplementations<T>(predicate)?.ActivateAll<T>(args);
        }

        /// <summary>
        /// Creates a typed collection of instances from a collection of compatible types
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        public static IEnumerable<T> ActivateAll<T>(this IEnumerable<Type> implementations)
        {
            return implementations.ActivateAll<T>(new object[] { });
        }

        /// <summary>
        /// Creates a typed collection of instances from a collection of compatible types
        /// </summary>
        /// <typeparam name="T">The type of instance to create</typeparam>
        /// <param name="args">Constructor arguments</param>
        public static IEnumerable<T> ActivateAll<T>(this IEnumerable<Type> implementations, params object[] args)
        {
            List<T> instances = new List<T>();

            foreach (Type type in implementations)
            {
                if (!type.GetTypeInfo().IsAbstract)
                {
                    T instance = (T)Activator.CreateInstance(type, args);
                    instances.Add(instance);
                }
            }
            return instances;
        }
    }
}