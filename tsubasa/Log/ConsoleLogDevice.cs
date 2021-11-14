using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsubasa.Log
{
    public class ConsoleLogDevice : ILogDevice
    {
        public string Name { get; set; }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
