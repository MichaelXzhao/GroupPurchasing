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

        // 獲取"cartUniqueKey"對應值的方法
        static string GetCartUniqueKeyValue(JToken token)
        {
            JObject obj = JObject.Parse(token.ToString());
            //Console.WriteLine(obj.ToString());
          
            JToken cartUniqueKeyToken = token["data"]["cartUniqueKey"];
            if (cartUniqueKeyToken != null)
            {
                //Console.WriteLine(cartUniqueKeyToken);
                return cartUniqueKeyToken.ToString();
            }

            return null; // 如果沒找到"cartUniqueKey"，回傳null
        }

        // 獲取"uniqueKey"對應值的方法
        static string GetUniqueKeyValue(JToken token)
        {
            JObject obj = JObject.Parse(token.ToString());
            //Console.WriteLine(obj.ToString());
          
            JToken UniqueKeyToken = token["data"]["uniqueKey"];
            if (UniqueKeyToken != null)
            {
                //Console.WriteLine(UniqueKeyToken);
                return UniqueKeyToken.ToString();
            }

            return null; // 如果沒找到"UniqueKeyToken"，回傳null
        }

        string cookie = "gaClientId=78749efe-cdd4-4b44-9c28-6ff421d016af; uAUTH=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; uAUTH_samesite=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; __lt__cid=e556f3ba-9714-4800-8871-b6dd760e306e; currency=TWD; GUID=2f161e2e-a76f-4d3f-aa66-a70a98ee9027; _fbp=fb.1.1695204836875.1699548327; ai_user=xAmJC|2023-09-20T10:15:23.828Z; auth=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; auth_samesite=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; MID=5600747285; isPromotion21194AlreadyPrompt=true; allowGetPrivacyInfo=true; _ga=GA1.1.78749efe-cdd4-4b44-9c28-6ff421d016af; _ga=GA1.2.78749efe-cdd4-4b44-9c28-6ff421d016af; fr=; fr2=; salePageViewList=888345,885704,884643,888399; isPromotion21302AlreadyPrompt=true; 91_FPID_v3_4_1=34cade3b34f722e5b9e6e12c1c118917; _clck=di1mzx|2|ffd|0|1358; __lt__sid=05cc1235-baef0f4b; isPromotion20938AlreadyPrompt=true; isPromotion21316AlreadyPrompt=true; ai_session=jhD6w|1695778144946|1695779542422.6; _clsk=3zmjc3|1695779542648|21|1|s.clarity.ms/collect";

        [HttpGet("Detail")]
        public async Task<ActionResult> GetCartDetail(string salepageid)
        {
            var existingsalepageid = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.salepageid.Equals(salepageid));
            if(existingsalepageid==null){
                // (1) [GET] product/detail
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
                    qty=0,
                    price=nineyi_price,
                    product=nineyi_title,
                    picture=nineyi_picurl,
                    salepageid=nineyi_salepageid,
                    shopid=nineyi_shopid,
                    skuid=nineyi_sku,
                    recommender=null,
                    points=0
                };
                _dbcontext.Orders.Add(newOrders);
                await _dbcontext.SaveChangesAsync();              
            }
           
            var orders = await _dbcontext.Orders
                .Where(o => o.salepageid.Equals(salepageid))
                .ToListAsync(); // 獲取所有符合的訂單

            if (orders == null || orders.Count == 0)
            {
                return BadRequest(new { message = "找不到符合條件的訂單" });
            }

            // 用字典合併相同的會員(可能出現在同一成員，在不同推薦人的連結下單)
            var memberDataDictionary = new Dictionary<string, int>();
            foreach (var order in orders)
            {
                // 排除掉資料庫中 member=host 的資料
                if (order.member == "host")
                {
                    continue;
                }
                
                if(memberDataDictionary.ContainsKey(order.member))
                {
                    // 如果已經存在相同的會員，則qty相加
                    memberDataDictionary[order.member] += order.qty ??0;
                }
                else
                {
                    // 沒有的話就獨立一筆
                    memberDataDictionary[order.member] = order.qty ??0;
                }                   
            }

            var memberData = memberDataDictionary.Select(kv => new
            {
                member = kv.Key,
                qty = kv.Value,
            }).ToList();

            // 加總所有的商品數
            var totalQty = memberData.Sum(member => member.qty);
            string totalQtyString = totalQty.ToString();
            var discountData=(object)null;

            if(totalQty!=0)
            {
                // (2)[POST] cart/create
                HttpClient clientCreate = new HttpClient();
                HttpRequestMessage requestCreate = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/shopping/api/carts/create"),
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91dev.tw" },
                    },
                };
                string requestBodyCreate = "{\r\n    \"source\": \"Web\"\r\n}"; // 替換為request正文
                requestCreate.Content = new StringContent(requestBodyCreate, Encoding.UTF8, "application/json");
                // 發送request並等待
                HttpResponseMessage responseCreate = await clientCreate.SendAsync(requestCreate);

                // 讀取回傳response
                string responseContentCreate = await responseCreate.Content.ReadAsStringAsync();

                // 解析JSON response 為 JObject
                JObject jsonResponseCreate = JObject.Parse(responseContentCreate);

                // 取得其他折扣、原價金額
                string TotalDiscount = jsonResponseCreate["data"]["salepageGroupList"][0]["salepageList"][0]["totalDiscount"].ToString();
                string TotalPayment = jsonResponseCreate["data"]["salepageGroupList"][0]["salepageList"][0]["totalPayment"].ToString();
                string TotalPrice = jsonResponseCreate["data"]["salepageGroupList"][0]["salepageList"][0]["totalPrice"].ToString();
                //Console.WriteLine("cartUniqueKey Value: " + cartUniqueKeyValue);

                //回傳所有折扣API相關資料
                discountData = new
                {
                    totalQtyString,                   
                    TotalDiscount,
                    TotalPayment,                   
                    TotalPrice
                };
            }
            else
            {
                discountData = new
                {
                    totalQtyString=0,                 
                    TotalDiscount=0,
                    TotalPayment=0,                   
                    TotalPrice=0
                };
            }
           
           return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, salepageid =orders[0].salepageid, shopid=orders[0].shopid, skuid=orders[0].skuid, memberData, discountData}); 
        }


        [HttpPost("add")]
        public async Task<ActionResult> addorder([FromBody]AddOrderDto input)
        {
            
            try{
                //如果下訂者已在同一個商品下過單，則直接在同一成員的數量做變更
                var existingmember = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.member == input.user_name && o.product == input.product && o.recommender== input.recommender);
                if(existingmember!=null)
                {
                    existingmember.qty+=input.product_qty;
                    existingmember.points +=1 ;
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
                        recommender=input.recommender,
                        points =1                   
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
                    return BadRequest(); 
                }

                // 用字典合併相同的會員(可能出現在同一成員，在不同推薦人的連結下單)
                var memberDataDictionary = new Dictionary<string, int>();
                foreach (var order in orders)
                {
                    if(memberDataDictionary.ContainsKey(order.member))
                    {
                        // 如果已經存在相同的會員，則qty相加
                        memberDataDictionary[order.member] += order.qty ??0;
                    }
                    else
                    {
                        // 沒有的話就獨立一筆
                        memberDataDictionary[order.member] = order.qty ??0;
                    }                   
                }

                var memberData = memberDataDictionary.Select(kv => new
                {
                    member = kv.Key,
                    qty = kv.Value,
                }).ToList(); 

                

                // (1) [POST] cart/insert API
                HttpClient clientInsert = new HttpClient();
                HttpRequestMessage requestInsert = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/webapi/ShoppingCartV4/InsertItem"),
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91d/ev.tw" },
                    },
                };

                // 請根據API的要求，修改以下requestBody
                string InsertRequestBody = @"
                {
                    ""shopId"": " + input.shopid + @",
                    ""salePageId"": " + input.salepageid + @",
                    ""saleProductSKUid"": " + input.skuid + @",
                    ""qty"": " + input.product_qty + @",
                    ""optionalTypeDef"": """",
                    ""optionalTypeId"": 0,
                    ""IsSkuQtyAccumulate"": true,
                    ""optionalInfo"": null
                }";


                requestInsert.Content = new StringContent(InsertRequestBody, Encoding.UTF8, "application/json");

                // 發送request並等待
                HttpResponseMessage responseInsert = await clientInsert.SendAsync(requestInsert);

                // 讀取回傳response
                string responseInsertContent = await responseInsert.Content.ReadAsStringAsync();

                // 解析JSON response 為 JObject，根據需要處理回應
                JObject InsertJsonResponse = JObject.Parse(responseInsertContent);

                Console.WriteLine(responseInsertContent);

                // 加總所有的商品數
                var totalQty = memberData.Sum(member => member.qty);
                string totalQtyString = totalQty.ToString();

                // (2)[POST] cart/create
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/shopping/api/carts/create"),
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91dev.tw" },
                    },
                };
                string requestBody = "{\r\n    \"source\": \"Web\"\r\n}"; // 替換為request正文
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                // 發送request並等待
                HttpResponseMessage response = await client.SendAsync(request);

                // 讀取回傳response
                string responseContent = await response.Content.ReadAsStringAsync();

                // 解析JSON response 為 JObject
                JObject jsonResponse = JObject.Parse(responseContent);

                // 取得其他折扣、原價金額
                string TotalDiscount = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalDiscount"].ToString();
                string TotalPayment = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalPayment"].ToString();
                string TotalPrice = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalPrice"].ToString();

                //回傳所有折扣API相關資料
                var discountData = new
                {
                    totalQtyString,                   
                    TotalDiscount,
                    TotalPayment,                   
                    TotalPrice,
                };

                return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData, discountData});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}