namespace Glacier.Core.Interpreter.Values;

public record None : IGlacierObject
{
    private None() { }
    
    public static readonly None Instance = new();
    public override string ToString()
        => "None";
}