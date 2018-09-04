using System;
using System.Collections.Generic;
using System.Reflection;
using Starcounter.Startup.Routing;
using Starcounter.Startup.Routing.Activation;

namespace Starcounter.Startup
{
    public static class StringsFormatted
    {
        public static string DefaultPageCreator_CouldNotCreatePage(Type type) =>
            String.Format(Strings.DefaultPageCreator_CouldNotCreatePage, type);

        public static string ReflectionHelper_CouldNotInstantiateParameter(ParameterInfo parameterInfo) =>
            String.Format(Strings.DefaultPageCreator_CouldNotInstantiateParameter, parameterInfo);

        public static string DefaultPageCreator_TypeImplementsInitWithDependenciesBadly() =>
#pragma warning disable 618
            String.Format(Strings.DefaultPageCreator_TypeImplementsInitWithDependenciesBadly, nameof(IInitPageWithDependencies));
#pragma warning restore 618

        public static string ReflectionHelper_MultipleMethods(Type attribute, IEnumerable<MethodInfo> methodsWithAttribute) =>
            String.Format(Strings.ReflectionHelper_MultipleMethods,
                attribute.Name,
                String.Join(", ", methodsWithAttribute));

        public static string ReflectionHelper_InvalidApplicationOfAttribute(Type attribute, Type inspectedType, string details) =>
            string.Format(Strings.ReflectionHelper_InvalidApplicationOfAttribute, attribute, inspectedType, details);

        public static string ReflectionHelper_MethodShouldAcceptOneParameter(Type attribute, Type expectedParameterType, MethodInfo methodWithAttribute) =>
            string.Format(
                Strings.ReflectionHelper_MethodShouldAcceptOneParameter,
                attribute.Name, expectedParameterType.Name, methodWithAttribute);

        public static string ReflectionHelper_WrongReturnType(Type attribute, Type expectedReturnType, MethodInfo methodWithAttribute) =>
            string.Format(Strings.ReflectionHelper_WrongReturnType,
                attribute.Name, expectedReturnType.Name, methodWithAttribute);

        public static string ReflectionHelper_MethodNotStatic(Type attribute, MethodInfo methodWithAttribute) =>
            string.Format(Strings.ReflectionHelper_MethodNotStatic,
                attribute.Name, methodWithAttribute);

        public static string ReflectionHelper_MethodNotPublic(Type attribute, MethodInfo methodWithAttribute) =>
            string.Format(Strings.ReflectionHelper_MethodNotPublic,
                attribute.Name, methodWithAttribute);

        public static string ContextMiddleware_CouldNotCreateContext(Type contextType, Type viewModelType, string details) =>
            string.Format(Strings.ContextMiddleware_CouldNotCreateContext, contextType, viewModelType, details);

        public static string ContextMiddleware_ContextIsDbSoUriShouldHaveOneArgument(Type contextType) =>
            string.Format(Strings.ContextMiddleware_ContextIsDbSoUriShouldHaveOneArgument, contextType, nameof(UriToContextAttribute));

        public static string ContextMiddleware_MarkViewModelAsIBoundOrUriToContext(Type contextType) =>
            string.Format(Strings.ContextMiddleware_MarkViewModelAsIBoundOrUriToContext, contextType, nameof(UriToContextAttribute));

        public static string ContextMiddleware_CouldNotResolveUriToContextDependencies(MethodInfo uriToContext) =>
            string.Format(Strings.ContextMiddleware_CouldNotResolveUriToContextDependencies, uriToContext);
    }
}