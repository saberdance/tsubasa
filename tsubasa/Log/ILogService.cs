namespace tsubasa.Log;

using System.Threading.Channels;

public class LogRequest
{
    public List<ILogDevice> device;
    public string message;
}

public interface ILogService
{
    Channel<LogRequest> Channel { get; }
    void Start();
    void Stop();
    void Resume();
    bool Running { get; }
}
