using FeuerwehrUpdates.Models;
using FeuerwehrUpdates.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeuerwehrUpdates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EinsatzVerlaufController : ControllerBase
    {

        private readonly FWUpdatesDbContext _context;
        private readonly ILogger<EinsatzVerlaufController> _logger;

        public EinsatzVerlaufController(FWUpdatesDbContext context, ILogger<EinsatzVerlaufController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Einsatz>>> Get()
        {
            return Ok(_context.Einsaetze.AsEnumerable());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Einsatz>> GetById(Guid id)
        {
            return Ok(_context.Einsaetze.Find(id));
        }

    }
}
