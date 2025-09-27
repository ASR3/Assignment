using System.ComponentModel.DataAnnotations;
using CustomerSubscriptions.Web.Data;
using CustomerSubscriptions.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerSubscriptions.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public SubscriptionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET api/subscriptions?customerId=...&subscriptionName=...&start=...&end=...
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerSubscription>>> Get(
        [FromQuery][Required] string customerId,
        [FromQuery] string? subscriptionName,
        [FromQuery] DateOnly? start,
        [FromQuery] DateOnly? end)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return BadRequest(new { error = "CustomerId is required" });
        }

        IQueryable<CustomerSubscription> query = _db.CustomerSubscriptions.AsNoTracking()
            .Where(s => s.CustomerId == customerId);

        if (!string.IsNullOrWhiteSpace(subscriptionName))
        {
            query = query.Where(s => s.SubscriptionName == subscriptionName);
        }

        if (start.HasValue)
        {
            var sdt = start.Value;
            query = query.Where(s => s.StartDate == null || s.StartDate >= sdt);
        }
        if (end.HasValue)
        {
            var edt = end.Value;
            query = query.Where(s => s.EndDate == null || s.EndDate <= edt);
        }

        var results = await query.OrderBy(s => s.SubscriptionName).ToListAsync();
        return Ok(results);
    }

    // POST api/subscriptions
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CustomerSubscription>> Create(CustomerSubscription input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        _db.CustomerSubscriptions.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    // GET api/subscriptions/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerSubscription>> GetById(int id)
    {
        var entity = await _db.CustomerSubscriptions.FindAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    // PUT api/subscriptions/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CustomerSubscription input)
    {
        if (id != input.Id) return BadRequest(new { error = "Id mismatch" });
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var exists = await _db.CustomerSubscriptions.AnyAsync(s => s.Id == id);
        if (!exists) return NotFound();

        _db.Entry(input).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/subscriptions/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.CustomerSubscriptions.FindAsync(id);
        if (entity == null) return NotFound();
        _db.CustomerSubscriptions.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    public class CountUpdateRequest
    {
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        [Required]
        public string SubscriptionName { get; set; } = string.Empty;
        public int Delta { get; set; } = 1; // +1 or -1 typical
    }

    // POST api/subscriptions/count
    [HttpPost("count")]
    [Authorize]
    public async Task<ActionResult<CustomerSubscription>> UpdateCount([FromBody] CountUpdateRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var entity = await _db.CustomerSubscriptions
            .FirstOrDefaultAsync(s => s.CustomerId == request.CustomerId && s.SubscriptionName == request.SubscriptionName);
        if (entity == null) return NotFound(new { error = "Subscription not found" });

        var newCount = entity.SubscriptionCount + request.Delta;
        if (newCount < 0)
        {
            return BadRequest(new { error = "SubscriptionCount cannot be negative" });
        }

        entity.SubscriptionCount = newCount;
        await _db.SaveChangesAsync();
        return Ok(entity);
    }
}

