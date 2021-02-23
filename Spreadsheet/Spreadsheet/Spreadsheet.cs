// Author: David Clark
// CS 3500
// February 2021

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;

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

        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public Spreadsheet() : base(s => true, s => s, "default")
        {
            depGraph = new DependencyGraph();
            nonemptyCellsDict = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Returns true if "name" is valid as determined by the Formula class.
        /// </summary>
        /// <param name="name">a potential cell name</param>
        private bool isValidCellName(string name)
        {
            // Create a new Formula with the name. If Formula throws, the name was rejected, i.e. it is not a valid variable name.
            // This method creates consistency between Formula's and Spreadsheet's definitions of what constitutes a valid name.
            try
            {
                new Formula(name);
                return true;
            }
            catch (FormulaFormatException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Throws an InvalidNameException if...
        /// ...a cell name is null
        /// ...a cell name doesn't match the Formula specifications for a variable
        /// </summary>
        /// <param name="name"></param>
        private void throwIfInvalidName(string name)
        {
            if (ReferenceEquals(name, null))
                throw new InvalidNameException();

            if (!isValidCellName(name))
                throw new InvalidNameException();
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

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return nonemptyCellsDict.Keys;
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

        protected override IList<string> SetCellContents(string name, double number)
        {
            // throws exceptions if necessary
            throwIfInvalidName(name);

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
            // throws exceptions if necessary
            throwIfInvalidName(name);
            if (ReferenceEquals(text, null))
                throw new ArgumentNullException();

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
            // throw exceptions if necessary
            throwIfInvalidName(name);
            if (ReferenceEquals(formula, null))
                throw new ArgumentNullException();

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

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return depGraph.GetDependents(name);
        }

        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            throw new NotImplementedException();
        }
    }
}
