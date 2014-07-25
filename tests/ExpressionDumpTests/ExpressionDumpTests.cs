using System;
using System.Linq.Expressions;
using ExpressionDumpTests.TestObjects;
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
        public void New_Parameterless()
        {
            Assert.AreEqual("new ExpressionDumpTests.TestObjects.TestThing()", InvokeDump(() => new TestThing()));
        }

        
        [Test]
        public void New_WithParameters()
        {
            Assert.AreEqual(
                "new ExpressionDumpTests.TestObjects.TestThing(new ExpressionDumpTests.TestObjects.TestThing())", 
                InvokeDump(() => new TestThing(new TestThing())));
            Assert.AreEqual(
                "new ExpressionDumpTests.TestObjects.TestThing(new ExpressionDumpTests.TestObjects.TestThing(), new ExpressionDumpTests.TestObjects.TestThing())", 
                InvokeDump(() => new TestThing(new TestThing(), new TestThing())));
        }


        [Test]
        public void New_Generic()
        {
            Assert.AreEqual(
                "new TypedTestThing<string>(7, 8)", 
                InvokeDump(() => new TypedTestThing<string>(7, 8)));
        }


        [Test]
        public void MethodInvocation_InstanceMethod()
        {
            var tt = new TestThing();

            Assert.AreEqual("tt.TestMethod()", InvokeDump( () => tt.TestMethod() ));
        }


        [Test]
        public void MethodInvocation_RegularStaticMethod()
        {
            Assert.AreEqual("ExpressionDumpTests.TestObjects.TestThing.Test2()", InvokeDump( () => TestThing.Test2() ));
        }


        [Test]
        public void MethodInvocation_ExtensionMethod()
        {
            var tt = new TestThing();

            Assert.AreEqual("tt.ExtensionMethod()", InvokeDump( () => tt.ExtensionMethod()));
        }
        

        [Test]
        public void MethodInvocation_ArgumentsTest()
        {            
            var tt = new TestThing();

            Assert.AreEqual(@"tt.Horatio(""asdf"", 2)", InvokeDump( () => tt.Horatio("asdf", 2)));
        }


        [Test]
        public void MethodInvocation_TypeArgumentsTest()
        {
            var tt = new TestThing();

            Assert.AreEqual("tt.Horatio<int>()", InvokeDump( () => tt.Horatio<int>() ));
            
            Assert.AreEqual("ExpressionDumpTests.TestObjects.TestThing.StaticGenericMethod<int>(3)", InvokeDump( () => TestThing.StaticGenericMethod(3)));
            Assert.AreEqual("ExpressionDumpTests.TestObjects.TestThing.StaticGenericMethod<int>(3)", InvokeDump( () => TestThing.StaticGenericMethod<int>(3)));

            Assert.AreEqual(@"tt.GenericExtensionMethod<int>(7, ""asdf"")", InvokeDump(() => tt.GenericExtensionMethod(7, "asdf")));
            Assert.AreEqual(@"tt.GenericExtensionMethod<int>(7, ""asdf"")", InvokeDump(() => tt.GenericExtensionMethod<int>(7, "asdf")));
        }

        
        [Test]
        public void TestComplexStuff()
        {
            Assert.AreEqual(
                "new ExpressionDumpTests.TestObjects.TestThing(new ExpressionDumpTests.TestObjects.TestThing()).TestMethod()", 
                InvokeDump( () => new TestThing(new TestThing()).TestMethod() ));
            Assert.AreEqual("ExpressionDumpTests.TestObjects.TestThing.Test2()", InvokeDump( () => TestThing.Test2() ));
            Assert.AreEqual(
                "new ExpressionDumpTests.TestObjects.TestThing(new ExpressionDumpTests.TestObjects.TestThing()).Horatio<int>()", 
                InvokeDump( () => new TestThing(new TestThing()).Horatio<int>() ));
            Assert.AreEqual(
                "new ExpressionDumpTests.TestObjects.TestThing(new ExpressionDumpTests.TestObjects.TestThing()).Horatio<int>(2, 7)", 
                InvokeDump( () => new TestThing(new TestThing()).Horatio<int>(2, 7) ));

            {
                Func<int, int, string> func = (a, b) => new TestThing().Horatio(a, b);
                // Ideally should be able to omit type parameters if unnecessary
                Assert.AreEqual("func.Curry<int, int, string>()(5)", InvokeDump( () => func.Curry()(5) ));
            }
        }
    }
    
}
