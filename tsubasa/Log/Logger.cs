using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsubasa.Log
{
    public record Logger : ILogger
    {
        private ILogService logService;
        private List<ILogDevice> _devices = new();

        public ILogger WithService(ILogService service)
        {
            return this with { logService = service };
        }

        public ILogger WithDevice(ILogDevice device)
        {
            return this with { _devices = new List<ILogDevice> { device} };
        }

        public ILogger WithDevices(List<ILogDevice> devices)
        {
            return this with { _devices = devices };
        }

        public void Debug(string message)
        {
            CallLogService(GenFormattedTimeString("DEBUG", message));
        }

        public void Error(string message)
        {
            CallLogService(GenFormattedTimeString("ERROR", message));
        }

        public void Info(string message)
        {
            CallLogService(GenFormattedTimeString("INFO", message));
        }

        public void Log(string message)
        {
            CallLogService(GenFormattedTimeString("LOG", message));
        }

        public void Warn(string message)
        {
            CallLogService(GenFormattedTimeString("WARN", message));
        }

        private string GenFormattedTimeString(string level,string message)
        {
            return $"[{level}][{string.Format("{0:d}", DateTime.Now).Replace('/', '_')}]{message}";
        }

        private void CallLogService(string message)
        {
            logService.Channel.Writer.TryWrite(new() { device = _devices ,message = message});
        }
    }
}
