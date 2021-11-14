namespace tsubasa.Log;

public interface ILogger
{
    ILogger WithService(ILogService service);
    ILogger WithDevice(ILogDevice dervice);
    ILogger WithDevices(List<ILogDevice> dervices);

    void Log(string message);
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
}
