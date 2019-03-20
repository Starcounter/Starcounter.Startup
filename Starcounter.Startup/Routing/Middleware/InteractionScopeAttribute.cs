using System;
using Starcounter.Startup.Routing.Enums;

namespace Starcounter.Startup.Routing.Middleware
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InteractionScopeAttribute : Attribute
    {
        public InteractionScopeMode Value { get; set; }

        public InteractionScopeAttribute(InteractionScopeMode value = InteractionScopeMode.AttachOrCreate)
        {
            Value = value;
        }
    }
}