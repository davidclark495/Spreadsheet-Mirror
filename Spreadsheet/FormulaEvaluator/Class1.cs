using System;

namespace FormulaEvaluator
{
    /// <summary>
    /// 
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //to-do



            //to-do: repurpose code, borrowed from assignment description, divides string into tokens
            //string[] substrings = Regex.Split(s, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return 0;
        }






    }
}
