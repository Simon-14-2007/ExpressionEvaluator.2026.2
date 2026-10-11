using System.Globalization;

namespace Backend;

public static class ExpressionEvaluator
{
    public static double Evalute(string infix) => EvalutePostfix(ToPostfix(infix));

    // Converts the infix expression to postfix using an operator stack.
    // The result is a list of tokens: complete numbers (e.g. "144", "3.1416") or operators.
    private static List<string> ToPostfix(string infix)
    {
        var postfix = new List<string>();
        var stack = new Stack<char>();
        var number = string.Empty; // accumulates the digits and the decimal point of the number being read

        foreach (var item in infix)
        {
            if (char.IsWhiteSpace(item))
            {
                continue;
            }

            // Part of a number: keep accumulating until an operator is found
            if (char.IsDigit(item) || item == '.')
            {
                number += item;
                continue;
            }

            // An operator was found: if there was an accumulated number, it is now complete
            if (number.Length > 0)
            {
                postfix.Add(number);
                number = string.Empty;
            }

            if (!IsOperator(item))
            {
                throw new Exception("Invalid expression.");
            }

            if (item == ')')
            {
                // Pop operators until the matching '(' is found
                while (stack.Count > 0 && stack.Peek() != '(')
                {
                    postfix.Add(stack.Pop().ToString());
                }

                if (stack.Count == 0)
                {
                    throw new Exception("Invalid expression."); // ')' without '('
                }

                stack.Pop(); // discard the '('
            }
            else
            {
                // Pop ALL operators from the stack with greater or equal priority
                while (stack.Count > 0 && PriorityStack(stack.Peek()) >= PriorityInfix(item))
                {
                    postfix.Add(stack.Pop().ToString());
                }

                stack.Push(item);
            }
        }

        // If the expression ended with a number, add it
        if (number.Length > 0)
        {
            postfix.Add(number);
        }

        // Empty the stack
        while (stack.Count > 0)
        {
            var ope = stack.Pop();
            if (ope == '(')
            {
                throw new Exception("Invalid expression."); // '(' without ')'
            }

            postfix.Add(ope.ToString());
        }

        return postfix;
    }

    private static int PriorityStack(char op) => op switch
    {
        '^' => 3,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid expression."),
    };

    private static int PriorityInfix(char op) => op switch
    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid expression."),
    };

    private static bool IsOperator(char item) => item == '^' || item == '*' || item == '/' || item == '+' || item == '-' || item == '(' || item == ')';

    // Evaluates the list of postfix tokens using a stack of numbers.
    private static double EvalutePostfix(List<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (var token in postfix)
        {
            if (token.Length == 1 && IsOperator(token[0]))
            {
                if (stack.Count < 2)
                {
                    throw new Exception("Invalid expression.");
                }

                var ope2 = stack.Pop();
                var ope1 = stack.Pop();
                stack.Push(Calculate(ope1, ope2, token[0]));
            }
            else
            {
                // InvariantCulture: the dot is always the decimal separator,
                // regardless of the Windows regional settings.
                if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    throw new Exception("Invalid expression.");
                }

                stack.Push(value);
            }
        }

        if (stack.Count != 1)
        {
            throw new Exception("Invalid expression.");
        }

        return stack.Pop();
    }

    private static double Calculate(double ope1, double ope2, char item) => item switch
    {
        '*' => ope1 * ope2,
        '/' => ope1 / ope2,
        '+' => ope1 + ope2,
        '-' => ope1 - ope2,
        '^' => Math.Pow(ope1, ope2),
        _ => throw new Exception("Invalid expression."),
    };
}