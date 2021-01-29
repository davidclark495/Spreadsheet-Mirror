using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulaEvaluator;

namespace EvaluatorUnitTester
{
    [TestClass]
    public class EvaluatorUnitTester
    {
        public int zeroLookup(string str)
        {
            return 0;
        }

        public int firstLetterLookup(string str)
        {
            char first = str.ToCharArray()[0];
            int asciiValue = (int)first;
            return asciiValue;
        }






        [TestMethod]
        public void SingleNumber()
        {
            Assert.AreEqual(3, FormulaEvaluator.FormulaEvaluator.Evaluate("3", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleAddition()
        {
            Assert.AreEqual(5, FormulaEvaluator.FormulaEvaluator.Evaluate("1 + 4", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleSubtraction()
        {
            Assert.AreEqual(2, FormulaEvaluator.FormulaEvaluator.Evaluate("8 - 6", this.zeroLookup));
        }

        [TestMethod]
        public void SubtractionWithNegativeResult()
        {
            Assert.AreEqual(-12, FormulaEvaluator.FormulaEvaluator.Evaluate("30 - 42", this.zeroLookup));
        }

        [TestMethod]
        public void AdditionWithMultipleTerms()
        {
            Assert.AreEqual(20, FormulaEvaluator.FormulaEvaluator.Evaluate("5 + 3 + 4 + 6 + 2", this.zeroLookup));
        }

        [TestMethod]
        public void MixedAdditionAndSubtraction()
        {
            Assert.AreEqual(0, FormulaEvaluator.FormulaEvaluator.Evaluate("10 + 2 - 6 + 1 - 7", this.zeroLookup));
        }

        [TestMethod]
        public void singleVar()
        {
            Assert.AreEqual(0, FormulaEvaluator.FormulaEvaluator.Evaluate("A7", this.zeroLookup));
            Assert.AreEqual(0, FormulaEvaluator.FormulaEvaluator.Evaluate("AA0", this.zeroLookup));
            Assert.AreEqual(0, FormulaEvaluator.FormulaEvaluator.Evaluate("XZ883", this.zeroLookup));
            Assert.AreEqual(0, FormulaEvaluator.FormulaEvaluator.Evaluate("A7", this.zeroLookup));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void invalidVar()
        {
            FormulaEvaluator.FormulaEvaluator.Evaluate("letters", this.zeroLookup);
        }


        /// <summary>
        /// Tests the regex expression used in FormulaEvaluator, does not directly reference it though.
        /// </summary>
        [TestMethod]
        public void VariableIdentifierRegex()
        {
            string[] goodVariables = { "A4", "AaR42901", "Z0" };
            string[] badVariables  = { "4A", "1234", "X4randomcrap3490^3#2", "X4!", "(A2)", "", " " };

            foreach(string goodVar in goodVariables)
            {
                bool goodVarIsValid = System.Text.RegularExpressions.Regex.IsMatch(goodVar, "^[A-Za-z]+[0-9]+$");
                Assert.IsTrue(goodVarIsValid);
            }

            foreach (string badVar in badVariables)
            {
                bool badVarIsValid = System.Text.RegularExpressions.Regex.IsMatch(badVar, "^[A-Za-z]+[0-9]+$");
                Assert.IsFalse(badVarIsValid);
            }


            
        }
    }
}
