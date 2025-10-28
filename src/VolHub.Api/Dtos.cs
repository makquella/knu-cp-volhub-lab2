// Файл src/VolHub.Api/Dtos.cs
namespace VolHub.Api;

using VolHub.Domain;

public record CreateRequestDto(string Title, string Location, int Priority);
public record UpdateRequestDto(string Title, string Location, int Priority, RequestStatus Status);
public record CreateVolunteerDto(string Name, string Phone);
public record AssignDto(Guid RequestId, Guid VolunteerId);
