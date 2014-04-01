using System;
using System.Collections.Generic;
using QSI.Keyhole.Processing;

namespace QSI.Keyhole.Interactive
{
    /// <summary>
    /// Report status to the console
    /// </summary>
    public class ConsoleStatusReporter : IStatusReporter
    {
        public void ReportStatus(string statusMessage)
        {
            Console.WriteLine(statusMessage);
        }
    }
}
