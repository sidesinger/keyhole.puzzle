using System;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Describes the results of an attempt to solve the Keyhole puzzle
    /// </summary>
    public class KeySearchResult
    {
        public bool WasCorrectKeyFound { get; private set; }
        public string FinalKey { get; private set; }
        public string Message { get; private set; }
        public int AttemptsMade { get; private set; }

        public KeySearchResult(bool wasCorrectKeyFound, string finalKey, string message, int attemptsMade)
        {
            WasCorrectKeyFound = wasCorrectKeyFound;
            FinalKey = finalKey;
            Message = message;
            AttemptsMade = attemptsMade;
        }
    }
}
