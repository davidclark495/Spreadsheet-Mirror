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
    }
}
