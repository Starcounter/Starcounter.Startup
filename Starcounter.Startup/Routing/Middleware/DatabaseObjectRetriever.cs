using System;

namespace Starcounter.Startup.Routing.Middleware
{
    public class DatabaseObjectRetriever : IObjectRetriever
    {
        public object GetById(Type desiredType, string id)
        {
            ulong objectId;
            try
            {
                objectId = DbHelper.Base64DecodeObjectID(id);
            }
            catch (ArgumentException)
            {
                return null; // in case a "random" string is supplied, it's leniently converted to int and FromID returns null
            }
            var obj = Db.FromId(objectId);
            return desiredType.IsInstanceOfType(obj) ? obj : null;
        }
    }
}