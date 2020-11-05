using System;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class DurationParserTests
    {
        [TestCase("1d 13:50:28.047")]
        //[TestCase("4d  13:54:07.920")]
        public void WillParseDuration(string value)
        {
            var result = DurationParser.Parse(value);
            Assert.AreEqual(new TimeSpan(1, 13, 50, 28, 47), result);
        } 
    }
}