// Author: David Clark
// CS 3500
// February 2021

using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// A Cell stores its contents (either a Double, String, or Formula)
    /// and its value (either a Double, String, or FormulaError).
    /// </summary>
    internal class Cell
    {
        /// <summary>
        /// Stores a Double, String, or Formula representing the user-entered data for the cell.
        /// The private member behind Contents.
        /// </summary>
        private object p_contents;

        /// <summary>
        /// Stores a Double, String, or FormulaError derived from evaluating the cell's contents.
        /// The private member behind Value.
        /// </summary>
        private object p_value;
        
        /// <summary>
        /// This delegate is used to evaluate Formulas.
        /// Cells are meant to be mutable, so every Cell needs a lookup delegate
        /// in case it's contents are set to a Formula object.
        /// </summary>
        private Func<string, double> lookup;

        /// <summary>
        /// Creates a new Cell with the specified content.
        /// </summary>
        /// <param name="content">Must be a Double, String, or Formula.</param>
        /// <param name="lookup">Used to evaluate Formulas in the cell.</param>
        public Cell(Object content, Func<string, double> lookup)
        {
            this.lookup = lookup;
            Contents = content;
            RecalculateValue();
        }

        /// <summary>
        /// Generates the value associated with the Cell's contents. 
        /// Should be called whenever the contents are reset.
        /// </summary>
        public void RecalculateValue()
        {
            if (Contents is Double)
                Value = (Double)Contents;
            else if (Contents is Formula)
                Value = ((Formula)Contents).Evaluate(lookup);
            else if (Contents is String)
                Value = (String)Contents;
        }

        /// <summary>
        /// Represents a Double, String, or Formula.
        /// Resetting a Cell's "Contents" will cause it to recalculate its value.
        /// </summary>
        public object Contents
        {
            get
            {
                return p_contents;
            }

            set
            {
                if ((value is Double) || (value is String) || (value is Formula))
                {
                    p_contents = value;
                    this.RecalculateValue();
                }
            }
        }

        /// <summary>
        /// The value of the cell.
        /// Represents a Double, String, or FormulaError.
        /// Derived from the cell's Contents.
        /// </summary>
        public object Value
        {
            get
            {
                return p_value;
            }
            private set
            {
                p_value = value;
            }
        }

    }
}
