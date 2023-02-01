﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
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
        private ArrayList variables = new();
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
            if (array[index+1] == "")
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
        public Formula(String formula, Func<string, string> normalize, Func<string,
    bool> isValid)
        {
            string[] formulaArray = Regex.Split(formula, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            var left = formula.Count(x => x == '(');
            var right = formula.Count(x => x == ')');
            if (left != right) throw new FormulaFormatException("Cannot have open parentheses");

            for (int i = 0; i < formulaArray.Length; i++)
            {
                ///This skips over white space
                if (formulaArray[i] == "")
                {
                    i++;
                    if (i == formulaArray.Length)
                    {
                        break;
                    }
                }
                ///This if statement checks for all errors associated with operators. Two operators cannot be next to each other,
                ///an operator cannot precede a right parenthesis, and a division by zero cannot occur.
                if (formulaArray[i] == "+" || formulaArray[i] == "-" || formulaArray[i] == "*" || formulaArray[i] == "/")
                {
                    ///This determines the index of the next token.
                    int next = determineNext(formulaArray, i);
                    ///If an operator is right next to another operator or a right parenthesis, throws an exception.
                    if (i < formulaArray.Length - 1 && (formulaArray[next] == "+" || formulaArray[next] == "-" || formulaArray[next] == "*" || formulaArray[next] == "/" || formulaArray[next] == ")"))
                    {
                        throw new FormulaFormatException("Cannot have two consecutive opperators or an opperator outside of parentheses.");
                    }
                    ///If a division by zero occurs, throws an exception.
                    if (formulaArray[i] == "/" && formulaArray[next] == "0")
                    {
                        throw new FormulaFormatException("Cannot divide by zero");
                    }
                    ///Builds the formula string for later use.
                    this.formula = this.formula + formulaArray[i];
                }

                ///This if statement checks for all errors associated with integers. Two integers cannot be next to each other,
                ///an integer cannot be right next to a variable, and an integer cannot be outside of parentheses.
                else if (Int32.TryParse(formulaArray[i], result: out int intResult))
                {
                    ///This determines the index of the next token.
                    int next = determineNext(formulaArray, i);
                    ///If two integers are right next to each other, throws exception.
                    if (i < formulaArray.Length - 1 && (Int32.TryParse(formulaArray[next], result: out int intResult2)))
                    {
                        throw new FormulaFormatException("Cannot have two consecutive numbers in expression.");
                    }
                    ///If an integer is next to a variable, throws an exception.
                    else if (i < formulaArray.Length - 1 && Regex.IsMatch(formulaArray[next], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("Cannot have an integer right next to a variable.");
                    }
                    else if (i < formulaArray.Length - 1 && (formulaArray[next] == "("))
                    {
                        throw new FormulaFormatException("Cannot have a variable right outside of parentheses.");
                    }
                    //Builds the formula string for later use.
                    this.formula = this.formula + intResult;
                }

                else if (formulaArray[i] == ")")
                {
                    int next = determineNext(formulaArray, i);

                    if (i < formulaArray.Length - 1 && Int32.TryParse(formulaArray[next], result: out int intResult2))
                    {
                        throw new FormulaFormatException("Cannot have an integer right outisde of parentheses.");
                    }
                    else if (i < formulaArray.Length - 1 && Regex.IsMatch(formulaArray[next], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("Cannot have a variable right outisde of parentheses.");
                    }
                    else if (i < formulaArray.Length - 1 && (formulaArray[next] == "+" || formulaArray[next] == "-" || formulaArray[next] == "*" || formulaArray[next] == "/"))
                    {
                        throw new FormulaFormatException("Cannot have a opperator right outisde of parentheses.");
                    }
                    this.formula = this.formula + formulaArray[i];
                }

                else if (formulaArray[i] != "(")
                {
                    int next = determineNext(formulaArray, i);

                    if (!Regex.IsMatch(formulaArray[i], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("The variable entered must have one integer and one character.");
                    }
                    else if (!isValid(normalize(formulaArray[i])))
                    {
                        throw new FormulaFormatException("The variable entered is not valid.");
                    }
                    else if (i < formulaArray.Length - 1 && Int32.TryParse(formulaArray[next], result: out int intResult2))
                    {
                        throw new FormulaFormatException("Cannot have an integer right next to a variable.");
                    }
                    else if (i < formulaArray.Length - 1 && Regex.IsMatch(formulaArray[next], "[a-z|A-Z][0-9]"))
                    {
                        throw new FormulaFormatException("Cannot have two consecutive variables in expression");
                    }

                    ///This builds the variables list and the formula string for later use
                    if (!variables.Contains(normalize(formulaArray[i])))
                    {
                        variables.Add(normalize(formulaArray[i]));
                    }
                    this.formula = this.formula + formulaArray[i];
                }
            }
        }
        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values 
        /// of
        /// variables.  When a variable symbol v needs to be determined, it should be 
        // looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was 
        /// passed to
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts 
        /// all the letters
    /// in a string to upper case:
    /// 
    /// new Formula("x+7", N, s => true).Evaluate(L) is 11
    /// new Formula("x+7").Evaluate(L) is 9
    /// 
    /// Given a variable symbol as its parameter, lookup returns the variable's 
    /// value
/// (if it has one) or throws an ArgumentException (otherwise).
/// 
/// If no undefined variables or divisions by zero are encountered when 
/// evaluating
    /// this Formula, the value is returned.  Otherwise, a FormulaError is 
/// returned.
/// The Reason property of the FormulaError should have a meaningful 
/// explanation.
    ///
    /// This method should never throw an exception.
    /// </summary>
    public object Evaluate(Func<string, double> lookup)
        {
            ArrayList expression = (ArrayList)GetTokens(formula);
            Stack<int> ValueStack = new System.Collections.Generic.Stack<int>();
            Stack<string> OperatorStack = new System.Collections.Generic.Stack<string>();
            foreach (string token in expression)
            {
                ///This if statement sets up the operator stack for later use. Only the "(", "*" and "/"
                ///operators should be added to the stack
                if (token == "(" || token == "*" || token == "/")
                {
                    OperatorStack.Push(token);
                }
                ///This if statement determines if the string being worked with is an integer. If it is,
                ///then checks how the integer should be treated based on the operators being used.
                else if (Int32.TryParse(token, result: out int intResult))
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                    {
                        if (OperatorStack.Peek() == "*")
                        {
                            OperatorStack.Pop();
                            ValueStack.Push(ValueStack.Pop() * intResult);
                        }
                        else
                        {
                            OperatorStack.Pop();
                            ValueStack.Push(ValueStack.Pop() / intResult);
                        }
                    }
                    else
                    {
                        ValueStack.Push(intResult);
                    }
                }

                ///Repeats the process of the previous if statement, except with variables. A
                ///delegate is used to look up the value of a variable so that it can be evaluated.
                else if (token != ")" && token != "-" && token != "+")
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack != null && OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                    {
                        if (OperatorStack.Peek() == "*")
                        {
                            OperatorStack.Pop();
                            ValueStack.Push(ValueStack.Pop() * (int)lookup(token));
                        }
                        else
                        {

                            OperatorStack.Pop();
                            ValueStack.Push(ValueStack.Pop() / (int)lookup(token));
                        }
                    }
                    else
                    {
                        ValueStack.Push((int)lookup(token));
                    }
                }

                ///This if statement works with the "+" and "-" operators. If there is already
                ///a "+" or "-" at the top of the operator stack, then two integers from the
                ///value stack are evaluated using one of the operators. The operator then gets
                ///pushed onto the operator stack.
                if (token == "+" || token == "-")
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                    {
                        if (OperatorStack.Peek() == "+")
                        {
                            ValueStack.Push(ValueStack.Pop() + ValueStack.Pop());
                        }
                        else
                        {
                            ValueStack.Push(0 - ValueStack.Pop() + ValueStack.Pop());
                        }
                        OperatorStack.Pop();
                    }
                    OperatorStack.Push(token);
                }

                ///This if statement works with right parentheses. It evaluates the integers in the
                ///value stack depending on the operator at the top of the operator stack.
                if (token == ")")
                {
                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                    {
                        if (OperatorStack.Peek() == "+")
                        {
                            ValueStack.Push(ValueStack.Pop() + ValueStack.Pop());
                        }
                        else
                        {
                            ValueStack.Push(0 - ValueStack.Pop() + ValueStack.Pop());
                        }
                        OperatorStack.Pop();
                    }

                    OperatorStack.Pop();

                    if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                    {
                        if (OperatorStack.Peek() == "*")
                        {
                            ValueStack.Push(ValueStack.Pop() * ValueStack.Pop());
                        }
                        else
                        {
                            int val1 = ValueStack.Pop();
                            int val2 = ValueStack.Pop();
                            ValueStack.Push(val2 / val1);
                        }
                        OperatorStack.Pop();
                    }
                }
            }

            int result = 0;

            ///If there is a remaining expression that didn't get resolved while the for loop
            ///was running, this is where this expression gets handled.
            if (ValueStack.Count() > 1)
            {
                if (OperatorStack.Peek() == "*")
                {
                    result = ValueStack.Pop() * ValueStack.Pop();
                }
                if (OperatorStack.Peek() == "/")
                {
                    int val1 = ValueStack.Pop();
                    int val2 = ValueStack.Pop();
                    result = val2 / val1;
                }
                if (OperatorStack.Peek() == "+")
                {
                    result = ValueStack.Pop() + ValueStack.Pop();
                }
                if (OperatorStack.Peek() == "-")
                {
                    result = 0 - ValueStack.Pop() + ValueStack.Pop();
                }
            }
            else
            {
                result = ValueStack.Pop();
            }

            return result;
        }
    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in 
    /// this 
    /// formula.  No normalization may appear more than once in the enumeration, 
    /// even
/// if it appears more than once in this Formula.
/// 
/// For example, if N is a method that converts all the letters in a string to 
/// upper case:
    /// 
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", 
/// "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and 
/// "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<String> GetVariables()
        {
            return (IEnumerable<String>)variables;
        }
        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to 
        ///upper case:
    /// 
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
        {
            return formula;
        }
        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as 
        ///strings 
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being 
    ///"normalized" 
    /// by C#'s standard conversion from string to double, then back to string. 
    ///This 
    /// eliminates any inconsistencies due to limited floating point precision.
    /// Variable tokens are considered equal if their normalized forms are equal, 
    ///as 
    /// defined by the provided normalizer.
    /// 
    /// For example, if N is a method that converts all the letters in a string to 
    //upper case:
    ///  
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
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
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 
    ///can be null!</change>
    /// Reports whether f1 == f2, using the notion of equality from the Equals 
    ///method.
    /// 
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
            {
                return true;
            }
            return false;
        }
    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 
    ///can be null!</change>
    ///   <change> Note: != should almost always be not ==, if you get my meaning 
    ///</change>
    ///   Reports whether f1 != f2, using the notion of equality from the Equals 
    ///method.
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be 
        ///the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability 
        /// that two
        /// randomly-generated unequal Formulae have the same hash code should be 
        /// extremely small.
    /// </summary>
    public override int GetHashCode()
        {
            return formula.Length%31;
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
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) |  ({ 5})",
            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);
            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern,
      RegexOptions.IgnorePatternWhitespace))
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
// <change>
//   If you are using Extension methods to deal with common stack operations (e.g.,
//checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is
//"biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) 
//where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can 
//contain any object
//   as long as it doesn't allow nulls!
// </change>

// jim was here.