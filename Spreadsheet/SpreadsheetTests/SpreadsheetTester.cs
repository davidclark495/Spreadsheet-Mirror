// Author: David Clark
// CS 3500
// February 2021

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SpreadsheetUtilities;
using SS;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTester
    {

        // ---------- New Tests for PS5 ----------

        /// <summary>
        /// The validator should cause an exception to be thrown in any method with a cell name as a param.
        /// </summary>
        [TestMethod]
        public void ValidatorRejectsName_GetCellContents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet(s => false, s => s, "default");
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("A1"));

        }

        /// <summary>
        /// The validator should cause an exception to be thrown in any method with a cell name as a param.
        /// </summary>
        [TestMethod]
        public void ValidatorRejectsName_GetCellValue()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet(s => false, s => s, "default");
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellValue("A1"));
        }

        /// <summary>
        /// The validator should cause an exception to be thrown in any method with a cell name as a param.
        /// </summary>
        [TestMethod]
        public void ValidatorRejectsName_SetContentsOfCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet(s => false, s => s, "default");
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("A1", "valid cell contents"));
        }

        /// <summary>
        /// Tests the case where the normalizer converts variables into an invalid form 
        /// (as defined by the default Spreadsheet specifications).
        /// This should cause an exception to be thrown in any method with a cell name as a param.
        /// </summary>
        [TestMethod]
        public void NormalizerCausesInvalidName_GetCellContents()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet(s => true, s => "bad replacement", "default");
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests the case where the normalizer converts variables into an invalid form 
        /// (as defined by the default Spreadsheet specifications).
        /// This should cause an exception to be thrown in any method with a cell name as a param.
        /// </summary>
        [TestMethod]
        public void NormalizerCausesInvalidName_GetCellValue()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet(s => true, s => "bad replacement", "default");
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests the case where the normalizer converts variables into an invalid form 
        /// (as defined by the default Spreadsheet specifications).
        /// This should cause an exception to be thrown in any method with a cell name as a param.
        /// </summary>
        [TestMethod]
        public void NormalizerCausesInvalidName_SetContentsOfCell()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet(s => true, s => "bad replacement", "default");
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("A1", "valid contents"));
        }

        /// <summary>
        /// Demonstrates basic operations of the normalizer.
        /// Spreadsheet is expected to normalize cell names before storing them.
        /// </summary>
        [TestMethod]
        public void Normalizer_SetContentsOfCell()
        {
            SS.Spreadsheet spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");

            spreadsheet.SetContentsOfCell("a1", "contents");

            IEnumerator<string> cellNamesEnum = spreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator();
            cellNamesEnum.MoveNext();
            Assert.AreEqual("A1", cellNamesEnum.Current);
        }

        /// <summary>
        /// Demonstrates basic operations of the normalizer.
        /// Spreadsheet is expected to normalize cell names before using them.
        /// </summary>
        [TestMethod]
        public void Normalizer_GetCellContents()
        {
            SS.Spreadsheet spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");

            spreadsheet.SetContentsOfCell("a1", "2.0");
            spreadsheet.SetContentsOfCell("b1", "=4.5 - a1");
            spreadsheet.SetContentsOfCell("c1", "string contents");

            // Check that parameters are normalized before being evaluated.
            Assert.AreEqual(2.0, spreadsheet.GetCellContents("a1"));
            Assert.AreEqual(new Formula("4.5 - A1"), spreadsheet.GetCellContents("b1"));
            Assert.AreEqual("string contents", spreadsheet.GetCellContents("c1"));

            // Check that cells can be accessed with their normalized names.
            Assert.AreEqual(2.0, spreadsheet.GetCellContents("A1"));
            Assert.AreEqual(new Formula("4.5 - A1"), spreadsheet.GetCellContents("B1"));
            Assert.AreEqual("string contents", spreadsheet.GetCellContents("C1"));
        }

        /// <summary>
        /// Demonstrates basic operations of the normalizer.
        /// Spreadsheet is expected to normalize cell names before using them.
        /// </summary>
        [TestMethod]
        public void Normalizer_GetCellValue()
        {
            SS.Spreadsheet spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");

            spreadsheet.SetContentsOfCell("a1", "2.0");
            spreadsheet.SetContentsOfCell("b1", "=4.5 - a1");
            spreadsheet.SetContentsOfCell("c1", "string contents");

            // Check that parameters are normalized before being evaluated.
            Assert.AreEqual(2.0, spreadsheet.GetCellValue("a1"));
            Assert.AreEqual(2.5, spreadsheet.GetCellValue("b1"));
            Assert.AreEqual("string contents", spreadsheet.GetCellValue("c1"));

            // Check that cells can be accessed with their normalized names.
            Assert.AreEqual(2.0, spreadsheet.GetCellValue("A1"));
            Assert.AreEqual(2.5, spreadsheet.GetCellValue("B1"));
            Assert.AreEqual("string contents", spreadsheet.GetCellValue("C1"));
        }

        /// <summary>
        /// Demonstrates that two cells with different names may be normalized, 
        /// resulting in both names referring to the same cell.
        /// </summary>
        [TestMethod]
        public void Normalizer_Convergence()
        {
            SS.Spreadsheet spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");

            spreadsheet.SetContentsOfCell("a1", "2.0");
            spreadsheet.SetContentsOfCell("A1", "5.5");

            Assert.AreEqual(5.5, spreadsheet.GetCellContents("a1"));
            Assert.AreEqual(spreadsheet.GetCellContents("a1"), spreadsheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetCellValue_Double()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "3.0");
            spreadsheet.SetContentsOfCell("B1", "6e4");
            spreadsheet.SetContentsOfCell("C1", "-9");
            spreadsheet.SetContentsOfCell("D1", "0.555");

            Assert.AreEqual(3.0, spreadsheet.GetCellValue("A1"));
            Assert.AreEqual(6e4, spreadsheet.GetCellValue("B1"));
            Assert.AreEqual(-9.0, spreadsheet.GetCellValue("C1"));
            Assert.AreEqual(0.555, spreadsheet.GetCellValue("D1"));
        }

        /// <summary>
        /// Asserts that the value of a cell containing a Formula 
        /// is a double representing the Formula's solution (i.e. its evaluated state).
        /// </summary>
        [TestMethod]
        public void GetCellValue_Formula_SimpleArithmetic()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "=2 + 3");
            spreadsheet.SetContentsOfCell("B1", "= (5 * 1) / 3");

            Assert.AreEqual(5.0, spreadsheet.GetCellValue("A1"));
            Assert.AreEqual(5.0 / 3.0, spreadsheet.GetCellValue("B1"));
        }

        /// <summary>
        /// Asserts that the value of a cell containing a Formula with an error
        /// is a FormulaError object.
        /// </summary>
        [TestMethod]
        public void GetCellValue_Formula_FormulaError()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "=6 / 0");  // division by 0
            spreadsheet.SetContentsOfCell("B1", "=A1");     // Lookup, dependence on un-evaluate-able Formula
            spreadsheet.SetContentsOfCell("C1", "=X1");     // Lookup, dependence on empty cell / cell with String as contents

            Assert.IsTrue(spreadsheet.GetCellValue("A1") is FormulaError);
            Assert.IsTrue(spreadsheet.GetCellValue("B1") is FormulaError);
            Assert.IsTrue(spreadsheet.GetCellValue("C1") is FormulaError);
        }

        /// <summary>
        /// If a cell has dependees, it's value should change when its dependees change.
        /// </summary>
        [TestMethod]
        public void GetCellValue_Formula_ChangingDependees()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "=B1 * 2");
            Assert.IsTrue(spreadsheet.GetCellValue("A1") is FormulaError);

            spreadsheet.SetContentsOfCell("B1", "=4");
            Assert.AreEqual(8.0, spreadsheet.GetCellValue("A1"));

            spreadsheet.SetContentsOfCell("B1", "=( 9 * 3 )");
            Assert.AreEqual(54.0, spreadsheet.GetCellValue("A1"));

            spreadsheet.SetContentsOfCell("B1", "=1 / 0");
            Assert.IsTrue(spreadsheet.GetCellValue("A1") is FormulaError);
        }

        /// <summary>
        /// Asserts that the value of a cell containing a String
        /// is the string that was originally passed in as the content.
        /// </summary>
        [TestMethod]
        public void GetCellValue_String()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "valid string");
            spreadsheet.SetContentsOfCell("B1", "");
            spreadsheet.SetContentsOfCell("C1", "not a formula: =1 + 3");

            Assert.AreEqual("valid string", spreadsheet.GetCellValue("A1"));
            Assert.AreEqual("", spreadsheet.GetCellValue("B1"));
            Assert.AreEqual("not a formula: =1 + 3", spreadsheet.GetCellValue("C1"));

            Assert.AreEqual("", spreadsheet.GetCellValue("Z1"));    // uninitiallized cell, still holds string value
        }

        /// <summary>
        /// Asserts that an exception is thrown when GetCellValue() is passed an invalid cell name.
        /// </summary>
        [TestMethod]
        public void GetCellValue_Error_InvalidName()
        {
            SS.AbstractSpreadsheet spreadsheet = new SS.Spreadsheet();

            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellValue("-invalid-name-"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellValue(null));
        }

        /// <summary>
        /// Asserts that the spreadsheet's "Changed" property is
        /// false after calling the constructor.
        /// </summary>
        [TestMethod]
        public void Changed_OnCreate()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();
            Assert.IsFalse(spreadsheet.Changed);
        }

        /// <summary>
        /// Asserts that the spreadsheet's "Changed" property is
        /// true after changing a cell's contents.
        /// </summary>
        [TestMethod]
        public void Changed_AfterModifyingCell()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "new content");

            Assert.IsTrue(spreadsheet.Changed);
        }

        /// <summary>
        /// Asserts that the spreadsheet's "Changed" property is
        /// false after saving.
        /// </summary>
        [TestMethod]
        public void Changed_AfterSaving()
        {
            SS.Spreadsheet spreadsheet = new SS.Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "new content");
            spreadsheet.Save("save.xml");

            Assert.IsFalse(spreadsheet.Changed);
        }

        /// <summary>
        /// Asserts that an exception is thrown when saving to an invalid file.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Save_Error_InvalidFilePath()
        {
            SS.Spreadsheet savedSpreadsheet = new SS.Spreadsheet();

            savedSpreadsheet.Save("/missing/save.xml");
        }

        /// <summary>
        /// Asserts that an exception is thrown when saving to a null filepath.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Save_Error_NullFilePath()
        {
            SS.Spreadsheet savedSpreadsheet = new SS.Spreadsheet();

            savedSpreadsheet.Save(null);



        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Asserts that an empty spreadsheet can be loaded from an appropriate xml file.
        /// </summary>
        [TestMethod]
        public void Load_EmptySpreadsheet()
        {
            SS.Spreadsheet savedSpreadsheet = new SS.Spreadsheet();
            savedSpreadsheet.Save("save.xml");

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("save.xml", s => true, s => s, "default");

            IEnumerator<string> nonemptyNamesEnum = loadedSpreadsheet.GetNamesOfAllNonemptyCells().GetEnumerator();
            Assert.IsFalse(nonemptyNamesEnum.MoveNext());
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Asserts that a simple spreadsheet can be loaded from an appropriate xml file.
        /// The saved spreadsheet contains examples of doubles, strings, and formulas without dependencies.
        /// </summary>
        [TestMethod]
        public void Load_SimpleSpreadsheet()
        {
            SS.Spreadsheet savedSpreadsheet = new SS.Spreadsheet();
            savedSpreadsheet.SetContentsOfCell("A1", "3.14");
            savedSpreadsheet.SetContentsOfCell("B1", "=2 * ( X3 - 4 )");
            savedSpreadsheet.SetContentsOfCell("C1", "short string");
            savedSpreadsheet.Save("save.xml");

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("save.xml", s => true, s => s, "default");

            Assert.AreEqual(3.14, loadedSpreadsheet.GetCellContents("A1"));
            Assert.AreEqual(new Formula("2 * ( X3 - 4 )"), loadedSpreadsheet.GetCellContents("B1"));
            Assert.AreEqual("short string", loadedSpreadsheet.GetCellContents("C1"));
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Asserts that a simple spreadsheet can be loaded from an appropriate xml file.
        /// The saved spreadsheet contains a list of dependencies.
        /// </summary>
        [TestMethod]
        public void Load_SpreadsheetWithDependencies()
        {
            SS.Spreadsheet savedSpreadsheet = new SS.Spreadsheet();
            savedSpreadsheet.SetContentsOfCell("A5", "=A4 + 1");
            savedSpreadsheet.SetContentsOfCell("A4", "=A3 + 1");
            savedSpreadsheet.SetContentsOfCell("A3", "=A2 + 1");
            savedSpreadsheet.SetContentsOfCell("A2", "=A1 + 1");
            savedSpreadsheet.SetContentsOfCell("A1", "=1");
            savedSpreadsheet.Save("save.xml");

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("save.xml", s => true, s => s, "default");

            Assert.AreEqual(1.0, loadedSpreadsheet.GetCellValue("A1"));
            Assert.AreEqual(2.0, loadedSpreadsheet.GetCellValue("A2"));
            Assert.AreEqual(3.0, loadedSpreadsheet.GetCellValue("A3"));
            Assert.AreEqual(4.0, loadedSpreadsheet.GetCellValue("A4"));
            Assert.AreEqual(5.0, loadedSpreadsheet.GetCellValue("A5"));
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when the version defined in the constructor does not match 
        /// the version of the saved spreadsheet.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_Error_IncompatibleVersions()
        {
            SS.Spreadsheet savedSpreadsheet = new SS.Spreadsheet(s => true, s => s, "Version 1.0");

            savedSpreadsheet.Save("save.xml");

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("save.xml", s => true, s => s, "Version 5.0");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when attempting to load from an invalid file path.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_Error_InvalidFilePath()
        {
            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("/missing/save.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file without version information.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CellWithoutVersionAttribute()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "string of contents");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }



        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with incomplete "cell" tags.
        /// This method tests the event where the saved spreadsheet contains a single cell,
        /// and this cell has no "name" tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CellWithoutName()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with poorly written "cell" tags.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CellWithTwoNames()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("name", "A5");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with incomplete "cell" tags.
        /// This method tests the event where the saved spreadsheet contains a single cell,
        /// and this cell's name does not meet the spreadsheet's basic requirements.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CellWithInvalidName()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "$badname^&#@");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with incomplete "cell" tags.
        /// This method tests the event where the saved spreadsheet contains a single cell,
        /// and this cell has no "contents" tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CellWithoutContents()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with poorly written "cell" tags.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CellWithTwoContents()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteElementString("contents", "another string of contents");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with nested "cell" tags.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_NestedCells()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");


                writer.WriteStartElement("cell");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "Z1");
                writer.WriteElementString("contents", "inner contents");
                writer.WriteEndElement();

                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteEndElement();


                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file with "name" and "contents" tags outside of a cell.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_MissingCellTag()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "string of contents");

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires an exception when loading a file containing cells with circular dependencies.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Load_InvalidSave_CircularDependencies()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "=B1 + 1");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "=A1 + 1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("InvalidSave.xml", s => true, s => s, "default");
        }


        /// <summary>
        /// Any spreadsheet should be able to read the version description from a saved file.
        /// </summary>
        [TestMethod]
        public void GetSavedVersion()
        {
            SS.Spreadsheet defaultSpreadsheet = new SS.Spreadsheet();
            defaultSpreadsheet.Save("save.xml");
            Assert.AreEqual("default", defaultSpreadsheet.GetSavedVersion("save.xml"));

            SS.Spreadsheet v1Spreadsheet = new SS.Spreadsheet(s => true, s => s, "v1");
            v1Spreadsheet.Save("save.xml");
            Assert.AreEqual("v1", v1Spreadsheet.GetSavedVersion("save.xml"));

            // show that GetSavedVersion does not depend on the spreadsheet that calls it
            Assert.AreEqual("v1", defaultSpreadsheet.GetSavedVersion("save.xml"));
        }

        /// <summary>
        /// It should be impossible to get version information from an invalid file,
        /// so an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_Error_InvalidFilePath()
        {
            SS.Spreadsheet dummySpreadsheet = new SS.Spreadsheet();

            dummySpreadsheet.GetSavedVersion("/missing/save.xml");
        }

        /// <summary>
        /// An exception should be thrown when the filepath parameter is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_Error_NullFilename()
        {
            SS.Spreadsheet dummySpreadsheet = new SS.Spreadsheet();

            dummySpreadsheet.GetSavedVersion(null);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_InvalidSave_CellWithoutVersionAttribute()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("InvalidSave.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "string of contents");
                writer.WriteElementString("contents", "string of contents");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet dummySpreadsheet = new SS.Spreadsheet();
            dummySpreadsheet.GetSavedVersion("InvalidSave.xml");
        }

        /// <summary>
        /// Test that the spreadsheet throws when setting a cell's contents to null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContents_Error_NullContent()
        {
            SS.Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", null);
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires that the spreadsheet can load with large numbers of cells.
        /// </summary>
        [TestMethod]
        public void Load_LargeFile()
        {
            int numCells = 1000;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("save.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                for (int i = 1; i <= numCells; i++)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A" + i);
                    writer.WriteElementString("contents", "" + i);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("save.xml", s => true, s => s, "default");

            for (int i = 1; i <= numCells; i++)
            {
                Assert.AreEqual((double)i, (double)loadedSpreadsheet.GetCellValue("A" + i), 1e-9);
            }
        }

        /// <summary>
        /// Tests the four-argument constructor.
        /// Requires that the spreadsheet can load with large numbers of cells.
        /// </summary>
        [TestMethod]
        public void Load_LargeFile_DependencyChain()
        {
            int numCells = 100;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            using (XmlWriter writer = XmlWriter.Create("save.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                for (int i = 1; i <= numCells; i++)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A" + i);
                    writer.WriteElementString("contents", "=A" + (i + 1) + " - 1");
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A" + (numCells + 1));
                writer.WriteElementString("contents", "=" + (numCells + 1));
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            SS.Spreadsheet loadedSpreadsheet = new SS.Spreadsheet("save.xml", s => true, s => s, "default");

            for (int i = 1; i <= numCells; i++)
            {
                Assert.AreEqual((double)i, (double)loadedSpreadsheet.GetCellValue("A" + i), 1e-9);
            }
        }


        // ---------- My PS4 Tests (with modifications) ----------

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
            spreadsheet.SetContentsOfCell("_valid_cell", null);
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
            spreadsheet.SetContentsOfCell(null, "=1 + 2");
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
