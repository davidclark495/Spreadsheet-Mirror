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

        public Spreadsheet()
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

        public override IList<string> SetCellContents(string name, double number)
        {
            throwIfInvalidName(name);

            // replaces the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
                cellToReset.Contents = number;
            else
                nonemptyCellsDict.Add(name, new Cell(number));

            // cells without formulas cannot depend on other cells
            depGraph.ReplaceDependees(name, new List<string>());

            return GetListToRecalculate(name);
        }

        public override IList<string> SetCellContents(string name, string text)
        {
            throwIfInvalidName(name);
            if (ReferenceEquals(text, null))
                throw new ArgumentNullException();

            // replace the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
                cellToReset.Contents = text;
            else
                nonemptyCellsDict.Add(name, new Cell(text));

            // if the cell is now empty, remove the reference to it
            if (text == "")
                nonemptyCellsDict.Remove(name);

            // cells without formulas cannot depend on other cells
            depGraph.ReplaceDependees(name, new List<string>());

            return GetListToRecalculate(name);
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            throwIfInvalidName(name);
            if (ReferenceEquals(formula, null))
                throw new ArgumentNullException();

            // replaces the Cell's contents and creates a backup, or creates a new cell
            Cell originalCell = null;
            Object originalContentsBackup = null;
            if (nonemptyCellsDict.TryGetValue(name, out originalCell))
            {
                originalContentsBackup = originalCell.Contents;
                originalCell.Contents = formula;
            }
            else
                nonemptyCellsDict.Add(name, new Cell(formula));

            // updates the Cell's dependencees and creates a backup
            IEnumerable<string> originalDependees = depGraph.GetDependees(name);
            depGraph.ReplaceDependees(name, formula.GetVariables());

            try // return as expected
            {
                return GetListToRecalculate(name);
            }
            catch (CircularException e) // a Circular Exception was introduced --> revert the Cell, then throw the exception
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
    }
}
