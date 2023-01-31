using FormulaEvaluator;
namespace FormulaEvaluatorTester;
class Program
{
    static void Main(string[] args)
    {
        ///Expressions with single digits
        if (Evaluator.Evaluate("5*5/5", s => 5) == 5) Console.WriteLine("Correct Result for Test 1");
        if (Evaluator.Evaluate("(4/2+7)", s => 5) == 9) Console.WriteLine("Correct Result for Test 2");
        if (Evaluator.Evaluate("4/2+7-0*8", s => 5) == 9) Console.WriteLine("Correct Result for Test 3");
        if (Evaluator.Evaluate("5/5/5", s => 5) == 0) Console.WriteLine("Correct Result for Test 4");
        if (Evaluator.Evaluate("0/(9+8)*1", s => 5) == 0) Console.WriteLine("Correct Result for Test 5");

        ///Expressions with double and more digits
        if (Evaluator.Evaluate("12*345*6", s => 5) == 24840) Console.WriteLine("Correct Result for Test 6");
        if (Evaluator.Evaluate("1234 - (5)", s => 5) == -1229) Console.WriteLine("Correct Result for Test 7");
        if (Evaluator.Evaluate("((18 - 48) / (6 * 5))", s => 5) == -1) Console.WriteLine("Correct Result for Test 8");
        if (Evaluator.Evaluate("123 + 45", s => 5) == 168) Console.WriteLine("Correct Result for Test 9");
        if (Evaluator.Evaluate("444444/444444", s => 5) == 1) Console.WriteLine("Correct Result for Test 10");

        ///Exceptions
        try
        {
            Evaluator.Evaluate("5/0", s => 5);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Correct Result for Test 11");
        }
        try
        {
            Evaluator.Evaluate("(6-9))", s => 5);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Correct Result for Test 12");
        }
        try
        {
            Evaluator.Evaluate("++++", s => 5);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Correct Result for Test 13");
        }

        ///Variables and delagates
        if (Evaluator.Evaluate("5*s/5", s => 5) == 5) Console.WriteLine("Correct Result for Test 14");
        if (Evaluator.Evaluate("s/2+7", s => 4) == 9) Console.WriteLine("Correct Result for Test 15");
        if (Evaluator.Evaluate("4/2+s-0*8", s => 7) == 9) Console.WriteLine("Correct Result for Test 16");
        if (Evaluator.Evaluate("(5/s)/5", s => 5) == 0) Console.WriteLine("Correct Result for Test 17");
        if (Evaluator.Evaluate("s/9+8*0", s => 0) == 0) Console.WriteLine("Correct Result for Test 18");
        if (Evaluator.Evaluate("(s)+45", s => 123) == 168) Console.WriteLine("Correct Result for Test 19");
        if (Evaluator.Evaluate("s/s", s => 444444) == 1) Console.WriteLine("Correct Result for Test 20");

        Console.Read();
    }
}
