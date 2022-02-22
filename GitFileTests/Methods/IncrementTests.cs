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
    public class IncrementTests
    {
        [TestMethod()]
        public void IncrementTest()
        {
            var result1 = "2";
            var result2 = "0";
            var result3 = "1.1";
            var result4 = "1.0.1";
            var result5 = "1.1.0";
            var result6 = "2.0.0.0";

            var method1 = new Increment("1");
            var method2 = new Increment("-1");
            var method3 = new Increment("1.0, 9");
            var method4 = new Increment("1.0.0, 1, 1");
            var method5 = new Increment("1.0.1, 1, 1");
            var method6 = new Increment("1.3.1.1, 2, 1, 1");

            Assert.ThrowsException<ArgumentNullException>(() => new Increment(""));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Increment("1.945638953652362745749457.1, 2, 1"));
            Assert.ThrowsException<FormatException>(() => new Increment("1..1, 2, 1, 1"));
            Assert.ThrowsException<FormatException>(() => new Increment("1.3.1, 2, 1.3, 1"));
            Assert.ThrowsException<FormatException>(() => new Increment("1.3.1, 2, 1, 1"));
            Assert.ThrowsException<FormatException>(() => new Increment("1.5f, 6"));
            Assert.ThrowsException<FormatException>(() => new Increment("1.5, 6f"));

            Assert.AreEqual(result1, method1.Result);
            Assert.AreEqual(result2, method2.Result);
            Assert.AreEqual(result3, method3.Result);
            Assert.AreEqual(result4, method4.Result);
            Assert.AreEqual(result5, method5.Result);
            Assert.AreEqual(result6, method6.Result);
        }
    }
}