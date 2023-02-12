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
/// The method GetCellContents, which returns the contents of a specified cell
/// The method GetNamesOfAllNonemptyCells, which returns a list of all nonempty
/// cells.
/// Three variations of the SetCellContents method, with one of the variations
/// handling a double as one of the parameters, one of the variations handling
/// a Formula class, and the last variation handling a string parameter. All
/// three of these variations add new cells or new contents to existing cells,
/// but the variation that takes in a Formula class also checks for circulation
/// and resolves said circulation if found.
/// The GetDirectDependents method, which returns all of the direct dependents
/// of a specified cell.
/// </summary>
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Spreadsheet;
using SpreadsheetUtilities;
namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph dependencies = new();
        private Dictionary<string, Cell> cells = new();

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
        }

        /// <summary>
        /// Private helper method for the three SetCellContents
        /// methods.
        /// </summary>
        /// <param name="name">Name of cell</param>
        /// <param name="contents">The contents of the cell</param>
        /// <returns>A list of all cells that directly or indirectly depend on
        /// the given cell.</returns>
        /// <exception cref="InvalidNameException"></exception>
        private IList<string> SetCell(string name, object contents)
        {
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]*[0-9]*$"))
            {
                throw new InvalidNameException();
            }
            else if (contents == null)
            {
                throw new ArgumentNullException();
            }
            else if (cells.ContainsKey(name))
            {
                cells[name].SetContents(contents);
            }
            else if (contents + "" != "")
            {
                cells.Add(name, new Cell(contents));
            }
            return GetCellsToRecalculate(name).ToList();
        }

        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the contents or value of the given cell.
        /// </summary>
        /// <param name="name">The cell being looked at</param>
        /// <returns>The contents or value of the cell</returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            if (name == null || !cells.ContainsKey(name) || !Regex.IsMatch(name, "^[a-z|A-Z]*[0-9]*$"))
            {
                throw new InvalidNameException();
            }
            return cells[name].GetContents();
        }

        /// <summary>
        /// Gets the value of the cell specified.
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <returns>The value of the cell</returns>
        public override object GetCellValue(string name)
        {
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]*[0-9]*$"))
            {
                throw new InvalidNameException();
            }
            return cells[name].GetValue();
        }

        /// <summary>
        /// This method gets all of the cells in the spreadsheet that are nonempty.
        /// </summary>
        /// <returns>A list of the names of the nonempty cells</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }







        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]*[0-9]*$"))
            {
                throw new InvalidNameException();
            }
            if (Double.TryParse(content, result: out double Result))
            {
                cells[name].SetContents(Result);
                cells[name].SetValue(Result);
            }
            else if (content[0] == '=')
            {
                string expression = content.Remove('=');
                Formula formula = new Formula(expression, Normalize, IsValid);
                cells[name].SetContents(formula.Evaluate());
                cells[name].SetValue(formula.Evaluate());
            }
            else
            {
                cells[name].SetContents(content);
                cells[name].SetValue(content);
            }
            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Gets all of the direct dependents of the given cell.
        /// </summary>
        /// <param name="name">The cell being checked for dependents</param>
        /// <returns>A list of all of the direct dependents of the given cell.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]*[0-9]*$"))
            {
                throw new InvalidNameException();
            }
            return dependencies.GetDependents(name);
        }

        /// <summary>
        /// This method either adds a new cell to the cell dictionary created
        /// earlier, or replaces the current contents of the given cell if that
        /// cell exists in the dictionary already. Then gets every cell that
        /// depends on the "name" cell.
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <param name="number">The numerical value of the cell</param>
        /// <returns>A list of all cells that directly or indirectly depend on
        /// the given cell.</returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return SetCell(name, number);
        }
        /// <summary>
        /// This method either adds a new cell to the cell dictionary created
        /// earlier, or replaces the current contents of the given cell if that
        /// cell exists in the dictionary already. Then gets every cell that
        /// depends on the "name" cell.
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <param name="text">The contents or value of the cell in string form.</param>
        /// <returns>A list of all cells that directly or indirectly depend on
        /// the given cell.</returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return SetCell(name, text);
        }
        /// <summary>
        /// This method either adds a new cell to the cell dictionary created
        /// earlier, or replaces the current contents of the given cell if that
        /// cell exists in the dictionary already. Then gets every cell that
        /// depends on the given cell.
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <param name="formula">The formula content in the cell</param>
        /// <returns>A list of all cells that directly or indirectly depend on
        /// the given cell.</returns>
        /// <exception cref="InvalidNameException"></exception>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            IList<string> returnList = SetCell(name, formula);
            List<string> ToSave = dependencies.GetDependents(name).ToList<string>();
            dependencies.ReplaceDependents(name, formula.GetVariables());

            try
            {
                GetCellsToRecalculate(name);
            }
            catch
            {
                dependencies.ReplaceDependents(name, ToSave);
                throw;
            }

            return returnList;
        }
    }
}