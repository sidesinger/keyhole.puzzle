using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QSI.Keyhole.Processing;

namespace QSI.Keyhole.Tests
{
    [TestClass]
    public class KeyAttemptTests
    {
        [TestMethod]
        public void CorrectKey()
        {
            string key = "abc123";
            string rawResults = "1111";
            bool isKeyCorrect = true;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(key, target.KeyUsed);
            Assert.AreEqual(rawResults, target.RawResult);
            Assert.AreEqual(isKeyCorrect, target.IsKeyCorrect);
        }

        [TestMethod]
        public void NoCharsExist()
        {
            string key = "abc123";
            string rawResults = "000000000000";
            bool isKeyCorrect = false;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(key, target.KeyUsed);
            Assert.AreEqual(rawResults, target.RawResult);
            Assert.AreEqual(isKeyCorrect, target.IsKeyCorrect);
            Assert.AreEqual(0, target.ExistsInCorrectKeyCount);
            Assert.AreEqual(0, target.IsInCorrectPlaceCount);
        }

        [TestMethod]
        public void SomeCharsExistNoneInRightPlace()
        {
            string key = "abc123";
            string rawResults = "010100000001";
            bool isKeyCorrect = false;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(key, target.KeyUsed);
            Assert.AreEqual(rawResults, target.RawResult);
            Assert.AreEqual(isKeyCorrect, target.IsKeyCorrect);
            Assert.AreEqual(3, target.ExistsInCorrectKeyCount);
            Assert.AreEqual(0, target.IsInCorrectPlaceCount);
        }

        [TestMethod]
        public void TestSomeCharsExistSomeInRightPlace()
        {
            string key = "abc123";
            string rawResults = "110100000011";
            bool isKeyCorrect = false;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(key, target.KeyUsed);
            Assert.AreEqual(rawResults, target.RawResult);
            Assert.AreEqual(isKeyCorrect, target.IsKeyCorrect);
            Assert.AreEqual(3, target.ExistsInCorrectKeyCount);
            Assert.AreEqual(2, target.IsInCorrectPlaceCount);
        }

        [TestMethod]
        public void RemoveAllKeyPartsNotInCorrectKey()
        {
            string key = "abc123";
            string rawResults = "000000000000";
            bool isKeyCorrect = false;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(6, target.Count);
            target.RemoveKeyPartsNotInCorrectKey();
            Assert.AreEqual(0, target.Count);
        }

        public void RemoveSomeKeyPartsNotInCorrectKey()
        {
            string key = "abc123";
            string rawResults = "010000010011";
            bool isKeyCorrect = false;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(6, target.Count);
            target.RemoveKeyPartsNotInCorrectKey();
            Assert.AreEqual(3, target.Count);
        }

        public void RemoveNoKeyPartsNotInCorrectKey()
        {
            string key = "abc123";
            string rawResults = "010101010111";
            bool isKeyCorrect = false;
            KeyAttempt target = new KeyAttempt(key, rawResults, isKeyCorrect);

            Assert.AreEqual(6, target.Count);
            target.RemoveKeyPartsNotInCorrectKey();
            Assert.AreEqual(6, target.Count);
        }
    }
}
