using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator;
///<header>
///Sasha Rybalkina
///Version: January 20, 2023
///</header>
public static class Evaluator
{
    public delegate int Lookup(String variable_name);
    /// <summary>
    /// Uses stacks to separate the operators in the equation from
    /// the integers and variables, and uses a delegate to find the
    /// value of a given variable in the equation.
    /// </summary>
    /// <param name="expression"> The string expression to be evaluated </param>
    /// <param name="variableEvaluator"> Evaluates variables </param>
    /// <returns> The evaluation of the expression </returns>
    public static int Evaluate(string expression, Lookup variableEvaluator)
    {
        expression = expression.Trim();
        if (expression == null || expression == "")
        {
            throw new ArgumentException();
        }
        Stack<int> ValueStack = new System.Collections.Generic.Stack<int>();
        Stack<string> OperatorStack = new System.Collections.Generic.Stack<string>();

        var left = expression.Count(x => x == '(');
        var right = expression.Count(x => x == ')');
        string[] expressionArray = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
        if (left != right) throw new ArgumentException();

        for (int i = 0; i < expressionArray.Length; i++)
        {
            ///For some reason there are whitespace tokens in the array, and so this if statement
            ///helps with skipping over those tokens.
            if (expressionArray[i] == "")
            {
                i++;
                if (i == expressionArray.Length)
                {
                    break;
                }
            }

            ///This if statement sets up the operator stack for later use. Only the "(", "*" and "/"
            ///operators should be added to the stack
            if (expressionArray[i] == "(" || expressionArray[i] == "*" || expressionArray[i] == "/")
            {
                OperatorStack.Push(expressionArray[i]);
            }
            ///This if statement determines if the string being worked with is an integer. If it is,
            ///then checks how the integer should be treated based on the operators being used.
            else if (Int32.TryParse(expressionArray[i], result: out int intResult))
            {
                if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                {
                    try
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
                    catch
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    ValueStack.Push(intResult);
                }
            }

            ///Repeats the process of the previous if statement, except with variables. A
            ///delegate is used to look up the value of a variable so that it can be evaluated.
            else if (expressionArray[i] != ")" && expressionArray[i] != "-" && expressionArray[i] != "+")
            {
                if (!Regex.IsMatch(expressionArray[i], "[a-z|A-Z][0-9]"))
                {
                    throw new ArgumentException();
                }
                if (OperatorStack.Count() != 0 && (OperatorStack != null && OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                {
                    try
                    {
                        if (OperatorStack.Peek() == "*")
                        {
                            OperatorStack.Pop();
                            ValueStack.Push(ValueStack.Pop() * variableEvaluator(expressionArray[i]));
                        }
                        else
                        {

                            OperatorStack.Pop();
                            ValueStack.Push(ValueStack.Pop() / variableEvaluator(expressionArray[i]));
                        }
                    }
                    catch
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    ValueStack.Push(variableEvaluator(expressionArray[i]));
                }
            }

            ///This if statement works with the "+" and "-" operators. If there is already
            ///a "+" or "-" at the top of the operator stack, then two integers from the
            ///value stack are evaluated using one of the operators. The operator then gets
            ///pushed onto the operator stack.
            if (expressionArray[i] == "+" || expressionArray[i] == "-")
            {
                if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                {
                    try
                    {
                        if (OperatorStack.Peek() == "+")
                        {
                            ValueStack.Push(ValueStack.Pop() + ValueStack.Pop());
                        }
                        else
                        {
                            ValueStack.Push(0 - ValueStack.Pop() + ValueStack.Pop());
                        }
                    }
                    catch
                    {
                        throw new ArgumentException();
                    }
                    OperatorStack.Pop();
                }
                OperatorStack.Push(expressionArray[i]);
            }

            ///This if statement works with right parentheses. It evaluates the integers in the
            ///value stack depending on the operator at the top of the operator stack.
            if (expressionArray[i] == ")")
            {
                if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "+" || OperatorStack.Peek() == "-"))
                {
                    try
                    {
                        if (OperatorStack.Peek() == "+")
                        {
                            ValueStack.Push(ValueStack.Pop() + ValueStack.Pop());
                        }
                        else
                        {
                            ValueStack.Push(0 - ValueStack.Pop() + ValueStack.Pop());
                        }
                    }
                    catch
                    {
                        throw new ArgumentException();
                    }
                    OperatorStack.Pop();
                }

                OperatorStack.Pop();

                if (OperatorStack.Count() != 0 && (OperatorStack.Peek() == "*" || OperatorStack.Peek() == "/"))
                {
                    try
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
                    }
                    catch
                    {
                        throw new ArgumentException();
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
            try
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
            catch
            {
                throw new ArgumentException();
            }
        }
        else
        {
            if (OperatorStack.Count != 0)
            {
                throw new ArgumentException();
            }
            result = ValueStack.Pop();
        }

        ///Both of the stacks should be empty by this point, and if they aren't, that is
        ///how we know the expression is invalid. This if statement throws an exception in
        ///such a case.
        if (ValueStack.Count() != 0)
        {
            throw new ArgumentException();
        }

        return result;
    }
}