namespace VolHub.Domain;

public interface IAssignmentService
{
    void EnsureCanAssign(Request request);
    Request Start(Request request);
    Request Complete(Request request);
}

public class AssignmentService : IAssignmentService
{
    public void EnsureCanAssign(Request request)
    {
        if (request.Status is RequestStatus.Done or RequestStatus.Cancelled)
            throw new InvalidOperationException("Cannot assign a closed request.");
    }

    public Request Start(Request request)
    {
        if (request.Status != RequestStatus.New)
            throw new InvalidOperationException("Only NEW requests can be started.");
        return request with { Status = RequestStatus.InProgress };
    }

    public Request Complete(Request request)
    {
        if (request.Status != RequestStatus.InProgress)
            throw new InvalidOperationException("Only IN_PROGRESS requests can be completed.");
        return request with { Status = RequestStatus.Done };
    }
}