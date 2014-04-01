using System;
using System.Collections.Generic;
using System.Linq;
using QSI.Threading;
using QSI.Extensions;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Executes searching for a correct key to the provided keyhole service.  Will process until the key is found or max attempts is reached.
    /// </summary>
    public class KeyProcessor
    {
        private IKeyService _keyService; // does the calls to check the key
        private ThreadOperator _waitHandler; // allows for external stopping and pausing of the processor
        private IStatusReporter _statusReporter;
        int? _searchLimit;

        private const string _initialKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // all valid key chars

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyService">The service to perform the key validation</param>
        /// <param name="waitHandler">Used to pause and stop the search</param>
        /// <param name="searchLimit">Maximum number of keys to try</param>
        /// <param name="statusReporter">Optionaly report status of the key search.</param>
        public KeyProcessor(IKeyService keyService, ThreadOperator waitHandler, int? searchLimit = null, IStatusReporter statusReporter = null)
        {
            _keyService = keyService;
            _waitHandler = waitHandler;
            _statusReporter = statusReporter;
            _searchLimit = searchLimit;
        }

        /// <summary>
        /// Searches for a correct key.  First sends the service every possible character to see which exists in the correc key
        /// Then it repeatedly tries keys by moving all chars not in the correct place down one position and loops the last bad char
        /// to the front.
        /// </summary>
        /// <returns></returns>
        public KeySearchResult RunUntilCorrectKeyFoundOrSearchLimitHit()
        {
            // find which characters exist in the final key
            string nextKey = FindValuesInCorrectKey();
            int attemptsMade = 1;

            while (true)
            {
                if (_searchLimit.HasValue && attemptsMade >= _searchLimit)
                {
                    return new KeySearchResult(false, nextKey, "Key search stopped; search limit reached.", attemptsMade);
                }

                // check if consumer has requested a stop or pause
                _waitHandler.WaitIfPaused();
                if (_waitHandler.IsStopRequested())
                {
                    return new KeySearchResult(false, nextKey, "Key search stopped by user.", attemptsMade);
                }

                ReportStatus("Trying key '" + nextKey + "'");
                var nextResponse = _keyService.TryKey(nextKey);
                ReportStatus("Result: '" + (nextResponse.IsKeyCorrect ? "Success" : nextResponse.RawResult) + "'");

                if (nextResponse.IsKeyCorrect)
                {
                    // all characters are in place->correct key found
                    return new KeySearchResult(true, nextKey, string.Empty, attemptsMade);
                }

                // make a new key - take the bad located chars and push them down to the next bad location
                // and place the last bad in the first bad location

                // make a 'characters not in the correct place' ordered queue
                var badCharsInOrder = new List<char>();
                for (int i = 0; i < nextKey.Length; i++) //nextKey.Length - 1; i >= 0; i--) // work backwards to have the last bad char at the front of the queue
                {
                    if (!nextResponse[i].IsInCorrectPlace)
                    {
                        badCharsInOrder.Add(nextResponse[i].Value);
                    }
                }

                // note: this doesn't solve multiple existances of a char in the correct key, service returns '1' still.
                // hope that's ok!
                // it could take a lot of attempts when you have a full 11 result, but not "Success" 
                //      the possible permuations is quite high ...  
                if (badCharsInOrder.Count < 2)
                {
                    string message = @"The correct key seems to have multiple uses of the same character,"
                        + "this processor is not set up to solve this case."
                        + Environment.NewLine
                        + "Hope thats ok :S";
                    return new KeySearchResult(false, nextKey, message, attemptsMade);
                }

                // pivot the last bad character to the front, pushing all the others back a spot
                var pivotChar = badCharsInOrder[badCharsInOrder.Count - 1];
                badCharsInOrder.RemoveAt(badCharsInOrder.Count - 1);
                badCharsInOrder.Insert(0, pivotChar);

                // now place the reordered bad chars into the key
                for (int i = 0; i < nextKey.Length; i++)
                {
                    if (!nextResponse[i].IsInCorrectPlace)
                    {
                        var nextChar = badCharsInOrder[0];
                        badCharsInOrder.RemoveAt(0);
                        nextKey = nextKey.ReplaceCharAt(i, nextChar);
                    }
                }
                attemptsMade++;
            }
        }

        private string FindValuesInCorrectKey()
        {
            // find which characters exist in the final key
            ReportStatus("Trying key '" + _initialKey + "'");
            var initialKeyResults = _keyService.TryKey(_initialKey);
            ReportStatus("Result: '" + (initialKeyResults.IsKeyCorrect ? "Success" : "Failure") + "'");

            initialKeyResults.RemoveKeyPartsNotInCorrectKey();

            // set up our first real attempt, going with the "hint" that the key should start with Q
            // the instructions are somewhat ambiguous if the Q hint holds when the key changes, but everything still works in that case.
            var finalKeyValues = initialKeyResults.GetValuesThatExistInCorrectKey().ToArray();
            return new string(finalKeyValues);
        }

        /// <summary>
        /// If the Status Reporter was passed in during construction, use it to report status.
        /// </summary>
        /// <param name="message"></param>
        private void ReportStatus(string message)
        {
            if (_statusReporter != null)
            {
                _statusReporter.ReportStatus(message);
            }
        }
    }
}
