namespace ExpressionDump.CSharpStyles
{

    // ReSharper disable SimplifyConditionalTernaryExpression
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
    }
    // ReSharper restore SimplifyConditionalTernaryExpression

}
