using System.Threading.Tasks;
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

        /// <summary>
        /// Get aggregated data from various sources.
        /// </summary>
        /// <param name="location">The location for weather data (e.g., city name).</param>
        /// <param name="query">The search query for Twitter, News, Spotify, and GitHub data.</param>
        /// <param name="fromDate">The start date for filtering data (YYYY-MM-DD).</param>
        /// <param name="toDate">The end date for filtering data (YYYY-MM-DD).</param>
        /// <param name="sortBy">The field to sort by (e.g., relevance, date).</param>
        /// <returns>Aggregated data from multiple APIs.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string location, [FromQuery] string query, [FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string sortBy)
        {
            var data = await _aggregationService.GetAggregatedDataAsync(location, query/*, fromDate, toDate, sortBy*/);
            return Ok(data);
        }
    }
}
