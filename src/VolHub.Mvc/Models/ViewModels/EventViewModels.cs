using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VolHub.Mvc.Models.ViewModels;

public class EventCategoryDetailsViewModel
{
    public EventCategory Category { get; init; } = null!;
    public IReadOnlyList<VolunteerEventSummary> Events { get; init; } = Array.Empty<VolunteerEventSummary>();
}

public class VenueDetailsViewModel
{
    public Venue Venue { get; init; } = null!;
    public IReadOnlyList<VolunteerEventSummary> Events { get; init; } = Array.Empty<VolunteerEventSummary>();
}

public class VolunteerEventSummary
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Venue { get; init; } = string.Empty;
    public DateTime StartDateUtc { get; init; }
    public DateTime EndDateUtc { get; init; }
}

public class EventSearchForm
{
    [Display(Name = "Початок від")]
    [DataType(DataType.DateTime)]
    public DateTime? StartFrom { get; set; }

    [Display(Name = "Початок до")]
    [DataType(DataType.DateTime)]
    public DateTime? StartTo { get; set; }

    [Display(Name = "Категорії")]
    public List<int> CategoryIds { get; set; } = new();

    [Display(Name = "Назва починається з")]
    public string? TitleStartsWith { get; set; }

    [Display(Name = "Організатор закінчується на")]
    public string? OrganizerEndsWith { get; set; }
}

public class EventSearchViewModel
{
    public EventSearchForm Form { get; init; } = new();
    public IReadOnlyList<VolunteerEventSummary> Results { get; init; } = Array.Empty<VolunteerEventSummary>();
    public IReadOnlyList<SelectListItem> Categories { get; init; } = Array.Empty<SelectListItem>();
}
