using API_Aggregation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_Aggregation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string location, [FromQuery] string query)
        {
            var data = await _aggregationService.GetAggregatedDataAsync(location, query);
            return Ok(data);
        }
    }
    
}
