using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using API.Models;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly GoShopContext _dbcontext; 

        public OrderController(GoShopContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet("Detail")]
        public async Task<ActionResult> GetCartDetail(string product_name)
        {
            var orders = await _dbcontext.Orders
                .Where(o => o.product == product_name)
                .ToListAsync(); // 獲取所有符合的訂單

            if (orders == null || orders.Count == 0)
            {
                return NotFound(); 
            }

            var memberData = orders.Select(order => new
            {
                member = order.member,
                qty = order.qty,
            }).ToList(); 

            return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData });
        }



        [HttpPost("add")]
        public async Task<ActionResult> addorder(AddOrderDto input)
        {
            var newOrders= new Orders{
                member=input.user_name,
                qty=input.product_qty,
                price=input.price,
                product=input.product,
                picture=$"./img/{input.product}.jpg"
            };
            
            _dbcontext.Orders.Add(newOrders);
            await _dbcontext.SaveChangesAsync();


            //回傳所有產品相關的資料
            var orders = await _dbcontext.Orders
                .Where(o => o.product == input.product)
                .ToListAsync(); // 獲取所有符合的訂單

            if (orders == null || orders.Count == 0)
            {
                return NotFound(); 
            }

            var memberData = orders.Select(order => new
            {
                member = order.member,
                qty = order.qty,
            }).ToList(); 

            return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData });
        }
    }
}