using System;

namespace QSI.Keyhole.Interactive
{
    class Program
    {
        /// <summary>
        /// Application entry, uses the Interactive Console
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var client = new InteractiveConsole();
            client.RunClient();
        }
    }
}