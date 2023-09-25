using API.Services.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public TestsController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        [Route("/test")]
        public async Task<IActionResult> Test()
        {
            string cartUniqueKey = await _orderService.GetCartDetail();
            return Ok(cartUniqueKey);
        }
    }
}
