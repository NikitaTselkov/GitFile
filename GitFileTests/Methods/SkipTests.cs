using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitFile.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFile.Methods.Tests
{
    [TestClass()]
    public class SkipTests
    {
        [TestMethod()]
        public void SkipTest()
        {
            var result1 = "lue";
            var result2 = "value";

            var method1 = new Skip("value, 2");
            var method2 = new Skip("value, -2");

            Assert.ThrowsException<ArgumentNullException>(() => new Skip(""));
            Assert.ThrowsException<FormatException>(() => new Skip("value"));
            Assert.ThrowsException<FormatException>(() => new Skip("value, 2, 3"));
            Assert.ThrowsException<FormatException>(() => new Skip("value, text"));

            Assert.AreEqual(result1, method1.Result);
            Assert.AreEqual(result2, method2.Result);
        }
    }
}