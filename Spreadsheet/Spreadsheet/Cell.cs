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
        // backend values  
        private object p_contents;
        private object p_value;

        // provided by Spreadsheet
        private Func<string, double> lookup;


        /// <summary>
        /// Sets the Cell's contents to the provided double.
        /// </summary>
        public Cell(double content)
        {
            p_contents = content;
            RecalculateValue();
        }

        /// <summary>
        /// Sets the Cell's contents to the provided string.
        /// </summary>
        public Cell(string content)
        {
            p_contents = content;
            RecalculateValue();
        }

        /// <summary>
        /// Sets the Cell's contents to the provided Formula.
        /// </summary>
        public Cell(Formula content)
        {
            p_contents = content;
            RecalculateValue();
        }

        /// <summary>
        /// Generates the value associated with the Cell's contents. 
        /// Should be called whenever the contents are reset.
        /// </summary>
        private void RecalculateValue()
        {
            if (Contents is Double)
                Value = (Double)Contents;
            if (Contents is Formula)
                Value = ((Formula)Contents).Evaluate(lookup);
            if (Contents is String)
                Value = (String)Contents;
        }


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
