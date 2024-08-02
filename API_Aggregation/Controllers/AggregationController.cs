using System.Threading.Tasks;
using API_Aggregation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_Aggregation.Controllers
{
    //used to designate a class as an API controller
    [ApiController]
    //the base route for the controller
    [Route("[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        /// <summary>
        /// Get aggregated data from various sources.
        /// </summary>
        /// <param name="location">The location for weather data (e.g., city name).</param>
        /// <param name="query">The search query for Twitter, News, Spotify, and GitHub/Country data.</param>
        /// <param name="fromDate">The start date for filtering data (YYYY-MM-DD).</param>
        /// <param name="toDate">The end date for filtering data (YYYY-MM-DD).</param>
        /// <param name="sortBy">The field to sort by (e.g., relevance, date).</param>
        /// 
        /// <returns>Aggregated data from multiple APIs.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string location, [FromQuery] string query, [FromQuery] bool dateTimeFiltering = false, [FromQuery] string fromDate = "2023-01-01T00:00:00Z", [FromQuery] string toDate = "2024-01-01T00:00:00Z")
        {
            var data = await _aggregationService.GetAggregatedDataAsync(location, query, dateTimeFiltering, fromDate, toDate);
            return Ok(data);
        }
    }
}
