namespace ExpressionDump.CSharpStyles
{

    
    /// <summary>
    /// This class represents purely stylistic rules which CodeWriter should follow when generating C# code
    /// </summary>
    /// ReSharper disable SimplifyConditionalTernaryExpression
    public class CSharpStyle
    {
        internal bool InsertSpaceAfterComma
        {

            get { return insertSpaceAfterComma.HasValue ? insertSpaceAfterComma.Value : true; }
            set { insertSpaceAfterComma = value; }
        }
        bool? insertSpaceAfterComma;


        internal bool SurroundOperatorsWithSpace
        {
            get { return surroundOperatorsWithSpace.HasValue ? surroundOperatorsWithSpace.Value : true; }
            set { surroundOperatorsWithSpace = value; }
        }
        bool? surroundOperatorsWithSpace;


        /// <summary>
        /// Determines whether the empty parentheses are stripped in expressions like 'new Widget() { Blah = 2 }'
        /// </summary>
        internal bool StripRedundantEmptyParentheses
        {
            get { return stripRedundantEmptyParentheses.HasValue ? stripRedundantEmptyParentheses.Value : true; }
            set { stripRedundantEmptyParentheses = value; }
        }
        bool? stripRedundantEmptyParentheses;
    }
    // ReSharper restore SimplifyConditionalTernaryExpression

}
