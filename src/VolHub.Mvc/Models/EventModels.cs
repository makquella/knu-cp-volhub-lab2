using System.ComponentModel.DataAnnotations;

namespace VolHub.Mvc.Models;

public class EventCategory
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва обов'язкова.")]
    [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "Опис не може перевищувати 400 символів.")]
    public string? Description { get; set; }

    public ICollection<VolunteerEvent> Events { get; set; } = new HashSet<VolunteerEvent>();
}

public class Venue
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва локації обов'язкова.")]
    [StringLength(120, ErrorMessage = "Назва локації не може перевищувати 120 символів.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Місто обов'язкове.")]
    [StringLength(80, ErrorMessage = "Місто не може перевищувати 80 символів.")]
    public string City { get; set; } = string.Empty;

    [StringLength(160, ErrorMessage = "Адреса не може перевищувати 160 символів.")]
    public string? Address { get; set; }

    [StringLength(80, ErrorMessage = "Додаткові вказівки не можуть перевищувати 80 символів.")]
    public string? Room { get; set; }

    public ICollection<VolunteerEvent> Events { get; set; } = new HashSet<VolunteerEvent>();
}

public class VolunteerEvent
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва події обов'язкова.")]
    [StringLength(160, ErrorMessage = "Назва події не може перевищувати 160 символів.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опис події обов'язковий.")]
    [StringLength(1000, ErrorMessage = "Опис не може перевищувати 1000 символів.")]
    public string Summary { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ім'я організатора обов'язкове.")]
    [StringLength(120, ErrorMessage = "Ім'я організатора не може перевищувати 120 символів.")]
    public string Organizer { get; set; } = string.Empty;

    [Display(Name = "Дата початку")]
    [DataType(DataType.DateTime)]
    public DateTime StartDateUtc { get; set; }

    [Display(Name = "Дата завершення")]
    [DataType(DataType.DateTime)]
    public DateTime EndDateUtc { get; set; }

    [Range(1, 10000, ErrorMessage = "Місткість має бути в діапазоні 1-10000.")]
    public int Capacity { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedUtc { get; set; }

    public int EventCategoryId { get; set; }
    public EventCategory EventCategory { get; set; } = null!;

    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
}
