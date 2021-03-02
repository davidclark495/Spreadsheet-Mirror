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




        /// <summary>
        /// Invokes the three-argument constructor. 
        /// The resulting spreadsheet will have:
        /// ...a validator that always accepts a given cell name (although the name may be rejected for other reasons)
        /// ...a normalize delegate that doesn't modify incoming names
        /// ...the "default" version setting
        /// </summary>
        public Spreadsheet()
            : this(s => true, s => s, "default")
        {
        }

        /// <summary>
        /// Creates a new Spreadsheet object with the provided delegates and version information.
        /// </summary>
        /// <param name="isValid">A delegate used to impose additional constraints on cell names.</param>
        /// <param name="normalize">A delegate used to standardize cell names.</param>
        /// <param name="version">A string to determine version compatibility, used when saving or loading.</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            depGraph = new DependencyGraph();
            nonemptyCellsDict = new Dictionary<string, Cell>();
            Changed = false;
        }

        /// <summary>
        /// Invokes the three-argument constructor, then populates the spreadsheet with data read from the given file.
        /// </summary>
        /// <param name="filename">A file containing the spreadsheet to be loaded.</param>
        /// <param name="isValid">A delegate used to impose additional constraints on cell names.</param>
        /// <param name="normalize">A delegate used to standardize cell names.</param>
        /// <param name="version">A string to determine version compatibility, used when saving or loading.</param>
        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : this(isValid, normalize, version)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        // handle <spreadsheet> and version info.
                        if (reader.IsStartElement() && reader.Name == "spreadsheet")
                        {
                            if (this.Version != reader["version"])
                                throw new SpreadsheetReadWriteException("The saved spreadsheet's version information " +
                                    "does not match the provided version information.");
                        }

                        // handle <cell>, <name>, and <content> in sequence
                        else if (reader.IsStartElement() && reader.Name == "cell")
                        {
                            // move the reader along until the next element is found
                            while (reader.Read() && !(reader.IsStartElement())) { }
                            
                            // tag sequence error: next element is not <name>
                            if (reader.Name != "name")
                                throw new SpreadsheetReadWriteException("The saved spreadsheet contains " +
                                    "a cell without a name.");

                            // read and error-check the <name> element
                            reader.Read();
                            string tempCellName = reader.Value;
                            try { throwIfInvalidName(tempCellName); }
                            catch (InvalidNameException)
                            {
                                throw new SpreadsheetReadWriteException("The saved spreadsheet contains " +
                                    "a cell with an invalid name: " + tempCellName);
                            }

                            // move the reader along until the next element is found
                            while (reader.Read() && !(reader.IsStartElement())) { }

                            // tag sequence error: next element is not <contents>
                            if (reader.Name != "contents")
                                throw new SpreadsheetReadWriteException("The saved spreadsheet contains " +
                                    "a cell without contents.");

                            // read the <contents> element and create the new Cell
                            reader.Read();
                            try { SetContentsOfCell(tempCellName, reader.Value); }
                            catch (CircularException)
                            {
                                throw new SpreadsheetReadWriteException("The saved spreadsheet contains " +
                                    "a cycle.");
                            }
                        }
                    }
                }
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
                        if (reader.IsStartElement() &&
                            reader.Name == "spreadsheet" &&
                            !ReferenceEquals(reader["version"], null))
                        {
                            return reader["version"];
                        }
                    }
                }

                throw new SpreadsheetReadWriteException("The file is improperly written. " +
                    "It does not contain any version information.");
            }
            catch (ArgumentNullException)
            {
                throw new SpreadsheetReadWriteException("File name cannot be null");
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

            try
            {
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
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("The spreadsheet could not be saved to the provided file location.");
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

            IList<string> returnList;

            // set the value of the cell
            if (Double.TryParse(content, out double contentAsDouble))
                returnList = SetCellContents(name, contentAsDouble);

            else if (content.StartsWith("="))
                returnList = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));

            else
                returnList = SetCellContents(name, content);

            // update the values of each of the cell's direct/indirect dependents
            foreach (string cellName in returnList)
            {
                if (nonemptyCellsDict.TryGetValue(cellName, out Cell dependent))
                    dependent.RecalculateValue();
            }

            return returnList;
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
            Cell originalCell = null;
            Object originalContentsBackup = null;
            if (nonemptyCellsDict.TryGetValue(name, out originalCell))
            {
                originalContentsBackup = originalCell.Contents;
                originalCell.Contents = formula;
            }
            else
                nonemptyCellsDict.Add(name, new Cell(formula, this.Lookup));

            // updates the Cell's dependencees and creates a backup
            IEnumerable<string> originalDependees = depGraph.GetDependees(name);
            depGraph.ReplaceDependees(name, formula.GetVariables());

            // returns the list of self + direct/indirect dependents, or detects a Circular Exception and resets
            try // return the list as expected
            {
                return GetListToRecalculate(name);
            }
            catch (CircularException e) // a Circular dependency was introduced --> revert the Cell, then throw the exception
            { 
                // if the cell was previously nonempty, then restore the cell's contents
                // else the cell was empty, remove it from the non-empty dictionary
                if (!ReferenceEquals(originalCell, null))
                    originalCell.Contents = originalContentsBackup;
                else
                    nonemptyCellsDict.Remove(name);

                // restore the cell's original dependencies
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
        /// <param name="name">a cell/variable name</param>
        /// <returns>the double value associated with the given cell/variable</returns>
        private double Lookup(string name)
        {
            try
            {
                return (double)GetCellValue(name);
            }
            catch (Exception)
            {
                throw new ArgumentException("The variable '" + name + "' could not be evaluated.");
            }
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
