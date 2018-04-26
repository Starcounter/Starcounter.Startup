using System;

namespace Starcounter.Startup.Routing
{
    /// <summary>
    /// Use this attribute to mark a method that will obtain context for the page using URI arguments.
    /// </summary>
    /// This method must be public, static, accept an array of strings and return whatever type is used to represent a context.
    /// If the page implements <see cref="IPageContext{T}"/> or <see cref="Starcounter.IBound{DataType}"/> then it should return T.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class UriToContextAttribute : Attribute
    {
    }
}