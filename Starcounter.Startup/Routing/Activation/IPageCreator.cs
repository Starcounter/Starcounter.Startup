namespace Starcounter.Startup.Routing.Activation
{
    public interface IPageCreator
    {
        Response Create(RoutingInfo routingInfo);
    }
}