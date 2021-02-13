// Author: David Clark
// CS 3500
// February 2021

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        // This is a Lookup delegate that will give any variable the value 0.
        private Func<string, double> zeroLookup = str => 0.0;

        // This is a Lookup delegate that won't recognize any variable.
        private Func<string, double> alwaysThrowsLookup = str => { throw new ArgumentException("Variable Not Found"); };

        // This determines the precision of the assert-equals statements that test the Evaluate method. 
        // The professor recommended this value.
        private double delta = 1e-9;

        /// <summary>
        /// This test shows an example of an error-free Formula.
        /// </summary>
        [TestMethod]
        public void Constructor_Basic()
        {
            // needs more test cases
            Formula form;
            form = new Formula("( 12 + 3 ) / 5 * A5");
            form = new Formula(" 7 - (2 * 0)");
            form = new Formula("6 / x + ( ( y ) )");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_UnrecognizedCharacter()
        {
            Formula form = new Formula("3 + 5!");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_AtLeastOneTokenRule()
        {
            Formula form = new Formula(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_RightParenthesesRule()
        {
            Formula form = new Formula(" ( C3 + 2 ) ) + ( 5");
        }

        /// <summary>
        /// Per the specifications, the total number of left- and right-parentheses must be equal.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_BalancedParenthesesRule()
        {
            Formula form = new Formula("( ( 5 )");
        }

        /// <summary>
        /// Per the specifications, the first token must be a number, var, or "(".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_StartingTokenRule()
        {
            Formula form = new Formula("/ 24");
        }

        /// <summary>
        /// Per the specifications, the last token must be a number, var, or ")".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_EndingTokenRule()
        {
            Formula form = new Formula("x + y * ");
        }

        /// <summary>
        /// Per the specifications, the token following a "(" or operator must be a number, var, or "(".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_ParenthesisOrOperatorFollowingRule()
        {
            Formula form = new Formula("( + 1");
        }

        /// <summary>
        /// Per the specifications, the token following a number, var, or ")" must be an operator or ")".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_Error_ExtraFollowingRule()
        {
            Formula form = new Formula("a1 3 + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_InvalidVar_StandardRules()
        {
            Formula form = new Formula("1337vars4c00lk1ds + 5");
        }

        /// <summary>
        /// The validator rejects a variable in the string. 
        /// In this case, the validator rejects all variables.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_InvalidVar_ValidatorRules()
        {
            Formula form = new Formula("a1 + 5", str => str, str => false);
        }

        /// <summary>
        /// The normalize function changes the variable str to an invalid string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_InvalidVar_NormalizeRuinsIt_StandardRules()
        {
            Formula form = new Formula("a1 + 5", str => "---TheOnlyVariable---", str => true);
        }

        /// <summary>
        /// The normalize function changes the variable str to a string that the validator rejects.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Constructor_InvalidVar_NormalizeRuinsIt_ValidatorRules()
        {
            Formula form = new Formula("a1 + 5", str => "long_name_that_the_validator_rejects", str => str.Length < 5);
        }


        [TestMethod]
        public void Evaluate_DivByZero()
        {
            Assert.IsInstanceOfType(new Formula("1 / 0").Evaluate(zeroLookup), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / ( 3 - 3 )").Evaluate(zeroLookup), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / _zeroVar").Evaluate(zeroLookup), typeof(FormulaError));

            // this bit isn't implementation agnostic, but it shows that the right error is being thrown
            FormulaError error = (FormulaError)(new Formula("1 / 0").Evaluate(zeroLookup));
            Assert.AreEqual("Division By Zero error. Try changing your formula, or redefining certain variables.", error.Reason);
        }

        [TestMethod]
        public void Evaluate_UnknownVar()
        {
            Assert.IsInstanceOfType(new Formula("1 * A1").Evaluate(alwaysThrowsLookup), typeof(FormulaError));
        }

        [TestMethod]
        public void Evaluate_NoDecimals()
        {
            double val1 = (double)(new Formula("1 + 1").Evaluate(zeroLookup));
            double val2 = (double)(new Formula("( 1 + 3 ) * 2").Evaluate(zeroLookup));
            double val3 = (double)(new Formula("25 / 5 + 3 - 3").Evaluate(zeroLookup));
            Assert.AreEqual(2.0, val1, delta);
            Assert.AreEqual(8.0, val2, delta);
            Assert.AreEqual(5.0, val3, delta);
        }

        [TestMethod]
        public void Evaluate_Variables()
        {
            double val1 = (double)(new Formula("1 + A0").Evaluate(zeroLookup));
            double val2 = (double)(new Formula("( x + 3 ) * 2").Evaluate(zeroLookup));
            double val3 = (double)(new Formula("_value / 5 + 3 - 3").Evaluate(zeroLookup));
            Assert.AreEqual(1.0, val1, delta);
            Assert.AreEqual(6.0, val2, delta);
            Assert.AreEqual(0.0, val3, delta);
        }

        /// <summary>
        /// This is mostly to test that I'm measuring precision correctly.
        /// This shows that the Formula works with floating point arithmetic, and that it is prone to slight errors.
        /// </summary>
        [TestMethod]
        public void Evaluate_DeltaTest()
        {
            double value = (double)(new Formula("5.6 - 3.6").Evaluate(zeroLookup));

            // double precision: 5.6 - 3.6 != 2.0
            Assert.AreNotEqual(2.0, value);

            // with delta, 5.6 - 3.6 is close enough to 2.0
            Assert.AreEqual(2.0, value, delta);
        }

        /// <summary>
        /// The formula should be able to return a list of variables without repeats. 
        /// A normalize function is not provided to the Formula.
        /// </summary>
        [TestMethod]
        public void GetVariables_DefaultNormalizer()
        {
            Formula form = new Formula("A1 + b1 - C1 / (D1 * A1) + d1");
            HashSet<string> expectedVars = new HashSet<string>();
            expectedVars.Add("A1");
            expectedVars.Add("b1");
            expectedVars.Add("C1");
            expectedVars.Add("D1");
            expectedVars.Add("d1");

            // Remove each variable from "expectedVars" as it appears in the Formula's enumeration.
            // If a variable appears twice, the second removal will fail.
            // If a variable appears which is not in expectedVars, the removal will fail.
            // If a variable is missing from the Formula's list, it will not be removed.
            foreach (string var in form.GetVariables())
            {
                Assert.IsTrue(expectedVars.Remove(var));
            }

            // If a variable was missing from the Formula's list, Count > 0, so the test will fail.
            Assert.AreEqual(0, expectedVars.Count);
        }

        /// <summary>
        /// The formula should be able to return a list of variables without repeats. 
        /// A normalize function is not provided to the Formula.
        /// The variables are meant to appear in the order they are found in the formula.
        /// </summary>
        [TestMethod]
        public void GetVariables_InSequence()
        {
            Formula form = new Formula("A1 + b1 - C1 / (D1 * A1) + d1");

            IEnumerator<string> varEnum = form.GetVariables().GetEnumerator();

            varEnum.MoveNext();
            Assert.AreEqual("A1", varEnum.Current);
            varEnum.MoveNext();
            Assert.AreEqual("b1", varEnum.Current);
            varEnum.MoveNext();
            Assert.AreEqual("C1", varEnum.Current);
            varEnum.MoveNext();
            Assert.AreEqual("D1", varEnum.Current);
            varEnum.MoveNext();
            Assert.AreEqual("d1", varEnum.Current);
        }

        /// <summary>
        /// The Formula should be able to return a list of variables without repeats. 
        /// The Formula has been given a normalize function.
        /// </summary>
        [TestMethod]
        public void GetVariables_CustomNormalizer()
        {
            Formula form = new Formula("a1 + b1 - C1 / (D1 * a1) + d1", str => str.ToUpper(), str => true);
            HashSet<string> expectedVars = new HashSet<string>();
            expectedVars.Add("A1");
            expectedVars.Add("B1");
            expectedVars.Add("C1");
            expectedVars.Add("D1");

            // Remove each variable from "expectedVars" as it appears in the Formula's enumeration.
            // If a variable appears twice, the second removal will fail.
            // If a variable appears which is not in expectedVars, the removal will fail.
            // If a variable is missing from the Formula's list, it will not be removed.
            foreach (string var in form.GetVariables())
            {
                Assert.IsTrue(expectedVars.Remove(var));
            }

            // If a variable was missing from the Formula's list, Count > 0, so the test will fail.
            Assert.AreEqual(0, expectedVars.Count);
        }

        /// <summary>
        /// ToString() should produce a string that can be used to initialize a new Formula.
        /// The old and new Formulas should be equivalent.
        /// </summary>
        [TestMethod]
        public void ToString_Reproducibility()
        {
            Formula parent = new Formula("1 + 1");
            Formula child = new Formula(parent.ToString());

            Assert.IsTrue(parent == child);
        }

        /// <summary>
        /// ToString() should return a normalized version of the original string it was initialized with.
        /// </summary>
        [TestMethod]
        public void ToString_Normalized()
        {
            Formula form = new Formula("a5 + 30 - x9", str => str.ToUpper(), str => true);

            Assert.AreEqual("A5 + 30 - X9", form.ToString());
        }

        [TestMethod]
        public void Equals()
        {
            Formula form1 = new Formula("5 + 5");
            Formula form1WithSpaces = new Formula(" 5 +    5 ");
            Formula form2 = new Formula("3 * 6");
            Object obj = new object();

            Assert.IsTrue(form1.Equals(form1));
            Assert.IsTrue(form1.Equals(form1WithSpaces));
            Assert.IsFalse(form1.Equals(form2));
            Assert.IsFalse(form1.Equals(obj));
        }


        /// <summary>
        /// The same tests as in "Equals", but using the == operator.
        /// </summary>
        [TestMethod]
        public void Equals_EqualsOp()
        {
            Formula form1 = new Formula("5 + 5");
            Formula form1WithSpaces = new Formula(" 5 +    5 ");
            Formula form2 = new Formula("3 * 6");

            Assert.IsTrue(form1 == form1);
            Assert.IsTrue(form1 == form1WithSpaces);
            Assert.IsFalse(form1 == form2);
        }

        /// <summary>
        /// The same tests as in "Equals", but using the != operator (and with the logic flipped).
        /// </summary>
        [TestMethod]
        public void Equals_NotEqualsOp()
        {
            Formula form1 = new Formula("5 + 5");
            Formula form1WithSpaces = new Formula(" 5 +    5 ");
            Formula form2 = new Formula("3 * 6");

            Assert.IsFalse(form1 != form1);
            Assert.IsFalse(form1 != form1WithSpaces);
            Assert.IsTrue(form1 != form2);
        }

        /// <summary>
        /// This method contains tests for null values using each equality function/operator.
        /// </summary>
        [TestMethod]
        public void Equals_Null()
        {
            Formula basicFormula = new Formula("5 + 5");
            Assert.IsFalse(basicFormula.Equals(null));
            Assert.IsFalse(basicFormula == null);
            Assert.IsTrue(basicFormula != null);

            Formula nullFormula = null;
            Assert.IsTrue(nullFormula == null);
            Assert.IsFalse(nullFormula != null);
        }

        /// <summary>
        /// If two formulas are equal, then they should have the same hash-code.
        /// </summary>
        [TestMethod]
        public void GetHashCode_Equality()
        {
            Formula form1 = new Formula("10 + 10");
            Formula form2 = new Formula("10 + 10");

            // Formulas are equal.
            Assert.IsTrue(form1 == form2);

            // Formulas have the same HashCode.
            Assert.AreEqual(form1.GetHashCode(), form2.GetHashCode());
        }

        /// <summary>
        /// Unequal Formulas should be unlikely to have equivalent hash-codes.
        /// </summary>
        [TestMethod]
        public void GetHashCode_Variability()
        {
            Formula stableForm = new Formula("10 * 0");

            // This loop generates a new formula, then ensures that the two hash-codes don't match.
            // New Formulas are of the form "10 * i".
            for (int i = 1; i <= 100; i++)
            {
                string newFormStr = "10 * " + i;
                Formula tempForm = new Formula(newFormStr);
                Assert.AreNotEqual(stableForm.GetHashCode(), tempForm.GetHashCode());
            }
        }
    }
}
