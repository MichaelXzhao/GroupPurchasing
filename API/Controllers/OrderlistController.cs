using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Globalization;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderlistController : ControllerBase
    {
        private readonly GoShopContext _dbcontext; 

        public OrderlistController(GoShopContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // 支付型態轉換
        public static string ConvertPaymentType(string paymentType)
        {
            switch (paymentType)
            {
                case "CashOnDelivery":
                    return "貨到付款";
                case "CreditCardOnce":
                    return "信用卡結帳";
                default:
                    return paymentType; 
            }
        }


         string cookie = "gaClientId=78749efe-cdd4-4b44-9c28-6ff421d016af; uAUTH=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; uAUTH_samesite=8qkUhcWXMIjVVrT+RAMOHGKUMJxUCpvi8czmdDEGgCdOVwZ3YmMZ6AMeo9g9F6e1VrlhWDfbsDqJzdJJK4dWfHlgeGMR+FDxoiPPtDFjqAg=; __lt__cid=e556f3ba-9714-4800-8871-b6dd760e306e; GUID=2f161e2e-a76f-4d3f-aa66-a70a98ee9027; _fbp=fb.1.1695204836875.1699548327; ai_user=xAmJC|2023-09-20T10:15:23.828Z; isPromotion21194AlreadyPrompt=true; allowGetPrivacyInfo=true; _ga=GA1.1.78749efe-cdd4-4b44-9c28-6ff421d016af; _ga=GA1.2.78749efe-cdd4-4b44-9c28-6ff421d016af; isPromotion21302AlreadyPrompt=true; isPromotion20938AlreadyPrompt=true; isPromotion21316AlreadyPrompt=true; auth=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; auth_samesite=od+XMJjXjeNCLaky+2jWL6ydbCxapZDcTG1yEZHNKtqOe/roW9An/SQ316lhHcLEFF1rS11i40OiI2O3g5NVnjlNRrNWqlA3x/s8oFk3Ihf1QBG5gzGTeUyq+H/1O4FSEGJRRfr/65pgRKjIDUt7KAmckXRz4I9vVv5vliljLBsl1WdIHR4ibUh539lSR9YPXi2MiQOzQz3nH6PkFuV6lfJQmKQOwOpVYx/UzPILZFfMdlKp/Nf4d/iGMDm4oUQRagHLH25qM4kk7PShaHs8C7f5QEfbpgJdR3khZ8Hag+stXHb61DAxMtF8wq9uh3lrB/KmZ1Xic3tFqbrlGrf80GwNfOrJuwU2X7ms/b25yqCkKlv2xlehktUBmhwN6hZWkGwr0zMSN4wG+W90DEymTh4CoiQFrNfaVLov4rXoKFmY7jov1wUUCyWlj6V4+FRf0/nU507NRklGnW6ixO5r1mu77igPFQdvPNxlIIhIOyRZAkODcD+XRSy1aBHlAImpLS47q6n6KpYp8Iu/mhabj8nhRJXKxU/zkNBGB+wuznmAlCPRkD2pnCjgvVfHwZ/BDhPGTyZfNcJocg9iiGpMNPiwMJRZshV4OyfjCb8tXospN80qQbfULGabGeGs8D/E7OphfB8CfYGFC3nHLsfv1rPFZE37pvsL44fKlLtemminsXdwnpQYALgMeU2ByLRGPd82+WcKCnYKUnohoduxJbDL2ZKPhZuNkI0Y45Jd+2CfNPo8YIhqdBKsU3k0KSYXpR588W6A4V6+F6H/KJoZcSt/q/zndigR8PN+ZWZkNMs=; MID=5600747285; isPromotion19570AlreadyPrompt=true; fr=; fr2=; isPromotion19213AlreadyPrompt=true; 91_FPID_v3_4_1=34cade3b34f722e5b9e6e12c1c118917; lang=zh-TW; _clck=di1mzx|2|ffl|0|1358; currency=TWD; __lt__sid=05cc1235-18d04e2a; salePageViewList=885704,888603; _gat=1; ai_session=njNbf|1696466303616|1696468500403; _clsk=iqwqqt|1696468500734|6|1|s.clarity.ms/collect";

        [HttpGet("Orderlist")]
        public async Task<ActionResult> OrderList()
        {
            HttpClient clientCheckOrder = new HttpClient();
            string payload = @"{
                ""shopId"": 10230
            }";

            HttpContent CheckOrdercontent = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpRequestMessage requestCheckOrder = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://10230.shop.qa.91dev.tw/webapi/MemberTradesOrder/GetList?shopId=10230&startIndex=0&maxCount=5&lang=zh-TW"), // Remove shopId from the query parameters
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

            JObject jsonCheckOrderResponse = JObject.Parse(responseCheckOrderContent);

            List<object> orderInfos = new List<object>();

            for(int orderlistnum=0 ; orderlistnum<5 ; orderlistnum++)
            {
                string OrderCode = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][orderlistnum]["Code"].ToString();
                string Title = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][orderlistnum]["TradesOrderList"][0]["TradesOrderSlaveList"][0]["SaleProductTitle"].ToString();
                string Picture = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][orderlistnum]["TradesOrderList"][0]["TradesOrderSlaveList"][0]["PicUrl"].ToString();
                string TotalPrice = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][orderlistnum]["TradesOrderList"][0]["TradesOrderSlaveList"][0]["TotalPayment"].ToString();
                string PayType = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][orderlistnum]["PayProfileTypeDef"].ToString();
                string Date = jsonCheckOrderResponse["Data"]["TradesOrderGroupList"][orderlistnum]["DateTime"].ToString();
            
                var orderInfo = new
                {
                    orderCode = OrderCode,
                    title = Title,
                    picture = Picture,
                    totalprice = TotalPrice,
                    paytype = ConvertPaymentType(PayType),
                    date = Date
                };
                
                orderInfos.Add(orderInfo);
            }

            return Ok(orderInfos);
        }
    }
}