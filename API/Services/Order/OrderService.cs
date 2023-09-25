using API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace API.Services.Order
{
    public class OrderService : IOrderService
    {
        private string cookie = "gaClientId=aac6bbb6-42e8-4566-beb6-70b39ecccd46; __lt__cid=2178dfab-832a-424c-ad74-beab110fcd4f; uAUTH=KUgNJm2SrPlcG9oZpdbaEij5WqcNRzaS3IlESf4JfVYZiTFE/8OAb3ljMvqGTjKsoIxG47NEDSR0SNEGQq4w/Px3lVuGZHvp5cAxzWAUMbI=; uAUTH_samesite=KUgNJm2SrPlcG9oZpdbaEij5WqcNRzaS3IlESf4JfVYZiTFE/8OAb3ljMvqGTjKsoIxG47NEDSR0SNEGQq4w/Px3lVuGZHvp5cAxzWAUMbI=; _fbp=fb.1.1695024637093.1648731515; ai_user=neuP9|2023-09-18T08:11:11.189Z; GUID=8b7f9804-9726-4894-94f4-16c594486040; auth=od+XMJjXjeNCLaky+2jWL8+c+/Yqgxf2syxeW2oYUoF5ajC1dJZxe2d7qV2gK9+/mKN5XPlgy7iEU7KZpsuu12RBJYtTlImeuhEPKs8nf2lkLDvC1poe7lbPK9k/ZPcjZXP/eR51ijOD4Ql3pCDaKSrMi3oUoLYOJqLI5pQQ5JBofoUZjs5vnZ2iqnfZvIrNUl7NTvob1byrMdFvBMeKhB9NpV0/z6vVjhRD3fCqhCIBjM2uFzc5KtmM7qdgWIVNGa+ZiDIJsxHy8yoUcn0dM1W3SX1msZzglokOBiHG1aSgSo2drq6mjAIPqPZbwPAnlnYlAeNB+6IjKGDSn6d1ifSgrrztIzoEDBo8CaXyBiFgyHEWiF+jMe42MofgnyzhkHvI1yl8uNAwFU3qPJWtzZ6GtDRY/fezEPzZhoedAPZgcrWFlmHnmE3I4wIPcAH+RokEnsBGPMrBA6Czzlv6IXQEcrIkw52Piq+JcZiSVvuOMGYhR41MZaeHMAeBTqytZLUx4kkEwXa9DOUkQs3QuRuzbSqMmx70O9zwWW2pkZMKiZ94JDrJ+BWIn/tAqGqfGTHBuXzYpUvdtdKsHuxjFvWSfHvOqFzpuTZt6mEILqWdtta3vVwNJ5Z0d0b7Nk60jbqRYY1DSOX98vbI9Sv3a6wXAoAWZREyaMCfhAMXlSFzX810hUGuavcwZdfRC++8anoCzDWEOD7FmWJONOJ0L3ojI5GuJW340kQhHUU2UQkk2Z7Ga7sXgxLccyWwA0spFFn6O0z1RZY/vOrTC2pZ6bi9hhm9ymEdNS3bBOSrHUo=; auth_samesite=od+XMJjXjeNCLaky+2jWL8+c+/Yqgxf2syxeW2oYUoF5ajC1dJZxe2d7qV2gK9+/mKN5XPlgy7iEU7KZpsuu12RBJYtTlImeuhEPKs8nf2lkLDvC1poe7lbPK9k/ZPcjZXP/eR51ijOD4Ql3pCDaKSrMi3oUoLYOJqLI5pQQ5JBofoUZjs5vnZ2iqnfZvIrNUl7NTvob1byrMdFvBMeKhB9NpV0/z6vVjhRD3fCqhCIBjM2uFzc5KtmM7qdgWIVNGa+ZiDIJsxHy8yoUcn0dM1W3SX1msZzglokOBiHG1aSgSo2drq6mjAIPqPZbwPAnlnYlAeNB+6IjKGDSn6d1ifSgrrztIzoEDBo8CaXyBiFgyHEWiF+jMe42MofgnyzhkHvI1yl8uNAwFU3qPJWtzZ6GtDRY/fezEPzZhoedAPZgcrWFlmHnmE3I4wIPcAH+RokEnsBGPMrBA6Czzlv6IXQEcrIkw52Piq+JcZiSVvuOMGYhR41MZaeHMAeBTqytZLUx4kkEwXa9DOUkQs3QuRuzbSqMmx70O9zwWW2pkZMKiZ94JDrJ+BWIn/tAqGqfGTHBuXzYpUvdtdKsHuxjFvWSfHvOqFzpuTZt6mEILqWdtta3vVwNJ5Z0d0b7Nk60jbqRYY1DSOX98vbI9Sv3a6wXAoAWZREyaMCfhAMXlSFzX810hUGuavcwZdfRC++8anoCzDWEOD7FmWJONOJ0L3ojI5GuJW340kQhHUU2UQkk2Z7Ga7sXgxLccyWwA0spFFn6O0z1RZY/vOrTC2pZ6bi9hhm9ymEdNS3bBOSrHUo=; MID=5500747275; allowGetPrivacyInfo=true; lang=zh-TW; currency=TWD; fr=; fr2=; _ga=GA1.1.aac6bbb6-42e8-4566-beb6-70b39ecccd46; _ga=GA1.2.aac6bbb6-42e8-4566-beb6-70b39ecccd46; _clck=1gwpfc9|2|ff8|0|1356; __lt__sid=911bb3e4-3d46449e; 91_FPID_v3_4_1=bba2f0d057c32f7f3e3043895830d1c5; salePageViewList=888347,887456,888248,884643,885038,885035,885238,885704; _gat=1; _clsk=1e9tsou|1695364012509|21|1|s.clarity.ms/collect; ai_session=a327H|1695363102788|1695364015879.4";
        
        private readonly GoShopContext _dbContext;

        public OrderService(GoShopContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> GetCartDetail()
        {
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

            return cartUniqueKeyValue;
        }

        // 獲取"cartUniqueKey"對應值的方法
        private static string GetCartUniqueKeyValue(JToken token)
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
    }
}
