using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using ExpressionDump.CodeFormatters;
using ExpressionDump.Core;

namespace ExpressionDump
{

    public static class ExpressionDump
    {
        static bool ShouldLog = true;

        static void Log(string msg)
        {
#if DEBUG
            if (ShouldLog)
                Console.WriteLine(msg);
#endif
        }


        public static void KillLogging()
        {
            ShouldLog = false;
        }


        public static void EnableLogging()
        {
            ShouldLog = true;
        }



        public static string Dump(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Log("Dump invoked with expression: " + expression + " of type " + expression.GetType());

            var wr = new CodeWriter(new DefaultFormatter());

            wr.Visit(expression);

            return wr.ToString();


            return SwitchOnExpressionType<string>(expression,
                methodCallLambda: methodCallExpression =>
                {
                    Log("methodCallExpression");
                    
                    string methodSubject, methodName = methodCallExpression.Method.Name, args;

                    if (!methodCallExpression.Method.IsExtensionMethod())
                    {
                        methodSubject = methodCallExpression.Object.IfNotNull(o => o.ToString()) ?? methodCallExpression.Method.ReflectedType.Name;
                        args = DumpArgsOrParameters(methodCallExpression.Arguments);
                    }
                    else
                    {
                        methodSubject = Dump(methodCallExpression.Arguments[0]);
                        args = DumpArgsOrParameters(methodCallExpression.Arguments.Skip(1));
                    }
                    
                    return String.Format("{0}.{1}{2}{3}", methodSubject, methodName, 
                        DumpTypeArgsOrParameters(methodCallExpression.Method.GetGenericArguments()), 
                        args);
                },

                constantExprLambda: constantExpression =>
                {
                    Log("constantExpression");

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

                    return Dump(invocationExpression.Expression) + DumpArgsOrParameters(invocationExpression.Arguments);    
                },


                lambdaExprLambda: lambdaExpression =>
                {
                    Log("lambdaExpression");
                   
                    MethodInfo mi = lambdaExpression.Compile().Method;
                    IEnumerable<Type> typeParameters = mi.GetParameters()
                        .Skip(1)    // The first parameter is always of type Closure - this is a C# implementation detail and we want to ignore it
                        .Select(p => p.ParameterType)
                        .If(() => lambdaExpression.ReturnType != null, types => types.Append(lambdaExpression.ReturnType));

                    string lambdaType = lambdaExpression.ReturnType != null ? "Func" : "Action";
                    
                    return String.Format("new {0}{1}({2} => {3})", lambdaType, DumpTypeArgsOrParameters(typeParameters), 
                        DumpArgsOrParameters(lambdaExpression.Parameters, false), Dump(lambdaExpression.Body));
                },


                parameterExprLambda: parameterExpression =>
                {
                    return parameterExpression.Name;
                },


                binaryExprLambda: binaryExpression =>
                {
                    // TODO
                    string op = binaryExpression.NodeType == ExpressionType.Add || binaryExpression.NodeType == ExpressionType.AddChecked
                        ? "+"
                        : null;
                    
                    return String.Format("{0} {1} {2}", Dump(binaryExpression.Left), op, Dump(binaryExpression.Right));
                },

                
                defaultLambda: e =>
                {
                    Log("Yielding unknown");
                    return "UNKNOWN - " + e.GetType();
                }
            );
        }

        
        static string DumpArgsOrParameters(IEnumerable<Expression> expressions, bool requireBracketsForOne = true)
        {
            if (!requireBracketsForOne && expressions.Count() == 1)
                return Dump(expressions.First());

            return "(" + expressions.Select(Dump).StringJoin(", ") + ")";
        }


        static string DumpTypeArgsOrParameters(IEnumerable<Type> types)
        {
            return types.Any() ? "<" + types.Select(TypeExt.GetTypeAliasOrSelf).StringJoin(", ") + ">" : "";
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
            Func<BinaryExpression, T> binaryExprLambda = null,
            Func<Expression, T> defaultLambda = null)
        {
            MethodCallExpression methodCallExpression = null;
            LambdaExpression lambdaExpression = null;
            MemberExpression memberExpression = null;
            ConstantExpression constantExpression = null;
            NewExpression newExpression = null;
            InvocationExpression invocationExpression = null;
            ParameterExpression parameterExpression = null;
            BinaryExpression binaryExpression = null;
            

            // Short circuited type checking / assignment - more efficient than executing n is checks and 1 as conversion.
            object blah = (object)(methodCallExpression = expr as MethodCallExpression)
                ?? (object)(lambdaExpression = expr as LambdaExpression)
                ?? (object)(memberExpression = expr as MemberExpression)
                ?? (object)(newExpression = expr as NewExpression)
                ?? (object)(lambdaExpression = expr as LambdaExpression)
                ?? (object)(constantExpression = expr as ConstantExpression)
                ?? (object)(parameterExpression = expr as ParameterExpression)
                ?? (object)(binaryExpression = expr as BinaryExpression)
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
            else if (binaryExpression != null)
                return binaryExprLambda(binaryExpression);
            else
                return defaultLambda(expr);
        }
    }

}
