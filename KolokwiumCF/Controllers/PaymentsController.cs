namespace KolokwiumCF.Controllers
{
    using global::KolokwiumCF.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;

    namespace KolokwiumCF.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class PaymentsController : ControllerBase
        {
            private readonly AppDbContext _context;

            public PaymentsController(AppDbContext context)
            {
                _context = context;
            }

            [HttpPost]
            public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
            {
                // Sprawdź czy klient istnieje
                var client = await _context.Clients.FindAsync(paymentRequest.IdClient);
                if (client == null)
                {
                    return NotFound("Klient o podanym Id nie istnieje.");
                }

                // Sprawdź czy subskrypcja istnieje i jest aktywna
                var subscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(s => s.IdSubscription == paymentRequest.IdSubscription && s.EndTime > DateTime.Now);
                if (subscription == null)
                {
                    return BadRequest("Podana subskrypcja nie istnieje lub nie jest aktywna.");
                }

                // Sprawdź czy klient nie opłacił już subskrypcji za ten okres
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.IdClient == paymentRequest.IdClient && p.IdSubscription == paymentRequest.IdSubscription);
                if (existingPayment != null)
                {
                    return Conflict("Klient już opłacił subskrypcję za ten okres.");
                }

                // Sprawdź czy wpłacana kwota jest zgodna z kwotą subskrypcji
                var subscriptionPrice = subscription.Price;
                if (paymentRequest.Payment != subscriptionPrice)
                {
                    return BadRequest("Wpłacana kwota nie jest zgodna z ceną subskrypcji.");
                }

                // Sprawdź czy istnieje aktywna zniżka przypisana do klienta
                var activeDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.IdClient == paymentRequest.IdClient && d.DateTo > DateTime.Now);
                if (activeDiscount != null)
                {
                    subscriptionPrice -= activeDiscount.Value;
                }

                // Dodaj rekord do tabeli Payment
                var payment = new Payment
                {
                    IdClient = paymentRequest.IdClient,
                    IdSubscription = paymentRequest.IdSubscription,
                    Value = subscriptionPrice,
                    Date = DateTime.Now
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Zwróć wygenerowaną wartość Id
                return Ok(new { PaymentId = payment.IdPayment });
            }
        }

        public class PaymentRequest
        {
            public int IdClient { get; set; }
            public int IdSubscription { get; set; }
            public decimal Payment { get; set; }
        }
    }

}
