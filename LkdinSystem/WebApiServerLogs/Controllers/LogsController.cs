using Domain;
using Microsoft.AspNetCore.Mvc;
using WebApiServerLogs.Repositories;
using WebApiServerLogs.SearchCriterias;

namespace WebApiServerLogs.Controllers
{
    [ApiController]
    [Route("logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogger<LogsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Log> Get([FromQuery] LogSearchCriteria filters)
        {
            return LogsRepository.GetInstance().GetLogs(filters);
        }
    }
}