// Author: David Clark
// CS 3500
// February 2021

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formula;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTester
    {
        // this is a Lookup delegate that will give any variable the value 0
        private System.Func<string, double> zeroLookup = str => 0.0;

        [TestMethod]
        public void EvaluateDivByZero()
        {
            
            Assert.IsTrue(new Formula("1 / 0").Evaluate(zeroLookup) is FormulaError);
        }

        [TestMethod]
        public void EvaluateInts()
        {
            Assert.AreEqual(2, new Formula("1 + 1").Evaluate(zeroLookup));
            Assert.AreEqual(8, new Formula("( 1 + 3 ) * 2").Evaluate(zeroLookup));
            Assert.AreEqual(5, new Formula("25 / 5 + 3 - 3").Evaluate(zeroLookup));
            Assert.AreEqual(2, new Formula("1 + 1").Evaluate(zeroLookup));
        }
    }
}
