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
        Formula formula = new Formula("1+6-7*2-(6/3+(8-4))");
        Formula formula2 = new Formula("1+6-7.00*2-(6/3+(8.00-4))");
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
    /// This tests the exceptions that should be thrown when using operators.
    /// An exception should be thrown if there are two consecutive operators,
    /// and if an operator is next to an integer when it shouldn't be.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestOperatorsExceptions()
    {
        Formula formula = new Formula("++");
        Formula formula2 = new Formula("-5");
        Formula formula3 = new Formula("5-");
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
        Formula formula1 = new Formula("()2");
        Formula formula2 = new Formula("3()");
        Formula formula3 = new Formula("(-)");
        Formula formula4 = new Formula("(5-)");
        Formula formula5 = new Formula("(()");
        Formula formula6 = new Formula("()");
    }
    /// <summary>
    /// This tests the exceptions that should be thrown when using variables
    /// and integers. An exception should be thrown if there are two consecutive
    /// variables, if there are two consecutive integers, if a division by zero
    /// occurs, or if a given variable isn't valid.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestVarAndIntExceptions()
    {
        Formula formula1 = new Formula("a2 y3");
        Formula formula2 = new Formula("5 32");
        Formula formula3 = new Formula("6/0");
        Formula formula4 = new Formula("xx");
    }
    /// <summary>
    /// this tests the Evaluate method. The formula given should evaluate to -6.
    /// </summary>
    [TestMethod]
    public void TestEvaluate()
    {
        Formula formula = new Formula("1+6-7*2-(6/3+(8-4))");
        Assert.AreEqual((double)-6, formula.Evaluate(s => 5));
    }
    /// <summary>
    /// this tests the Evaluate method. The formula given should evaluate to -5.
    /// </summary>
    [TestMethod]
    public void TestEvaluateWithNormalize()
    {
        Formula formula = new Formula("1+x6-7*X6", normalize, IsValid);
        Assert.AreEqual((double)-29, formula.Evaluate(s => 5));
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
        Formula formula = new Formula("1.0+x6-7*2");
        Assert.AreEqual("1.0+x6.0-7.0*2.0", formula.ToString());
    }
    /// <summary>
    /// This tests the ToString method. The string returned should be the same
    /// as the string formula given.
    /// </summary>
    [TestMethod]
    public void TestToStringWithNormalize()
    {
        Formula formula = new Formula("1.0+x6-7*y2", normalize, IsValid);
        Assert.AreEqual("1.0+X6.0-7.0*Y2", formula.ToString());
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
    /// This creates the "normalize" delegate.
    /// </summary>
    /// <param name="x">The variable to be capitalized</param>
    /// <returns>A capitalized string</returns>
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
    private static bool isvalid(string x)
    {
        if (Regex.IsMatch(x, "[a-z|A-Z][0-9]"))
        {
            return true;
        }
        return false;
    }
    Func<string, string> normalize = Capitalize;
    Func<string, bool> IsValid = isvalid;
}