using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ExpressionDump.CodeWriterConfig;
using ExpressionDump.Core;

namespace ExpressionDump
{
    
    class CodeWriter : ExpressionVisitor
    {
        readonly CodeWriterConfig.CodeWriterConfig config;
        readonly StringBuilder sb = new StringBuilder();
        

        internal CodeWriter(CodeWriterConfig.CodeWriterConfig config = null)
        {
            this.config = config ?? new CodeWriterConfig.CodeWriterConfig();
        }


        public override Expression Visit(Expression node)
        {
            if (node != null)
                Console.WriteLine("Visiting expression of type " + node.GetType());
            
            return base.Visit(node);
        }
        

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }


        
        protected override Expression VisitNew(NewExpression node)
        {
            return VisitNew(node, null);
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
        }


        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                sb.Append("new ");
                VisitType(node.Type);
                Space();

                VisitBracedList(node.Expressions, expression => Visit(expression));
            }
            else if (node.NodeType == ExpressionType.NewArrayBounds)
            {
                sb.Append("new ");
                VisitType(node.Type);
                Space();
                VisitBracketedList(node.Expressions, expression => Visit(expression));
            }


            return node;
        }


        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            VisitNew(node.NewExpression, true);
            // TODO: Preceding space
            // TODO: Style for braces spaces
            VisitCommaSeparatedList(node.Bindings, " { ", b => VisitMemberBinding(b), " }");

            return node;
        }


        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            sb.Append(node.Member.Name);
            sb.Append(config.CodeFormatter.OperatorToString("="));

            Visit(node.Expression);

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
            sb.Append(config.CodeFormatter.OperatorToString(GetBinaryOperator(node.NodeType)));
            Visit(node.Right);

            return node;
        }


        protected override Expression VisitConditional(ConditionalExpression node)
        {
            bool isTernary = node.Type != typeof(void) && (node.IfTrue.NodeType != ExpressionType.Block
                || (node.IfFalse != null && node.IfFalse.NodeType != ExpressionType.Block));

            if (!isTernary)
            {
                sb.Append("if (");
                Visit(node.Test);
                sb.Append(") { ");

                Visit(node.NodeType == ExpressionType.Block ? node : (Expression)Expression.Block(node.IfTrue));
            
                //if (node.IfFalse != null)
                //{
                //    sb.Append("else ");

                //    Visit(node.NodeType == ExpressionType.Block ? node : (Expression)Expression.Block(node.IfFalse));
                //} 
            }
            else
            {
                sb.Append("(");
                Visit(node.Test);
                sb.Append(" ? ");
                Visit(node.IfTrue);
                sb.Append(" : ");
                Visit(node.IfFalse);
                sb.Append(")");
            }

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
            sb.Append(TypeToString(type));

            if (type.IsGenericType)
                VisitTypeParameterArguments(type.GetGenericArguments());
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


        void VisitBracedList<T>(IEnumerable<T> list, Action<T> writer)
        {
            VisitCommaSeparatedList(list, "{ ", writer, " }");
        }


        void VisitBracketedList<T>(IEnumerable<T> list, Action<T> writer)
        {
            VisitCommaSeparatedList(list, "[", writer, "]");
        }


        void VisitCommaSeparatedList<T>(IEnumerable<T> list, string opening, Action<T> writer, string closing)
        {
            sb.Append(opening);
            
            list.ForEach(
                allItemsAction: writer, 
                inBetweenItemsAction: () => sb.Append(config.CodeFormatter.CommaSeparatorToString()));

            sb.Append(closing);
        }
        
        
        Expression VisitNew(NewExpression node, bool? isInContextOfMemberInitialiser)
        {
            sb.Append("new ");
            
            VisitType(node.Constructor.DeclaringType);

            // In an expression like 'new Widget() { Blah = 2 }', we might want to omit the parentheses
            bool canOmitParentheses = isInContextOfMemberInitialiser.HasValue && isInContextOfMemberInitialiser.Value && node.Arguments.Count == 0;

            if (!canOmitParentheses || !config.Style.StripRedundantEmptyParentheses)
                VisitMethodParametersOrArguments(node.Arguments);

            return node;
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


        string TypeToString(Type type)
        {
            if (type.IsArray)
                return TypeToString(type.GetElementType()) + "[]";
                    
            if (aliasDictionary.ContainsKey(type))
                return aliasDictionary[type];

            if (type.IsGenericType)
                return type.Name.UpToButExcludingLast("`");
    
            return type.ToString();
        }


        static readonly Dictionary<Type, string> aliasDictionary = new Dictionary<Type, string>()
        {
            { typeof(int), "int" },
            { typeof(string), "string" },
        };
    }

}
