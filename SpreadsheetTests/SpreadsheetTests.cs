/// <summary>
/// Author:    Sasha Rybalkina
/// Partner:   None
/// Date:      Febuary 17, 2023
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
/// Tests for SetContentsOfCell
/// Tests for XML reading and writing
/// Tests for GetCellContents
/// Tests for GetNamesOfAllEmptyCells
/// Tests for GetDirectDependents
/// Tests for throwing exceptions when they should be thrown
/// Stress tests.
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
        s.SetContentsOfCell("A1", "4");
        s.SetContentsOfCell("A2", "5");
        s.SetContentsOfCell("A3", "6");
        s.SetContentsOfCell("A3", "7");

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
    public void TestSetCellContentsWithStringAndNormalize()
    {
        Spreadsheet s2 = new(s => true, s => s.ToUpper(), "default");
        s.SetContentsOfCell("a1", "4+2+7");
        s.SetContentsOfCell("a2", "7-5-2");
        s.SetContentsOfCell("a3", "1*1*1");
        s.SetContentsOfCell("a3", "0*0*0");
        s.SetContentsOfCell("a4", "");

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
        s.SetContentsOfCell("A1", "=4+2+7");
        s.SetContentsOfCell("A2", "=7-5-2");
        s.SetContentsOfCell("A3", "=1*1*1");
        s.SetContentsOfCell("A3", "=0*0*0");

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
        s.SetContentsOfCell("A1", "eeeeeeeeee");
        Assert.AreEqual("eeeeeeeeee", s.GetCellContents("A1"));
    }
    /// <summary>
    /// Tests the GetCellContents method. Invoking this method on cell
    /// A1 should return the string "eeeeeeeeee"
    /// </summary>
    [TestMethod]
    public void TestGetCellValue()
    {
        s.SetContentsOfCell("A1", "=8+7");
        Assert.AreEqual(15d, s.GetCellValue("A1"));
    }
    /// <summary>
    /// Tests the GetNamesOfAllEmptyCells method. The list returned
    /// should contain A1, A2, and A3.
    /// </summary>
    [TestMethod]
    public void TestGetNamesOfAllEmptyCells()
    {
        s.SetContentsOfCell("A1", "=4+2+7");
        s.SetContentsOfCell("A2", "=7-5-2");
        s.SetContentsOfCell("A3", "=0*0*0");

        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A1"));
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A2"));
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A3"));
    }
    /// <summary>
    /// Tests the constructor which takes in the PathToFile parameter,
    /// and tests the Save method simultaneously.
    /// </summary>
    [TestMethod]
    public void TestPathToFileConstructorAndSave()
    {
        s.SetContentsOfCell("A1", "=4+2+7");
        s.SetContentsOfCell("A2", "=7-5-2");
        s.SetContentsOfCell("A3", "=0*0*0");
        s.Save("save.txt");

        Spreadsheet s2 = new("save.txt", s => true, s => s, "default");

        Assert.IsTrue(s2.GetNamesOfAllNonemptyCells().Contains("A1"));
        Assert.IsTrue(s2.GetNamesOfAllNonemptyCells().Contains("A2"));
        Assert.IsTrue(s2.GetNamesOfAllNonemptyCells().Contains("A3"));
        Assert.IsTrue(s2.GetNamesOfAllNonemptyCells().SequenceEqual(new List<string> { "A1", "A2", "A3" }));
    }
    /// <summary>
    /// Tests the constructor which takes in the PathToFile parameter,
    /// and tests the Save method simultaneously.
    /// </summary>
    [TestMethod]
    public void TestGetSavedVersion()
    {
        Spreadsheet s2 = new(s => true, s => s, "version 1");
        s2.SetContentsOfCell("A1", "=4+2+7");
        s2.SetContentsOfCell("A2", "=7-5-2");
        s2.SetContentsOfCell("A3", "=0*0*0");
        s2.Save("save.txt");
        Assert.AreEqual("version 1", s.GetSavedVersion("save.txt"));
    }
    /// <summary>
    /// Tests the time efficiency of Spreadsheet
    /// </summary>
    [TestMethod]
    [Timeout(1000)]
    public void StressTest()
    {
        for (int i = 1; i < 1001; i++)
        {
            s.SetContentsOfCell("A" + i, "1");
        }
    }
    /// <summary>
    /// Tests the functioning of the dependency chains
    /// </summary>
    [TestMethod]
    [Timeout(1000)]
    public void StressTest2()
    {
        for (int i = 1; i < 20; i++)
        {
            s.SetContentsOfCell("A" + i, "A" + (i + 1) + " + 1");
        }
        s.SetContentsOfCell("A20", "1");
        Assert.AreEqual(s.GetCellValue("A1"), 20d);
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
        s.SetContentsOfCell(".X", "4");
    }
    //Tests for an invalid name exception in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestSetCellContentsWithStringException()
    {
        s.SetContentsOfCell("X,", "4+2+7");
    }
    //Tests for an invalid name exception in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestSetCellContentsWithFormulaException()
    {
        s.SetContentsOfCell("54", "=4+2+7");
    }
    //Tests for CircularException in SetCellContents
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void TestSetCellContentsCircularException()
    {
        s.SetContentsOfCell("X1", "=X3");
        s.SetContentsOfCell("X2", "=X1 + 10");
        s.SetContentsOfCell("X3", "=X2 + 10");
    }
    //Tests for an invalid name exception in GetCellContents
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void TestGetCellContentsException2()
    {
        s.SetContentsOfCell("A1", "eeeeeeeeee");
        s.GetCellContents("6");
    }
}
