using System;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Pertinent information about one character's result from the keyhole service
    /// </summary>
    public class KeyAttemptPart
    {
        public char Value { get; set; }
        public bool ExistsInCorrectKey { get; set; }
        public bool IsInCorrectPlace { get; set; }
    }
}
