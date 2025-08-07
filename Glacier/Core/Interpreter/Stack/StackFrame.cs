using Glacier.Core.Interpreter.Exceptions;

namespace Glacier.Core.Interpreter.Stack;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    where TValue : new()
    {
        if (dict.TryGetValue(key, out var res))
            return res;
        res = new TValue();
        dict.Add(key, res);
        return res;
    }
}
public class StackFrame(StackFrame? parent)
{
    public readonly StackFrame? Parent = parent;
    public readonly Dictionary<string, List<IGlacierObject>> Locals = [];


    public void PushVar(string name, IGlacierObject obj) 
        => Locals.GetOrAdd(name).Add(obj);

    /// <exception cref="StackExhaustedException">Target variable isnt present in the execution context</exception>
    public IGlacierObject GetVar(string name)
    {
        if (Locals.TryGetValue(name, out var res))
            return res.Last();
        
        if (Parent is null)
            throw new StackExhaustedException();
        
        return Parent.GetVar(name);
    }

    public override string ToString()
    {
        var items = Locals.Select(x => 
            $"{x.Key}: {String.Join(" -> ", x.Value.AsEnumerable().Reverse().ToArray<object>())}");
        return $"[{string.Join(", ", items)}]";
    }
}