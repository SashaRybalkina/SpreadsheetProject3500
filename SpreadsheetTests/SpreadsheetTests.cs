/// <summary>
/// Author:    Sasha Rybalkina
/// Partner:   None
/// Date:      Febuary 9, 2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Sasha Rybalkina - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Sasha Rybalkina, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
/// Tests for the SetCellContents methods
/// Tests for GetCellContents
/// Tests for GetNamesOfAllEmptyCells
/// Tests for GetDirectDependents
/// Tests for throwing exceptions when they should be thrown
/// </summary>
using SpreadsheetUtilities;
using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpreadsheetTests;

[TestClass]
public class methods
{
    Spreadsheet s = new();

    /// <summary>
    /// Tests the SetCellContents method that passes in a double.
    /// Cell A1 should contain 4, cell A2 should contain 5, and cell
    /// A3 should contain 7.
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsWithDouble()
    {
        s.SetCellContents("A1", 4);
        s.SetCellContents("A2", 5);
        s.SetCellContents("A3", 6);
        s.SetCellContents("A3", 7);

        Assert.AreEqual((double)4, s.GetCellContents("A1"));
        Assert.AreEqual((double)5, s.GetCellContents("A2"));
        Assert.AreEqual((double)7, s.GetCellContents("A3"));
    }
    /// <summary>
    /// Tests the SetCellContents method that passes in a string.
    /// Cell A1 should contain "4+2+7", cell A2 should contain
    /// "7-5-2", and cell A3 should contain "0*0*0".
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsWithString()
    {
        s.SetCellContents("A1", "4+2+7");
        s.SetCellContents("A2", "7-5-2");
        s.SetCellContents("A3", "1*1*1");
        s.SetCellContents("A3", "0*0*0");
        s.SetCellContents("A4", "");

        Assert.AreEqual("4+2+7", s.GetCellContents("A1"));
        Assert.AreEqual("7-5-2", s.GetCellContents("A2"));
        Assert.AreEqual("0*0*0", s.GetCellContents("A3"));
        Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("A4"));
    }
    /// <summary>
    /// Tests the SetCellContents method that passes in a string.
    /// Cell A1 should contain a Formula object with the expression
    /// "4+2+7", cell A2 should contain a Formula object with the
    /// expression "7-5-2", and cell A3 should contain a Formula object
    /// with the expression "0*0*0".
    /// </summary>
    [TestMethod]
    public void TestSetCellContentsWithFormula()
    {
        s.SetCellContents("A1", new Formula("4+2+7"));
        s.SetCellContents("A2", new Formula("7-5-2"));
        s.SetCellContents("A3", new Formula("1*1*1"));
        s.SetCellContents("A3", new Formula("0*0*0"));

        Assert.AreEqual(new Formula("4+2+7"), s.GetCellContents("A1"));
        Assert.AreEqual(new Formula("7-5-2"), s.GetCellContents("A2"));
        Assert.AreEqual(new Formula("0*0*0"), s.GetCellContents("A3"));
    }
    /// <summary>
    /// Tests the GetCellContents method. Invoking this method on cell
    /// A1 should return the string "eeeeeeeeee"
    /// </summary>
    [TestMethod]
    public void TestGetCellContents()
    {
        s.SetCellContents("A1", "eeeeeeeeee");
        Assert.AreEqual("eeeeeeeeee", s.GetCellContents("A1"));
    }
    /// <summary>
    /// Tests the GetNamesOfAllEmptyCells method. The list returned
    /// should contain A1, A2, and A3.
    /// </summary>
    [TestMethod]
    public void TestGetNamesOfAllEmptyCells()
    {
        s.SetCellContents("A1", new Formula("4+2+7"));
        s.SetCellContents("A2", new Formula("7-5-2"));
        s.SetCellContents("A3", new Formula("0*0*0"));

        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A1"));
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A2"));
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A3"));
    }
}
/// <summary>
/// This test class tests all exceptions that should come from the methods
/// of the Sreadsheet class.
/// </summary>
[TestClass]
public class Exceptions
{
    Spreadsheet s = new();
    //Tests for an invalid name exception in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestSetCellContentsWithDoubleException()
    {
        s.SetCellContents(".X", 4);
    }
    //Tests for an invalid name exception in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestSetCellContentsWithStringException()
    {
        s.SetCellContents("X,", "4+2+7");
    }
    //Tests for ArgumentNullException in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestSetCellContentsWithStringException2()
    {
        s.SetCellContents("X3", (string)null);
    }
    //Tests for an invalid name exception in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestSetCellContentsWithFormulaException()
    {
        s.SetCellContents("54", new Formula("4+2+7"));
    }
    //Tests for ArgumentNullException in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestSetCellContentsWithFormulaException2()
    {
        s.SetCellContents("X3", (Formula)null);
    }
    //Tests for CircularException in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void TestSetCellContentsCircularException()
    {
        s.SetCellContents("X1", new Formula("X3"));
        s.SetCellContents("X2", new Formula("X1 + 10"));
        s.SetCellContents("X3", new Formula("X2 + 10"));
    }
    //Tests for an invalid name exception GetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestGetCellContentsException()
    {
        s.SetCellContents("A1", "eeeeeeeeee");
        s.GetCellContents("A4");
    }
    //Tests for an invalid name exception in GetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestGetCellContentsException2()
    {
        s.SetCellContents("A1", "eeeeeeeeee");
        s.GetCellContents("6");
    }
    //Tests for an invalid name exception if string is empty
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestGetDirectDependentsException()
    {
        s.GetDirectDependents("44");
    }
}
