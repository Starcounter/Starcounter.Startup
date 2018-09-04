using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Starcounter.Startup.Routing
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns a method from specified type, marked with specified attribute.
        /// If this method is not public and static or its return type or parameter type doesn't match the requirement
        /// or if it doesn't accept the required parameter, this method will throw an exception.
        /// </summary>
        /// <param name="inspectedType">The type in which the method is sought</param>
        /// <param name="attribute">The attribute with which the sought method should be marked</param>
        /// <param name="expectedParameterType">The required parameter type for the method.
        ///  The seeked method can accept more parameters, but if it doesn't accept any or if the first one has type
        ///  different than <paramref name="expectedParameterType"/> then this method will throw exception</param>
        /// <param name="expectedReturnType">The required return type for the method</param>
        /// <returns>The found method handle or null if no method with specified attribute exists in specified type</returns>
        public static MethodInfo GetStaticMethodWithAttribute(
            Type inspectedType,
            Type attribute,
            Type expectedParameterType,
            Type expectedReturnType)
        {
            if (inspectedType == null) throw new ArgumentNullException(nameof(inspectedType));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));
            if (expectedParameterType == null) throw new ArgumentNullException(nameof(expectedParameterType));
            if (expectedReturnType == null) throw new ArgumentNullException(nameof(expectedReturnType));
            // we're looking not only for public static methods to inform the user if they used an attribute incorrectly
            // there are cases when they applied attribute, but forgot to make the method public static and they rather
            // be informed of their mistake with an exception
            var methodsWithAttribute = inspectedType
                .GetMethods(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Static |
                            BindingFlags.Instance)
                .Where(method => method.GetCustomAttribute(attribute) != null)
                .ToList();
            if (methodsWithAttribute.Count > 1)
            {
                throw BuildException(attribute, inspectedType,
                    StringsFormatted.ReflectionHelper_MultipleMethods(attribute, methodsWithAttribute));
            }
            if (methodsWithAttribute.Count == 0)
            {
                return null;
            }

            var methodWithAttribute = methodsWithAttribute.First();
            if (!methodWithAttribute.IsStatic)
            {
                throw BuildException(attribute, inspectedType,
                    StringsFormatted.ReflectionHelper_MethodNotStatic(attribute, methodWithAttribute));
            }
            if (!methodWithAttribute.IsPublic)
            {
                throw BuildException(attribute, inspectedType,
                    StringsFormatted.ReflectionHelper_MethodNotPublic(attribute, methodWithAttribute));
            }
            if (!expectedReturnType.IsAssignableFrom(methodWithAttribute.ReturnType))
            {
                throw BuildException(attribute, inspectedType,
                    StringsFormatted.ReflectionHelper_WrongReturnType(attribute, expectedReturnType, methodWithAttribute));
            }
            if (methodWithAttribute.GetParameters()
                .Select(p => p.ParameterType)
                .FirstOrDefault() != expectedParameterType )
            {
                throw BuildException(attribute, inspectedType,
                    StringsFormatted.ReflectionHelper_MethodShouldAcceptOneParameter(attribute, expectedParameterType, methodWithAttribute));
            }

            return methodWithAttribute;
        }

        /// <summary>
        /// Returns a value for parameter resolved from a service provider.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static object GetParamValue(ParameterInfo parameter, IServiceProvider serviceProvider)
        {
            try
            {
                return serviceProvider.GetRequiredService(parameter.ParameterType);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(StringsFormatted.ReflectionHelper_CouldNotInstantiateParameter(parameter), e);
            }
        }

        private static Exception BuildException(Type attribute, Type inspectedType, string details)
        {
            return new InvalidOperationException(StringsFormatted.ReflectionHelper_InvalidApplicationOfAttribute(attribute, inspectedType, details));
        }
    }
}