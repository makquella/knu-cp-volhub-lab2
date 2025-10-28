namespace VolHub.Domain;

public enum RequestStatus { New, InProgress, Done, Cancelled }

public record Request(
    Guid Id,
    string Title,
    string Location,
    int Priority,
    RequestStatus Status,
    DateTime CreatedAt
);

public record Volunteer(
    Guid Id,
    string Name,
    string Phone
);

public record Assignment(
    Guid RequestId,
    Guid VolunteerId,
    DateTime AssignedAt
);