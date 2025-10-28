using System.Collections.Concurrent;
using Microsoft.OpenApi.Models;
using VolHub.Domain;
using VolHub.Api; // доступ до DTO, оголошених у Dtos.cs

var builder = WebApplication.CreateBuilder(args);

// Реєстрація доменного сервісу
builder.Services.AddSingleton<IAssignmentService, AssignmentService>();

// Swagger-документація
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VolHub API", Version = "v1" });
});

var app = builder.Build();

// Увімкнено Swagger і редирект з кореня на інтерфейс документації
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger"));

// app.UseHttpsRedirection(); // розкоментуйте, якщо потрібен HTTPS у локальному середовищі

// In-memory сховища даних
var requests = new ConcurrentDictionary<Guid, Request>();
var volunteers = new ConcurrentDictionary<Guid, Volunteer>();
var assignments = new ConcurrentBag<Assignment>();

// Початкові записи для демонстрації
var r1 = new Request(Guid.NewGuid(), "Допомога з ліками", "Kyiv", 1, RequestStatus.New, DateTime.UtcNow);
requests[r1.Id] = r1;
var vol1 = new Volunteer(Guid.NewGuid(), "Олена", "+380000000001");
volunteers[vol1.Id] = vol1;

// Маршрути
app.MapGet("/api/requests", (RequestStatus? status, string? location, int? priority) =>
{
    var list = requests.Values.AsEnumerable();
    if (status.HasValue) list = list.Where(r => r.Status == status.Value);
    if (!string.IsNullOrWhiteSpace(location)) list = list.Where(r => r.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
    if (priority.HasValue) list = list.Where(r => r.Priority == priority.Value);
    return Results.Ok(list.OrderByDescending(r => r.CreatedAt));
});

app.MapGet("/api/requests/{id:guid}", (Guid id) =>
{
    return requests.TryGetValue(id, out var r) ? Results.Ok(r) : Results.NotFound();
});

app.MapPost("/api/requests", (CreateRequestDto dto) =>
{
    var req = new Request(Guid.NewGuid(), dto.Title, dto.Location, dto.Priority, RequestStatus.New, DateTime.UtcNow);
    requests[req.Id] = req;
    return Results.Created($"/api/requests/{req.Id}", req);
});

app.MapPut("/api/requests/{id:guid}", (Guid id, UpdateRequestDto dto) =>
{
    if (!requests.TryGetValue(id, out var existing)) return Results.NotFound();
    var updated = existing with
    {
        Title = dto.Title,
        Location = dto.Location,
        Priority = dto.Priority,
        Status = dto.Status
    };
    requests[id] = updated;
    return Results.Ok(updated);
});

app.MapDelete("/api/requests/{id:guid}", (Guid id) =>
{
    return requests.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/api/volunteers", () => Results.Ok(volunteers.Values));

app.MapPost("/api/volunteers", (CreateVolunteerDto dto) =>
{
    var v = new Volunteer(Guid.NewGuid(), dto.Name, dto.Phone);
    volunteers[v.Id] = v;
    return Results.Created($"/api/volunteers/{v.Id}", v);
});

app.MapPost("/api/assign", (AssignDto dto, IAssignmentService svc) =>
{
    if (!requests.TryGetValue(dto.RequestId, out var req)) return Results.NotFound($"Request {dto.RequestId} not found");
    if (!volunteers.ContainsKey(dto.VolunteerId)) return Results.NotFound($"Volunteer {dto.VolunteerId} not found");

    try
    {
        svc.EnsureCanAssign(req);
        assignments.Add(new Assignment(dto.RequestId, dto.VolunteerId, DateTime.UtcNow));
        return Results.Ok(new { message = "Assigned", dto.RequestId, dto.VolunteerId });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/requests/{id:guid}/start", (Guid id, IAssignmentService svc) =>
{
    if (!requests.TryGetValue(id, out var req)) return Results.NotFound();
    try
    {
        var started = svc.Start(req);
        requests[id] = started;
        return Results.Ok(started);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/requests/{id:guid}/complete", (Guid id, IAssignmentService svc) =>
{
    if (!requests.TryGetValue(id, out var req)) return Results.NotFound();
    try
    {
        var done = svc.Complete(req);
        requests[id] = done;
        return Results.Ok(done);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/stats", () =>
{
    var total = requests.Count;
    var byStatus = requests.Values.GroupBy(r => r.Status)
        .ToDictionary(g => g.Key.ToString(), g => g.Count());
    return Results.Ok(new { total, byStatus });
});

app.Run();
