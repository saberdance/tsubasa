namespace tsubasa.Log;

using System.Threading.Channels;
using System.Threading.Tasks;

public class LogService : ILogService
{
    private Channel<LogRequest> _channel = System.Threading.Channels.Channel.CreateUnbounded<LogRequest>();
    private bool _running = false;
    private bool _started = false;

    public Channel<LogRequest> Channel => _channel;
    public bool Running => _running;

    public void Start()
    {
        if (_started) return;
        _started = true;
        _running = true;
        Task.Run(() =>
        {
            while (true)
            {
                if (_running)
                {
                    LogRequest logRequest;
                    var succ = Channel.Reader.TryRead(out logRequest);
                    if (succ)
                    {
                        logRequest.device.ForEach(dev => dev.Write(logRequest.message));
                    }
                    else
                    {
                        //缓冲，避免空channel重复读取
                        Task.Delay(10).Wait();
                    }
                }
            }        
        });
    }

    public void Resume() => _running = true;

    public void Stop() => _running = false;
}
