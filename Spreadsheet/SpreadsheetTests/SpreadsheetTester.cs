using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTester
    {
        /// <summary>
        /// Asserts that cells with contents are properly enumerated.
        /// </summary>
        [TestMethod]
        public void GetNamesOfNonemptyCells()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 1);
            spreadsheet.SetCellContents("K2", "content");
            spreadsheet.SetCellContents("_cell", new Formula("3 * 6"));
            spreadsheet.SetCellContents("___", Double.MaxValue);

            HashSet<string> expectedNames = new HashSet<string>();
            expectedNames.Add("A1");
            expectedNames.Add("K2");
            expectedNames.Add("_cell");
            expectedNames.Add("___");

            // asserts that getNames...() only enumerates the expected names && doesn't return the same name more than once
            foreach (string cellName in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                Assert.IsTrue(expectedNames.Remove(cellName));
            }
            // asserts that getNames...() didn't skip any of the expected names
            Assert.AreEqual(0, expectedNames.Count);
        }

        /// <summary>
        /// Tests behavior when a cell with contents is set to the empty string.
        /// The spreadsheet shouldn't include the newly emptied cell in it's enumeration.
        /// </summary>
        [TestMethod]
        public void GetNamesOfNonemptyCells_ClearedCells()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 1.0);
            spreadsheet.SetCellContents("G4", 6.5);
            spreadsheet.SetCellContents("test", "valid contents");
            spreadsheet.SetCellContents("test", "");

            HashSet<string> expectedNames = new HashSet<string>();
            expectedNames.Add("A1");
            expectedNames.Add("G4");

            // asserts that getNames...() only enumerates the expected names && doesn't return the same name more than once
            foreach (string cellName in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                Assert.IsTrue(expectedNames.Remove(cellName));
            }
            // asserts that getNames...() didn't skip any of the expected names
            Assert.AreEqual(0, expectedNames.Count);
        }


        public void SetCellDouble_NewCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            IList<string> A1List = spreadsheet.SetCellContents("A1", 1.0);
            IList<string> B1List = spreadsheet.SetCellContents("B1", 0.005);
            IList<string> C1List = spreadsheet.SetCellContents("C1", 2e9);

            Assert.IsTrue(A1List.Count == 1);
            Assert.IsTrue(A1List.Contains("A1"));
            Assert.IsTrue(B1List.Count == 1);
            Assert.IsTrue(B1List.Contains("B1"));
            Assert.IsTrue(C1List.Count == 1);
            Assert.IsTrue(C1List.Contains("C1"));
        }

        public void SetCellDouble_DirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            
            // populate the spreadsheet
            spreadsheet.SetCellContents("A1", 10);

            spreadsheet.SetCellContents("D1", new Formula("A1 * 1.7"));
            spreadsheet.SetCellContents("D2", new Formula("A1 * A1"));
            spreadsheet.SetCellContents("D3", new Formula("A1 / 3e2"));
            spreadsheet.SetCellContents("D4", new Formula("( A1 + 12 ) / 5"));

            spreadsheet.SetCellContents("X1", new Formula("2 + 2"));
            spreadsheet.SetCellContents("Y1", new Formula("3 * 1"));

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetCellContents("A1", 1.0);
            IEnumerator<string> A1Enum = A1Dependents.GetEnumerator();

            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "A1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D2");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D3");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D4");

            Assert.IsFalse(A1Enum.MoveNext());
        }

        public void SetCellDouble_IndirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetCellContents("A1", 10);
            spreadsheet.SetCellContents("B1", new Formula("A1 * 1.7"));
            spreadsheet.SetCellContents("C1", new Formula("B1 + 20.99"));

            spreadsheet.SetCellContents("X1", new Formula("2 + 2"));
            spreadsheet.SetCellContents("Y1", new Formula("3 * 1"));

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetCellContents("A1", 1.0);
            IEnumerator<string> A1Enum = A1Dependents.GetEnumerator();

            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "A1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "B1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "C1");

            Assert.IsFalse(A1Enum.MoveNext());
        }

        public void SetCellString_NewCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            IList<string> A1Results = spreadsheet.SetCellContents("A1", "new cell content");
            IList<string> B1Results = spreadsheet.SetCellContents("B1", "");
            IList<string> C1Results = spreadsheet.SetCellContents("C1", "&%$@!#");

            Assert.IsTrue(A1Results.Count == 1);
            Assert.IsTrue(A1Results.Contains("A1"));
            Assert.IsTrue(B1Results.Count == 1);
            Assert.IsTrue(B1Results.Contains("B1"));
            Assert.IsTrue(C1Results.Count == 1);
            Assert.IsTrue(C1Results.Contains("C1"));
        }

        public void SetCellString_DirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetCellContents("A1", "cell contents");

            spreadsheet.SetCellContents("D1", new Formula("A1 * 1.7"));
            spreadsheet.SetCellContents("D2", new Formula("A1 * A1"));
            spreadsheet.SetCellContents("D3", new Formula("A1 / 3e2"));
            spreadsheet.SetCellContents("D4", new Formula("( A1 + 12 ) / 5"));

            spreadsheet.SetCellContents("X1", new Formula("2 + 2"));
            spreadsheet.SetCellContents("Y1", new Formula("3 * 1"));

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetCellContents("A1", "new cell contents (exciting)");
            IEnumerator<string> A1Enum = A1Dependents.GetEnumerator();

            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "A1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D2");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D3");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D4");

            Assert.IsFalse(A1Enum.MoveNext());
        }

        public void SetCellString_IndirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetCellContents("A1", "excellent cell");
            spreadsheet.SetCellContents("B1", new Formula("A1 * 1.7"));
            spreadsheet.SetCellContents("C1", new Formula("B1 + 20.99"));

            spreadsheet.SetCellContents("X1", new Formula("2 + 2"));
            spreadsheet.SetCellContents("Y1", new Formula("3 * 1"));

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetCellContents("A1", "should've bought doge coin");
            IEnumerator<string> A1Enum = A1Dependents.GetEnumerator();

            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "A1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "B1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "C1");

            Assert.IsFalse(A1Enum.MoveNext());
        }

        public void SetCellFormula_NewCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            IList<string> A1Results = spreadsheet.SetCellContents("A1", new Formula("1 + 2"));
            IList<string> B1Results = spreadsheet.SetCellContents("B1", new Formula("_var - 12"));

            Assert.IsTrue(A1Results.Count == 1);
            Assert.IsTrue(A1Results.Contains("A1"));
            Assert.IsTrue(B1Results.Count == 1);
            Assert.IsTrue(B1Results.Contains("B1"));
        }

        public void SetCellFormula_DirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetCellContents("A1", new Formula("1 * 2 * 3 * 4"));

            spreadsheet.SetCellContents("D1", new Formula("A1 * 1.7"));
            spreadsheet.SetCellContents("D2", new Formula("A1 * A1"));
            spreadsheet.SetCellContents("D3", new Formula("A1 / 3e2"));
            spreadsheet.SetCellContents("D4", new Formula("( A1 + 12 ) / 5"));

            spreadsheet.SetCellContents("X1", new Formula("2 + 2"));
            spreadsheet.SetCellContents("Y1", new Formula("3 * 1"));

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetCellContents("A1", new Formula("1 * 1 * 1 * 1"));
            IEnumerator<string> A1Enum = A1Dependents.GetEnumerator();

            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "A1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D2");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D3");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "D4");

            Assert.IsFalse(A1Enum.MoveNext());
        }

        public void SetCellFormula_IndirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetCellContents("A1", new Formula("12"));
            spreadsheet.SetCellContents("B1", new Formula("A1 * 1.7"));
            spreadsheet.SetCellContents("C1", new Formula("B1 + 20.99"));

            spreadsheet.SetCellContents("X1", new Formula("2 + 2"));
            spreadsheet.SetCellContents("Y1", new Formula("3 * 1"));

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetCellContents("A1", new Formula("9000.0001"));
            IEnumerator<string> A1Enum = A1Dependents.GetEnumerator();

            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "A1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "B1");
            A1Enum.MoveNext();
            Assert.AreEqual(A1Enum.Current, "C1");

            Assert.IsFalse(A1Enum.MoveNext());
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with a "null" name.
        /// Specifically tests the Double version of Spreadsheet.SetCellContents()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDouble_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents(null, 0.0);
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with an invalid name.
        /// Specifically tests the Double version of Spreadsheet.SetCellContents()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDouble_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("1A", 0.0);
        }

        ///// <summary>
        ///// Asserts that an error is thrown when assigning "null" as a cell's content.
        ///// Specifically tests the String version of Spreadsheet.SetCellContents()
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(System.ArgumentNullException))]
        //public void SetCellString_Error_NullContent()
        //{
        //    // problem: currently untestable (ambiguous method call, could be either version of string or formula)
        //    SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
        //    spreadsheet.SetCellContents("_valid_cell", null);
        //}

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with a "null" name.
        /// Specifically tests the String version of Spreadsheet.SetCellContents()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellString_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents(null, "innocuous string");
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with an invalid name.
        /// Specifically tests the String version of Spreadsheet.SetCellContents()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellString_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("1A", "a meaningless string");
        }

        ///// <summary>
        ///// Asserts that an error is thrown when assigning "null" as a cell's content.
        ///// Specifically tests the Formula version of Spreadsheet.SetCellContents()
        ///// </summary>
        //[TestMethod]
        //[ExpectedException(typeof(System.ArgumentNullException))]
        //public void SetCellFormula_Error_NullContent()
        //{
        //    // problem: currently untestable (ambiguous method call, could be either version of string or formula)
        //    SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
        //    spreadsheet.SetCellContents("_valid_cell", null);
        //}


        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with a "null" name.
        /// Specifically tests the Formula version of Spreadsheet.SetCellContents()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormula_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents(null, new Formula("1 + 2"));
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with an invalid name.
        /// Specifically tests the Formula version of Spreadsheet.SetCellContents()
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormula_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("1A", new Formula("3 + 2"));
        }

        /// <summary>
        /// This might not be necessary, as it essentially tests the Formula() constructor.
        /// Asserts that an exception is thrown when setting the value of a cell to an invalid Formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetCellFormula_Error_InvalidFormula()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", new Formula("2 * this isn't even close to valid + 3"));
        }

        /// <summary>
        /// Asserts that an error is thrown when the new Formula content would result in a cycle of dependencies.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellFormula_Error_CircularDependency()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetCellContents("A1", 1);
            spreadsheet.SetCellContents("B1", new Formula("A1 + 1"));
            spreadsheet.SetCellContents("C1", new Formula("B1 + 1"));
            spreadsheet.SetCellContents("A1", new Formula("C1 + 1"));
        }


    }
}
