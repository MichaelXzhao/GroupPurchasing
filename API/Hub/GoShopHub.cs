using Microsoft.AspNetCore.SignalR;
using SignalR.Hub;
using System.Threading.Tasks;

public class GoShopHub : Hub<IMessageHubClient>
{
    public async Task SendOrderData(object orderData)
    {
        await Clients.All.SendOrderData(orderData);
    }
}
