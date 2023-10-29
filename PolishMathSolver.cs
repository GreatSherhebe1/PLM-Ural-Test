using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;

namespace PLM_Ural_Test;

public class PolishMathSolver
{
    private Dictionary<string, Operation> operations;
    private List<string> availableOperationSymbols;

    private List<string> tokens;
    private Stack<double> valueStack = new Stack<double>();
    private Stack<string> operationStack  = new Stack<string>();

    private bool isPreviousSignOpenBracket;

    public double SolveMathExp(string income)
    {
        if (!AreBracketsCorrect(income))
            throw new Exception("Incorrect placement of brackets: closed != opened");
        
        InitializeAvailableOperations();
        
        tokens = ExtractTokens(income);
        
        return Solve();
    }

    private void InitializeAvailableOperations()
    {
        operations = new Dictionary<string, Operation>
        {
            {"^", new Operation(3, (v1, v2) => Math.Pow(v1, v2))},
            {"*", new Operation(2, (v1, v2) => v1 * v2)},
            {"/", new Operation(2, (v1, v2) =>
            {
                if (v2 == 0)
                    throw new Exception("Try to divide by zero : {v1} / {v2}");
                return v1 / v2;
            })},
            {"+", new Operation(1, (v1, v2) => v1 + v2)},
            {"-", new Operation(1, (v1, v2) => v1 - v2)}
        };

        availableOperationSymbols = new List<string>() { "(", ")" };
        availableOperationSymbols.AddRange(operations.Keys);
    }
    
    private bool AreBracketsCorrect(string income)
    {
        var stack = new Stack<char>();
        foreach (var symbol in income)
        {
            if (symbol == '(')
                stack.Push(symbol);

            if (symbol == ')')
            {
                if (stack.Count == 0 || stack.Pop() != '(')
                    return false;
            }
        }

        return stack.Count == 0;
    }

    private List<string> ExtractTokens(string income)
    {
        var cleanIncome = income.Replace(" ", "");
        var result = new List<string>();

        var startIndex = 0;
        double numItem;
        for (var i = 0; i < cleanIncome.Length; i++)
        {
            if (IsAvailableSymbol(cleanIncome[i].ToString()))
            {
                if (double.TryParse(cleanIncome.Substring(startIndex, i - startIndex), out numItem))
                    result.Add(numItem.ToString());

                result.Add(cleanIncome[i].ToString());
                startIndex = i + 1;
            }
        }
        
        if(startIndex != cleanIncome.Length)
            result.Add(cleanIncome.Substring(startIndex, cleanIncome.Length - startIndex));

        return result;
    }

    private double Solve()
    {
        double numToken;
        var opToken = string.Empty;
        
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out numToken))
            {
                // putting minus operation in value to rid of "henging" operations
                if (operationStack.Count > 0 && operationStack.Peek() == "-")
                    numToken = HandleMinusOperation(numToken);

                isPreviousSignOpenBracket = false;
                valueStack.Push(numToken);
                continue;
            }

            if (IsAvailableSymbol(token))
            {
                HandleAvailableToken(token);
                continue;
            }

            throw new ArgumentException($"unidentified operation \"{token}\"");
        }

        HandleLastOperationsAfterTokensEnd(opToken);

        valueStack.TryPop(out numToken);
        return numToken;
    }

    private void HandleAvailableToken(string token)
    {
        if (operationStack.Any())
        {
            if (token == ")")
            {
                HandleCloseBracket();
                return;
            }

            if (token == "(" || operationStack.Peek() == "(")
            {
                if (token == "(")
                    isPreviousSignOpenBracket = true;
                operationStack.Push(token);
                return;
            }

            if (IsOperationPriorityGreaterOrEqualOperationInStack(token))
            {
                CheckOperationPossibility();
                HandleGreaterOrEqualOperation(token);
                return;
            }

            operationStack.Push(token);
            isPreviousSignOpenBracket = false;
            
        }

        else
        {
            if (token == "(")
                isPreviousSignOpenBracket = true;

            operationStack.Push(token);
        }
    }

    private double HandleMinusOperation(double numToken)
    {
        numToken *= -1;
        operationStack.Pop();

        if (valueStack.Count == 0
            || isPreviousSignOpenBracket)
        {
            isPreviousSignOpenBracket = false;
            return numToken;
        }

        operationStack.Push("+");
        return numToken;
    }

    private void HandleLastOperationsAfterTokensEnd(string opToken)
    {
        if (operationStack.Any() && valueStack.Any())
        {
            while (operationStack.TryPop(out opToken))
                Operate(opToken);
        }
    }

    private void Operate(string operation)
    {
        var secondValue = valueStack.Pop();
        var firstValue = valueStack.Pop();
        
        valueStack.Push(operations[operation].Operate(firstValue, secondValue));
    }

    private void HandleCloseBracket()
    {
        while (operationStack.Peek() != "(")
        {
            CheckOperationPossibility();
            Operate(operationStack.Pop());
        }

        operationStack.Pop();
    }

    private void HandleGreaterOrEqualOperation(string token)
    {
        Operate(operationStack.Pop());
        operationStack.Push(token);
    }

    private bool IsAvailableSymbol(string symbol)
    {
        return availableOperationSymbols.Contains(symbol);
    }

    private bool IsOperationPriorityGreaterOrEqualOperationInStack(string token)
    {
        return operations[operationStack.Peek()].Priority >= operations[token].Priority;
    }

    private void CheckOperationPossibility()
    {
        if (!IsPossibleOperate())
            throw new Exception("Impossible to operate without values");
    }

    private bool IsPossibleOperate()
    {
        return valueStack.Count >= 2;
    }
}