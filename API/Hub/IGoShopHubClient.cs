namespace SignalR.Hub
{
    public interface IMessageHubClient
    {
        Task SendOrderData(object orderData);
    }
}
