using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text;


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
        public async Task<ActionResult> GetCartDetail(int salepageid)
        {
            var existingsalepageid = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.salepageid == salepageid);
            if(existingsalepageid==null){
                // (1)product/detail
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://10230.shop.qa.91dev.tw/webapi/SalePageV2/GetSalePageV2Info/10230/{salepageid}/1"),
                };
                string requestBody = "{\r\n    \"source\": \"Web\"\r\n}"; // 替換為request正文
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                // 發送request並等待
                HttpResponseMessage response = await client.SendAsync(request);

                // 讀取回傳response
                string responseContent = await response.Content.ReadAsStringAsync();

                // 解析JSON response 為 JObject
                JObject jsonResponse = JObject.Parse(responseContent);
            
                // 抓取商品資訊
                string salepageidStr = jsonResponse["Data"]["Id"].ToString();
                int nineyi_salepageid = Convert.ToInt32(salepageidStr);
                string shopidStr = jsonResponse["Data"]["ShopId"].ToString();
                int nineyi_shopid = Convert.ToInt32(shopidStr);
                string nineyi_title = jsonResponse["Data"]["Title"].ToString();
                string priceStr = jsonResponse["Data"]["Price"].ToString();
                int nineyi_price = Convert.ToInt32(priceStr);
                string nineyi_picurl = jsonResponse["Data"]["ImageList"][0]["PicUrl"].ToString();
                string skuStr = jsonResponse["Data"]["SaleProductSKUIdList"][0].ToString();
                int nineyi_sku = Convert.ToInt32(skuStr);

                //儲存到DB
                var newOrders= new Orders{
                    member="host",
                    qty=1,
                    price=nineyi_price,
                    product=nineyi_title,
                    picture=nineyi_picurl,
                    salepageid=nineyi_salepageid,
                    shopid=nineyi_shopid,
                    skuid=nineyi_sku
                };
                _dbcontext.Orders.Add(newOrders);
                await _dbcontext.SaveChangesAsync();
               
            }

            
            var orders = await _dbcontext.Orders
                .Where(o => o.salepageid == salepageid)
                .ToListAsync(); // 獲取所有符合的訂單

            if (orders == null || orders.Count == 0)
            {
                return NotFound(new { message = "找不到符合條件的訂單" });
            }

            var memberData = orders.Select(order => new
            {
                member = order.member,
                qty = order.qty,
            }).ToList(); 

            return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData }); 
        }



        [HttpPost("add")]
        public async Task<ActionResult> addorder([FromBody]AddOrderDto input)
        {
            try{
                //如果下訂者已在同一個商品下過單，則直接在同一成員的數量做變更
                var existingmember = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.member == input.user_name && o.product == input.product);
                if(existingmember!=null)
                {
                    existingmember.qty+=input.product_qty;
                }
                else
                {
                    //如果為新的成員，則直接新增一筆
                    var newOrders= new Orders{
                        member=input.user_name,
                        qty=input.product_qty,
                        price=input.price,
                        product=input.product,
                        picture=input.picture,
                        salepageid=input.salepageid,
                        shopid=input.shopid,
                        skuid=input.skuid,                       
                    };
                    _dbcontext.Orders.Add(newOrders);
                }     
                
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}