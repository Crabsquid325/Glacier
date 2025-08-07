using System.Numerics;

namespace Glacier.Core.Interpreter.Values;

public readonly record struct Number(BigInteger Value) : IGlacierObject
{
    public override string ToString() 
        => Value.ToString();
}