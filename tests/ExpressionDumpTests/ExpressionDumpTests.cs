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
            // Ideally Dump would output 'TestThing.ConstMember', but C# limitations prevent us from getting at the member name.
            Assert.AreEqual(@""""  + TestThing.ConstMember + @"""", InvokeDump( () => TestThing.ConstMember ));
        }


        [Test]
        public void ReadonlyMemberTests()
        {
            // TODO: TestThing.ReadonlyMember or ReadonlyMember?
            Assert.AreEqual("ReadonlyMember", InvokeDump( () => TestThing.ReadonlyMember) );
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
            Assert.AreEqual("new ExpressionDumpTests.TestThing().Horatio<int>()", InvokeDump( () => new TestThing().Horatio<int>() ));
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
        public void New()
        {
            Assert.AreEqual("new ExpressionDumpTests.TestThing()", InvokeDump(() => new TestThing()));
            Assert.AreEqual(
                "new ExpressionDumpTests.TestThing(new ExpressionDumpTests.TestThing())", 
                InvokeDump(() => new TestThing(new TestThing())));
            Assert.AreEqual(
                "new ExpressionDumpTests.TestThing(new ExpressionDumpTests.TestThing(), new ExpressionDumpTests.TestThing())", 
                InvokeDump(() => new TestThing(new TestThing(), new TestThing())));
        }


        [Test]
        public void RegularStaticMethodInvocation()
        {
            Assert.AreEqual("ExpressionDumpTests.TestThing.Test2()", InvokeDump( () => TestThing.Test2() ));
        }


        [Test]
        public void ExtensionMethodInvocation()
        {
            var tt = new TestThing();

            Assert.AreEqual("tt.ExtensionMethod()", InvokeDump( () => tt.ExtensionMethod()));
        }


        [Test]
        public void InstanceMethodInvocation()
        {
            var tt = new TestThing();

            Assert.AreEqual("tt.TestMethod()", InvokeDump( () => tt.TestMethod() ));
        }


        //[Test]
        //public void TestComplexStuff()
        //{
        //    Assert.AreEqual(
        //        "new ExpressionDumpTests.TestThing(new ExpressionDumpTests.TestThing()).TestMethod()", 
        //        InvokeDump( () => new TestThing(new TestThing()).TestMethod() ));
        //    Assert.AreEqual("ExpressionDumpTests.TestThing.Test2()", InvokeDump( () => TestThing.Test2() ));
        //    Assert.AreEqual(
        //        "new ExpressionDumpTests.TestThing(new ExpressionDumpTests.TestThing()).Horatio<int>()", 
        //        InvokeDump( () => new TestThing(new TestThing()).Horatio<int>() ));
        //    Assert.AreEqual(
        //        "new ExpressionDumpTests.TestThing(new ExpressionDumpTests.TestThing()).Horatio<int>(2, 7)", 
        //        InvokeDump( () => new TestThing(new TestThing()).Horatio<int>(2, 7) ));

        //    {
        //        Func<int, int, string> func = (a, b) => new TestThing().Horatio(a, b);
        //        // Ideally should be able to omit type parameters if unnecessary
        //        Assert.AreEqual("func.Curry<int, int, string>()(5)", InvokeDump( () => func.Curry()(5) ));
        //    }
        //}
    }
    
}
