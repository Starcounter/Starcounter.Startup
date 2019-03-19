using System;

namespace Starcounter.Startup.Routing.Middleware
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InteractionScopeAttribute : Attribute
    {
        public enum InteractionScopeMode
        {
            AttachOrCreate = 0,
            AlwaysCreate = 0x1,
            None = 0x2
        }

        public InteractionScopeMode Value { get; set; }

        public InteractionScopeAttribute(InteractionScopeMode value = InteractionScopeMode.AttachOrCreate)
        {
            Value = value;
        }
    }
}