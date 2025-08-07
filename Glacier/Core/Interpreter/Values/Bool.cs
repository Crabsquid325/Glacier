using System.Numerics;

namespace Glacier.Core.Interpreter.Values;

public readonly record struct Bool(bool Value) : IGlacierObject
{
    public override string ToString() 
        => Value.ToString();
}