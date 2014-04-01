using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using QSI.Extensions;

namespace QSI.Keyhole.Processing
{
    /// <summary>
    /// Contacts the Keyhole service with a specified key and provides the coded result.
    /// </summary>
    public class QSIKeyService : IKeyService
    {
        private string _serviceUrl;
        private string _correctKeyOverride;

        private const string _correctKeySearchText = "Congratulations. You got in!";

        public QSIKeyService(string serviceUrl, string correctKeyOverride = null)
        {
            _serviceUrl = serviceUrl;
            _correctKeyOverride = correctKeyOverride;

            // the service does accept some non alphanumeric correct keys, but they are not within scope
            if (correctKeyOverride != null && !correctKeyOverride.IsAlphaNumericNoWhiteSpace())
            {
                throw new FormatException("The correct key must be alpha-numeric with no whitespace.");
            }
        }

        /// <summary>
        /// Connect to the remote keyhole service and try a key, return the result.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyAttempt TryKey(string key)
        {
            KeyAttempt result;
            using (var client = new WebClient())
            {
                var response = client.DownloadString(MakeUrl(key));
                result = GetResultFromRawResponse(key, response);
            }
            return result;
        }

        private string MakeUrl(string key)
        {
            var finalUrl = _serviceUrl + "?key=" + key;
            if (!string.IsNullOrEmpty(_correctKeyOverride))
            {
                finalUrl += "&keyhole=" + _correctKeyOverride;
            }
            return finalUrl;
        }

        /// <summary>
        /// Parse the response to get success/failure and the charater placement "code"
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rawResponse"></param>
        /// <returns></returns>
        private KeyAttempt GetResultFromRawResponse(string key, string rawResponse)
        {
            string responseCode; ;
            bool isKeyCorrect;

            if (rawResponse.Contains(_correctKeySearchText))
            {
                responseCode = _correctKeySearchText;
                isKeyCorrect = true;
            }
            else
            {
                // strip the html and pre text, leave only the 1s and 0s code.
                responseCode = Regex.Replace(rawResponse, @"<[^>]*>", string.Empty);
                responseCode = responseCode.Replace("KEY DENIED: ", string.Empty);
                isKeyCorrect = false;
            }

            return new KeyAttempt(key, responseCode, isKeyCorrect);
        }

    }
}
