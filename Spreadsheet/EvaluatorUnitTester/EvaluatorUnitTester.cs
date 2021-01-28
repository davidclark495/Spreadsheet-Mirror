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
            Assert.AreEqual(3, Evaluator.Evaluate("3", zeroLookup));
        }

        
    }
}
