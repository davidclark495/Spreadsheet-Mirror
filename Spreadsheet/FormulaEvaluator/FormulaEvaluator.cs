// Author:  David Clark
// Date:    2021-01-27

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// 
    /// </summary>
    public static class FormulaEvaluator
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

                // if t is only whitespace, ignore
                if (char.IsWhiteSpace( token.ToCharArray()[0] )){
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
                if (isVariable(token))
                {
                    int tokenValue = variableEvaluator(token);
                    valueStack.Push(tokenValue);
                }

                // check: t is a + or -
                else if (token == "+" || token =="-")
                {
                    // If the previous operator was a + or -, apply that operation to members of the value stack.
                    addOrSubtract(valueStack, opStack);
                    opStack.Push(token);

                    //to-do: handle errors
                }

                // check: t is a * or /
                else if (token == "*" || token == "/")
                {
                    opStack.Push(token);
                }

                // check: t is a left parenthesis
                else if (token == "(")
                {
                    opStack.Push(token);
                }

                // check: t is a right parenthesis
                else if (token == ")")
                {
                    // if + or - is on top of the opStack, apply the operation accordingly
                    addOrSubtract(valueStack, opStack);

                    // expectation: next token in the opStack should be a "("
                    if (opStack.Peek() != "(")
                        throw new ArgumentException("Expected '('");
                    opStack.Pop();

                    // if * or / is on top of the opStack, apply the operation accordingly
                    multiplyOrDivide(valueStack, opStack);
                }
                
            }

            // after last token is processed...
            
            // if opStack is not empty
            if(opStack.Count != 0)
            {
                if (opStack.Count != 1)
                    throw new ArgumentException("End of processing: Too many operators");
                if (opStack.Peek() != "+" && opStack.Peek() != "-")
                    throw new ArgumentException("End of processing: Last remaining operator must be '+' or '-'");
                if (valueStack.Count != 2)
                    throw new ArgumentException("End of processing: Value stack contains wrong number of elements");

                addOrSubtract(valueStack, opStack);
            }

            if (valueStack.Count != 1)
                throw new ArgumentException("End of processing: Value stack should contain 1 element");

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
            // avoid error: opStack is empty
            if (opStack.Count == 0)
                return;

            // avoid error: valueStack has fewer than 2 items
            if (valueStack.Count < 2)
                throw new ArgumentException("Missing a value");

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
            // avoid error: opStack is empty
            if (opStack.Count == 0)
                return;

            // avoid error: valueStack has fewer than 2 items
            if (valueStack.Count < 2)
                throw new ArgumentException("Missing a value");

            if (opStack.Peek() == "*" || opStack.Peek() == "/")
            {
                String prevOp = opStack.Pop();
                int currValue = valueStack.Pop();
                int prevValue = valueStack.Pop();

                if (prevOp == "*")
                    valueStack.Push(prevValue * currValue);
                else if (prevOp == "/" && currValue != 0)
                    valueStack.Push(prevValue / currValue);
                else 
                    throw new ArgumentException("Division by 0");
                   
                
            }
        }

        /// <summary>
        /// Helper method for Evaluate.
        /// Returns true if the string represents a valid variable name.
        /// </summary>
        /// <param name="str">A string that may or may not represent a variable name.</param>
        /// <returns>True only if the string is a valid variable name.</returns>
        public static Boolean isVariable(string str)
        {
            // TODO: better definition
            // if the first character is a letter, that's enough
            if (char.IsLetter(str.ToCharArray()[0]))
                return true;
            else
                return false;
        }
    }
}
