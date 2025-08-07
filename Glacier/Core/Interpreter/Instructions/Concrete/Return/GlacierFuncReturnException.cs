namespace Glacier.Core.Interpreter.Instructions.Concrete.Return;

public class GlacierFuncReturnException(IGlacierObject Obj) : Exception
{
    public IGlacierObject Value => Obj;
}