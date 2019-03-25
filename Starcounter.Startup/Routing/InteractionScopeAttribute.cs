using System;

namespace Starcounter.Startup.Routing
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