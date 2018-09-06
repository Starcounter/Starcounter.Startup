using System;
using System.Linq;
using System.Reflection;
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
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Response Create(RoutingInfo routingInfo)
        {
            try
            {
                var page = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, routingInfo.SelectedPageType);
#pragma warning disable 618
                if (page is IInitPageWithDependencies pageWithDependencies)
#pragma warning restore 618
                {
                    var init = routingInfo.SelectedPageType.GetMethod("Init", BindingFlags.Instance|BindingFlags.Public);
                    if (init == null)
                    {
                        throw new InvalidOperationException(StringsFormatted.DefaultPageCreator_TypeImplementsInitWithDependenciesBadly());
                    }
                    var arguments = init.GetParameters().Select(parameter => ReflectionHelper.GetParamValue(parameter, _serviceProvider)).ToArray();
                    init.Invoke(pageWithDependencies, arguments);
                }
#pragma warning disable 618
                if (page is IInitPage initPage)
#pragma warning restore 618
                {
                    initPage.Init();
                }
                PageContextSupport.HandleContext(page, routingInfo.Context);
                return new Response() { Resource = (IResource)page };
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(StringsFormatted.DefaultPageCreator_CouldNotCreatePage(routingInfo.SelectedPageType), e);
            }
        }
    }
}