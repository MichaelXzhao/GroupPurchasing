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
    public class DashboardController : ControllerBase
    {
        private readonly GoShopContext _dbcontext; 

        public DashboardController(GoShopContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // [HttpGet("Grouplist")]
        // public async Task<ActionResult> GetGroupList(string campaign, int estimate, string start, string finish, string salepageid)
        // {
        //     var Orders = await _dbcontext.Orders
        //         .Where(o => o.member == "host" && o.status == "unpaid" && o.salepageid == salepageid)
        //         .ToListAsync();

            

        //     foreach (var Order in Orders)
        //     {
        //         Order.campaign = input.campaign;
        //         Order.estimate = input.estimate;
        //         Order.start = DateTime.Parse(input.start);
        //         Order.finish = DateTime.Parse(input.finish);
        //     }                


        //     return Ok();
        // }

        [HttpPost("Leader")]
        public async Task<ActionResult> AddLeader([FromBody]AddLeaderDto input)
        {
            var Orders = await _dbcontext.Orders
                .Where(o => o.member == "host" && o.status == "unpaid" && o.salepageid == input.salepageid)
                .ToListAsync();

            foreach (var Order in Orders)
            {
                Order.campaign = input.campaign;
                Order.estimate = input.estimate;
                Order.start = DateTime.Parse(input.start);
                Order.finish = DateTime.Parse(input.finish);
            }                
            await _dbcontext.SaveChangesAsync();
            return Ok();
        }

    }
}