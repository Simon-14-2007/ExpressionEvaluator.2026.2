using System.Globalization;

namespace Backend;

public static class ExpressionEvaluator
{
    public static double Evalute(string infix) => EvalutePostfix(ToPostfix(infix));

    // Convierte la expresión infija a postfija usando una pila de operadores.
    // El resultado es una lista de tokens: números completos (ej. "144", "3.1416") u operadores.
    private static List<string> ToPostfix(string infix)
    {
        var postfix = new List<string>();
        var stack = new Stack<char>();
        var number = string.Empty; // acumula los dígitos y el punto del número que se está leyendo

        foreach (var item in infix)
        {
            if (char.IsWhiteSpace(item))
            {
                continue;
            }

            // Parte de un número: se acumula hasta encontrar un operador
            if (char.IsDigit(item) || item == '.')
            {
                number += item;
                continue;
            }

            // Llegó un operador: si había un número acumulado, ya está completo
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
                // Se sacan operadores hasta encontrar el '(' correspondiente
                while (stack.Count > 0 && stack.Peek() != '(')
                {
                    postfix.Add(stack.Pop().ToString());
                }

                if (stack.Count == 0)
                {
                    throw new Exception("Invalid expression."); // ')' sin '('
                }

                stack.Pop(); // descarta el '('
            }
            else
            {
                // Se sacan TODOS los operadores de la pila con prioridad mayor o igual
                while (stack.Count > 0 && PriorityStack(stack.Peek()) >= PriorityInfix(item))
                {
                    postfix.Add(stack.Pop().ToString());
                }

                stack.Push(item);
            }
        }

        // Si la expresión terminó con un número, se agrega
        if (number.Length > 0)
        {
            postfix.Add(number);
        }

        // Se vacía la pila
        while (stack.Count > 0)
        {
            var ope = stack.Pop();
            if (ope == '(')
            {
                throw new Exception("Invalid expression."); // '(' sin ')'
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

    // Evalúa la lista de tokens postfijos usando una pila de números.
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
                // InvariantCulture: el punto siempre es el separador decimal,
                // sin importar la configuración regional de Windows.
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