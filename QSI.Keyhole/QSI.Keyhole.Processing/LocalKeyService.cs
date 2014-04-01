using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QSI.Extensions;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// A local implementation of the key service for non mocked unit testing and fun.
    /// (didn't want to add an external dependancy)
    /// </summary>
    public class LocalKeyService : IKeyService
    {
        private string _correctKey;

        public LocalKeyService(string correctKey)
        {
            _correctKey = correctKey;

            // the service could accept some non alphanumeric correct keys, but they are not within scope
            if (correctKey != null && !correctKey.IsAlphaNumericNoWhiteSpace())
            {
                throw new FormatException("The correct key must be alpha-numeric with no whitespace.");
            }
        }

        public KeyAttempt TryKey(string key)
        {
            Thread.Sleep(1500); // pause here so you can try the system's pause/resume/stop lagniappe 
            return new KeyAttempt(key, GetKeyResultCode(key), key == _correctKey);
        }

        private string GetKeyResultCode(string key)
        {
            string code = "";
            for (int i = 0; i < key.Length; i++)
            {
                int countInCorrectKey = _correctKey.Count(t => t == key[i]);
                if (countInCorrectKey > 0)
                {
                    if (i < _correctKey.Length && _correctKey[i] == key[i])
                    {
                        // the QSI service returns a 1 in the rightside position
                        // even when the character exists multiple times in a correct key
                        // here we report the actual count
                        code += "1" + countInCorrectKey.ToString();
                    }
                    else
                    {
                        code += "0" + countInCorrectKey.ToString();
                    }
                }
                else
                {
                    code += "00";
                }
            }
            return code;
        }
    }
}
