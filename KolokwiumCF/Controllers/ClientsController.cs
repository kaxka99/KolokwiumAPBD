using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KolokwiumCF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientWithSubscriptions(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Sales)
                .ThenInclude(s => s.Subscription)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.IdClient == id);

            if (client == null)
            {
                return NotFound();
            }

            var result = new
            {
                firstName = client.FirstName,
                lastName = client.LastName,
                email = client.Email,
                phone = client.Phone,
                subscriptions = client.Sales.GroupBy(s => s.IdSubscription)
                    .Select(g => new
                    {
                        IdSubscription = g.Key,
                        Name = g.First().Subscription.Name,
                        RenewalPeriod = g.First().Subscription.RenewalPeriod,
                        TotalPaidAmount = client.Payments
                            .Where(p => p.IdSubscription == g.Key)
                            .Sum(p => p.Value)
                    }).ToList()
            };

            return Ok(result);
        }
    }

}
