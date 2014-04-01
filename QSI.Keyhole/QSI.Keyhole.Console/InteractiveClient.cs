using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;
using QSI.Keyhole.Processing;
using QSI.Threading;
using System.Net;

namespace QSI.Keyhole.Interactive
{
    /// <summary>
    /// Runs a Keyhole key search, displays results, and allows the user to pause and stop the search.
    /// </summary>
    class InteractiveConsole
    {
        private static string _whoopsMessage = "Well, this is a little embarrassing.  The system hit an unexpected snag. We're going to shut down, but first here are the details.";

        private int? _searchLimit;
        private string _remoteServiceUrl;
        private bool _useRemoteService;
        private string _localCorrectKey;
        private string _remoteCorrectKeyOverride;

        /// <summary>
        /// Start a keyhole key search.  Use App.config settings to configure.  Allows the user to pause and stop the worker thread.
        /// </summary>
        public void RunClient()
        {
            try
            {
                LoadConfigurationSettings();
                PrintIntroMessage(_remoteServiceUrl);

                var consoleReporter = new ConsoleStatusReporter();
                var waitHandler = new ThreadOperator(); // handles pause/stop communication between this and the worker thread

                IKeyService keyService;
                if (_useRemoteService)
                {
                    keyService = new QSIKeyService(_remoteServiceUrl, _remoteCorrectKeyOverride);
                }
                else
                {
                    keyService = new LocalKeyService(_localCorrectKey); // a local implementation for testing and fun
                }

                var keyProcessor = new KeyProcessor(keyService, waitHandler, _searchLimit, consoleReporter);

                // do the search in a separate thread
                var processingThread = new Thread(FindKey);
                processingThread.Start(keyProcessor);

                // allow the user to pause and stop the worker thread
                while (true)
                {
                    var input = Console.ReadKey();
                    if (input.KeyChar == 's')
                    {
                        waitHandler.RequestStop();
                    }
                    else if (input.KeyChar == 'p')
                    {
                        waitHandler.RequestPause();
                        Console.WriteLine("Pausing.. press any key to resume.");
                        Console.ReadKey();
                        waitHandler.RequestResume();
                    }
                    else if (input.KeyChar == 'x')
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(_whoopsMessage);
                Console.WriteLine(ex.ToString());
            }
        }

        private void LoadConfigurationSettings()
        {
            _searchLimit = null;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SearchLimit"]))
            {
                _searchLimit = int.Parse(ConfigurationManager.AppSettings["SearchLimit"]);
            }

            _remoteServiceUrl = ConfigurationManager.AppSettings["RemoteServiceUrl"];
            _localCorrectKey = ConfigurationManager.AppSettings["LocalCorrectKey"];
            _useRemoteService = ConfigurationManager.AppSettings["UseRemoteService"] == "true";
            _remoteCorrectKeyOverride = ConfigurationManager.AppSettings["RemoteCorrectKeyOverride"];
        }

        /// <summary>
        /// Run in a separate thread, this calls the processor's search method
        /// </summary>
        /// <param name="keyProcessor"></param>
        private void FindKey(object keyProcessor)
        {
            try
            {
                var processor = keyProcessor as KeyProcessor;
                var result = processor.RunUntilCorrectKeyFoundOrSearchLimitHit();
                PrintResult(result);
            }
            catch (WebException webEx)
            {
                Console.WriteLine(_whoopsMessage);
                Console.WriteLine("Could not connect to the remote keyhole service.  Check the Url in the App.config and try again.");
                Console.WriteLine(webEx.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(_whoopsMessage);
                Console.WriteLine(ex.ToString());
            }
        }

        private void PrintIntroMessage(string serviceUrl)
        {
            Console.WriteLine("Starting Keyhole Puzzle Search");
            if (_useRemoteService)
            {
                Console.WriteLine("The system will connect using this service url:" + serviceUrl);
            }
            else
            {
                Console.WriteLine("The system will connect to the local service.");
            }
            Console.WriteLine("You can stop the search by typing 's' at any time.");
            Console.WriteLine("You can pause the search by typing 'p' at any time.");
            Console.WriteLine("You can exit the program by typing 'x' at any time.");
        }

        private void PrintResult(KeySearchResult result)
        {
            Console.WriteLine("A correct key was " + (result.WasCorrectKeyFound ? "" : "not ") + " found.");
            Console.WriteLine("The last key used was '" + result.FinalKey + "'");
            Console.WriteLine(result.AttemptsMade.ToString() + " attempt" + (result.AttemptsMade == 1 ? " was " : "s were ") + "made");
            if (!string.IsNullOrEmpty(result.Message))
            {
                Console.WriteLine("Additional Information: '" + result.Message + "'");
            }
            Console.WriteLine("Press 'x' to exit the program.");
        }
    }
}
