namespace ExpressionDumpTests
{

    public class TestThing
    {
        public const string ConstMember = "value: i am a const";

        public TestThing TestThingInner { get; set; }

        public TestThing()
        {

        }

        public TestThing(TestThing tt)
        {
            TestThingInner = tt;

        }


        public TestThing(int _)
        {
            
        }

        public string TestMethod()
        {
            return "value: hello to you too";
        }


        public static string Test2()
        {
            return "value: Test2";
        }

        public string Horatio<T>()
        {
            return default(T).ToString();
        }


        public string Horatio<T>(T unused, int number = 7)
        {
            return number.ToString();
        }
    }

}
