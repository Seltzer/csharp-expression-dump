using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionDump.Core;
using NUnit.Framework;

namespace ExpressionDumpTests
{

    [TestFixture]
    class LoggerTests
    {
        [Test]
        public void TestStuff()
        {
            {
                Console.WriteLine("a");
                var blah = new TestThing(new TestThing()).TestMethod();
                ParseExpression(() => blah);
            }
            {
                Console.WriteLine("b");
                ParseExpression(() => new TestThing(new TestThing()).TestMethod());
            }
            {
                Console.WriteLine("c");
                ParseExpression(() => TestThing.Test2());
            }
            {
                Console.WriteLine("d");
                ParseExpression(() => TestThing.ConstMember);
            }
            {
                Console.WriteLine("e");
                ParseExpression(() => new TestThing(new TestThing()).Horatio<int>());
            }

            {
                Console.WriteLine("f");
                //ParseExpression(() => new TestThing(new TestThing()).Horatio<int>(2));
            }

            {
                Console.WriteLine("g");
                ParseExpression(() => new TestThing(new TestThing()).Horatio<int>(2, 7));
            }


            {
                Console.WriteLine("h");

                Func<int, int, string> func = (a, b) => new TestThing().Horatio(a, b);

                ParseExpression(() => func.Curry()(5));
            }


            {
                Console.WriteLine("i");

                ParseExpression(() => new TestThing(new TestThing(8)).Horatio(new TestThing(), 8));
            }

            {
                Console.WriteLine("j");

                ParseExpression(() => TestThing.ReadonlyMember);
            }


            {
                Console.WriteLine("l");
                
                ParseExpression(() => new Func<int, int, int>((a, b) => a)(1, 2));
            }

            Assert.True(1 == 1);
        }


        void ParseExpression<T>(Expression<Func<T>> expr)
        {
            Console.WriteLine("representation = " + ToString(expr));
            //Console.WriteLine("value = " + expr.Compile().DynamicInvoke());
        }


        string ToString<T>(Expression<Func<T>> expr)
        {
  

            return ExpressionDump.ExpressionDump.Dump(expr.Body);

            //if (methodCallExpression != null)
            //{
            //    Console.WriteLine("methodCallExpression");
            //    return ExpressionDump.ExpressionDump.Dump(methodCallExpression);
            //}
            //else if (lambdaExpression != null)
            //{
            //    Console.WriteLine("lambdaExpression");

            //    //Console.WriteLine(lambdaExpression);
            //    //Console.WriteLine(lambdaExpression.Body.ToString());
            //    //Console.WriteLine(lambdaExpression.Name.ToString());
            //    //Console.WriteLine(lambdaExpression.Compile().DynamicInvoke());

            //}
            //if (memberExpression != null)
            //{
            //    Console.WriteLine("memberExpression");
            //    //Console.WriteLine(memberExpression);

            //    FieldInfo fi = (FieldInfo)memberExpression.Member;
            //    ConstantExpression bodyExpression = memberExpression.Expression as ConstantExpression;

            //    return fi.Name;

            //    //Console.WriteLine(fi.ToString());
            //    //Console.WriteLine(fi.Name.ToString());
            //    //Console.WriteLine(fi.GetValue(bodyExpression.Value).ToString());
            //}

            ////var bodyMember = body.Member as FieldInfo;
            ////if (bodyMember == null)
            ////    throw new Exception("bodyMember is null");


            ////var bodyExpression = body.Expression as ConstantExpression;
            ////if (bodyExpression == null)
            ////    throw new Exception("bodyExpression is null");



            //return null;
        }
    }
    
}
