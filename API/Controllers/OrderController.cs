using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;


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
        public async Task<ActionResult> GetCartDetail(string salepageid)
        {
            var existingsalepageid = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.salepageid.Equals(salepageid));
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
                string nineyi_salepageid = jsonResponse["Data"]["Id"].ToString();
                string nineyi_shopid = jsonResponse["Data"]["ShopId"].ToString();
                string nineyi_title = jsonResponse["Data"]["Title"].ToString();
                string priceStr = jsonResponse["Data"]["Price"].ToString();
                int nineyi_price = Convert.ToInt32(priceStr);
                string nineyi_picurl = jsonResponse["Data"]["ImageList"][0]["PicUrl"].ToString();
                string nineyi_sku = jsonResponse["Data"]["SaleProductSKUIdList"][0].ToString();

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
                .Where(o => o.salepageid.Equals(salepageid))
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

            Console.WriteLine("order:"+orders[0]);

            // 加總所有的商品數
            var totalQty = memberData.Sum(member => member.qty);
            string totalQtyString = totalQty.ToString();

            // (2) [POST] Calculate 根據每次加總件數，再去打折扣API
                HttpClient Calculateclient = new HttpClient();
                HttpRequestMessage Calculaterequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/webapi//PromotionEngine/Calculate"),
                };

                // payload格式要注意，反斜線也要顯示
                string CalculaterequestBody = "{\"promotionDetailDiscount\":\"{\\\"ShopId\\\":10230,\\\"PromotionId\\\":\\\"21302\\\",\\\"SalePageList\\\":[{\\\"Price\\\":"+orders[0].price.ToString() +",\\\"Qty\\\":"+ totalQtyString +",\\\"SalePageId\\\":"+ orders[0].salepageid +",\\\"SaleProductSKUId\\\":"+ orders[0].skuid +",\\\"TagIds\\\":[\\\"20769\\\",\\\"22311\\\"]}]}\",\"source\":\"Web\"}";
                Calculaterequest.Content = new StringContent(CalculaterequestBody, Encoding.UTF8, "application/json");
                HttpResponseMessage Calculateresponse = await Calculateclient.SendAsync(Calculaterequest);
                string CalculateresponseContent = await Calculateresponse.Content.ReadAsStringAsync();
                JObject CalculatejsonResponse = JObject.Parse(CalculateresponseContent);
                //Console.WriteLine("Response JSON: {0}", jsonResponse);

                string TotalQty = CalculatejsonResponse["Data"]["TotalQty"].ToString();
                string TotalPrice = CalculatejsonResponse["Data"]["TotalPrice"].ToString();
                string TotalOriginalPrice = CalculatejsonResponse["Data"]["TotalOriginalPrice"].ToString();
                string PromotionDiscount = CalculatejsonResponse["Data"]["PromotionDiscount"].ToString();
                string PromotionConditionTitle = CalculatejsonResponse["Data"]["PromotionConditionTitle"].ToString();
                string PromotionDiscountTitle = CalculatejsonResponse["Data"]["PromotionDiscountTitle"].ToString();
                string RecommendConditionTitle = CalculatejsonResponse["Data"]["RecommendConditionTitle"].ToString();          

                //回傳所有折扣API相關資料
                var discountData = new
                {
                    TotalQty,
                    TotalPrice,
                    TotalOriginalPrice,
                    PromotionDiscount,
                    PromotionConditionTitle,
                    PromotionDiscountTitle,
                    RecommendConditionTitle
                };


            return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, salepageid =orders[0].salepageid, shopid=orders[0].shopid, skuid=orders[0].skuid, memberData, discountData }); 
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

                // 加總所有的商品數
                var totalQty = memberData.Sum(member => member.qty);
                string totalQtyString = totalQty.ToString();
                //Console.WriteLine("total:"+totalQty);


                // [POST] Calculate 根據每次加總件數，再去打折扣API
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/webapi//PromotionEngine/Calculate"),
                };

                // payload格式要注意，反斜線也要顯示
                string requestBody = "{\"promotionDetailDiscount\":\"{\\\"ShopId\\\":10230,\\\"PromotionId\\\":\\\"21302\\\",\\\"SalePageList\\\":[{\\\"Price\\\":"+input.price.ToString() +",\\\"Qty\\\":"+ totalQtyString +",\\\"SalePageId\\\":"+ input.salepageid +",\\\"SaleProductSKUId\\\":"+ input.skuid +",\\\"TagIds\\\":[\\\"20769\\\",\\\"22311\\\"]}]}\",\"source\":\"Web\"}";
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseContent);
                //Console.WriteLine("Response JSON: {0}", jsonResponse);

                string TotalQty = jsonResponse["Data"]["TotalQty"].ToString();
                string TotalPrice = jsonResponse["Data"]["TotalPrice"].ToString();
                string TotalOriginalPrice = jsonResponse["Data"]["TotalOriginalPrice"].ToString();
                string PromotionDiscount = jsonResponse["Data"]["PromotionDiscount"].ToString();
                string PromotionConditionTitle = jsonResponse["Data"]["PromotionConditionTitle"].ToString();
                string PromotionDiscountTitle = jsonResponse["Data"]["PromotionDiscountTitle"].ToString();
                string RecommendConditionTitle = jsonResponse["Data"]["RecommendConditionTitle"].ToString();          

                //回傳所有折扣API相關資料
                var discountData = new
                {
                    TotalQty,
                    TotalPrice,
                    TotalOriginalPrice,
                    PromotionDiscount,
                    PromotionConditionTitle,
                    PromotionDiscountTitle,
                    RecommendConditionTitle
                };

                return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData, discountData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}