using System;
using System.Linq.Expressions;
using NUnit.Framework;
using ExpressionDump.Core;

namespace ExpressionDumpTests
{

    [TestFixture]
    class ExpressionDumpTests
    {
        string InvokeDump<T>(Expression<Func<T>> expr)
        {
            return ExpressionDump.ExpressionDump.Dump(expr.Body);
        }
        

        [Test]
        public void ConstMemberTests()
        {
            Assert.AreEqual("TestThing.ConstMember", InvokeDump( () => TestThing.ConstMember ));
        }

        [Test]
        public void ReadonlyMemberTests()
        {
            Assert.AreEqual("TestThing.ReadonlyMember", InvokeDump( () => TestThing.ReadonlyMember) );
        }


        [Test]
        public void LocalVarTests()
        {
            var blah = new TestThing(new TestThing()).TestMethod();
            Assert.AreEqual("blah", InvokeDump(() => blah));
        }


        [Test]
        public void GenericInstanceMethodTests()
        {
            Assert.AreEqual("new TestThing().Horatio<int>()", InvokeDump( () => new TestThing().Horatio<int>() ));
        }


        [Test]
        public void InlineFuncInvocationTests()
        {
            Assert.AreEqual("new Func<int, int, int>((a, b) => a)(1, 2)", InvokeDump( () => new Func<int, int, int>((a, b) => a)(1, 2) ));
        }
        
        [Test]
        public void OperatorTests()
        {
            Assert.AreEqual("new Func<int, int, int>((a, b) => a + b)(1, 2)", InvokeDump( () => new Func<int, int, int>((a, b) => a + b)(1, 2) ));
        }


        [Test]
        public void TestComplexStuff()
        {
            Assert.AreEqual("new TestThing(new TestThing()).TestMethod()", InvokeDump( () => new TestThing(new TestThing()).TestMethod() ));
            Assert.AreEqual("TestThing.Test2()", InvokeDump( () => TestThing.Test2() ));
            Assert.AreEqual("new TestThing(new TestThing()).Horatio<int>()", InvokeDump( () => new TestThing(new TestThing()).Horatio<int>() ));
            Assert.AreEqual("new TestThing(new TestThing()).Horatio<int>(2, 7)", InvokeDump( () => new TestThing(new TestThing()).Horatio<int>(2, 7) ));

            {
                Func<int, int, string> func = (a, b) => new TestThing().Horatio(a, b);
                Assert.AreEqual("new TestThing(new TestThing()).Horatio<int>(2, 7)", InvokeDump( () => func.Curry()(5) ));
            }
            
            Assert.AreEqual("new TestThing(new TestThing(8)).Horatio(new TestThing(), 8)", 
                InvokeDump( () => new TestThing(new TestThing(8)).Horatio(new TestThing(), 8) ));
        }
    }
    
}
