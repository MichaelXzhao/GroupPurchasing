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


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly GoShopContext _dbcontext; 

        public DashboardController(GoShopContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        private string HashLink(string username)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(username));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        [HttpGet("Grouplist")]
        public async Task<ActionResult> GetGroupList()
        {
            var groupedData = _dbcontext.Orders
            .GroupBy(o => new { o.salepageid, o.campaign })
            .Select(group => new
            {
                SalePageId = group.Key.salepageid,
                Campaign = group.Key.campaign,
                Totaldiscount = group.Select(o => o.totaldiscount).FirstOrDefault(),
                Afterdiscount = group.Select(o => o.afterdiscount).FirstOrDefault(),
                Totalprice = group.Select(o => o.totalprice).FirstOrDefault(),
                Start = group.Select(o => o.start).FirstOrDefault(),
                Finish = group.Select(o => o.finish).FirstOrDefault(),
                Status = group.Select(o => o.status) .FirstOrDefault(),
                Membercount = group.Count(o => o.qty!=0 )
            })
            .ToList();

            return Ok(groupedData);
        }



        [HttpPost("Leader")]
        public async Task<ActionResult> AddLeader([FromBody]AddLeaderDto input)
        {
            var existingsalepageid = await _dbcontext.Orders
                    .FirstOrDefaultAsync(o => o.salepageid.Equals(input.salepageid) && o.status == "開團中");
            if(existingsalepageid==null){
                // (1) [GET] product/detail
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://10230.shop.qa.91dev.tw/webapi/SalePageV2/GetSalePageV2Info/10230/{input.salepageid}"),
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
                    member=input.user_name,
                    qty=0,
                    price=nineyi_price,
                    product=nineyi_title,
                    picture=nineyi_picurl,
                    salepageid=input.salepageid,
                    shopid=nineyi_shopid,
                    skuid=nineyi_sku,
                    recommender=null,
                    points=0,
                    status="開團中",
                    campaign = input.campaign,
                    start = input.start,
                    finish = input.finish,
                    sharelink=HashLink(input.user_name),
                    role="host" 
                };
                _dbcontext.Orders.Add(newOrders);
                await _dbcontext.SaveChangesAsync();              
            }
            return Ok();
        }

    }
}