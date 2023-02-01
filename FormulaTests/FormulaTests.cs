using SpreadsheetUtilities;

namespace FormulaTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestEquals()
    {
        Formula formula = new Formula("1+6-7*2-(6/3+(8-4))");
        Formula formula2 = new Formula("1+6-7*2-(6/3+(8-4))");
        Formula formula3 = new Formula("1+6");
        Assert.IsTrue(formula.Equals(formula2));
        Assert.IsFalse(formula.Equals(formula3));
    }
    [TestMethod]
    public void TestOperatorsExceptions()
    {
        Formula formula = new Formula("++");
        Formula formula2 = new Formula("-5");
        Formula formula3 = new Formula("5-");
        Assert.ThrowsException<string>(new Formula("++"));
    }
    [TestMethod]
    public void TestParenthesesExceptions()
    {
        Formula formula1 = new Formula("()2");
        Formula formula2 = new Formula("3()");
        Formula formula3 = new Formula("(-)");
        Formula formula4 = new Formula("(5-)");
        Formula formula5 = new Formula("(()");
    }
    [TestMethod]
    public void TestVarAndIntExceptions()
    {
        Formula formula1 = new Formula("a2 y3");
        Formula formula2 = new Formula("5 32");
        Formula formula3 = new Formula("6/0");
        Formula formula4 = new Formula("xx");
    }
    [TestMethod]
    public void TestEvaluate()
    {
        Formula formula = new Formula("1+6-7*2-(6/3+(8-4))");
        Assert.AreEqual(formula.Evaluate(s => 5), -6);
    }
    [TestMethod]
    public void TestGetVariables()
    {
        Formula formula = new Formula("x2 + X4 + Y7");
        Assert.IsTrue(formula.GetVariables().Contains("x2"));
        Assert.IsTrue(formula.GetVariables().Contains("X4"));
        Assert.IsTrue(formula.GetVariables().Contains("Y7"));
    }
    [TestMethod]
    public void TestToString()
    {
        Formula formula = new Formula("1+x6-7*2-(6/3+(y8-4))");
        Assert.AreEqual(formula.ToString(), "1+x6-7*2-(6/3+(y8-4))");
    }
    [TestMethod]
    public void TestGetHashCode()
    {
        Formula formula = new Formula("1+6");
        Formula formula2 = new Formula("x2 + X4 + Y7");
        Assert.AreEqual(formula.GetHashCode(), 3);
        Assert.AreEqual(formula2.GetHashCode(), 2);
    }
    [TestMethod]
    public void TestOperators()
    {
        Formula formula = new Formula("1+6");
        Formula formula2 = new Formula("1+6");
        Formula formula3 = new Formula("1+5");
        Assert.IsTrue(formula == formula2);
        Assert.IsTrue(formula != formula3);
        Assert.IsFalse(formula == formula3);
        Assert.IsFalse(formula != formula2);
    }
}