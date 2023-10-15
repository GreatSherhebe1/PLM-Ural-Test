namespace PLM_Ural_Test;

public class Operation
{
    public int Priority { get; private set; }
    
    public Func<double, double, double> Operate { get; private set; }

    public Operation(int priority, Func<double, double, double> makeOperation)
    {
        Priority = priority;
        Operate = makeOperation;
    }
}