using System;
using Starcounter.XSON;

namespace Starcounter.Startup.Routing.Middleware
{
    /// <summary>
    /// Base classes for master page, used to coordinate blending in a common transaction (aka scope).
    /// Override this class and use <see cref="RouterServiceCollectionExtensions.SetMasterPage{T}"/> to add
    /// features like navigation overlays for blended pages.
    /// </summary>
    public abstract class MasterPageBase : Json
    {
        /// <summary>
        /// Override this method to control the Db.Scope in which
        /// the view-models will be created.
        /// </summary>
        /// <param name="innerJsonFactory">Creates the blended view-models using Self.GET. This should be executed either in <see cref="Db.Scope"/> or <see cref="IScopeContext.Scope"/></param>
        public virtual Json ExecuteInScope(Func<Json> innerJsonFactory)
        {
            return Db.Scope(innerJsonFactory);
        }

        /// <summary>
        /// Override this method to assign the blended view-model to an exposed json property.
        /// </summary>
        /// <param name="blended">The blended view-model that should be exposed.</param>
        public abstract void SetBlended(Json blended);
    }
}