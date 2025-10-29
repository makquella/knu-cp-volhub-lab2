using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Data;
using VolHub.Mvc.Models;

namespace VolHub.Mvc.Controllers;

public class VolunteerEventsController : Controller
{
    private readonly AppDbContext _db;

    public VolunteerEventsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var events = await _db.VolunteerEvents
            .AsNoTracking()
            .Include(e => e.EventCategory)
            .Include(e => e.Venue)
            .OrderBy(e => e.StartDateUtc)
            .ToListAsync();

        return View(events);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var volunteerEvent = await _db.VolunteerEvents
            .AsNoTracking()
            .Include(e => e.EventCategory)
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (volunteerEvent is null)
        {
            return NotFound();
        }

        return View(volunteerEvent);
    }
}
