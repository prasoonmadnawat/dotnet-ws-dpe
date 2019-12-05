using System;
using System.Threading.Tasks;
using CoffeeApi.Connectors;
using CoffeeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace CoffeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CoffeeController
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<CoffeeController> _logger;

        public CoffeeController(ILogger<CoffeeController> logger, IMessagePublisher messagePublisher)
        {
            this._logger = logger;
            this._messagePublisher = messagePublisher;
        }

        [HttpPost]
        public async Task<string> Post([FromBody] CoffeeOrder coffeeOrder)
        {
            try
            {
                await this._messagePublisher.PublishMessageAsync($"{coffeeOrder.CoffeeType} {coffeeOrder.Sugar} {coffeeOrder.MilkType}");
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}