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

        /// <summary>
        /// Sets the Cell's contents to the provided double.
        /// </summary>
        public Cell(double content)
        {
            p_contents = content;
        }

        /// <summary>
        /// Sets the Cell's contents to the provided string.
        /// </summary>
        public Cell(string content)
        {
            p_contents = content;
        }

        /// <summary>
        /// Sets the Cell's contents to the provided Formula.
        /// </summary>
        public Cell(Formula content)
        {
            p_contents = content;
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
                    p_contents = value;
            }
        }


    }
}
