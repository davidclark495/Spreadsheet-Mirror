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
        // 
        private DependencyGraph depGraph;
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

        /// <summary>
        /// Helper for SetCellContents(Formula).
        /// Throws a CircularException if changing this Cell's value to the given formula
        /// would result in a cycle.
        /// </summary>
        /// <param name="name">the name of a valid cell in this spreadsheet</param>
        /// <param name="formula">the formula that would be used as the Cell's new value</param>
        private void throwIfCircularException(string name, Formula formula)
        {
            // enumerate through each dependent of the cell tagged as "name"
            IEnumerator<string> dependentsEnum = GetCellsToRecalculate(name).GetEnumerator();

            // skip the first element, which should be the original cell itself
            dependentsEnum.MoveNext();

            while (dependentsEnum.MoveNext())
            {
                string currDependent = dependentsEnum.Current;

                // if the currDependent cell is referenced in the formula, then the cell would depend on its own dependents
                // --> throw exception
                foreach (string var in formula.GetVariables())
                {
                    if (var == currDependent)
                        throw new CircularException();
                }

            }


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

        public override IList<string> SetCellContents(string name, string text)
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

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            // throws exceptions if necessary
            throwIfInvalidName(name);

            if (ReferenceEquals(formula, null))
                throw new ArgumentNullException();
            
            throwIfCircularException(name, formula);


            // replaces the Cell's contents, or creates a new cell
            if (nonemptyCellsDict.TryGetValue(name, out Cell cellToReset))
                cellToReset.Contents = formula;
            else
                nonemptyCellsDict.Add(name, new Cell(formula));

            // updates the Cell's dependencees
            depGraph.ReplaceDependees(name, formula.GetVariables());


            // returns the list of self + direct/indirect dependents
            return GetListToRecalculate(name);
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return depGraph.GetDependents(name);
        }
    }
}
