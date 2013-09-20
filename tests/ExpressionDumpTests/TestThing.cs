using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ExpressionDumpTests
{

    public class Experiment2222
    {
       

        public int Foo(int k)
        {
            return k + 5;
        }


        public int Bar(int k)
        {
            return k + 50;
        }


        public static Func<A, C> Compose<A, B, C>(Func<B, C> f, Func<A, B> g)
        {
            return x => f(g(x));
        }


        public void TestStuff()
        {
            Assert.AreEqual(56, Compose<int, int, int>(Foo, Bar)(1));
        }

    }
  


    public class TestThing
    {
        public const string ConstMember = "value: i am a const";
        public static readonly string ReadonlyMember = "value: i am a const";

        public TestThing TestThingInner { get; set; }

        public TestThing()
        {

        }

        public TestThing(TestThing tt)
        {
            TestThingInner = tt;

        }


        public TestThing(int _)
        {
            
        }

        public string TestMethod()
        {
            return "value: hello to you too";
        }


        public static string Test2()
        {
            return "value: Test2";
        }

        public string Horatio<T>()
        {
            return default(T).ToString();
        }


        public string Horatio<T>(T unused, int number = 7)
        {
            return number.ToString();
        }
    }

}
