using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly GoShopContext _dbcontext; 
        private readonly IHubContext<GoShopHub> _hubContext;

        public OrderController(GoShopContext dbcontext, IHubContext<GoShopHub> hubContext)
        {
            _dbcontext = dbcontext;
            _hubContext = hubContext;
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

        string cookie = "gaClientId=aac6bbb6-42e8-4566-beb6-70b39ecccd46; __lt__cid=2178dfab-832a-424c-ad74-beab110fcd4f; uAUTH=KUgNJm2SrPlcG9oZpdbaEij5WqcNRzaS3IlESf4JfVYZiTFE/8OAb3ljMvqGTjKsoIxG47NEDSR0SNEGQq4w/Px3lVuGZHvp5cAxzWAUMbI=; uAUTH_samesite=KUgNJm2SrPlcG9oZpdbaEij5WqcNRzaS3IlESf4JfVYZiTFE/8OAb3ljMvqGTjKsoIxG47NEDSR0SNEGQq4w/Px3lVuGZHvp5cAxzWAUMbI=; _fbp=fb.1.1695024637093.1648731515; ai_user=neuP9|2023-09-18T08:11:11.189Z; GUID=8b7f9804-9726-4894-94f4-16c594486040; allowGetPrivacyInfo=true; _ga=GA1.1.aac6bbb6-42e8-4566-beb6-70b39ecccd46; _ga=GA1.2.aac6bbb6-42e8-4566-beb6-70b39ecccd46; auth=od+XMJjXjeNCLaky+2jWL8+c+/Yqgxf2syxeW2oYUoF5ajC1dJZxe2d7qV2gK9+/mKN5XPlgy7iEU7KZpsuu12RBJYtTlImeuhEPKs8nf2lkLDvC1poe7lbPK9k/ZPcjZXP/eR51ijOD4Ql3pCDaKSrMi3oUoLYOJqLI5pQQ5JBofoUZjs5vnZ2iqnfZvIrNUl7NTvob1byrMdFvBMeKhB9NpV0/z6vVjhRD3fCqhCIBjM2uFzc5KtmM7qdgWIVNGa+ZiDIJsxHy8yoUcn0dM1W3SX1msZzglokOBiHG1aSgSo2drq6mjAIPqPZbwPAnlnYlAeNB+6IjKGDSn6d1ifSgrrztIzoEDBo8CaXyBiFgyHEWiF+jMe42MofgnyzhkHvI1yl8uNAwFU3qPJWtzZ6GtDRY/fezEPzZhoedAPZgcrWFlmHnmE3I4wIPcAH+RokEnsBGPMrBA6Czzlv6IXQEcrIkw52Piq+JcZiSVvuOMGYhR41MZaeHMAeBTqytZLUx4kkEwXa9DOUkQs3QuRuzbSqMmx70O9zwWW2pkZMKiZ94JDrJ+BWIn/tAqGqfGTHBuXzYpUvdtdKsHuxjFvWSfHvOqFzpuTZt6mEILqWdtta3vVwNJ5Z0d0b7Nk60jbqRYY1DSOX98vbI9Sv3a6wXAoAWZREyaMCfhAMXlSFzX810hUGuavcwZdfRC++8anoCzDWEOD7FmWJONOJ0L3ojI5GuJW340kQhHUU2UQkk2Z7Ga7sXgxLccyWwA0spFFn6O0z1RZY/vOrTC2pZ6bi9hhm9ymEdNS3bBOSrHUo=; auth_samesite=od+XMJjXjeNCLaky+2jWL8+c+/Yqgxf2syxeW2oYUoF5ajC1dJZxe2d7qV2gK9+/mKN5XPlgy7iEU7KZpsuu12RBJYtTlImeuhEPKs8nf2lkLDvC1poe7lbPK9k/ZPcjZXP/eR51ijOD4Ql3pCDaKSrMi3oUoLYOJqLI5pQQ5JBofoUZjs5vnZ2iqnfZvIrNUl7NTvob1byrMdFvBMeKhB9NpV0/z6vVjhRD3fCqhCIBjM2uFzc5KtmM7qdgWIVNGa+ZiDIJsxHy8yoUcn0dM1W3SX1msZzglokOBiHG1aSgSo2drq6mjAIPqPZbwPAnlnYlAeNB+6IjKGDSn6d1ifSgrrztIzoEDBo8CaXyBiFgyHEWiF+jMe42MofgnyzhkHvI1yl8uNAwFU3qPJWtzZ6GtDRY/fezEPzZhoedAPZgcrWFlmHnmE3I4wIPcAH+RokEnsBGPMrBA6Czzlv6IXQEcrIkw52Piq+JcZiSVvuOMGYhR41MZaeHMAeBTqytZLUx4kkEwXa9DOUkQs3QuRuzbSqMmx70O9zwWW2pkZMKiZ94JDrJ+BWIn/tAqGqfGTHBuXzYpUvdtdKsHuxjFvWSfHvOqFzpuTZt6mEILqWdtta3vVwNJ5Z0d0b7Nk60jbqRYY1DSOX98vbI9Sv3a6wXAoAWZREyaMCfhAMXlSFzX810hUGuavcwZdfRC++8anoCzDWEOD7FmWJONOJ0L3ojI5GuJW340kQhHUU2UQkk2Z7Ga7sXgxLccyWwA0spFFn6O0z1RZY/vOrTC2pZ6bi9hhm9ymEdNS3bBOSrHUo=; MID=5500747275; lang=zh-TW; _clck=1gwpfc9|2|ffj|0|1356; currency=TWD; __lt__sid=911bb3e4-26a2563f; 91_FPID_v3_4_1=bba2f0d057c32f7f3e3043895830d1c5; _gat=1; ai_session=4EyGa|1696303852676|1696303888424.7; _clsk=1014kvj|1696303888895|2|1|s.clarity.ms/collect";

        [HttpGet("Detail")]
        public async Task<ActionResult> GetCartDetail(string salepageid)
        {
            // var existingsalepageid = await _dbcontext.Orders
            //         .FirstOrDefaultAsync(o => o.salepageid.Equals(salepageid) && o.status == "開團中");
            // if(existingsalepageid==null){
            //     // (1) [GET] product/detail
            //     HttpClient client = new HttpClient();
            //     HttpRequestMessage request = new HttpRequestMessage()
            //     {
            //         Method = HttpMethod.Get,
            //         RequestUri = new Uri($"https://10230.shop.qa.91dev.tw/webapi/SalePageV2/GetSalePageV2Info/10230/{salepageid}"),
            //     };
            //     string requestBody = "{\r\n    \"source\": \"Web\"\r\n}"; // 替換為request正文
            //     request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            //     // 發送request並等待
            //     HttpResponseMessage response = await client.SendAsync(request);

            //     // 讀取回傳response
            //     string responseContent = await response.Content.ReadAsStringAsync();

            //     // 解析JSON response 為 JObject
            //     JObject jsonResponse = JObject.Parse(responseContent);
            
            //     // 抓取商品資訊
            //     string nineyi_salepageid = jsonResponse["Data"]["Id"].ToString();
            //     string nineyi_shopid = jsonResponse["Data"]["ShopId"].ToString();
            //     string nineyi_title = jsonResponse["Data"]["Title"].ToString();
            //     string priceStr = jsonResponse["Data"]["Price"].ToString();
            //     int nineyi_price = Convert.ToInt32(priceStr);
            //     string nineyi_picurl = jsonResponse["Data"]["ImageList"][0]["PicUrl"].ToString();
            //     string nineyi_sku = jsonResponse["Data"]["SaleProductSKUIdList"][0].ToString();

            //     //儲存到DB
            //     var newOrders= new Orders{
            //         member="host",
            //         qty=0,
            //         price=nineyi_price,
            //         product=nineyi_title,
            //         picture=nineyi_picurl,
            //         salepageid=nineyi_salepageid,
            //         shopid=nineyi_shopid,
            //         skuid=nineyi_sku,
            //         recommender=null,
            //         points=0,
            //         status="開團中"
            //     };
            //     _dbcontext.Orders.Add(newOrders);
            //     await _dbcontext.SaveChangesAsync();              
            // }
           
            var orders = await _dbcontext.Orders
                .Where(o => o.salepageid.Equals(salepageid))
                .ToListAsync(); // 獲取所有符合的訂單
            Console.WriteLine("orders:"+orders);

            if (orders == null || orders.Count == 0)
            {
                return BadRequest(new { message = "找不到符合條件的訂單" });
            }

            // 用字典合併相同的會員(可能出現在同一成員，在不同推薦人的連結下單)
            var memberDataDictionary = new Dictionary<string, int>();
            foreach (var order in orders)
            {
                // 排除掉資料庫中 member=host 的資料
                if (order.member == "host" || order.status == "已結團")
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
            Console.WriteLine("memberDataGET:"+memberData);

            // 加總所有的商品數
            var totalQty = orders.Where(order => order.status == "開團中").Sum(member => member.qty);
            string totalQtyString = totalQty.ToString();
            var discountData=(object)null;

            // 如果資料庫的商品數不為0，且團友狀態為unpaid
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

            await _hubContext.Clients.All.SendAsync("SendOrderData", new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, salepageid =orders[0].salepageid, shopid=orders[0].shopid, skuid=orders[0].skuid, memberData, discountData});
           
           return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, salepageid =orders[0].salepageid, shopid=orders[0].shopid, skuid=orders[0].skuid, memberData, discountData}); 
        }


        [HttpPost("add")]
        public async Task<ActionResult> addorder([FromBody]AddOrderDto input)
        {           
            try{
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
                int totaldiscount=int.Parse(TotalDiscount);
                string TotalPayment = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalPayment"].ToString();
                int afterdiscount=int.Parse(TotalPayment);
                string TotalPrice = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalPrice"].ToString();
                int totalprice=int.Parse(TotalPrice);
                string PromotionId = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["discountDisplayList"][0]["promotionId"].ToString();

           
                //儲存資料到DB
                //如果下訂者已在同一個商品下過單，則直接在同一成員的數量做變更
                var existingmember = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.member == input.user_name && o.product == input.product && o.recommender== input.recommender && o.status=="開團中");
                if(existingmember!=null )
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
                        recommender=input.recommender,
                        points =1,
                        status="開團中",                   
                    };
                    _dbcontext.Orders.Add(newOrders);
                }    
                await _dbcontext.SaveChangesAsync();

                var existingdata = await _dbcontext.Orders
                    .Where(o => o.product == input.product && o.status == "開團中" && o.salepageid == input.salepageid)
                    .ToListAsync();

                if (existingdata != null)
                {
                    foreach (var data in existingdata)
                    {
                        data.totaldiscount = totaldiscount;
                        data.afterdiscount = afterdiscount;
                        data.totalprice = totalprice;
                    }

                    await _dbcontext.SaveChangesAsync();
                }
                else
                {
                    return BadRequest();
                }


                // 將host的資料存到其他member欄位
                // 複製的資料
                var matchingOrders = await _dbcontext.Orders
                    .Where(o => o.member == "host" && o.salepageid == input.salepageid && o.status == "開團中")
                    .ToListAsync();

                // 貼上的資料
                var matchingOrder = await _dbcontext.Orders
                    .Where(o => o.member != "host" && o.salepageid == input.salepageid && o.status == "開團中")
                    .ToListAsync();

                if (matchingOrders.Any())
                {
                    // 符合條件的記錄，找到最新的記錄
                    var latestOrder = matchingOrder.OrderByDescending(o => o.id).FirstOrDefault();

                    latestOrder.campaign = matchingOrders[0].campaign;
                    latestOrder.estimate = matchingOrders[0].estimate;
                    latestOrder.start = matchingOrders[0].start;
                    latestOrder.finish = matchingOrders[0].finish;

                    await _dbcontext.SaveChangesAsync();
                }



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
                    // 排除掉資料庫中 member=host 的資料
                    if (order.member == "host" || order.status == "已結團")
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
                Console.WriteLine("memberDataPOST:"+memberData);

                //回傳所有折扣API相關資料
                var discountData = new
                {
                    totalQtyString,                   
                    TotalDiscount,
                    TotalPayment,                   
                    TotalPrice,
                };

                await _hubContext.Clients.All.SendAsync("SendOrderData", new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData, discountData});
                return Ok(new { product = orders[0].product, price = orders[0].price, picture=orders[0].picture, memberData, discountData});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
