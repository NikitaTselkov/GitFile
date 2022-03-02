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
    public class TakeTests
    {
        [TestMethod()]
        public void TakeTest()
        {
            var result1 = "va";
            var result2 = "";
            var result3 = "";

            var method1 = new Take("value, 2");
            var method2 = new Take("value, -2");
            var method3 = new Take("");

            Assert.ThrowsException<FormatException>(() => new Take("value"));
            Assert.ThrowsException<FormatException>(() => new Take("value, 2, 3"));
            Assert.ThrowsException<FormatException>(() => new Take("value, text"));

            Assert.AreEqual(result1, method1.Result);
            Assert.AreEqual(result2, method2.Result);
            Assert.AreEqual(result3, method3.Result);
        }
    }
}