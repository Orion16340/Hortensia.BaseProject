using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Hortensia.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type GetActionType(this MethodInfo method) => Expression.GetActionType(method.GetParameters().Select(entry => entry.ParameterType).ToArray());

        public static bool HasInterface(this Type type, Type interfaceType) => type.FindInterfaces(FilterByName, interfaceType).Length > 0;

        private static bool FilterByName(Type typeObj, Object criteriaObj) => typeObj.ToString() == criteriaObj.ToString();

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type) where T : Attribute => type.GetCustomAttributes(typeof(T), false) as T[];

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type) where T : Attribute => type.GetCustomAttributes<T>().GetOrDefault(0);

        public static T CreateDelegate<T>(this ConstructorInfo ctor)
        {
            List<ParameterExpression> list =
                Enumerable.ToList(Enumerable.Select(ctor.GetParameters(),
                param => Expression.Parameter(param.ParameterType, string.Empty)));

            var list2 = list.ConvertAll(x => (Expression)x);
            return Expression.Lambda<T>(Expression.New(ctor, list2), list).Compile();
        }

        public static bool IsDerivedFromGenericType(this Type type, Type genericType)
        {
            if (type == typeof(object))
                return false;

            if (type == null)
                return false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                return true;

            return IsDerivedFromGenericType(type.BaseType, genericType);
        }

        public static MethodInfo GetMethodExt(this Type thisType, string name, int genericArgumentsCount, params Type[] parameterTypes)
             => GetMethodExt(thisType, name, genericArgumentsCount, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, parameterTypes);

        public static MethodInfo GetMethodExt(this Type thisType, string name, int genericArgumentsCount, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            GetMethodExt(ref matchingMethod, thisType, name, genericArgumentsCount, bindingFlags, parameterTypes);

            if (matchingMethod == null && thisType.IsInterface)
                foreach (Type interfaceType in thisType.GetInterfaces())
                    GetMethodExt(ref matchingMethod, interfaceType, name, genericArgumentsCount, bindingFlags, parameterTypes);

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod, Type type, string name, int genericArgumentsCount, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            foreach (MethodInfo methodInfo in type.GetMember(name, MemberTypes.Method, bindingFlags))
            {
                if (methodInfo.GetGenericArguments().Length != genericArgumentsCount)
                    continue;

                ParameterInfo[] parameterInfos = methodInfo.GetParameters();

                if (parameterInfos.Length == parameterTypes.Length)
                {
                    int i = 0;

                    for (; i < parameterInfos.Length; ++i)
                        if (!parameterInfos[i].ParameterType.IsSimilarType(parameterTypes[i]))
                            break;

                    if (i == parameterInfos.Length)
                        if (matchingMethod == null)
                            matchingMethod = methodInfo;
                        else
                            throw new AmbiguousMatchException("More than one matching method found!");
                }
            }
        }

        public static Delegate CreateDelegate(this MethodInfo method, params Type[] delegParams)
        {
            var methodParams = method.GetParameters().Select(p => p.ParameterType).ToArray();

            if (delegParams.Length != methodParams.Length)
                throw new Exception("Method parameters count != delegParams.Length");

            var dynamicMethod = new DynamicMethod(string.Empty, null, new[] { typeof(object) }.Concat(delegParams).ToArray(), true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            if (!method.IsStatic)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(method.DeclaringType.IsClass ? OpCodes.Castclass : OpCodes.Unbox, method.DeclaringType);
            }

            for (var i = 0; i < delegParams.Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);

                if (delegParams[i] == methodParams[i])
                    continue;

                if (methodParams[i].IsSubclassOf(delegParams[i]) || methodParams[i].HasInterface(delegParams[i]))
                    ilGenerator.Emit(methodParams[i].IsClass ? OpCodes.Castclass : OpCodes.Unbox, methodParams[i]);

                else
                    throw new Exception($"Cannot cast {methodParams[i].Name} to {delegParams[i].Name}");
            }

            ilGenerator.Emit(OpCodes.Call, method);
            ilGenerator.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(Expression.GetActionType(new[] { typeof(object) }.Concat(delegParams).ToArray()));

        }

        public class T
        {
        }

        private static bool IsSimilarType(this Type thisType, Type type)
        {
            if (thisType.IsByRef)
                thisType = thisType.GetElementType();

            if (type.IsByRef)
                type = type.GetElementType();

            if (thisType.IsArray && type.IsArray)
                return thisType.GetElementType().IsSimilarType(type.GetElementType());

            if (thisType == type || ((thisType.IsGenericParameter || thisType == typeof(T)) && (type.IsGenericParameter || type == typeof(T))))
                return true;

            if (thisType.IsGenericType && type.IsGenericType)
            {
                Type[] thisArguments = thisType.GetGenericArguments();
                Type[] arguments = type.GetGenericArguments();

                if (thisArguments.Length == arguments.Length)
                {
                    for (int i = 0; i < thisArguments.Length; ++i)
                        if (!thisArguments[i].IsSimilarType(arguments[i]))
                            return false;

                    return true;
                }
            }

            return false;
        }
    }
}