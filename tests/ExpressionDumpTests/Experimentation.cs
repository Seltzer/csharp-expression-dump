using System;
using System.Linq.Expressions;
using ExpressionDumpTests.TestObjects;
using NUnit.Framework;

namespace ExpressionDumpTests
{

    [TestFixture]
    class Experimentation
    {
        [Test]
        public void TestStuff()
        {
            {
                Console.WriteLine("a");
                var blah = new TestThing(new TestThing()).TestMethod();
                ParseExpression(() => blah);
            }
           
            Assert.True(1 == 1);
        }


        void ParseExpression<T>(Expression<Func<T>> expr)
        {
            Console.WriteLine(ExpressionDump.ExpressionDump.Dump(expr.Body));
        }
    }
    
}
