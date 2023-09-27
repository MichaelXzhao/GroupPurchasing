using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class GoShopHub : Hub
{
    public async Task SendOrderData(object orderData)
    {
        await Clients.All.SendAsync("ReceiveOrderData", orderData);
    }
}
