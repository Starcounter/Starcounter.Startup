using System;

namespace Starcounter.Startup.Routing.Middleware
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UseDbScopeAttribute : Attribute
    {
        public bool Value { get; set; }

        public UseDbScopeAttribute(bool value = true)
        {
            Value = value;
        }
    }
}