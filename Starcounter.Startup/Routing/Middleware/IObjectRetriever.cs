using System;

namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Fetches database object by type and ID
    /// </summary>
    public interface IObjectRetriever
    {
        /// <summary>
        /// Returns a database object of given type with given ID, or null.
        /// </summary>
        /// <param name="desiredType">The type of object to return. If the database object with <paramref name="id"/>
        ///  has different type, null will be returned</param>
        /// <param name="id">ID of the object to fetch. If this argument is malformed, null will be returned</param>
        /// <returns>A database object with given <paramref name="id"/> and of type <paramref name="desiredType"/>, or null if no such object was found</returns>
        object GetById(Type desiredType, string id);
    }
}