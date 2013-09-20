using System;
using NUnit.Framework;
using ExpressionDump.Core;

namespace ExpressionDumpTests.Core
{

    [TestFixture]
    class TypeTests
    {
        [Test]
        public void GetTypeAliasTests()
        {
            Assert.AreEqual("int", typeof(int).GetTypeAliasOrSelf());
            Assert.AreEqual("int", typeof(Int32).GetTypeAliasOrSelf());
            Assert.AreEqual("string", typeof(string).GetTypeAliasOrSelf());
            Assert.AreEqual("string", typeof(System.String).GetTypeAliasOrSelf());
            Assert.AreEqual("TestThing", typeof(TestThing).GetTypeAliasOrSelf());
        }
    }
    
}
