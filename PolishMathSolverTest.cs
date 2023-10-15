using NUnit.Framework;

namespace PLM_Ural_Test;

[TestFixture]
public class PolishMathSolverTest
{
    [TestCase("(")]
    [TestCase(")")]
    [TestCase(")(")]
    [TestCase("(()")]
    public void FailsWhenBracketsUncorrect(string income)
    {
        Assert.Catch(() => new PolishMathSolver().SolveMathExp(income));
    }

    [TestCase("2 + 2", 4)]
    [TestCase("5 - 3", 2)]
    [TestCase("2 ^ 4", 16)]
    [TestCase("2 * 3", 6)]
    [TestCase("15 / 3", 5)]
    public void DoesBasicOperationsCorrect(string income, double expected)
    {
        Assert.AreEqual(expected, new PolishMathSolver().SolveMathExp(income));
    }

    [TestCase("2 + (5 - 6)", 1)]
    [TestCase("(5 - 6)", -1)]
    [TestCase("(5 - 6) + 8", 7)]
    [TestCase("11 - 5 * (11 + 1 + (1 + 2)) + 2", -62)]
    [TestCase("(((1 - 5)))", -4)]
    public void DoesOperationsWithBracketsCorrect(string income, double expected)
    {
        Assert.AreEqual(expected, new PolishMathSolver().SolveMathExp(income));
    }

    [Test]
    public void FailsWhenTryToDivideByZero()
    {
        Assert.Catch(() => new PolishMathSolver().SolveMathExp("1 / 0"));
    }

    [TestCase("1       +      6", 7)]
    [TestCase("1-     10", -9)]
    [TestCase("1           +2", 3)]
    [TestCase("1-2*3", -5)]
    public void WorkWithOrWithoutWhiteSpaces(string income, double expected)
    {
        Assert.AreEqual(expected, new PolishMathSolver().SolveMathExp(income));
    }

    [TestCase("1", 1)]
    [TestCase("-1", -1)]
    [TestCase("0", 0)]
    public void WorkWithSingleNumber(string income, double expected)
    {
        Assert.AreEqual(expected, new PolishMathSolver().SolveMathExp(income));
    }
    
    [TestCase("***")]
    [TestCase("(-)")]
    [TestCase("--")]
    public void FailsWithoutNumbers(string income)
    {
        Assert.Catch(() => new PolishMathSolver().SolveMathExp(income));
    }
}