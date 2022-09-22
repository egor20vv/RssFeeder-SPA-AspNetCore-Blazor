namespace AspNetBlazor_RssFeeder.Services;

public class AutoUpdater : IDisposable
{
    private Timer? _timer;

    public AutoUpdater() { }

    public async Task SetUpTimer(float intervalSeconds, Func<Task> updateAction)
    {
        await Task.Run(() =>
        {
            _timer = new Timer(async (s) =>
            {
                await updateAction.Invoke();
            }, null, (int)(1000 * intervalSeconds), (int)(1000 * intervalSeconds));
        });
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
