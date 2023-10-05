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
            string cookie = "gaClientId=78749efe-cdd4-4b44-9c28-6ff421d016af; uAUTH=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; uAUTH_samesite=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; __lt__cid=e556f3ba-9714-4800-8871-b6dd760e306e; GUID=2f161e2e-a76f-4d3f-aa66-a70a98ee9027; _fbp=fb.1.1695204836875.1699548327; ai_user=xAmJC|2023-09-20T10:15:23.828Z; isPromotion21194AlreadyPrompt=true; allowGetPrivacyInfo=true; _ga=GA1.1.78749efe-cdd4-4b44-9c28-6ff421d016af; _ga=GA1.2.78749efe-cdd4-4b44-9c28-6ff421d016af; isPromotion21302AlreadyPrompt=true; isPromotion20938AlreadyPrompt=true; isPromotion21316AlreadyPrompt=true; auth=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; auth_samesite=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; MID=5600747285; isPromotion19570AlreadyPrompt=true; fr=; fr2=; isPromotion19213AlreadyPrompt=true; 91_FPID_v3_4_1=34cade3b34f722e5b9e6e12c1c118917; lang=zh-TW; _clck=di1mzx|2|ffl|0|1358; currency=TWD; __lt__sid=05cc1235-18d04e2a; salePageViewList=885704,888603; _gat=1; ai_session=njNbf|1696466303616|1696468500403; _clsk=iqwqqt|1696468500734|6|1|s.clarity.ms/collect";

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
                    order.status = "已結團"; 
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
