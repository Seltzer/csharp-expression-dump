using System;
using System.Linq.Expressions;
using ExpressionDump.CodeFormatters;

namespace ExpressionDump
{

    public static class ExpressionDump
    {
        public static string Dump(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Console.WriteLine("Dump invoked with expression: " + expression + " of type " + expression.GetType());

            //Console.WriteLine("LOL");
            //Console.WriteLine(expression.ToString());

            var wr = new CodeWriter(new DefaultFormatter());

            wr.Visit(expression);

            return wr.ToString();
        }
    }

}
