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


        public override Expression Visit(Expression node)
        {
            if (node != null)
                Console.WriteLine("Visiting expression of type " + node.GetType());

            //if (node is FieldExpression)
            //{
            //    sb.Append((node as ParameterExpression).Name);

            //    return node;
            //}

            return base.Visit(node);
        }
        

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }


        protected override Expression VisitNew(NewExpression node)
        {
            sb.Append("new ");
            
            
            VisitType(node.Constructor.DeclaringType);
            VisitMethodParametersOrArguments(node.Arguments);

            return node;
        }
        

        protected override Expression VisitConstant(ConstantExpression node)
        {
            sb.Append(node);

            return base.VisitConstant(node);
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            sb.Append(node.Member.Name);

            return node;
            return base.VisitMember(node);
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
            VisitMethodParametersOrArguments(lambdaExpression.Parameters);
            sb.Append(" => ");
            Visit(lambdaExpression.Body);
            sb.Append(")");

            return lambdaExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            sb.Append(node.Name);

            return base.VisitParameter(node);
        }


        protected override Expression VisitInvocation(InvocationExpression node)
        {
            Visit(node.Expression);
            VisitMethodParametersOrArguments(node.Arguments);

            return node;
        }

        
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            sb.Append(formatter.OperatorToString(GetBinaryOperator(node.NodeType)));
            Visit(node.Right);

            return node;
        }


        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;

            if (method.IsStatic)
            {
                if (!method.IsExtensionMethod())
                    VisitType(method.DeclaringType);
                else
                    Visit(node.Arguments.First());
            }
            else
            {
                Visit(node.Object);                     
            }
                
            
            sb.Append("." + method.Name);

            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
                VisitTypeParameterArguments(method.GetGenericArguments());
            
            if (!node.Method.IsExtensionMethod())
                VisitMethodParametersOrArguments(node.Arguments);
            else
                VisitMethodParametersOrArguments(node.Arguments.Skip(1));
           
            return node;
        }
        

        void VisitType(Type type)
        {
            if (type.IsGenericType)
            {
                // FIXME
                sb.Append(type.Name.UpToButExcludingLast("`"));

                VisitTypeParameterArguments(type.GetGenericArguments());
            }
            else
            {
                sb.Append(formatter.TypeToString(type));
            }
        }

        
        void VisitMethodParametersOrArguments(IEnumerable<Expression> expressions)
        {
            VisitParenthesisedList(expressions, e => Visit(e));
        }


        void VisitTypeParameterArguments(IEnumerable<Type> typeParameterArguments)
        {
            VisitCommaSeparatedList(typeParameterArguments, "<", VisitType, ">");
        }
        

        void VisitParenthesisedList<T>(IEnumerable<T> list, Action<T> action)
        {
            VisitCommaSeparatedList(list, "(", action, ")");
        }


        void VisitCommaSeparatedList<T>(IEnumerable<T> list, string opening, Action<T> writer, string closing)
        {
            sb.Append(opening);
            
            list.ForEach(
                allItemsAction: writer, 
                inBetweenItemsAction: () => sb.Append(formatter.CommaSeparatorToString()));

            sb.Append(closing);
        }


        void Space()
        {
            sb.Append(" ");
        }

        
        static string GetBinaryOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                    return "+=";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.AndAssign:
                    return "&=";
                case ExpressionType.Assign:
                    return "=";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.DivideAssign:
                    return "/=";
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.ExclusiveOrAssign:
                    return "^=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.LeftShiftAssign:
                    return "<<=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ModuloAssign:
                    return "%=";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                    return "*=";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrAssign:
                    return "|=";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.RightShift:
                    return ">>";
                case ExpressionType.RightShiftAssign:
                    return ">>=";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    return "-=";
                default:
                    throw new NotImplementedException(type.ToString());
            }
        }

        
        public override string ToString()
        {
            return sb.ToString();
        }
    }

}
