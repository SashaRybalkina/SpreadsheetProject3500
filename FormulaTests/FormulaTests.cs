///<summary>
///These are tests for Formula.cs
///Written by Sasha Rybalkina, Febuary 2023.
///<summary>
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;

namespace FormulaTests;

[TestClass]
public class UnitTest1
{
    /// <summary>
    /// This tests the Equals method. fomula and formula2 should be considered
    /// equal, and fomula and formula3 shouldn't be.
    /// </summary>
    [TestMethod]
    public void TestEquals()
    {
        Formula formula = new Formula("1+6-7*2-(6/3+(8-4))-1");
        Formula formula2 = new Formula("1+6-7.00*2-(6/3+(8.00-4))-1");
        Formula formula3 = new Formula("1+6");
        Assert.IsTrue(formula.Equals(formula2));
        Assert.IsFalse(formula.Equals(formula3));
    }
    /// <summary>
    /// This tests the Equals method. fomula and formula2 should be considered
    /// equal, and fomula and formula3 shouldn't be.
    /// </summary>
    [TestMethod]
    public void TestEqualsWithNormalize()
    {
        Formula formula = new Formula("1+x6-7*y2", normalize, IsValid);
        Formula formula2 = new Formula("1.00+X6-7.00*Y2");
        Formula formula3 = new Formula("1+6");
        Assert.IsTrue(formula.Equals(formula2));
        Assert.IsFalse(formula.Equals(formula3));
    }
    /// <summary>
    /// This tests the FormulaErrors that should get returned if a division by
    /// zero occurs, if an empty lookup is given, or if a variable cannot be
    /// looked up.
    /// </summary>
    [TestMethod]
    public void TestFormulaErrors()
    {
        Formula formula = new Formula("x5");
        Formula formula2 = new Formula("1/(x6-x6)");
        Assert.AreEqual(formula.Evaluate(null), new FormulaError("Parameter 'lookup' cannot be null"));
        Assert.AreEqual(formula.Evaluate(s => throw new ArgumentException()), new FormulaError("Unable to look up variable"));
        Assert.AreEqual(formula2.Evaluate(lookup), new FormulaError("Division by zero occurred"));
    }
    /// <summary>
    /// This tests the exception that should be thrown if an empty formula
    /// is given.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestEmptyFormulaException()
    {
        Formula formula = new Formula("");
    }
    /// <summary>
    /// This tests the exception that should be thrown if a null formula is
    /// given.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestNullFormulaException()
    {
        Formula formula = new Formula(null);
    }
    /// <summary>
    /// This tests the exception that should be thrown if a normalization is
    /// invalid.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestInvalidNormalizationException()
    {
        Formula formula = new Formula("x6", normalize, s => false);
    }
    /// <summary>
    /// This tests the exceptions that should be thrown when using operators.
    /// An exception should be thrown if there are two consecutive operators,
    /// if a division by zero occurs, and if an operator is next to an integer
    /// when it shouldn't be.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestOperatorsExceptions()
    {
        Formula formula = new Formula("++");
    }
    //Exception test for a negative number entered
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestOperatorsExceptions2()
    {
        Formula formula = new Formula("-5");
    }
    //Exception test for an operator at last index
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestOperatorsExceptions3()
    {
        Formula formula = new Formula("5-");
    }
    //Exception test for division by zero
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestoperatorstExceptions4()
    {
        Formula formula3 = new Formula("6/0");
    }
    /// <summary>
    /// This tests the exceptions that should be thrown when using parentheses.
    /// An exception should be thrown if an integer is outside of parentheses,
    /// if an operator is in front of a left parenthesis or behind a right
    /// parenthesis, if a pair of closed parentheses is empty, and if there is
    /// an uneven amount of left and right parentheses.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions()
    {
        Formula formula = new Formula("()2");
    }
    //Exception test for integer outside of parentheses
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions2()
    {
        Formula formula = new Formula("3()");
    }
    //Exception test for operator is in front of a left parenthesis
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions3()
    {
        Formula formula = new Formula("(-)");
    }
    //Exception test operator is behind a right parentheses
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions4()
    {
        Formula formula = new Formula("(5-)");
    }
    //Exception test for unbalanced parentheses
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions5()
    {
        Formula formula = new Formula("(()");
    }
    //Exception test for empty parentheses
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions6()
    {
        Formula formula = new Formula("()");
    }
    //Exception tests for variables outside of parentheses
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions7()
    {
        Formula formula = new Formula("()x2");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestParenthesesExceptions8()
    {
        Formula formula = new Formula("y3()");
    }
    /// <summary>
    /// This tests the exceptions that should be thrown when using variables
    /// and integers. An exception should be thrown if there are two consecutive
    /// variables, if there are two consecutive integers, if an integer is next
    /// to a variable, or if a given variable isn't valid.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestVarAndIntExceptions()
    {
        Formula formula1 = new Formula("a2 y3");
    }
    //Exception test for consecutive integers
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestVarAndIntExceptions2()
    {
        Formula formula2 = new Formula("5 32");
    }
    //Exception test for invalid variable
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestVarAndIntExceptions3()
    {
        Formula formula4 = new Formula("xx");
    }
    //Exception tests for a variable next to an integer
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestVarAndIntExceptions4()
    {
        Formula formula4 = new Formula("x3 56");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestVarAndIntExceptions5()
    {
        Formula formula4 = new Formula("56 x3");
    }
    /// <summary>
    /// this tests the Evaluate method. The formula given should evaluate to -6.
    /// </summary>
    [TestMethod]
    public void TestEvaluate()
    {
        Formula formula = new Formula("x1+6-7*2-(6/3+(8-4))+1"); 
        Assert.AreEqual((double)-12, formula.Evaluate(s => 1));
    }
    /// <summary>
    /// this tests the Evaluate method. The formula given should evaluate to -5.
    /// </summary>
    [TestMethod]
    public void TestEvaluateWithNormalize()
    {
        Formula formula = new Formula("1*x6*7*X6", normalize, IsValid);
        Assert.AreEqual((double)63, formula.Evaluate(lookup));
    }
    /// <summary>
    /// This tests the GetVariables method. The formula given has the variables
    /// "x2", "X4", and "Y7", so get variables should return a list containing
    /// those variables.
    /// </summary>
    [TestMethod]
    public void TestGetVariables()
    {
        Formula formula = new Formula("x2 + X4 + Y7");
        Assert.IsTrue(formula.GetVariables().Contains("x2"));
        Assert.IsTrue(formula.GetVariables().Contains("X4"));
        Assert.IsTrue(formula.GetVariables().Contains("Y7"));
    }
    /// <summary>
    /// This tests the GetVariables method. The formula given has the variables
    /// "x4", "X4", and "y7", so get variables should return a list containing
    /// X4 and Y7.
    /// </summary>
    [TestMethod]
    public void TestGetVariablesWithNormalize()
    {
        Formula formula = new Formula("x4 + X4 + y7", normalize, IsValid);
        Assert.IsTrue(formula.GetVariables().Contains("X4"));
        Assert.IsTrue(formula.GetVariables().Contains("Y7"));
        Assert.IsFalse(formula.GetVariables().Contains("y7"));
        Assert.IsFalse(formula.GetVariables().Contains("x4"));
    }
    /// <summary>
    /// This tests the ToString method. The string returned should be the same
    /// as the string formula given.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Formula formula = new Formula("1.0+x6-7E+0*2.0");
        Assert.AreEqual("1+x6-7*2", formula.ToString());
    }
    /// <summary>
    /// This tests the ToString method. The string returned should be the same
    /// as the string formula given.
    /// </summary>
    [TestMethod]
    public void TestToStringWithNormalize()
    {
        Formula formula = new Formula("2E+2+x6-7*y2", normalize, IsValid);
        Assert.AreEqual("200+X6-7*Y2", formula.ToString());
    }
    /// <summary>
    /// This tests the GetHashCode method. The method should create a hash code
    /// based on the length of the string and the remainder of dividing the
    /// length by 10.
    /// </summary>
    [TestMethod]
    public void TestGetHashCode()
    {
        Formula formula = new Formula("x2 + X4 + y7", normalize, IsValid);
        Formula formula2 = new Formula("X2 + X4 + Y7");
        Formula formula3 = new Formula("X3 + X4 + Y7");
        Assert.AreEqual(formula2.GetHashCode(), formula.GetHashCode());
        Assert.AreNotEqual(formula3.GetHashCode(), formula.GetHashCode());
    }
    /// <summary>
    /// This tests the operators of the class. == should work exactly like the
    /// Equals method, and != should work as the opposite of the Equals method.
    /// </summary>
    [TestMethod]
    public void TestOperators()
    {
        Formula formula = new Formula("1+6");
        Formula formula2 = new Formula("1+6.0");
        Formula formula3 = new Formula("1+5");
        Assert.IsTrue(formula == formula2);
        Assert.IsTrue(formula != formula3);
        Assert.IsFalse(formula == formula3);
        Assert.IsFalse(formula != formula2);
    }
    /// <summary>
    /// This tests the operators of the class. == should work exactly like the
    /// Equals method, and != should work as the opposite of the Equals method.
    /// </summary>
    [TestMethod, Timeout(5000)]
    public void StressTest()
    {
        Formula formula = new Formula("4+6 *6- 0");
        Formula formula2 = new Formula("4+6.0 * 6E+0 -0");
        Formula formula3 = new Formula("10 * 2+ 7*3 - 1");
        Assert.IsTrue(formula == formula2);
        Assert.IsTrue(formula != formula3);
        object form1Eval = formula.Evaluate(s => 5);
        object form2Eval = formula2.Evaluate(s => 5);
        object form3Eval = formula3.Evaluate(s => 5);
        Assert.AreEqual((double)form1Eval, (double)form3Eval);
        Assert.AreEqual((double)form2Eval, (double)form3Eval);
    }
    /// <summary>
    /// This creates the "normalize" delegate.
    /// </summary>
    /// <param name="x">The variable to be capitalized</param>
    /// <returns>A capitalized variable</returns>
    private static string Capitalize(string x)
    {
        string output = "";
        Char[] input = x.ToCharArray();
        for (int i = 0; i < input.Length; i++)
        {
            if (i == 0)
            {
                input[i] = char.ToUpper((input[i]));
            }
            output = output + input[i];
        }
        return output;
    }
    /// <summary>
    /// This creates the "IsValid" delegate.
    /// </summary>
    /// <param name="x">The variable being checked</param>
    /// <returns>True if the variable is valid, false otherwise.</returns>
    private static bool isvalid(string x)
    {
        if (Regex.IsMatch(x, "[a-z|A-Z][0-9]"))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// This craetes the "Lookup" delegate.
    /// </summary>
    /// <param name="x">The variable being looked up</param>
    /// <returns>An integer value for a given variable</returns>
    private static double Lookup(string x)
    {
        if (x == "x6")
        {
            return 5;
        }
        else
        {
            return 3;
        }
    }
    Func<string, double> lookup = Lookup;
    Func<string, string> normalize = Capitalize;
    Func<string, bool> IsValid = isvalid;
}
