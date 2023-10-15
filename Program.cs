// See https://aka.ms/new-console-template for more information

using PLM_Ural_Test;

Console.WriteLine("Enter your expression:");
var income = Console.ReadLine();

Console.WriteLine(new PolishMathSolver().SolveMathExp(income));