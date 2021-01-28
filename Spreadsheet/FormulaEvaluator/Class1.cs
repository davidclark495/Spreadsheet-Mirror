using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        /// <param name="exp"> The expression to be evaluated. </param>
        /// <param name="variableEvaluator"> A function that receives a variable name (a string) and returns an integer. </param>
        /// <returns> The integer value of the expression. </returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            // init. 2 stacks
            Stack<String> opStack = new Stack<String>();
            Stack<int> valueStack = new Stack<int>();

            // split string into tokens
            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            
            // trim tokens
            for(int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Trim();
                //to-do: check for empty strings
                //to-do: throw exception if token is invalid
            }

            // process tokens
            foreach(String token in tokens)
            {
                char firstChar = token.ToCharArray()[0];    // used throughout the block
                string prevOp = opStack.Peek();             // used throughout the block

                // if t is only whitespace, ignore
                if (char.IsWhiteSpace(firstChar)){
                    continue;
                }

                // check: t is an int 
                try
                {
                    valueStack.Push(Int32.Parse(token));
                    
                    // if * or / is on top of the opStack, apply the operation accordingly
                    multiplyOrDivide(valueStack, opStack);
                } catch (Exception e) { }

                // check: t is a var
                if (char.IsLetter(firstChar))
                {
                    int tokenValue = variableEvaluator(token);
                    valueStack.Push(tokenValue);
                }

                // check: t is a + or -
                if (token == "+" || token =="-")
                {
                    // If the previous operator was a + or -, apply that operation to members of the value stack.
                    addOrSubtract(valueStack, opStack);
                    opStack.Push(token);

                    //to-do: handle errors
                }

                // check: t is a * or /
                if (token == "*" || token == "/")
                {
                    opStack.Push(token);
                }

                // check: t is a left parenthesis
                if (token == "(")
                {
                    opStack.Push(token);
                }

                // check: t is a right parenthesis
                if (token == ")")
                {
                    // if + or - is on top of the opStack, apply the operation accordingly
                    addOrSubtract(valueStack, opStack);

                    // expectation: next token in the opStack should be a "("
                    opStack.Pop();

                    // if * or / is on top of the opStack, apply the operation accordingly
                    multiplyOrDivide(valueStack, opStack);
                }

            }

            // after last token is processed...
            
            // if stack is not empty
            if(opStack.Count != 0)
            {
                addOrSubtract(valueStack, opStack);
                // handle errors
            }

            return valueStack.Pop();
        }

        
        /// <summary>
        /// Helper method for Evaluate.
        /// Checks to see if the most recently pushed operator is + or -.
        /// If so, the operation is applied to the last two items in the value stack.
        /// The result is pushed to the value stack.
        /// </summary>
        /// <param name="valueStack">The stack containing all previously encountered values.</param>
        /// <param name="opStack">The stack containing all previously encountered operators.</param>
        public static void addOrSubtract(Stack<int> valueStack, Stack<string> opStack)
        {
            if (opStack.Peek() == "+" || opStack.Peek() == "-")
            {
                String prevOp = opStack.Pop();
                int currValue = valueStack.Pop();
                int prevValue = valueStack.Pop();

                if (prevOp == "+")
                    valueStack.Push(prevValue + currValue);
                else
                    valueStack.Push(prevValue - currValue);
            }
        }

        /// <summary>
        /// Helper method for Evaluate.
        /// Checks to see if the most recently pushed operator is * or /.
        /// If so, the operation is applied to the last two items in the value stack.
        /// The result is pushed to the value stack.
        /// </summary>
        /// <param name="valueStack">The stack containing all previously encountered values.</param>
        /// <param name="opStack">The stack containing all previously encountered operators.</param>
        public static void multiplyOrDivide(Stack<int> valueStack, Stack<string> opStack)
        {
            if (opStack.Peek() == "*" || opStack.Peek() == "/")
            {
                String prevOp = opStack.Pop();
                int currValue = valueStack.Pop();
                int prevValue = valueStack.Pop();

                if (prevOp == "+")
                    valueStack.Push(prevValue * currValue);
                else
                    valueStack.Push(prevValue / currValue);
            }
        }

    }
}
