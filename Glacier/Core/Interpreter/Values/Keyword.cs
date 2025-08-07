namespace Glacier.Core.Interpreter.Values;

public readonly record struct Keyword(string Value) : IGlacierObject
{
    public override string ToString() 
        => Value;
}