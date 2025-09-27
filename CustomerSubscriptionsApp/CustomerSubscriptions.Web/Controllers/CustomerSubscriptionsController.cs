using CustomerSubscriptions.Web.Data;
using CustomerSubscriptions.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerSubscriptions.Web.Controllers;

[Authorize]
public class CustomerSubscriptionsController : Controller
{
    private readonly ApplicationDbContext _db;

    public CustomerSubscriptionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? customerId)
    {
        var query = _db.CustomerSubscriptions.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(customerId))
        {
            query = query.Where(x => x.CustomerId == customerId);
        }
        var items = await query.OrderBy(x => x.CustomerId).ThenBy(x => x.SubscriptionName).ToListAsync();
        return View(items);
    }

    public IActionResult Create()
    {
        return View(new CustomerSubscription { IsActive = true, SubscriptionCount = 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerSubscription model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.CustomerSubscriptions.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.CustomerSubscriptions.FindAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerSubscription model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.CustomerSubscriptions.FindAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _db.CustomerSubscriptions.FindAsync(id);
        if (entity == null) return NotFound();
        _db.CustomerSubscriptions.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

