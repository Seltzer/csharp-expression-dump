using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ExpressionDump.CodeFormatters;
using ExpressionDump.Core;

namespace ExpressionDump
{
    
    class CodeWriter : ExpressionVisitor
    {
        readonly ICodeFormatter formatter;
        readonly StringBuilder sb = new StringBuilder();
        

        internal CodeWriter(ICodeFormatter formatter)
        {
            this.formatter = formatter;
        }


        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }


        protected override Expression VisitNew(NewExpression node)
        {
            sb.Append("new ");
            
            VisitType(node.Constructor.DeclaringType);
            VisitArguments(node.Arguments);

            return node;
        }


        protected override Expression VisitLambda<T>(Expression<T> lambdaExpression)
        {
            MethodInfo mi = ((LambdaExpression)lambdaExpression).Compile().Method;
            IEnumerable<Type> typeParameters = mi.GetParameters()
                .Skip(1)    // The first parameter is always of type Closure - this is a C# implementation detail and we want to ignore it
                .Select(p => p.ParameterType)
                .If(() => lambdaExpression.ReturnType != null, types => types.Append(lambdaExpression.ReturnType));

            string lambdaType = lambdaExpression.ReturnType != null ? "Func" : "Action";
            
            sb.Append("new " + lambdaType);
            VisitTypeParameterArguments(typeParameters);
            sb.Append("(");
            VisitArguments(lambdaExpression.Parameters);
            sb.Append(" => ");
            Visit(lambdaExpression.Body);
            sb.Append(")");

            return lambdaExpression;
        }


        void VisitType(Type type)
        {
            sb.Append(formatter.TypeToString(type));
        }


        void VisitArguments(IEnumerable<Expression> expressions)
        {
            VisitParenthesizedList(expressions, e => Visit(e));
        }


        void VisitTypeParameterArguments(IEnumerable<Type> typeParameterArguments)
        {
            VisitList(typeParameterArguments, "<", VisitType, ">");
        }


        void VisitParenthesizedList<T>(IEnumerable<T> list, Action<T> writer)
        {
            VisitList(list, "(", writer, ")");
        }


        void VisitList<T>(IEnumerable<T> list, string opening, Action<T> writer, string closing)
        {
            sb.Append(opening);
            
            for (int i = 0; i < list.Count(); i++)
            {
                if (i > 0)
                    sb.Append(formatter.CommaSeparatorToString());

                writer(list.ElementAt(i));
            }

            sb.Append(closing);
        }
        


        public override string ToString()
        {
            return sb.ToString();
        }
    }

}
