using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly GoShopContext _dbcontext; 

        public CartsController(GoShopContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public class CartCreate
        {
            string source;
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

        string OrderCode;

        [HttpPost]
        [Route("/Carts/Checkout")]
        public async Task<IActionResult> Insert(CartCheckoutDto input)
        {
            string cookie = "uAUTH=uMy2bMc8RrP/BWJadbxhFSSX4Xb5nYYE2fuRASj+cCC/Xi6OikbdSG80O7WvAiP72404zwY2pA4u73mgqah5lnn0hUf5h45B/VN093nHYp8=; uAUTH_samesite=uMy2bMc8RrP/BWJadbxhFSSX4Xb5nYYE2fuRASj+cCC/Xi6OikbdSG80O7WvAiP72404zwY2pA4u73mgqah5lnn0hUf5h45B/VN093nHYp8=; ai_user=DRG7d|2023-09-21T02:48:11.332Z; gaClientId=eed70906-4b5f-423d-abb7-33af67c24a90; GUID=83748c63-df5f-4a1f-a4db-e4fbda245196; __lt__cid=694bb7e5-b6db-47e9-b5f5-1ed67d9e2bc1; _fbp=fb.1.1695264492779.296659842; allowGetPrivacyInfo=true; fr=; fr2=; isPromotion20658AlreadyPrompt=true; isPromotion21302AlreadyPrompt=true; _clck=jho68m|2|ffd|0|1359; isPromotion21194AlreadyPrompt=true; isPromotion20938AlreadyPrompt=true; isPromotion19570AlreadyPrompt=true; 91_FPID_v3_4_1=df26cef9cad065d7525ec9f16b51a2fe; auth=od+XMJjXjeNCLaky+2jWLzZnc35kjQf2Ibdhll/HTXRIpShGsFU3jfr31DRt30aqySi/a5psRsV04FEFdQJhTG0M60iYj8i8gRCo2+QYWgMfaG+l8WUa908EbvZduhDFyfH71UxlEotd/ulKr2lfKt2vq79KdZlkGrOssgCSVXKOadf45kcTs36P39rrQtDvMcZMV7pmNowEoJN82AbkFOhpc6NHAYPJo2GWY8tixVgYhbfcn1//fOnHRuPJ9cv8Y6MI9bcy10iyDgBLuGC111Mb9EJy48s40dajZrDqB61HnpbEWDfiMn3YQEvt7XpvkHBJrZWwxZP7o3olZRqEE9Jp0THUcVVtiGtLKOLJfhc7BPK7kgfjrHG1G6uQARaD/2yebPw0essdbgYzEOs1SckTloA3svEW30Dp9yn94tyIBOe2sq3+u4yjT91OiVGDf8GbfozRBLNsW41xetGPvutuoO+RrJweRLfaJ31CCwQosQs/dV9IVPYTvS6AZYwpD1UceSxwFkHEnL9VzDDNmYwYZkvNOp6ZW6/Nd6954NnQBZpDLGsqf7SfWHG/Qz5cesFQz/D4kzO71RB9rYOZJzcymXVX2x8y1/JrLxmHbe+dfnOG26yalRXUH4tHXDkiQeCtA7aM2k4iIW4VSUh+8DJcx2Jk37mEKzwzGieXyhqWaLIAEMHPKuUuIV0e4CD2d98k0UqV/koPMUbejhXPuWxk+J9G0qMGKYI0WGhBIY+zzbSp2q8L3GVa56GglVAZYubSRTb68mAd1y3JtTdN7Jc0gVV9MhaW31gE1pFn9G4=; auth_samesite=od+XMJjXjeNCLaky+2jWLzZnc35kjQf2Ibdhll/HTXRIpShGsFU3jfr31DRt30aqySi/a5psRsV04FEFdQJhTG0M60iYj8i8gRCo2+QYWgMfaG+l8WUa908EbvZduhDFyfH71UxlEotd/ulKr2lfKt2vq79KdZlkGrOssgCSVXKOadf45kcTs36P39rrQtDvMcZMV7pmNowEoJN82AbkFOhpc6NHAYPJo2GWY8tixVgYhbfcn1//fOnHRuPJ9cv8Y6MI9bcy10iyDgBLuGC111Mb9EJy48s40dajZrDqB61HnpbEWDfiMn3YQEvt7XpvkHBJrZWwxZP7o3olZRqEE9Jp0THUcVVtiGtLKOLJfhc7BPK7kgfjrHG1G6uQARaD/2yebPw0essdbgYzEOs1SckTloA3svEW30Dp9yn94tyIBOe2sq3+u4yjT91OiVGDf8GbfozRBLNsW41xetGPvutuoO+RrJweRLfaJ31CCwQosQs/dV9IVPYTvS6AZYwpD1UceSxwFkHEnL9VzDDNmYwYZkvNOp6ZW6/Nd6954NnQBZpDLGsqf7SfWHG/Qz5cesFQz/D4kzO71RB9rYOZJzcymXVX2x8y1/JrLxmHbe+dfnOG26yalRXUH4tHXDkiQeCtA7aM2k4iIW4VSUh+8DJcx2Jk37mEKzwzGieXyhqWaLIAEMHPKuUuIV0e4CD2d98k0UqV/koPMUbejhXPuWxk+J9G0qMGKYI0WGhBIY+zzbSp2q8L3GVa56GglVAZYubSRTb68mAd1y3JtTdN7Jc0gVV9MhaW31gE1pFn9G4=; MID=4800747206; __lt__sid=5c4bce63-fd0360ad; lang=zh-TW; currency=TWD; ai_session=jBYOV|1695821247762|1695823940791.9; _clsk=rhn97q|1695823940793|2|1|s.clarity.ms/collect";

            try
            {
                // (1)[post API] cart/create
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

                // 獲取"cartUniqueKey"對應的值
                string cartUniqueKeyValue = GetCartUniqueKeyValue(jsonResponse);

                Console.WriteLine("cartUniqueKey Value: " + cartUniqueKeyValue);
                // Console.WriteLine("JSON Response: " + responseContent);



                // (2)[post API] checkout/create
                HttpClient clientCheckoutCreate = new HttpClient();
                HttpRequestMessage requestCheckoutCreate = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/shopping/api/Checkout/Create"),
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91dev.tw" },
                    },
                };

                string checkoutRequestBody = "{\"cartUniqueKey\": \"" + cartUniqueKeyValue + "\"}";
                requestCheckoutCreate.Content = new StringContent(checkoutRequestBody, Encoding.UTF8, "application/json");

                HttpResponseMessage responseCheckoutCreate = await client.SendAsync(requestCheckoutCreate);

                // 讀取回傳response
                string responseCheckoutCreateContent = await responseCheckoutCreate.Content.ReadAsStringAsync();

                // 解析JSON response 為 JObject
                JObject CheckoutCreatejsonResponse = JObject.Parse(responseCheckoutCreateContent);

                // 獲取"uniqueKey"對應的值
                string uniqueKey = GetUniqueKeyValue(CheckoutCreatejsonResponse);

                Console.WriteLine("uniqueKey: " + uniqueKey);

                //(3)[get API] checkout
                var queryParameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "checkoutUniqueKey", uniqueKey },
                    { "lang", "zh-TW" },
                    { "shopId", "10230" }
                };

                // 构建查询字符串
                var clientCheckout = new HttpClient();
                var queryString = new System.Text.StringBuilder();
                foreach (var parameter in queryParameters)
                {
                    queryString.Append($"{parameter.Key}={parameter.Value}&");
                }

                // 移除最后一个"&"字符
                if (queryString.Length > 0)
                {
                    queryString.Length--;
                }
                // 构建请求URI，附加查询字符串
                var requestUri = new UriBuilder("https://10230.shop.qa.91dev.tw/shopping/api/Checkout");
                requestUri.Query = queryString.ToString();

                var requestCheckout = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = requestUri.Uri,
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91dev.tw" },
                    },
                };

                var responseCheckout = await clientCheckout.SendAsync(requestCheckout);

                // 处理响应
                if (response.IsSuccessStatusCode)
                {
                    // 处理成功响应
                    var content = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(content);

                    // 解析JSON response 為 JObject
                    JObject CheckoutjsonResponse = JObject.Parse(content);

                    Console.WriteLine(GetCartUniqueKeyValue(CheckoutjsonResponse));
                }
                else
                {
                    // 处理错误响应
                    Console.WriteLine($"HTTP请求失败：{response.StatusCode}");
                }

                //(4)[post API] checkout/complete
                HttpClient clientCheckComplete = new HttpClient();
                HttpRequestMessage requestCheckComplete = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/shopping/api/Checkout/Complete"),
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91dev.tw" },
                    },
                };
                string requestCheckCompleteBody = "{\r\n \"applePayInfo\": {\r\n \"decryptedData\": null,\r\n \"encryptedInfo\": \"\",\r\n \"merchantIdentifier\": null\r\n },\r\n \"checkoutUniqueKey\": \"{uniqueKey}\",\r\n \"creditCardGatewayType\": \"Nine1Payment\",\r\n \"email\": \"\",\r\n \"isEnableEDM\": true,\r\n \"isNeedCreditCheck\": false,\r\n \"paymentInfo\": null,\r\n \"rememberCreditCardNo\": true,\r\n \"tapPayCardInfo\": null,\r\n \"userMemo\": \"\"\r\n}";

                requestCheckCompleteBody = requestCheckCompleteBody.Replace("{uniqueKey}", uniqueKey);

                requestCheckComplete.Content = new StringContent(requestCheckCompleteBody, Encoding.UTF8, "application/json");
                // 發送request並等待
                HttpResponseMessage responseCheckComplete = await clientCheckComplete.SendAsync(requestCheckComplete);

                // 讀取回傳response
                string responseCheckCompleteContent = await responseCheckComplete.Content.ReadAsStringAsync();
                Console.WriteLine(responseCheckCompleteContent);


                HttpClient clientCheckOrder = new HttpClient();
                string payload = @"{
                    ""shopId"": 10230
                }";

                HttpContent CheckOrdercontent = new StringContent(payload, Encoding.UTF8, "application/json");

                HttpRequestMessage requestCheckOrder = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://10230.shop.qa.91dev.tw/webapi/MemberTradesOrder/GetList?maxCount=1&lang=zh-TW"), // Remove shopId from the query parameters
                    Headers =
                    {
                        { "N1-SHOP-ID", "10230" },
                        { "Cookie", cookie },
                        { "N1-HOST", "10230.shop.qa.91dev.tw" },
                    },
                    Content = CheckOrdercontent, // Include the JSON payload in the request body
                };

                HttpResponseMessage responseCheckOrder = await clientCheckOrder.SendAsync(requestCheckOrder);

                // 讀取回傳response
                string responseCheckOrderContent = await responseCheckOrder.Content.ReadAsStringAsync();
                //Console.WriteLine(responseCheckOrderContent);
                JObject jsonCheckOrderResponse = JObject.Parse(responseCheckOrderContent);
                OrderCode = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][0]["Code"].ToString();
                Console.WriteLine("訂單編號: "+OrderCode);

                // 更改付款狀態為paid
                var orders = await _dbcontext.Orders
                    .Where(o => o.salepageid.Equals(input.salepageid))
                    .ToListAsync();

                foreach (var order in orders)
                {
                    order.status = "paid"; 
                }
                await _dbcontext.SaveChangesAsync();
   
            }
            catch (Exception ex) 
            {

            }
            return Ok(new {
                orderNumber = OrderCode,
            });
        }
    }
}
