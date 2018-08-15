using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Starcounter.Startup.Routing.Activation
{
    /// <summary>
    /// Creates a router that will create pages using default constructor and call <see cref="PageContextSupport.HandleContext"/>
    /// Works only with pages that are <see cref="Starcounter.IResource"/> - this category includes <see cref="Starcounter.Json"/>, so covers most cases
    /// </summary>
    public class DefaultPageCreator : IPageCreator
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultPageCreator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DefaultPageCreator()
        {
        }

        public Response Create(RoutingInfo routingInfo)
        {
            try
            {
                var page = Activator.CreateInstance(routingInfo.SelectedPageType);
                if (_serviceProvider != null && page is IInitPageWithDependencies pageWithDependencies)
                {
                    var init = routingInfo.SelectedPageType.GetMethod("Init", BindingFlags.Instance|BindingFlags.Public);
                    if (init == null)
                    {
                        throw new InvalidOperationException(string.Format(Strings.DefaultPageCreator_TypeImplementsInitWithDependenciesBadly, nameof(IInitPageWithDependencies)));
                    }
                    var arguments = init.GetParameters().Select(GetParamValue).ToArray();
                    init.Invoke(pageWithDependencies, arguments);
                }
                if (page is IInitPage initPage)
                {
                    initPage.Init();
                }
                PageContextSupport.HandleContext(page, routingInfo.Context);
                return new Response() { Resource = (IResource)page };
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(string.Format(Strings.DefaultPageCreator_CouldNotCreatePage, routingInfo.SelectedPageType), e);
            }
        }

        private object GetParamValue(ParameterInfo parameter)
        {
            try
            {
                return _serviceProvider.GetRequiredService(parameter.ParameterType);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(string.Format(Strings.DefaultPageCreator_CouldNotInstantiateParameter, parameter), e);
            }
        }
    }
}