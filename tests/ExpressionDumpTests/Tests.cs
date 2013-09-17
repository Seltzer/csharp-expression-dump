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

                ParseExpression(() => new TestThing().Horatio(new TestThing(), 8));
            }

            Assert.True(1 == 1);
        }


        void ParseExpression<T>(Expression<Func<T>> expr)
        {
            Console.WriteLine("representation = " + ToString(expr));
            Console.WriteLine("value = " + expr.Compile().DynamicInvoke());
        }




        string ToStringSub(Expression expr)
        {
            MethodCallExpression methodCallExpression = null;
            LambdaExpression lambdaExpression = null;
            MemberExpression memberExpression = null;
            ConstantExpression constantExpression = null;

            object blah = (object)(methodCallExpression = expr as MethodCallExpression)
                ?? (object)(lambdaExpression = expr as LambdaExpression)
                ?? (object)(memberExpression = expr as MemberExpression)
                ?? (constantExpression = expr as ConstantExpression);

            if (methodCallExpression != null)
            {
                Console.WriteLine("methodCallExpression");

                string methodSubject = methodCallExpression.Object.IfNotNull(o => o.ToString()) ?? methodCallExpression.Method.ReflectedType.Name;
                string methodName = methodCallExpression.Method.Name;
                string args = methodCallExpression.Arguments.Select(ToStringSub).StringJoin(", ");

                return String.Format("{0}.{1}({2})", methodSubject, methodName, args);
            }
            else if (constantExpression != null)
            {
                return constantExpression.ToString();
            }
            else
            {
                return "UNKNOWN - " + expr.GetType();
            }
        }


        string ToString<T>(Expression<Func<T>> expr)
        {
            MethodCallExpression methodCallExpression = null;
            LambdaExpression lambdaExpression = null;
            MemberExpression memberExpression = null;

            //bool matches = (methodCallExpression = expr.Body as MethodCallExpression) != null
            //    || (lambdaExpression = expr.Body as LambdaExpression) != null;

            object blah = (object)(methodCallExpression = expr.Body as MethodCallExpression)
                ?? (object)(lambdaExpression = expr.Body as LambdaExpression)
                ?? (memberExpression = expr.Body as MemberExpression);

            if (methodCallExpression != null)
            {
                Console.WriteLine("methodCallExpression");
                return ToStringSub(methodCallExpression);
            }
            else if (lambdaExpression != null)
            {
                Console.WriteLine("lambdaExpression");

                //Console.WriteLine(lambdaExpression);
                //Console.WriteLine(lambdaExpression.Body.ToString());
                //Console.WriteLine(lambdaExpression.Name.ToString());
                //Console.WriteLine(lambdaExpression.Compile().DynamicInvoke());

            }
            if (memberExpression != null)
            {
                Console.WriteLine("memberExpression");
                //Console.WriteLine(memberExpression);

                FieldInfo fi = (FieldInfo)memberExpression.Member;
                ConstantExpression bodyExpression = memberExpression.Expression as ConstantExpression;

                return fi.Name;

                //Console.WriteLine(fi.ToString());
                //Console.WriteLine(fi.Name.ToString());
                //Console.WriteLine(fi.GetValue(bodyExpression.Value).ToString());
            }

            //var bodyMember = body.Member as FieldInfo;
            //if (bodyMember == null)
            //    throw new Exception("bodyMember is null");


            //var bodyExpression = body.Expression as ConstantExpression;
            //if (bodyExpression == null)
            //    throw new Exception("bodyExpression is null");



            return null;
        }
    }
    
}
