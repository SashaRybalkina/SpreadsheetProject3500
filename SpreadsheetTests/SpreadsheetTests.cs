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
/// Tests for SetContentsOfCell
/// Tests for GetCellContents
/// Tests for GetNamesOfAllEmptyCells
/// Tests for GetDirectDependents
/// Tests for XML methods
/// Tests for throwing exceptions when they should be thrown
/// Stress tests
/// </summary>
using SpreadsheetUtilities;
using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Channels;
using System.Xml;

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
    public void TestSetCellContentsWithStringWithNormalize()
    {
        Spreadsheet s2 = new(s => true, s => s.ToUpper(), "default");
        s2.SetContentsOfCell("a1", "4+2+7");
        s2.SetContentsOfCell("a2", "7-5-2");
        s2.SetContentsOfCell("a3", "1*1*1");
        s2.SetContentsOfCell("a3", "0*0*0");
        s2.SetContentsOfCell("a4", "");

        Assert.AreEqual("4+2+7", s2.GetCellContents("A1"));
        Assert.AreEqual("7-5-2", s2.GetCellContents("A2"));
        Assert.AreEqual("0*0*0", s2.GetCellContents("A3"));
        Assert.IsFalse(s2.GetNamesOfAllNonemptyCells().Contains("A4"));
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
        s.SetContentsOfCell("A1", "=5+5");
        Assert.AreEqual(10d, s.GetCellValue("A1"));
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
            s.SetContentsOfCell("A" + i, "=A" + (i + 1) + " + 1");
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
    //Tests for GetSavedversionException
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void TestGetSavedVersionException()
    {
        using XmlWriter write = XmlWriter.Create("test.txt");
        ///Writes spreadsheet start element
        write.WriteStartDocument();
        write.WriteStartElement("spreadsheet");
        write.WriteAttributeString("version", null);
        write.WriteEndElement();
        write.WriteEndDocument();

        s.GetSavedVersion("test.txt");
    }
    //Tests for an incorrect file structure exception
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void TestConstructorException()
    {
        using XmlWriter write = XmlWriter.Create("test2.txt");
        ///Writes spreadsheet start element
        write.WriteStartDocument();
        write.WriteStartElement("spreadsheet");
        write.WriteAttributeString("version", "default");
        write.WriteEndElement();
        write.WriteEndDocument();

        Spreadsheet s2 = new("test2.txt", s => true, s=>s, "default");
    }
    //Tests for an incorrect version exception
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void TestConstructorException2()
    {
        using XmlWriter write = XmlWriter.Create("test2.txt");
        ///Writes spreadsheet start element
        write.WriteStartDocument();
        write.WriteStartElement("spreadsheet");
        write.WriteAttributeString("version", "version 1");
        write.WriteEndElement();
        write.WriteEndDocument();
        Spreadsheet s2 = new("test2.txt", s => true, s => s, "default");
    }
}
