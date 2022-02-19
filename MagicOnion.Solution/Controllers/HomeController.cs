using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagicOnion.Solution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public HomeController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("FirstService")]
        public async Task<string> Get()
        {
            try
            {
                // Connect to the server using gRPC channel.
                var channel = GrpcChannel.ForAddress("http://localhost:5001");

                // Create a proxy to call the server transparently.
                var client = MagicOnionClient.Create<IMyFirstService>(channel);

                var result = await client.SumAsync(123, 456);

                return result.ToString();
            }catch(Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
