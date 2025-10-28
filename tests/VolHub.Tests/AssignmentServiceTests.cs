using VolHub.Domain;
using Xunit;

namespace VolHub.Tests;

public class AssignmentServiceTests
{
    private readonly AssignmentService _svc = new();

    [Fact]
    public void EnsureCanAssign_Allows_New()
    {
        var r = new Request(Guid.NewGuid(), "Test", "Kyiv", 1, RequestStatus.New, DateTime.UtcNow);
        var ex = Record.Exception(() => _svc.EnsureCanAssign(r));
        Assert.Null(ex);
    }

    [Fact]
    public void EnsureCanAssign_Denies_Closed()
    {
        var r = new Request(Guid.NewGuid(), "Test", "Kyiv", 1, RequestStatus.Done, DateTime.UtcNow);
        Assert.Throws<InvalidOperationException>(() => _svc.EnsureCanAssign(r));
    }

    [Fact]
    public void Start_OnlyFrom_New()
    {
        var r = new Request(Guid.NewGuid(), "Test", "Kyiv", 1, RequestStatus.New, DateTime.UtcNow);
        var started = _svc.Start(r);
        Assert.Equal(RequestStatus.InProgress, started.Status);

        var notNew = started;
        Assert.Throws<InvalidOperationException>(() => _svc.Start(notNew));
    }

    [Fact]
    public void Complete_OnlyFrom_InProgress()
    {
        var r = new Request(Guid.NewGuid(), "Test", "Kyiv", 1, RequestStatus.New, DateTime.UtcNow);
        var started = _svc.Start(r);
        var done = _svc.Complete(started);
        Assert.Equal(RequestStatus.Done, done.Status);

        Assert.Throws<InvalidOperationException>(() => _svc.Complete(done));
    }
}