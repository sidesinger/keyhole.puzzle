using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QSI.Keyhole.Processing;
using QSI.Threading;
namespace QSI.Keyhole.Tests
{
    [TestClass]
    public class KeyProcessorTests
    {
        private void TestResult(string correctKey, bool expectSuccess = true)
        {
            var service = new LocalKeyService(correctKey);
            var target = new KeyProcessor(service, new ThreadOperator(), null, null);
            KeySearchResult tryResult = target.RunUntilCorrectKeyFoundOrSearchLimitHit();
            Assert.AreEqual(expectSuccess, tryResult.WasCorrectKeyFound);
        }

        [TestMethod]
        public void ReportsCorrectKey()
        {
            TestResult("tesTkEy");
        }

        [TestMethod]
        public void ReportsCorrectOneCharKey()
        {
            TestResult("t");
        }

        [TestMethod]
        public void ReportsCorrectTwoCharKey()
        {
            TestResult("t2");
        }

        [TestMethod]
        public void ReportsCorrectTwoCharKeyReverseAlphabetical()
        {
            TestResult("ta");
        }

        [TestMethod]
        public void ReportsCorrectThreeCharKey()
        {
            TestResult("zat");
        }

        [TestMethod]
        public void ReportsCorrectThreeCharKeyReverseAlphabetical()
        {
            TestResult("zta");
        }

        [TestMethod]
        public void ReportsCorrectOnEmptyKey()
        {
            TestResult("", true);
        }

        [TestMethod]
        public void FailsOnCorrectKeyWithDuplicates()
        {
            TestResult("testkEy", false);
            TestResult("tt", false);
            TestResult("ttt", false);
            TestResult("testkEy", false);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FailsOnBadlyFormattedCorrectKey()
        {
            TestResult("$3 2", false);
        }
    }
}
