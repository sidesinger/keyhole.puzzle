using System;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Describes a way for the processor to report status
    /// </summary>
    public interface IStatusReporter
    {
        void ReportStatus(string statusMessage);
    }
}
