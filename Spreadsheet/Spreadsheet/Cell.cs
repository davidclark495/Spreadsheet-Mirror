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

        // helps to identify the cell's contents
        private bool isDouble = false;
        private bool isString = false;
        private bool isFormula = false;

        /// <summary>
        /// Sets the Cell's contents to the provided double.
        /// </summary>
        public Cell(double content)
        {
            this.isDouble = true;
            p_contents = content;
        }

        public Cell(string content)
        {
            this.isString = true;
            p_contents = content;
        }

        public Cell(Formula content)
        {
            this.isFormula = true;
            p_contents = content;
        }

        


        public object Contents
        {
            get
            {
                if (this.isDouble)
                    return (double)p_contents;
                else if (this.isString)
                    return (string)p_contents;
                else if (this.isFormula)
                    return (Formula)p_contents;
                else
                    throw new Exception("Invalid state. Cell contents are not marked " +
                        "as containing a double, string, or formula: cannot be evaluated.");
            }

            set
            {
                // if value is an invalid type, return without modifying this cell
                if (!((value is Double) || (value is String) || (value is Formula)))
                    return;

                // clear the cell's previous state
                this.isDouble = false;
                this.isString = false;
                this.isFormula = false;
                p_contents = null;

                // set the cell's new data
                if (value is Double)
                {
                    this.isDouble = true;
                    p_contents = value;
                }
                else if (value is String)
                {
                    this.isString = true;
                    p_contents = value;
                }
                else if (value is Formula)
                {
                    this.isFormula = true;
                    p_contents = value;
                }
            }
        }


    }
}
