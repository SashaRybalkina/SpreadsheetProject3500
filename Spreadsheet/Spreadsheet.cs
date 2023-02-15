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
using System.Xml;
using Microsoft.VisualBasic;
using Spreadsheet;
using SpreadsheetUtilities;
namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph dependencies = new();
        private Dictionary<string, Cell> cells = new();
        private string ?PathToFile;

        public Spreadsheet() : this(s => true, s => s, "default")
        {
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
        }

        public Spreadsheet(string pathToFile, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            try
            {
                XmlReader read = XmlReader.Create(pathToFile);
                while (read.Read())
                {
                    if (read.IsStartElement())
                    {
                        if (read.Name.Equals("spreadsheet"))
                        {
                            string? cell = read["cell"];
                            if (cell is null)
                            {
                                throw new SpreadsheetReadWriteException("Cannot find version element");
                            }
                            else
                            {
                                string? name = read["name"];
                                string? contents = read["contents"];
                                if (name is null || contents is null)
                                {
                                    throw new SpreadsheetReadWriteException("Cannot find name or contents of cell element");
                                }
                                else
                                {
                                    SetContentsOfCell(name, contents);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Cannot find version element");
            }
            throw new SpreadsheetReadWriteException("Cannot find version element");
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
            if (contents == null && !(contents is double))
            {
                throw new ArgumentNullException();
            }
            else if (cells.ContainsKey(name))
            {
                cells[Normalize(name)].SetContents(contents);
                cells[Normalize(name)].SetValue(contents);
            }
            else if (contents + "" != "")
            {
                cells.Add(Normalize(name), new Cell(contents));
                dependencies.AddDependency(name, "");
            }
            return GetCellsToRecalculate(name).ToList();
        }

        public override bool Changed
        {
            get =>  false;
            protected set => Changed = false;
        }

        /// <summary>
        /// Gets the contents or value of the given cell.
        /// </summary>
        /// <param name="name">The cell being looked at</param>
        /// <returns>The contents or value of the cell</returns>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            if (name != null && Regex.IsMatch(name, @"^[a-z|A-Z|]+[0-9]+$"))
            {
                if (!cells.ContainsKey(name))
                {
                    return "";
                }
                return cells[name];
            }
            throw new InvalidNameException();
        }

        /// <summary>
        /// Gets the value of the cell specified.
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <returns>The value of the cell</returns>
        public override object GetCellValue(string name)
        {
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]+[0-9]+$") || !IsValid(name))
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

        /// <summary>
        /// This method gets the version of the file being looked at by reading
        /// every line of the file, going into the "spreadsheet" element, then
        /// into the "version" element and reading it. If there is no versioning,
        /// or if any issues come up with reading the file, and exception is
        /// thrown.
        /// </summary>
        /// <param name="filename">The file being looked at</param>
        /// <returns>The versioning of the file</returns>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                XmlReader read = XmlReader.Create(filename);
                while (read.Read())
                {
                    if (read.IsStartElement())
                    {
                        if (read.Name.Equals("spreadsheet"))
                        {
                            string? version = read["version"];
                            if (version is null)
                            {
                                throw new SpreadsheetReadWriteException("Cannot find version element");
                            }
                            else
                            {
                                return version;
                            }
                        }
                    }
                }
            }
            ///If anything goes wrong reading the file, or if spreadsheet does
            ///not contain a version element, an exception is thrown.
            catch
            {
                throw new SpreadsheetReadWriteException("Unable to read file properly");
            }
            throw new SpreadsheetReadWriteException("Cannot find version element");
        }

        /// <summary>
        /// Write a brand new file based on the non empty cells that spreadsheet
        /// has.
        /// </summary>
        /// <param name="filename">The file being looked at</param>
        public override void Save(string filename)
        {
            XmlWriter write = XmlWriter.Create(filename);
            write.WriteStartDocument();
            write.WriteStartElement("spreadsheet");
            write.WriteAttributeString("version", Version);

            foreach (string cell in cells.Keys)
            {
                write.WriteStartElement("cell");
                string name = cell;
                string contents = "";
                object objectContents = cells[cell].GetContents();
                if (objectContents is double)
                {
                    contents = objectContents.ToString();
                }
                else if (objectContents is string)
                {
                    contents = (string)objectContents;
                }
                else if (objectContents is Formula)
                {
                    contents = "=" + objectContents.ToString();
                }
                write.WriteAttributeString("name", name);
                write.WriteAttributeString("contents", contents);
                write.WriteEndElement();
            }
            write.WriteEndElement();
            write.WriteEndDocument();
            Changed = false;
        }

        /// <summary>
        /// Checks if the string content given is a double, string, or Formula,
        /// and uses the appropriate SetCellContents method to set the cell to
        /// its right contents and value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            Changed = true;
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]+[0-9]+$") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            else if (Double.TryParse(content, result: out double Result))
            {
                SetCellContents(name, Result);
            }
            else if (!(content[0] == '='))
            {
                SetCellContents(name, content);
            }
            else
            {
                string expression = content.Remove('=');
                Formula formula = new Formula(expression, Normalize, IsValid);
                SetCellContents(name, formula);
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
            if (name == null || !Regex.IsMatch(name, "^[a-z|A-Z]+[0-9]+$") || !IsValid(name))
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
            dependencies.ReplaceDependents(name, new HashSet<string>());
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
            dependencies.ReplaceDependents(name, new HashSet<string>());
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
            ///Sets the value of cell
            string expression = formula.ToString();
            Formula NormFormula = new Formula(expression, Normalize, IsValid);
            cells[name].SetValue(formula.Evaluate(lookup));

            List<string> ToSave = dependencies.GetDependents(name).ToList<string>();
            dependencies.ReplaceDependents(name, NormFormula.GetVariables());

            try
            {
                GetCellsToRecalculate(name);
            }
            catch
            {
                dependencies.ReplaceDependents(name, ToSave);
                throw;
            }

            return SetCell(name, NormFormula);
        }

        /// <summary>
        /// Private delagate for looking up variables.
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        private double lookup(string var)
        {
            if (!cells.ContainsKey(var))
            {
                throw new InvalidNameException();
            }
            return (double)GetCellValue(var);
        }
    }
}