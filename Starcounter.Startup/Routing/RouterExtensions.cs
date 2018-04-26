using System;
using System.Linq;
using System.Reflection;

namespace Starcounter.Startup.Routing
{
    public static class RouterExtensions
    {
        public static void RegisterAllFromAssembly(this Router router, Assembly assembly)
        {
            foreach (var typeAndUrl in assembly.GetTypes()
                .SelectMany(type => CustomAttributeExtensions.GetCustomAttributes<UrlAttribute>((MemberInfo) type), Tuple.Create))
            {
                router.HandleGet(typeAndUrl.Item2.Value, typeAndUrl.Item1);
            }
        }

        public static void RegisterAllFromCurrentAssembly(this Router router)
        {
            RegisterAllFromAssembly(router, Assembly.GetCallingAssembly());
        }
    }
}