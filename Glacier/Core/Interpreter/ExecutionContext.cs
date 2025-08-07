using     Glacier.Core.Interpreter.Stack;
namespace Glacier.Core.Interpreter;

public class ExecutionContext
{
    public readonly StackFrame Root = new(null);
    public StackFrame CurrStackFrame;

    
    //Values
    public void PushVar(string name, IGlacierObject obj) 
        => CurrStackFrame.PushVar(name, obj);

    public IGlacierObject GetVar(string name) 
        => CurrStackFrame.GetVar(name);
    
    public T GetVar<T>(string name)
    where T : IGlacierObject
        => (T)CurrStackFrame.GetVar(name);

    //Stack frame stuff
    public void PushFrame() 
        => CurrStackFrame = new StackFrame(CurrStackFrame);

    public void PopFrame() 
        => CurrStackFrame = CurrStackFrame.Parent ?? throw new InvalidOperationException("PopFrame() on exhausted stack");


    public ExecutionContext()
    {
        CurrStackFrame = Root;
    }

    public override string ToString()
    {
        List<StackFrame> frames = [];
        var tgtFrame = CurrStackFrame;
        while (tgtFrame is not null)
        {
            frames.Add(tgtFrame);
            tgtFrame = tgtFrame.Parent;
        }
        return string.Join("\n", frames.AsEnumerable().Reverse());
    }

    public void DebugPrint()
    {
        Console.WriteLine(ToString());
        Console.WriteLine("#################################################");
    }
}