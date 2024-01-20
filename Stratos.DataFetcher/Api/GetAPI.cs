using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Stratos.DataFetcher.Api
{
    [ApiController]
    [Route("/api/changestate")]
    public class GetAPI
    {
        [HttpGet]
        public IActionResult Get([FromBody] string value)
        {
            // You can handle the received POST data here
            // For simplicity, let's just return the received value
            return Ok
        }
    }
}
