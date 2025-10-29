using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Data;
using VolHub.Mvc.Models.ViewModels;

namespace VolHub.Mvc.Controllers;

public class VenuesController : Controller
{
    private readonly AppDbContext _db;

    public VenuesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var venues = await _db.Venues
            .AsNoTracking()
            .OrderBy(v => v.City)
            .ThenBy(v => v.Name)
            .ToListAsync();

        return View(venues);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var venue = await _db.Venues
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venue is null)
        {
            return NotFound();
        }

        var events = await _db.VolunteerEvents
            .AsNoTracking()
            .Where(e => e.VenueId == id)
            .Include(e => e.EventCategory)
            .OrderBy(e => e.StartDateUtc)
            .Select(e => new VolunteerEventSummary
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.EventCategory.Name,
                Venue = $"{venue.Name}, {venue.City}",
                StartDateUtc = e.StartDateUtc,
                EndDateUtc = e.EndDateUtc
            })
            .ToListAsync();

        var viewModel = new VenueDetailsViewModel
        {
            Venue = venue,
            Events = events
        };

        return View(viewModel);
    }
}
