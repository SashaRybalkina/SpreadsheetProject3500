// Skeleton written by Joe Zachary for CS 3500, September 2013
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
//
// Final version by Sasha Rybalkina (2/1/2023)
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard 
    ///precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-
    ///precision
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four 
    ///operator
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, 
    ///"xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a 
    ///single variable;
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a 
    ///validator.The
    /// normalizer is used to convert variables into a canonical form, and the 
    ///validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard 
    ///requirement
    /// that it consist of a letter or underscore followed by zero or more letters, 
    ///underscores,
    /// or digits.)  Their use is described in detail in the constructor and method 
    ///comments.
    /// </summary>
    public class Formula
    {
        private List<string> variables = new();
        private List<string> tokens = new();
        private string formula = "";
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression 
        ///written as
        /// described in the class comment.  If the expression is syntactically 
        /// invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated 
        /// validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }
        /// <summary>
        /// This is a private helper method for determining the index of the
        /// next token in the formula.
        /// </summary>
        /// <param name="array">The formula tokens</param>
        /// <param name="index">The current index</param>
        /// <returns></returns>
        private static int determineNext(string[] array, int index)
        {
            if (array[index + 1] == "")
            {
                return index + 2;
            }
            else
            {
                return index + 1;
            }
        }
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression 
        /// written as
        /// described in the class comment.  If the expression is syntactically 
        /// incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third 
        /// parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal 
        /// variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is 
        /// false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to 
        /// upper case, and
        /// that V is a method that returns true only if a string consists of one 
        /// letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is 
        /// false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is 
        /// syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            if (formula == null || formula == "")
            {
                throw new FormulaFormatException("Cannot have a null or empty formula");
            }
            List<string> formulaArray = GetTokens(formula).ToList();
            int lastIndex = formulaArray.Count() - 1;
            var left = formula.Count(x => x == '(');
            var right = formula.Count(x => x == ')');
            if (left != right)
            {
                throw new FormulaFormatException("Cannot have open parentheses");
            }
            for (int i = 0; i < lastIndex + 1; i++)
            {
                ///The next index
                int next = i + 1;

                ///This if statement checks for all errors associated with operators. Two operators cannot be next to each other,
                ///an operator cannot precede a right parenthesis, and a division by zero cannot occur.
                if (formulaArray[i] == "+" || formulaArray[i] == "-" || formulaArray[i] == "*" || formulaArray[i] == "/")
                {
                    if (i == 0 || i == lastIndex)
                    {
                        throw new FormulaFormatException("Cannot have trailing operators");
                    }
                    ///If an operator is right next to another operator or a right parenthesis, throws an exception.
                    if (i < lastIndex && (formulaArray[next] == "+" || formulaArray[next] == "-" || formulaArray[next] == "*" || formulaArray[next] == "/" || formulaArray[next] == ")"))
                    {
                        throw new FormulaFormatException("Cannot have two consecutive opperators or an opperator outside of parentheses.");
                    }
                    ///If a division by zero occurs, throws an exception.
                    if (formulaArray[i] == "/" && formulaArray[next] == "0")
                    {
                        throw new FormulaFormatException("Cannot divide by zero");
                    }
                    ///Builds the formula string and tokens list for later use.
                    this.formula = this.formula + formulaArray[i];
                    tokens.Add(formulaArray[i]);
                }

                ///This if statement checks for all errors associated with integers. Two integers cannot be next to each other,
                ///an integer cannot be right next to a variable, and an integer cannot be outside of parentheses.
                else if (Double.TryParse(formulaArray[i], result: out double Result))
                {
                    if (i < lastIndex && (Double.TryParse(formulaArray[next], result: out double Result2)))
                    {
                        throw new FormulaFormatException("Cannot have two consecutive numbers in expression.");
                    }
                    else if (i < lastIndex && Regex.IsMatch(formulaArray[next], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("Cannot have an integer right next to a variable.");
                    }
                    else if (i < lastIndex && (formulaArray[next] == "("))
                    {
                        throw new FormulaFormatException("Cannot have a variable right outside of parentheses.");
                    }

                    this.formula = this.formula + Result;
                    tokens.Add("" + Result);
                }

                ///This if statement checks for all errors associated with the right parenthesis. An integer cannot be outside
                ///of parentheses, a variable caanot be outside of parentheses, and an operator cannot be outside of parentheses.
                else if (formulaArray[i] == ")")
                {
                    if (i < lastIndex && Double.TryParse(formulaArray[next], result: out Result))
                    {
                        throw new FormulaFormatException("Cannot have an integer right outisde of parentheses.");
                    }
                    else if (i < lastIndex && Regex.IsMatch(formulaArray[next], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("Cannot have a variable right outisde of parentheses.");
                    }
                    else if (i < lastIndex && (formulaArray[next] == "+" || formulaArray[next] == "-" || formulaArray[next] == "*" || formulaArray[next] == "/"))
                    {
                        throw new FormulaFormatException("Cannot have a opperator right outisde of parentheses.");
                    }

                    this.formula = this.formula + formulaArray[i];
                    tokens.Add(formulaArray[i]);
                }

                ///This if statement checks for all errors associated with the left parenthesis. An operator cannot come after a
                ///right parenthesis and closed parentheses cannot be empty.
                else if (formulaArray[i] == "(")
                {
                    if (i < lastIndex && (formulaArray[next] == "+" || formulaArray[next] == "-" || formulaArray[next] == "*" || formulaArray[next] == "/"))
                    {
                        throw new FormulaFormatException("Cannot have an operator after a left parenthesis.");
                    }
                    if (i < lastIndex && (formulaArray[next] == ")"))
                    {
                        throw new FormulaFormatException("Cannot have empty parentheses.");
                    }

                    this.formula = this.formula + formulaArray[i];
                    tokens.Add(normalize(formulaArray[i]));
                }

                ///This is where all errors associated with variables are handled. If a variable is invalid, or is next to another
                ///variable or integer, or if the variable is outside of parentheses, throws an exception.
                else
                {
                    if (!Regex.IsMatch(formulaArray[i], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("The variable entered must have one integer and one character.");
                    }
                    else if (!isValid(normalize(formulaArray[i])))
                    {
                        throw new FormulaFormatException("The variable entered is not valid.");
                    }
                    else if (i < lastIndex && Double.TryParse(formulaArray[next], result: out Result))
                    {
                        throw new FormulaFormatException("Cannot have an integer right next to a variable.");
                    }
                    else if (i < lastIndex && Regex.IsMatch(formulaArray[next], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("Cannot have two consecutive variables in expression");
                    }
                    else if (i < lastIndex && formulaArray[next] == "(")
                    {
                        throw new FormulaFormatException("Cannot have a variable outised of parentheses");
                    }

                    if (!variables.Contains(normalize(formulaArray[i])))
                    {
                        variables.Add(normalize(formulaArray[i]));
                    }
                    this.formula = this.formula + normalize(formulaArray[i]);
                    tokens.Add(normalize(formulaArray[i]));
                }
            }
        }
        /// <summary>
        /// Private helper method for adding and subtracting integers.
        /// </summary>
        /// <param name="value1">First integer for evaluation</param>
        /// <param name="value2">Second integer for evaluation</param>
        /// <param name="op">The operator to be used</param>
        /// <returns>The evaluation of the two integers based on the operator given.</returns>
        private static double AddOrSubtract(double value1, double value2, string op)
        {
            if (op == "+")
            {
                return value1 + value2;
            }
            else
            {
                return value1 - value2;
            }
        }
        /// <summary>
        /// Private helper method for multiplying and dividing integers.
        /// </summary>
        /// <param name="value1">First integer for evaluation</param>
        /// <param name="value2">Second integer for evaluation</param>
        /// <param name="op">The operator to be used</param>
        /// <returns>The evaluation of the two integers</returns>
        private static double MultiplyOrDivide(double value1, double value2, string op)
        {
            if (op == "*")
            {
                return value1 * value2;
            }
            else
            {
                return value1 / value2;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookup">The delegate for looking up the values of variables</param>
        /// <returns></returns>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<Double> ValueStack = new System.Collections.Generic.Stack<Double>();
            Stack<string> OperatorStack = new System.Collections.Generic.Stack<string>();
            if (lookup == null)
            {
                return new FormulaError();
            }
            foreach (string token in tokens)
            {
                ///This if statement sets up the operator stack for later use. Only the "(", "*" and "/"
                ///operators should be added to the stack
                if (token == "(" || token == "*" || token == "/")
                {
                    OperatorStack.Push(token);
                }
                ///This if statement determines if the string being worked with is an integer. If it is,
                ///then checks how the integer should be treated based on the operators being used.
                else if (Double.TryParse(token, result: out double Result))
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                    {
                        ValueStack.Push(MultiplyOrDivide(ValueStack.Pop(), Result, OperatorStack.Pop()));
                    }
                    else
                    {
                        ValueStack.Push(Result);
                    }
                }

                ///Repeats the process of the previous if statement, except with variables. A
                ///delegate is used to look up the value of a variable so that it can be evaluated.
                else if (Regex.IsMatch(token, "[a-z|A-Z][0-9]"))
                {
                    try
                    {
                        if (OperatorStack.Count() != 0 && (OperatorStack != null && OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                        {
                            ValueStack.Push(MultiplyOrDivide(ValueStack.Pop(), (int)lookup(token), OperatorStack.Pop()));
                        }
                        else
                        {
                            ValueStack.Push((int)lookup(token));
                        }
                    }
                    catch
                    {
                        return new FormulaError();
                    }
                }

                ///This if statement works with the "+" and "-" operators. If there is already
                ///a "+" or "-" at the top of the operator stack, then two integers from the
                ///value stack are evaluated using one of the operators. The operator then gets
                ///pushed onto the operator stack.
                else if (token == "+" || token == "-")
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                    {
                        double value2 = ValueStack.Pop();
                        double value1 = ValueStack.Pop();
                        ValueStack.Push(AddOrSubtract(value1, value2, OperatorStack.Pop()));
                    }
                    OperatorStack.Push(token);
                }

                ///This if statement works with right parentheses. It evaluates the integers in the
                ///value stack depending on the operator at the top of the operator stack.
                else if (token == ")")
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                    {
                        double value2 = ValueStack.Pop();
                        double value1 = ValueStack.Pop();
                        ValueStack.Push(AddOrSubtract(value1, value2, OperatorStack.Pop()));
                    }

                    OperatorStack.Pop();

                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                    {
                        double value2 = ValueStack.Pop();
                        double value1 = ValueStack.Pop();
                        ValueStack.Push(MultiplyOrDivide(value1, value2, OperatorStack.Pop()));
                    }
                }
            }

            double result = 0;

            ///If there is a remaining expression that didn't get resolved while the for loop
            ///was running, this is where this expression gets handled.
            if (ValueStack.Count() > 1)
            {
                double value2 = ValueStack.Pop();
                double value1 = ValueStack.Pop();
                if (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/")
                {
                    result = MultiplyOrDivide(value1, value2, OperatorStack.Pop());
                }
                if (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-")
                {
                    result = AddOrSubtract(value1, value2, OperatorStack.Pop());
                }
            }
            else
            {
                result = ValueStack.Pop();
            }

            return result;
        }
        /// <summary>
        /// Returns all of the the variables in a given formula. If the formula entered is
        /// "x5 + y6 + X5" and a normalizer is given that capitalizes the character in a
        /// given variable, then this method should return the list {X5, Y6}. If there is
        /// no normalizer, then the list should be {x5, y6, X5}.
        /// </summary>
        /// <returns>All variables in the formula</returns>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return formula;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            String string1 = this.ToString();
            String string2 = obj.ToString();
            if (string1.Equals(string2))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// This method creates a "==" operator for the Formula class by comparing
        /// two objects based on the rules of the Equals method.
        /// </summary>
        /// <param name="f1">The first object to be compared</param>
        /// <param name="f2">The second object to be compared</param>
        /// <returns>True if the two objects are equal, false otherwise.</returns>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// This method creates a "!=" operator that serevs as an opposite of the
        /// "==" operator and compared two objects to determine of they are unequal.
        /// </summary>
        /// <param name="f1">The first object to be compared</param>
        /// <param name="f2">The second object to be compared</param>
        /// <returns>True if the two objects are unequal, false otherwise.</returns>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns a hash code for the class based on the hash code of the string
        /// of the formula
        /// </summary>
        /// <returns>The hash code of the class</returns>
        public override int GetHashCode()
        {
            return formula.GetHashCode();
        }
        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are 
        /// left paren;
        /// right paren; one of the four operator symbols; a string consisting of a 
        /// letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal;
        /// and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token 
        /// contains white space.
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
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) |  ({5})",
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
}