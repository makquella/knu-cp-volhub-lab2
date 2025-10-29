using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Data;
using VolHub.Mvc.Models.ViewModels;

namespace VolHub.Mvc.Controllers;

public class EventSearchController : Controller
{
    private const int ResultLimit = 100;

    private readonly AppDbContext _db;

    public EventSearchController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] EventSearchForm form)
    {
        form.CategoryIds ??= new List<int>();

        var categories = await _db.EventCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        var query = _db.VolunteerEvents
            .AsNoTracking()
            .Include(e => e.EventCategory)
            .Include(e => e.Venue)
            .AsQueryable();

        if (form.StartFrom.HasValue)
        {
            var fromUtc = EnsureUtc(form.StartFrom.Value);
            query = query.Where(e => e.StartDateUtc >= fromUtc);
        }

        if (form.StartTo.HasValue)
        {
            var toUtc = EnsureUtc(form.StartTo.Value);
            query = query.Where(e => e.StartDateUtc <= toUtc);
        }

        if (form.CategoryIds.Count > 0)
        {
            query = query.Where(e => form.CategoryIds.Contains(e.EventCategoryId));
        }

        if (!string.IsNullOrWhiteSpace(form.TitleStartsWith))
        {
            var prefix = form.TitleStartsWith.Trim();
            query = query.Where(e => EF.Functions.Like(e.Title, prefix + "%"));
        }

        if (!string.IsNullOrWhiteSpace(form.OrganizerEndsWith))
        {
            var suffix = form.OrganizerEndsWith.Trim();
            query = query.Where(e => EF.Functions.Like(e.Organizer, "%" + suffix));
        }

        var results = await query
            .OrderBy(e => e.StartDateUtc)
            .Take(ResultLimit)
            .Select(e => new VolunteerEventSummary
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.EventCategory.Name,
                Venue = $"{e.Venue.Name}, {e.Venue.City}",
                StartDateUtc = e.StartDateUtc,
                EndDateUtc = e.EndDateUtc
            })
            .ToListAsync();

        var viewModel = new EventSearchViewModel
        {
            Form = new EventSearchForm
            {
                StartFrom = form.StartFrom,
                StartTo = form.StartTo,
                CategoryIds = form.CategoryIds.ToList(),
                TitleStartsWith = form.TitleStartsWith,
                OrganizerEndsWith = form.OrganizerEndsWith
            },
            Results = results,
            Categories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = form.CategoryIds.Contains(c.Id)
                })
                .ToList()
        };

        return View(viewModel);
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        if (value.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime();
        }

        return value.ToUniversalTime();
    }
}
