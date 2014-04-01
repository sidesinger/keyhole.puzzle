using System;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Describes how to interact with a keyhole service
    /// </summary>
    public interface IKeyService
    {
        /// <summary>
        /// Make one call to a keyhole service to validate the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        KeyAttempt TryKey(string key);
    }
}
