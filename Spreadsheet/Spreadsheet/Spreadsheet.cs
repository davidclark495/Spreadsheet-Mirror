// Author: David Clark
// CS 3500
// February 2021

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Tracks the relationships between Cells.
        /// References Cells using a string (the cell's name), does not contain a reference to the Cell object itself.
        /// </summary>
        private DependencyGraph depGraph;

        /// <summary>
        /// Tracks all Cells that currently contain a Double, a Formula, or a non-empty String.
        /// The key-value pairs correspond to a cell name (a string representing a valid variable) and a Cell object.
        /// Cells do not track their own names.
        /// </summary>
        private Dictionary<string, Cell> nonemptyCellsDict;

        /// <summary>
        /// The underlying data for the public property Changed.
        /// </summary>
        private bool p_changed;

        public override bool Changed
        {
            get => p_changed;
            protected set => p_changed = value;
        }





        public Spreadsheet() : base(s => true, s => s, "default")
        {
            depGraph = new DependencyGraph();
            nonemptyCellsDict = new Dictionary<string, Cell>();
            Changed = false;
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            depGraph = new DependencyGraph();
            nonemptyCellsDict = new Dictionary<string, Cell>();
            Changed = false;
        }

        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            depGraph = new DependencyGraph();
            nonemptyCellsDict = new Dictionary<string, Cell>();
            Changed = false;

            // load the file
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    string tempCellName = null;
                    string tempCellContents = null;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    if (this.Version != reader["version"])
                                        throw new SpreadsheetReadWriteException("The saved spreadsheet's version information " +
                                            "does not match the provided version information.");
                                    break;

                                case "cell":
                                    break;

                                case "name":
                                    reader.Read();
                                    tempCellName = reader.Value;
                                    try { throwIfInvalidName(tempCellName); }
                                    catch (InvalidNameException)
                                    {
                                        throw new SpreadsheetReadWriteException("The saved spreadsheet contains " +
                                            "a cell with an invalid name: " + tempCellName);
                                    }
                                    break;

                                case "contents":
                                    reader.Read();
                                    tempCellContents = reader.Value;
                                    try { SetContentsOfCell(tempCellName, tempCellContents); }
                                    catch (CircularException)
                                    {
                                        throw new SpreadsheetReadWriteException("The saved spreadsheet contains " +
                                            "a cycle.");
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("The file could not be found: " + filename);
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("The file could not be loaded.");
            }
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && reader.Name == "spreadsheet")
                        {
                            return reader["version"];
                        }
                    }
                }

                // This line should never be executed when reading a correctly written save file.
                return null;
            }
            catch (ArgumentNullException)
            {
                throw new SpreadsheetReadWriteException("File name cannot be null");
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("The file could not be found: " + filename);
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("The file could not be loaded.");
            }
        }

        public override void Save(string filename)
        {
            if (ReferenceEquals(filename, null))
                throw new SpreadsheetReadWriteException("File name cannot be null.");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", this.Version);

                foreach (string tempCellName in GetNamesOfAllNonemptyCells())
                {
                    // this should never return false
                    nonemptyCellsDict.TryGetValue(tempCellName, out Cell tempCell);

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", tempCellName);
                    writer.WriteStartElement("contents");
                    if (tempCell.Contents is Double)
                        writer.WriteValue(((double)tempCell.Contents).ToString());
                    else if (tempCell.Contents is Formula)
                        writer.WriteValue("=" + ((Formula)tempCell.Contents).ToString());
                    else if (tempCell.Contents is String)
                        writer.WriteValue((string)tempCell.Contents);
                    writer.WriteEndElement();// ends "contents" element
                    writer.WriteEndElement();// ends "cell" element
                }
                writer.WriteEndElement();// ends "spreadsheet" element
                writer.WriteEndDocument();
            }

            Changed = false;
        }



        public override object GetCellContents(string name)
        {
            // throws an InvalidNameException if necessary
            throwIfInvalidName(name);

            name = Normalize(name);

            // if cell is not empty
            if (nonemptyCellsDict.TryGetValue(name, out Cell resultCell))
            {
                return resultCell.Contents;
            }
            else
            {
                return "";
            }

        }

        public override object GetCellValue(string name)
        {
            throwIfInvalidName(name);

            name = Normalize(name);

            if (nonemptyCellsDict.TryGetValue(name, out Cell cell))
                return cell.Value;
            else
                return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return nonemptyCellsDict.Keys;
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            throwIfInvalidName(name);
            if (ReferenceEquals(content, null))
                throw new ArgumentNullException();

            name = Normalize(name);
            Changed = true;


            if (Double.TryParse(content, out double contentAsDouble))
                return SetCellContents(name, contentAsDouble);
            
            else if (content.StartsWith("="))
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            
            else 
                return SetCellContents(name, content);
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            // replaces the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
                cellToReset.Contents = number;
            else
                nonemptyCellsDict.Add(name, new Cell(number, this.Lookup));

            depGraph.ReplaceDependees(name, new string[] { });

            // returns the list of self + direct/indirect dependents
            return GetListToRecalculate(name);
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            // replaces the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
                cellToReset.Contents = text;
            else
                nonemptyCellsDict.Add(name, new Cell(text, this.Lookup));

            // if the cell was set to the empty string, remove it from the dictionary
            if (text == "")
                nonemptyCellsDict.Remove(name);

            depGraph.ReplaceDependees(name, new string[] { });

            // returns the list of self + direct/indirect dependents
            return GetListToRecalculate(name);
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // replaces the Cell's contents and creates a backup, or creates a new cell
            Object originalCellContents = null;
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
            {
                originalCellContents = cellToReset.Contents;
                cellToReset.Contents = formula;
            }
            else
                nonemptyCellsDict.Add(name, new Cell(formula, this.Lookup));

            // updates the Cell's dependencees and creates a backup
            IEnumerable<string> originalDependees = depGraph.GetDependees(name);
            depGraph.ReplaceDependees(name, formula.GetVariables());

            // returns the list of self + direct/indirect dependents, or detects a Circular Exception and resets
            try
            {
                return GetListToRecalculate(name);
            }
            catch (CircularException e)
            {
                cellToReset.Contents = originalCellContents;
                depGraph.ReplaceDependees(name, originalDependees);

                throw e;
            }
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return depGraph.GetDependents(name);
        }



        /// <summary>
        /// Method used to evaluate Formulas in Cells. 
        /// Associates each valid cell name with its cell's value.
        /// Throws an ArgumentException if:
        ///     ...name is not a valid cell identifier.
        ///     ...the cell's value is not a double.
        /// Exceptions should be caught when evaluating a Formula.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private double Lookup(string name)
        {
            throwIfInvalidName(name);

            name = Normalize(name);

            nonemptyCellsDict.TryGetValue(name, out Cell cell);

            if (ReferenceEquals(cell, null))
                throw new ArgumentException("The cell being referenced is empty and can't be evaluated in a Formula.");

            if (!(cell.Value is double))
                throw new ArgumentException("The cell being referenced could not be evaluated to a number.");

            return (double)cell.Value;
        }

        /// <summary>
        /// Throws an InvalidNameException if:
        /// ...a cell name is null.
        /// ...a cell name doesn't match the SpreadSheet specifications for a variable.
        /// ...a cell name doesn't match the Validator specifications for a variable.
        /// </summary>
        /// <param name="name"></param>
        private void throwIfInvalidName(string name)
        {
            // check null
            if (ReferenceEquals(name, null))
                throw new InvalidNameException();

            name = Normalize(name);

            // check general spreadsheet validity
            string validNamePattern = @"^[a-zA-Z]+[0-9]+$";
            if (!Regex.IsMatch(name, validNamePattern))
                throw new InvalidNameException();

            // check specific Validator validity
            if (!IsValid(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// Helper for SetCellContents().
        /// Returns a list of a Cell and all of its direct/indirect dependents.
        /// Based on GetCellsToRecalculate().
        /// </summary>
        /// <param name="name">the name of a cell in this spreadsheet</param>
        private IList<string> GetListToRecalculate(string name)
        {
            IList<string> listToRecalc = new List<string>();
            foreach (string tempCellName in GetCellsToRecalculate(name))
            {
                listToRecalc.Add(tempCellName);
            }

            return listToRecalc;
        }


    }
}
