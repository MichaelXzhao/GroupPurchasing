namespace API.Services.Order
{
    public interface IOrderService
    {
        Task<string> GetCartDetail();
    }
}
