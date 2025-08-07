using Glacier.Core.Interpreter.Instructions.Concrete.Return;
using Glacier.Core.Interpreter.Values;

namespace Glacier.Core.Interpreter.Instructions.Concrete;

public record Call(IGlacierInstruction Source, List<IGlacierInstruction> Args) : IGlacierInstruction
{
    public IGlacierObject Exec(ExecutionContext ctx)
    {
        if (Source.Exec(ctx) is not FunctionObject targetFn)
            throw new InvalidOperationException("Cant invoke non-functions!");
        
        ctx.PushFrame();
        try
        {
            return targetFn.Execute(ctx, Args);
        }
        catch (GlacierFuncReturnException retEx)
        {
            return retEx.Value;
        }
        finally
        {
            ctx.PopFrame();
        }
    }

    public override string ToString()
    {
        return $"Call {{ Source = {Source}, Args = {"[" + String.Join(",", Args) + "]"} }}";
    }
}