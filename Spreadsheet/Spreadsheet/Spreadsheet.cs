// Author: David Clark
// CS 3500
// February 2021

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Tracks the relationships between Cells.
        // References Cells using a string (the cell's name), does not contain a reference to the Cell class itself
        private DependencyGraph depGraph;

        // Tracks all Cells that currently contain a Double, a Formula, or a non-empty String
        // The key-value pairs correspond to a cell name (a string representing a valid variable) and a Cell object.
        // Cells do not track their own names.
        private Dictionary<string, Cell> nonemptyCellsDict;

        public override bool Changed
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }

        public Spreadsheet() : base(s => true, s => s, "default")
        {
            depGraph = new DependencyGraph();
            nonemptyCellsDict = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Throws an InvalidNameException if...
        /// ...a cell name is null
        /// ...a cell name doesn't match the SpreadSheet specifications for a variable
        /// </summary>
        /// <param name="name"></param>
        private void throwIfInvalidName(string name)
        {
            if (ReferenceEquals(name, null))
                throw new InvalidNameException();

            if (!isValidCellName(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// Returns true if "name" is valid for a spreadsheet AND is valid for the spreadsheet's validator function.
        /// "name" must be one or more letters followed by one or more digits.
        /// </summary>
        /// <param name="name">a potential cell name</param>
        private bool isValidCellName(string name)
        {
            string validNamePattern = @"^[a-zA-Z]+[0-9]+$";
            bool passesSpreadsheetSpecs = System.Text.RegularExpressions.Regex.IsMatch(name, validNamePattern);

            bool passesValidator = IsValid(name);

            return passesSpreadsheetSpecs && passesValidator;
        }

        public override object GetCellContents(string name)
        {
            // throws an InvalidNameException if necessary
            throwIfInvalidName(name);

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


            if (Double.TryParse(content, out double newContentDouble))
            {
                return SetCellContents(name, newContentDouble);
            }
            else if (content.StartsWith("="))
            {
                try { return SetCellContents(name, new Formula(content, Normalize, IsValid)); }
                catch (FormulaFormatException e) { }
            }

            // contents must be a string
            return SetCellContents(name, content);
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            // replaces the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
                cellToReset.Contents = number;
            else
                nonemptyCellsDict.Add(name, new Cell(number));

            // returns the list of self + direct/indirect dependents
            return GetListToRecalculate(name);
        }

        protected override IList<string> SetCellContents(string name, string text)
        {
            // replaces the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
            {
                cellToReset.Contents = text;

                // if the cell was set to the empty string, remove it from the dictionary
                if (text == "")
                    nonemptyCellsDict.Remove(name);
            }
            else
            {
                nonemptyCellsDict.Add(name, new Cell(text));
            }

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
                nonemptyCellsDict.Add(name, new Cell(formula));

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


        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return depGraph.GetDependents(name);
        }

        public override string GetSavedVersion(string filename)
        {
            return "blatantly unfinished";
            using (XmlReader reader = XmlReader.Create("temp.xml"))
            {

            }
        }

        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";

            using (XmlWriter writer = XmlWriter.Create("temp.xml", settings))
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
                        writer.WriteValue(tempCell.Contents.ToString());
                    else if (tempCell.Contents is Formula)
                        writer.WriteValue("="+tempCell.Contents.ToString());
                    else if (tempCell.Contents is String)
                        writer.WriteValue(tempCell.Contents.ToString());
                    writer.WriteEndElement();// ends "contents" element
                    writer.WriteEndElement();// ends "cell" element
                }
                writer.WriteEndElement();// ends "spreadsheet" element
                writer.WriteEndDocument();
            }
        }



    }
}
