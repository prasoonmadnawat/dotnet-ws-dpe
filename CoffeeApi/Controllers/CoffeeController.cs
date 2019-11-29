using System.Threading.Tasks;
using CoffeeApi.Connectors;
using CoffeeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoffeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task Post([FromBody] CoffeeOrder coffeeOrder)
        {
            await this._messagePublisher.PublishMessageAsync($"{coffeeOrder.CoffeeType} {coffeeOrder.Sugar} {coffeeOrder.MilkType}");
        }
    }
}