// Author: David Clark
// CS 3500
// February 2021

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
        /// Asserts that an empty spreadsheet has 0 non-empty cells.
        /// </summary>
        [TestMethod]
        public void GetNamesOfNonemptyCells_EmptySpreadsheet()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            IEnumerator<string> emptyCellEnum = spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator();
            Assert.IsFalse(emptyCellEnum.MoveNext());
        }

        /// <summary>
        /// Asserts that cells with contents are properly enumerated.
        /// </summary>
        [TestMethod]
        public void GetNamesOfNonemptyCells()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "1");
            spreadsheet.SetContentsOfCell("K2", "content");
            spreadsheet.SetContentsOfCell("cell19", "=3 * 6");
            spreadsheet.SetContentsOfCell("AaBbZz00", "" + Double.MaxValue);

            HashSet<string> expectedNames = new HashSet<string>();
            expectedNames.Add("A1");
            expectedNames.Add("K2");
            expectedNames.Add("cell19");
            expectedNames.Add("AaBbZz00");

            // asserts that getNames...() only enumerates the expected names && doesn't return the same name more than once
            foreach (string cellName in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                // only true if expectedNames contains the element
                // changes expectedNames so that removing the same cell twice would fail
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
            spreadsheet.SetContentsOfCell("A1", "1.0");
            spreadsheet.SetContentsOfCell("G4", "6.5");
            spreadsheet.SetContentsOfCell("test1", "valid contents");
            spreadsheet.SetContentsOfCell("test1", "");

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

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getCellContents_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getCellContents_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.GetCellContents("--invalid_cell_name--");
        }

        /// <summary>
        /// The spreadsheet should return the empty string when asked for the contents of an empty cell.
        /// </summary>
        [TestMethod]
        public void getCellContents_EmptyCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            Assert.AreEqual("", spreadsheet.GetCellContents("A1"));
            Assert.AreEqual("", spreadsheet.GetCellContents("B24601"));
            Assert.AreEqual("", spreadsheet.GetCellContents("CZEY6"));
        }

        [TestMethod]
        public void getCellContents_Double()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "2.0");
            spreadsheet.SetContentsOfCell("B1", "9.99999");
            spreadsheet.SetContentsOfCell("C1", "-512");

            Assert.AreEqual(2.0, spreadsheet.GetCellContents("A1"));
            Assert.AreEqual(9.99999, spreadsheet.GetCellContents("B1"));
            Assert.AreEqual(-512.0, spreadsheet.GetCellContents("C1"));
        }

        [TestMethod]
        public void getCellContents_String()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "basic string");
            spreadsheet.SetContentsOfCell("B1", "contents II");
            spreadsheet.SetContentsOfCell("C1", "~fridays~");

            Assert.AreEqual("basic string", spreadsheet.GetCellContents("A1"));
            Assert.AreEqual("contents II", spreadsheet.GetCellContents("B1"));
            Assert.AreEqual("~fridays~", spreadsheet.GetCellContents("C1"));
        }

        [TestMethod]
        public void getCellContents_Formula()
        {
            Formula form1 = new Formula("3");
            Formula form2 = new Formula("12.6 * X5");
            Formula form3 = new Formula("( 8 - 8.5 * 2 )");

            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=" + form1.ToString());
            spreadsheet.SetContentsOfCell("B1", "=" + form2.ToString());
            spreadsheet.SetContentsOfCell("C1", "=" + form3.ToString());

            Assert.AreEqual(form1, spreadsheet.GetCellContents("A1"));
            Assert.AreEqual(form2, spreadsheet.GetCellContents("B1"));
            Assert.AreEqual(form3, spreadsheet.GetCellContents("C1"));
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellDouble_NewCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            IList<string> A1List = spreadsheet.SetContentsOfCell("A1", "1.0");
            IList<string> B1List = spreadsheet.SetContentsOfCell("B1", "0.005");
            IList<string> C1List = spreadsheet.SetContentsOfCell("C1", "2e9");

            Assert.IsTrue(A1List.Count == 1);
            Assert.IsTrue(A1List.Contains("A1"));
            Assert.IsTrue(B1List.Count == 1);
            Assert.IsTrue(B1List.Contains("B1"));
            Assert.IsTrue(C1List.Count == 1);
            Assert.IsTrue(C1List.Contains("C1"));
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellDouble_DirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetContentsOfCell("A1", "10");

            spreadsheet.SetContentsOfCell("D1", "=A1 * 1.7");
            spreadsheet.SetContentsOfCell("D2", "=A1 * A1");
            spreadsheet.SetContentsOfCell("D3", "=A1 / 68");
            spreadsheet.SetContentsOfCell("D4", "=A1 + 9000.0001");
                                               
            spreadsheet.SetContentsOfCell("X1", "=2 + 2");
            spreadsheet.SetContentsOfCell("Y1", "=3 * 1");

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetContentsOfCell("A1", "1.0");
            
            HashSet<string> expectedDependents = new HashSet<string>();
            expectedDependents.Add("A1");
            expectedDependents.Add("D1");
            expectedDependents.Add("D2");
            expectedDependents.Add("D3");
            expectedDependents.Add("D4");

            // asserts that getNames...() only enumerates the expected names && doesn't return the same name more than once
            foreach (string cellName in A1Dependents)
            {
                // only true if expectedNames contains the element
                // changes expectedNames so that removing the same cell twice would fail
                Assert.IsTrue(expectedDependents.Remove(cellName));
            }
            // asserts that getNames...() didn't skip any of the expected names
            Assert.AreEqual(0, expectedDependents.Count);
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellDouble_IndirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetContentsOfCell("A1", "10");
            spreadsheet.SetContentsOfCell("B1", "=A1 * 1.7");
            spreadsheet.SetContentsOfCell("C1", "=B1 + 20.99");
                                                
            spreadsheet.SetContentsOfCell("X1", "=2 + 2");
            spreadsheet.SetContentsOfCell("Y1", "=3 * 1");

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetContentsOfCell("A1", "1.0");
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
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellString_NewCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            IList<string> A1Results = spreadsheet.SetContentsOfCell("A1", "new cell content");
            IList<string> B1Results = spreadsheet.SetContentsOfCell("B1", "");
            IList<string> C1Results = spreadsheet.SetContentsOfCell("C1", "&%$@!#");

            Assert.IsTrue(A1Results.Count == 1);
            Assert.IsTrue(A1Results.Contains("A1"));
            Assert.IsTrue(B1Results.Count == 1);
            Assert.IsTrue(B1Results.Contains("B1"));
            Assert.IsTrue(C1Results.Count == 1);
            Assert.IsTrue(C1Results.Contains("C1"));
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellString_DirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetContentsOfCell("A1", "cell contents");
                        
            spreadsheet.SetContentsOfCell("D1", "=A1 * 1.7");
            spreadsheet.SetContentsOfCell("D2", "=A1 * A1");
            spreadsheet.SetContentsOfCell("D3", "=A1 / 3e2");
            spreadsheet.SetContentsOfCell("D4", "=( A1 + 12 ) / 5");
                                               
            spreadsheet.SetContentsOfCell("X1", "=2 + 2");
            spreadsheet.SetContentsOfCell("Y1", "=3 * 1");

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetContentsOfCell("A1", "1.0");

            HashSet<string> expectedDependents = new HashSet<string>();
            expectedDependents.Add("A1");
            expectedDependents.Add("D1");
            expectedDependents.Add("D2");
            expectedDependents.Add("D3");
            expectedDependents.Add("D4");

            // asserts that getNames...() only enumerates the expected names && doesn't return the same name more than once
            foreach (string cellName in A1Dependents)
            {
                // only true if expectedNames contains the element
                // changes expectedNames so that removing the same cell twice would fail
                Assert.IsTrue(expectedDependents.Remove(cellName));
            }
            // asserts that getNames...() didn't skip any of the expected names
            Assert.AreEqual(0, expectedDependents.Count);
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellString_IndirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetContentsOfCell("A1", "excellent cell");
            spreadsheet.SetContentsOfCell("B1", "=A1 * 1.7");
            spreadsheet.SetContentsOfCell("C1", "=B1 + 20.99");
                        
            spreadsheet.SetContentsOfCell("X1", "=2 + 2");
            spreadsheet.SetContentsOfCell("Y1", "=3 * 1");

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetContentsOfCell("A1", "should've bought GME");
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
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellFormula_NewCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            IList<string> A1Results = spreadsheet.SetContentsOfCell("A1", "=1 + 2");
            IList<string> B1Results = spreadsheet.SetContentsOfCell("B1", "=V4 - 12");

            Assert.IsTrue(A1Results.Count == 1);
            Assert.IsTrue(A1Results.Contains("A1"));
            Assert.IsTrue(B1Results.Count == 1);
            Assert.IsTrue(B1Results.Contains("B1"));
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellFormula_DirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetContentsOfCell("A1", "=1 * 2 * 3 * 4");
                                                
            spreadsheet.SetContentsOfCell("D1", "=A1 * 1.7");
            spreadsheet.SetContentsOfCell("D2", "=A1 * A1");
            spreadsheet.SetContentsOfCell("D3", "=A1 / 3e2");
            spreadsheet.SetContentsOfCell("D4", "=( A1 + 12 ) / 5");
                                                
            spreadsheet.SetContentsOfCell("X1", "=2 + 2");
            spreadsheet.SetContentsOfCell("Y1", "=3 * 1");

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetContentsOfCell("A1", "1.0");

            HashSet<string> expectedDependents = new HashSet<string>();
            expectedDependents.Add("A1");
            expectedDependents.Add("D1");
            expectedDependents.Add("D2");
            expectedDependents.Add("D3");
            expectedDependents.Add("D4");

            // asserts that getNames...() only enumerates the expected names && doesn't return the same name more than once
            foreach (string cellName in A1Dependents)
            {
                // only true if expectedNames contains the element
                // changes expectedNames so that removing the same cell twice would fail
                Assert.IsTrue(expectedDependents.Remove(cellName));
            }
            // asserts that getNames...() didn't skip any of the expected names
            Assert.AreEqual(0, expectedDependents.Count);
        }

        /// <summary>
        /// Tests that SetCell...() returns the correct lists.
        /// </summary>
        [TestMethod]
        public void SetCellFormula_IndirectDependents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            // populate the spreadsheet
            spreadsheet.SetContentsOfCell("A1", "=12");
            spreadsheet.SetContentsOfCell("B1", "=A1 * 1.7");
            spreadsheet.SetContentsOfCell("C1", "=B1 + 20.99");
                               
            spreadsheet.SetContentsOfCell("X1", "=2 + 2");
            spreadsheet.SetContentsOfCell("Y1", "=3 * 1");

            // retrieve / test the list of results
            IList<string> A1Dependents = spreadsheet.SetContentsOfCell("A1", "=9000.0001");
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
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDouble_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "0.0");
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with an invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDouble_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("1A", "0.0");
        }

        /// <summary>
        /// Asserts that an error is thrown when assigning "null" as a cell's content.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellString_Error_NullContent()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("_valid_cell", (string)null);
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with a "null" name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellString_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "innocuous string");
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellString_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("1A", "a meaningless string");
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with a "null" name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormula_Error_NullName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell(null,"=1 + 2");
        }

        /// <summary>
        /// Asserts that an error is thrown when setting the value of a cell with an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormula_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("1A", "=3 + 2");
        }

        /// <summary>
        /// Asserts that an exception is thrown when setting the value of a cell to an invalid Formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetCellContents_Error_InvalidFormula()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=2 * invalid formula + 3");
        }

        /// <summary>
        /// Asserts that an error is thrown when the new Formula content would result in a cycle of dependencies.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellFormula_Error_CircularDependency()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "1");
            spreadsheet.SetContentsOfCell("B1", "=A1 + 1");
            spreadsheet.SetContentsOfCell("C1", "=B1 + 1");
            spreadsheet.SetContentsOfCell("A1", "=C1 + 1");
        }


    }
}
