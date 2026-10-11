using Backend;

var infix = "4*5/(4+6)";
Console.WriteLine($"Infix = {infix}, Result = {ExpressionEvaluator.Evalute(infix):N5}"); // 2

var infix2 = "4*(5+6-(8/2^3)-7)-1";
Console.WriteLine($"Infix = {infix2}, Result = {ExpressionEvaluator.Evalute(infix2):N5}"); // 11

var infix3 = "4*7^(1/3)*7*((1+9)/3*7^4)";
Console.WriteLine($"Infix = {infix3}, Result = {ExpressionEvaluator.Evalute(infix3):N5}"); // 428,675.12518474100 

var infix4 = "144^(1/2)";
Console.WriteLine($"Infix = {infix4}, Result = {ExpressionEvaluator.Evalute(infix4):N5}"); // 12

var infix5 = "(3.1416+70)/(300^(1/3.2))";
Console.WriteLine($"Infix = {infix5}, Result = {ExpressionEvaluator.Evalute(infix5)}"); // 12.304502656266802

var infix6 = "1-2*3+4";
Console.WriteLine($"Infix = {infix6}, Result = {ExpressionEvaluator.Evalute(infix6)}"); // -1

var infix7 = "12.5*10+100/4";
Console.WriteLine($"Infix = {infix7}, Result = {ExpressionEvaluator.Evalute(infix7)}"); // 150