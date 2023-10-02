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

        string cookie = "uAUTH=uMy2bMc8RrP/BWJadbxhFSSX4Xb5nYYE2fuRASj+cCC/Xi6OikbdSG80O7WvAiP72404zwY2pA4u73mgqah5lnn0hUf5h45B/VN093nHYp8=; uAUTH_samesite=uMy2bMc8RrP/BWJadbxhFSSX4Xb5nYYE2fuRASj+cCC/Xi6OikbdSG80O7WvAiP72404zwY2pA4u73mgqah5lnn0hUf5h45B/VN093nHYp8=; ai_user=DRG7d|2023-09-21T02:48:11.332Z; gaClientId=eed70906-4b5f-423d-abb7-33af67c24a90; GUID=83748c63-df5f-4a1f-a4db-e4fbda245196; __lt__cid=694bb7e5-b6db-47e9-b5f5-1ed67d9e2bc1; _fbp=fb.1.1695264492779.296659842; allowGetPrivacyInfo=true; isPromotion20658AlreadyPrompt=true; isPromotion21302AlreadyPrompt=true; isPromotion21194AlreadyPrompt=true; isPromotion20938AlreadyPrompt=true; isPromotion19570AlreadyPrompt=true; auth=od+XMJjXjeNCLaky+2jWLzZnc35kjQf2Ibdhll/HTXRIpShGsFU3jfr31DRt30aqySi/a5psRsV04FEFdQJhTG0M60iYj8i8gRCo2+QYWgMfaG+l8WUa908EbvZduhDFyfH71UxlEotd/ulKr2lfKt2vq79KdZlkGrOssgCSVXKOadf45kcTs36P39rrQtDvMcZMV7pmNowEoJN82AbkFOhpc6NHAYPJo2GWY8tixVgYhbfcn1//fOnHRuPJ9cv8Y6MI9bcy10iyDgBLuGC111Mb9EJy48s40dajZrDqB61HnpbEWDfiMn3YQEvt7XpvkHBJrZWwxZP7o3olZRqEE9Jp0THUcVVtiGtLKOLJfhc7BPK7kgfjrHG1G6uQARaD/2yebPw0essdbgYzEOs1SckTloA3svEW30Dp9yn94tyIBOe2sq3+u4yjT91OiVGDf8GbfozRBLNsW41xetGPvutuoO+RrJweRLfaJ31CCwQosQs/dV9IVPYTvS6AZYwpD1UceSxwFkHEnL9VzDDNmYwYZkvNOp6ZW6/Nd6954NnQBZpDLGsqf7SfWHG/Qz5cesFQz/D4kzO71RB9rYOZJzcymXVX2x8y1/JrLxmHbe+dfnOG26yalRXUH4tHXDkiQeCtA7aM2k4iIW4VSUh+8DJcx2Jk37mEKzwzGieXyhqWaLIAEMHPKuUuIV0e4CD2d98k0UqV/koPMUbejhXPuWxk+J9G0qMGKYI0WGhBIY+zzbSp2q8L3GVa56GglVAZYubSRTb68mAd1y3JtTdN7Jc0gVV9MhaW31gE1pFn9G4=; auth_samesite=od+XMJjXjeNCLaky+2jWLzZnc35kjQf2Ibdhll/HTXRIpShGsFU3jfr31DRt30aqySi/a5psRsV04FEFdQJhTG0M60iYj8i8gRCo2+QYWgMfaG+l8WUa908EbvZduhDFyfH71UxlEotd/ulKr2lfKt2vq79KdZlkGrOssgCSVXKOadf45kcTs36P39rrQtDvMcZMV7pmNowEoJN82AbkFOhpc6NHAYPJo2GWY8tixVgYhbfcn1//fOnHRuPJ9cv8Y6MI9bcy10iyDgBLuGC111Mb9EJy48s40dajZrDqB61HnpbEWDfiMn3YQEvt7XpvkHBJrZWwxZP7o3olZRqEE9Jp0THUcVVtiGtLKOLJfhc7BPK7kgfjrHG1G6uQARaD/2yebPw0essdbgYzEOs1SckTloA3svEW30Dp9yn94tyIBOe2sq3+u4yjT91OiVGDf8GbfozRBLNsW41xetGPvutuoO+RrJweRLfaJ31CCwQosQs/dV9IVPYTvS6AZYwpD1UceSxwFkHEnL9VzDDNmYwYZkvNOp6ZW6/Nd6954NnQBZpDLGsqf7SfWHG/Qz5cesFQz/D4kzO71RB9rYOZJzcymXVX2x8y1/JrLxmHbe+dfnOG26yalRXUH4tHXDkiQeCtA7aM2k4iIW4VSUh+8DJcx2Jk37mEKzwzGieXyhqWaLIAEMHPKuUuIV0e4CD2d98k0UqV/koPMUbejhXPuWxk+J9G0qMGKYI0WGhBIY+zzbSp2q8L3GVa56GglVAZYubSRTb68mAd1y3JtTdN7Jc0gVV9MhaW31gE1pFn9G4=; MID=4800747206; lang=zh-TW; _clck=jho68m|2|ffi|0|1359; currency=TWD; 91_FPID_v3_4_1=0c4612ef2ebeac95a2280a822e5425df; fr=; fr2=; isPromotion19213AlreadyPrompt=true; isPromotion20859AlreadyPrompt=true; salePageViewList=884643,888399,888560; __lt__sid=5c4bce63-d1a1e12c; _gat=1; ai_session=kZZP8|1696228771541|1696228809517.4; _clsk=1113bjf|1696228809765|4|1|s.clarity.ms/collect";

        [HttpGet("Detail")]
        public async Task<ActionResult> GetCartDetail(string salepageid)
        {
            var existingsalepageid = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.salepageid.Equals(salepageid) && o.status == "unpaid");
            if(existingsalepageid==null){
                // (1) [GET] product/detail
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://10230.shop.qa.91dev.tw/webapi/SalePageV2/GetSalePageV2Info/10230/{salepageid}"),
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
                    points=0,
                    status="unpaid"
                };
                _dbcontext.Orders.Add(newOrders);
                await _dbcontext.SaveChangesAsync();              
            }
           
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
                if (order.member == "host" || order.status == "paid")
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
            var totalQty = orders.Where(order => order.status == "unpaid").Sum(member => member.qty);
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
                string TotalPayment = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalPayment"].ToString();
                string TotalPrice = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["totalPrice"].ToString();
                string PromotionId = jsonResponse["data"]["salepageGroupList"][0]["salepageList"][0]["discountDisplayList"][0]["promotionId"].ToString();

            
                //儲存資料到DB
                //如果下訂者已在同一個商品下過單，則直接在同一成員的數量做變更
                var existingmember = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.member == input.user_name && o.product == input.product && o.recommender== input.recommender && o.status=="unpaid");
                if(existingmember!=null )
                {
                    existingmember.qty+=input.product_qty;
                    //existingmember.promotionid = PromotionId; 
                    //existingmember.points +=1 ; //如果每團友每加購一筆，團長就多一點積分
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
                        status="unpaid"
                        //promotionid = PromotionId                   
                    };
                    _dbcontext.Orders.Add(newOrders);
                }    
                await _dbcontext.SaveChangesAsync();

                // 更新 member欄位等於"host" 的資料的 promotionId。只會在host紀錄
                var hostOrders = await _dbcontext.Orders
                    .Where(o => o.status == "unpaid" && o.salepageid == input.salepageid)
                    .ToListAsync();

                foreach (var hostOrder in hostOrders)
                {
                    hostOrder.promotionid = PromotionId;
                }                
                await _dbcontext.SaveChangesAsync();

                // 將host的資料存到其他member欄位
                // 複製的資料
                var matchingOrders = await _dbcontext.Orders
                    .Where(o => o.member == "host" && o.salepageid == input.salepageid && o.status == "unpaid")
                    .ToListAsync();

                // 貼上的資料
                var matchingOrder = await _dbcontext.Orders
                    .Where(o => o.member != "host" && o.salepageid == input.salepageid && o.status == "unpaid")
                    .ToListAsync();

                if (matchingOrders.Any())
                {
                    // 符合條件的記錄，找到最新的記錄
                    var latestOrder = matchingOrder.OrderByDescending(o => o.id).FirstOrDefault();

                    // 直接更新符合條件的記錄的 promotionid、campaign 和 estimate
                    latestOrder.promotionid = matchingOrders[0].promotionid;
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
                    if (order.member == "host" || order.status == "paid")
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
