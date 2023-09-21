using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
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
        [HttpGet]
        [Route("/Carts/Create")]
        public async Task<IActionResult> Create()
        {
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
                        { "Cookie", "lang=zh-TW; gaClientId=78749efe-cdd4-4b44-9c28-6ff421d016af; uAUTH=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; uAUTH_samesite=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; __lt__cid=e556f3ba-9714-4800-8871-b6dd760e306e; currency=TWD; 91_FPID_v3_4_1=34cade3b34f722e5b9e6e12c1c118917; GUID=2f161e2e-a76f-4d3f-aa66-a70a98ee9027; _fbp=fb.1.1695204836875.1699548327; fr=; fr2=; ai_user=xAmJC|2023-09-20T10:15:23.828Z; auth=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; auth_samesite=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; MID=5600747285; salePageViewList=888345,885704; _clck=di1mzx|2|ff7|0|1358; __lt__sid=05cc1235-8ba43fae; isPromotion21194AlreadyPrompt=true; ai_session=w6hKv|1695264712563|1695265925582.4; _clsk=1vs27sc|1695265925871|10|1|s.clarity.ms/collect" },
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
                        { "Cookie", "lang=zh-TW; gaClientId=78749efe-cdd4-4b44-9c28-6ff421d016af; uAUTH=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; uAUTH_samesite=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; __lt__cid=e556f3ba-9714-4800-8871-b6dd760e306e; currency=TWD; 91_FPID_v3_4_1=34cade3b34f722e5b9e6e12c1c118917; GUID=2f161e2e-a76f-4d3f-aa66-a70a98ee9027; _fbp=fb.1.1695204836875.1699548327; fr=; fr2=; ai_user=xAmJC|2023-09-20T10:15:23.828Z; auth=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; auth_samesite=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; MID=5600747285; salePageViewList=888345,885704; _clck=di1mzx|2|ff7|0|1358; __lt__sid=05cc1235-8ba43fae; isPromotion21194AlreadyPrompt=true; ai_session=w6hKv|1695264712563|1695265925582.4; _clsk=1vs27sc|1695265925871|10|1|s.clarity.ms/collect" },
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


                // (3)[grt API] checkout
                // HttpClient clientCheckoutCreate = new HttpClient();
                // HttpRequestMessage requestCheckoutCreate = new HttpRequestMessage()
                // {
                //     Method = HttpMethod.Post,
                //     RequestUri = new Uri("https://10230.shop.qa.91dev.tw/shopping/api/checkout"),
                //     Headers =
                //     {
                //         { "N1-SHOP-ID", "10230" },
                //         { "Cookie", "lang=zh-TW; gaClientId=78749efe-cdd4-4b44-9c28-6ff421d016af; uAUTH=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; uAUTH_samesite=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; __lt__cid=e556f3ba-9714-4800-8871-b6dd760e306e; currency=TWD; 91_FPID_v3_4_1=34cade3b34f722e5b9e6e12c1c118917; GUID=2f161e2e-a76f-4d3f-aa66-a70a98ee9027; _fbp=fb.1.1695204836875.1699548327; fr=; fr2=; ai_user=xAmJC|2023-09-20T10:15:23.828Z; auth=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; auth_samesite=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; MID=5600747285; salePageViewList=888345,885704; _clck=di1mzx|2|ff7|0|1358; __lt__sid=05cc1235-8ba43fae; isPromotion21194AlreadyPrompt=true; ai_session=w6hKv|1695264712563|1695265925582.4; _clsk=1vs27sc|1695265925871|10|1|s.clarity.ms/collect" },
                //         { "N1-HOST", "10230.shop.qa.91dev.tw" },
                //     },
                // };

                // string checkoutRequestBody = "{\"cartUniqueKey\": \"" + cartUniqueKeyValue + "\"}";
                // requestCheckoutCreate.Content = new StringContent(checkoutRequestBody, Encoding.UTF8, "application/json");

                // HttpResponseMessage responseCheckoutCreate = await client.SendAsync(requestCheckoutCreate);

                // // 讀取回傳response
                // string responseCheckoutCreateContent = await responseCheckoutCreate.Content.ReadAsStringAsync();

                // // 解析JSON response 為 JObject
                // JObject CheckoutCreatejsonResponse = JObject.Parse(responseCheckoutCreateContent);

                // // 獲取"uniqueKey"對應的值
                // string uniqueKey = GetUniqueKeyValue(CheckoutCreatejsonResponse);

                // Console.WriteLine("uniqueKey: " + uniqueKey);


            }
            catch (Exception ex) 
            {

            }
            return Ok();
        }
    }
}
