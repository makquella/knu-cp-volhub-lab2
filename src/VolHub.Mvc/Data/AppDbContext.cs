using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Models;

namespace VolHub.Mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> o) : base(o) { }

    public DbSet<AppUserProfile> Profiles => Set<AppUserProfile>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VolunteerEvent> VolunteerEvents => Set<VolunteerEvent>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<AppUserProfile>()
          .HasIndex(p => p.Username)
          .IsUnique();
        mb.Entity<AppUserProfile>()
          .HasIndex(p => p.Email)
          .IsUnique();

        mb.Entity<EventCategory>()
          .HasIndex(c => c.Name)
          .IsUnique();

        mb.Entity<Venue>()
          .HasIndex(v => new { v.Name, v.City })
          .IsUnique();

        mb.Entity<VolunteerEvent>()
          .HasIndex(e => e.StartDateUtc);

        mb.Entity<EventCategory>().HasData(
            new EventCategory { Id = 1, Name = "Екологічні акції", Description = "Прибирання парків, сортування відходів, зелені ініціативи." },
            new EventCategory { Id = 2, Name = "Освітні заходи", Description = "Лекції, майстер-класи та наставництво для громад." },
            new EventCategory { Id = 3, Name = "Соціальна підтримка", Description = "Допомога людям похилого віку, центрам тимчасового перебування та військовим." }
        );

        mb.Entity<Venue>().HasData(
            new Venue { Id = 1, Name = "Центральний парк", City = "Київ", Address = "вул. Шевченка, 12" },
            new Venue { Id = 2, Name = "Освітній простір «Knowledge Hub»", City = "Львів", Address = "пл. Ринок, 5", Room = "Зал 2" },
            new Venue { Id = 3, Name = "Волонтерський штаб «Разом»", City = "Дніпро", Address = "пр-т Гагаріна, 101" }
        );

        mb.Entity<VolunteerEvent>().HasData(
            new VolunteerEvent
            {
                Id = 1,
                Title = "Осіннє прибирання Дніпра",
                Summary = "Командне прибирання берега Дніпра з сортуванням зібраного пластику.",
                Organizer = "VolHub Community",
                StartDateUtc = new DateTime(2025, 11, 15, 6, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2025, 11, 15, 10, 0, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2025, 10, 1, 9, 0, 0, DateTimeKind.Utc),
                Capacity = 120,
                EventCategoryId = 1,
                VenueId = 1
            },
            new VolunteerEvent
            {
                Id = 2,
                Title = "Майстер-клас з цифрової грамотності",
                Summary = "Навчання базовим онлайн-сервісам для літніх людей та внутрішньо переміщених осіб.",
                Organizer = "Digital Volunteers UA",
                StartDateUtc = new DateTime(2025, 12, 2, 13, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2025, 12, 2, 16, 0, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2025, 10, 5, 8, 0, 0, DateTimeKind.Utc),
                Capacity = 45,
                EventCategoryId = 2,
                VenueId = 2
            },
            new VolunteerEvent
            {
                Id = 3,
                Title = "Збір теплих речей для центру підтримки",
                Summary = "Сортування, пакування та доставка теплого одягу до пункту підтримки.",
                Organizer = "Разом сильніші",
                StartDateUtc = new DateTime(2025, 12, 10, 7, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2025, 12, 10, 12, 0, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2025, 10, 12, 11, 0, 0, DateTimeKind.Utc),
                Capacity = 80,
                EventCategoryId = 3,
                VenueId = 3
            },
            new VolunteerEvent
            {
                Id = 4,
                Title = "Нічний велозаїзд з прибиранням",
                Summary = "Велосипедний патруль із прибиранням набережної та сортуванням сміття.",
                Organizer = "Cycle Impact",
                StartDateUtc = new DateTime(2026, 1, 20, 17, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 1, 20, 21, 0, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2025, 10, 20, 7, 30, 0, DateTimeKind.Utc),
                Capacity = 60,
                EventCategoryId = 1,
                VenueId = 1
            }
        );
    }
}
