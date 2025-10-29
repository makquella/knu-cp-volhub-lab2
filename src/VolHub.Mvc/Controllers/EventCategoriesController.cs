using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Data;
using VolHub.Mvc.Models.ViewModels;

namespace VolHub.Mvc.Controllers;

public class EventCategoriesController : Controller
{
    private readonly AppDbContext _db;

    public EventCategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _db.EventCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var category = await _db.EventCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        var events = await _db.VolunteerEvents
            .AsNoTracking()
            .Where(e => e.EventCategoryId == id)
            .Include(e => e.Venue)
            .OrderBy(e => e.StartDateUtc)
            .Select(e => new VolunteerEventSummary
            {
                Id = e.Id,
                Title = e.Title,
                Category = category.Name,
                Venue = $"{e.Venue.Name}, {e.Venue.City}",
                StartDateUtc = e.StartDateUtc,
                EndDateUtc = e.EndDateUtc
            })
            .ToListAsync();

        var viewModel = new EventCategoryDetailsViewModel
        {
            Category = category,
            Events = events
        };

        return View(viewModel);
    }
}
