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
            string cookie = "gaClientId=aac6bbb6-42e8-4566-beb6-70b39ecccd46; __lt__cid=2178dfab-832a-424c-ad74-beab110fcd4f; uAUTH=KUgNJm2SrPlcG9oZpdbaEij5WqcNRzaS3IlESf4JfVYZiTFE/8OAb3ljMvqGTjKsoIxG47NEDSR0SNEGQq4w/Px3lVuGZHvp5cAxzWAUMbI=; uAUTH_samesite=KUgNJm2SrPlcG9oZpdbaEij5WqcNRzaS3IlESf4JfVYZiTFE/8OAb3ljMvqGTjKsoIxG47NEDSR0SNEGQq4w/Px3lVuGZHvp5cAxzWAUMbI=; _fbp=fb.1.1695024637093.1648731515; ai_user=neuP9|2023-09-18T08:11:11.189Z; GUID=8b7f9804-9726-4894-94f4-16c594486040; auth=od+XMJjXjeNCLaky+2jWL8+c+/Yqgxf2syxeW2oYUoF5ajC1dJZxe2d7qV2gK9+/mKN5XPlgy7iEU7KZpsuu12RBJYtTlImeuhEPKs8nf2lkLDvC1poe7lbPK9k/ZPcjZXP/eR51ijOD4Ql3pCDaKSrMi3oUoLYOJqLI5pQQ5JBofoUZjs5vnZ2iqnfZvIrNUl7NTvob1byrMdFvBMeKhB9NpV0/z6vVjhRD3fCqhCIBjM2uFzc5KtmM7qdgWIVNGa+ZiDIJsxHy8yoUcn0dM1W3SX1msZzglokOBiHG1aSgSo2drq6mjAIPqPZbwPAnlnYlAeNB+6IjKGDSn6d1ifSgrrztIzoEDBo8CaXyBiFgyHEWiF+jMe42MofgnyzhkHvI1yl8uNAwFU3qPJWtzZ6GtDRY/fezEPzZhoedAPZgcrWFlmHnmE3I4wIPcAH+RokEnsBGPMrBA6Czzlv6IXQEcrIkw52Piq+JcZiSVvuOMGYhR41MZaeHMAeBTqytZLUx4kkEwXa9DOUkQs3QuRuzbSqMmx70O9zwWW2pkZMKiZ94JDrJ+BWIn/tAqGqfGTHBuXzYpUvdtdKsHuxjFvWSfHvOqFzpuTZt6mEILqWdtta3vVwNJ5Z0d0b7Nk60jbqRYY1DSOX98vbI9Sv3a6wXAoAWZREyaMCfhAMXlSFzX810hUGuavcwZdfRC++8anoCzDWEOD7FmWJONOJ0L3ojI5GuJW340kQhHUU2UQkk2Z7Ga7sXgxLccyWwA0spFFn6O0z1RZY/vOrTC2pZ6bi9hhm9ymEdNS3bBOSrHUo=; auth_samesite=od+XMJjXjeNCLaky+2jWL8+c+/Yqgxf2syxeW2oYUoF5ajC1dJZxe2d7qV2gK9+/mKN5XPlgy7iEU7KZpsuu12RBJYtTlImeuhEPKs8nf2lkLDvC1poe7lbPK9k/ZPcjZXP/eR51ijOD4Ql3pCDaKSrMi3oUoLYOJqLI5pQQ5JBofoUZjs5vnZ2iqnfZvIrNUl7NTvob1byrMdFvBMeKhB9NpV0/z6vVjhRD3fCqhCIBjM2uFzc5KtmM7qdgWIVNGa+ZiDIJsxHy8yoUcn0dM1W3SX1msZzglokOBiHG1aSgSo2drq6mjAIPqPZbwPAnlnYlAeNB+6IjKGDSn6d1ifSgrrztIzoEDBo8CaXyBiFgyHEWiF+jMe42MofgnyzhkHvI1yl8uNAwFU3qPJWtzZ6GtDRY/fezEPzZhoedAPZgcrWFlmHnmE3I4wIPcAH+RokEnsBGPMrBA6Czzlv6IXQEcrIkw52Piq+JcZiSVvuOMGYhR41MZaeHMAeBTqytZLUx4kkEwXa9DOUkQs3QuRuzbSqMmx70O9zwWW2pkZMKiZ94JDrJ+BWIn/tAqGqfGTHBuXzYpUvdtdKsHuxjFvWSfHvOqFzpuTZt6mEILqWdtta3vVwNJ5Z0d0b7Nk60jbqRYY1DSOX98vbI9Sv3a6wXAoAWZREyaMCfhAMXlSFzX810hUGuavcwZdfRC++8anoCzDWEOD7FmWJONOJ0L3ojI5GuJW340kQhHUU2UQkk2Z7Ga7sXgxLccyWwA0spFFn6O0z1RZY/vOrTC2pZ6bi9hhm9ymEdNS3bBOSrHUo=; MID=5500747275; allowGetPrivacyInfo=true; _ga=GA1.1.aac6bbb6-42e8-4566-beb6-70b39ecccd46; _ga=GA1.2.aac6bbb6-42e8-4566-beb6-70b39ecccd46; fr=; fr2=; lang=zh-TW; _clck=1gwpfc9|2|ffd|0|1356; currency=TWD; __lt__sid=911bb3e4-8341e748; 91_FPID_v3_4_1=bba2f0d057c32f7f3e3043895830d1c5; _gat=1; _clsk=wqku1a|1695807115749|6|1|s.clarity.ms/collect; ai_session=vHZYm|1695806446904|1695807127587";

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
