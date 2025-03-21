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

        public int oneLookup(string str)
        {
            return 1;
        }

        public int alwaysThrowsLookup(string str)
        {
            throw new System.Exception("This delegate just throws exceptions");
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
            Assert.AreEqual(3, FormulaEvaluator.Evaluator.Evaluate("3", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleAddition()
        {
            Assert.AreEqual(5, FormulaEvaluator.Evaluator.Evaluate("1 + 4", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleSubtraction()
        {
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate("8 - 6", this.zeroLookup));
        }

        [TestMethod]
        public void SubtractionWithNegativeResult()
        {
            Assert.AreEqual(-12, FormulaEvaluator.Evaluator.Evaluate("30 - 42", this.zeroLookup));
        }

        [TestMethod]
        public void AdditionWithMultipleTerms()
        {
            Assert.AreEqual(20, FormulaEvaluator.Evaluator.Evaluate("5 + 3 + 4 + 6 + 2", this.zeroLookup));
        }

        [TestMethod]
        public void MixedAdditionAndSubtraction()
        {
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate("10 + 2 - 6 + 1 - 7", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleMultiplication()
        {
            Assert.AreEqual(50, FormulaEvaluator.Evaluator.Evaluate("2 * 5 * 5", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleDivision()
        {
            Assert.AreEqual(5, FormulaEvaluator.Evaluator.Evaluate("38 / 7", this.zeroLookup));
        }

        [TestMethod]
        public void OrderOfOps()
        {
            Assert.AreEqual(-20, FormulaEvaluator.Evaluator.Evaluate("10 - 5 * 6", this.zeroLookup));
            Assert.AreEqual(-92, FormulaEvaluator.Evaluator.Evaluate("4 + 8 / 2 - 9 * 10 - 6 - 4", this.zeroLookup));
        }

        [TestMethod]
        public void OrderOfOpsWithParentheses()
        {
            Assert.AreEqual(16, FormulaEvaluator.Evaluator.Evaluate("( 3 - 1 ) * 8", this.zeroLookup));
            Assert.AreEqual(-4, FormulaEvaluator.Evaluator.Evaluate("4 + 8 / ( 2 - 9 ) * ( 10 - 6 ) - 4", this.zeroLookup));
        }

        [TestMethod]
        public void SimpleParentheses()
        {
            Assert.AreEqual(5, FormulaEvaluator.Evaluator.Evaluate(" (5) ", this.zeroLookup));
            Assert.AreEqual(10, FormulaEvaluator.Evaluator.Evaluate(" ( 10 ) ", this.zeroLookup));
        }

        [TestMethod]
        public void NestedParentheses()
        {
            Assert.AreEqual(60, FormulaEvaluator.Evaluator.Evaluate("( ( 2 + 3 ) * 2 ) * ( 2 * 3 )", this.zeroLookup));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_TooManyOperators()
        {
            FormulaEvaluator.Evaluator.Evaluate(" 5 + 3 + ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void Error_DelegateThrows(){
            FormulaEvaluator.Evaluator.Evaluate(" 3 + A5 ", this.alwaysThrowsLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_DivideBy0()
        {
            FormulaEvaluator.Evaluator.Evaluate(" 10 / 0 ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_TooManyValues()
        {
            FormulaEvaluator.Evaluator.Evaluate(" 10 + A3 + A6 5 3 ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_UnmatchedLeftParenthesis()
        {
            FormulaEvaluator.Evaluator.Evaluate(" 10 + ( A3 + 2", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_UnmatchedRightParenthesis()
        {
            FormulaEvaluator.Evaluator.Evaluate(" 10 + A3 ) + 2", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_AddOrSubtractMissingValue()
        {
            FormulaEvaluator.Evaluator.Evaluate(" + 4 ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_MultiplyOrDivideMissingValue()
        {
            FormulaEvaluator.Evaluator.Evaluate(" * 4 ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_AllTokensProcessed_MissingValue()
        {
            FormulaEvaluator.Evaluator.Evaluate(" * ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_EmptyExpression()
        {
            FormulaEvaluator.Evaluator.Evaluate(" ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_UnaryMinus()
        {
            FormulaEvaluator.Evaluator.Evaluate(" -4 + 10 ", this.zeroLookup);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_UnrecognizedToken()
        {
            FormulaEvaluator.Evaluator.Evaluate(" 4! + 3 ", this.zeroLookup);
        }

        [TestMethod]
        public void singleVar()
        {
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate("A7", this.zeroLookup));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate("AA0", this.zeroLookup));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate("XZ883", this.zeroLookup));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate("A7", this.zeroLookup));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Error_invalidVar()
        {
            FormulaEvaluator.Evaluator.Evaluate("letters", this.zeroLookup);
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
