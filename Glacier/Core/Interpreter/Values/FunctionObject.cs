using Glacier.Core.Interpreter.Instructions;

namespace Glacier.Core.Interpreter.Values;

public record FunctionObject(FunctionMetadata Metadata) : IGlacierObject
{
    private Dictionary<string, IGlacierObject> _closure = [];

    public void MakeClosure(ExecutionContext ctx)
    {
        foreach (var capturedVar in Metadata.CaptureGroup)
        {
            _closure[capturedVar] = ctx.GetVar(capturedVar);
        }
    }

    public IGlacierObject Execute(ExecutionContext ctx, IEnumerable<IGlacierInstruction> passedArgs)
    {
        foreach (var (varName, varValue) in _closure)
            ctx.PushVar(varName, varValue);

        
        //PROBLEM Maybe explicit Clone() instead of pass-by-ref?
        using (var e = passedArgs.GetEnumerator())
        {
            e.MoveNext();
            foreach (var paramName in Metadata.ArgsNames)
            {
                ctx.PushVar(paramName, e.Current.Exec(ctx));
                e.MoveNext();
            }
        }

        var res = Metadata.Body?.Exec(ctx);
        
        return res ?? None.Instance;
    }
}

public sealed record FunctionMetadata
{
    public  string Name { get; private set; } = "UNDEFINED";
    public  IGlacierInstruction? Body { get; private set; } = null;

    private List<string> _captureGroup  = [];
    public  IReadOnlyList<string>   CaptureGroup  =>_captureGroup;

    private List<string> _argsNames = [];
    public IReadOnlyList<string>   ArgsNames => _argsNames;
        
    
    public struct FunctionMetadataBuilder
    {
        private FunctionMetadata _res = new();

        public FunctionMetadataBuilder()
        {
            
        }

        public FunctionMetadataBuilder WithCaptureGroup(params List<string> captureGroup)
        {
            _res._captureGroup = captureGroup;
            return this;
        }

        public FunctionMetadataBuilder WithParamNames(params List<string> paramNames)
        {
            _res._argsNames = paramNames;
            return this;
        }

        public FunctionMetadataBuilder WithBody(IGlacierInstruction body)
        {
            _res.Body = body;
            return this;
        }
        
        public FunctionMetadataBuilder WithName(string name)
        {
            _res.Name = name;
            return this;
        }
        
        public FunctionMetadata Build()
            => _res;
    }

    public override string ToString()
    {
        return $"FunctionMetadata {{ {nameof(CaptureGroup)} = [{string.Join(",", _captureGroup)}], {nameof(ArgsNames)} = [{String.Join(",", ArgsNames)}] }}";
    }
}