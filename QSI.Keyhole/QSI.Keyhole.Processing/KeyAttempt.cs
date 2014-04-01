using System;
using System.Collections.Generic;
using System.Linq;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Holds the results of a Keyhole puzzle attempt
    /// </summary>
    public class KeyAttempt : List<KeyAttemptPart>
    {
        public string KeyUsed { get; private set; }
        public bool IsKeyCorrect { get; private set; }
        public string RawResult { get; private set; }

        public int ExistsInCorrectKeyCount { get { return this.Count(t => t.ExistsInCorrectKey); } }
        public int IsInCorrectPlaceCount { get { return this.Count(t => t.IsInCorrectPlace); } }

        public KeyAttempt(string keyUsed, string rawResults, bool isKeyCorrect)
        {
            IsKeyCorrect = isKeyCorrect;
            KeyUsed = keyUsed;
            RawResult = rawResults;

            if (!IsKeyCorrect)
            {
                // parse raw results into KeyResultParts
                for (int i = 0; i < keyUsed.Length; i++)
                {
                    char keyValue = keyUsed[i];

                    // for each char in the key used, there will be two digits
                    // in left position, 1 if it's in the correct place
                    bool isInCorrectPlace = rawResults[i * 2] == '1';
                    // in the right position, 1 if it's in the correct key 
                    // (when a character exists multiple times in a key, it is still 1)
                    bool existsInCorrectKey = Convert.ToInt32(rawResults[i * 2 + 1].ToString()) >= 1;

                    var keyPart = new KeyAttemptPart
                    {
                        Value = keyValue,
                        IsInCorrectPlace = isInCorrectPlace,
                        ExistsInCorrectKey = existsInCorrectKey
                    };
                    this.Add(keyPart);
                }
            }
        }

        public void RemoveKeyPartsNotInCorrectKey()
        {
            this.RemoveAll(t => !t.ExistsInCorrectKey);
        }

        public char[] GetValuesThatExistInCorrectKey()
        {
            return this.Where(t => t.ExistsInCorrectKey).Select(t => t.Value).ToArray();
        }
    }
}
