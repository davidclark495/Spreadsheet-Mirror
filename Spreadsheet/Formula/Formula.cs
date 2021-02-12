// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
// Implementation by David Clark, CS 3500, February 2021


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private string formulaStr;
        private Func<string, string> normalize;
        private Func<string, bool> isValid;


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // TODO: implement checks, detect invalid formulas
            this.formulaStr = formula;
            this.normalize = normalize;
            this.isValid = isValid;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // init. 2 stacks
            Stack<String> opStack = new Stack<String>();
            Stack<double> valueStack = new Stack<double>();

            // process tokens
            foreach (String token in Formula.GetTokens(this.formulaStr))
            {
                // check: t is a double 
                if (Double.TryParse(token, out double result))
                {
                    valueStack.Push(result);

                    // if * or / is on top of the opStack, apply the operation accordingly, return error if necessary
                    multiplyOrDivide(valueStack, opStack, out bool divisionByZeroOccurred);
                    if (divisionByZeroOccurred)
                        return DivisionByZeroError();
                }

                // check: t is a var
                else if (isVariable(token))
                {
                    // if token cannot be evaluated, return an error
                    try
                    {
                        double value = lookup(normalize(token));
                        valueStack.Push(value);
                    }
                    catch (ArgumentException e)
                    {
                        return new FormulaError("Unknown Variable: your Lookup function couldn't find a reference to " + normalize(token) + ".");
                    }

                    // if * or / is on top of the opStack, apply the operation accordingly, return error if necessary
                    multiplyOrDivide(valueStack, opStack, out bool divisionByZeroOccurred);
                    if (divisionByZeroOccurred)
                        return DivisionByZeroError();
                }

                // check: t is a + or -
                else if (token == "+" || token == "-")
                {
                    // If the previous operator was a + or -, apply that operation to members of the value stack.
                    addOrSubtract(valueStack, opStack);
                    opStack.Push(token);
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

                    // next token in the opStack should be a "(", should be discarded
                    opStack.Pop();

                    // if * or / is on top of the opStack, apply the operation accordingly, return error if necessary
                    multiplyOrDivide(valueStack, opStack, out bool divisionByZeroOccurred);
                    if (divisionByZeroOccurred)
                        return DivisionByZeroError();
                }
            }

            // after last token is processed...

            // if opStack is not empty
            if (opStack.Count != 0)
            {
                addOrSubtract(valueStack, opStack);
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
        private void addOrSubtract(Stack<double> valueStack, Stack<string> opStack)
        {
            if (opStack.IsOnTop("+") || opStack.IsOnTop("-"))
            {
                String prevOp = opStack.Pop();
                double currValue = valueStack.Pop();
                double prevValue = valueStack.Pop();

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
        /// <param name="divisionByZeroOccurred">True only if the operation could not be completed because of a Divide By Zero error</param>
        private void multiplyOrDivide(Stack<double> valueStack, Stack<string> opStack, out bool divisionByZeroOccurred)
        {
            if (opStack.IsOnTop("*") || opStack.IsOnTop("/"))
            {
                String prevOp = opStack.Pop();
                double currValue = valueStack.Pop();
                double prevValue = valueStack.Pop();

                if (prevOp == "*")
                    valueStack.Push(prevValue * currValue);
                else if (prevOp == "/" && currValue != 0)
                    valueStack.Push(prevValue / currValue);
                else
                    divisionByZeroOccurred = true;
            }
            divisionByZeroOccurred = false;
        }

        /// <summary>
        /// Helper method for Evaluate and GetVariables.
        /// Returns true if the string represents a valid variable name.
        /// </summary>
        /// <param name="str">A string that may or may not represent a variable name.</param>
        /// <returns>True only if the string is a valid variable name.</returns>
        private Boolean isVariable(string str)
        {
            // pattern borrowed from GetTokens() method
            String basicVarPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";

            // true if str meets the general criteria for a variable 
            // i.e. starts with a letter or an underscore, then contains letters/numbers/underscores
            bool isBasicVar = Regex.IsMatch(normalize(str), basicVarPattern);

            // true if str meets this Formula's specific Validator requirements
            bool passesValidator = isValid(normalize(str));

            return isBasicVar && passesValidator;
        }

        /// <summary>
        /// Helper for Evaluate.
        /// </summary>
        /// <returns>Returns a Formula Error to be used when division by zero occurs.</returns>
        private FormulaError DivisionByZeroError()
        {
            return new FormulaError("Division By Zero error. Try changing your formula, or redefining certain variables.");
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            // a set containing each variable (without duplicates)
            HashSet<string> distinctVariables = new HashSet<string>();

            foreach(string token in Formula.GetTokens(this.formulaStr))
            {
                if (isVariable(token))
                    distinctVariables.Add(token);
            }

            return distinctVariables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return formulaStr;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            // if object is null, return false
            if (obj == null)
                return false;
            // if object is not a formula, return false
            if (!(obj is Formula))
                return false;

            return this.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null))
                return ReferenceEquals(f2, null);

            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }

    /// <summary>
    /// Class containing useful extensions for the Stack class. 
    /// Used by Evaluate() and its helper methods
    /// </summary>
    static class StackExtensions
    {

        /// <summary>
        /// Returns true if the next on item on the stack is "item."
        /// </summary>
        /// <typeparam name="T">The type of the stack and expected item.</typeparam>
        /// <param name="stack">A stack of type T</param>
        /// <param name="item">The item of interest that might be on the stack</param>
        /// <returns></returns>
        public static bool IsOnTop<T>(this Stack<T> stack, T item)
        {
            return stack.Count > 0 && stack.Peek().Equals(item);
        }
    }

}

