using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ExpressionDump.Core;

namespace ExpressionDump
{

    public static class ExpressionDump
    {
        static void Log(string msg)
        {
#if DEBUG
            Console.WriteLine(msg);
#endif
        }


        public static string Dump(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Log("Dump invoked with expression: " + expression.ToString());

            return SwitchOnExpressionType<string>(expression,
                methodCallLambda: methodCallExpression =>
                {
                    Log("methodCallExpression");
                    
                    string methodSubject, methodName = methodCallExpression.Method.Name, args;

                    if (!methodCallExpression.Method.IsExtensionMethod())
                    {
                        methodSubject = methodCallExpression.Object.IfNotNull(o => o.ToString()) ?? methodCallExpression.Method.ReflectedType.Name;
                        args = DumpParameters(methodCallExpression.Arguments);
                    }
                    else
                    {
                        methodSubject = Dump(methodCallExpression.Arguments[0]);
                        args = DumpParameters(methodCallExpression.Arguments.Skip(1));
                    }
                    
                    return String.Format("{0}.{1}{2}", methodSubject, methodName, args);
                },

                constantExprLambda: constantExpression =>
                {
                    Log("constantExpression");
                    Log(constantExpression.Type.ToString());
                    Log(constantExpression.Value.ToString());
                    Log(constantExpression.NodeType.ToString());
                    Log(constantExpression.ToString());

                    Log(constantExpression.ToString());

                    return constantExpression.ToString();
                },

                newExpressionLambda: newExpression =>
                {
                    Log("newExpression");

                    // Doesn't work with object initialisation syntax
                    return String.Format("new {0}{1}({2})", newExpression.Type, "", newExpression.Arguments.Select(Dump).StringJoin());
                },

                memberExprLambda: memberExpression =>
                {
                    Log("memberExpression");

                    //Log("Dumping sub" + Dump(memberExpression.Expression));
                    
                
                    return memberExpression.Member.Name;
                },

                invocationExprLambda: invocationExpression =>
                {
                    Log("invocationExpression");

                    return invocationExpression.Expression.IfNotNull(Dump) + invocationExpression.Arguments.IfNotNull(DumpParameters);    
                },


                lambdaExprLambda: lambdaExpression =>
                {
                    Log("lambdaExpression");
                    
                    return String.Format("{0} => {1}", DumpParameters(lambdaExpression.Parameters), Dump(lambdaExpression.Body));
                },


                parameterExprLambda: parameterExpression =>
                {
                    return parameterExpression.Name;
                },

                
                defaultLambda: e => "UNKNOWN - " + e.GetType()
            );
        }


        static string DumpParameters(IEnumerable<Expression> expressions)
        {
            return "(" + expressions.Select(Dump).StringJoin(", ") + ")";
        }

        /// <summary>
        /// Efficient and convenient way to switch on an expression based on its type
        /// </summary>
        static T SwitchOnExpressionType<T>(Expression expr, Func<MethodCallExpression, T> methodCallLambda = null,
            Func<ConstantExpression, T> constantExprLambda = null,
            Func<MemberExpression, T> memberExprLambda = null,
            Func<NewExpression, T> newExpressionLambda = null,
            Func<LambdaExpression, T> lambdaExprLambda = null,
            Func<InvocationExpression, T> invocationExprLambda = null,
            Func<ParameterExpression, T> parameterExprLambda = null,
            Func<Expression, T> defaultLambda = null)
        {
            MethodCallExpression methodCallExpression = null;
            LambdaExpression lambdaExpression = null;
            MemberExpression memberExpression = null;
            ConstantExpression constantExpression = null;
            NewExpression newExpression = null;
            InvocationExpression invocationExpression = null;
            ParameterExpression parameterExpression = null;
            

            // Short circuited type checking / assignment - more efficient than executing n is checks and 1 as conversion.
            object blah = (object)(methodCallExpression = expr as MethodCallExpression)
                ?? (object)(lambdaExpression = expr as LambdaExpression)
                ?? (object)(memberExpression = expr as MemberExpression)
                ?? (object)(newExpression = expr as NewExpression)
                ?? (object)(lambdaExpression = expr as LambdaExpression)
                ?? (object)(constantExpression = expr as ConstantExpression)
                ?? (object)(parameterExpression = expr as ParameterExpression)
                ?? (invocationExpression = expr as InvocationExpression)
                ;
                

            if (methodCallExpression != null)
                return methodCallLambda(methodCallExpression);
            else if (constantExpression != null)
                return constantExprLambda(constantExpression);
            else if (newExpression != null)
                return newExpressionLambda(newExpression);
            else if (memberExpression != null)
                return memberExprLambda(memberExpression);
            else if (lambdaExpression != null)
                return lambdaExprLambda(lambdaExpression);
            else if (invocationExpression != null)
                return invocationExprLambda(invocationExpression);
            else if (parameterExpression != null)
                return parameterExprLambda(parameterExpression);
            else
                return defaultLambda(expr);
        }
    }

}
