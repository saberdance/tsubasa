using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsubasa.Log
{
    public class FileLogDevice : ILogDevice
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public void Write(string message)
        {
            try
            {
                using (StreamWriter writer = new(FilePath))
                {
                    writer.WriteLine(message);
                }
            }
            catch
            {
            };
        }
    }
}
