using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class ConcurrentDictionaryTests
    {
        [Test]
        public void WillSkipRedundantItems()
        {
            var data = new List<RecIdentity>();
            for (var i = 0; i < 4; i++)
            {
                data.Add(new RecIdentity("folderId", "sco"));
            }
            var dict = new ConcurrentDictionary<RecIdentity, double>();
            Parallel.ForEach(data, (x) =>
            {
                var result = dict.TryAdd(x, 1.0);
            });

            Assert.AreEqual(1, dict.Count);
        } 
    }
}