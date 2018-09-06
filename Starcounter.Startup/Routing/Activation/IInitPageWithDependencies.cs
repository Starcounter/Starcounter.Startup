using System;

namespace Starcounter.Startup.Routing.Activation
{
    /// <summary>
    /// Implement this interface in a view-model and create a public void method called Init and accepting
    /// a set of dependencies. This method will be called with dependencies filled after this view-model is
    /// constructed.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public partial class UsersViewModel: Json, IInitPageWithDependencies
    /// {
    ///   private ILogger<UsersViewModel> _logger;
    ///   private IUserRepository _userRepository;
    /// 
    ///   public IEnumerable AllUsers {get; private set;}
    ///   public void Init(ILogger<UsersViewModel> logger, IUserRepository userRepository)
    ///   {
    ///     _logger = logger;
    ///     _userRepository = userRepository;
    /// 
    ///     AllUsers = _userRepository.FindAll();
    ///   }
    /// }
    ///  ]]></code></example>
    [Obsolete("Declare a constructor instead")]
    public interface IInitPageWithDependencies
    {
    }
}